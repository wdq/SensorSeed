#include <TinyWireM.h>
#include <USI_TWI_Master.h>
#include <Wire.h>
#include <RFM69.h>
#include <SPI.h>
#include <stdlib.h>

volatile byte* Float1ArrayPtr;
double dataToSend;
unsigned long lastReceivedMillis = millis();
unsigned long lastSentMillis = millis();
char radiopacket[200];

// Settings for the Adafruit RFM69HCW radio
#define NETWORKID     81
#define NODEID        1
#define FREQUENCY     RF69_915MHZ
#define ENCRYPTKEY    "wquade"
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

 //Wire.begin(7);
 //Wire.onRequest(requestEvent); 
 //Wire.onReceive(receiveEvent);  
  Serial.println("Setup complete"); 
}

void loop() {
  //check if something was received (could be an interrupt from the radio)
  if (radio.receiveDone())
  {
    Serial.println("Got something...");
    //print message received to serial
    Serial.print('[');Serial.print(radio.SENDERID);Serial.print("] ");
    Serial.print((char*)radio.DATA);
    Serial.print("   [RX_RSSI:");Serial.print(radio.RSSI);Serial.print("]");

    lastReceivedMillis = millis();
    
    //check if sender wanted an ACK
    if (radio.ACKRequested())
    {
      radio.sendACK();
      Serial.println(" - ACK sent");
    }
  }
 
  radio.receiveDone(); //put radio in RX mode
  Serial.flush(); //make sure all serial data is clocked out before sleeping the MCU
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

   //String data = String(dataToSend, DEC);
   //dataToSend = 123456.78;


  if(dataToSend == 3) {
    //String data = String(radiopacket);
    lastSentMillis = millis();
    Wire.write(radiopacket);
  } else {
   char temp1[10];
   dtostrf(dataToSend, 10, 2, temp1);
   String data = String(temp1);
   Wire.write(temp1);   
  }

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
  if(c == 1) { // Is there new data?
    dataToSend = isDataNew();
    //dataToSend = 1.2;
  } else if(c == 2) { // What is the data?
     // dataToSend = 3.4;
    dataToSend = 3;  
  }
}      







