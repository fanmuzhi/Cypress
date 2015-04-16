from PythonDriver import bridgeDriver
from PythonDriver import xmlReader
from time import sleep
import numpy
import json
import os

returnData={}
returnData['TestItem']='Opens'
returnData['TestResult']='Pass'
returnData['ErrorCode']=0
returnData['ErrorMessage']=""
returnData['Data']=[]

bridge=bridgeDriver.BridgeDevice()   
ReCalibration=True
TrackpadColume=22
TrackpadRow=22
#  0x01 0xE5  in ReadIDACs command means the number of data to read. it calculated by TrackpadColume*TrackpadRow+1(globalIDAC)
class OpensCommands:
    CalSensorIDAC   =[0xA0, 0x00, 0x09, 0x00, 0x00, 0x00]
    CalButtonIDAC   =[0xA0, 0x00, 0x09, 0x01, 0x00, 0x00]
    SelfTest     =[0xA0, 0x00, 0x07, 0x03, 0x01]
    GetSensorDataStruct   =[0xA0, 0x00, 0x10, 0x00, 0x00, 0x01, 0xE5, 0x00]
    GetButtonDataStruct   =[0xA0, 0x00, 0x10, 0x00, 0x00, 0x00, 0x05, 0x03]

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
    
    
def OpensTest():
    
    file_list = []
    for root, dirs, files in os.walk(os.getcwd()):
        for f in files:
            (filename, ext) = os.path.splitext(f)
            if ext == ".xml" or ext == ".XML":
                # print f
                file_list.append(os.path.join(root, f))
    
    configPath=file_list[0]
    
    configFile_data=xmlReader.captureXMLConfigFile() 
    config=configFile_data.get_data(configPath)    
    #print config.SensorRows
    TrackpadColume=config.SensorColumns
    TrackpadRow=config.SensorRows
    
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
       
        
    #Run Self Test
    WaitForComplet=True
    bridge.WriteI2CData(0x24, 0x00, OpensCommands.SelfTest)
    while WaitForComplet:
        sleep(0.1)
        datain=bridge.ReadI2CData(0x24, 0x00, 3)        
        if (datain[2]&0x80==0x80) or (datain[2]&0x40==0x40):
            WaitForComplet=False
    
    #Retrieve Data Structure - Sensor iDACs (Command 0x10, Data ID = 0x00)
    dataToRead=TrackpadColume*TrackpadRow+10    
    bridge.WriteI2CData(0x24, 0x00, OpensCommands.GetSensorDataStruct)
    sleep(0.1)
    datain=bridge.ReadI2CData(0x24, 0x00, dataToRead)
    for dataTemp in datain[9:493]:
        returnData['Data'].append(dataTemp)
    #print datain
    
#     bridge.WriteI2CData(0x24, 0x00, OpensCommands.GetButtonDataStruct)
#     sleep(0.1)
#     datain=bridge.ReadI2CData(0x24, 0x00, dataToRead)
#     print datain    
        
 
     
    LocalLen=TrackpadColume*TrackpadRow
    for Open in returnData['Data'][1:LocalLen]:
        if Open<20:
            returnData['ErrorMessage']="Opens test fail "
            returnData['TestResult']='Fail'
            returnData['ErrorCode']=0x56
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
    OpensTest()
        
        