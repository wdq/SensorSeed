#!/bin/bash

timestamp="$(date)"
echo "Outside Home Weather Station: $timestamp"

# get temperature and humidity
/root/Adafruit-sht31-for-PI/sht31-d p

# get temperature180, pressure, altitude
/root/bmp180/src/example

# get wind
wind="$(python /root/arduino/arduino.py 1)"
echo "wind: $wind"

# get gust
gust="$(python /root/arduino/arduino.py 2)"
echo "gust: $gust"

# get rain
rain="$(python /root/arduino/arduino.py 3)"
echo "rain: $rain"

# get battery
battery="$(python /root/arduino/arduino.py 6)"
echo "battery: $battery"

# get solar
solar="$(python /root/arduino/arduino.py 5)"
echo "solar: $solar"

# get direction
direction="$(python /root/arduino/arduino.py 4)"
echo "direction: $direction"

# light sensors, even though they aren't hooked up
echo "veml6070: 0"
echo "lux: 0"

# get temperaturedht22 and humiditydht22
python /root/Adafruit_Python_DHT/examples/simpletest.py
