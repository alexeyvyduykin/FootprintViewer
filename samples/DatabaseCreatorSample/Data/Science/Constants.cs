using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseCreatorSample.Data
{


    public static class Constants
    {
        // WGS - World Geodetic System

        public const double Me = 5.98e24;                // [WGS-84] Mass of earth, kg
        //public const double G = 6.67e-11;                // [WGS-84] Gravitational constant, m3 kg–1 s-2
        public const double GM = 398600.4418;            // [WGS-84] Standard gravitational parameter ( G * Me ), km3 s–2 
                                                         // public const double Ea = 6378.137;              // [WGS-84] Earth Equatorial radius, km
                                                         // public const double Eb = 6356.7523;             // [WGS-84] Earth Polar radius, km
        public const double Re = 6371.0088;             // Earth mean radius, km
        public const double Einvf = 1 / 298.257223560;   // [WGS-84] Inverse Flattening Factor of the Earth, f = (a - b) / a

        /// <summary>                                                                     
        /// Earth nominal mean angular velocity, rad/s                                                         
        /// </summary>
        public const double Omega = 7.292115e-5;         // [WGS-84] Nominal Mean Angular Velocity, rad/s

        public const double DaySidereal = (23 * 3600) + (56 * 60) + 4.09;  // sec
        public const double DaySolar = (24 * 3600);   // sec

        public const double HoursPerDay = 24.0;        // Hours per day   (solar)
        public const double MinPerDay = 1440.0;        // Minutes per day (solar)
        public const double SecPerDay = 86400.0;       // Seconds per day (solar)
        public const double OmegaE = 1.00273790934;    // Earth rotation per sidereal day
    }


}
