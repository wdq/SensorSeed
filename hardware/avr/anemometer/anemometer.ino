// anemometer
// This is code for an ATtiny44/88 I2C slave to send anemometer data over I2C.
// I'm using the anemometer that came with the SparkFun Weather Sensors.
// Datasheet: https://www.sparkfun.com/datasheets/Sensors/Weather/Weather%20Sensor%20Assembly..pdf
// Each turn causes a reed switch to trigger an interrupt.
// One turn per second is 1.492 MPH or 2.4 km/h. 
// Code is based on: https://blog.kkessler.com/2012/06/21/sparkfun-weather-station/
// Going to try just software debouncing first.

// Since I'll be using the internal oscillator inside of the ATtiny44 (not very accurate),
// I'll do the time calculations to get speed on the I2C master. 
// So instead of returning a speed, I'll be returning the number of clicks.

// Command flow:
//    1. Send 1 to slave, to reset it, and log the time it happened.
//    2. Wait a duration of time.
//    3. Send 2 to slave, to request wind speed ticks, and log the time it happened.
//    4. Read 4 byte unsigned long from slave, calculate wind speed.
//    5. Send 3 to slave, to request gust speed ticks, and log the time it happened.
//    6. Read 4 byte unsigned long from slave, calculate gust speed.
//    7. Send 1 to slave to reset it and log the time it happened.

#include <Wire.h>

#define I2C_ADDRESS             0x06
#define ANEMOMETER_PIN          8             // PB0
#define ANEMOMETER_INT          0             // PCINT0
#define LED_PIN                 0             // PD0
#define WIND_FACTOR             2.4           // 2.4 km/h

volatile unsigned long dataToSend;
volatile unsigned long anemometerClicks = 0;
volatile unsigned long lastAnemometerClick = 0;
volatile unsigned long anemometerMin = 0xffffffff;
bool transmitting;

ISR(PCINT0_vect) {
  if(digitalRead(ANEMOMETER_PIN) == 0) { // falling edge pin change interrupt
    long nowMicros = micros() - lastAnemometerClick;
    lastAnemometerClick = micros();
    if(nowMicros > 500) {
      anemometerClicks++;
      if(nowMicros < anemometerMin) {
        anemometerMin = nowMicros;
      }
    }
  }
}

// Average wind speed since last request
unsigned long getWind() {
  unsigned long reading = anemometerClicks;
  anemometerClicks = 0;
  return reading;
}

// Use the shortest time between two interrupts to find the fastest wind speed since last request
unsigned long getGust() {
  unsigned long reading = anemometerMin;
  anemometerMin = 0xffffffff;
  return reading; 
}

void setupI2C() {
  Wire.begin(I2C_ADDRESS);
  Wire.onReceive(wireReceiveEvent);
  Wire.onRequest(wireRequestEvent);  
}

void setupInterrupts() {
  pinMode(ANEMOMETER_PIN, INPUT_PULLUP);
  digitalWrite(ANEMOMETER_PIN, HIGH); // Turn on pullup
  PCMSK0 |= (1<<PCINT0);
  PCICR |= (1<<PCIE0);
  sei();
}

void setup() {
  setupI2C();
  setupInterrupts();
  pinMode(LED_PIN, OUTPUT);
}

// Handle debug LED
void loop() {
  if(transmitting) {
    digitalWrite(LED_PIN, HIGH);
    delay(500);
    digitalWrite(LED_PIN, LOW);
    transmitting = false;
  }  
}

// Handle I2C commands
void wireReceiveEvent(int count) {
  int x = Wire.read();
  if(x == 1) { // Init/reset
    anemometerClicks = 0;
    lastAnemometerClick = 0;
    anemometerMin = 0;
    transmitting = false;
    dataToSend = 0;
  } else if(x == 2) { // Wind
    dataToSend = getWind();        
  } else if(x == 3) { // Gust
    dataToSend = getGust();
  }
  
}

// Send the four byte unsigned long value requested over I2C
void wireRequestEvent() {  
  byte *dataBytes = (byte *)&dataToSend; 
  Wire.write(dataBytes, 4);
}

