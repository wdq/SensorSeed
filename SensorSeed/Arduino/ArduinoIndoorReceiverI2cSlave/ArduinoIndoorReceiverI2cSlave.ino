#include <Adafruit_SleepyDog.h>
#include <TinyWireM.h>
#include <USI_TWI_Master.h>
#include <Wire.h>
#include <RFM69.h>
#include <SPI.h>
#include <stdlib.h>
#include <avr/wdt.h>

volatile byte* Float1ArrayPtr;
double dataToSend;
unsigned long lastReceivedMillis = millis();
unsigned long lastSentMillis = millis();

char temperatureSHT31[7]; // 7 max chars (based on -999.99 to 999.99)
char humiditySHT32[6]; // 6 max chars (based on 0.00 to 100.00)
char pressureBMP180[9]; // 9 max cahrs (based on database model)
char altitudeBMP180[9]; // 9 max chars (based on database model)
char windSpeedI2C[10]; // 10 max chars (based on slave conversion)
char gustSpeedI2C[10]; // 10 max chars (based on slave conversion)
char rainI2C[10]; // 10 max chars (based on slave conversion)
char batteryI2C[10]; // 10 max chars (based on slave conversion)
char solarI2C[10]; // 10 max chars (based on slave conversion)
char directionI2C[10]; // 10 max chars (based on slave conversion)
char temperatureBMP180[7]; // 7 max chars (based on -999.99 to 999.99)
char temperatureDHT22[7]; // 7 max chars (based on -999.99 to 999.99)
char humidityDHT22[6]; // 6 max chars (based on 0.00 to 100.00)
char radiopacket1[61];
char radiopacket2[61];
char radiopacket3[61];
char radiopacket4[61];

// Settings for the Adafruit RFM69HCW radio
#define NETWORKID     81
#define NODEID        1
#define FREQUENCY     RF69_915MHZ
#define ENCRYPTKEY    "wquadeEncryptKey"
#define IS_RFM69HCW   true

// Pins for the Adafruit RFM69HCW radio
#define RFM69_CS      10
#define RFM69_IRQ     2
#define RFM69_IRQN    0  // IRQ 0
#define RFM69_RST     9

#define LED           13

#define SERIAL_BAUD   9600

// Setup the radio library
RFM69 radio = RFM69(RFM69_CS, RFM69_IRQ, IS_RFM69HCW, RFM69_IRQN);

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
  Serial.print("\nListening at ");
  Serial.print(FREQUENCY==RF69_433MHZ ? 433 : FREQUENCY==RF69_868MHZ ? 868 : 915);
  Serial.println(" MHz");  
}


