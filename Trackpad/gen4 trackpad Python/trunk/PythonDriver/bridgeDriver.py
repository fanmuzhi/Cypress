'''
Module Name:    ppcom.py
Author(s):      Jon Peterson (PIJ)
                Jenny Bui (BNJ)
Description:    This module implements communications with the I2C bridge via
                the PSoCProgrammerCOM interface.
Dependencies:   win32com
'''

# Imports
#import win32com.client
# from PSoCProgrammerCOM import *

import comtypes
import comtypes.client
import array
import math
import thread
from time import sleep
#import UpgradePPCOM
import os
import numpy


# General definitions
_kDefaultPortName = 'Bridge/00000000'
_kPumpRate = 0.005  # 5ms "pump events" interval

class enumModes:
    OPMode = 0
    SYSMode = 1
    CMDMode = 2

class enumPowerModes:
    SoftReset = 0x01
    DeepSleep = 0x02
    LowPower = 0x04
    
class enumCanPowerDevice:
    CAN_MEASURE_POWER             =0x4        # from enum enumCanPowerDevice
    CAN_POWER_DEVICE              =0x1        # from enum enumCanPowerDevice
    CAN_READ_POWER                =0x2        # from enum enumCanPowerDevice
    CAN_MEASURE_POWER_2           =0x8        # from enum enumCanPowerDevice

class enumCanProgram:
    CAN_PROGRAM_CARBON            =0x1        # from enum enumCanProgram
    CAN_PROGRAM_ENCORE            =0x2        # from enum enumCanProgram
    
class enumInterfaces:
    I2C                           =0x4        # from enum enumInterfaces
    ISSP                          =0x2        # from enum enumInterfaces
    JTAG                          =0x1        # from enum enumInterfaces
    SWD                           =0x8        # from enum enumInterfaces
    SPI                           =0x10       # from enum enumInterfaces

class enumFrequencies:
    FREQ_01_5                     =0xc0       # from enum enumFrequencies
    FREQ_01_6                     =0x98       # from enum enumFrequencies
    FREQ_03_0                     =0xe0       # from enum enumFrequencies
    FREQ_03_2                     =0x18       # from enum enumFrequencies
    FREQ_06_0                     =0x60       # from enum enumFrequencies
    FREQ_08_0                     =0x90       # from enum enumFrequencies
    FREQ_12_0                     =0x84       # from enum enumFrequencies
    FREQ_16_0                     =0x10       # from enum enumFrequencies
    FREQ_24_0                     =0x4        # from enum enumFrequencies
    FREQ_48_0                     =0x0        # from enum enumFrequencies
    FREQ_RESET                    =0xfc       # from enum enumFrequencies

class enumI2Cspeed:
    CLK_100K                      =0x1        # from enum enumI2Cspeed
    CLK_400K                      =0x2        # from enum enumI2Cspeed
    CLK_50K                       =0x4        # from enum enumI2Cspeed
    CLK_1000K                     =0x5

class enumSonosArrays:
    ARRAY_ALL                     =0x1f       # from enum __MIDL___MIDL_itf_PSoCProgrammerCOM_0000_0001
    ARRAY_EEPROM                  =0x2        # from enum __MIDL___MIDL_itf_PSoCProgrammerCOM_0000_0001
    ARRAY_FLASH                   =0x1        # from enum __MIDL___MIDL_itf_PSoCProgrammerCOM_0000_0001
    ARRAY_NVL_FACTORY             =0x8        # from enum __MIDL___MIDL_itf_PSoCProgrammerCOM_0000_0001
    ARRAY_NVL_USER                =0x4        # from enum __MIDL___MIDL_itf_PSoCProgrammerCOM_0000_0001
    ARRAY_NVL_WO_LATCHES          =0x10       # from enum __MIDL___MIDL_itf_PSoCProgrammerCOM_0000_0001

class enumUpgradeFirmware:
    FINALIZE                      =0x3        # from enum enumUpgradeFirmware
    INITIALIZE                    =0x0        # from enum enumUpgradeFirmware
    UPGRADE_BLOCK                 =0x1        # from enum enumUpgradeFirmware
    VERIFY_BLOCK                  =0x2        # from enum enumUpgradeFirmware

class enumValidAcquireModes:
    CAN_POWER_CYCLE_ACQUIRE       =0x2        # from enum enumValidAcquireModes
    CAN_POWER_DETECT_ACQUIRE      =0x4        # from enum enumValidAcquireModes
    CAN_RESET_ACQUIRE             =0x1        # from enum enumValidAcquireModes
    
class enumVoltages:
    VOLT_18V                      =0x8        # from enum enumVoltages
    VOLT_25V                      =0x4        # from enum enumVoltages
    VOLT_33V                      =0x2        # from enum enumVoltages
    VOLT_50V                      =0x1        # from enum enumVoltages
    
class Voltages:
    VOLT_33V                      = '3.3'
    VOLT_50V                      = '5.0'

class I2CPins:
    kOriginalPins68               = 0
    kISSPPins97                   = 1
    kSPI1315                      = 2

class ResetPolarity:
    XRES_POLARITY_NEGATIVE  = 0x00
    XRES_POLARITY_POSITIVE  = 0x01



