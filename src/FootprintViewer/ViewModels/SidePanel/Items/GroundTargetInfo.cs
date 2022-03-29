﻿using FootprintViewer.Data;
using ReactiveUI;

namespace FootprintViewer.ViewModels
{
    public class GroundTargetInfo : ReactiveObject
    {
        private readonly GroundTarget _groundTarget;
        private readonly GroundTargetType _type;
        private readonly string _name;

        public GroundTargetInfo(GroundTarget groundTarget)
        {
            _groundTarget = groundTarget;
            _type = groundTarget.Type;
            _name = groundTarget.Name!;
        }

        public GroundTarget GroundTarget => _groundTarget;

        public string Name => _name;

        public GroundTargetType Type => _type;
    }

}
