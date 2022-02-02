namespace FootprintViewer.Models
{
    public interface ITool : IToolItem
    {
        string? Tooltip { get; set; }

        string? Title { get; set; }
    }
}
