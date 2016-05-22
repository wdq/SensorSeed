# SensorSeed

This is C# code for a custom weather station I built. 

Parts list:

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
|                                                  |         |                                                                                                                                                               |                                                                                                                                               |
| Total:                                           | $310.75 |                                                                                                                                                               |                                                                                                                                               |