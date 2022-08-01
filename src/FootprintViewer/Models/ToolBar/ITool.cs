namespace FootprintViewer.Models
{
    public interface ITool : IToolItem, ISelectorItem
    {
        object? Tag { get; set; }
    }
}
