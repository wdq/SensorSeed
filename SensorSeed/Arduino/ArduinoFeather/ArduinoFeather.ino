#include <Arduino.h>
#include <Wire.h>
#include <Adafruit_SHT31.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BMP085_U.h>
#include <Adafruit_VEML6070.h>
#include <Adafruit_TSL2561_U.h>
#include <DHT.h>
#include <DHT_U.h>
#include <SPI.h>
#include <Adafruit_WINC1500.h>
#include <avr/dtostrf.h>


#define VBATPIN A7
#define ANEMOMETER_PIN 5
#define VANE_PWR 6
#define VANE_PIN A1
#define SOLAR_PIN A2
#define RAIN_GAUGE_PIN 11
#define DHTPIN            9
#define DHTTYPE           DHT22

#define RAIN_FACTOR 0.2794
#define WIND_FACTOR 2.4
#define TEST_PAUSE 60000

#define WINC_CS   8
#define WINC_IRQ  7
#define WINC_RST  4
#define WINC_EN   2


volatile unsigned long rain_count = 0;
volatile unsigned long rain_last = 0;
volatile unsigned long anem_count = 0;
volatile unsigned long anem_last = 0;
volatile unsigned long anem_min = 0xffffffff;
static const int vaneValues[] PROGMEM = {66, 84, 92, 127, 184, 244, 287, 406, 461, 600, 631, 702, 786, 827, 889, 946};
static const int vaneDirections[] PROGMEM = {1125, 675, 900, 1575, 1350, 2025, 1800, 225, 450, 2475, 2250, 3375, 0, 2925, 3150, 2700};

Adafruit_SHT31 sht31 = Adafruit_SHT31();
Adafruit_BMP085_Unified bmp180 = Adafruit_BMP085_Unified(10085);
Adafruit_WINC1500 WiFi(WINC_CS, WINC_IRQ, WINC_RST);
Adafruit_VEML6070 veml6070 = Adafruit_VEML6070();
Adafruit_TSL2561_Unified tsl = Adafruit_TSL2561_Unified(TSL2561_ADDR_FLOAT, 12345);
DHT_Unified dht(DHTPIN, DHTTYPE);

char ssid[] = "wpa.quade.local";      // your network SSID (name)
char pass[] = "aWq$HDvYE0Zn";   // your network password
int keyIndex = 0;                 // your network key Index number (needed only for WEP)

int status = WL_IDLE_STATUS;
//IPAddress server(10,0,0,64);
//#define webpage "/SensorSeed/Sensor/AddSensorData"
unsigned long lastConnectionTime = 0;
const unsigned long postingInterval = 300L * 1000L;

Adafruit_WINC1500Server server(80);
//Adafruit_WINC1500Client client;

void anemometerClick()
{
  long thisTime = millis() - anem_last;
  anem_count++;
  anem_last = millis();

  if (thisTime < anem_min)
  {
    anem_min = thisTime;
  }

}

void rainGageClick()
{
  long thisTime = millis() - rain_last;
  if (thisTime > 10)
  {
    rain_last = millis();
    rain_count++;
  }
}

void setup() {
#ifdef WINC_EN
  pinMode(WINC_EN, OUTPUT);
  digitalWrite(WINC_EN, HIGH);
#endif
  Serial.begin(9600);
  Serial.println("Starting...");
  pinMode(VBATPIN, INPUT);
  pinMode(SOLAR_PIN, INPUT);

  initializeSensors();

  // check for the presence of the shield:
  if (WiFi.status() == WL_NO_SHIELD) {
    Serial.println("WiFi shield not present");
    // don't continue:
    while (true);
  }

  // attempt to connect to Wifi network:
  while ( status != WL_CONNECTED) {
    Serial.print("Attempting to connect to SSID: ");
    Serial.println(ssid);
    // Connect to WPA/WPA2 network. Change this line if using open or WEP network:
    status = WiFi.begin(ssid, pass);

    // wait 10 seconds for connection:
    delay(10000);
  }
  server.begin();
  // you're connected now, so print out the status:
  printWifiStatus();
  WiFi.setSleepMode(M2M_PS_H_AUTOMATIC, 1);

}

