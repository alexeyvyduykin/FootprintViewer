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
            if (item is AOIInfoPanel)
            {
                return AOITemplate;
            }

            if (item is RouteInfoPanel)
            {
                return RouteTemplate;
            }

            return default;
        }
    }
}
