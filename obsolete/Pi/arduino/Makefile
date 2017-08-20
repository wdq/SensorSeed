DEBUG = -O3
CC = gcc
INCLUDE = -I/usr/local/include -I/usr/include/glib-2.0 -I/usr/lib/arm-linux-gnueabihf/glib-2.0/include  -lglib-2.0 -I/usr/include/mysql -DBIG_JOINS=1 -fno-strict-aliasing -g -DNDEBUG
CFLAGS = $(DEBUG) -Wall $(INCLUDE) -Winline -pipe -O -DBIG_JOINS=1  -fno-strict-aliasing   -g
LDFLAGS = -L/usr/local/lib
LDLIBS = -lpthread -O -lpigpio -L/usr/lib/arm-linux-gnueabihf -L/usr/lib/arm-linux-gnueabihf -lmysqlclient -lpthread -lz -lm -lrt -ldl
SRC = test.c testmysql.c


