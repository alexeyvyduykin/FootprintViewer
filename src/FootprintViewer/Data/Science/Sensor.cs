using System;
using System.Collections.Generic;
using System.Text;

namespace FootprintViewer.Data.Science
{
        public class PRDCTSensor
        {
        double _verticalHalfAngleDEG;
        double _rollAngleDEG;
        public PRDCTSensor(double verticalHalfAngleDEG, double rollAngleDEG)
        {
            _verticalHalfAngleDEG = verticalHalfAngleDEG;
            _rollAngleDEG = rollAngleDEG;
        }

        public double VerticalHalfAngleDEG => _verticalHalfAngleDEG;
        public double RollAngleDEG => _rollAngleDEG;

        }    
}
