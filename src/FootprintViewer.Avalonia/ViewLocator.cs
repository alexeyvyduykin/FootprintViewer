using Avalonia.Controls;
using Avalonia.Controls.Templates;
using vm = FootprintViewer.ViewModels;
using tabs = FootprintViewer.Avalonia.Views.SidePanelTabs;
using v = FootprintViewer.Avalonia.Views;
using ReactiveUI;
using System;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace FootprintViewer.Avalonia
{
    public class ViewLocator : IDataTemplate
    {
        private static Type[]? _classes;
        
        static ViewLocator()
        {
            var asm = Assembly.Load("FootprintViewer.Avalonia");
            _classes = asm.GetTypes().Where(s => 
            {
                if (s.Namespace != null)
                {
                    return s.Namespace.Contains("FootprintViewer.Avalonia.Views") && s.Name.Contains("View");
                }

                return false;
            }).ToArray();
        }

        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {        
                var name2 = data.GetType().Name! + "View";
      
                var res = _classes.Where(type => type.Name.Equals(name2)).FirstOrDefault();
                
                if (res != null)
                {
                    return (Control)Activator.CreateInstance(res)!;
                }

                return new TextBlock { Text = "Not Found: " + name };
            }
        }

        public bool Match(object data)
        {
            return data is ReactiveObject;
        }
    }
}
