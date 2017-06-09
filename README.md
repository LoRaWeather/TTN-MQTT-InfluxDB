# TTN-MQTT-InfluxDB
This repository is for the .NET Core program that subscribes to the MQTT broker of The Things Network and saves all incoming data into InfluxDB. See the loraweather organization page on GitHub for more info, you can find the page [here](https://github.com/LoRaWeather).

### Program
The program is in the folder Program. The program will subscribe to the MQTT broker of The Things Network. Every messag that is send to The Things Network will be send to the broker. All subscribed program's will get that message. Once there is a message the program will decode the data and save it into InfluxDB. InfluxDB is a time series database. [Here](https://www.thethingsnetwork.org/docs/applications/mqtt/) you can find info about the MQTT broker of The Things Network.

### Doxygen
For this program the documentation is generated by Doxygen. This documentation can be found within the folder Doxygen. To view the documentation you have to open the index.html files. This will bring you to the homepage of the documentation.
