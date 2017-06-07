﻿using System;

namespace MQTTbrokerTTNtoDB
{
    /// <summary>
    /// This is generated by visual studio.
    /// It represents the given Json object that is send by The Things Network.
    /// Not all data is important to know but required in the class.
    /// </summary>
    public class Telementary
    {
        public string app_id { get; set; }
        public string dev_id { get; set; }
        public string hardware_serial { get; set; }
        public int port { get; set; }
        public int counter { get; set; }
        public string payload_raw { get; set; }
        public Metadata metadata { get; set; }
    }

    /// <summary>
    /// This is generated by visual studio.
    /// It represents the given Json object that is send by The Things Network.
    /// Not all data is important to know but required in the class.
    /// </summary>
    public class Metadata
    {
        public string time { get; set; }
        public float frequency { get; set; }
        public string modulation { get; set; }
        public string data_rate { get; set; }
        public string coding_rate { get; set; }
        public Gateway[] gateways { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    /// <summary>
    /// This is generated by visual studio.
    /// It represents the given Json object that is send by The Things Network.
    /// Not all data is important to know but required in the class.
    /// </summary>
    public class Gateway
    {
        public string gtw_id { get; set; }
        public long timestamp { get; set; }
        public DateTime time { get; set; }
        public int channel { get; set; }
        public int rssi { get; set; }
        public float snr { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int altitude { get; set; }
    }
}