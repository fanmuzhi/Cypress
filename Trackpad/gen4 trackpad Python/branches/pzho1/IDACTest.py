from PythonDriver import bridgeDriver
from PythonDriver import xmlReader
from time import sleep
import numpy
import json
import os

returnData={}
returnData['TestItem']='IDAC'
returnData['TestResult']='Pass'
returnData['ErrorCode']=0
returnData['ErrorMessage']=""
returnData['Data']=[]
ReCalibration=True
TrackpadColume=22
TrackpadRow=22

bridge = bridgeDriver.BridgeDevice() 
#  0x01 0xE5  in ReadIDACs command means the number of data to read. it calculated by TrackpadColume*TrackpadRow+1(globalIDAC)
class IDACCommands:
    ReCalMulCap   =[0xA0, 0x00, 0x09, 0x00, 0x00, 0x00]
    ReCalSelCap   =[0xA0, 0x00, 0x09, 0x02, 0x00, 0x00]
    ReadIDACs     =[0xA0, 0x00, 0x10, 0x00, 0x00, 0x01, 0xE5, 0x00]

class IDACTestLimits:   
    LocalIDACH              =28
    LocalIDACL              =3   
    GlobalIDACH             =200
    GlobalIDACL             =10



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
    
    
def IDACTest():
    file_list = []
    for root, dirs, files in os.walk(os.getcwd()):
        for f in files:
            (filename, ext) = os.path.splitext(f)
            if ext == ".xml" or ext == ".XML":
                # print f
                file_list.append(os.path.join(root, f))
    
    configPath = file_list[0]     
    
    configFile_data = xmlReader.captureXMLConfigFile(configPath)    
    configTest = configFile_data.get_item("Global IDAC")
    IDACTestLimits.GlobalIDACL = configTest.Min
    IDACTestLimits.GlobalIDACH = configTest.Max
    #     configFile_data=xmlReader.captureXMLConfigFile()    
    configTest = configFile_data.get_item("Local IDAC")
    IDACTestLimits.LocalIDACL = configTest.Min
    IDACTestLimits.LocalIDACH = configTest.Max
    
    # print config.SensorRows
    TrackpadColume = configFile_data.get_data().SensorColumns
    TrackpadRow = configFile_data.get_data().SensorRows
          
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
    #enter Sys info mode    
    bridge.WriteI2CData(0x00, [0x28]) 
    sleep(0.1)
    
    #force recalibrate DUT
    if ReCalibration:
        WaitForComplet=True
        bridge.WriteI2CData(0x00, IDACCommands.ReCalMulCap)
        while (WaitForComplet):        
            sleep(0.2)            
            datain=bridge.ReadI2CData(0x00, 3)
            if (datain[2]&0x40==0x40):
                WaitForComplet=False
        
        WaitForComplet=True
        bridge.WriteI2CData(0x00, IDACCommands.ReCalSelCap)
        while (WaitForComplet):        
            sleep(0.2)
            datain=bridge.ReadI2CData(0x00, 3)
            if (datain[2]&0x40==0x40):
                WaitForComplet=False
    
    
    #Read GlobalIDAC and LocalIDAC
    WaitForComplet=True
    bridge.WriteI2CData(0x00, IDACCommands.ReadIDACs)
    while WaitForComplet:
        sleep(0.1)
        datain=bridge.ReadI2CData(0x00, 3)        
        if (datain[2]&0x80==0x80) or (datain[2]&0x40==0x40):
            WaitForComplet=False
    
    dataToRead=TrackpadColume*TrackpadRow+10
    sleep(0.1)
    datain=bridge.ReadI2CData( 0x00, dataToRead)    
    for dataTemp in datain[8:492]:        
        returnData['Data'].append(dataTemp)
    #print datain
    
    #print returnData['Data']    
    if  returnData['Data'][0]<IDACTestLimits.GlobalIDACL:
        returnData['ErrorMessage']="Low Global iDAC"
        returnData['TestResult']='Fail'
        returnData['ErrorCode']=0x52
        powerOff()
        closePort()    
        jsonReturn=json.dumps(returnData, True)
        print jsonReturn
        return jsonReturn  
    if  returnData['Data'][0]>IDACTestLimits.GlobalIDACH:
        returnData['ErrorMessage']="High Global iDAC"
        returnData['TestResult']='Fail'
        returnData['ErrorCode']=0x53
        powerOff()
        closePort()    
        jsonReturn=json.dumps(returnData, True)
        print jsonReturn
        return jsonReturn  
    
    LocalLen=TrackpadColume*TrackpadRow
    for localIDAC in returnData['Data'][1:LocalLen]:
        if  localIDAC<IDACTestLimits.LocalIDACL:
            returnData['ErrorMessage']="Low Local iDAC"
            returnData['TestResult']='Fail'
            returnData['ErrorCode']=0x50
            powerOff()
            closePort()    
            jsonReturn=json.dumps(returnData, True)
            print jsonReturn
            return jsonReturn  
        if  localIDAC>IDACTestLimits.LocalIDACH:
            returnData['ErrorMessage']="High Local iDAC"
            returnData['TestResult']='Fail'
            returnData['ErrorCode']=0x51
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
    IDACTest()
        
        
