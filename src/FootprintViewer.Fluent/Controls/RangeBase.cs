using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Utilities;

namespace FootprintViewer.Fluent.Controls;

public abstract class RangeBase : TemplatedControl
{
    public static readonly DirectProperty<RangeBase, double> MinimumProperty =
        AvaloniaProperty.RegisterDirect<RangeBase, double>(
            nameof(Minimum),
            o => o.Minimum,
            (o, v) => o.Minimum = v);

    public static readonly DirectProperty<RangeBase, double> MaximumProperty =
        AvaloniaProperty.RegisterDirect<RangeBase, double>(
            nameof(Maximum),
            o => o.Maximum,
            (o, v) => o.Maximum = v);

    public static readonly DirectProperty<RangeBase, double> LowerSelectedValueProperty =
        AvaloniaProperty.RegisterDirect<RangeBase, double>(
            nameof(LowerSelectedValue),
            o => o.LowerSelectedValue,
            (o, v) => o.LowerSelectedValue = v,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<RangeBase, double> UpperSelectedValueProperty =
        AvaloniaProperty.RegisterDirect<RangeBase, double>(
            nameof(UpperSelectedValue),
            o => o.UpperSelectedValue,
            (o, v) => o.UpperSelectedValue = v,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<double> SmallChangeProperty =
        AvaloniaProperty.Register<RangeBase, double>(nameof(SmallChange), 1);

    public static readonly StyledProperty<double> LargeChangeProperty =
        AvaloniaProperty.Register<RangeBase, double>(nameof(LargeChange), 10);

    private double _minimum;
    private double _maximum = 100.0;
    private double _lowerSelectedValue;
    private double _upperSelectedValue;
    private bool _upperValueInitializedNonZeroValue = false;

    public RangeBase()
    {
    }

    public double Minimum
    {
        get
        {
            return _minimum;
        }

        set
        {
            if (!ValidateDouble(value))
            {
                return;
            }

            if (IsInitialized)
            {
                SetAndRaise(MinimumProperty, ref _minimum, value);
                Maximum = ValidateMaximum(Maximum);
                LowerSelectedValue = ValidateLowerValue(LowerSelectedValue);
                UpperSelectedValue = ValidateUpperValue(UpperSelectedValue);
            }
            else
            {
                SetAndRaise(MinimumProperty, ref _minimum, value);
            }
        }
    }

    public double Maximum
    {
        get
        {
            return _maximum;
        }

        set
        {
            if (!ValidateDouble(value))
            {
                return;
            }

            if (IsInitialized)
            {
                value = ValidateMaximum(value);
                SetAndRaise(MaximumProperty, ref _maximum, value);
                LowerSelectedValue = ValidateLowerValue(LowerSelectedValue);
                UpperSelectedValue = ValidateUpperValue(UpperSelectedValue);
            }
            else
            {
                SetAndRaise(MaximumProperty, ref _maximum, value);
            }
        }
    }

    public double LowerSelectedValue
    {
        get
        {
            return _lowerSelectedValue;
        }

        set
        {
            if (!ValidateDouble(value))
            {
                return;
            }

            if (IsInitialized)
            {
                value = ValidateLowerValue(value);
                SetAndRaise(LowerSelectedValueProperty, ref _lowerSelectedValue, value);
            }
            else
            {
                SetAndRaise(LowerSelectedValueProperty, ref _lowerSelectedValue, value);
            }
        }
    }

    public double UpperSelectedValue
    {
        get
        {
            return _upperSelectedValue;
        }

        set
        {
            if (!ValidateDouble(value))
            {
                return;
            }

            if (IsInitialized)
            {
                value = ValidateUpperValue(value);
                _upperValueInitializedNonZeroValue = value > 0.0;
                SetAndRaise(UpperSelectedValueProperty, ref _upperSelectedValue, value);
            }
            else
            {
                SetAndRaise(UpperSelectedValueProperty, ref _upperSelectedValue, value);
            }
        }
    }

    public double SmallChange
    {
        get => GetValue(SmallChangeProperty);
        set => SetValue(SmallChangeProperty, value);
    }

    public double LargeChange
    {
        get => GetValue(LargeChangeProperty);
        set => SetValue(LargeChangeProperty, value);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Maximum = ValidateMaximum(Maximum);
        LowerSelectedValue = ValidateLowerValue(LowerSelectedValue);
        UpperSelectedValue = ValidateUpperValue(UpperSelectedValue);
    }

    private static bool ValidateDouble(double value)
    {
        return !double.IsInfinity(value) || !double.IsNaN(value);
    }

    private double ValidateMaximum(double value)
    {
        return Math.Max(value, Minimum);
    }

    private double ValidateLowerValue(double value)
    {
        return _upperValueInitializedNonZeroValue
            ? MathUtilities.Clamp(value, Minimum, UpperSelectedValue)
            : MathUtilities.Clamp(value, Minimum, Maximum);
    }

    private double ValidateUpperValue(double value)
    {
        return MathUtilities.Clamp(value, LowerSelectedValue, Maximum);
    }
}