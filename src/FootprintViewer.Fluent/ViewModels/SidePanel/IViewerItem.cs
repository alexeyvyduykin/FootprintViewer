namespace FootprintViewer.Fluent.ViewModels.SidePanel;

public interface IViewerItem
{
    string Name { get; }

    bool IsShowInfo { get; set; }
}
