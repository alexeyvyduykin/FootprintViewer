using FootprintViewer.ViewModels;

namespace FootprintViewer.Designer
{
    public class DesignTimeTip : Tip
    {
        public DesignTimeTip() : base()
        {
            var area = 34545.432;

            Text = "Отпустите клавишу мыши для завершения рисования";

            Title = $"Область: {FormatHelper.ToArea(area)}";
        }
    }
}
