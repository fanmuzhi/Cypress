from PythonDriver import bridgeDriver
from PythonDriver import xmlReader
import pywinusb.hid as hid
from time import sleep
import numpy
import json
import os

ICOM=[]
readCount=1
returnData={}
returnData['TestItem']='Current'
returnData['TestResult']='Pass'
returnData['ErrorCode']=0
returnData['ErrorMessage']=""
returnData['Data']=[]
sleepTime=5
readInterval=0.1
bridge=bridgeDriver.BridgeDevice()   

def readData_handler(data):
    if not data==None:
        TempI=0  # @IndentOk
        TempI=data[4]*256+data[3]
        ICOM.append(TempI)
        print TempI        
        #print("Raw data: {0}".format(data))
    

class powerTestLimits:   
    IcomHigh        =200
    IcomLow         =20
    IauxHigh        =200
    IauxLow         =20

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
    #PowerData=bridge.GetPower()
    #print PowerData['Vaux']

def ActiveCurrent():
    
    
    if not openPort():
        jsonReturn=json.dumps(returnData, True)
        print jsonReturn
        return jsonReturn
    sleep(0.1)
    powerOn()
    sleep(0.1)
    
    print "active current testing``````````"
    #print "exiting bootloader"
    bridge.ExitBootloaderMode()
    bridge.WriteI2CData(0x24, 0x02, [0x11]) #change sint from low to pull high
    bridge.WriteI2CData(0x24, 0x00, [0x00])#force active mode
    
    # Current test using pywinusb
    device = hid.HidDeviceFilter(vendor_id = 0x04b4, product_id = 0xf123).get_devices()[0]
    #print device
    device.open()
       
    for i in range(readCount):        
        device.set_raw_data_handler(readData_handler)
        #print device.find_output_reports()[0]
        report = device.find_output_reports()[0]        
        readCurrent=[0x00]*65
        readCurrent[1]=0x02
        readCurrent[2]=0x00
        readCurrent[3]=0xA6
        readCurrent[4]=0x00
        readCurrent[5]=0x00
        readCurrent[6]=0x00        
        report.set_raw_data(readCurrent)
        report.send(readCurrent)
        sleep(readInterval)
    
       
    ICOMvalue=numpy.mean(ICOM[-readCount:-1])    
    #ICOMvalue=float('%0.1f'%ICOMvalue)
    ICOMvalue=int(ICOMvalue)  
    returnData['Data'].append(ICOMvalue)   
    device.close()
    powerOff()
    closePort()
    
    jsonReturn=json.dumps(returnData, True)
    #print jsonReturn
    sleep(2)
    
def IdleCurrent():
    if not openPort():
        jsonReturn=json.dumps(returnData, True)
        print jsonReturn
        return jsonReturn
    sleep(0.1)
    powerOn()
    sleep(0.1)
    
    print "Idle current testing``````````"
    #print "exiting bootloader"
    bridge.ExitBootloaderMode()
    bridge.WriteI2CData(0x24, 0x00, [0x00]) 
    bridge.WriteI2CData(0x24, 0x02, [0x09, 0x18, 0x00]) #change sint from low to pull high
    sleep(sleepTime)
    
    # Current test using pywinusb
    device = hid.HidDeviceFilter(vendor_id = 0x04b4, product_id = 0xf123).get_devices()[0]
    #print device
    device.open()
    #sleep(1)   
    for i in range(readCount):        
        device.set_raw_data_handler(readData_handler)
        #print device.find_output_reports()[0]
        report = device.find_output_reports()[0]        
        readCurrent=[0x00]*65
        readCurrent[1]=0x02
        readCurrent[2]=0x00
        readCurrent[3]=0xA6
        readCurrent[4]=0x00
        readCurrent[5]=0x00
        readCurrent[6]=0x00        
        report.set_raw_data(readCurrent)
        report.send(readCurrent)
        sleep(readInterval)
    
    #print ICOM
    ICOMvalue=numpy.mean(ICOM[-readCount:-1])    
    #ICOMvalue=float('%0.1f'%ICOMvalue)
    ICOMvalue=int(ICOMvalue)  
    returnData['Data'].append(ICOMvalue)
    
    device.close()
    powerOff()
    closePort()
    
    jsonReturn=json.dumps(returnData, True)
    #print jsonReturn
    sleep(2)
    
    
