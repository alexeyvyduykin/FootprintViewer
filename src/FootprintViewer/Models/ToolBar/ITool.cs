namespace FootprintViewer.Models
{
    public interface ITool : IToolItem, ISelectorItem
    {
        string? Tooltip { get; set; }

        string? Title { get; set; }
    }
}
