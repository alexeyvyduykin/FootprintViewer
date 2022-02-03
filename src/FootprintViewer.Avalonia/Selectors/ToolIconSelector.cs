using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using FootprintViewer.Models;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Avalonia
{
    public class ToolIconSelector : IDataTemplate
    {
        [Content]
        public Dictionary<string, IDataTemplate> Templates { get; } = new Dictionary<string, IDataTemplate>();

        public IControl Build(object param)
        {
            var key = ((ITool)param).Title;

            if (key != null)
            {
                if (Templates.ContainsKey(key) == true)
                {
                    return Templates[key].Build(param);
                }
                else
                {
                    return new TextBlock() { Text = "ToolIconSelector not find template" };
                }
            }

            throw new Exception();
        }

        public bool Match(object data)
        {
            return data is ITool;
        }
    }
}
