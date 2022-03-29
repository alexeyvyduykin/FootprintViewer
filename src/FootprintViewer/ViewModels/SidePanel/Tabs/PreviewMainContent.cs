using ReactiveUI;

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
