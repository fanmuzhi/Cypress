from PythonDriver import bridgeDriver
from PythonDriver import xmlReader
from time import sleep
import numpy as np
import json
import os

returnData={}
returnData['TestItem']='RawData'
returnData['TestResult']='Pass'
returnData['ErrorCode']=0
returnData['ErrorMessage']=""
returnData['Data']=[]

readCount=1
bridge=bridgeDriver.BridgeDevice()   
ReCalibration=True
TrackpadColume=22
TrackpadRow=22
#  0x01 0xE5  in ReadIDACs command means the number of data to read. it calculated by TrackpadColume*TrackpadRow+1(globalIDAC)
class SelfRawDataCommands:
    InitBaseline   =[0xA0, 0x00, 0x0A, 0x0F]
    FullPanelScan   =[0xA0, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x00, 0x00]
    GetSensorFullPanelScan     =[0xA0, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x58, 0x03]
    ReCalMulCap   =[0xA0, 0x00, 0x09, 0x00, 0x00, 0x00]
    ReCalSelCap   =[0xA0, 0x00, 0x09, 0x02, 0x00, 0x00]

class SelfRawDataLimits:   
    RawDataHigh              =50
    RawDataLow              =-50
    

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
    
 
def getSignedIntfrom2Byte(datain):
    dataout=[]
    for i in range(len(datain)/2):
        if(datain[2*i+1]&0x80==0x80):
            data1=255-datain[2*i]            
            data2=127-(datain[2*i+1]&0x7F)            
            data=(data2*256+data1+1)*(-1)            
            dataout.append(data)
        else:            
            dataout.append(datain[2*i+1]*256+datain[2*i])
    #print dataout
    return dataout

    
def SelfRawDataTest():
    
#     if not bridge.openDevicePort():
#         returnData['ErrorMessage']="can not open port or port not exist"
#         returnData['TestResult']='Fail'
#         jsonReturn=json.dumps(returnData, True)
#         print jsonReturn
#         return jsonReturn
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
       
    #force recalibrate DUT
    if ReCalibration:
        WaitForComplet=True
        bridge.WriteI2CData(0x24, 0x00, SelfRawDataCommands.ReCalMulCap)
        while (WaitForComplet):        
            sleep(0.2)            
            datain=bridge.ReadI2CData(0x24, 0x00, 3)
            if (datain[2]&0x40==0x40):
                WaitForComplet=False
        
        WaitForComplet=True
        bridge.WriteI2CData(0x24, 0x00, SelfRawDataCommands.ReCalSelCap)
        while (WaitForComplet):        
            sleep(0.2)
            datain=bridge.ReadI2CData(0x24, 0x00, 3)
            if (datain[2]&0x40==0x40):
                WaitForComplet=False
    
    
    #Initialize Baselines
    WaitForComplet=True
    bridge.WriteI2CData(0x24, 0x00, SelfRawDataCommands.InitBaseline)
    while WaitForComplet:
        sleep(0.1)
        datain=bridge.ReadI2CData(0x24, 0x00, 3)        
        if (datain[2]&0x80==0x80) or (datain[2]&0x40==0x40):
            WaitForComplet=False
    
    rawDataTemp=[]
    dataToRead=2*(TrackpadColume+TrackpadRow)+10 
    for i in range(readCount):        
        #Execute Full Panel Scan (Command 0x0B) ...
        WaitForComplet=True
        bridge.WriteI2CData(0x24, 0x00, SelfRawDataCommands.FullPanelScan)
        while WaitForComplet:
            sleep(0.1)
            datain=bridge.ReadI2CData(0x24, 0x00, 3)        
            if (datain[2]&0x80==0x80) or (datain[2]&0x40==0x40):
                WaitForComplet=False
        
        #Retrieve Full Panel Scan - Sensor
        WaitForComplet=True
        bridge.WriteI2CData(0x24, 0x00, SelfRawDataCommands.GetSensorFullPanelScan)
        while WaitForComplet:
            sleep(0.1)
            datain=bridge.ReadI2CData(0x24, 0x00, 3)        
            if (datain[2]&0x80==0x80) or (datain[2]&0x40==0x40):
                WaitForComplet=False
        
        #Retrieve Data          
        datain=bridge.ReadI2CData(0x24, 0x00, dataToRead)
        #print datain
        datainTemp=getSignedIntfrom2Byte(datain[8:8+2*(TrackpadColume+TrackpadRow)])        
        rawDataTemp.append(datainTemp)
        
    
    rawDataTemp=np.array(rawDataTemp)
    #print rawDataTemp
    SelfrawData=rawDataTemp.mean(axis=0)
    #print SelfrawData    
    
    for dataTemp in SelfrawData:
        returnData['Data'].append(int(dataTemp))
    
    for dataTemp in returnData['Data']:        
        if  dataTemp<SelfRawDataLimits.RawDataLow:
            returnData['ErrorMessage']="Low self Raw data"
            returnData['TestResult']='Fail'
            returnData['ErrorCode']=0x10
            powerOff()
            closePort()    
            jsonReturn=json.dumps(returnData, True)
            print jsonReturn
            return jsonReturn   
        if  dataTemp>SelfRawDataLimits.RawDataHigh:
            returnData['ErrorMessage']="High self Raw data"
            returnData['TestResult']='Fail'
            returnData['ErrorCode']=0x11
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
    TrackpadColume=config.SensorColumns
    TrackpadRow=config.SensorRows    
    configTest=configFile_data.get_item("Self-cap Raw Data",configPath)    
    SelfRawDataLimits.RawDataHigh=configTest.Max
    SelfRawDataLimits.RawDataLow=configTest.Min
    readCount=configTest.Samples 
    SelfRawDataCommands.GetSensorFullPanelScan[5]=(TrackpadColume+TrackpadRow)*2/256
    SelfRawDataCommands.GetSensorFullPanelScan[6]=(TrackpadColume+TrackpadRow)*2%256
    #print NoiseCommands.GetSensorFullPanelScan    
    SelfRawDataTest()
        
        