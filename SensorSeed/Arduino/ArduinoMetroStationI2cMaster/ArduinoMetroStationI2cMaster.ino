#include <TinyWireM.h>
#include <USI_TWI_Master.h>
#include <Wire.h>
#include <RFM69.h>
#include <SPI.h>
#include <Adafruit_SHT31.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BMP085_U.h>
#include <DHT.h>
#include <DHT_U.h>
#include <stdlib.h>

// Settings for the Adafruit RFM69HCW radio
#define NETWORKID     81
#define NODEID        2
#define RECEIVER      1
#define FREQUENCY     RF69_915MHZ
#define ENCRYPTKEY    "eq^YkQ3n7HvC#9c$"
#define IS_RFM69HCW   true

// Settings for the DHT22
#define DHTPIN            6
#define DHTTYPE           DHT22

// Pins for the Adafruit RFM69HCW radio
#define RFM69_CS      10
#define RFM69_IRQ     2
#define RFM69_IRQN    0  // IRQ 0
#define RFM69_RST     9

#define LED           13

#define SERIAL_BAUD   9600
#define TRANSMIT_INTERVAL 150000 // in milliseconds, this is set to 2.5 minutes

int16_t packetnum = 0;

// Setup some libraries for devices
RFM69 radio = RFM69(RFM69_CS, RFM69_IRQ, IS_RFM69HCW, RFM69_IRQN);
Adafruit_SHT31 sht31 = Adafruit_SHT31();
Adafruit_BMP085_Unified bmp180 = Adafruit_BMP085_Unified(10085);
DHT_Unified dht(DHTPIN, DHTTYPE);

void setupRadio() {
  // Hard Reset the RFM module
  pinMode(RFM69_RST, OUTPUT);
  digitalWrite(RFM69_RST, HIGH);
  delay(100);
  digitalWrite(RFM69_RST, LOW);
  delay(100);  
  // Initialize radio
  radio.initialize(FREQUENCY,NODEID,NETWORKID);
  if (IS_RFM69HCW) {
    radio.setHighPower();    // Only for RFM69HCW & HW!
  }
  radio.setPowerLevel(31); // power output ranges from 0 (5dBm) to 31 (20dBm)
  
  radio.encrypt(ENCRYPTKEY);
  
  pinMode(LED, OUTPUT);
  Serial.print("\nTransmitting at ");
  Serial.print(FREQUENCY==RF69_433MHZ ? 433 : FREQUENCY==RF69_868MHZ ? 868 : 915);
  Serial.println(" MHz");  
}


void initializeSensors() {
  sht31.begin(0x44);
  bmp180.begin();
}

void setup() {
  Serial.begin(SERIAL_BAUD);
  Serial.println("Starting Arduino Metro weather station I2C master, radio transmitter...");
  
  setupRadio();
  initializeSensors();

    Serial.println("Setup complete");
}

void loop() {
    delay(TRANSMIT_INTERVAL); // Transmit at an interval

  String data = String();

  //char temperatureSHT31[7]; // 7 max chars (based on -999.99 to 999.99)
  String temperatureSHT31 = String(getTemperatureSHT31());
  String humiditySHT32 = String(getHumiditySHT31());
  String pressureBMP180 = String(getPressureBMP180());
  String altitudeBMP180 = String(getAltitudeBMP180());
  String windSpeedI2C = String(getWindSpeedI2C());
  String gustSpeedI2C = String(getGustSpeedI2C());
  String rainI2C = String(getRainI2C());
  String batteryI2C = String(getBatteryI2C());
  String solarI2C = String(getSolarI2C());
  String directionI2C = String(getDirectionI2C());
  String temperatureBMP180 = String(getTemperatureBMP180());
  String temperatureDHT22 = String(getTemperatureDHT22());
  String humidityDHT22 = String(getHumidityDHT22());

  data += temperatureSHT31;
  data += String(",");
  data += humiditySHT32;
  data += String(",");  
  data += pressureBMP180;
    data += String(",");
  data += altitudeBMP180;
    data += String(",");
  data += windSpeedI2C;
    data += String(",");
  data += gustSpeedI2C;
    data += String(",");
  data += rainI2C;
    data += String(",");
  data += batteryI2C;
    data += String(",");
  data += solarI2C;
    data += String(",");
  data += directionI2C;
    data += String(",");
  data += temperatureBMP180;
    data += String(",");
  data += temperatureDHT22;
    data += String(",");
  data += humidityDHT22;


  // Total data bytes: 7 + 6 + 9 + 9 + 10 + 10 + 10 + 10 + 10 + 10 + 7 + 7 + 6 = 111
  // Total separation comma bytes: 12
  // Total combined bytes: 111 + 12 = 123
  // Round that up with some extra spare bits: 150? 

  /*char radiopacket[151];
  radiopacket[0] = '\0';
  strcat(radiopacket, temperatureSHT31);
  strcat(radiopacket, ",");
  strcat(radiopacket, humiditySHT32);
  strcat(radiopacket, ",");
  strcat(radiopacket, pressureBMP180);
  strcat(radiopacket, ",");
  strcat(radiopacket, altitudeBMP180);
  strcat(radiopacket, ",");
  strcat(radiopacket, windSpeedI2C);
  strcat(radiopacket, ",");
  strcat(radiopacket, gustSpeedI2C);
  strcat(radiopacket, ",");
  strcat(radiopacket, rainI2C);
  strcat(radiopacket, ",");
  strcat(radiopacket, batteryI2C);
  strcat(radiopacket, ",");
  strcat(radiopacket, solarI2C);
  strcat(radiopacket, ",");
  strcat(radiopacket, directionI2C);
  strcat(radiopacket, ",");
  strcat(radiopacket, temperatureBMP180);
  strcat(radiopacket, ",");
  strcat(radiopacket, temperatureDHT22);
  strcat(radiopacket, ",");
  strcat(radiopacket, humidityDHT22);

  */

//Serial.println(data);
  char radiopacket[200];
  data.toCharArray(radiopacket, 200);
  //itoa(packetnum++, radiopacket+13, 10);
  //Serial.print("Sending "); Serial.println(radiopacket);
  Serial.println("Sending");
  if (radio.sendWithRetry(RECEIVER, radiopacket, strlen(radiopacket))) { //target node Id, message as string or byte array, message length
    Serial.println("OK");
  }
 
  radio.receiveDone(); //put radio in RX mode
  Serial.flush(); //make sure all serial data is clocked out before sleeping the MCU
}