void loop() {

  Adafruit_WINC1500Client client = server.available();
  if (client) {
    Serial.println("new client");
    // an http request ends with a blank line
    boolean currentLineIsBlank = true;
    while (client.connected()) {
      if (client.available()) {
        char c = client.read();
        Serial.write(c);
        // if you've gotten to the end of the line (received a newline
        // character) and the line is blank, the http request has ended,
        // so you can send a reply
        if (c == '\n' && currentLineIsBlank) {
          // send a standard http response header
          client.println("HTTP/1.1 200 OK");
          client.println("Content-Type: text/plain");
          client.println("Connection: close");  // the connection will be closed after completion of the response
          client.println();
          client.print("Outside Home Weather Station\n");
          float temperature = getTemperature();
          float humidity = getHumidity(); // i2c
          float pressure = getPressure(); // i2c
          float altitude = getAltitude(); // i2c
          //double wind = getUnitWind();
          //double gust = getGust();
          //double rain = getUnitRain();
          String wind = getWindSpeedI2C(); // i2c
          String gust = getGustSpeedI2C(); // i2c
          String rain = getRainI2C(); // i2c
          float battery = getBattery(); // adc
          float solar = getSolar(); // adc
          unsigned int dir = getDirection(); // adc
          float temperaturebmp180 = getTemperatureBMP180(); // i2c
          uint16_t veml6070Data = veml6070.readUV(); // i2c
          float luxData = getLux(); // i2c
          float temperatureDHT22 = getTemperatureDHT22(); // other serial interface
          float humidityDHT22 = getHumidityDHT22(); // other serial interface

          client.print("temperature: ");
          client.print(temperature);
          client.print("\n");
          client.print("humidity: ");
          client.print(humidity);
          client.print("\n");
          client.print("pressure: ");
          client.print(pressure);
          client.print("\n");
          client.print("altitude: ");
          client.print(altitude);
          client.print("\n");
          client.print("wind: ");
          client.print(wind);
          client.print("\n");
          client.print("gust: ");
          client.print(gust);
          client.print("\n");
          client.print("rain: ");
          client.print(rain);
          client.print("\n");
          client.print("battery: ");
          client.print(battery);
          client.print("\n");
          client.print("solar: ");
          client.print(solar);
          client.print("\n");
          client.print("direction: ");
          client.print(dir);
          client.print("\n");
          client.print("temperaturebmp180: ");
          client.print(temperaturebmp180);
          client.print("\n");
          client.print("veml6070: ");
          client.print(veml6070Data);
          client.print("\n");
          client.print("lux: ");
          client.print(luxData);
          client.print("\n");
          client.print("temperatureDHT22: ");
          client.print(temperatureDHT22);
          client.print("\n");
          client.print("humidityDHT22: ");
          client.print(humidityDHT22);
          client.print("\n");

          break;
        }
        if (c == '\n') {
          // you're starting a new line
          currentLineIsBlank = true;
        }
        else if (c != '\r') {
          // you've gotten a character on the current line
          currentLineIsBlank = false;
        }
      }
    }
    // give the web browser time to receive the data
    delay(1);

    // close the connection:
    client.stop();
    Serial.println("client disconnected");
  }
}

void initializeSensors() {
  sht31.begin(0x44);
  bmp180.begin();
  veml6070.begin(VEML6070_1_T);
  tsl.enableAutoRange(true);
  tsl.setIntegrationTime(TSL2561_INTEGRATIONTIME_13MS);
  pinMode(VANE_PIN, INPUT);
  //pinMode(ANEMOMETER_PIN,INPUT);
  //pinMode(RAIN_GAUGE_PIN,INPUT_PULLUP);
  //pinMode(VANE_PWR,OUTPUT);
  //digitalWrite(VANE_PWR,LOW);
  //attachInterrupt(ANEMOMETER_PIN, anemometerClick, FALLING);
  //attachInterrupt(RAIN_GAUGE_PIN, rainGageClick, FALLING);
  interrupts();
}

