using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Model
{
    public class Forecast
    {
        public string Sensor { get; set; }
        public DateTime Date { get; set; }
        public double AverageWindSpeed { get; set; }
        public double AverageWindDirection{ get; set; }

        public Forecast(string sensor, double averageWindSpeed, double averageWindDirection)
        {
            Sensor = sensor;
            Date = DateTime.Now;
            AverageWindSpeed = averageWindSpeed;
            AverageWindDirection = averageWindDirection;
        }

        public override string ToString()
        {
            return "{\n" +
                $"\"sensor\": \"{Sensor}\",\n" +
                $"\"date\": \"{Date}\",\n" +
                $"\"averageWindSpeed\": {AverageWindSpeed.ToString("#.##", CultureInfo.InvariantCulture)},\n" +
                $"\"averageWingDirection\": {AverageWindDirection.ToString("#.##", CultureInfo.InvariantCulture)}\n" +
                "}";
        }
    }
}
