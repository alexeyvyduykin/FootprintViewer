using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;
using System;
using System.Linq;
using System.Reflection;
using UserGeometriesDatabaseSample.Data;
using UserGeometriesDatabaseSample.ViewModels;
using UserGeometriesDatabaseSample.Views;

namespace UserGeometriesDatabaseSample
{
    public class ViewLocator : IDataTemplate
    {
        private static Type[]? _classes;

        static ViewLocator()
        {
            var asm = Assembly.Load("UserGeometriesDatabaseSample");
            _classes = asm.GetTypes().Where(s =>
            {
                if (s.Namespace != null)
                {
                    return s.Namespace.Contains("UserGeometriesDatabaseSample.Views") && s.Name.Contains("View");
                }

                return false;
            }).ToArray();
        }

        public bool SupportsRecycling => false;


        //public IControl Build(object data)
        //{
        //    var type = data.GetType();

        //    if (type == typeof(MainWindowViewModel))
        //    {
        //        return (Control)Activator.CreateInstance(typeof(MainWindow))!;
        //    }
        //    else if (type == typeof(UserGeometry))
        //    {
        //        return (Control)Activator.CreateInstance(typeof(UserGeometryView))!;
        //    }

        //    throw new Exception();
        //}

        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            //if (type != null)
            //{
            //    return (Control)Activator.CreateInstance(type)!;
            //}
            //else
            {
                var name2 = data.GetType().Name! + "View";

                var res = _classes?.Where(type => type.Name.Equals(name2)).FirstOrDefault();

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
