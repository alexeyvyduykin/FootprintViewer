using Mapsui.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace InteractivitySample.FeatureBuilders
{
    public class RouteBuilder : BaseFeatureBuilder
    {
        public override void Ending(Point worldPosition, Predicate<Point> isEnd)
        {
            throw new NotImplementedException();
        }

        public override void Hover(Point worldPosition)
        {
            throw new NotImplementedException();
        }

        public override void Moving(Point worldPosition)
        {
            throw new NotImplementedException();
        }

        public override void Starting(Point worldPosition)
        {
            throw new NotImplementedException();
        }
    }
}
