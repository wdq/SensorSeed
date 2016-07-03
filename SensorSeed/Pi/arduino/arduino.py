# -*- coding: utf-8 -*-

#!/usr/bin/python


import sys
import smbus
import time
bus = smbus.SMBus(1)

address = 0x06

def writeNumber(value):
    bus.write_byte(address, value)
    return -1

def readNumber():
    number = bus.read_i2c_block_data(address, 0x00, 10)
    return number

writeNumber(int(sys.argv[1]))
time.sleep(1)

number = readNumber()
text = "".join([chr(c) for c in number]).strip()
print text
