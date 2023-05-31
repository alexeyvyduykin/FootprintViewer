namespace FootprintViewer.UI.ViewModels.SidePanel;

public interface IViewerItem
{
    string Name { get; }

    bool IsShowInfo { get; set; }
}
