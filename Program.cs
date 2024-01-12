using SunsetSunrise;

var today = DateTime.UtcNow;

// Montreal coordinates
var latitude = 45.5019;
var longitude = -73.5674;

var dateTimeSunrise = SunsetSunriseAC1990.GetSunrise(today, latitude, longitude);
var dateTimeSunset = SunsetSunriseAC1990.GetSunset(today, latitude, longitude);

Console.WriteLine($"Sunrise: {dateTimeSunrise}");
Console.WriteLine($"Sunset: {dateTimeSunset}");