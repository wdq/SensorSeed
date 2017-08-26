# SensorSeed

![Weather station wind and rain sensors](https://raw.githubusercontent.com/wdq/SensorSeed/master/images/install-1.jpg)

This is everything relating to my weather station project, including C# website code, AVR code for the hardware, PCB layouts, and schematics.

There is an outdoor component to the station that collects temperature, humidity, pressure, wind speed, wind direction, and rain every two and a half minutes, I might bump this up to every minute in the future. Data is pushed over the air using a 100mW 900MHz radio to a Particle Photon inside my house. The Photon is connected to WiFi and uploads the data to the website. 

You can view the data collected from the weather station on [my website](sensorseed.quade.co/HomeOutsideWeatherStation/Home), and on the [Weather Underground website](https://www.wunderground.com/personal-weather-station/dashboard?ID=KNELINCO88)

![Weather station hardware](https://raw.githubusercontent.com/wdq/SensorSeed/master/images/install-2.jpg)

Parts list: 

| Item                              | Price | Description                                                                                                            |
|-----------------------------------|-------|------------------------------------------------------------------------------------------------------------------------|
| Outdoor PCB                       | $15   | The PCB for the outside part of the weather station. I got them made at Elecrow and the price shipped was about $15.   |
| Indoor PCB                        | $15   | The PCB for the indoor part of the weather station. I got them made at Elecrow and the price shipped was about $15.    |
| ATmega328p                        | $2    | Main microcontroller for the outside station. PDIP package.                                                            |
| 2x ATtiny44/88                    | $4    | I2C slaves that keep track of wind speed and rain outside. PDIP package.                                               |
| 2x Adafruit RFM69HCW              | $20   | Radios to handle communication between indoor and outdoor device. I use the 900MHz version with the nine pin breakout. |
| Particle Photon                   | $20   | The microcontroller for the indoor station.                                                                            |
| 2x 900MHz antennas                | $10   | For the radios. I used right angle SMA ones that Adafruit sells.                                                       |
| 2x SMA jacks                      | $5    | Edge mount SMA jacks for 1.6mm PCBs for the radios. Adafruit sells these.                                              |
| 6x 220 ohm resistors              | $0.60 | Through hole package. These are for LEDs, so you can use different sizes based on how bright you want the LEDs to be.  |
| 6x LEDs                           | $1    | Through hole 5mm LEDs. Any color.                                                                                      |
| 2x 20pF capacitors                | $0.20 | Through hole package, ceramic. May be different depending on the crystal oscillator you pick.                          |
| 6x 0.1uF capacitors               | $0.60 | Through hole package, ceramic. For the AVR decoupling.                                                                 |
| 8MHz crystal                      | $0.30 | An 8MHz quartz crystal oscillator. You can use other frequencies if you want.                                          |
| 2x 1k ohm resistors               | $0.20 | For voltage divider. Through hole.                                                                                     |
| 3x 2k ohm resistors               | $0.30 | For voltage divider. Through hole.                                                                                     |
| 10k ohm resistor                  | $0.10 | For the wind vane. Through hole.                                                                                       |
| 4.7k ohm resistor                 | $0.10 | Pullup resistor for the radio. Through hole.                                                                           |
| 2x 10uF capacitors                | $0.20 | Bypass capacitors, through hole, one per board, electrolytic.                                                          |
| 6x jumpers                        | $1.00 | Jumpers to turn on/off the LEDs for debugging                                                                          |
| Pin headers                       | $1    | Used throughout both boards                                                                                            |
| Pin header sockets                | $1    | Used throughout both boards                                                                                            |
| 3x 28 pin DIP sockets             | $1    | Used to make the AVR chips removable.                                                                                  |
|  Sparkfun weather meters          | $77   | There are lots of distributors. I got mine on Amazon. Has the rain gauge, anemometer, and wind vane.                   |
| Weatherproof box                  | $30   | A box to hold the outside station.                                                                                     |
| Solar radiation shield            | $40   | Very important for accurate temperature and humidity readings.                                                         |
| 35W solar panel                   | $40   | A panel to charge the battery, bigger is generally better.                                                             |
| 12V 18Ah lead acid battery        | $40   | To run the outside station. Lead acid charges at lower temperatures than lithium ion. Bigger is generally better.      |
| Lead acid solar charge controller | $20   | To charge the battery.                                                                                                 |
| Adafruit SHT31-D                  | $14   | Temperature/humidity sensor.                                                                                           |
| DHT22                             | $4    | Temperature/humidity sensor.                                                                                           |
| BMP180                            | $4    | Pressure/Temperature/Altitude sensor.                                                                                  |
| Silica Gel packets                            | $10    | To help keep the outdoor station dry inside the box                                                                                  |
|                                   |       |                                                                                                                        |
| Total:                            | $378  |                                              

I recommend buying extras for many of the cheaper electronic parts. That way you can have a replacement board if something goes wrong, and generally buying in bulk is cheaper anyways.

![Outdoor board](https://raw.githubusercontent.com/wdq/SensorSeed/master/images/outdoor-v1-1.jpg)
![Indoor board](https://raw.githubusercontent.com/wdq/SensorSeed/master/images/indoor-v1-1.jpg)

You can find more images of this station [here](https://github.com/wdq/SensorSeed/tree/master/images).