void setup() {
  Serial.begin(SERIAL_BAUD);
  Serial.println("Starting Arduino Metro weather station I2C master, radio receiver...");
  setupRadio();
pinMode(8, OUTPUT);
digitalWrite(8, LOW);
 Wire.begin(7);
 Wire.onRequest(requestEvent); 
 Wire.onReceive(receiveEvent);
  int countdownMS = Watchdog.enable();   
  Serial.println("Setup complete"); 
}
void loop() {
  Watchdog.reset();
  //check if something was received (could be an interrupt from the radio)
  if (radio.receiveDone())
  {
  Watchdog.reset();
  int sensorIndex = 1;
  int sensorCharIndex = 0;
  int charIndex = 0;

    Serial.println("Got something...");
    //print message received to serial
    //Serial.print('[');Serial.print(radio.SENDERID);Serial.print("] ");

    char packetNumber = (char)radio.DATA[0];
    if(packetNumber == '1') {
      memset(radiopacket1, 0, 61);   
      memset(temperatureSHT31, 0, 7);
      memset(humiditySHT32, 0, 6);
      memset(pressureBMP180, 0, 9);
      memset(altitudeBMP180, 0, 9);
      for(byte i = 2; i < radio.DATALEN; i++) {
        char newChar = (char)radio.DATA[i];
        radiopacket1[i] = newChar;      
        if(isAlphaNumeric(newChar) || newChar == ',' || newChar == '.') {
          if(newChar == ',') {
            sensorIndex++;
            sensorCharIndex = 0;                  
          } else {
            if(sensorIndex == 1) {
              temperatureSHT31[sensorCharIndex] = newChar;
            } else if(sensorIndex == 2) {
              humiditySHT32[sensorCharIndex] = newChar;
            } else if(sensorIndex == 3) {
              pressureBMP180[sensorCharIndex] = newChar;
            } else if(sensorIndex == 4) {
              altitudeBMP180[sensorCharIndex] = newChar;
            }
            sensorCharIndex++;
          }
           charIndex++;        
        } else {
        }
  
      } 
      Serial.print("Got data packet 1: ");
      //Serial.print(radiopacket1);
      Serial.print("   [RX_RSSI:");Serial.print(radio.RSSI);Serial.println("]");
    } else if(packetNumber == '2') {
      memset(radiopacket2, 0, 61); 
      memset(windSpeedI2C, 0, 10);
      memset(gustSpeedI2C, 0, 10);

      for(byte i = 2; i < radio.DATALEN; i++) {
        char newChar = (char)radio.DATA[i];
        radiopacket2[i] = newChar;      
        if(isAlphaNumeric(newChar) || newChar == ',' || newChar == '.') {
          if(newChar == ',') {
            sensorIndex++;
            sensorCharIndex = 0;                  
          } else {
            if(sensorIndex == 1) {
              windSpeedI2C[sensorCharIndex] = newChar;
            } else if(sensorIndex == 2) {
              gustSpeedI2C[sensorCharIndex] = newChar;
            }
            sensorCharIndex++;
          }
           charIndex++;        
        } else {
        }
      } 
      Serial.print("Got data packet 2: ");
      //Serial.print(radiopacket2);
      Serial.print("   [RX_RSSI:");Serial.print(radio.RSSI);Serial.println("]");
  } else if(packetNumber == '3') {
    memset(radiopacket3, 0, 61); 
    memset(rainI2C, 0, 10);
    memset(batteryI2C, 0, 10);
    memset(solarI2C, 0, 10);

      for(byte i = 2; i < radio.DATALEN; i++) {
        char newChar = (char)radio.DATA[i];
        radiopacket3[i] = newChar;      
        if(isAlphaNumeric(newChar) || newChar == ',' || newChar == '.') {
          if(newChar == ',') {
            sensorIndex++;
            sensorCharIndex = 0;                  
          } else {
            if(sensorIndex == 1) {
              rainI2C[sensorCharIndex] = newChar;
            } else if(sensorIndex == 2) {
              batteryI2C[sensorCharIndex] = newChar;
            } else if(sensorIndex == 3) {
              solarI2C[sensorCharIndex] = newChar;
            }
            sensorCharIndex++;
          }
           charIndex++;        
        } else {
        }
      } 
      Serial.print("Got data packet 3: ");
      //Serial.print(radiopacket3);
      Serial.print("   [RX_RSSI:");Serial.print(radio.RSSI);Serial.println("]");
  } else if(packetNumber == '4') {
    memset(radiopacket4, 0, 61); 
    memset(directionI2C, 0, 10);
    memset(temperatureBMP180, 0, 7);
    memset(temperatureDHT22, 0, 7);
    memset(humidityDHT22, 0, 6); 

      for(byte i = 2; i < radio.DATALEN; i++) {
        char newChar = (char)radio.DATA[i];
        radiopacket4[i] = newChar;      
        if(isAlphaNumeric(newChar) || newChar == ',' || newChar == '.') {
          if(newChar == ',') {
            sensorIndex++;
            sensorCharIndex = 0;                  
          } else {
            if(sensorIndex == 1) {
              directionI2C[sensorCharIndex] = newChar;
            } else if(sensorIndex == 2) {
              temperatureBMP180[sensorCharIndex] = newChar;
            } else if(sensorIndex == 3) {
              temperatureDHT22[sensorCharIndex] = newChar;
            } else if(sensorIndex == 4) {
              humidityDHT22[sensorCharIndex] = newChar;
            }
            sensorCharIndex++;
          }
           charIndex++;        
        } else {
        }
      } 
      Serial.print("Got data packet 4: ");
      //Serial.print(radiopacket4);
      Serial.print("   [RX_RSSI:");Serial.print(radio.RSSI);Serial.println("]");
      lastReceivedMillis = millis();
      digitalWrite(8, HIGH);
  }
  radio.receiveDone(); //put radio in RX mode
  Serial.flush(); //make sure all serial data is clocked out before sleeping the MCU
  }
}



