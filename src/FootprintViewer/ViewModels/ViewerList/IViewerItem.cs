namespace FootprintViewer.ViewModels
{
    public interface IViewerItem
    {
        string Name { get; }

        bool IsShowInfo { get; set; }
    }
}