float getTemperatureSHT31() {
  float temperature = sht31.readTemperature();
  return temperature;
}

float getHumiditySHT31() {
  float humidity = sht31.readHumidity();
  return humidity;
}

float getPressureBMP180() {
  sensors_event_t event;
  bmp180.getEvent(&event);
  return event.pressure;
}

float getAltitudeBMP180() {
  sensors_event_t event;
  bmp180.getEvent(&event);
  float seaLevelPressure = SENSORS_PRESSURE_SEALEVELHPA;
  return bmp180.pressureToAltitude(seaLevelPressure, event.pressure);
}

String getWindSpeedI2C() {
  Wire.beginTransmission(6); // transmit to device #2
  Wire.write(1);
  Wire.endTransmission();
  delay(25);
  int i = 0;
  char temp1[10];
  Wire.requestFrom(6, 10);    // request 4 bytes from slave device #2
  while (Wire.available()) { // slave may send less than requested
    temp1[i] = Wire.read(); // receive a byte as character
    i++;
    //Serial.print(c);         // print the character
  }
  String data = String(temp1);
  data.trim();
  delay(5);
  return data;
}

String getGustSpeedI2C() {
  Wire.beginTransmission(6); // transmit to device #2
  Wire.write(2);
  Wire.endTransmission();
  delay(25);
  int i = 0;
  char temp1[10];
  Wire.requestFrom(6, 10);    // request 4 bytes from slave device #2
  while (Wire.available()) { // slave may send less than requested
    temp1[i] = Wire.read(); // receive a byte as character
    i++;
    //Serial.print(c);         // print the character
  }
  String data = String(temp1);
  data.trim();
  delay(5);
  return data;
}

String getRainI2C() {
  Wire.beginTransmission(6); // transmit to device #2
  Wire.write(3);
  Wire.endTransmission();
  delay(25);
  int i = 0;
  char temp1[10];
  Wire.requestFrom(6, 10);    // request 4 bytes from slave device #2
  while (Wire.available()) { // slave may send less than requested
    temp1[i] = Wire.read(); // receive a byte as character
    i++;
    //Serial.print(c);         // print the character
  }
  String data = String(temp1);
  data.trim();
  delay(5);
  return data;
}

String getBatteryI2C() {
  Wire.beginTransmission(6); // transmit to device #2
  Wire.write(6);
  Wire.endTransmission();
  delay(25);
  int i = 0;
  char temp1[10];
  Wire.requestFrom(6, 10);    // request 4 bytes from slave device #2
  while (Wire.available()) { // slave may send less than requested
    temp1[i] = Wire.read(); // receive a byte as character
    i++;
    //Serial.print(c);         // print the character
  }
  String data = String(temp1);
  data.trim();
  delay(5);
  return data;
}

String getSolarI2C() {
  Wire.beginTransmission(6); // transmit to device #2
  Wire.write(5);
  Wire.endTransmission();
  delay(25);
  int i = 0;
  char temp1[10];
  Wire.requestFrom(6, 10);    // request 4 bytes from slave device #2
  while (Wire.available()) { // slave may send less than requested
    temp1[i] = Wire.read(); // receive a byte as character
    i++;
    //Serial.print(c);         // print the character
  }
  String data = String(temp1);
  data.trim();
  delay(5);
  return data;
}

String getDirectionI2C() {
  Wire.beginTransmission(6); // transmit to device #2
  Wire.write(4);
  Wire.endTransmission();
  delay(25);
  int i = 0;
  char temp1[10];
  Wire.requestFrom(6, 10);    // request 4 bytes from slave device #2
  while (Wire.available()) { // slave may send less than requested
    temp1[i] = Wire.read(); // receive a byte as character
    i++;
    //Serial.print(c);         // print the character
  }
  String data = String(temp1);
  data.trim();
  delay(5);
  return data;
}

float getTemperatureBMP180() {
  sensors_event_t event;
  bmp180.getEvent(&event);
  float temperature;
  bmp180.getTemperature(&temperature);
  return temperature;
}

float getTemperatureDHT22() {
  sensors_event_t event;
  dht.temperature().getEvent(&event);
  return event.temperature;
}

float getHumidityDHT22() {
  sensors_event_t event;
  dht.humidity().getEvent(&event);
  return event.relative_humidity;
}



