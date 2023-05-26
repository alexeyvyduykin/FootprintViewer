using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Generators;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Reactive;
using System.Collections;

namespace FootprintViewer.UI.Controls;

public class SidePanel : TabControl
{
    public SidePanel() : base()
    {
        AffectsMeasure<SidePanel>(IsExpandedProperty);
    }

    protected override IItemContainerGenerator CreateItemContainerGenerator()
    {
        return new SidePanelItemContainerGenerator(this);
    }

    public static readonly StyledProperty<bool> IsExpandedProperty =
        AvaloniaProperty.Register<SidePanel, bool>(nameof(IsExpanded), true);

    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public static readonly StyledProperty<IBrush> PaneBackgroundProperty =
        SplitView.PaneBackgroundProperty.AddOwner<SidePanel>();

    public IBrush PaneBackground
    {
        get => GetValue(PaneBackgroundProperty);
        set => SetValue(PaneBackgroundProperty, value);
    }

    public static readonly StyledProperty<IBrush> ContentBackgroundProperty =
        AvaloniaProperty.Register<SidePanel, IBrush>(nameof(ContentBackground));

    public IBrush ContentBackground
    {
        get => GetValue(ContentBackgroundProperty);
        set => SetValue(ContentBackgroundProperty, value);
    }

    public static readonly StyledProperty<IBrush> TitleBackgroundProperty =
        AvaloniaProperty.Register<SidePanel, IBrush>(nameof(TitleBackground));

    public IBrush TitleBackground
    {
        get => GetValue(TitleBackgroundProperty);
        set => SetValue(TitleBackgroundProperty, value);
    }

    public static readonly StyledProperty<double> PaneTitleHeightProperty =
        AvaloniaProperty.Register<SidePanel, double>(nameof(PaneTitleHeight), 50);

    public double PaneTitleHeight
    {
        get => GetValue(PaneTitleHeightProperty);
        set => SetValue(PaneTitleHeightProperty, value);
    }

    public static readonly StyledProperty<double> PaneWidthProperty =
        AvaloniaProperty.Register<SidePanel, double>(nameof(PaneWidth), 100);

    public double PaneWidth
    {
        get => GetValue(PaneWidthProperty);
        set => SetValue(PaneWidthProperty, value);
    }

    private IEnumerable _actionTabs = new AvaloniaList<object>();

    public static readonly DirectProperty<SidePanel, IEnumerable> ActionTabsProperty =
        AvaloniaProperty.RegisterDirect<SidePanel, IEnumerable>(nameof(ActionTabs), o => o.ActionTabs, (o, v) => o.ActionTabs = v);

    public IEnumerable ActionTabs
    {
        get { return _actionTabs; }
        set { SetAndRaise(ActionTabsProperty, ref _actionTabs, value); }
    }

    private class SidePanelItemContainerGenerator : ItemContainerGenerator<SidePanelItem>
    {
        public SidePanelItemContainerGenerator(SidePanel owner)
            : base(owner, ContentControl.ContentProperty, ContentControl.ContentTemplateProperty)
        {
            Owner = owner;
        }

        public new SidePanel Owner { get; }

        protected override IControl CreateContainer(object item)
        {
            var sidePanelItem = (SidePanelItem)base.CreateContainer(item)!;

            sidePanelItem.Bind(SidePanelItem.TabStripPlacementProperty, new OwnerBinding<Dock>(
                sidePanelItem,
                SidePanel.TabStripPlacementProperty));

            if (sidePanelItem.HeaderTemplate == null)
            {
                sidePanelItem.Bind(SidePanelItem.HeaderTemplateProperty, new OwnerBinding<IDataTemplate?>(
                    sidePanelItem,
                    SidePanel.ItemTemplateProperty));
            }

            if (sidePanelItem.Header == null)
            {
                if (item is IHeadered headered)
                {
                    sidePanelItem.Header = headered.Header;
                }
                else
                {
                    if (!(sidePanelItem.DataContext is IControl))
                    {
                        sidePanelItem.Header = sidePanelItem.DataContext;
                    }
                }
            }

            if (!(sidePanelItem.Content is IControl))
            {
                sidePanelItem.Bind(SidePanelItem.ContentTemplateProperty, new OwnerBinding<IDataTemplate?>(
                    sidePanelItem,
                    SidePanel.ContentTemplateProperty));
            }

            return sidePanelItem;
        }

        private class OwnerBinding<T> : SingleSubscriberObservableBase<T>
        {
            private readonly SidePanelItem _item;
            private readonly StyledProperty<T> _ownerProperty;
            private IDisposable? _ownerSubscription;
            private IDisposable? _propertySubscription;

            public OwnerBinding(SidePanelItem item, StyledProperty<T> ownerProperty)
            {
                _item = item;
                _ownerProperty = ownerProperty;
            }

            protected override void Subscribed()
            {
                _ownerSubscription = ControlLocator.Track(_item, 0, typeof(SidePanel)).Subscribe(OwnerChanged);
            }

            protected override void Unsubscribed()
            {
                _ownerSubscription?.Dispose();
                _ownerSubscription = null;
            }

            private void OwnerChanged(ILogical? c)
            {
                _propertySubscription?.Dispose();
                _propertySubscription = null;

                if (c is SidePanel sidePanel)
                {
                    _propertySubscription = sidePanel.GetObservable(_ownerProperty)
                        .Subscribe(x => PublishNext(x));
                }
            }
        }

    }
}
