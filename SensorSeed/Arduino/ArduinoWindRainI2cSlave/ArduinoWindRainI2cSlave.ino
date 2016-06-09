#include <Wire.h> 

volatile byte* Float1ArrayPtr;

#define FEATHER_PIN 4
#define ANEMOMETER_PIN 3
#define ANEMOMETER_INT 1
#define RAIN_GAUGE_PIN 2
#define RAIN_GAUGE_INT 0

#define WIND_FACTOR 2.4
#define TEST_PAUSE 60000
 
volatile unsigned long anem_count=0;
volatile unsigned long anem_last=0;
volatile unsigned long anem_min=0xffffffff;

#define RAIN_FACTOR 0.2794 // millimeters
 
volatile unsigned long rain_count=0;
volatile unsigned long rain_last=0;
double dataToSend;

unsigned long previousMillis = millis();
unsigned long previousWindSpeedRetrieveMillis = millis();
long interval = 600000; // interval at which to do something (milliseconds)

 

void setupWeatherInts()
{
  digitalWrite(FEATHER_PIN, HIGH);  
  pinMode(FEATHER_PIN, OUTPUT);
  digitalWrite(FEATHER_PIN, HIGH);
  pinMode(ANEMOMETER_PIN,INPUT);  
  pinMode(RAIN_GAUGE_PIN,INPUT_PULLUP);
  //digitalWrite(ANEMOMETER_PIN,HIGH);  // Turn on the internal Pull Up Resistor  
  digitalWrite(RAIN_GAUGE_PIN,HIGH);  // Turn on the internal Pull Up Resistor
  attachInterrupt(ANEMOMETER_INT,anemometerClick,FALLING);
  attachInterrupt(RAIN_GAUGE_INT,rainGageClick,FALLING);
  //t not interrupts();
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

double getUnitRain()
{
 
  unsigned long reading=rain_count;
  rain_count=0;
  double unit_rain=reading*RAIN_FACTOR;
 
  return unit_rain;
}
 
void rainGageClick()
{
    long thisTime=micros()-rain_last;
    rain_last=micros();
    if(thisTime>500)
    {
      rain_count++;
    }
        //Serial.println("RAIN CLICK");

}

void setup() {
 Wire.begin(2);
 Wire.onRequest(requestEvent); 
 Wire.onReceive(receiveEvent);
 //Serial.begin(9600);
 setupWeatherInts();
}

void loop() {
  unsigned long currentMillis = millis();
  if(currentMillis - previousMillis > interval) {
     previousMillis = currentMillis;
       digitalWrite(FEATHER_PIN, LOW);
       delay(10);
       digitalWrite(FEATHER_PIN, HIGH);
  }
}

void requestEvent(){
  // char frame[30];
 //sprintf(frame, "%8.2", dataToSend);

   //String data = String(dataToSend, DEC);
   //dataToSend = 123456.78;
   char temp1[10];
  dtostrf(dataToSend, 10, 2, temp1);
  String data = String(temp1);
  Wire.write(temp1);

  //Wire.write(frame, 30);
  //Wire.write("abcd");
  //Serial.print("Sent: ");
  //Serial.println(data);
  //Serial.println("-----");

  
}


void receiveEvent(int howMany){
  int c;
  c = Wire.read(); // receive byte as a character
  //Serial.print("Got: ");
  //Serial.println(c);
  if(c == 1) { // wind
    dataToSend = getUnitWind();
    //dataToSend = 1.2;
  } else if(c == 2) { // gust
     // dataToSend = 3.4;
    dataToSend = getGust();  
  } else if(c == 3) { // rain
     // dataToSend = 5.6;
    dataToSend = getUnitRain();  
  }
  previousMillis = millis();
}      

