using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Models;

namespace FootprintViewer.UI.ViewModels.SidePanel.Items;

public sealed class UserGeometryViewModel : ViewModelBase, IViewerItem, ISelectorItem
{
    private readonly UserGeometry _model;

    public UserGeometryViewModel() : this(UserGeometryBuilder.CreateRandom()) { }

    public UserGeometryViewModel(UserGeometry model)
    {
        _model = model;
        Key = _model.Type.ToString();
    }

    public string Key { get; set; }

    public UserGeometry Model => _model;

    public string Name => _model.Name!;

    public UserGeometryType Type => _model.Type;

    public bool IsShowInfo { get; set; }
}
