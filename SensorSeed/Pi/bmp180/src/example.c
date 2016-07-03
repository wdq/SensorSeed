#include "bmp180.h"
#include <unistd.h>
#include <stdio.h>

int main(int argc, char **argv){
    char *i2c_device = "/dev/i2c-1";
    int address = 0x77;

    void *bmp = bmp180_init(address, i2c_device);

    if(bmp != NULL){
        int i;
        //for(i = 0; i < 10; i++) {
            float t = bmp180_temperature(bmp);
            float p = bmp180_pressure(bmp) / 100.0;
            float alt = bmp180_altitude(bmp);
            printf("temperaturebmp180: %.2f\npressure:  %.02f\naltitude: %.2f\n", t, p, alt);
        //}

        bmp180_close(bmp);
    }

    return 0;
}
