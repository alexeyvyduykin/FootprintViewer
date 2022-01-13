using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.ViewModels
{
    public class PreviewMainContent : ReactiveObject
    {
        private readonly string _text;
        public PreviewMainContent(string text)
        {
            _text = text;
        }

        public string Text => _text;
    }
}
