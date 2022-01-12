using Avalonia.Controls;
using Avalonia.Controls.Templates;
using FootprintViewer.Avalonia.Views.SidePanelTabs;
using vm = FootprintViewer.ViewModels;
using tabs = FootprintViewer.Avalonia.Views.SidePanelTabs;
using v = FootprintViewer.Avalonia.Views;
using ReactiveUI;
using System;

namespace FootprintViewer.Avalonia
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);


            if(data.GetType().Equals(typeof(vm.FootprintObserverFilter)) == true)
            {
                type = typeof(FootprintViewerFilter);
            }

            if (data.GetType().Equals(typeof(vm.SceneSearchFilter)) == true)
            {
                type = typeof(tabs.SceneSearchFilter);
            }

            if (data.GetType().Equals(typeof(vm.WorldMapSelector)) == true)
            {
                type = typeof(v.WorldMapSelectorView);
            }

            if (data.GetType().Equals(typeof(vm.SceneSearch)) == true)
            {
                type = typeof(tabs.SceneSearchTab);
            }

            if (data.GetType().Equals(typeof(vm.SatelliteViewer)) == true)
            {
                type = typeof(tabs.SatelliteViewerTab);
            }

            if (data.GetType().Equals(typeof(vm.GroundTargetViewer)) == true)
            {
                type = typeof(tabs.GroundTargetViewerTab);
            }

            if (data.GetType().Equals(typeof(vm.FootprintObserver)) == true)
            {
                type = typeof(tabs.FootprintViewerTab);
            }

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object data)
        {
            return data is ReactiveObject;
        }
    }
}
