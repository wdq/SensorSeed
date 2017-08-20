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
#define ENCRYPTKEY    "wquadeEncryptKey"
#define IS_RFM69HCW   true
int16_t packetnum = 0;
#define TRANSMIT_INTERVAL 90000 // in milliseconds, this is set to 1.5 minutes
#define TIME_BETWEEN_PACKETS 15000
unsigned long previousTransmitMillis = millis();
unsigned long previousPacketSentMillis = millis();
bool transmitting = false;

// Settings for the DHT22
#define DHTPIN            4
#define DHTTYPE           DHT22

// Pins for the Adafruit RFM69HCW radio
#define RFM69_CS      10
#define RFM69_IRQ     2
#define RFM69_IRQN    0  // IRQ 0
#define RFM69_RST     9
#define LED           13

// Other pins

#define VANE_PIN A1
#define SOLAR_PIN A2
#define BATTERY_PIN A3

// Anemometer settings
#define ANEMOMETER_PIN 3
#define ANEMOMETER_INT 1
#define WIND_FACTOR 2.4
#define TEST_PAUSE 60000
volatile unsigned long anem_count=0;
volatile unsigned long anem_last=0;
volatile unsigned long anem_min=0xffffffff;
unsigned long previousWindSpeedRetrieveMillis = millis();

// Rain settings
#define RAIN_GAUGE_PIN 5
#define RAIN_FACTOR 0.2794 // millimeters
volatile unsigned long rain_count=0;
volatile unsigned long rain_last=0;

#define SERIAL_BAUD   9600
int state = 0;
char radiopacket1[61];
char radiopacket2[61];
char radiopacket3[61];
char radiopacket4[61];


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
  dht.begin();
  pinMode(VANE_PIN,INPUT);
  pinMode(ANEMOMETER_PIN,INPUT);  
  pinMode(RAIN_GAUGE_PIN,INPUT);
  attachInterrupt(ANEMOMETER_INT,anemometerClick,FALLING);
  pinMode(BATTERY_PIN, INPUT);
  pinMode(SOLAR_PIN, INPUT);    
}

void setup() {
  Serial.begin(SERIAL_BAUD);
  Serial.println("Starting Arduino outside weather station...");
  setupRadio();
  initializeSensors();
  Serial.println("Setup complete");
}

void loop() {
  unsigned long currentMillis = millis();
  if((currentMillis - previousTransmitMillis > TRANSMIT_INTERVAL) && !transmitting) {
    state = 1; // create packets 
  } else if((currentMillis - previousPacketSentMillis > TIME_BETWEEN_PACKETS) && transmitting) {
    state++;    
  }

  if(digitalRead(RAIN_GAUGE_PIN)) {
    if((micros() - rain_last) > 500) {
      rain_count++;        
    }
    rain_last = micros();
  }

  
  if(state == 1) { // create packets
      String packet1 = String();
      String packet2 = String();
      String packet3 = String();
      String packet4 = String();
      
      String temperatureSHT31 = String(getTemperatureSHT31());
      String humiditySHT32 = String(getHumiditySHT31());
      String pressureBMP180 = String(getPressureBMP180());
      String altitudeBMP180 = String(getAltitudeBMP180());
      
      String windSpeed = String(getUnitWind());
      String gustSpeed = String(getGust());
      
      String rain = String(getUnitRain());
      String battery = String(getBattery());
      String solar = String(getSolar());
      
      String windDirection = String(getDirection());
      String temperatureBMP180 = String(getTemperatureBMP180());
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
      state = 2; // send packet 1
      transmitting = true;         
  }

  if(state == 2) { // send packet 1
      Serial.print("Sending "); Serial.println(radiopacket1);
      radio.send(RECEIVER, radiopacket1, strlen(radiopacket1)); //target node Id, message as string or byte array, message length
      Serial.println("OK");
      radio.receiveDone(); //put radio in RX mode
      Serial.flush(); //make sure all serial data is clocked out before sleeping the MCU
      previousPacketSentMillis = millis();
  }
  if(state == 3) { // send packet 2
      Serial.print("Sending "); Serial.println(radiopacket2);
      radio.send(RECEIVER, radiopacket2, strlen(radiopacket2)); //target node Id, message as string or byte array, message length
      Serial.println("OK");
      radio.receiveDone(); //put radio in RX mode
      Serial.flush(); //make sure all serial data is clocked out before sleeping the MCU
      previousPacketSentMillis = millis();          
  }
  if(state == 4) { // send packet 3
      Serial.print("Sending "); Serial.println(radiopacket3);
      radio.send(RECEIVER, radiopacket3, strlen(radiopacket3)); //target node Id, message as string or byte array, message length
      Serial.println("OK");
      radio.receiveDone(); //put radio in RX mode
      Serial.flush(); //make sure all serial data is clocked out before sleeping the MCU
      previousPacketSentMillis = millis();       
  }
  if(state == 5) { // send packet 4    
      Serial.print("Sending "); Serial.println(radiopacket4);
      radio.send(RECEIVER, radiopacket4, strlen(radiopacket4)); //target node Id, message as string or byte array, message length
      Serial.println("OK");
      radio.receiveDone(); //put radio in RX mode
      Serial.flush(); //make sure all serial data is clocked out before sleeping the MCU
      previousPacketSentMillis = millis();  
      previousTransmitMillis = currentMillis;
      transmitting = false;
      state = 0;
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

void anemometerClick()
{
  long thisTime=micros()-anem_last; // time since last click
  anem_last=micros(); // current time
  //if(thisTime>500)
  //{
    anem_count++; // count up
    if(thisTime<anem_min) // will run the first time through since anem_min is initially set to maximum value
    {
      anem_min=thisTime; // set it to the time since last click
    }
    //Serial.println("WIND CLICK");
 
  //}
}

double getUnitWind()
{
  unsigned long currentWindSpeedRetrieveMillis = millis();
  unsigned long windSpeedRetrieveMillisDifference =   currentWindSpeedRetrieveMillis - previousWindSpeedRetrieveMillis;
  previousWindSpeedRetrieveMillis = millis();
  unsigned long reading=anem_count; // get the raw counter value
  anem_count=0; // clear it
  double returnValue = (WIND_FACTOR*reading)/(windSpeedRetrieveMillisDifference/1000); 
  /*
   * (2.4 * counter)
   * / 60000/ 1000 (or 60)
   * 
  */
  //Serial.print("Unit wind: ");
  //Serial.println(returnValue);
  return returnValue;
}
 
double getGust()
{
 
  unsigned long reading=anem_min; // minimum time between clicks?
  anem_min=0xffffffff; // reset it
  double time=reading/1000000.0; // not sure what this is for, it isn't used
 
  return (1/(reading/1000000.0))*WIND_FACTOR;
  /*
   * 1 /
   * anem_min / 1000000
   * * 2.4
   */
}

double getUnitRain()
{
 
  unsigned long reading=rain_count;
  rain_count=0;
  double unit_rain=reading*RAIN_FACTOR;
 
  return unit_rain;
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
  unsigned int adc;
  adc = averageAnalogRead(VANE_PIN);

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


