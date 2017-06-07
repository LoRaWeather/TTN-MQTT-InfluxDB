using M2Mqtt;
using M2Mqtt.Messages;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

/*! \mainpage Main Page
 *
 * \section intro_sec Introduction
 *
 * On this doxygen page you will find the documentation of the program MQTTbrokerTTNtoDB.
 * 
 * This project is to get messages from <a href="https://www.thethingsnetwork.org/">The Things Network</a> 
 * and send it to the databases.
 * 
 * This program will subscribe to the MQTT broker of The Things Network. Every time a device sends a message to the
 * created application (on The Things Network) it will be recognized by the broker. Than the message will be unpacked and
 * data will be decoded. The data will then be send to the databases. 
 *
 * 
 * \author Name: Maurice Markvoort <br />
 *         Email: m.markvoort@xablu.com <br />
 *         Company: <a href="https://www.xablu.com/">Xablu</a>
 * \date 12-4-2017
 * \version 1.0
 */

namespace MQTTbrokerTTNtoDB
{
    /// <summary>
    /// Main class of the program.
    /// In this class the mqtt broker will created.
    /// </summary>
    class Program
    {
        private static readonly int negativetemp = 0b1000000000000000;

        /// <summary>
        /// Variable that manage the Influx database.
        /// </summary>
        private static ManageDB manageDB;

        /// <summary>
        /// Variable that manage MySql database.
        /// </summary>
        private static MySqlConnection connection;

        /// <summary>
        /// This function is the start of the program.
        /// It will create connections to the database.
        /// After that it will connect to the mqtt broker of The Things Network.
        /// Once connected it will subscribe to an event.
        /// 
        /// Every time data is uploaded to The Things Network, to this application-id, that data is send to de databases.
        /// </summary>
        private static void Main(string[] args)
        {
            connection = new MySqlConnection
            {
                ConnectionString = "[User database]"
            };
            connection.Open();
            var cultureInfo = new CultureInfo("en-US");

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            manageDB = new ManageDB("[InfluxDB link]", "8086");

            try
            {
                var client = new MqttClient("eu.thethings.network");

                client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

                var clientId = Guid.NewGuid().ToString();

                var subscriptionId = client.Subscribe(new string[] { "#" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

                client.ConnectionClosed += Client_ConnectionClosed;

                client.MqttMsgSubscribed += Client_MqttMsgSubscribed;

                var connectionId = client.Connect(
                      clientId,
                      "[application ID]",
                      "[application Key]");
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Exception main: {0}", ex.Message));
            }
            
        }

        /// <summary>
        /// This function is called when a message is recieved on the mqtt broker.
        /// It wil than pass important data to function that will decode it to readable data.
        /// Data that will be send: payload_raw, hardware_serial(device_eui) and the time of receiving.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The object that includes the message.</param>
        private static void Client_MqttMsgPublishReceived(
                        object sender, MqttMsgPublishEventArgs e)
        {
            var jsonText = Encoding.ASCII.GetString(e.Message);
            Console.WriteLine(jsonText + "\n");
            var message = JsonConvert.DeserializeObject<Telementary>(jsonText);

            if (message.payload_raw != null)
            {
                DecodeSensorData(message.payload_raw, message.hardware_serial, message.metadata.gateways[0].time);
            }
        }

        /// <summary>
        /// With this function the raw data will be decoded.
        /// After that the data will be passed to another function which sends the data to the databases.
        /// </summary>
        /// <param name="EncodedData">The raw payload.</param>
        /// <param name="macAddress">Mac address of the device.</param>
        /// <param name="time">Time data has been send.</param>
        public static void DecodeSensorData(string EncodedData, string macAddress, DateTime time)
        {
            byte[] hexData = Convert.FromBase64String(EncodedData);
            byte[] hexTemperatue = { hexData[0], hexData[1] };
            byte[] hexPressure = { hexData[2], hexData[3] };

            Array.Reverse(hexTemperatue);
            Array.Reverse(hexPressure);
            int temperature = BitConverter.ToInt16(hexTemperatue, 0);
            int pressure = BitConverter.ToInt16(hexPressure, 0);
            int humidity = hexData[4];
            int ver_bat = hexData[5];

            if ((temperature & negativetemp) == negativetemp)
                temperature = -(temperature - negativetemp);

            SensorData tempsensordata = new SensorData()
            {
                version = ver_bat & 0b00001111,
                sensor_id = Convert.ToUInt32(getDeviceId(macAddress).Result),
                battery = (ver_bat >> 4) & 1,
                temperature = (Convert.ToDouble(temperature) / 100),
                pressure = (Convert.ToDouble(pressure) / 10),
                humidity = humidity
            };
            manageDB.addSensorDataToDB(tempsensordata);
        }

        /// <summary>
        /// With this function the device-id wil be returned that is connected to the given mac address.
        /// </summary>
        /// <param name="macAddress">Mac address of the device.</param>
        /// <returns>id of the device within the database.</returns>
        private static async Task<uint> getDeviceId(string macAddress)
        {
            var cmd = connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT id FROM devices WHERE mac = '" + macAddress + "'";
            DbDataReader reader = await cmd.ExecuteReaderAsync();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    return await reader.GetFieldValueAsync<uint>(0);
                }
            }
            return 0;
        }


        /// <summary>
        /// This function will simple print the message from the mqtt broker once there is a message.
        /// </summary>
        private static void Client_MqttMsgSubscribed(
             object sender, MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine("Client_MqttMsgSubscribed: " + e.ToString() + "\n");
        }

        /// <summary>
        /// When the connection to the broker closed this function will print a message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Client_ConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine("Client_ConnectionClosed: " + e.ToString());
        }
    }
}