def SuspendCurrent():
    if not openPort():
        jsonReturn=json.dumps(returnData, True)
        print jsonReturn
        return jsonReturn
    sleep(0.1)
    powerOn()
    sleep(0.1)
    
    print "suspend current testing``````````"
    #print "exiting bootloader"
    bridge.ExitBootloaderMode()
   
    bridge.WriteI2CData(0x24, 0x02, [0x11]) #change sint from low to pull high
    
    bridge.WriteI2CData(0x24, 0x02, [0x12]) #change sint from low to pull high
   
    bridge.WriteI2CData(0x24, 0x00, [0x02])#force suspend mode
    sleep(sleepTime)
    # Current test using pywinusb
    device = hid.HidDeviceFilter(vendor_id = 0x04b4, product_id = 0xf123).get_devices()[0]
    #print device
    device.open()
    #sleep(1)   
    for i in range(readCount):        
        device.set_raw_data_handler(readData_handler)
        #print device.find_output_reports()[0]
        report = device.find_output_reports()[0]        
        readCurrent=[0x00]*65
        readCurrent[1]=0x02
        readCurrent[2]=0x00
        readCurrent[3]=0xA6
        readCurrent[4]=0x00
        readCurrent[5]=0x00
        readCurrent[6]=0x00        
        report.set_raw_data(readCurrent)
        report.send(readCurrent)
        sleep(readInterval)
    
    #print ICOM
    ICOMvalue=numpy.mean(ICOM[-readCount:-1])    
    #ICOMvalue=float('%0.1f'%ICOMvalue)
    ICOMvalue=int(ICOMvalue)  
    returnData['Data'].append(ICOMvalue)  
    device.close()
    powerOff()
    closePort()
    
    jsonReturn=json.dumps(returnData, True)
    #print jsonReturn
    sleep(2)
    
    
def SleepCurrent():
    if not openPort():
        jsonReturn=json.dumps(returnData, True)
        print jsonReturn
        return jsonReturn
    sleep(0.1)
    powerOn()
    sleep(0.1)
    
    print "sleep current testing``````````"
    #print "exiting bootloader"
    bridge.ExitBootloaderMode()
    bridge.WriteI2CData(0x24, 0x00, [0x00]) 
    bridge.WriteI2CData(0x24, 0x02, [0x09, 0x64, 0x00]) #change sint from low to pull high
    sleep(sleepTime)
    
    # Current test using pywinusb
    device = hid.HidDeviceFilter(vendor_id = 0x04b4, product_id = 0xf123).get_devices()[0]
    #print device
    device.open()
    #sleep(1)   
    for i in range(readCount):        
        device.set_raw_data_handler(readData_handler)
        #print device.find_output_reports()[0]
        report = device.find_output_reports()[0]        
        readCurrent=[0x00]*65
        readCurrent[1]=0x02
        readCurrent[2]=0x00
        readCurrent[3]=0xA6
        readCurrent[4]=0x00
        readCurrent[5]=0x00
        readCurrent[6]=0x00        
        report.set_raw_data(readCurrent)
        report.send(readCurrent)
        sleep(readInterval)
    
    #print ICOM
    ICOMvalue=numpy.mean(ICOM[-readCount:-1])    
    #ICOMvalue=float('%0.1f'%ICOMvalue)
    ICOMvalue=int(ICOMvalue)  
    returnData['Data'].append(ICOMvalue)
    
    device.close()
    powerOff()
    closePort()
    
    jsonReturn=json.dumps(returnData, True)
    #print jsonReturn
    sleep(2)
    
def DeepSleepCurrent():
    if not openPort():
        jsonReturn=json.dumps(returnData, True)
        print jsonReturn
        return jsonReturn
    sleep(0.1)
    powerOn()
    sleep(0.1)
    
    print "Deep sleep current testing``````````"
    #print "exiting bootloader"
    bridge.ExitBootloaderMode()
    bridge.WriteI2CData(0x24, 0x00, [0x00]) 
    bridge.WriteI2CData(0x24, 0x02, [0x09, 0xF4, 0x01]) #change sint from low to pull high
    sleep(sleepTime)
    
    # Current test using pywinusb
    device = hid.HidDeviceFilter(vendor_id = 0x04b4, product_id = 0xf123).get_devices()[0]
    #print device
    device.open()
    #sleep(1)   
    for i in range(readCount):        
        device.set_raw_data_handler(readData_handler)
        #print device.find_output_reports()[0]
        report = device.find_output_reports()[0]        
        readCurrent=[0x00]*65
        readCurrent[1]=0x02
        readCurrent[2]=0x00
        readCurrent[3]=0xA6
        readCurrent[4]=0x00
        readCurrent[5]=0x00
        readCurrent[6]=0x00        
        report.set_raw_data(readCurrent)
        report.send(readCurrent)
        sleep(readInterval)
    
    #print ICOM
    ICOMvalue=numpy.mean(ICOM[-readCount:-1])    
    #ICOMvalue=float('%0.1f'%ICOMvalue)
    ICOMvalue=int(ICOMvalue)  
    returnData['Data'].append(ICOMvalue)
    
    device.close()    
    
    powerOff()
    closePort()
    
    jsonReturn=json.dumps(returnData, True)
    #print jsonReturn
    
