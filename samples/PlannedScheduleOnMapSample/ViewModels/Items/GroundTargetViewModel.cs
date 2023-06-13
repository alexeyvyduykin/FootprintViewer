using FootprintViewer.Data.Builders;
using FootprintViewer.Data.Models;

namespace PlannedScheduleOnMapSample.ViewModels.Items;

public class GroundTargetViewModel : ViewModelBase
{
    public GroundTargetViewModel() : this(GroundTargetBuilder.CreateRandom()) { }

    public GroundTargetViewModel(GroundTarget groundTarget)
    {
        Model = groundTarget;

        Name = groundTarget.Name!;

        Type = groundTarget.Type!.ToString();
    }

    public string Name { get; set; }

    public string Type { get; set; }

    public GroundTarget Model { get; set; }
}
