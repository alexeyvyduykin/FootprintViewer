using FootprintViewer.Data;
using FootprintViewer.ViewModels;
using System;

namespace FootprintViewer.Designer
{
    public class DesignTimeGroundTargetInfo : GroundTargetInfo
    {
        private readonly static Random _random = new Random();

        public DesignTimeGroundTargetInfo() : base(BuildModel())
        {

        }

        public static GroundTarget BuildModel()
        {
            var type = (GroundTargetType)_random.Next(0, 3);

            return new GroundTarget()
            {
                Name = $"GroundTarget{_random.Next(1, 101):000}",
                Type = type,
            };
        }
    }
}
