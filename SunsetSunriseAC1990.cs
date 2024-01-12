namespace SunsetSunrise
{
    /// <summary>
    /// Implementation of the algorithm detailed in the	Almanac for Computers, 
    /// published in 1990 by Nautical Almanac Office,
    /// United States Naval Observatory, Washington, DC 20392.
    /// </summary>
    public static class SunsetSunriseAC1990
    {
        // Constants
        private const double DegToRad = Math.PI / 180;
        private const double RadToDeg = 180 / Math.PI;

        // The official zenith angle for sunrise and sunset
        private const double OfficialZenith = 90 + 50 / 60.0;

        // The coefficients for calculating the sun's mean anomaly
        private const double MeanAnomalyCoefficient1 = 0.9856;
        private const double MeanAnomalyCoefficient2 = 3.289;

        // The coefficients for calculating the sun's true longitude
        private const double TrueLongitudeCoefficient1 = 1.916;
        private const double TrueLongitudeCoefficient2 = 0.020;
        private const double TrueLongitudeCoefficient3 = 282.634;

        // The coefficient for calculating the sun's right ascension
        private const double RightAscensionCoefficient = 0.91764;

        // The coefficient for calculating the sun's declination
        private const double DeclinationCoefficient = 0.39782;

        // The coefficients for calculating the sun's local hour angle
        private const double LocalHourAngleCoefficient1 = 0.06571;
        private const double LocalHourAngleCoefficient2 = 6.622;

        /// <summary>
        /// Calculates the sunrise in local time given a datetime object in Coordinated Universal Time (UTC) and geocoordinates.
        /// </summary>
        /// <param name="date">Datetime object in Coordinated Universal Time (UTC).</param>
        /// <param name="latitude">Latitude geocoordinate.</param>
        /// <param name="longitude">Longitude geocoordinate.</param>
        /// <returns></returns>
        public static DateTime? GetSunrise(DateTime date, double latitude, double longitude)
        {
            return GetSunTime(date, latitude, longitude, isSunrise: true)?.ToLocalTime();
        }

        /// <summary>
        /// Calculates the sunset in local time given a datetime object in Coordinated Universal Time (UTC) and geocoordinates.
        /// </summary>
        /// <param name="date">Datetime object in Coordinated Universal Time (UTC).</param>
        /// <param name="latitude">Latitude geocoordinate.</param>
        /// <param name="longitude">Longitude geocoordinate.</param>
        /// <returns></returns>
        public static DateTime? GetSunset(DateTime date, double latitude, double longitude)
        {
            return GetSunTime(date, latitude, longitude, isSunrise: false)?.ToLocalTime();
        }

        private static DateTime? GetSunTime(DateTime date, double latitude, double longitude, bool isSunrise)
        {
            // Convert the longitude to hour value and calculate an approximate time
            double lngHour = longitude / 15;
            double t = isSunrise ? date.DayOfYear + (6 - lngHour) / 24 : date.DayOfYear + (18 - lngHour) / 24;

            // Calculate the sun's mean anomaly
            double M = MeanAnomalyCoefficient1 * t - MeanAnomalyCoefficient2;

            // Calculate the sun's true longitude
            double L = M + TrueLongitudeCoefficient1 * Math.Sin(DegToRad * M) + TrueLongitudeCoefficient2 * Math.Sin(DegToRad * 2 * M) + TrueLongitudeCoefficient3;
            L %= 360;

            // Calculate the sun's right ascension
            double RA = RadToDeg * Math.Atan(RightAscensionCoefficient * Math.Tan(DegToRad * L));
            RA %= 360;

            // Adjust the right ascension to the same quadrant as L
            double Lquadrant = Math.Floor(L / 90) * 90;
            double RAquadrant = Math.Floor(RA / 90) * 90;
            RA += Lquadrant - RAquadrant;

            // Convert the right ascension to hours
            RA /= 15;

            // Calculate the sun's declination
            double sinDec = DeclinationCoefficient * Math.Sin(DegToRad * L);
            double cosDec = Math.Cos(Math.Asin(sinDec));

            // Calculate the sun's local hour angle
            double cosH = (Math.Cos(DegToRad * OfficialZenith) - sinDec * Math.Sin(DegToRad * latitude)) / (cosDec * Math.Cos(DegToRad * latitude));

            // Check if the sun never rises or sets on this date
            if (cosH > 1 && isSunrise)
            {
                // The sun never rises
                return null;
            }
            else if (cosH < -1 && !isSunrise)
            {
                // The sun never sets
                return null;
            }

            // Finish calculating H and convert into hours
            double H = isSunrise ? 360 - RadToDeg * Math.Acos(cosH) : RadToDeg * Math.Acos(cosH);
            H /= 15;

            // Calculate local mean time of rising or setting
            double T = H + RA - LocalHourAngleCoefficient1 * t - LocalHourAngleCoefficient2;

            // Adjust back to UTC
            double UT = T - lngHour;
            UT %= 24;

            // Convert UT value to local time zone of latitude and longitude
            return date.Date.AddHours(UT);
        }
    }
}