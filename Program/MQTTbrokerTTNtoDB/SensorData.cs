namespace MQTTbrokerTTNtoDB
{
    /// <summary>
    /// Object for all the important data of the sensor.
    /// </summary>
    public class SensorData
    {
        /// <summary>
        /// Version of the program.
        /// </summary>
        public int version;

        /// <summary>
        /// The sensor_id of the sensor.
        /// </summary>
        public uint sensor_id;

        /// <summary>
        /// If value = 1 the batter is below 25%.
        /// </summary>
        public int battery;

        /// <summary>
        /// Temperature data.
        /// </summary>
        public double temperature;

        /// <summary>
        /// Humidity data.
        /// </summary>
        public int humidity;

        /// <summary>
        /// Pressure data.
        /// </summary>
        public double pressure;

        /// <summary>
        /// Methode to print all data of the sensor.
        /// </summary>
        /// <returns>String with all data of the sensor.</returns>
        public string toString()
        {
            return string.Format("Version: {0} \nTemperature: {1} C\nHumidity: {2}%\nPressure: {3} pHa\n", version, temperature, humidity, pressure);
        }
    }
}
