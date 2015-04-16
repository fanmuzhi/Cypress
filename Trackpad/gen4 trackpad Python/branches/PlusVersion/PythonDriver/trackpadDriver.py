import bridgeDriver
import xmlReader
from time import sleep
import numpy
import json
import os
import thread

class trackpadDevice(object):
    
    def __init__(self):
        
        self._bridge=bridgeDriver.BridgeDevice()
        self._bridge.deviceAddress=0x24
        
    def poweron(self):
        # Set the voltage to be applied to the device
        
        self._bridge.SetVoltage(bridgeDriver.Voltages.VOLT_33V)
        # Apply power to the device
        self._bridge.PowerOn()
                
        # Wait for device power to be applied
        self._bridge.WaitForPower()
        
        # Set the protocol to I2C
        self._bridge.SetProtocol(bridgeDriver.enumInterfaces.I2C)
        
        # Reset the I2C bus
        self._bridge.ResetI2CBus()
        
        # Set the I2C Pins to use
        self._bridge.SetI2CPins(pins=bridgeDriver.I2CPins.kSPI1315)
        
        # Set the I2C speed
        self._bridge.SetI2CSpeed(bridgeDriver.enumI2Cspeed.CLK_400K)
        
    def openPort(self):
        # Get a list of ports
        portList = self._bridge.GetPorts()
        #print 'Port list:', portList
        #print sys.getsizeof(portList)
        if portList is None:
            raise Exception("NO BRIDGES AVAILABLE")            
            return
        else:    
            # Get the first port
            port = None
            for p in portList:
                origP = p
                p = p.lower()            
                loc = p.find('bridge')
                if loc >= 0:
                    port = origP
                    break        
            
            if port is None:
                raise Exception("NO BRIDGES AVAILABLE")            
                return
            else:
            # Open the port
                openPortStatus=self._bridge.OpenPort(port)                      
                if openPortStatus:
                    print "port opened"
                    return
                else:
                    #print "port can not be opened"
                    raise Exception("port can not be opened")            
                    return
    def ExitBootloaderMode(self):
        self._bridge.ExitBootloaderMode()
        
    def WriteI2CData(self, offset=0, data=None):
        self._bridge.WriteI2CData(offset,data)
        
    def ReadI2CData(self, offset=0, numBytes=61):
        self._bridge.ReadI2CData(offset,numBytes)