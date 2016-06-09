# SensorSeed

This is C# code for a custom weather station I built.

There is an outdoor component to the station that collects temperature, humidity, pressure, wind speed, wind direction, and rain every five minutes. There are four small indoor devices that collect just temperature and humidity data in four rooms every five minutes.



Outdoor weather station parts list:

| Item                                             | Price   | Description                                                                                                                                                   | Link                                                                                                                                          |
|--------------------------------------------------|---------|---------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------|
| Adafruit Feather M0 WiFi - ATSAMD21 + ATWINC1500 | $34.95  | Arduino compatible microcontroller with WiFi to communicate with SQL server.                                                                                  | https://www.adafruit.com/product/3010                                                                                                         |
| Sparkfun Weather Meters                          | $76.95  | Wind vane, cup anemometer, tipping bucket rain gauge to measure wind and rain.                                                                                | https://www.sparkfun.com/products/8942                                                                                                        |
| Adafruit Sensiron SHT32-D                        | $13.95  | Temperature and humidity I2C sensor.                                                                                                                          | https://www.adafruit.com/product/2857                                                                                                         |
| BMP180                                           | $9.95   | Barometric pressure, temperature, and altitude sensor.                                                                                                        | https://www.adafruit.com/product/1603                                                                                                         |
| Arduino Uno                                      | $24.95  | Microcontroller that processes Sparkfun Weather Meters and interfaces with the Adafruit Feather over I2C as an I2C slave.                                     | https://www.adafruit.com/products/50                                                                                                          |
| 4V 1A Lithium Battery Charger                    | $5      | Charge the batteries using the solar panels.                                                                                                                  | http://www.ebay.com/itm/10PCS-5V-Mini-USB-1A-Lithium-Battery-Charging-Board-Charger-Module-IN-4-5V-5-5v-/221921100545                         |
| 2x 2.5W/5V/500mA Solar Panels                    | $20     | Charge the batteries, two in parallel provide 5V/1A which is the maximum the charger supports. More current and a better charger may be needed in the winter. | http://www.amazon.com/ALLPOWERS-Encapsulated-Battery-Charger-130x150mm/dp/B00CBT8A14                                                          |
| 12x 18650 Batteries                              | $60     | Power the station, I got mine from old laptop batteries.                                                                                                      | http://www.amazon.com/Samsung-INR18650-25R-18650-Rechargeable-Batteries/dp/B00NUI46HM/ref=sr_1_5?ie=UTF8&qid=1463718225&sr=8-5&keywords=18650 |
| Ambient Weather SRS100LX                         | $40     | Solar radiation shield to get accurate temperature and humidity measurements.                                                                                 | http://www.amazon.com/Ambient-Weather-SRS100LX-Temperature-Radiation/dp/B003EB3GE4?ie=UTF8&psc=1&redirect=true&ref_=oh_aui_detailpage_o06_s00 |
| BUD Industries NBF-32018                         | $25     | Sealed, weather proof, enclosure for the microcontrollers, battery, and other electronics.                                                                    | http://www.amazon.com/BUD-Industries-NBF-32018-Plastic-Economy/dp/B005T990I0?ie=UTF8&psc=1&redirect=true&ref_=oh_aui_detailpage_o00_s00       |
| Adafruit TSL2561                         | $6     | Luminosity/Lux/Light sensor.                                                                    | https://www.adafruit.com/products/439       |
| Adafruit VEML6070                         | $6     | UV index sensor.                                                                    | https://www.adafruit.com/products/2899      |
| Photo transistor                         | $1     | Photo transistor, will be used to measure solar irradiance/flux.                                                                    | https://www.adafruit.com/products/2831      |
|                                                  |         |                                                                                                                                                               |                                                                                                                                               |
| Total:                                           | $323.75 |                                                                                                                                                               |                                                                                
|
Indoor weather station parts list:

| Item                | Price | Description                                                                           | Link                                                                                                                                                                    |
|---------------------|-------|---------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| 5x DHT11            | $9    | Temperature/humidity sensor.                                                          | http://www.ebay.com/itm/5X-DHT11-Temperature-and-Relative-Humidity-Sensor-Module-for-arduino-/321972401029?hash=item4af70cf385:g:rCUAAOSwaA5WkrJA                       |
| 5x ESP8266          | $12   | Arduino compatible microcontroller with WiFi support.                                 | http://www.ebay.com/itm/5pcs-ESP8266-Serial-WIFI-Wireless-Transceiver-Module-WIFI-ESP-01-Support-AP-STA-/291362195261?hash=item43d68a473d:g:r1cAAOSwstxVFSb9            |
| 5x LD1117V33        | $3    | Linear voltage regulator to convert 5V USB power to 3V power for the microcontroller. | http://www.ebay.com/itm/5PCS-LD1117V33-Linear-Voltage-Regulator-800mA-3-3V-TO-220-/181941289370?hash=item2a5c8bc19a:g:pN4AAOSwv-NWVDIz                                  |
| 4x USB Wall Charger | $12   | Used to power the stations.                                                           | http://www.ebay.com/itm/2A-Fast-Wall-Charger-USB-Data-Cable-For-Samsung-Galaxy-S6-S6-Edge-Note-5-White-/272094650576?var=&hash=item3f5a1ae0d0:m:mC8CwZiIfjTshipOqjTuGcQ |
|                     |       |                                                                                       |                                                                                                                                                                         |
| Total:              | $46   |                                                                                       |                                                                                                                                                                         |