class BridgeDevice(object):

    # Message Queue severity definitions
    kException = 1
    kCriticalError = 2
    kWarning = 3
    kDebugMessage = 4
    deviceAddress=0x24
    I2C_SYS_LENGTH = 146
    
    # I2C Register Offsets (OP)
    I2C_OP_HST_MODE = 0
    I2C_OP_RESERVED = 1
    I2C_OP_COMMAND = 2
    I2C_OP_COMMAND_COMPLETE_BIT = 0x40
    I2C_OP_COMMAND_TOGGLE_BIT = 0x80
    I2C_OP_COMMAND_DATA_0 = 3
    I2C_OP_COMMAND_DATA_1 = 4
    I2C_OP_COMMAND_DATA_2 = 5
    I2C_OP_COMMAND_DATA_3 = 6
    I2C_OP_COMMAND_DATA_4 = 7
    I2C_OP_COMMAND_DATA_5 = 8
    I2C_OP_REP_LEN = 9
    I2C_OP_REP_STAT = 0x0A
    I2C_OP_TT_STAT = 0x0B
    
    #---------------------------------------------------------------------------
    #                               __init__
    #---------------------------------------------------------------------------
    def __init__(self):
        
        # Initialize class variables
        self._messageQueue = []
        self._interruptMode = False
        self._connectedCallback = None
        self._disconnectedCallback = None
        self._notifyCallback = None
        self._receivedDataCallback = None
    
        # Create an interface to the COM object
        self._bridge = None
        try:
            
            # Create the bridge object
            #self._bridge = win32com.client.Dispatch("PSoCProgrammerCOM.PSoCProgrammerCOM_Object.11")
            self._bridge = comtypes.client.CreateObject('PSoCProgrammerCOM.PSoCProgrammerCOM_Object')
            #self._bridge = comtypes.client.CreateObject('PSoCProgrammerCOM.PSoCProgrammerCOM_Object.11')
            
            # Associate the process ID with the COM object
            pid = os.getpid()
            self._serverProcessID = self._bridge._StartSelfTerminator(pid)
        
            # See if there was an error connecting to the bridge
            if self._bridge is None:
                msg = 'Failed to create a reference to the COM object.'
                self._writeMessage('__init__', msg, self.kCriticalError)
      
        except Exception as ex:
            msg = 'An exception occurred while creating a reference to the COM object.\n'
            msg += '     Exception: ' + str(ex)
            self._writeMessage('__init__', msg, self.kException)
            
            # If we get an exception here, we should delete the auto-generated COM Types code
            # out of program files. This can be an error when the DLL has been upgraded.
        
        return
    
    #---------------------------------------------------------------------------
    #                             _writeMessage
    #---------------------------------------------------------------------------
    def _writeMessage(self, fromMethod, message, severity):
        '''
        This "private" method is used to write error, warning and debug messages
        to an internal queue which can either be read or ignored.
        '''
        
        # Create a dictionary to store the message
        errorDict = {}
        errorDict['fromMethod'] = fromMethod
        errorDict['message'] = message
        errorDict['severity'] = severity
        
        if (severity == self.kCriticalError) or (severity == self.kException):
            errorDict['shouldExit'] = True
        else:
            errorDict['shouldExit'] = False
        
        # Add the dictionary to the queue
        self._messageQueue.append(errorDict)
        
        return

    #---------------------------------------------------------------------------
    #                           ClearMessageQueue
    #---------------------------------------------------------------------------
    def ClearMessageQueue(self):
        ''' This method removes all messages in the message queue. '''
        
        del self._messageQueue
        self._messageQueue = []
        return

    #---------------------------------------------------------------------------
    #                            GetLastMessage
    #---------------------------------------------------------------------------
    def GetLastMessage(self):
        ''' This method returns the last message added to the message queue. '''
        
        try:
            value = self._messageQueue.pop()
        except:
            self._writeMessage('GetLastMessage', 'Could not pop message from queue.', self.kWarning)
            value = None
            
        return value

    #---------------------------------------------------------------------------
    #                            GetCriticalErrors
    #---------------------------------------------------------------------------
    def GetCriticalErrors(self):
        ''' This method gets a list of critical errors in the message queue.'''
        
        # Create an empty list to contain errors
        errors = []
        
        # Look through all messages in the queue
        for msg in self._messageQueue:
            if (msg['severity'] == self.kCriticalError) or \
               (msg['severity'] == self.kException):
                # Add the error to the list
                errors.append(msg)
                
        # Remove all errors from the message queue
        for error in errors:
            self._messageQueue.remove(error)
            
        return errors
        
    #---------------------------------------------------------------------------
    #                            PrintMessages
    #---------------------------------------------------------------------------
    def PrintMessages(self, displaySeverity=0, errorList=None):
        '''
        This method prints all messages in the message queue. All messages are
        removed from the queue when this method completes. You can choose to 
        view only messages of a certain type (kCriticalErrors, kWarning, or
        kDebugMessage) or you can view all messages. Regardless of the 
        displaySeverity setting, all messages are removed from the queue.
        '''
        
        while (len(self._messageQueue) > 0) or \
              ((errorList is not None) and (len(errorList) > 0)):
            # Get the last message from the queue
            if errorList is None:
                dictM = self._messageQueue.pop()
            else:
                try:
                    dictM = errorList.pop()
                except:
                    msg = 'Exception occurred while poping from error list'
                    self._writeMessage('PrintMessages', msg, self.kWarning)
                    break
            
            # Determine the severity
            severity = dictM['severity']
            
            if (displaySeverity == 0) or (severity == displaySeverity):
                if severity == self.kException:
                    print 'EXCEPTION: ',
                elif severity == self.kCriticalError:
                    print 'CRITICAL ERROR ',
                elif severity == self.kWarning:
                    print 'WARNING ',
                elif severity == self.kDebugMessage:
                    print 'DEBUG ',
                    
                # Print the method which sent the message
                print '(%s): ' % dictM['fromMethod'],
                
                # Print the message body
                print dictM['message']
        
        return
        
    #---------------------------------------------------------------------------
    #                          _OperationSucceeded
    #---------------------------------------------------------------------------
    def _OperationSucceeded(self, hr):
        '''
        This is a "private" method for determining if a PPCom operation was
        successful or not.
        '''
        return hr >= 0
    
    #---------------------------------------------------------------------------
    #                              GetPorts
    #---------------------------------------------------------------------------
    def GetPorts(self):
        ''' This method gets a list of available ports. '''
        
        # Create a variable to store the port list
        portList = None
        
        try:
            # Get a list of ports
            result = self._bridge.GetPorts()
            
            # Determine if the operation was successful
            status = result[2]
            lastError = result[1]
            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not get a list of ports.\n'
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('GetPorts', msg, self.kCriticalError)
            else:
                portList = result[0]
                
        except Exception as ex:
            msg = 'An exception occurred while getting the port list.\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('GetPorts', msg, self.kException)

        return portList
    
    #---------------------------------------------------------------------------
    #                              OpenPort
    #---------------------------------------------------------------------------
    def OpenPort(self, portName=_kDefaultPortName):
        ''' This method opens the PPCOM port. '''
        
        if (portName is None):
            raise Exception('OpenPort Exception', 'portName can not be "none".')
        
        try:
            self._portName = portName
            result = self._bridge.OpenPort(self._portName)
            
            status = result[1]
            lastError = result[0]
            
            if (status == -2147483647):                
                msg = 'Invalid bridge version.\n'
                #msg += '    Last Error=' + str(lastError)
                #self._writeMessage('OpenPort', msg, self.kWarning)
            elif (self._OperationSucceeded(status) == False):
                msg = 'Could not open port %s.\n' % self._portName
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('OpenPort', msg, self.kCriticalError)
            
        except Exception as ex:
            msg = 'An exception occurred while opening the port.\n'
            msg += '    Exception: ' + str(ex) + '\n'
            msg += '    Port Name: ' + str(portName)
            self._writeMessage('OpenPort', msg, self.kException)
            
        return self._OperationSucceeded(result)
    
    #---------------------------------------------------------------------------
    #                             ClosePort
    #---------------------------------------------------------------------------
    def ClosePort(self):
        ''' This method closes the PPCOM port. '''
        
        result = None
        
        try:
            if self._bridge is not None:
                result = self._bridge.ClosePort()
        
                if (self._OperationSucceeded(result) == False):
                    msg = 'Could not close the port. Result=' + str(result)
                    self._writeMessage('ClosePort', msg, self.kWarning)
        
                # See if any connections still exist
