#include <stdio.h>
#include <stdlib.h>
#include <pigpio.h>
#include <unistd.h>
#include <time.h>
#include <math.h>
#include <string.h>


int connectI2C() {
        if(gpioInitialise() < 0) {
                return -1;
        }

        int i2cHandle = i2cOpen(1, 0x03, 0);

        if(i2cHandle < 0) {
                return -1;
        }

	return i2cHandle;
}

void disconnectI2C(int handle) {
	i2cClose(handle);
	gpioTerminate();
}

void getWind() {
	char WindReadBuf[10];
	int i;
	int i2cHandle = connectI2C();

        __attribute__((unused)) int requestWind = i2cWriteByte(i2cHandle, 0x01);
        usleep(100000);
        __attribute__((unused)) int readWind = i2cReadDevice(i2cHandle, WindReadBuf, 10);
        usleep(100000);

        for(i = 0; i < 10; i++) {
		printf("%c", WindReadBuf[i]);
        }

        disconnectI2C(i2cHandle);
}


void getSolar() {
	char SolarReadBuf[10];
	int i;
	int i2cHandle = connectI2C();

        __attribute__((unused)) int requestSolar = i2cWriteByte(i2cHandle, 0x05);
        usleep(100000);
        __attribute__((unused)) int readSolar = i2cReadDevice(i2cHandle, SolarReadBuf, 10);
        usleep(100000);

        for(i = 0; i < 10; i++) {

                printf("%c", SolarReadBuf[i]);
        }

	disconnectI2C(i2cHandle);
}





int main(int argc, char *argv[]) {
        if(atoi(argv[1]) == 1) {
                getWind();
        }


        if(atoi(argv[1]) == 5) {
                getSolar();
        }

return 0;

}
