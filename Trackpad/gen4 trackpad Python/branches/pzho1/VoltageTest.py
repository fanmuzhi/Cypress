from PythonDriver import bridgeDriver
from PythonDriver import xmlReader
from time import sleep
import numpy
import json
import sys
import os

class powerTestLimits:
    VcomHigh        =3800
    VcomLow         =2800
    VauxHigh        =3800
    VauxLow         =2800
    
returnData={}
returnData['TestItem']='Voltage'
returnData['TestResult']='Pass'
returnData['ErrorCode']=0
returnData['ErrorMessage']=""
returnData['Data']=[]

bridge=bridgeDriver.BridgeDevice()   

def openPort():  
    # Get a list of ports
    portList = bridge.GetPorts()
    #print 'Port list:', portList
    #print sys.getsizeof(portList)
    if portList is None:
        print 'NO BRIDGES AVAILABLE'
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
                print "port can not be opened"
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
    #PowerData=bridge.GetPower()
    #print PowerData['Vaux']
    
def voltageTest():
    file_list = []
    for root, dirs, files in os.walk(os.getcwd()):
        for f in files:
            (filename, ext) = os.path.splitext(f)
            if ext == ".xml" or ext == ".XML":
                # print f
                file_list.append(os.path.join(root, f))
    
    configPath=file_list[0]            
    configFile_data=xmlReader.captureXMLConfigFile()    
    configTest=configFile_data.get_item("VCOM",configPath)
    powerTestLimits.VcomLow=configTest.Min
    powerTestLimits.VcomHigh=configTest.Max
    #print powerTestLimits.VcomLow    
    
    if not openPort():
        returnData['ErrorCode']=0x30
        jsonReturn=json.dumps(returnData, True)
        print jsonReturn
        return jsonReturn
    sleep(0.1)
    powerOn()
    sleep(0.1)
    # Exit bootloader  
    bridge.WriteI2CData(0x24, 0x00, [0x7F,0x01,0x3B,0x00,0x00,0x4F,0x6D,0x17])    
    sleep(0.1)
        
    #enter operation    
    bridge.WriteI2CData(0x24, 0x00, [0x08]) 
    sleep(0.1)
    
    # Voltage Test
    PowerData=bridge.GetPower()
    #print PowerData
    VCOMvalue=PowerData['Vcom']
    VCOMvalue=float(VCOMvalue)/1000
    returnData['Data'].append(VCOMvalue)
    #powerTestStatus=(powerTestLimits.VcomHigh>VCOMvalue>powerTestLimits.VcomLow) and (powerTestLimits.VauxHigh>VCOMvalue>powerTestLimits.VauxLow)    
    if  powerTestLimits.VcomLow>VCOMvalue:
        powerOff()
        closePort()
        returnData['TestResult']='Fail'    
        returnData['ErrorMessage']="Low Voltage"
        returnData['ErrorCode']=0x40    
        jsonReturn=json.dumps(returnData, True)
        print jsonReturn
        return jsonReturn
   
    if  VCOMvalue>powerTestLimits.VcomHigh:
        powerOff()
        closePort()
        returnData['TestResult']='Fail'    
        returnData['ErrorMessage']="High Voltage"
        returnData['ErrorCode']=0x41    
        jsonReturn=json.dumps(returnData, True)
        print jsonReturn
        return jsonReturn
    
    powerOff()
    closePort()
    
    jsonReturn=json.dumps(returnData, True)
    print jsonReturn
    return jsonReturn
    
if __name__=="__main__":
    voltageTest()
        
        