#                numInterfaces = win32com.client.pythoncom._GetInterfaceCount()
#                
#                # See if we need to manually close any interfaces
#                if numInterfaces > 0:
#                    self._bridge = None
#                    try:
#                        win32com.client.pythoncom.CoUninitialize()
#                    except:
#                        pass
#                
#                # See if any connections still exist
#                numInterfaces = win32com.client.pythoncom._GetInterfaceCount()
#                
#                if numInterfaces > 0:
#                    msg = 'A connection to the COM object still exists (%d).' % numInterfaces
#                    self._writeMessage('ClosePort', msg, self.kWarning)
        
        except Exception as ex:
            msg = 'An exception occurred while closing the port.\n'
            msg += '     Exception: ' + str(ex)
            self._writeMessage('ClosePort', msg, self.kException)
        
        return result

    #---------------------------------------------------------------------------
    #                             GetDeviceList
    #---------------------------------------------------------------------------
    def GetDeviceList(self):
        '''
        This method gets a list of devices which can be accessed through
        the PPCOM bridge interface.
        '''
        
        deviceList = None
        
        try:
            result = self._bridge.I2C_GetDeviceList()
            
            status = result[2]
            lastError = result[1]
            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not get a list of devices %s.\n' + self._portName
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('GetDeviceList', msg, self.kCriticalError)
            else:
                deviceList = []
                devices = result[0]
                for i in range(0, len(devices)):
                    #deviceList.append(ord(devices[i]))
                    deviceList.append(devices[i])
        
        except Exception as ex:
            msg = 'An exception occurred while getting the device list.\n'
            msg += '     Exception: ' + str(ex)
            self._writeMessage('GetDeviceList', msg, self.kException)
        
        return deviceList

    #---------------------------------------------------------------------------
    #                              SetProtocol
    #---------------------------------------------------------------------------
    def SetProtocol(self, interface=enumInterfaces.I2C):
        '''
        This method sets the communications protocol implemented by the bridge.
        A list of supported protocols is defined in the enumInterfaces class.
        '''
        
        try:
            result = self._bridge.SetProtocol(interface)
            
            status = result[1]
            lastError = result[0]
            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not set the protocol to %d.\n' % interface
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('SetProtocol', msg, self.kCriticalError)
        
        except Exception as ex:
            msg = 'An exception occurred while setting the protocol.\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('SetProtocol', msg, self.kException)
            
        return


    #---------------------------------------------------------------------------
    #                              SetVoltage
    #---------------------------------------------------------------------------
    def SetVoltage(self, voltage=Voltages.VOLT_50V):
        ''' 
        This method sets the voltage to be supplied by the bridge to the
        device. A list of available voltages is defined in the Voltages class. 
        '''

        try:
            result = self._bridge.SetPowerVoltage(voltage)
            status = result[1]
            lastError = result[0]
            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not set the voltage to %s volts.\n' % str(voltage)
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('SetVoltage', msg, self.kCriticalError)
        
        except Exception as ex:
            msg = 'An exception occurred while setting the voltage.\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('SetVoltage', msg, self.kException)

        return

    #---------------------------------------------------------------------------
    #                              PowerOn
    #---------------------------------------------------------------------------
    def PowerOn(self):
        ''' This method applies power to the device via the bridge. '''

        try:
            result = self._bridge.PowerOn()
            
            status = result[1]
            lastError = result[0]
            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not apply power to the device.\n'
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('PowerOn', msg, self.kWarning)
        
        except Exception as ex:
            msg = 'An exception occurred while applying power.\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('PowerOn', msg, self.kException)

        return
        
    #---------------------------------------------------------------------------
    #                              PowerOff
    #---------------------------------------------------------------------------
    def PowerOff(self):
        ''' This method removes power currently applied to the device. '''

        try:
            result = self._bridge.PowerOff()
            status = result[1]
            lastError = result[0]
            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not remove power from the device.\n'
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('PowerOff', msg, self.kWarning)
        
        except Exception as ex:
            msg = 'An exception occurred while removing power.\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('PowerOff', msg, self.kException)

        return
    
    
    #---------------------------------------------------------------------------
    #                             WaitForPower
    #---------------------------------------------------------------------------
    def WaitForPower(self):
        ''' 
        This method is a blocking routine which waits for power to be
        applied by the bridge before returning.
        '''

        try:
            powerStatus = 0
            errorCount = 0
            
            while (powerStatus == 0):
                result = self._bridge.GetPower()
                
                status = result[2]
                powerStatus = result[0]
                lastError = result[1]
                
                if (self._OperationSucceeded(status) == False):
                    msg = 'Could not get power from the device.\n'
                    msg += '    Status=' + str(status) + '\n'
                    msg += '    Last Error=' + str(lastError)
                    self._writeMessage('WaitForPower', msg, self.kWarning)
                    errorCount += 1
                    
                if (errorCount == 10):
                    msg = 'Failed 10 times to get device power.'
                    self._writeMessage('WaitForPower', msg, self.kWarning)
                    break
        
        except Exception as ex:
            msg = 'An exception occurred while checking device power.\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('WaitForPower', msg, self.kException)
    
        return powerStatus
    
    
    #---------------------------------------------------------------------------
    #                             Get Power
    #---------------------------------------------------------------------------
    def GetPower(self):
        dictM = {}
        
        try:
            result = self._bridge.GetPower()
                    
            status = result[2]
            powerStatus = result[0]
            lastError = result[1]
                        
            # Parse the power status
            fPowerDetected = ((powerStatus & 0x01) == 0x01)
            fPowerSupplied = ((powerStatus & 0x02) == 0x02)
            
            # Get the Vcom and Vaux voltages
            result = self._bridge.GetPower2()
            Vcom = result[1]
            Vaux = result[2]
            
            # Create a dictionary to store the results
            dictM['PowerSupplied'] = fPowerSupplied
            dictM['PowerDetected'] = fPowerDetected
            dictM['Vcom'] = Vcom
            dictM['Vaux'] = Vaux
            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not get power from the device.\n'
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('GetPower', msg, self.kWarning)
                
                
        except Exception as ex:
            msg = 'An exception occurred while checking device power.\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('GetPower', msg, self.kException)
            
        return dictM
    
    #---------------------------------------------------------------------------
    #                             ResetI2CBus
    #---------------------------------------------------------------------------
    def ResetI2CBus(self):
        ''' This method resets the I2C bus. '''

        try:
            result = self._bridge.I2C_ResetBus()
            status = result[1]
            lastError = result[0]
            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not reset the I2C bus.\n'
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('ResetI2CBus', msg, self.kCriticalError)
        
        except Exception as ex:
            msg = 'An exception occurred while resetting the I2C bus.\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('ResetI2CBus', msg, self.kException)

        return

    #---------------------------------------------------------------------------
    #                             SetI2CSpeed
    #---------------------------------------------------------------------------
    def SetI2CSpeed(self, speed=enumI2Cspeed.CLK_400K):
        ''' This method sets the I2C clock speed. '''

        try:
            result = self._bridge.I2C_SetSpeed(speed)
            status = result[1]
            lastError = result[0]
            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not set the I2C clock speed (%d).\n' % speed
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('SetI2CSpeed', msg, self.kCriticalError)
        
        except Exception as ex:
            msg = 'An exception occurred while setting the I2C clock speed.\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('SetI2CSpeed', msg, self.kException)

        return

    
    #---------------------------------------------------------------------------
    #                             ReadI2CData
    #---------------------------------------------------------------------------
    def ReadI2CData(self, deviceAddress=0, offset=0, numBytes=61):
        ''' 
        This method reads I2C data from a specified device at a specified 
        offset. This method handles all logic necessary to perform reads that 
        are larger than 64 bytes in size.
        '''
        
        try:

            # Set the maximum number of bytes
            maxPacketSize = 61
                   
            if numBytes <= maxPacketSize:
                return self._readSmallData(deviceAddress, offset, numBytes)
            else:
                # Create a byte array to store all of the data 
                totalData = array.array('B', [0]*numBytes)
    
                # Get the number of small packets required for assembling a large packet 
                numPackets = int(math.ceil(float(numBytes)/float(maxPacketSize)))
    
                # Create a value to store the number of bytes remaining 
                remainingBytes = numBytes
    
                # Create an index into the large packet 
                index = 0
    
                # Loop through all packets 
                for packetNum in range(0, numPackets):
    
                    # Determine how many bytes to read 
                    bytesToRead = 0;
                    if(remainingBytes > maxPacketSize):
                        bytesToRead = maxPacketSize
                    else:
                        bytesToRead = remainingBytes
                    
                    # Read the small packet 
                    smallPacket = self._readSmallData(deviceAddress, offset, bytesToRead)
    
                    # Save the data to the large packet 
                    for i in range(0, bytesToRead):
                        # Save the data
                        totalData[index] = smallPacket[i]
    
                        # Increment the index
                        index += 1
                    
                    # Add to the offset 
                    offset += maxPacketSize;
    
                    # Subtract the number of remaining bytes 
                    remainingBytes -= maxPacketSize;
                
                # Return the large data packet 
                return totalData;
        
        except Exception as ex:
            msg = 'An exception occurred while reading I2C data.\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('ReadI2CData', msg, self.kException)
                
        return None

    #---------------------------------------------------------------------------
    #                             _readSmallData
    #---------------------------------------------------------------------------
    def _readSmallData(self, deviceAddress=0, offset=0, numBytes=61):
        ''' This "private" method reads small I2C packets (up to 61 bytes). '''
    
        # Set the offset of the read
        self.WriteI2CData(deviceAddress, offset, None)

        data = None

        try:
            # Read the data
            result = self._bridge.I2C_ReadData(deviceAddress, numBytes)

            status = result[2]
            readData = result[0]
            lastError = result[1]
            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not read the I2C data.\n'
                msg += '    Device Address: ' + str(deviceAddress) + '\n'
                msg += '    Offset: ' + str(offset) + '\n'
                msg += '    Number of Bytes: ' + str(numBytes) + '\n'
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('_readSmallData', msg, self.kCriticalError)
            else:
                data = []
                for i in range(0, len(readData)):
                    #data.append(ord(readData[i]))
                    data.append(readData[i])
        
        except Exception as ex:
            msg = 'An exception occurred while reading the I2C data.\n'
            msg += '    Device Address: ' + str(deviceAddress) + '\n'
            msg += '    Offset: ' + str(offset) + '\n'
            msg += '    Number of Bytes: ' + str(numBytes) + '\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('_readSmallData', msg, self.kException)
        
        return data

    #---------------------------------------------------------------------------
    #                             WriteI2CData
    #---------------------------------------------------------------------------
    def WriteI2CData(self, deviceAddress=0, offset=0, data=None):
        
        try:
            # Format the packet
            if data is not None:
                sendData = [offset]
                if type(data) == type([]):
                    sendData.extend(data)
                elif type(data) == type(0):
                    sendData.extend([data])
                else:
                    msg = 'Invalid type for "data". Type should be a list.'
                    self._writeMessage('WriteI2CData', msg, self.kCriticalError)
                    
                sendData = array.array('B', sendData)
            else:
                sendData = array.array('B', [offset])
            
            # Send the data
            result = self._bridge.I2C_SendData(deviceAddress, sendData)
            status = result[1]
            lastError = result[0]
            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not write I2C data.\n'
                msg += '    Device Address: ' + str(deviceAddress) + '\n'
                msg += '    Offset: ' + str(offset) + '\n'
                msg += '    Data: ' + str(data) + '\n'
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('WriteI2CData', msg, self.kCriticalError)
        
        except Exception as ex:
            msg = 'An exception occurred while writing I2C data.\n'
            msg += '    Device Address: ' + str(deviceAddress) + '\n'
            msg += '    Offset: ' + str(offset) + '\n'
            msg += '    Data: ' + str(data) + '\n'
            msg += '    Exception: ' + str(ex)
            #self._writeMessage('WriteI2CData', msg, self.kException)
            
        return



    #---------------------------------------------------------------------------
    #                             ResetBridge
    #---------------------------------------------------------------------------
    def ResetBridge(self):
        data = array.array('B', [0x00])
        #result = self._bridge.I2C_SendData(int(0x82), buffer(data))
        result = self._bridge.I2C_SendData(int(0x82), data)
        
        # Apply XRES
        self.ToggleXRES()
        return
    
    #---------------------------------------------------------------------------
    #                             ToggleXRES
    #---------------------------------------------------------------------------
    def ToggleXRES(self, polarity=ResetPolarity.XRES_POLARITY_NEGATIVE, duration=2):
        ''' Toggle the XRES line. Specify the polarity and duration (in ms). '''
        retVal = self._bridge.ToggleReset(int(polarity), int(duration))        
        return
        
    #---------------------------------------------------------------------------
    #                             SetI2CPins
    #---------------------------------------------------------------------------
    def SetI2CPins(self, pins=I2CPins.kISSPPins97):
        '''
        New versions of the I2C bridge firmware (1.07+) support different 
        pins to communicate over I2C. This method configures which pins to 
        use for I2C Communications.
        '''

        try:
            result = self._bridge.SetProtocolConnector(int(pins))
            status = result[1]
            lastError = result[0]
            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not set the I2C Pins.\n'
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('SetI2CPins', msg, self.kCriticalError)
        
        except Exception as ex:
            msg = 'An exception occurred while setting the protocol.\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('SetI2CPins', msg, self.kException)
        
        return
    


    def EnterInterruptMode(self, deviceAddress=0, bulkBufferSize = 175):

        # Set the interrupt pin
        self._SetInterruptPin();

        # Get the next power of 2 for the buffer size
        bulkSize = self._NextPow2(bulkBufferSize)
        
        # Set the bulk buffer size
        self._SetBulkBufferSize(bulkSize)

        try:
            
            # Break the bulk buffer into two pieces
            bulkSizeHi = (bulkBufferSize >> 8) & 0xff
            bulkSizeLo = (bulkBufferSize) & 0xff
            
            # Send the command to the bridge
            inData = [0x02,  0x00,  0x94, deviceAddress,  0x00,  bulkSizeLo, bulkSizeHi]
            data = array.array('B', inData)
            result = self._bridge.USB2IIC_DataTransfer(data)
            
            status = result[2]
            readData = result[0]
            lastError = result[1]
                
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not enter interrupt mode.\n'
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('EnterInterruptMode', msg, self.kCriticalError)
            else:
                self._SetSink()
                self._interruptMode = True

        except Exception as ex:
            msg = 'An exception occurred while entering interrupt mode.\n'
            msg += '    Device Address: ' + str(deviceAddress) + '\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('EnterInterruptMode', msg, self.kException)

        return
        
    
    def _SetSink(self):
        
        bridgeDevice = self
        
        #class EventSink(win32com.client.getevents('PSoCProgrammerCOM.PSoCProgrammerCOM_Object.8')):
        class EventSink(object):    
            def _IPSoCProgrammerCOM_ObjectEvents_Connected(self, bridge):
                if (bridgeDevice._connectedCallback is not None):
                    bridgeDevice._connectedCallback(bridge)
            
            def _IPSoCProgrammerCOM_ObjectEvents_Disconnected(self, bridge):
                if (bridgeDevice._disconnectedCallback is not None):
                    bridgeDevice._disconnectedCallback(bridge)
            
            def _IPSoCProgrammerCOM_ObjectEvents_Notify(self, this):
                if (bridgeDevice._notifyCallback is not None):
                    bridgeDevice._notifyCallback(this)
                
            def _IPSoCProgrammerCOM_ObjectEvents_USB2IIC_ReceivedData(self,  data):
                if (bridgeDevice._receivedDataCallback is not None):
                    bridgeDevice._receivedDataCallback(data[1:])
                #print data[1:]
    
        self.sink = EventSink()
        self.connection = comtypes.client.GetEvents(self._bridge, self.sink)
    
        return
        
    def RegisterReceivedDataCallback(self, callback):
        self._receivedDataCallback = callback
        return
        
    def RegisterNotifyCallback(self, callback):
        self._notifyCallback = callback
        return
        
    def RegisterConnectedCallback(self, callback):
        self._connectedCallback = callback
        return
        
    def RegisterDisconnectedCallback(self, callback):
        self._disconnectedCallback = callback
        return
    
    def _NextPow2(self, value):
        nextVal = 2**math.ceil(math.log(value) / math.log(2))
        return int(nextVal)
    
    
    def _SetBulkBufferSize(self, bulkBufferSize=255):
        
        try:
            outData = [0]*4
            outData[0] = ((bulkBufferSize >> 0) & 0xff)
            outData[1] = ((bulkBufferSize >> 8) & 0xff)
            outData[2] = ((bulkBufferSize >> 16) & 0xff)
            outData[3] = ((bulkBufferSize >> 24) & 0xff)
            data = array.array('B', outData)
            #result = self._bridge.USB2IIC_AsyncMode1(2, buffer(data))
            result = self._bridge.USB2IIC_AsyncMode1(2, data)
            
            status = result[1]
            lastError = result[0]
                            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not set the bulk buffer size.\n'
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('_SetBulkBufferSize', msg, self.kCriticalError)
            
        except Exception as ex:
            msg = 'An exception occurred while setting the bulk buffer size.\n'
            msg += '    Exception: ' + str(ex) + '\n'
            self._writeMessage('_SetBulkBufferSize', msg, self.kException)
            
        return
        
    def _SetInterruptPin(self):

        try:            
            inData = [0x02,  0x00,  0x90, 0x08]
            data = array.array('B', inData)
            #result = self._bridge.USB2IIC_DataTransfer(buffer(data))
            result = self._bridge.USB2IIC_DataTransfer(data)
            
            status = result[1]
            lastError = result[0]
                            
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not set the interrupt pin.\n'
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('_SetInterruptPin', msg, self.kCriticalError)
        
        except Exception as ex:
            msg = 'An exception occurred while setting the interrupt pin.\n'
            msg += '    Exception: ' + str(ex) + '\n'
            self._writeMessage('_SetInterruptPin', msg, self.kException)
        
        return
            
            
    def ExitInterruptMode(self):
        
        try:
            # Send the command to the bridge
            inData = [0x02,  0x00,  0x93]
            data = array.array('B', inData)
            #result = self._bridge.USB2IIC_DataTransfer(buffer(data))
            result = self._bridge.USB2IIC_DataTransfer(data)
            self._bridge.USB2IIC_AsyncMode(0)
            
            status = result[0]
            readData = result[1]
            lastError = result[2]
    
            if (self._OperationSucceeded(status) == False):
                msg = 'Could not exit interrupt mode.\n'
                msg += '    Status=' + str(status) + '\n'
                msg += '    Last Error=' + str(lastError)
                self._writeMessage('ExitInterruptMode', msg, self.kCriticalError)
            else:
                self._interruptMode = False
    
        except Exception as ex:
            msg = 'An exception occurred while exiting interrupt mode.\n'
            msg += '    Exception: ' + str(ex)
            self._writeMessage('ExitInterruptMode', msg, self.kException)
        
        return
    
    def PumpEvents(self, rate=0):
        comtypes.client.PumpEvents(rate)
        return
    
    def ExitBootloaderMode(self):
        ''' 
        This method assumes the device is already in bootloader mode. The 
        method first starts by finding the bootloader device, then it issues
        the exit-bootloader command.      '''
           
        blAddress = self.deviceAddress   
        attempts = 0
        try:
            while True:    
                attempts += 1            
                # Hard-Code the bootloader address (HACK FOR NOW)
                self.bootloaderAddress = blAddress
            
                # Issue the exit-bootloader command
                exitCommand = [0x7F, 0x01, 0x3B, 0x00, 0x00, 0x4F, 0x6D, 0x17]
                offset = 0
        
                # Write the data            
                self.WriteI2CData(self.bootloaderAddress, offset, exitCommand)                    
                # Wait a second
                sleep(0.1 * attempts)            
                # Verify the device has left bootloader mode
                
                allDevices = []
                while len(allDevices) == 0:
                    allDevices = self.GetDeviceList()
                
                # Remove the bootloader address
                allDevices.remove(blAddress)
                
                if (len(allDevices) > 0) and (allDevices[0] != blAddress):
                    return True
                    break
                
                if attempts == 10:
                    print 'ERROR: Could not exit bootloader mode after 10 attempts.'
                    return False
                    break
                    
        except Exception as ex:                
                msg = 'An exception occurred while exiting Bootloader.\n'
                msg += '    Exception: ' + str(ex)
                self._writeMessage('ExitInterruptMode', msg, self.kException)            
        
    def SetMode(self, mode=enumModes.OPMode):

        ''' This method sets the device mode and waits for the mode-set to complete '''
        try:
            # Read the current mode
            currentMode = self.ReadI2CData(self.deviceAddress, self.I2C_OP_HST_MODE, 5)[0]        
            # Define the mode mask
            modeMask = 0x30        
            # See if we need to switch modes
            temp = (currentMode & modeMask) >> 4
            if (temp == mode):
                return
            
            # Mask out the current host byte mode bits 
            currentMode &= ~modeMask        
            # Shift the mode bits into the correct position
            mode = ((mode << 4) & modeMask)        
            # Or in the current mode
            mode |= currentMode        
            # Or in the mode change request bit
            mode |= 0x08        
            # Write the mode to the device
            self.WriteI2CData(self.deviceAddress, self.I2C_OP_HST_MODE, mode)       
            
            # Wait for the mode change to complete
            newMode = self.ReadI2CData(self.deviceAddress, self.I2C_OP_HST_MODE, 5)[0]        
            while (newMode & 0x08) > 0:
                sleep(0.01)
                newMode = self.ReadI2CData(self.deviceAddress, self.I2C_OP_HST_MODE, 5)[0]
        except Exception as ex:                
                msg = 'An exception occurred while enter xx mode.\n'
                msg += '    Exception: ' + str(ex)
                self._writeMessage('ExitInterruptMode', msg, self.kException)               
        return
    
    def EnterOpMode(self):
        ''' This method sets the device to run in OP mode '''
        self.SetMode(enumModes.OPMode)
        return
        
    def EnterSysMode(self):
        ''' This method sets the device to run in SYS mode '''
        self.SetMode(enumModes.SYSMode)
        return
        
    def EnterCmdMode(self):
        ''' This method sets the device to run in CMD mode '''
        self.SetMode(enumModes.CMDMode)
        return
    
    
    def EnterLowPowerMode(self):
        ''' This method requests the device enter lower power mode. '''
        self._ChangePowerMode(mode=enumPowerModes.LowPower)
        return
        
    def EnterDeepSleep(self):
        ''' This method requests the device enter deep sleep. '''
        self._ChangePowerMode(mode=enumPowerModes.DeepSleep)
        return
        
    def SoftResetDevice(self):
        ''' This method requests the device do a software reset. '''
        # Change the power mode
        self._ChangePowerMode(mode=enumPowerModes.SoftReset)
        
        # Wait for the device to come back in SYS mode
        currentMode = self.ReadI2CData(self.deviceAddress, self.I2C_OP_HST_MODE, 5)[0]
        while (currentMode != 0x10):
            currentMode = self.ReadI2CData(self.deviceAddress, self.I2C_OP_HST_MODE, 5)[0]
            sleep(0.02)
            
        # Switch to OP mode
        self.EnterOpMode()
        
        return
    
    def _ChangePowerMode(self, mode=enumPowerModes.SoftReset):
    
        # Read the current mode
        currentMode = self.ReadI2CData(self.deviceAddress, self.I2C_OP_HST_MODE, 5)[0]
        
        # Clear power mode bits
        currentMode &= ~0x07
        
        # Or in the new mode
        currentMode |= (mode & 0x07)
    
        # Write the mode to the device
        self.WriteI2CData(self.deviceAddress, self.I2C_OP_HST_MODE, mode)
    
        return



    def openDevicePort(self):  
        # Get a list of ports
        portList = self._bridge.GetPorts()[0]
        
        if portList is None:
            #print 'NO BRIDGES AVAILABLE'
            #returnData['ErrorMessage']="NO BRIDGES AVAILABLE"
            #returnData['TestResult']='Fail'
            return False
        else:    
            # Get the first port
            port = None
            for p in portList:
                origP = p
                print p
                p = p.lower()            
                loc = p.find('bridge')
                if loc >= 0:
                    port = origP
                    break        
            
            if port is None:
                #returnData['ErrorMessage']="NO BRIDGES AVAILABLE"
                #returnData['TestResult']='Fail'
                return False
            else:
            # Open the port
                openPortStatus=self._bridge.OpenPort(port)
                #self._bridge.PrintMessages()       
                if openPortStatus:
                    print "port opened"
                    return True
                else:
                    #print "port can not be opened"
                    #returnData['ErrorMessage']="port can not be opened"
                    #returnData['TestResult']='Fail'
                    return False

    
