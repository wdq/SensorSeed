// outside
// This code is for an ATmega328p I2C master to collect data from a bunch of sensors.

#include <Wire.h>
#include <RFM69.h>
#include <SPI.h>
#include <Adafruit_SHT31.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BMP085_U.h>
#include <DHT.h>
#include <DHT_U.h>
#include <stdlib.h>

#define NETWORKID                               81
#define NODEID                                  2
#define RECEIVER                                1
#define FREQUENCY                               RF69_915MHZ
#define ENCRYPTKEY                              "wquadeEncryptKey"
#define IS_RFM69HCW                             true
#define TRANSMIT_INTERVAL                       90000 // in milliseconds, this is set to 1.5 minutes
#define TIME_BETWEEN_PACKETS                    15000 // 15 seconds

// I2C slaves
#define ANEMOMETER_I2C_ADDRESS                  0x06
#define RAINGAUGE_I2C_ADDRESS                   0x07
#define WIND_FACTOR                             2.4
// IO pins
#define LED_PIN                                 14
#define VANE_PIN                                A1
#define VANE_EN_PIN                             A0
#define SOLAR_PIN                               A3
#define BATTERY_PIN                             A2
#define SERIAL_BAUD                             9600                             
// DHT22
#define DHTPIN                                  7
#define DHTTYPE                                 DHT22
// RFM69HCW
#define RFM69_CS                                4
#define RFM69_IRQ                               3
#define RFM69_IRQN                              1
#define RFM69_RST                               2


int16_t packetnum = 0;
unsigned long previousTransmitMillis = millis();
unsigned long previousPacketSentMillis = millis();
unsigned long previousWindSpeedRetrieveMillis = millis();
bool transmitting = false;

int state = 0;
char radiopacket0[61];
char radiopacket1[61];
char radiopacket2[61];
char radiopacket3[61];
char radiopacket4[61];

RFM69 radio = RFM69(RFM69_CS, RFM69_IRQ, IS_RFM69HCW, RFM69_IRQN);
Adafruit_SHT31 sht31 = Adafruit_SHT31();
Adafruit_BMP085_Unified bmp180 = Adafruit_BMP085_Unified(10085);
DHT_Unified dht(DHTPIN, DHTTYPE);

bool sht31Connected = false;
bool bmp180Connected = false;

void enableRadio() {
  pinMode(RFM69_RST, OUTPUT);
  digitalWrite(RFM69_RST, HIGH);
  delay(100);
  digitalWrite(RFM69_RST, LOW);
  delay(100);
  radio.initialize(FREQUENCY,NODEID,NETWORKID);
  if (IS_RFM69HCW) {
    radio.setHighPower();
  }
  radio.setPowerLevel(31);
  radio.encrypt(ENCRYPTKEY);
  Serial.println(F("Radio enabled"));
}

void setupSensors() {
  // Reset rain gauge and anemometer.
  Serial.print("Resetting rain gauge and anemometer...");
  Wire.beginTransmission(RAINGAUGE_I2C_ADDRESS);
  Wire.write(1);
  Wire.endTransmission();
  Wire.beginTransmission(ANEMOMETER_I2C_ADDRESS);
  Wire.write(1);
  Wire.endTransmission();
  previousWindSpeedRetrieveMillis = millis();
  Serial.println(F("SHT31 begin "));
  sht31Connected = sht31.begin(0x44);
  if(!sht31Connected) {
    Serial.println(F("FAILED"));
  }
  Serial.println(F("BMP180 begin "));
  bmp180Connected = bmp180.begin();
  if(!bmp180Connected) {
    Serial.println(F("FAILED"));
  }
  dht.begin(); // library doesn't return a status on begin
  pinMode(VANE_PIN, INPUT);
  pinMode(VANE_EN_PIN, OUTPUT);
  digitalWrite(VANE_EN_PIN, LOW);
  pinMode(BATTERY_PIN, INPUT);
  pinMode(SOLAR_PIN, INPUT);
  Serial.println(F("Sensors setup"));
}

void setup() {
  Serial.begin(SERIAL_BAUD);
  Serial.println(F("Starting station..."));
  enableRadio();
  setupSensors();

  Serial.println(F("Sending test packet"));
  String packet0 = String();
  String packet0Data = String("Hello world");
  packet0 += String("0:");
  packet0 += packet0Data;
  memset(radiopacket0, 0, sizeof(radiopacket0));
  packet0.toCharArray(radiopacket0, 61);
  Serial.print(F("Sending: ")); Serial.println(radiopacket0);
  radio.send(RECEIVER, radiopacket0, strlen(radiopacket0)); //target node Id, message as string or byte array, message length
  Serial.println(F("OK"));
  radio.receiveDone(); //put radio in RX mode
  Serial.flush(); //make sure all serial data is clocked out before sleeping the MCU  
  Serial.println(F("Finished setup"));
}

