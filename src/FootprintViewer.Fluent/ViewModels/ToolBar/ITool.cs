using FootprintViewer.Fluent.ViewModels.SidePanel;

namespace FootprintViewer.Fluent.ViewModels.ToolBar;

public interface ITool : IToolItem, ISelectorItem
{
    object? Tag { get; set; }
}
