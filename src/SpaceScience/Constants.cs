namespace SpaceScience;

public static class Constants
{
    // WGS - World Geodetic System

    /// <summary>
    /// [WGS-84] Mass of earth, kg
    /// </summary>
    public const double Me = 5.98e24;

    //public const double G = 6.67e-11;                // [WGS-84] Gravitational constant, m3 kg–1 s-2

    /// <summary>
    /// [WGS-84] Standard gravitational parameter ( G * Me ), km3 s–2 
    /// </summary>
    public const double GM = 398600.4418;

    // public const double Ea = 6378.137;              // [WGS-84] Earth Equatorial radius, km                                                        
    // public const double Eb = 6356.7523;             // [WGS-84] Earth Polar radius, km

    /// <summary>
    /// Earth mean radius, km
    /// </summary>
    public const double Re = 6371.0088;

    /// <summary>
    /// [WGS-84] Inverse Flattening Factor of the Earth, f = (a - b) / a
    /// </summary>
    public const double Einvf = 1 / 298.257223560;

    /// <summary>                                                                     
    /// [WGS-84] Nominal Mean Angular Velocity, rad/s                                                      
    /// </summary>
    public const double Omega = 7.292115e-5;

    /// <summary>
    /// sec
    /// </summary>
    public const double DaySidereal = (23 * 3600) + (56 * 60) + 4.09;

    /// <summary>
    /// sec
    /// </summary>
    public const double DaySolar = (24 * 3600);

    /// <summary>
    /// Hours per day   (solar)
    /// </summary>
    public const double HoursPerDay = 24.0;

    /// <summary>
    /// Minutes per day (solar)
    /// </summary>
    public const double MinPerDay = 1440.0;

    /// <summary>
    /// Seconds per day (solar)
    /// </summary>
    public const double SecPerDay = 86400.0;

    /// <summary>
    /// Earth rotation per sidereal day
    /// </summary>
    public const double OmegaE = 1.00273790934;
}
