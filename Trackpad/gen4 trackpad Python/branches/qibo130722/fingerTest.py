import bridgeDriver
from time import sleep
 
bridge=bridgeDriver.BridgeDevice()
bridge.PrintMessages()

class command:
    exitBootLoader                  =[0x7F,0x01,0x3B,0x00,0x00,0x4F,0x6D,0x17]
    operationAndLowPoerMode         =[0x0C]
    updateFingers                   =[0x04,0x00,0x12]
    

def openPort():    
    
    # Get a list of ports
    portList = bridge.GetPorts()
    print 'Port list:', portList
    
    if portList is None:
        print 'NO BRIDGES AVAILABLE'
        return False
    
    # Get the first port
    port = None
    for p in portList:
        origP = p
        p = p.lower()
        loc = p.find('bridge')
        if loc >= 0:
            port = origP
            break
    
    # Open the port
    openPortStatus=bridge.OpenPort(port)
    bridge.PrintMessages()
   
    if openPortStatus:
        print "port opened"
    
    return openPortStatus

def powerOn():
       
    # Set the voltage to be applied to the device
    bridge.SetVoltage(bridgeDriver.Voltages.VOLT_33V)
    #bridge.SetVoltage(Voltages.VOLT_33V)
    bridge.PrintMessages()   
       
    # Apply power to the device
    bridge.PowerOn()
    bridge.PrintMessages()
    
    # Wait for device power to be applied
    bridge.WaitForPower()
    bridge.PrintMessages()
    
    # Set the protocol to I2C
    bridge.SetProtocol(bridgeDriver.enumInterfaces.I2C)
    bridge.PrintMessages()
    
    # Reset the I2C bus
    bridge.ResetI2CBus()
    bridge.PrintMessages()
    
    # Set the I2C Pins to use
    bridge.SetI2CPins(pins=bridgeDriver.I2CPins.kSPI1315)
    bridge.PrintMessages()
    
    # Set the I2C speed
    bridge.SetI2CSpeed(bridgeDriver.enumI2Cspeed.CLK_400K)
    bridge.PrintMessages()   
    
def powerOff():
    bridge.PowerOff()
    bridge.PrintMessages()
    
def closePort():
    bridge.ClosePort()
    bridge.PrintMessages()    
    #PowerData=bridge.GetPower()
    #print PowerData['Vaux']
    
def powerTest():
    
    openPort()
    sleep(0.5)
    powerOn()
    sleep(0.5)
    # Exit bootloader    
    bridge.WriteI2CData(0x24, 0x00, [0x7F,0x01,0x3B,0x00,0x00,0x4F,0x6D,0x17])    
    sleep(0.5)
    
    #enter operation and low power mode
    #bridge.WriteI2CData(0x24, 0x00, [0x08])
    bridge.WriteI2CData(0x24, 0x00, [0x0C]) # for 11200300
    sleep(0.5)
    
    #updata fingers
    #bridge.WriteI2CData(0x24, 0x00, [0x04,0x00,0x12])
    sleep(0.5)
    
    datain=[]
    for i in range(10):
        #for 11200300 update finger
        bridge.WriteI2CData(0x24, 0x00, [0x04,0x00,0x12])
        sleep(0.5)
        datain=bridge.ReadI2CData(0x24, 0x00, 50)
        print datain
            
    
    
    # Current test    
    print "pass"
    powerOff()
    closePort()
    
if __name__=="__main__":
    powerTest()
        
        