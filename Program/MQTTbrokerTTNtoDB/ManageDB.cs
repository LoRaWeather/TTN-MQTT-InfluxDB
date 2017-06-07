using System;
using System.IO;
using System.Net.Http;

namespace MQTTbrokerTTNtoDB
{
    /// <summary>
    /// This class will manage the Influx database.
    /// Data can be send to the database.
    /// </summary>
    public class ManageDB
    {
        /// <summary>
        /// The url of the database.
        /// </summary>
        private string _urlDB;

        /// <summary>
        /// Http client to make request to the database.
        /// Requests like: post, put, get, etc.
        /// </summary>
        private HttpClient _client;

        /// <summary>
        /// The constructor of the class ManageDB.
        /// </summary>
        /// <param name="adressDB">Address of the database.</param>
        /// <param name="portDB">Port number of the database.</param>
        public ManageDB(string addressDB, string portDB)
        {
            _urlDB = string.Format("{0}:{1}/write?db=weatherdb", addressDB, portDB);
            _client = new HttpClient();
        }

        /// <summary>
        /// This function will create a string of the given sensor data.
        /// The string is a request parameter that will be passed into another function.
        /// </summary>
        /// <param name="sensorData">Object with all data of the sensor.</param>
        public void addSensorDataToDB(SensorData sensorData)
        {
            string data = string.Format("{0},sensor_id={1},battery={2},temperature={3},humidity={4},pressure={5} version={6}",
                "devicedata", sensorData.sensor_id, sensorData.battery, sensorData.temperature, sensorData.humidity, 
                sensorData.pressure, sensorData.version);
            StringContent requestContent = new StringContent(data);
            postDataToDB(requestContent);
        }

        /// <summary>
        /// This function will post the sensor data to the database via http post request.
        /// </summary>
        /// <param name="content">StringContent object with data as string inside.</param>
        private async void postDataToDB(StringContent content)
        {
            HttpResponseMessage response = await _client.PostAsync(_urlDB, content);
            HttpContent responseContent = response.Content;

            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                Console.WriteLine(await reader.ReadToEndAsync());
            }
        }
    }
}
