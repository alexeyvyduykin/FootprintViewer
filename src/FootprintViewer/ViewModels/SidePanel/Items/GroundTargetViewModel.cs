using FootprintViewer.Data;
using ReactiveUI;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetViewModel : ReactiveObject, ISelectorItem, IViewerItem
    {
        private readonly GroundTarget _groundTarget;
        private readonly GroundTargetType _type;
        private readonly string _name;

        public GroundTargetViewModel(GroundTarget groundTarget)
        {
            _groundTarget = groundTarget;
            _type = groundTarget.Type;
            _name = groundTarget.Name!;
        }

        public string GetKey() => _type.ToString();

        public GroundTarget GroundTarget => _groundTarget;

        public string Name => _name;

        public GroundTargetType Type => _type;

        public bool IsShowInfo { get; set; }
    }
}
