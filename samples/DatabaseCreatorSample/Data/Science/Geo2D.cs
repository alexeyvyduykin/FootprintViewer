using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseCreatorSample.Data
{
    public enum GeoCoordTypes
    {
        Radians,
        Degrees
    }

    public class Geo2D
    {
        public static Geo2D Empty = new Geo2D();

        private Geo2D()
        {
            Lon = Double.NaN;
            Lat = Double.NaN;
            Type = GeoCoordTypes.Radians;
        }

        public Geo2D(double lon, double lat, GeoCoordTypes type = GeoCoordTypes.Radians)
        {
            this.Lon = lon;
            this.Lat = lat;
            this.Type = type;
        }

        public Geo2D(Geo2D geo)
        {
            this.Lon = geo.Lon;
            this.Lat = geo.Lat;
            this.Type = geo.Type;
        }

        //public Geo2D LongitudeNormalize()
        //{
        //    Geo2D p = new Geo2D(Lon, Lat);
        //    while (p.Lon < 0.0)
        //        p.Lon += 2.0 * Math.PI;
        //    while (p.Lon > 2.0 * Math.PI)
        //        p.Lon -= 2.0 * Math.PI;

        //    return p;
        //}

        //public void Normalize()
        //{
        //    if (Type == GeoCoordTypes.Radians)
        //    {
        //        Lon = MyMath.WrapAngle(Lon);
        //    }
        //    else
        //    {
        //        Lon = MyMath.WrapAngle360(Lon);
        //    }
        //}

        //public static Geo2D Normalized(Geo2D position)
        //{
        //    Geo2D p = new Geo2D(position.Lon, position.Lat, position.Type);
        //    p.Normalize();
        //    return p;
        //}

        //public Geo2D Normalized()
        //{
        //    Geo2D p = new Geo2D(Lon, Lat, Type);
        //    p.Normalize();
        //    return p;
        //}

        public Geo2D ToRadians()
        {
            Geo2D point = new Geo2D(Lon, Lat, Type);
            point.Radians();
            return point;
        }

        public Geo2D ToDegrees()
        {
            Geo2D point = new Geo2D(Lon, Lat, Type);
            point.Degrees();
            return point;
        }

        private void Radians()
        {
            if (Type == GeoCoordTypes.Degrees)
            {
                Lon *= ScienceMath.DegreesToRadians;
                Lat *= ScienceMath.DegreesToRadians;
                this.Type = GeoCoordTypes.Radians;
            }
        }

        private void Degrees()
        {
            if (Type == GeoCoordTypes.Radians)
            {
                Lon *= ScienceMath.RadiansToDegrees;
                Lat *= ScienceMath.RadiansToDegrees;
                this.Type = GeoCoordTypes.Degrees;
            }
        }


        public static void OffsetLeftLon(Geo2D coord)
        {
            if (coord.Type == GeoCoordTypes.Radians)
            {
                coord.Lon -= Math.PI;
            }
            else
            {
                coord.Lon -= 180.0;
            }
        }

        public static void OffsetRightLon(Geo2D coord)
        {
            if (coord.Type == GeoCoordTypes.Radians)
            {
                coord.Lon += Math.PI;
            }
            else
            {
                coord.Lon += 180.0;
            }

        }

        public GeoCoordTypes Type { get; protected set; }

        public double Lon { get; protected set; }
        public double Lat { get; protected set; }
    }
}
