﻿using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using FootprintViewer.ViewModels;
using System;
using System.Collections.Generic;

namespace FootprintViewer.Avalonia.Selectors
{
    public class SidePanelIconSelector : IDataTemplate
    {
        [Content]
        public Dictionary<string, IDataTemplate> Templates { get; } = new Dictionary<string, IDataTemplate>();

        public IControl Build(object param)
        {
            var key = param.GetType().Name;

            if (key != null)
            {
                return Templates[key].Build(param);
            }

            throw new Exception();
        }

        public bool Match(object data)
        {
            return data is SidePanelTab;
        }
    }
}