String getWindSpeedI2C() {
  Wire.beginTransmission(2); // transmit to device #2
  Wire.write(1);
  Wire.endTransmission();
  delay(25);
  int i = 0;
  char temp1[10];
  Wire.requestFrom(2, 10);    // request 4 bytes from slave device #2
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
  Wire.beginTransmission(2); // transmit to device #2
  Wire.write(2);
  Wire.endTransmission();
  delay(25);
  int i = 0;
  char temp1[10];
  Wire.requestFrom(2, 10);    // request 4 bytes from slave device #2
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
  Wire.beginTransmission(2); // transmit to device #2
  Wire.write(3);
  Wire.endTransmission();
  delay(25);
  int i = 0;
  char temp1[10];
  Wire.requestFrom(2, 10);    // request 4 bytes from slave device #2
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


float getBattery() {
  float measuredvbat = analogRead(VBATPIN);
  //measuredvbat *= 2;    // we divided by 2, so multiply back
  //measuredvbat *= 3.3;  // Multiply by 3.3V, our reference voltage
  //measuredvbat /= 1024; // convert to voltage
  return measuredvbat;
}

float getSolar() {
  float measuredvsolar = analogRead(SOLAR_PIN);
  //measuredvsolar *= 3;    // we divided by 2, so multiply back
  //measuredvsolar *= 3.3;  // Multiply by 3.3V, our reference voltage
  //measuredvsolar /= 1024; // convert to voltage
  return measuredvsolar;
}

unsigned int getDirection() {
  unsigned int adc;
  adc = averageAnalogRead(VANE_PIN);

  return adc;
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

String getSHT31() {
  float temperature = sht31.readTemperature();
  float humidity = sht31.readHumidity();
  char temp1[10];
  char temp2[10];
  dtostrf(temperature, 1, 2, temp1);
  dtostrf(humidity, 1, 2, temp2);
  String data = String(temp1) + "," + String(temp2);
  return data;
} 

String getBMP180() {
  sensors_event_t event;
  bmp180.getEvent(&event);
  float pressure = event.pressure;
  float temperature;
  bmp180.getTemperature(&temperature);
  float seaLevelPressure = SENSORS_PRESSURE_SEALEVELHPA;
  float altitude = bmp180.pressureToAltitude(seaLevelPressure, event.pressure);
  char temp1[10];
  char temp2[10];
  char temp3[10];
  dtostrf(pressure, 1, 2, temp1);
  dtostrf(temperature, 1, 2, temp2);
  dtostrf(altitude, 1, 2, temp3);
  String data = String(temp1) + "," + String(temp2) + "," + String(temp3);
  return data;
}

String getWindSpeed() {
  double wind = getUnitWind();
  double gust = getGust();
  char temp1[10];
  char temp2[10];
  dtostrf(wind, 1, 2, temp1);
  dtostrf(gust, 1, 2, temp2);
  String data = String(temp1) + "," + String(temp2);
  return data;
}

String getRain() {
  double rain = getUnitRain();
  char temp1[10];
  dtostrf(rain, 1, 2, temp1);
  String data = String(temp1);
  return data;
}

float getTemperature() {
  float temperature = sht31.readTemperature();
  return temperature;
}

float getTemperatureBMP180() {
  sensors_event_t event;
  bmp180.getEvent(&event);
  float temperature;
  bmp180.getTemperature(&temperature);
  return temperature;
}

float getHumidity() {
  float humidity = sht31.readHumidity();
  return humidity;
}

float getPressure() {
  sensors_event_t event;
  bmp180.getEvent(&event);
  return event.pressure;
}

float getLux() {
  sensors_event_t event;
  tsl.getEvent(&event);
  return event.light;
}


float getAltitude() {
  sensors_event_t event;
  bmp180.getEvent(&event);
  float seaLevelPressure = SENSORS_PRESSURE_SEALEVELHPA;
  return bmp180.pressureToAltitude(seaLevelPressure, event.pressure);
}

double getUnitWind()
{
  unsigned long reading = anem_count;
  anem_count = 0;
  return (WIND_FACTOR * reading) / (TEST_PAUSE / 1000);
}

double getGust()
{

  unsigned long reading = anem_min;
  anem_min = 0xffffffff;
  double time = reading / 1000000.0;

  return (1 / (reading / 1000000.0)) * WIND_FACTOR;
}



unsigned int getWindDirection() {
  digitalWrite(VANE_PWR, HIGH);
  unsigned int adc;
  adc = averageAnalogRead(VANE_PIN);
  digitalWrite(VANE_PWR, LOW);
  /*if (adc < 380) return (113);
    if (adc < 393) return (68);
    if (adc < 414) return (90);
    if (adc < 456) return (158);
    if (adc < 508) return (135);
    if (adc < 551) return (203);
    if (adc < 615) return (180);
    if (adc < 680) return (23);
    if (adc < 746) return (45);
    if (adc < 801) return (248);
    if (adc < 833) return (225);
    if (adc < 878) return (338);
    if (adc < 913) return (0);
    if (adc < 940) return (293);
    if (adc < 967) return (315);
    if (adc < 990) return (270);
    return (-1); // error, disconnected?  */
  return adc;
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

double getWindVane()
{
  //analogReference(DEFAULT);
  digitalWrite(VANE_PWR, HIGH);
  delay(100);
  for (int n = 0; n < 10; n++)
  {
    analogRead(VANE_PIN);
  }

  unsigned int reading = analogRead(VANE_PIN);
  digitalWrite(VANE_PWR, LOW);
  unsigned int lastDiff = 2048;

  for (int n = 0; n < 16; n++)
  {
    int diff = reading - pgm_read_word(&vaneValues[n]);
    diff = abs(diff);
    if (diff == 0)
      return pgm_read_word(&vaneDirections[n]) / 10.0;

    if (diff > lastDiff)
    {
      return pgm_read_word(&vaneDirections[n - 1]) / 10.0;
    }

    lastDiff = diff;
  }

  return pgm_read_word(&vaneDirections[15]) / 10.0;

}


double getUnitRain()
{

  unsigned long reading = rain_count;
  rain_count = 0;
  double unit_rain = reading * RAIN_FACTOR;

  return unit_rain;
}


void printWifiStatus() {
  // print the SSID of the network you're attached to:
  Serial.print("SSID: ");
  Serial.println(WiFi.SSID());

  // print your WiFi shield's IP address:
  IPAddress ip = WiFi.localIP();
  Serial.print("IP Address: ");
  Serial.println(ip);

  // print the received signal strength:
  long rssi = WiFi.RSSI();
  Serial.print("signal strength (RSSI):");
  Serial.print(rssi);
  Serial.println(" dBm");
  // print where to go in a browser:
  Serial.print("To see this page in action, open a browser to http://");
  Serial.println(ip);
}


