from PythonDriver import bridgeDriver
from PythonDriver import xmlReader
from time import sleep
import numpy
import json

returnData={}
returnData['TestItem']='Shorts'
returnData['TestResult']='Pass'
returnData['ErrorCode']=0
returnData['ErrorMessage']=""
returnData['Data']=[]

bridge=bridgeDriver.BridgeDevice()   
ReCalibration=True

#  0x01 0xE5  in ReadIDACs command means the number of data to read. it calculated by TrackpadColume*TrackpadRow+1(globalIDAC)
class ShortsCommands:
    ShortsSelfTest   =[0xA0, 0x00, 0x07, 0x04]
    ShortsSelfTestResultRead   =[0xA0, 0x00, 0x08, 0x00, 0x00, 0x00, 0x24, 0x04]


def openPort():  
    # Get a list of ports
    portList = bridge.GetPorts()
    #print 'Port list:', portList
    #print sys.getsizeof(portList)
    if portList is None:
        #print 'NO BRIDGES AVAILABLE'
        returnData['ErrorMessage']="NO BRIDGES AVAILABLE"
        returnData['TestResult']='Fail'
        return False
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
            returnData['ErrorMessage']="NO BRIDGES AVAILABLE"
            returnData['TestResult']='Fail'
            return False
        else:
        # Open the port
            openPortStatus=bridge.OpenPort(port)
            bridge.PrintMessages()       
            if openPortStatus:
                print "port opened"
                return True
            else:
                #print "port can not be opened"
                returnData['ErrorMessage']="port can not be opened"
                returnData['TestResult']='Fail'
                return False
        
        

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
    
    
def ShortsTest():
     
    if not openPort():
        returnData['ErrorCode']=0x30
        jsonReturn=json.dumps(returnData, True)
        print jsonReturn
        return jsonReturn
    sleep(0.1)
    powerOn()
    sleep(0.1)
    
    #print "exiting bootloader"
    bridge.ExitBootloaderMode()
    #enter CAT info mode    
    bridge.WriteI2CData(0x24, 0x00, [0x28]) 
    sleep(0.1)
    
    
    #Run self test
    WaitForComplet=True
    bridge.WriteI2CData(0x24, 0x00, ShortsCommands.ShortsSelfTest)
    while WaitForComplet:
        sleep(0.1)
        datain=bridge.ReadI2CData(0x24, 0x00, 3)
        #print datain        
        if (datain[2]&0x80==0x80) or (datain[2]&0x40==0x40):
            WaitForComplet=False
    
    # Read self test result
    bridge.WriteI2CData(0x24, 0x00, ShortsCommands.ShortsSelfTestResultRead)
    sleep(0.1)
    datain=bridge.ReadI2CData(0x24, 0x00, 40)
    #print datain
    for dataTemp in datain[9:36]:
        returnData['Data'].append(dataTemp)
    
    
    #LocalLen=18
    for short in returnData['Data'][9:27]:
        #print short
        if short > 0:
            returnData['ErrorMessage']="Shorts test fail"
            returnData['TestResult']='Fail'
            returnData['ErrorCode']=0x36
            powerOff()
            closePort()    
            jsonReturn=json.dumps(returnData, True)
            print jsonReturn
            return jsonReturn          
    
    powerOff()    
    closePort()
    
    jsonReturn=json.dumps(returnData, True)
    print jsonReturn
    return jsonReturn    
    
if __name__=="__main__":
    ShortsTest()
        
        