using System;

namespace FootprintViewer.InteractivityEx
{
    public class FeatureEventArgs : EventArgs
    {
        public AddInfo AddInfo { get; set; }
    }

    public class EditingFeatureEventArgs : EventArgs
    {
        public InteractiveFeature Feature { get; set; }
    }

    public delegate void FeatureEventHandler(object sender, FeatureEventArgs e);
    public delegate void EditingFeatureEventHandler(object sender, EditingFeatureEventArgs e);
}
