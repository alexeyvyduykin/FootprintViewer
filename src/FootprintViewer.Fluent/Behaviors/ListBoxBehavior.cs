using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using FootprintViewer.Fluent.Extensions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;

namespace FootprintViewer.Fluent.Behaviors;

public class ListBoxBehavior : AvaloniaObject
{
    static ListBoxBehavior()
    {
        ScrollSelectedIntoViewProperty.Changed.Subscribe(OnScrollSelectedIntoViewChanged);
    }

    public static bool GetScrollSelectedIntoView(ListBox listBox)
    {
        return (bool)listBox.GetValue(ScrollSelectedIntoViewProperty);
    }

    public static void SetScrollSelectedIntoView(ListBox listBox, bool value)
    {
        listBox.SetValue(ScrollSelectedIntoViewProperty, value);
    }

    public static readonly AttachedProperty<bool> ScrollSelectedIntoViewProperty =
        AvaloniaProperty.RegisterAttached<ListBoxBehavior, ListBox, bool>("ScrollSelectedIntoView", false);
    //  new UIPropertyMetadata(false, OnScrollSelectedIntoViewChanged));

    private static void OnScrollSelectedIntoViewChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var selector = e.Sender as SelectingItemsControl;

        if (selector == null)
        {
            return;
        }

        if (e.NewValue is bool == false)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            selector.AddHandler(SelectingItemsControl.SelectionChangedEvent, new EventHandler<RoutedEventArgs> /*RoutedEventHandler*/(ListBoxSelectionChangedHandler));
        }
        else
        {
            selector.RemoveHandler(SelectingItemsControl.SelectionChangedEvent, new EventHandler<RoutedEventArgs> /*RoutedEventHandler*/(ListBoxSelectionChangedHandler));
        }
    }

    private static void ListBoxSelectionChangedHandler(object? sender, RoutedEventArgs e)
    {
        if (!(sender is ListBox))
        {
            return;
        }

        var listBox = (sender as ListBox);

        if (listBox != null && listBox.SelectedItem != null)
        {
            //listBox.Dispatcher.BeginInvoke((Action)(() =>
            //{
            //    listBox.UpdateLayout();
            //    if (listBox.SelectedItem != null)
            //    {
            //        listBox.ScrollToCenterOfView(listBox.SelectedItem);
            //        //listBox.ScrollIntoView(listBox.SelectedItem);
            //    }
            //}));

            Dispatcher.UIThread.InvokeAsync(() =>
            {
                listBox.InvalidateMeasure();
                //   listBox.UpdateLayout();
                if (listBox.SelectedItem != null)
                {
                    listBox.ScrollToCenterOfView(listBox.SelectedItem);
                    //listBox.ScrollIntoView(listBox.SelectedItem);
                }
            });
        }
    }
}


public class CommandBasedBehavior<T> : Behavior<T> where T : AvaloniaObject
{
    private ICommand? _command;

    public static readonly DirectProperty<CommandBasedBehavior<T>, ICommand?> CommandProperty =
        AvaloniaProperty.RegisterDirect<CommandBasedBehavior<T>, ICommand?>(nameof(Command), commandBehavior => commandBehavior._command, (commandBehavior, command) => commandBehavior.Command = command, enableDataValidation: true);

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<CommandBasedBehavior<T>, object?>(nameof(CommandProperty));

    public ICommand? Command
    {
        get => _command;
        set => SetAndRaise(CommandProperty, ref _command, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected bool ExecuteCommand()
    {
        if (Command != null && Command.CanExecute(CommandParameter))
        {
            Command.Execute(CommandParameter);
            return true;
        }

        return false;
    }
}

public class CommandOnClickBehavior : CommandBasedBehavior<InputElement>
{
    private CompositeDisposable? Disposables { get; set; }

    protected override void OnAttached()
    {
        Disposables = new CompositeDisposable();

        base.OnAttached();

        if (AssociatedObject != null)
        {
            Disposables.Add(AssociatedObject.AddDisposableHandler(InputElement.PointerPressedEvent, (sender, e) =>
            {
                e.Handled = ExecuteCommand();
            }));
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        Disposables?.Dispose();
    }
}
