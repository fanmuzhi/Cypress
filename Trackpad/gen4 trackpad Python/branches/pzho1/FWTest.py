from PythonDriver import bridgeDriver
from PythonDriver import xmlReader
from time import sleep
import numpy
import json
import os

ICOM=[]
returnData={}
returnData['TestItem']='FW'
returnData['TestResult']='Pass'
returnData['ErrorCode']=0
returnData['ErrorMessage']=""
returnData['Data']=[]
bridge=bridgeDriver.BridgeDevice()   

   

class FWTestLimits:   
    FWVersion               =1.1    
    FWRevisionControl       =511393
    

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
    
    
def FWTest():
    file_list = []
    for root, dirs, files in os.walk(os.getcwd()):
        for f in files:
            (filename, ext) = os.path.splitext(f)
            if ext == ".xml" or ext == ".XML":
                # print f
                file_list.append(os.path.join(root, f))
    
    configPath=file_list[0]     
    configFile_data = xmlReader.captureXMLConfigFile(configPath)
    configTest=configFile_data.get_item("FW Version")
    FWTestLimits.FWVersion=configTest.Min
    
    configTest=configFile_data.get_item("FW Revision")
    FWTestLimits.FWRevisionControl=configTest.Max
    
    if not openPort():
        returnData['ErrorCode']=0x30
        jsonReturn=json.dumps(returnData, True)
        print jsonReturn
        return jsonReturn
    sleep(0.1)
    powerOn()
    sleep(0.1)
    
    #print "exiting bootloader"
    #bridge.ExitBootloaderMode()
    #enter Sys info mode    
    
    bridge.WriteI2CData(0x83)
    #sleep(0.5)
    datain=bridge.ReadI2CData(0x00, 50)
    print datain
        
    
    RevControl=1L
    RevControl=datain[3]*2**16+datain[2]*2**8+datain[1]
    print RevControl
    returnData['Data'].append(RevControl)
    if not (RevControl==FWTestLimits.FWRevisionControl):
        returnData['ErrorMessage']="Un-supported FW"
        returnData['TestResult']='Fail'
        returnData['ErrorCode']=0x14
    
    #print RevControl
    #print datain[18]   
    
    powerOff()
    closePort()
    
    jsonReturn=json.dumps(returnData, True)
    print jsonReturn
    return jsonReturn    
    
if __name__=="__main__":
    FWTest()
        
        