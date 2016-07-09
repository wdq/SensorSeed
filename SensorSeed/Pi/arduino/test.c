#include <stdio.h>
#include <stdlib.h>
#include <pigpio.h>
#include <unistd.h>
#include <time.h>
#include <math.h>
#include <string.h>



int connectI2C() {
//        if(gpioInitialise() < 0) {
 //               return -1;
   //     }

        int i2cHandle = i2cOpen(1, 0x07, 0);

        if(i2cHandle < 0) {
                return -1;
        }

	return i2cHandle;
}

void disconnectI2C(int handle) {
	i2cClose(handle);
//	gpioTerminate();
}


void  getDataStatus() {

        char StatusReadBuf[200];
        int i;
        int i2cHandle = connectI2C();

        __attribute__((unused)) int requestStatus = i2cWriteByte(i2cHandle, 0x01);
        usleep(100000);
        __attribute__((unused)) int readStatus = i2cReadDevice(i2cHandle, StatusReadBuf, 10);
        usleep(100000);

        for(i = 0; i < 10; i++) {
                printf("%c", StatusReadBuf[i]);
        }

        disconnectI2C(i2cHandle);
}




void getData(int numberOfBytes, int requestNumber, FILE *f) {
        char DataReadBuf[numberOfBytes];
        int i;
        int i2cHandle = connectI2C();

        __attribute__((unused)) int requestData = i2cWriteByte(i2cHandle, requestNumber);
        usleep(100000);
        __attribute__((unused)) int readData = i2cReadDevice(i2cHandle, DataReadBuf, numberOfBytes);
        usleep(100000);

        for(i = 0; i < numberOfBytes; i++) {
                printf("%c", DataReadBuf[i]);
                fprintf(f, "%c", DataReadBuf[i]);
        }

        disconnectI2C(i2cHandle);
	printf("\n");
        fprintf(f, "\n");
}


int main(int argc, char *argv[]) {
	gpioTerminate();
	gpioInitialise();
	gpioSetMode(4, PI_INPUT);

	while(1) {

		int isThereNewData = gpioRead(4);
		if(isThereNewData == 1) {
			FILE *f = fopen("/var/www/html/data.txt", "w");
			fprintf(f, "Home Outside Weather Station\n");
	                printf("temperatureSHT31: ");
                        fprintf(f, "temperatureSHT31: ");
        	        getData(7, 1, f);
                	printf("humiditySHT32: ");
                        fprintf(f, "humiditySHT32: ");
     	       		getData(6, 2, f);
                	printf("pressureBMP180: ");
                        fprintf(f, "pressureBMP180: ");
                	getData(9, 3, f);
	                printf("altitudeBMP180: ");
                        fprintf(f, "altitudeBMP180: ");
	                getData(9, 4, f);
	                printf("windSpeedI2C: ");
                        fprintf(f, "windSpeedI2C: ");
	                getData(10, 5, f);
	                printf("gustSpeedI2C: ");
                        fprintf(f, "gustSpeedI2C: ");
	                getData(10, 6, f);
	                printf("rainI2C: ");
                        fprintf(f, "rainI2C: ");
        	        getData(10, 7, f);
	                printf("batteryI2C: ");
                        fprintf(f, "batteryI2C: ");
	                getData(10, 8, f);
	                printf("solarI2C: ");
                        fprintf(f, "solarI2C: ");
	                getData(10, 9, f);
	                printf("directionI2C: ");
                        fprintf(f, "directionI2C: ");
	                getData(10, 10, f);
	                printf("temperatureBMP180: ");
                        fprintf(f, "temperatureBMP180: ");
	                getData(7, 11, f);
	                printf("temperatureDHT22: ");
                        fprintf(f, "temperatureDHT22: ");
	                getData(7, 12, f);
	                printf("humidityDHT22: ");
                        fprintf(f, "humidityDHT22: ");
	                getData(6, 13, f);
			fclose(f);
		} else {
		}
		sleep(1);

	}

        gpioTerminate();
return 0;

}

