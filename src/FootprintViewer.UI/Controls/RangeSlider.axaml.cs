using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Utilities;

namespace FootprintViewer.UI.Controls;

[PseudoClasses(":vertical", ":horizontal", ":pressed")]
public class RangeSlider : RangeBase
{
    private enum TrackThumb
    {
        None,
        Upper,
        InnerUpper,
        OuterUpper,
        Lower,
        InnerLower,
        OuterLower,
        Both,
        Overlapped
    };

    public static readonly StyledProperty<Orientation> OrientationProperty =
        ScrollBar.OrientationProperty.AddOwner<RangeSlider>();

    public static readonly StyledProperty<bool> IsDirectionReversedProperty =
        RangeTrack.IsDirectionReversedProperty.AddOwner<RangeSlider>();

    public static readonly StyledProperty<bool> IsThumbOverlapProperty =
        RangeTrack.IsThumbOverlapProperty.AddOwner<RangeSlider>();

    public static readonly StyledProperty<bool> IsSnapToTickEnabledProperty =
        AvaloniaProperty.Register<RangeSlider, bool>(nameof(IsSnapToTickEnabled), false);

    public static readonly StyledProperty<bool> MoveWholeRangeProperty =
        AvaloniaProperty.Register<RangeSlider, bool>(nameof(MoveWholeRange), false);

    public static readonly StyledProperty<double> TickFrequencyProperty =
        AvaloniaProperty.Register<RangeSlider, double>(nameof(TickFrequency), 0.0);

    public static readonly StyledProperty<TickPlacement> TickPlacementProperty =
        AvaloniaProperty.Register<TickBar, TickPlacement>(nameof(TickPlacement), 0d);

    public static readonly StyledProperty<AvaloniaList<double>> TicksProperty =
        TickBar.TicksProperty.AddOwner<RangeSlider>();

    private double _previousValue = 0.0;
    private bool _isDragging = false;
    private RangeTrack _track = null!;
    private TrackThumb _currentTrackThumb = TrackThumb.None;
    private const double Tolerance = 0.0001;

    static RangeSlider()
    {
        PressedMixin.Attach<RangeSlider>();
        FocusableProperty.OverrideDefaultValue<RangeSlider>(true);
        OrientationProperty.OverrideDefaultValue(typeof(RangeSlider), Orientation.Horizontal);

        LowerSelectedValueProperty.OverrideMetadata<RangeSlider>(new DirectPropertyMetadata<double>(enableDataValidation: true));
        UpperSelectedValueProperty.OverrideMetadata<RangeSlider>(new DirectPropertyMetadata<double>(enableDataValidation: true));
    }

    public RangeSlider()
    {
        UpdatePseudoClasses(Orientation);
    }

    public AvaloniaList<double> Ticks
    {
        get => GetValue(TicksProperty);
        set => SetValue(TicksProperty, value);
    }

