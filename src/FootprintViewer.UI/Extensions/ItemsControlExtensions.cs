using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Linq;

namespace FootprintViewer.UI.Extensions;

public static class ItemsControlExtensions
{
    public static void ScrollToCenterOfView(this ItemsControl itemsControl, object item)
    {
        // Scroll immediately if possible
        if (!itemsControl.TryScrollToCenterOfView(item))
        {
            // Otherwise wait untileverything is loaded, then scroll
            if (itemsControl is ListBox)
            {
                ((ListBox)itemsControl).ScrollIntoView(item);
            }

            //itemsControl.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            //{
            //    itemsControl.TryScrollToCenterOfView(item);
            //}));


            Dispatcher.UIThread.InvokeAsync(() =>
            {
                itemsControl.TryScrollToCenterOfView(item);
            }, DispatcherPriority.Loaded);
        }
    }

    private static bool TryScrollToCenterOfView(this ItemsControl itemsControl, object item)
    {
        int index = 0;
        foreach (var s in itemsControl.Items)
        {
            if (s != null && s.Equals(item) == true)
            {
                break;
            }
            index++;
        }

        // Find the container
        var container = itemsControl.ItemContainerGenerator.ContainerFromIndex(index);// ContainerFromItem(item) as UIElement;

        if (container == null)
        {
            return false;
        }

        // Find the ScrollContentPresenter
        ScrollContentPresenter? presenter = null;
        for (Visual? vis = container; vis != null && vis != itemsControl; vis = vis.GetVisualParent()/* VisualTreeHelper.GetParent(vis)*/)
        {
            if ((presenter = vis as ScrollContentPresenter) != null)
            {
                break;
            }
        }

        if (presenter == null)
        {
            return false;
        }

        // Find the IScrollable
        var scrollInfo = (presenter.CanHorizontallyScroll || presenter.CanVerticallyScroll) /*CanContentScroll*/ == false ? presenter :
            presenter.Content as IScrollable ??
            FirstVisualChild(presenter.Content as ItemsPresenter) as IScrollable ??
            presenter;

        //var scrollInfo = presenter;

        // Compute the center point of the container relative to the scrollInfo
        Size size = container.DesiredSize;//RenderSize;
                                          // Point c = container.TransformToAncestor((IVisual)scrollInfo).Transform(new Point(size.Width / 2, size.Height / 2));

        Point c = container.TranslatePoint(new Point(size.Width / 2, size.Height / 2), (Visual)scrollInfo)!.Value;

        Point center = new Point(c.X + scrollInfo.Offset.X, c.Y + scrollInfo.Offset.Y);

        // Adjust for logical scrolling
        if (scrollInfo is StackPanel || scrollInfo is VirtualizingStackPanel)
        {
            double logicalCenter = itemsControl.ItemContainerGenerator.IndexFromContainer(container) + 0.5;
            //     Orientation orientation = scrollInfo is StackPanel ? ((StackPanel)scrollInfo).Orientation : ((VirtualizingStackPanel)scrollInfo).Orientation;
            //      if (orientation == Orientation.Horizontal)
            {
                //          center = new Point(logicalCenter, c.Y + scrollInfo.Offset.Y);                    
            }
            //      else
            {
                center = new Point(c.X + scrollInfo.Offset.X, logicalCenter);
            }
        }

        // Scroll the center of the container to the center of the viewport
        if (scrollInfo.Offset.Y != 0/* CanVerticallyScroll*/)
        {
            var x = scrollInfo.Offset.X;
            scrollInfo.Offset = new Vector(x, CenteringOffset(center.Y, scrollInfo.Viewport.Height/*ViewportHeight*/, scrollInfo.Extent.Height/*ExtentHeight*/));// SetVerticalOffset(CenteringOffset(center.Y, scrollInfo.ViewportHeight, scrollInfo.ExtentHeight));
        }
        if (scrollInfo.Offset.X != 0/* CanHorizontallyScroll*/)
        {
            var y = scrollInfo.Offset.Y;
            scrollInfo.Offset = new Vector(CenteringOffset(center.X, scrollInfo.Viewport.Width/* ViewportWidth*/, scrollInfo.Extent.Width/* ExtentWidth*/), y);//.SetHorizontalOffset(CenteringOffset(center.X, scrollInfo.ViewportWidth, scrollInfo.ExtentWidth));
        }

        return true;
    }

    private static double CenteringOffset(double center, double viewport, double extent)
    {
        return Math.Min(extent - viewport, Math.Max(0, center - viewport / 2));
    }

    private static AvaloniaObject? FirstVisualChild(Visual? visual)
    {
        if (visual == null)
        {
            return null;
        }
        if (visual.GetVisualChildren().Count()/* VisualTreeHelper.GetChildrenCount(visual)*/ == 0)
        {
            return null;
        }

        return (AvaloniaObject)visual.GetVisualChildren().First();// VisualTreeHelper.GetChild(visual, 0);
    }
}
