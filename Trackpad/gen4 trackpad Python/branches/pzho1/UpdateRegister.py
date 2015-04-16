from PythonDriver import bridgeDriver
from PythonDriver import xmlReader
from PythonDriver import CalculateCRC
from time import sleep
import numpy as np
import json
import time
import datetime
import sys

returnData={}
returnData['TestItem']='UpdateRegister'
returnData['TestResult']='Pass'
returnData['ErrorCode']=0
returnData['ErrorMessage']=""
returnData['Data']=[]

readCount=1
bridge=bridgeDriver.BridgeDevice()   
ReCalibration=True
TrackpadColume=22
TrackpadRow=22
dutCommand=[]
#  0x01 0xE5  in ReadIDACs command means the number of data to read. it calculated by TrackpadColume*TrackpadRow+1(globalIDAC)
class UpdateRegisterCommands:
    GetRowSize   =[0xA0, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00]
    ReadConfigRow0   =[0xA0, 0x00, 0x03, 0x00, 0x00, 0x00, 0x80, 0x01]    
    ReCalMulCap   =[0xA0, 0x00, 0x09, 0x00, 0x00, 0x00]
    ReCalSelCap   =[0xA0, 0x00, 0x09, 0x02, 0x00, 0x00]


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
    
    
def UpdateRegisterTest():    
    
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
      
        
    #GetRowSize
    WaitForComplet=True
    bridge.WriteI2CData(0x24, 0x00, UpdateRegisterCommands.GetRowSize)
    while WaitForComplet:
        sleep(0.1)
        datain=bridge.ReadI2CData(0x24, 0x00, 10)        
        if (datain[2]&0x80==0x80) or (datain[2]&0x40==0x40):
            WaitForComplet=False
    
    rowSize=datain[4]
    UpdateRegisterCommands.ReadConfigRow0[6]=rowSize
    WaitForComplet=True
    bridge.WriteI2CData(0x24, 0x00, UpdateRegisterCommands.ReadConfigRow0)
    while WaitForComplet:
        sleep(0.1)
        datain=bridge.ReadI2CData(0x24, 0x00, 3)        
        if (datain[2]&0x80==0x80) or (datain[2]&0x40==0x40):
            WaitForComplet=False
    datain=bridge.ReadI2CData(0x24, 0x00, rowSize+8)
    RegisterData=datain[8:]
    #print datain
    # modify test data(pass,fail, test date, serial number)
    
    RegisterData[4]=int(sys.argv[1]) #test result pass or fail
    RegisterData[5]=0x2F #\'
    
    # add test time
    ymd=datetime.date.today()
    StrYMD=datetime.date.isoformat(ymd)    
    RegisterData[6]=int(StrYMD[2:4]) #Year
    RegisterData[7]=int(StrYMD[5:7]) #Month
    RegisterData[8]=int(StrYMD[8:10]) #Day
    RegisterData[9]=0x2F #\'
    
    index=10
    #add serial number
    SN=sys.argv[2]    
    SNLen=len(SN)    
    for i in range(SNLen):
        RegisterData[index+i]=ord(SN[i])
#     SNLen=len(SN)-10    
#     for i in range(SNLen):
#         RegisterData[index+i]=ord(SN[i+10])
        
    RegisterData[index+SNLen]=0x2F
    
    
    # modify test limits CRC
    RegisterData[64]=0xCF
    RegisterData[65]=0xAB
    RegisterData[66]=0x00
    
    # dutCommand
    dutCommand.append(0x00) #offset
    dutCommand.append(0x20) #CFG/TEST mode 
    dutCommand.append(0x00) #BL Active register 
    dutCommand.append(0x04) #Write Block Command
    dutCommand.append(0x00)
    dutCommand.append(0x00)
    dutCommand.append(0x00)
    dutCommand.append(rowSize)
    dutCommand.append(0x01) #Block ID
    
    for byte in RegisterData:
        dutCommand.append(byte)
    
    crc=CalculateCRC.calculate_fast(dutCommand[9:])
    
    # add security byte
    dutCommand.append(0xA5)
    dutCommand.append(0x01)
    dutCommand.append(0x02)
    dutCommand.append(0x03)
    dutCommand.append(0xFF)
    dutCommand.append(0xFE)
    dutCommand.append(0xFD)
    dutCommand.append(0x5A)
    
    # add crc
    dutCommand.append(crc>>8)
    dutCommand.append(crc&0x00FF)
    #print dutCommand
    for dataTemp in dutCommand:
        returnData['Data'].append(int(dataTemp))
    #write register data
    bridge.WriteI2CData(0x24, 0x00, dutCommand[1:])  
    sleep(2)  
        
    powerOff()    
    closePort()
    
    jsonReturn=json.dumps(returnData, True)
    print jsonReturn
    return jsonReturn    
    
if __name__=="__main__":
    UpdateRegisterTest()      
    