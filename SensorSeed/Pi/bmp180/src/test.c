#include "bmp180.h"
#include <unistd.h>
#include <stdio.h>

int main(int argc, char **argv){
	char *i2c_device = "/dev/i2c-1";
	int address = 0x77;
	
	void *bmp = bmp180_init(address, i2c_device);
	
	bmp180_eprom_t eprom;
	bmp180_dump_eprom(bmp, &eprom);
	
	
	bmp180_set_oss(bmp, 1);
	
	if(bmp != NULL){
		int i;
		for(i = 0; i < 10; i++) {
			float t = bmp180_temperature(bmp);
			long p = bmp180_pressure(bmp);
			float alt = bmp180_altitude(bmp);
			printf("t = %f, p = %lu, a= %f\n", t, p, alt);
			usleep(2 * 1000 * 1000);
		}
	
		bmp180_close(bmp);
	}
	
	return 0;
}