double isDataNew() {
  unsigned long difference = lastReceivedMillis -  lastSentMillis;
  if(difference > 0) {
    return 1;      
  } else {
    return 0;
  }
}



void requestEvent(){
  // char frame[30];
 //sprintf(frame, "%8.2", dataToSend);
    Watchdog.reset();
   //String data = String(dataToSend, DEC);
   //dataToSend = 123456.78;
  if(dataToSend == 0 || dataToSend == 1) { // isDataNew
   char temp1[10];
   dtostrf(dataToSend, 10, 2, temp1);
   String data = String(temp1);
   Wire.write(temp1);   
    
  } else {

    if((millis() - lastSentMillis) < 10000) { // if  data was sent less than ten seconds ago then restart the arduiono
      Serial.println("Sending bad data, restarting in 8 seconds...");
      while(1) {
        int isCrashed = 1;
        delay(1000);
        }
    }
    
    Serial.print("Index: ");
    //String data = String("error");
    int sensorIndex = dataToSend - 10;
    Serial.print(sensorIndex);
    Serial.print(" sent: ");
    if(sensorIndex == 1) {
      Wire.write(temperatureSHT31, 7);
      Serial.println(temperatureSHT31);
    } else if(sensorIndex == 2) {
      Wire.write(humiditySHT32, 6);
      Serial.println(humiditySHT32);
    } else if(sensorIndex == 3) {
      Wire.write(pressureBMP180, 9);
      Serial.println(pressureBMP180);
    } else if(sensorIndex == 4) {
      Wire.write(altitudeBMP180, 9);
      Serial.println(altitudeBMP180);
    } else if(sensorIndex == 5) {
      Wire.write(windSpeedI2C, 10);
      Serial.println(windSpeedI2C);
    } else if(sensorIndex == 6) {
      Wire.write(gustSpeedI2C, 10);
      Serial.println(gustSpeedI2C);
    } else if(sensorIndex == 7) {
      Wire.write(rainI2C, 10);
      Serial.println(rainI2C);
    } else if(sensorIndex == 8) {
      Wire.write(batteryI2C, 10);
      Serial.println(batteryI2C);
    } else if(sensorIndex == 9) {
      Wire.write(solarI2C, 10);
      Serial.println(solarI2C);
    } else if(sensorIndex == 10) {
      Wire.write(directionI2C, 10);
      Serial.println(directionI2C);
    } else if(sensorIndex == 11) {
      Wire.write(temperatureBMP180, 7);
      Serial.println(temperatureBMP180);
    } else if(sensorIndex == 12) {
      Wire.write(temperatureDHT22, 7);
      Serial.println(temperatureDHT22);
    } else if(sensorIndex == 13) {
      Wire.write(humidityDHT22, 6);
      Serial.println(humidityDHT22); 
      digitalWrite(8, LOW);     
    }
    lastSentMillis = millis();
    //Serial.println(data);
  }

  //Wire.write(frame, 30);
  //Wire.write("abcd");
  //Serial.print("Sent: ");
  //Serial.println(data);
  //Serial.println("-----"); 
}

      // 1: temperatureSHT31 7
      // 2: humiditySHT32 6 
      // 3: pressureBMP1809 
      // 4: altitudeBMP180 9
      // 5: windSpeedI2C 10
      // 6: gustSpeedI2C 10
      // 7: rainI2C 10
      // 8: batteryI2C 10
      // 9: solarI2C 10
      // 10: directionI2C 10
      // 11: temperatureBMP180 7
      // 12: temperatureDHT22 7
      // 13: humidityDHT22 6
      
void receiveEvent(int howMany){
  Watchdog.reset();
  int c;
  c = Wire.read(); // receive byte as a character
  //Serial.print("Got: ");
  //Serial.println(c);
  if(c == 0) { // Is there new data?
    dataToSend = isDataNew();
  } else { // New data, Example: to get temperatureSHT31 the Pi sends c=1, and the dataToSend will then be 11, so it doesn't interfere with the is there new data.
    dataToSend = 10 + c;  
  }
}      






