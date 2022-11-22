namespace FootprintViewer.ViewModels.Settings;

public class TableColumnViewModel
{
    public string? FieldName { get; set; }

    public string? FieldType { get; set; }

    public string? Hyperlink { get; set; }

    public bool PrimaryKey { get; set; }

    public string? Description { get; set; }

    public string? Dimension { get; set; }
}