#-------------------------------------------------------------------------------
#                                   main
#-------------------------------------------------------------------------------
               
    

def main():
       
    # Create a bridge object
    bridge = BridgeDevice()
    bridge.PrintMessages()   

    # Get a list of ports
    portList = bridge.GetPorts()
    print 'Port list:', portList
    
    if portList is None:
        print 'NO BRIDGES AVAILABLE'
        return
    
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
    bridge.OpenPort(port)
    bridge.PrintMessages()    
    
    # Remove power
    bridge.PowerOff()
    bridge.PrintMessages()   
    
    # Set the voltage to be applied to the device
    bridge.SetVoltage(Voltages.VOLT_33V)
    #bridge.SetVoltage(Voltages.VOLT_33V)
    bridge.PrintMessages()
    
    # Apply power to the device
    bridge.PowerOn()
    bridge.PrintMessages()
    
    # Wait for device power to be applied
    bridge.WaitForPower()
    bridge.PrintMessages()
    
    # Set the protocol to I2C
    bridge.SetProtocol(enumInterfaces.I2C)
    bridge.PrintMessages()
    
    # Reset the I2C bus
    bridge.ResetI2CBus()
    bridge.PrintMessages()
    
    # Set the I2C Pins to use
    bridge.SetI2CPins(pins=I2CPins.kSPI1315)
    bridge.PrintMessages()
    
    # Set the I2C speed
    bridge.SetI2CSpeed(enumI2Cspeed.CLK_400K)
    bridge.PrintMessages()
    
    # Get a list of devices
    deviceList = bridge.GetDeviceList()
    print 'Device list:', deviceList

    # Get the first device
    #deviceAddress = deviceList[0]
    
    
    # read some data````````````````````````````````````
    data=[0x7F,0x01,0x3B,0x00,0x00,0x4F,0x6D,0x17]
    bridge.WriteI2CData(0x24, 0x00, data)
    
    sleep(0.1)
    bridge.EnterSysMode()
    #bridge.WriteI2CData(0x24,0x00,[0x98])
    datain=[]
    datain=bridge.ReadI2CData(0x24, 0x00, 50)
    print "first\n"
    print datain
    sleep(0.1)
    datain=bridge.ReadI2CData(0x24, 0x00, 50)
    print "second\n"
    print datain
    
    
    #Erase IDAC~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    #bridge.WriteI2CData(0x24,0x00,[0x28])
    bridge.EnterCmdMode()
    sleep(0.1)
    bridge.WriteI2CData(0x24,0x00,[0xA0,0x00,0x10,0x00,0x00,0x01,0xA3,0x00])
    sleep(0.1)
    datain=datain=bridge.ReadI2CData(0x24, 0x00, 100)
    print "before cal"
    print datain
    #bridge.WriteI2CData(0x24,0x00,[0x28])
    bridge.EnterCmdMode()
    print "put finger on TP"
    sleep(5)
    bridge.WriteI2CData(0x24,0x00,[0xA0,0x00,0x09,0x00,0x00,0x00]) #Recal
    #bridge.WriteI2CData(0x24,0x02,[0x11,0x55,0xAA])
    sleep(1)
    
    #bridge.WriteI2CData(0x24,0x00,[0x28])
    bridge.EnterCmdMode()
    sleep(0.1)
    bridge.WriteI2CData(0x24,0x00,[0xA0,0x00,0x10,0x00,0x00,0x01,0xA3,0x00])
    sleep(0.1)
    datain=datain=bridge.ReadI2CData(0x24, 0x00, 100)
    print "AFTER cal"
    print datain
    
    print bridge.GetPower()
    
    # Wait for a bit
    print 'sleep'
    sleep(2.0)

    # Toggle Reset
    bridge.ToggleXRES()
    
    # Wait for a bit
    sleep(3.0)
    
    
    # Remove power
    bridge.PowerOff()
    bridge.PrintMessages()

    # Disconnect from the bridge
    bridge.ClosePort()
    bridge.PrintMessages()

    # Indicate the script has completed
    print 'Script completed...'

    return

if __name__ == '__main__':
    main()

# End of File