bool waitingBetweenPackets = false;
void loop() {
  unsigned long currentMillis = millis();
  if((currentMillis - previousTransmitMillis > TRANSMIT_INTERVAL) && !transmitting) {
    state = 1; // create packets
    Serial.println(F("Creating packets"));
  } else if((currentMillis - previousPacketSentMillis > TIME_BETWEEN_PACKETS) && transmitting) {
    state++;
    waitingBetweenPackets = false;
    Serial.println(F("Incrementing state"));    
  }
  
  if(state == 1) { // create packets
      Serial.println(F("Creating packets"));
      String packet1 = String();
      String packet2 = String();
      String packet3 = String();
      String packet4 = String();

      String temperatureSHT31 = String();
      String humiditySHT32 = String();
      if(sht31Connected) {
        temperatureSHT31 = String(getTemperatureSHT31());
        humiditySHT32 = String(getHumiditySHT31());
      } else {
        temperatureSHT31 = String("0.00");
        humiditySHT32 = String("0.00");
      }
      String pressureBMP180 = String();
      String altitudeBMP180 = String();
      String temperatureBMP180 = String();
      if(bmp180Connected) {
        pressureBMP180 = String(getPressureBMP180());
        altitudeBMP180 = String(getAltitudeBMP180());
        temperatureBMP180 = String(getTemperatureBMP180());
      } else {
        pressureBMP180 = String("0.00");
        altitudeBMP180 = String("0.00");
        temperatureBMP180 = String("0.00");        
      }
      
      String windSpeed = String(getWind());
      String gustSpeed = String(getGust());
      
      String rain = String(getRain());
      String battery = String(getBattery());
      Serial.print(F("Battery string: "));
      Serial.print(battery);
      Serial.print(F(", battery float: "));
      Serial.println(getBattery());
      String solar = String(getSolar());
      
      String windDirection = String(getDirection());
      
      String temperatureDHT22 = String(getTemperatureDHT22());
      String humidityDHT22 = String(getHumidityDHT22());

      packet1 += String("1:");
      packet1 += temperatureSHT31;
      packet1 += String(",");
      packet1 += humiditySHT32;
      packet1 += String(",");  
      packet1 += pressureBMP180;
      packet1 += String(",");
      packet1 += altitudeBMP180;
    
      packet2 += String("2:");  
      packet2 += windSpeed;
      packet2 += String(",");
      packet2 += gustSpeed;
    
      packet3 += String("3:");
      packet3 += rain;
      packet3 += String(",");
      packet3 += battery;
      packet3 += String(",");
      packet3 += solar;
    
      packet4 += String("4:");
      packet4 += windDirection;
      packet4 += String(",");
      packet4 += temperatureBMP180;
      packet4 += String(",");
      packet4 += temperatureDHT22;
      packet4 += String(",");
      packet4 += humidityDHT22;

      memset(radiopacket1, 0, sizeof(radiopacket1));
      memset(radiopacket2, 0, sizeof(radiopacket2));
      memset(radiopacket3, 0, sizeof(radiopacket3));
      memset(radiopacket4, 0, sizeof(radiopacket4));
      packet1.toCharArray(radiopacket1, 61);
      packet2.toCharArray(radiopacket2, 61);
      packet3.toCharArray(radiopacket3, 61);
      packet4.toCharArray(radiopacket4, 61);

      Serial.println(F("All packets are ready"));
      Serial.println(radiopacket1);
      Serial.println(radiopacket2);
      Serial.println(radiopacket3);
      Serial.println(radiopacket4);
      state = 2; // send packet 1
      transmitting = true;
      waitingBetweenPackets = false;         
  }

  if(state == 2 && !waitingBetweenPackets) { // send packet 1
      Serial.print(F("Sending ")); Serial.println(radiopacket1);
      radio.send(RECEIVER, radiopacket1, strlen(radiopacket1)); //target node Id, message as string or byte array, message length
      Serial.println(F("OK"));
      radio.receiveDone(); //put radio in RX mode
      Serial.flush(); //make sure all serial data is clocked out before sleeping the MCU
      previousPacketSentMillis = millis();
      waitingBetweenPackets = true;
  }
  if(state == 3 && !waitingBetweenPackets) { // send packet 2
      Serial.print(F("Sending ")); Serial.println(radiopacket2);
      radio.send(RECEIVER, radiopacket2, strlen(radiopacket2)); //target node Id, message as string or byte array, message length
      Serial.println(F("OK"));
      radio.receiveDone(); //put radio in RX mode
      Serial.flush(); //make sure all serial data is clocked out before sleeping the MCU
      previousPacketSentMillis = millis();
      waitingBetweenPackets = true;       
  }
  if(state == 4 && !waitingBetweenPackets) { // send packet 3
      Serial.print(F("Sending ")); Serial.println(radiopacket3);
      radio.send(RECEIVER, radiopacket3, strlen(radiopacket3)); //target node Id, message as string or byte array, message length
      Serial.println(F("OK"));
      radio.receiveDone(); //put radio in RX mode
      Serial.flush(); //make sure all serial data is clocked out before sleeping the MCU
      previousPacketSentMillis = millis();  
      waitingBetweenPackets = true;   
  }
  if(state == 5 && !waitingBetweenPackets) { // send packet 4   
      Serial.print(F("Sending ")); Serial.println(radiopacket4);
      radio.send(RECEIVER, radiopacket4, strlen(radiopacket4)); //target node Id, message as string or byte array, message length
      Serial.println(F("OK"));
      radio.receiveDone(); //put radio in RX mode
      Serial.flush(); //make sure all serial data is clocked out before sleeping the MCU
      previousPacketSentMillis = millis();  
      previousTransmitMillis = currentMillis;
      transmitting = false;
      state = 0;
      waitingBetweenPackets = false;
  }
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

double getWind() {
  Wire.beginTransmission(ANEMOMETER_I2C_ADDRESS);
  Wire.write(2);
  Wire.endTransmission();
  unsigned long previousWindSpeedRetrieveMillisTemp = previousWindSpeedRetrieveMillis;
  previousWindSpeedRetrieveMillis = millis();
  delay(50);
  Wire.requestFrom(ANEMOMETER_I2C_ADDRESS, 4);
  byte dataBuffer[4];
  int i = 0;
  while(Wire.available()) {
    dataBuffer[i] = Wire.read();
    i++;    
  }
  Wire.endTransmission();
  union dataUnionTag {byte dataBytes[4]; unsigned long dataLong;} dataUnion;
  dataUnion.dataBytes[0] = dataBuffer[0];
  dataUnion.dataBytes[1] = dataBuffer[1];
  dataUnion.dataBytes[2] = dataBuffer[2];
  dataUnion.dataBytes[3] = dataBuffer[3];
  unsigned long anemometerClicks = dataUnion.dataLong;
  double wind = (WIND_FACTOR*anemometerClicks)/((previousWindSpeedRetrieveMillis-previousWindSpeedRetrieveMillisTemp) / 1000);
  
  return wind;
}

double getGust() {
  Wire.beginTransmission(ANEMOMETER_I2C_ADDRESS);  
  Wire.write(3);
  Wire.endTransmission();
  delay(50);
  Wire.requestFrom(ANEMOMETER_I2C_ADDRESS, 4);
  byte dataBuffer[4];
  int i = 0;
  while(Wire.available()) {
    dataBuffer[i] = Wire.read();
    i++;
  }
  Wire.endTransmission();
  union dataUnionTag {byte dataBytes[4]; unsigned long dataLong;} dataUnion;
  dataUnion.dataBytes[0] = dataBuffer[0];
  dataUnion.dataBytes[1] = dataBuffer[1];
  dataUnion.dataBytes[2] = dataBuffer[2];
  dataUnion.dataBytes[3] = dataBuffer[3];
  unsigned long anemometerMin = dataUnion.dataLong;
  double gust = (1/(anemometerMin/1000000.0)) * WIND_FACTOR;

  // Reset the anemometer
  Wire.beginTransmission(ANEMOMETER_I2C_ADDRESS);
  Wire.write(1);
  Wire.endTransmission();
  previousWindSpeedRetrieveMillis = millis();  
  
  return gust;  
}

float getRain() {
  Wire.beginTransmission(RAINGAUGE_I2C_ADDRESS);
  Wire.write(2);
  Wire.endTransmission();
  delay(50);
  Wire.requestFrom(RAINGAUGE_I2C_ADDRESS, 4);
  delay(50);
  byte dataBuffer[4];
  int i = 0;
  while(Wire.available()) {
    dataBuffer[i] = Wire.read();    i++;
  }
  Wire.endTransmission();
  union dataUnionTag {byte dataBytes[4]; float dataFloat;} dataUnion;
  dataUnion.dataBytes[0] = dataBuffer[0];
  dataUnion.dataBytes[1] = dataBuffer[1];
  dataUnion.dataBytes[2] = dataBuffer[2];
  dataUnion.dataBytes[3] = dataBuffer[3];
  float rain = dataUnion.dataFloat;
  
  return rain;
}

float getBattery() {
  float measuredvbat = analogRead(BATTERY_PIN);
  return measuredvbat;
}

float getSolar() {
  float measuredvsolar = analogRead(SOLAR_PIN);
  return measuredvsolar;
}

int averageAnalogRead(int pinToRead)
{
  byte numberOfReadings = 8;
  unsigned int runningValue = 0;

  for (int x = 0 ; x < numberOfReadings ; x++)
    runningValue += analogRead(pinToRead);
  runningValue /= numberOfReadings;

  return (runningValue);
}

unsigned int getDirection() {
  digitalWrite(VANE_EN_PIN, HIGH);
  unsigned int adc;
  adc = averageAnalogRead(VANE_PIN);
  digitalWrite(VANE_EN_PIN, LOW);
  
  return adc;
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