def currentTest():   
    
    ActiveCurrent()
    SuspendCurrent()
    IdleCurrent()
    SleepCurrent()
    DeepSleepCurrent()
    jsonReturn=json.dumps(returnData, True)
    print jsonReturn
    return jsonReturn   

     
     
#     sleep(2)
#     
#     
#     if not openPort():
#         jsonReturn=json.dumps(returnData, True)
#         print jsonReturn
#         return jsonReturn
#     sleep(0.1)
#     powerOn()
#     sleep(0.1)
#     
#     print "Idle current testing method2``````````"
#     #print "exiting bootloader"
#     bridge.ExitBootloaderMode()
#     bridge.WriteI2CData(0x24, 0x00, [0x04]) 
#     bridge.WriteI2CData(0x24, 0x02, [0x11]) #change sint from low to pull high
#     #sleep(20)
#     
#     # Current test using pywinusb
#     device = hid.HidDeviceFilter(vendor_id = 0x04b4, product_id = 0xf123).get_devices()[0]
#     #print device
#     device.open()
#     #sleep(1)   
#     for i in range(10):        
#         device.set_raw_data_handler(readData_handler)
#         #print device.find_output_reports()[0]
#         report = device.find_output_reports()[0]        
#         readCurrent=[0x00]*65
#         readCurrent[1]=0x02
#         readCurrent[2]=0x00
#         readCurrent[3]=0xA6
#         readCurrent[4]=0x00
#         readCurrent[5]=0x00
#         readCurrent[6]=0x00        
#         report.set_raw_data(readCurrent)
#         report.send(readCurrent)
#         sleep(0.1)
#     
#     #print ICOM
#     ICOMvalue=numpy.mean(ICOM[-10:-1])    
#     #ICOMvalue=float('%0.1f'%ICOMvalue)
#     ICOMvalue=int(ICOMvalue)  
#     returnData['Data'].append(ICOMvalue)
#     jsonReturn=json.dumps(returnData, True)
#     print jsonReturn
#     
#     
#     print "sleep current testing method2``````````"
#     sleep(20)
#     for i in range(10):        
#         device.set_raw_data_handler(readData_handler)
#         #print device.find_output_reports()[0]
#         report = device.find_output_reports()[0]        
#         readCurrent=[0x00]*65
#         readCurrent[1]=0x02
#         readCurrent[2]=0x00
#         readCurrent[3]=0xA6
#         readCurrent[4]=0x00
#         readCurrent[5]=0x00
#         readCurrent[6]=0x00        
#         report.set_raw_data(readCurrent)
#         report.send(readCurrent)
#         sleep(0.1)
#     
#     #print ICOM
#     ICOMvalue=numpy.mean(ICOM[-10:-1])    
#     #ICOMvalue=float('%0.1f'%ICOMvalue)
#     ICOMvalue=int(ICOMvalue)  
#     returnData['Data'].append(ICOMvalue)
#     jsonReturn=json.dumps(returnData, True)
#     print jsonReturn
#     
#     
#     print "Deep sleep current testing method2``````````"
#     bridge.WriteI2CData(0x24, 0x02, [0x18, 0x01, 0x02, 0x01])
#     sleep(60)
#     for i in range(10):        
#         device.set_raw_data_handler(readData_handler)
#         #print device.find_output_reports()[0]
#         report = device.find_output_reports()[0]        
#         readCurrent=[0x00]*65
#         readCurrent[1]=0x02
#         readCurrent[2]=0x00
#         readCurrent[3]=0xA6
#         readCurrent[4]=0x00
#         readCurrent[5]=0x00
#         readCurrent[6]=0x00        
#         report.set_raw_data(readCurrent)
#         report.send(readCurrent)
#         sleep(0.1)
#     
#     #print ICOM
#     ICOMvalue=numpy.mean(ICOM[-10:-1])    
#     #ICOMvalue=float('%0.1f'%ICOMvalue)
#     ICOMvalue=int(ICOMvalue)  
#     returnData['Data'].append(ICOMvalue)
#     jsonReturn=json.dumps(returnData, True)
#     print jsonReturn
#     
#     
#     device.close()
#     powerOff()
#     closePort()
#     print "Active suspend Idle sleep deepsleep Idle2 sleep2 deepsleep2"
#     jsonReturn=json.dumps(returnData, True)
#     print jsonReturn
#     sleep(2)
    
    
     
    
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
    configTest=configFile_data.get_item("ICOM",configPath)
    powerTestLimits.IcomLow=configTest.Min*1000
    powerTestLimits.IcomHigh=configTest.Max*1000    
    readCount=configTest.Samples    
    #print readCount
    currentTest()
        
        