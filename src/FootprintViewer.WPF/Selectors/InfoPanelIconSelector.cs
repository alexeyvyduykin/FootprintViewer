using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using FootprintViewer.ViewModels;

namespace FootprintViewer.WPF
{
    public class InfoPanelIconSelector : DataTemplateSelector
    {
        public DataTemplate RouteTemplate { get; set; }
        public DataTemplate AOITemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is InfoPanelItem infoPanelItem)
            {
                if (infoPanelItem.Title == "Route")
                {
                    return RouteTemplate;
                }
                else if (infoPanelItem.Title == "AOI")
                {
                    return AOITemplate;
                }
            }

            return default;
        }
    }
}
