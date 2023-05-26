using FootprintViewer.Data.Builders;
using FootprintViewer.Fluent.ViewModels.AddPlannedSchedule.Items;
using FootprintViewer.Fluent.ViewModels.InfoPanel;
using FootprintViewer.Fluent.ViewModels.Navigation;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace FootprintViewer.Fluent.Designer;

public static class DesignData
{
    public static InfoPanelViewModel InfoPanel => CreateInfoPanel();

    public static RoutableViewModel ContentArea => new ContentAreaDesign() { IsActive = true };

    private static InfoPanelViewModel CreateInfoPanel()
    {
        var panel = new InfoPanelViewModel();

        panel.Show(InfoPanelItemViewModel.Create("Route", "Route description"));
        panel.Show(InfoPanelItemViewModel.Create("AOI", "AOI description"));
        panel.Show(FootprintInfoPanelItemViewModel.Create(new(FootprintBuilder.CreateRandom())));
        panel.Show(GroundTargetInfoPanelItemViewModel.Create(new(GroundTargetBuilder.CreateRandom())));
        panel.Show(UserGeometryInfoPanelItemViewModel.Create(new(UserGeometryBuilder.CreateRandom())));

        return panel;
    }
}

public class SelectRecordPageDesignViewModel : RoutableViewModel
{
    public SelectRecordPageDesignViewModel()
    {
        EnableBack = true;
        EnableCancel = true;

        Items = new()
        {
            new PlannedScheduleItemViewModel("PlannedSchedule1", new DateTime(2023, 4, 20, 12, 32, 43)),
            new PlannedScheduleItemViewModel("PlannedSchedule2", new DateTime(2023, 3, 23, 22, 54, 11)),
            new PlannedScheduleItemViewModel("PlannedSchedule3", new DateTime(2023, 4, 25, 03, 02, 54)),
            new PlannedScheduleItemViewModel("PlannedSchedule4", new DateTime(2023, 4, 21, 11, 11, 44)),
            new PlannedScheduleItemViewModel("PlannedSchedule5", new DateTime(2023, 4, 18, 19, 22, 42)),
        };

        SelectedItem = Items.FirstOrDefault();

        IsActive = true;
    }

    public override string Title { get => "Select planned schedule"; protected set { } }

    public List<PlannedScheduleItemViewModel> Items { get; set; }

    public PlannedScheduleItemViewModel? SelectedItem { get; set; }
}

public class ContentAreaDesign : RoutableViewModel
{
    public ContentAreaDesign()
    {
        EnableBack = true;
        EnableCancel = true;
        IsActive = true;

        SkipCommand = ReactiveCommand.Create(() => { });
        NextCommand = ReactiveCommand.Create(() => { });
    }

    public override string Title { get => "This is a title"; protected set { } }
}