    public Orientation Orientation
    {
        get { return GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    public bool MoveWholeRange
    {
        get { return GetValue(MoveWholeRangeProperty); }
        set { SetValue(MoveWholeRangeProperty, value); }
    }

    public bool IsDirectionReversed
    {
        get { return GetValue(IsDirectionReversedProperty); }
        set { SetValue(IsDirectionReversedProperty, value); }
    }

    public bool IsSnapToTickEnabled
    {
        get { return GetValue(IsSnapToTickEnabledProperty); }
        set { SetValue(IsSnapToTickEnabledProperty, value); }
    }

    public bool IsThumbOverlap
    {
        get { return GetValue(IsThumbOverlapProperty); }
        set { SetValue(IsThumbOverlapProperty, value); }
    }

    public double TickFrequency
    {
        get { return GetValue(TickFrequencyProperty); }
        set { SetValue(TickFrequencyProperty, value); }
    }

    public TickPlacement TickPlacement
    {
        get { return GetValue(TickPlacementProperty); }
        set { SetValue(TickPlacementProperty, value); }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _track = e.NameScope.Find<RangeTrack>("PART_Track");

        AddHandler(PointerPressedEvent, TrackPressed, RoutingStrategies.Tunnel);
        AddHandler(PointerMovedEvent, TrackMoved, RoutingStrategies.Tunnel);
        AddHandler(PointerReleasedEvent, TrackReleased, RoutingStrategies.Tunnel);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Handled || e.KeyModifiers != KeyModifiers.None)
        {
            return;
        }

        var handled = true;

        switch (e.Key)
        {
            case Key.Down:
            case Key.Left:
                MoveToNextTick(IsDirectionReversed ? SmallChange : -SmallChange);
                break;

            case Key.Up:
            case Key.Right:
                MoveToNextTick(IsDirectionReversed ? -SmallChange : SmallChange);
                break;

            case Key.PageUp:
                MoveToNextTick(IsDirectionReversed ? -LargeChange : LargeChange);
                break;

            case Key.PageDown:
                MoveToNextTick(IsDirectionReversed ? LargeChange : -LargeChange);
                break;

            case Key.Home:
                LowerSelectedValue = Minimum;
                break;

            case Key.End:
                UpperSelectedValue = Maximum;
                break;

            default:
                handled = false;
                break;
        }

        e.Handled = handled;
    }

    private void MoveToNextTick(double direction)
    {
        if (direction == 0.0)
        {
            return;
        }

        var value = LowerSelectedValue;

        var next = SnapToTick(Math.Max(Minimum, Math.Min(Maximum, value + direction)));

        var greaterThan = direction > 0;

        if (Math.Abs(next - value) < Tolerance
            && !(greaterThan && Math.Abs(value - Maximum) < Tolerance)
            && !(!greaterThan && Math.Abs(value - Minimum) < Tolerance))
        {
            var ticks = Ticks;

            if (ticks != null && ticks.Count > 0)
            {
                foreach (var tick in ticks)
                {
                    if (greaterThan && MathUtilities.GreaterThan(tick, value) &&
                        (MathUtilities.LessThan(tick, next) || Math.Abs(next - value) < Tolerance)
                        || !greaterThan && MathUtilities.LessThan(tick, value) &&
                        (MathUtilities.GreaterThan(tick, next) || Math.Abs(next - value) < Tolerance))
                    {
                        next = tick;
                    }
                }
            }
            else if (MathUtilities.GreaterThan(TickFrequency, 0.0))
            {
                var tickNumber = Math.Round((value - Minimum) / TickFrequency);

                if (greaterThan)
                {
                    tickNumber += 1.0;
                }
                else
                {
                    tickNumber -= 1.0;
                }

                next = Minimum + tickNumber * TickFrequency;
            }
        }

        if (Math.Abs(next - value) > Tolerance)
        {
            LowerSelectedValue = next;
        }
    }

    private void TrackPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;

            var pointerCoord = e.GetCurrentPoint(_track).Position;
            _previousValue = GetValueByPointOnTrack(pointerCoord);
            _currentTrackThumb = GetNearestTrackThumb(pointerCoord);

            if (IsPressedOnTrackBetweenThumbs() == false && MoveWholeRange == true)
            {
                MoveToPoint(pointerCoord, _currentTrackThumb);
            }
            else if (MoveWholeRange == false && _currentTrackThumb != TrackThumb.Overlapped)
            {
                MoveToPoint(pointerCoord, _currentTrackThumb);
            }
        }
    }

    private void TrackReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        _currentTrackThumb = TrackThumb.None;
    }

    private void TrackMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging)
        {
            var pointerCoord = e.GetCurrentPoint(_track).Position;

            if (_currentTrackThumb == TrackThumb.Overlapped)
            {
                SelectThumbBasedOnPointerDirection(pointerCoord);
            }

            if (IsPressedOnTrackBetweenThumbs() == false && MoveWholeRange)
            {
                MoveToPoint(pointerCoord, _currentTrackThumb);
            }
            if (IsPressedOnTrackBetweenThumbs() == true && MoveWholeRange)
            {
                MoveToPoint(pointerCoord, TrackThumb.Both);
            }
            else if (MoveWholeRange == false)
            {
                MoveToPoint(pointerCoord, _currentTrackThumb);
            }
        }
    }

    private void MoveToPoint(Point pointerCoord, TrackThumb trackThumb)
    {
        var value = GetValueByPointOnTrack(pointerCoord);

        switch (trackThumb)
        {
            case TrackThumb.Upper:
            case TrackThumb.InnerUpper:
            case TrackThumb.OuterUpper:
                UpperSelectedValue = SnapToTick(value);
                break;
            case TrackThumb.Lower:
            case TrackThumb.InnerLower:
            case TrackThumb.OuterLower:
                LowerSelectedValue = SnapToTick(value);
                break;
            case TrackThumb.Both:
                var delta = value - _previousValue;

                if ((Math.Abs(LowerSelectedValue - Minimum) <= Tolerance && delta <= 0d)
                    || (Math.Abs(UpperSelectedValue - Maximum) <= Tolerance && delta >= 0d))
                {
                    return;
                }

                if (IsSnapToTickEnabled == false)
                {
                    _previousValue = value;
                    LowerSelectedValue += delta;
                    UpperSelectedValue += delta;
                }
                else
                {
                    var closestTick = SnapToTick(Math.Abs(delta) / 2d);
                    if (closestTick > 0d)
                    {
                        _previousValue = value;
                        LowerSelectedValue += closestTick * Math.Sign(delta);
                        UpperSelectedValue += closestTick * Math.Sign(delta);
                    }
                }
                break;
        }
    }

    private double GetValueByPointOnTrack(Point pointerCoord)
    {
        var orient = Orientation == Orientation.Horizontal;
        var trackLength = orient ? _track.Bounds.Width : _track.Bounds.Height;
        var pointNum = orient ? pointerCoord.X : pointerCoord.Y;
        var thumbLength = orient ? _track.LowerThumb.Width : _track.LowerThumb.Height;

        trackLength += double.Epsilon;

        if (IsThumbOverlap)
        {
            thumbLength /= 2.0;
        }

        if (pointNum <= thumbLength)
        {
            return orient ? Minimum : Maximum;
        }
        if (pointNum > trackLength - thumbLength)
        {
            return orient ? Maximum : Minimum;
        }

        trackLength -= 2.0 * thumbLength;
        pointNum -= thumbLength;

        var logicalPos = MathUtilities.Clamp(pointNum / trackLength, 0.0d, 1.0d);
        var invert = orient
            ? IsDirectionReversed ? 1 : 0
            : IsDirectionReversed ? 0 : 1;
        var calcVal = Math.Abs(invert - logicalPos);
        var range = Maximum - Minimum;
        var finalValue = calcVal * range + Minimum;

        return finalValue;
    }

    private bool IsPressedOnTrackBetweenThumbs()
    {
        return _currentTrackThumb == TrackThumb.InnerLower || _currentTrackThumb == TrackThumb.InnerUpper;
    }

    private void SelectThumbBasedOnPointerDirection(Point pointerCoord)
    {
        var value = GetValueByPointOnTrack(pointerCoord);
        var delta = _previousValue - value;

        if (delta >= 0d && delta < Tolerance)
        {
            return;
        }

        if (delta > 0d)
        {
            _currentTrackThumb = TrackThumb.Lower;
        }
        else
        {
            _currentTrackThumb = TrackThumb.Upper;
        }
    }

    private TrackThumb GetNearestTrackThumb(Point pointerCoord)
    {
        var orient = Orientation == Orientation.Horizontal;

        var lowerThumbPos = orient ? _track.LowerThumb.Bounds.Position.X : _track.LowerThumb.Bounds.Position.Y;
        var upperThumbPos = orient ? _track.UpperThumb.Bounds.Position.X : _track.UpperThumb.Bounds.Position.Y;
        var thumbWidth = orient ? _track.LowerThumb.Bounds.Width : _track.LowerThumb.Bounds.Height;
        var thumbHalfWidth = thumbWidth / 2d;

        var pointerPos = orient ? pointerCoord.X : pointerCoord.Y;

        if (IsThumbOverlap == true)
        {
            var isThumbsOverlapped = Math.Abs(lowerThumbPos - upperThumbPos) < Tolerance;

            if (isThumbsOverlapped)
            {
                return TrackThumb.Overlapped;
            }
        }

        if (Math.Abs(lowerThumbPos + thumbHalfWidth - pointerPos) <= thumbHalfWidth)
        {
            return TrackThumb.Lower;
        }
        else if (Math.Abs(upperThumbPos + thumbHalfWidth - pointerPos) <= thumbHalfWidth)
        {
            return TrackThumb.Upper;
        }

        if (Math.Abs(lowerThumbPos - pointerPos) < Math.Abs(upperThumbPos - pointerPos))
        {
            if (pointerPos < lowerThumbPos)
            {
                return orient ? TrackThumb.OuterLower : TrackThumb.InnerLower;
            }
            else
            {
                return orient ? TrackThumb.InnerLower : TrackThumb.OuterLower;
            }
        }
        else
        {
            if (pointerPos < upperThumbPos)
            {
                return orient ? TrackThumb.InnerUpper : TrackThumb.OuterUpper;
            }
            else
            {
                return orient ? TrackThumb.OuterUpper : TrackThumb.InnerUpper;
            }
        }
    }

    protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
    {
        if (property == LowerSelectedValueProperty || property == UpperSelectedValueProperty)
        {
            DataValidationErrors.SetError(this, error);
        }
    }

    //protected override void UpdateDataValidation<T>(AvaloniaProperty<T> property, BindingValue<T> value)
    //{
    //    if (property == LowerSelectedValueProperty || property == UpperSelectedValueProperty)
    //    {
    //        DataValidationErrors.SetError(this, value.Error);
    //    }
    //}

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == OrientationProperty)
        {
            UpdatePseudoClasses(change.GetNewValue<Orientation>());
        }
    }

    private double SnapToTick(double value)
    {
        if (IsSnapToTickEnabled)
        {
            var previous = Minimum;
            var next = Maximum;
            var ticks = Ticks;

            if (ticks != null && ticks.Count > 0)
            {
                foreach (var tick in ticks)
                {
                    if (MathUtilities.AreClose(tick, value))
                    {
                        return value;
                    }

                    if (MathUtilities.LessThan(tick, value) && MathUtilities.GreaterThan(tick, previous))
                    {
                        previous = tick;
                    }
                    else if (MathUtilities.GreaterThan(tick, value) && MathUtilities.LessThan(tick, next))
                    {
                        next = tick;
                    }
                }
            }
            else if (MathUtilities.GreaterThan(TickFrequency, 0.0))
            {
                previous = Minimum + Math.Round((value - Minimum) / TickFrequency) * TickFrequency;
                next = Math.Min(Maximum, previous + TickFrequency);
            }

            value = MathUtilities.GreaterThanOrClose(value, (previous + next) * 0.5) ? next : previous;
        }

        return value;
    }

    private void UpdatePseudoClasses(Orientation o)
    {
        PseudoClasses.Set(":vertical", o == Orientation.Vertical);
        PseudoClasses.Set(":horizontal", o == Orientation.Horizontal);
    }
}