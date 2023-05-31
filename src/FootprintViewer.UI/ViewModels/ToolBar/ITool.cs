using FootprintViewer.UI.ViewModels.SidePanel;

namespace FootprintViewer.UI.ViewModels.ToolBar;

public interface ITool : IToolItem, ISelectorItem
{
    object? Tag { get; set; }
}
