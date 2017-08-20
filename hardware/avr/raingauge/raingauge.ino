// raingauge
// This is code for an ATtiny44/88 I2C slave to send rain gauge data over I2C.
// I'm using the rain gauge that came with the SparkFun Weather Sensors.
// Datasheet: https://www.sparkfun.com/datasheets/Sensors/Weather/Weather%20Sensor%20Assembly..pdf
// Each rain gauge tip is 0.011" or 0.2794 mm of rain.
// Code is based on: https://blog.kkessler.com/2012/06/21/sparkfun-weather-station/

// Going to try just software debouncing first, but might need to add a capacitor to the input pin.
// If I do hardware debouncing I could use the timer/counter with the pin change as a trigger.

#include <Wire.h>

#define I2C_ADDRESS             0x07
#define RAIN_PIN                8                // PB0
#define RAIN_INT                0                // PCINT0
#define LED_PIN                 19               // PC2
#define RAIN_FACTOR             0.2794           // 0.2794 mm

float dataToSend = 0;
volatile unsigned long rainClicks = 0;
volatile unsigned long rainLast = 0;
bool transmitting = false;

void rainClick() {
  long nowMicros = micros() - rainLast;
  rainLast = micros();
  //if(nowMicros > 500) {
    rainClicks++;
  //}
}
ISR(PCINT0_vect) {
  if(digitalRead(RAIN_PIN) == 0) { // falling edge pin change interrupt
    long nowMicros = micros() - rainLast;
    rainLast = micros();
    if(nowMicros > 500) {
      rainClicks++;
    }
  }
}


// Get total rain since last request
float getRain() {
  unsigned long reading = rainClicks;
  rainClicks = 0;
  return reading * RAIN_FACTOR;
}

void setupI2C() {
  Wire.begin(I2C_ADDRESS);
  Wire.onReceive(wireReceiveEvent);
  Wire.onRequest(wireRequestEvent);  
}

void setupInterrupts() {
  pinMode(RAIN_PIN, INPUT_PULLUP);
  digitalWrite(RAIN_PIN, HIGH); // Turn on pullup
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
    rainClicks = 0;
    rainLast = 0;
    dataToSend = 0;
    transmitting = false;        
  } else if(x == 2) { // Rain
    dataToSend = getRain();
    transmitting = true;
  }
  
}

// Send the four byte floating point value requested over I2C
void wireRequestEvent() {
  byte *dataBytes = (byte *)&dataToSend;
  Wire.write(dataBytes, 4);
}

