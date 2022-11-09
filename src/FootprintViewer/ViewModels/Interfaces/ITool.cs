namespace FootprintViewer.ViewModels;

public interface ITool : IToolItem, ISelectorItem
{
    object? Tag { get; set; }
}
