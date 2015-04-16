#!/usr/bin/env python
'''
--------------------------------------------------------------------------------
Module Name:    TTHE_I2C.py
Author:         Jon Peterson (PIJ)
Description:    This module communicates with the I2C bridge using the PPComObj
                module written by BNJ.
                
Dependencies:   1. Numpy
                2. PPComObj_Int
                3. comtypes - http://sourceforge.net/projects/comtypes/files/
--------------------------------------------------------------------------------
            Copyright 2011, Cypress Semiconductor Corporation.
--------------------------------------------------------------------------------
This software is owned by Cypress Semiconductor Corporation (Cypress) and is
protected by and subject to worldwide patent protection (United States and
foreign), United States copyright laws and international treaty provisions.
Cypress hereby grants licensee a personal, non-exclusive, non-transferable
license to copy, use, modify, create derivative works of, and compile the
Cypress Source Code and derivative works for the sole purpose of creating
custom software in support of licensee product to be used in conjunction
with a Cypress integrated circuit as specified in the applicable agreement.
Any reproduction, modification, translation, compilation, or representation
of this software except as specified above is prohibited without the express
written permission of Cypress.

Disclaimer: CYPRESS MAKES NO WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, WITH
REGARD TO THIS MATERIAL, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.
Cypress reserves the right to make changes without further notice to the
materials described herein. Cypress does not assume any liability arising
out of the application or use of any product or circuit described herein.
Cypress does not authorize its products for use as critical components in
life-support systems where a malfunction or failure may reasonably be
expected to result in significant injury to the user. The inclusion of
Cypress' product in a life-support systems application implies that the
manufacturer assumes all risk of such use and in doing so indemnifies
Cypress against all charges. Use may be limited by and subject to the
applicable Cypress software license agreement.
--------------------------------------------------------------------------------
'''
# Imports
import ppcom as pp
from ppcom import enumInterfaces, enumI2Cspeed, Voltages, I2CPins, enumFrequencies
from time import sleep, time
import numpy

# "Defines"
verboseMode = True
bridgeErrorCode = -2147418108
bridgeErrorCode2 = -2147467259
defaultTimeout = 5.0



class enumModes:
    OPMode = 0
    SYSMode = 1
    CMDMode = 2
    
class enumPowerModes:
    SoftReset = 0x01
    DeepSleep = 0x02
    LowPower = 0x04

#-------------------------------------------------------------------------------
#                              TTHE Device Class
#-------------------------------------------------------------------------------
class TTHEDevice(object):
    ''' 
    This class meets the specification for the TrueTouch protocol as 
    implemented on the "Solo" device. For documentation on this interface,
    please see CSJC#013
    '''
    
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
    
    I2C_OP_TOUCH_TouchRecord_Size = 9
    I2C_OP_TOUCH_X_Location = -1
    I2C_OP_TOUCH_X_Size = -1
    I2C_OP_TOUCH_Y_Location = -1
    I2C_OP_TOUCH_Y_Size = -1
    I2C_OP_TOUCH_Pressure_Location = -1
    I2C_OP_TOUCH_Pressure_Size = -1
    I2C_OP_TOUCH_TouchID_Location = -1
    I2C_OP_TOUCH_TouchID_Size = -1
    I2C_OP_TOUCH_EventID_Location = -1
    I2C_OP_TOUCH_EventID_Size = -1
    I2C_OP_TOUCH_TouchType_Location = -1
    I2C_OP_TOUCH_TouchType_Size = -1
    I2C_OP_TOUCH_TouchSize_Location = -1
    I2C_OP_TOUCH_TouchSize_Size = -1
        
    # I2C Register Offsets (SYS)
    I2C_SYS_HST_MODE = 0
    I2C_SYS_RESERVED = 1
    I2C_SYS_MAP_SZH = 2         # Map of the SYS record (high)
    I2C_SYS_MAP_SZL = 3         # Map of the SYS record (low)
    I2C_SYS_CYDATA_OFSH = 4     # MSB of the offset for Cypress Data Record
    I2C_SYS_CYDATA_OFSL = 5     # LSB of the offset for Cypress Data Record
    I2C_SYS_TEST_OFSH = 6       # MSB of the offset for the Test Record
    I2C_SYS_TEST_OFSL = 7       # LSB of the offset for the Test Record
    I2C_SYS_PCFG_OFSH = 8       # MSB of the offset for Panel Configuration Data Record
    I2C_SYS_PCFG_OFSL = 9       # LSB of the offset for Panel Configuration Data Record
    I2C_SYS_OPCFG_OFSH = 0xa    # MSB of the offset for Operating Mode Configuration Record
    I2C_SYS_OPCFG_OFSL = 0xb    # LSB of the offset for Operating Mode Configuration Record
    I2C_SYS_DDATA_OFSH = 0xc    # MSB of the offset for Design Data Record
    I2C_SYS_DDATA_OFSL = 0xd    # LSB of the offset for Design Data Record
    I2C_SYS_MDATA_OFSH = 0xe    # MSB of the offset for Manufacturing Data Record
    I2C_SYS_MDATA_OFSL = 0xf    # LSB of the offset for Manufacturing Data Record
    
    I2C_SYS_CYDATA_TTPIDH = 0           # MSB of the TrueTouch product ID
    I2C_SYS_CYDATA_TTPIDL = 1           # LSB of the TrueTouch product ID
    I2C_SYS_CYDATA_FW_VER_MAJOR = 2     # TrueTouch product firmware version major
    I2C_SYS_CYDATA_FW_VER_MINOR = 3     # TrueTouch product firmware version minor
    I2C_SYS_CYDATA_RECCTRL_0 = 4        # Internal Cypress revision Control Number
    I2C_SYS_CYDATA_RECCTRL_1 = 5
    I2C_SYS_CYDATA_RECCTRL_2 = 6
    I2C_SYS_CYDATA_RECCTRL_3 = 7
    I2C_SYS_CYDATA_RECCTRL_4 = 8
    I2C_SYS_CYDATA_RECCTRL_5 = 9
    I2C_SYS_CYDATA_RECCTRL_6 = 0xa
    I2C_SYS_CYDATA_RECCTRL_7 = 0xb
    I2C_SYS_CYDATA_BL_VER_MAJOR = 0xc   # Bootloader major version
    I2C_SYS_CYDATA_BL_VER_MINOR = 0xd   # Bootloader minor version
    I2C_SYS_CYDATA_JTAG_SI_ID3 = 0xe    # Device JTAG ID (bits 24-31) or Silicon ID (bits 8-15)
    I2C_SYS_CYDATA_JTAG_SI_ID2 = 0xf    # Device JTAG ID (bits 16-23) or Silicon ID (bits 0-7)
    I2C_SYS_CYDATA_JTAG_SI_ID1 = 0x10   # Device JTAG ID (bits 8-15)
    I2C_SYS_CYDATA_JTAG_SI_ID0 = 0x11   # Device JTAG ID (bits 0-7)
    I2C_SYS_CYDATA_MFGID_SZ = 0x12      # Size of Cypress Manufacturing ID (in bytes)
    I2C_SYS_CYDATA_MFG_ID0 = 0x13       # Lot Number [15:8]
    I2C_SYS_CYDATA_MFG_ID1 = 0x14       # Lot Number [7:0]
    I2C_SYS_CYDATA_MFG_ID2 = 0x15       # Wafer[7:0]
    I2C_SYS_CYDATA_MFG_ID3 = 0x16       # X[7:0]
    I2C_SYS_CYDATA_MFG_ID4 = 0x17       # Y[7:0]
    I2C_SYS_CYDATA_MFG_ID5 = 0x18       # Work Week[7:0]
    I2C_SYS_CYDATA_MFG_ID6 = 0x19       # Year[7:0]
    I2C_SYS_CYDATA_MFG_ID7 = 0x1a       # Minor Revision Number [7:0]
    I2C_SYS_CYDATA_CYITO_IDH = 0x1b     # MSB of the CYITO Project ID
    I2C_SYS_CYDATA_CYITO_IDL = 0x1c     # LSB of teh CYITO Project ID
    I2C_SYS_CYDATA_CYITO_VERH = 0x1d    # MSB of the CYITO Project Configuration Version
    I2C_SYS_CYDATA_CYITO_VERL = 0x1e    # LSB of the CYITO Project Configuration Version
    I2C_SYS_CYDATA_TTSP_VER_MAJOR = 0x1f    # TTSP Register Map Major Version
    I2C_SYS_CYDATA_TTSP_VER_MINOR = 0x20    # TTSP Register Map Minor Version
    I2C_SYS_CYDATA_DEVICE_INFO = 0x21   # Device information bits, including endianness and other device-specific information
    
    I2C_SYS_TEST_POST_CODEH = 0         # MSB of the Power-On Self Test (POST) return code
    I2C_SYS_TEST_POST_CODEL = 1         # LSB of the Power-On Self test (POST) return code
    
    I2C_SYS_PCFG_ELECTRODES_X = 0       # X axis number of electrodes
    I2C_SYS_PCFG_ELECTRODES_Y = 1       # Y axis number of electrodes
    I2C_SYS_PCFG_LEN_XH = 2             # MSB of Panel X Axis Length (in 1/100th mm)
    I2C_SYS_PCFG_LEN_XL = 3             # LSB of Panel X Axis Length (in 1/100th mm)
    I2C_SYS_PCFG_LEN_YH = 4             # MSB of Panel Y Axis Length (in 1/100th mm)
    I2C_SYS_PCFG_LEN_YL = 5             # LSB of Panel Y Axis Length (in 1/100th mm)
    I2C_SYS_PCFG_RES_XH = 6             # X-Axis resolution. Most significant 7 bits and X axis origin
    I2C_SYS_PCFG_RES_XL = 7             # X-Axis resolution. Least significant 8 bits
    I2C_SYS_PCFG_RES_YH = 8             # Y-Axis resolution. Most significant 7 bits and Y axis origin
    I2C_SYS_PCFG_RES_YL = 9             # Y-Axis resolution. Least significant 8 bits             
    I2C_SYS_PCFG_MAX_ZH = 0xa           # MSB of maximum Z value
    I2C_SYS_PCFG_MAX_ZL = 0xb           # LSB of maximum Z value
    
    I2C_SYS_OPCFG_CMD_OFS = 0           # OP:COMMAND register offset
    I2C_SYS_OPCFG_REP_OFS = 1           # OP:REP_LEN register offset
    I2C_SYS_OPCFG_REP_SZH = 2           # MSB of the maximum size of the Operating Mode Touch Report
    I2C_SYS_OPCFG_REP_SZL = 3           # LSB of the maximum size of the Operating Mode Touch Report
    I2C_SYS_OPCFG_NUM_BTNS = 4          # Number of buttons or switches.
    I2C_SYS_OPCFG_TT_STAT_OFS = 5       # OP:TT_STAT register offset
    I2C_SYS_OPCFG_OBJ_CFG0 = 6          # Object sensing configuration
    I2C_SYS_OPCFG_MAX_TCHS = 7          # Maximum number of touches reported
    I2C_SYS_OPCFG_TCH_REC_SIZ = 8       # Touch record size (in bytes)
    I2C_SYS_OPCFG_TCH_REC_0 = 9         # Touch record X field location
    I2C_SYS_OPCFG_TCH_REC_1 = 0xa       # Touch record X field size
    I2C_SYS_OPCFG_TCH_REC_2 = 0xb       # Touch record Y field location
    I2C_SYS_OPCFG_TCH_REC_3 = 0xc       # Touch record Y field size
    I2C_SYS_OPCFG_TCH_REC_4 = 0xd       # Touch record Z field location
    I2C_SYS_OPCFG_TCH_REC_5 = 0xe       # Touch record Z field size
    I2C_SYS_OPCFG_TCH_REC_6 = 0xf       # Touch record Touch ID field location
    I2C_SYS_OPCFG_TCH_REC_7 = 0x10      # Touch record Touch ID field size
    I2C_SYS_OPCFG_TCH_REC_8 = 0x11      # Touch record Event ID field location
    I2C_SYS_OPCFG_TCH_REC_9 = 0x12      # Touch record Event ID field size
    I2C_SYS_OPCFG_TCH_REC_10 = 0x13     # Touch record Event ID field size
    I2C_SYS_OPCFG_TCH_REC_11 = 0x14     # Touch record Event ID field size
    I2C_SYS_OPCFG_TCH_REC_12 = 0x15     # Touch record Event ID field size
    I2C_SYS_OPCFG_TCH_REC_13 = 0x16     # Touch record Event ID field size
    I2C_SYS_OPCFG_BTN_REC_SIZ = 0x17    # Button record size (in bytes)
    I2C_SYS_OPCFG_BTN_DIFF_OFS = 0x18   # Button data location (diff counts)
    I2C_SYS_OPCFG_TCH_REC_14 = 0x19     # Touch Record Major Axis Field Location
    I2C_SYS_OPCFG_TCH_REC_15 = 0x1a     # Touch Record Major Axis Field Size
    I2C_SYS_OPCFG_TCH_REC_16 = 0x1b     # Touch Record Minor Axis Field Location
    I2C_SYS_OPCFG_TCH_REC_17 = 0x1c     # Touch Record Minor Axis Field Size
    I2C_SYS_OPCFG_TCH_REC_18 = 0x1d     # Touch Record Orientation Field Location
    I2C_SYS_OPCFG_TCH_REC_19 = 0x1e     # Touch Record Orientation Field Size
    
    # I2C Register Offsets (CMD)
    I2C_CMD_HST_MODE = 0                # Host Mode byte
    I2C_CMD_RESERVED = 1                
    I2C_CMD_COMMAND = 2
    I2C_CMD_DATA = 3                    # 509 bytes
    
    # Packet Lengths
    I2C_SYS_LENGTH = 146    # <-- TODO: Figure this out
    I2C_OP_LENGTH = 0
    
    
    def __init__(self, bridgeID=None, deviceAddress=None, applyPower=False, \
                       voltage=Voltages.VOLT_50V, interruptMode=False):
        '''
        Method Name:    __init__
        Arguments:      bridgeID - a string which specifies the
                        deviceAddress
                        applyPower
        '''
        
        # Initialize class variables
        self._receivedDataCallback = None
        self.interruptMode = interruptMode
        
        # The last SYS packet sent by the device will be saved
        self.sysData = None

        retries = 0
        while (True):

            # Create a bridge object
            self.bridge = pp.BridgeDevice()
            
            # Check for errors
            errors = self.bridge.GetCriticalErrors()
            if len(errors) > 0:
                self.bridge.PrintMessages(errorList = errors)
                self.bridge = None
                print 'ERROR: Could not connect to COM object.'
                raise Exception('Bridge Exception', 'Could not connect to COM object.')
                return
                
        
            # Get the bridge ID
            if bridgeID is None:
            
                # Get a list of all bridges
                allPorts = self.bridge.GetPorts()
                
                # Go through each port and see how many bridges we have
                bridgeIDs = []
                for port in allPorts:
                    # Split the string
                    (name,id) = port.split('/')
                    
                    # If the name matches "bridge" or "TrueTouchBridge" it is a bridge
                    if (name.lower() == 'bridge') or \
                       (name.lower() == 'truetouchbridge'):
                        bridgeIDs.append(port)
        
                # Get the first bridge
                if len(bridgeIDs) > 0:
                    # Look for a TrueTouch bridge first
                    for bid in bridgeIDs:
                        if bid.startswith('True'):
                            bridgeID = bid
                            break
                    
                    if bridgeID is None:
                        bridgeID = bridgeIDs[0]
                else:
                    print 'ERROR: No bridges detected!'
                    raise Exception('Bridge Exception', 'No bridge detected')
                    return
        
                # Send a warning if there were multiple bridges connected
                if verboseMode == True:
                    if len(bridgeIDs) > 1:
                        print 'WARNING: Multiple bridges detected. The bridge that was'
                        print '         selected was: %s' % bridgeID
    
            else:
                if verboseMode == True:
                    print 'Bridge ID manually specified: %s' % bridgeID
        
        
            self.bridgeID = bridgeID
        
            # Open the port
            self.bridge.OpenPort(portName=bridgeID)           
            
            # See if there were any critical errors opening the port
            errors = self.bridge.GetCriticalErrors()
            if len(errors) > 0:
                if verboseMode == True:
                    self.bridge.PrintMessages(errorList = errors)
                
                # Could not connect to the bridge
                print 'Could not connect to the bridge. Resetting the bridge, then retrying.'
                
                # If there were critical errors
                self.bridge.ResetBridge()
        
                # Wait for a bit
                sleep(1.0 + retries*0.5)
                
                # Clear the bridge ID
                bridgeID = None
                
                # Increment the retries
                retries += 1
                
                # See if we need to time out
                if (retries == 5):
                    print 'ERROR: Could not connect to the bridge. Failed with %d retries.' % retries
                    raise Exception('Bridge Exception', 'Could not open bridge port')
                    return
            else:
                break
        
        
        # Apply power to the target
        self.applyPower = applyPower
        self.bridge.SetVoltage(voltage)
        if applyPower == True:
            self.bridge.PowerOn()
            self.bridge.WaitForPower()
            sleep(3.5)
        else:
            self.bridge.PowerOff()
        
        
        # Set the device to use I2C
        self.bridge.SetProtocol(enumInterfaces.I2C)
        self.bridge.ResetI2CBus()
        self.bridge.SetI2CPins(I2CPins.kSPI1315)
        sleep(0.05)
        self.bridge.SetI2CSpeed(enumI2Cspeed.CLK_400K)
        
        # Check for errors
        errors = self.bridge.GetCriticalErrors()
        if len(errors) > 0:
            self.bridge.PrintMessages(errorList = errors)
            self.bridge = None
            return
        
        
        
        # Exit bootloader mode
        print 'Exiting bootloader mode...'
        self.ExitBootloaderMode()
        
        
        # Get the device's I2C address
        if deviceAddress is None:
            
            # Get a list of all devices on the bus
            allDevices = self.bridge.GetDeviceList()

            # Get the first I2C Device
            try:
                deviceAddress = int(allDevices[0])
            except:
                print 'ERROR: Could not get a list of devices.'
                self.bridge.ClosePort()
                self.bridge = None
                raise Exception('Bridge Exception', 'Could get a list of devices.')
                return
            
            if verboseMode == True:
                print 'Connected to I2C device at: %d' % deviceAddress
        else:
            if verboseMode == True:
                print "The device's I2C address was manually specified: %s" % str(deviceAddress)
        
        # Save the device address
        self.deviceAddress = deviceAddress
        
                
        # See if any critical errors have occurred
        errors = self.bridge.GetCriticalErrors()
        for error in errors:
            if error['shouldExit'] == True:
                self.bridge.PrintMessages(errorList=[error])
                raise Exception('Bridge Exception', 'A critical error has occurred.')
                return
                    
        # Get system information
        print 'Getting System Information...'
        self.GetSystemInformation()
        
        # Switch to OP mode
        print 'Entering OP Mode...'
        self.EnterOpMode()
        
        # Register our callback with PPCOM
        self.bridge.RegisterReceivedDataCallback(self._ReceivedDataCallback)
        
        # Set interrupt mode
        if (self.interruptMode == True):
            self.bridge.EnterInterruptMode(self.deviceAddress)
        
        return
        
        
    def _writeData(self, data, offset=0):
        ''' This is a "private" method for writing data to the device. '''
        
        # Set the write request data
        if type(data) == numpy.ndarray:
            data = data.tolist()
        elif type(data) != type([]):
            data = [data]

        # Write the data
        self.bridge.WriteI2CData(self.deviceAddress, offset, data)
        
        return
        
                
    def _readData(self, numBytes=1, offset=0, timeout=defaultTimeout):
        ''' This is a "private" method for reading data from the device. '''
        
        dataValid = False
        startTime = time()
        while (time() < startTime + timeout) and (dataValid == False):

            # Read the data
            data = self.bridge.ReadI2CData(self.deviceAddress, offset, numBytes)
            
            if data is not None:
                try:
                    data = data[0:numBytes]
                    if len(data) == numBytes:
                        dataValid = True
                        break
                except:
                    print 'ERROR (_readData): Data array length is not valid.'
        
        if dataValid == False:
            print 'ERROR (_readData): A timeout occurred.'
            return None
        
        return data
        
    def ApplyPower(self, power=True):
        if power == True:
            self.bridge.PowerOn()
        else:
            self.bridge.PowerOff()
        return

    def GetPower(self):
        return self.bridge.GetPower()

    def ToggleXRES(self, polarity=pp.ResetPolarity.XRES_POLARITY_NEGATIVE, duration=2):
        self.bridge.ToggleXRES(polarity, duration)
        return

    def Disconnect(self):
        ''' This method disconnects from the bridge (COM object). '''
        if self.bridge is not None:
            if self.interruptMode == True:
                self.bridge.ExitInterruptMode()
                
            self.bridge.PowerOff()
            self.bridge.ClosePort()
            self.bridge = None
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
        
    def SetMode(self, mode=enumModes.OPMode):
        ''' This method sets the device mode and waits for the mode-set to complete '''
        
        # Read the current mode
        currentMode = self._readData(1, self.I2C_OP_HST_MODE)[0]
        
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
        self._writeData(data=mode, offset=self.I2C_OP_HST_MODE)
        
        # Wait for the mode change to complete
        newMode = self._readData(1, self.I2C_OP_HST_MODE)[0]
        while (newMode & 0x08) > 0:
            sleep(0.01)
            newMode = self._readData(1, self.I2C_OP_HST_MODE)[0]
        
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
        currentMode = self._readData(1, self.I2C_OP_HST_MODE)[0]
        while (currentMode != 0x10):
            currentMode = self._readData(1, self.I2C_OP_HST_MODE)[0]
            sleep(0.02)
            
        # Switch to OP mode
        self.EnterOpMode()
        
        return
    
    def _ChangePowerMode(self, mode=enumPowerModes.SoftReset):
    
        # Read the current mode
        currentMode = self._readData(1, self.I2C_OP_HST_MODE)[0]
        
        # Clear power mode bits
        currentMode &= ~0x07
        
        # Or in the new mode
        currentMode |= (mode & 0x07)
    
        # Write the mode to the device
        self._writeData(data=mode, offset=self.I2C_OP_HST_MODE)
    
        return
    
    def ExitBootloaderMode(self):
        ''' 
        This method assumes the device is already in bootloader mode. The 
        method first starts by finding the bootloader device, then it issues
        the exit-bootloader command.
        '''
           
        blAddress = 0x24
    
        attempts = 0
        while True:
    
            attempts += 1
            
            # Hard-Code the bootloader address (HACK FOR NOW)
            self.bootloaderAddress = blAddress
        
            # Issue the exit-bootloader command
            exitCommand = [0xFF, 0x01, 0x3b, 0x00, 0x00, 0x4f, 0x6d, 0x17]
            offset = 0
    
            # Write the data
            self.bridge.WriteI2CData(self.bootloaderAddress, offset, exitCommand)
            errors = self.bridge.GetCriticalErrors()
        
            # Wait a second
            sleep(0.5 * attempts)
            
            # Verify the device has left bootloader mode
            allDevices = []
            while len(allDevices) == 0:
                allDevices = self.bridge.GetDeviceList()
            
            # Remove the bootloader address
            allDevices.remove(blAddress)
            
            if (len(allDevices) > 0) and (allDevices[0] != blAddress):
                break
            
            if attempts == 10:
                print 'ERROR: Could not exit bootloader mode after 10 attempts.'
                break
            
        return
    
    def GetSystemInformation(self):
        ''' Enter SYS mode, read data, return a dictionary of system info '''       

        # Switch to SYS mode
        self.EnterSysMode()
        sleep(0.06)
                
        # Read the SYS mode data
        sysData = None
        sysProcessed = None
        retryCount = 0
        while (sysProcessed is None):          
            sysData = self._readData(numBytes=self.I2C_SYS_LENGTH, offset=0)
            
            if sysData is not None:               
                sysProcessed = self._ProcessReceivedData(sysData)
            
            retryCount += 1
            
            if retryCount == 5:
                print 'ERROR getting SYS info.'
                break
    
        # Return the data
        return sysProcessed
        
        
    def GetOPData(self, bytes=0):
    
        if bytes == 0:
            bytes = self.I2C_SYS_OPCFG_TT_STAT_OFS + 1 + 10*(self.I2C_OP_TOUCH_TouchRecord_Size)
        
        # Make sure we are in OP mode
        #self.EnterOpMode()
        
        # Read the OP mode data
        opData = None
        opProcessed = None
        retryCount = 0
        
        while (opProcessed is None):          
            opData = self._readData(numBytes=bytes, offset=0)
            
            if opData is not None:               
                opProcessed = self._ProcessReceivedData(opData)
            
            retryCount += 1
            
            if retryCount == 5:
                print 'ERROR getting OP data.'
                break
    
        # Return the data
        return opProcessed
        
        
    def RegisterReceivedDataCallback(self, callback):
        self._receivedDataCallback = callback
        return
        
    def _ReceivedDataCallback(self, data):                
        # Process the received data
        processedData = self._ProcessReceivedData(data)
        
        print processedData
        
        # Pass the processed data up the stack
        if self._receivedDataCallback is not None:
            self._receivedDataCallback(processedData)
            
        return
        
    def GetI2CEvents(self):
        if self.interruptMode == True:
            self.bridge.PumpEvents()
        return
        
    def _ProcessReceivedData(self,  data):
        '''
        This is a "private" method which processed the received data.
        '''
        #data = data[1:]  # Discard the first byte
        dict = None
        
        # Process the received data
        deviceMode = ((data[self.I2C_OP_HST_MODE] >> 4) & 0x03)
        reservedByte = data[self.I2C_OP_RESERVED]
        if reservedByte == 0x00:
            if deviceMode == 0x00:
                # The device is in OP mode
                #try:
                dict = self._ProcessOpPacket(data)
                #except Exception as ex:
                #    if verboseMode == True:
                #        print 'ERROR: Could not process OP packet.', ex
            elif deviceMode == 0x01:
                # The device is in SYS mode
                try:
                    dict = self._ProcessSysPacket(data)
                    if dict is not None:
                        self.sysData = dict
                except Exception as ex:
                    if verboseMode == True:
                        print 'ERROR: Could not process SYS packet.', ex
            elif deviceMode == 0x02:
                # The device is in CMD mode
                try:
                    dict = self._ProcessCmdPacket(data)
                except:
                    if verboseMode == True:
                        print 'ERROR: Could not process CMD packet.'
            else:
                if verboseMode == True:
                    print 'Incorrect device mode received (%02x).' % deviceMode
                
        return dict
        

    def _ProcessOpPacket(self, data):
        '''
        This is a "private" method which processes valid OP packets.
        '''
        # Create an empty dictionary
        dict = {}
        
        dict['data'] = data        
        
        # Get the report length
        dict['reportLength'] = data[self.I2C_OP_REP_LEN]
        
        # Get the report status
        reportStatus = data[self.I2C_OP_REP_STAT]
        
        dict['noiseEffect'] = (reportStatus >> 2) & 0x7
        dict['counter'] = (reportStatus >> 6) & 0x3
        dict['reportInvalid'] = bool((reportStatus >> 5) & 0x1)
        if dict['reportInvalid'] == True:
            return None
        bootloaderFlag = (reportStatus >> 4) & 0x01
        dict['bootloaderIsRunning'] = bool(bootloaderFlag)
        dict['applicationIsRunning'] = not bool(bootloaderFlag)
            
        # Get the TrueTouch Status
        ttStatus = data[self.I2C_OP_TT_STAT]
        objectDetected = (ttStatus >> 5) & 0x7
        dict['objectDetected'] = objectDetected
        if objectDetected == 0:
            dict['objectDetectedStr'] = 'finger'
        elif objectDetected == 1:
            dict['objectDetectedStr'] = 'large object'
        elif objectDetected == 2:
            dict['objectDetectedStr'] = 'stylus'
        elif objectDetected == 4:
            dict['objectDetectedStr'] = 'hover'
        else:
            if verboseMode == True:
                print 'Unknown object detected in TT_STAT (%02x).' % objectDetected
            
        numTouches = (ttStatus & 0x1F)
        dict['numTouches'] = numTouches
        
        # Get all touch data
        touches = []
        idx = self.I2C_OP_TT_STAT + 1;       
        for i in range(0, numTouches):
            # Get the touch report
            touchReport = data[idx:idx+self.I2C_OP_TOUCH_TouchRecord_Size]

            # Create a dictionary to store the touch data
            touchDict = {}
            totalBytes = 0
            if (len(touchReport) >= (totalBytes + self._GetBytesFromBits(self.I2C_OP_TOUCH_X_Size))):
                touchDict['x'] = self._GetDataFromArray(touchReport, self.I2C_OP_TOUCH_X_Location, self.I2C_OP_TOUCH_X_Size)
                totalBytes += self._GetBytesFromBits(self.I2C_OP_TOUCH_X_Size)
            
            if (len(touchReport) >= (totalBytes + self._GetBytesFromBits(self.I2C_OP_TOUCH_Y_Size))):
                touchDict['y'] = self._GetDataFromArray(touchReport, self.I2C_OP_TOUCH_Y_Location, self.I2C_OP_TOUCH_Y_Size)
                totalBytes += self._GetBytesFromBits(self.I2C_OP_TOUCH_Y_Size)
            
            if (len(touchReport) >= (totalBytes + self._GetBytesFromBits(self.I2C_OP_TOUCH_Pressure_Size))):
                touchDict['z'] = self._GetDataFromArray(touchReport, self.I2C_OP_TOUCH_Pressure_Location, self.I2C_OP_TOUCH_Pressure_Size)
                totalBytes += self._GetBytesFromBits(self.I2C_OP_TOUCH_Pressure_Size)
                touchDict['pressure'] = touchDict['z']

            if (len(touchReport) >= (totalBytes + self._GetBytesFromBits(self.I2C_OP_TOUCH_TouchID_Size))):
                # Get the touch ID
                touchDict['id'] = self._GetDataFromArray(touchReport,self.I2C_OP_TOUCH_TouchID_Location, self.I2C_OP_TOUCH_TouchID_Size)
                totalBytes += self._GetBytesFromBits(self.I2C_OP_TOUCH_TouchID_Size)
                
                # Get the event ID
                touchDict['eventID'] = self._GetDataFromArray(touchReport, self.I2C_OP_TOUCH_TouchID_Location, 8)
                touchDict['eventID'] >>= self.I2C_OP_TOUCH_TouchID_Size
                bitMask = 0
                for i in range(0, self.I2C_OP_TOUCH_EventID_Size):
                    bitMask <<= 1
                    bitMask += 1
                touchDict['eventID'] &= bitMask
                
                # Determine what kind of touch it was
                if touchDict['eventID'] == 0:
                    touchDict['eventID_str'] = 'No Event'
                elif touchDict['eventID'] == 1:
                    touchDict['eventID_str'] = 'Touchdown'
                elif touchDict['eventID'] == 2:
                    touchDict['eventID_str'] = 'Significant Displacement'
                elif touchDict['eventID'] == 3:
                    touchDict['eventID_str'] = 'Liftoff'
                else:
                    touchDict['eventID_str'] = 'UNKNOWN'
                    
                
            #if (len(touchReport) >= (totalBytes + self._GetBytesFromBits(self.I2C_OP_TOUCH_TouchType_Size))):
            #    touchDict['eventType'] = self._GetDataFromArray(touchReport, self.I2C_OP_TOUCH_TouchType_Location, self.I2C_OP_TOUCH_TouchType_Size)
            #    totalBytes += self._GetBytesFromBits(self.I2C_OP_TOUCH_TouchType_Size)
            
            #if (len(touchReport) >= (totalBytes + self._GetBytesFromBits(self.I2C_OP_TOUCH_TouchSize_Size))):            
            #    touchDict['size'] = self._GetDataFromArray(touchReport, self.I2C_OP_TOUCH_TouchSize_Location, self.I2C_OP_TOUCH_TouchSize_Size)
            #    totalBytes += self._GetBytesFromBits(self.I2C_OP_TOUCH_TouchSize_Size)
            
            #print 'TOUCH ID:', touchDict['id'], ' LOCATION:', self.I2C_OP_TOUCH_TouchID_Location, ' SIZE:', self.I2C_OP_TOUCH_TouchID_Size, ' DATA:', touchReport[self.I2C_OP_TOUCH_TouchID_Location]
            #print 'EVENT ID:', touchDict['eventID'], ' LOCATION:', self.I2C_OP_TOUCH_TouchID_Location,' DATA:', touchReport[self.I2C_OP_TOUCH_TouchID_Location]
            
            touches.append(touchDict)
                    
            # Increment the index
            idx += self.I2C_OP_TOUCH_TouchRecord_Size
        
        dict['touchArray'] = touches
                
        dict['lastIndex'] = idx
                
        # If a callback has been registered, pass up the data
        #if self.opCallback is not None:
        #    self.opCallback(dict)
        
        return dict
        
    def _GetBytesFromBits(self, numBits):
        numBytes = int(numpy.ceil(float(numBits)/8.))
        return numBytes
    
    def _GetDataFromArray(self, array=[], location=0, size=8):
        '''
        Helper method to return data of an arbitrary size from an array.
        location is an index.  size is in bits
        '''
                
        # Calculate the number of bytes to read
        numBytes = self._GetBytesFromBits(size) 
        
        # Get the data
        data = 0;
        for i in range(location, location+numBytes):
            if i < len(array):
                shiftAmt = (8*(numBytes-(i-location)-1))
                data += array[i] << shiftAmt
            else:
                data = 0
                break
              
        # Mask off the bits not used
        bitMask = 0
        for i in range(0, size):
            bitMask <<= 1
            bitMask += 1
        data &= bitMask    
                
        return data
        
        
    def _ProcessSysPacket(self, data):
        '''
        This is a "private" method which processes valid SYS packets.
        '''       
        
        if data is None:
            if verboseMode == True:
                print 'ERROR (_ProcessSysPacket): Data is None.'
            return
        
        # Create an empty dictionary
        dict = {}
        
        # Get the size of the register map
        mapSize = (int(data[self.I2C_SYS_MAP_SZH]) << 8) + data[self.I2C_SYS_MAP_SZL]
        
        # Get the Data offsets
        cyDataOffset = (int(data[self.I2C_SYS_CYDATA_OFSH]) << 8) + data[self.I2C_SYS_CYDATA_OFSL]
        testOffset = (int(data[self.I2C_SYS_TEST_OFSH]) << 8) + data[self.I2C_SYS_TEST_OFSL]
        pcfgOffset = (int(data[self.I2C_SYS_PCFG_OFSH]) << 8) + data[self.I2C_SYS_PCFG_OFSL]
        opcfgOffset = (int(data[self.I2C_SYS_OPCFG_OFSH]) << 8) + data[self.I2C_SYS_OPCFG_OFSL]
        ddataOffset = (int(data[self.I2C_SYS_DDATA_OFSH]) << 8) + data[self.I2C_SYS_DDATA_OFSL]
        mdataOffset = (int(data[self.I2C_SYS_MDATA_OFSH]) << 8) + data[self.I2C_SYS_MDATA_OFSL]

        # Get CYDATA
        cyDataSize = testOffset - cyDataOffset
        cyData = data[cyDataOffset:cyDataOffset+cyDataSize]
        cyDataDict = {}
                

        # Get individual fields
        cyDataDict['TrueTouchPID'] = (cyData[self.I2C_SYS_CYDATA_TTPIDH] << 8) + \
                                     (cyData[self.I2C_SYS_CYDATA_TTPIDL])
        cyDataDict['FirmwareVersionMajor'] = cyData[self.I2C_SYS_CYDATA_FW_VER_MAJOR]
        cyDataDict['FirmwareVersionMinor'] = cyData[self.I2C_SYS_CYDATA_FW_VER_MINOR]
        cyDataDict['RevisionControlArray'] = cyData[self.I2C_SYS_CYDATA_RECCTRL_0:self.I2C_SYS_CYDATA_RECCTRL_7+1]
        cyDataDict['RevisionControlNumber'] = long(0)
        for i, value in enumerate(cyDataDict['RevisionControlArray']):
            shiftAmt = 8 * (len(cyDataDict['RevisionControlArray']) - (i+1))
            cyDataDict['RevisionControlNumber'] += (value << shiftAmt)          
        cyDataDict['BootloaderVersionMajor'] = cyData[self.I2C_SYS_CYDATA_BL_VER_MAJOR]
        cyDataDict['BootloaderVersionMinor'] = cyData[self.I2C_SYS_CYDATA_BL_VER_MINOR]
        cyDataDict['SiliconID'] = (int(cyData[self.I2C_SYS_CYDATA_JTAG_SI_ID3]) << 8) + \
                                   cyData[self.I2C_SYS_CYDATA_JTAG_SI_ID2]
        cyDataDict['JtagID'] = (int(cyData[self.I2C_SYS_CYDATA_JTAG_SI_ID3]) << 24) + \
                               (int(cyData[self.I2C_SYS_CYDATA_JTAG_SI_ID2]) << 16) + \
                               (int(cyData[self.I2C_SYS_CYDATA_JTAG_SI_ID1]) << 8) + \
                               (int(cyData[self.I2C_SYS_CYDATA_JTAG_SI_ID0]))
        
        midSize = cyData[self.I2C_SYS_CYDATA_MFGID_SZ]
        cyDataDict['MFG_ID'] = cyData[self.I2C_SYS_CYDATA_MFG_ID0:self.I2C_SYS_CYDATA_MFG_ID0+midSize]
        if midSize == 8:
            cyDataDict['MFG_LotNumber'] = (int(cyData[self.I2C_SYS_CYDATA_MFG_ID0]) << 8) + \
                                      cyData[self.I2C_SYS_CYDATA_MFG_ID1]
            cyDataDict['MFG_Wafer'] = cyData[self.I2C_SYS_CYDATA_MFG_ID2]
            cyDataDict['MFG_X'] = cyData[self.I2C_SYS_CYDATA_MFG_ID3]
            cyDataDict['MFG_Y'] = cyData[self.I2C_SYS_CYDATA_MFG_ID4]
            cyDataDict['MFG_WorkWeek'] = cyData[self.I2C_SYS_CYDATA_MFG_ID5]
            cyDataDict['MFG_Year'] = cyData[self.I2C_SYS_CYDATA_MFG_ID6]
            cyDataDict['MFG_MinorRevisionNumber'] = cyData[self.I2C_SYS_CYDATA_MFG_ID7]
    
        cyDataDict['CYITO_ProjectID'] = (int(cyData[self.I2C_SYS_CYDATA_CYITO_IDH]) << 8) + \
                                        cyData[self.I2C_SYS_CYDATA_CYITO_IDL]
        cyDataDict['CYITO_ConfigVersion'] = (int(cyData[self.I2C_SYS_CYDATA_CYITO_VERH]) << 8) + \
                                             cyData[self.I2C_SYS_CYDATA_CYITO_VERL]

        
        dict['CYDATA'] = cyDataDict
        
        
        # Get TEST data
        testDataSize = pcfgOffset - testOffset
        testData = data[testOffset:testOffset+testDataSize]
        
        testDict = {}
        testDict['PowerOnSelfTest'] = (int(testData[self.I2C_SYS_TEST_POST_CODEH]) << 8) + \
                                      testData[self.I2C_SYS_TEST_POST_CODEL]
            
        dict['TEST'] = testDict
        
        
        # Get PCFG data
        pcfgDataSize = opcfgOffset - pcfgOffset
        pcfgData = data[pcfgOffset:pcfgOffset+pcfgDataSize]
        
        pcfgDict = {}
        pcfgDict['ElectrodesX'] = pcfgData[self.I2C_SYS_PCFG_ELECTRODES_X]
        pcfgDict['ElectrodesY'] = pcfgData[self.I2C_SYS_PCFG_ELECTRODES_Y]
        pcfgDict['LengthX'] = (int(pcfgData[self.I2C_SYS_PCFG_LEN_XH]) << 8) + \
                              pcfgData[self.I2C_SYS_PCFG_LEN_XL]
        pcfgDict['LengthY'] = (int(pcfgData[self.I2C_SYS_PCFG_LEN_YH]) << 8) + \
                              pcfgData[self.I2C_SYS_PCFG_LEN_YL]
        pcfgDict['xOrigin'] = (pcfgData[self.I2C_SYS_PCFG_RES_XH] >> 7) & 0x01
        pcfgDict['xRes'] = (int(pcfgData[self.I2C_SYS_PCFG_RES_XH] & 0x7F) << 8) + \
                              pcfgData[self.I2C_SYS_PCFG_RES_XL]
        pcfgDict['yOrigin'] = (pcfgData[self.I2C_SYS_PCFG_RES_YH] >> 7) & 0x01
        pcfgDict['yRes'] = (int(pcfgData[self.I2C_SYS_PCFG_RES_YH] & 0x7F) << 8) + \
                              pcfgData[self.I2C_SYS_PCFG_RES_YL]
        pcfgDict['zRes'] = (int(pcfgData[self.I2C_SYS_PCFG_MAX_ZH]) << 8) + \
                              pcfgData[self.I2C_SYS_PCFG_MAX_ZL]

            
        dict['PCFG'] = pcfgDict
        
        # Get OPCFG data
        opcfgDataSize = ddataOffset - opcfgOffset
        opcfgData = data[opcfgOffset:opcfgOffset+opcfgDataSize]
        

        opcfgDict = {}
        
        # Make sure our operation mode offsets are correct
        self.I2C_OP_COMMAND = opcfgData[self.I2C_SYS_OPCFG_CMD_OFS]
        self.I2C_OP_REP_LEN = opcfgData[self.I2C_SYS_OPCFG_REP_OFS]
        self.I2C_OP_LENGTH = (int(opcfgData[self.I2C_SYS_OPCFG_REP_SZH]) << 8) + \
                        opcfgData[self.I2C_SYS_OPCFG_REP_SZL]
        opcfgDict['numButtons'] = opcfgData[self.I2C_SYS_OPCFG_NUM_BTNS]
        self.I2C_OP_TT_STAT = opcfgData[self.I2C_SYS_OPCFG_TT_STAT_OFS]
        opcfgDict['objectSensingCfg'] = opcfgData[self.I2C_SYS_OPCFG_OBJ_CFG0]
        opcfgDict['maxTouches'] = opcfgData[self.I2C_SYS_OPCFG_MAX_TCHS]
        
        self.I2C_OP_TOUCH_TouchRecord_Size = opcfgData[self.I2C_SYS_OPCFG_TCH_REC_SIZ]
        touchRecord = opcfgData[self.I2C_SYS_OPCFG_TCH_REC_0:self.I2C_SYS_OPCFG_TCH_REC_0+14]  # TODO: Don't hard code 14
        opcfgDict['touchRecord'] = touchRecord
                            
        # Parse the touch record
        self.I2C_OP_TOUCH_X_Location = touchRecord[0]
        self.I2C_OP_TOUCH_X_Size = touchRecord[1]
        self.I2C_OP_TOUCH_Y_Location = touchRecord[2]
        self.I2C_OP_TOUCH_Y_Size = touchRecord[3]
        self.I2C_OP_TOUCH_Pressure_Location = touchRecord[4]
        self.I2C_OP_TOUCH_Pressure_Size = touchRecord[5]
        self.I2C_OP_TOUCH_TouchID_Location = touchRecord[6]
        self.I2C_OP_TOUCH_TouchID_Size = touchRecord[7]
        self.I2C_OP_TOUCH_EventID_Location = touchRecord[8]
        self.I2C_OP_TOUCH_EventID_Size = touchRecord[9]
        self.I2C_OP_TOUCH_TouchType_Location = touchRecord[10]
        self.I2C_OP_TOUCH_TouchType_Size = touchRecord[11]
        self.I2C_OP_TOUCH_TouchSize_Location = touchRecord[12]
        self.I2C_OP_TOUCH_TouchSize_Size = touchRecord[13]        
                       
        # TODO:  PARSE BUTTON DATA
        
        # TODO:  Append the touch record with the orientation data
        
        
        dict['OPCFG'] = opcfgDict
        
        # TODO:
        # Get DDATA
        # THIS IS CURRENTLY UNIMPLEMENTED.  THE SPEC NEEDS TO BE UPDATED TO
        # INCLUDE THE DDATA AREA. (SEE CSJC#013).
        
        #ddataSize = mdataOffset - ddataOffset
        #dData = data[ddataOffset:ddataOffset+ddataSize]
        
        #dict['DDATA'] = None
        
        # Get MDATA
        # THIS IS CURRENTLY NOT IMPLEMENTED, SINCE THE MAXIMUM REPORT LENGTH
        # IS 64 BYTES. THE CURRENT BRIDGE IS A HID DEVICE, SO THE PACKET
        # LENGTH IS TOO SMALL TO CONTAIN THIS DATA
        
        #dict['MDATA'] = None
        
        #print 'SYS Packet:'
        #print dict

        # If a callback has been registered, pass up the data
        #if self.sysCallback is not None:
        #    self.sysCallback(dict)

        return dict
        
    def _ProcessCmdPacket(self, data):
        '''
        This is a "private" method which processes valid CMD packets.
        '''

        # Create an empty dictionary object
        dict = {}
        dict['HST_MODE'] = data[self.I2C_CMD_HST_MODE]
        dict['command'] = data[self.I2C_CMD_COMMAND]
        dict['data'] = data[self.I2C_CMD_DATA:]

        #print 'CMD Packet:'
        #print dict
        
        # If a callback has been registered, pass up the data
        #if self.cmdCallback is not None:
        #    self.cmdCallback(dict)

        return dict

    
    def SendOpCommand(self, command, data):
        ''' This method sends an Operating Mode (OP) command '''
                
        # Wait for a previous command to complete
        self._waitForCommandComplete()
       
        # Read the latest command byte
        commandByte = self._readData(numBytes=self.I2C_OP_COMMAND+1, timeout=defaultTimeout)[self.I2C_OP_COMMAND]
       
        # Write data
        if len(data) > 6:
            print 'ERROR: data length is too large (%d > 6)' % len(data)
        self._writeData(data=data, offset=self.I2C_OP_COMMAND_DATA_0)
        
        # Create the command
        mask = (self.I2C_OP_COMMAND_COMPLETE_BIT | self.I2C_OP_COMMAND_TOGGLE_BIT)
        command &= ~mask
        command |= (commandByte & mask)
        command ^= self.I2C_OP_COMMAND_TOGGLE_BIT
        command &= ~self.I2C_OP_COMMAND_COMPLETE_BIT
        
        # Write the command
        self._writeData(data=command, offset=self.I2C_OP_COMMAND)
        
        # Wait for the command to complete
        self._waitForCommandComplete()
        
        return
        
        
    def ReadOpCommand(self):
        ''' This method reads the current OP Command buffer '''
        
        # Wait for any previous commands to complete
        self._waitForCommandComplete()
        
        # Read all of the data
        data = self._readData(numBytes=(self.I2C_OP_COMMAND_DATA_5+1), timeout=defaultTimeout)
        
        return data[self.I2C_OP_COMMAND:(self.I2C_OP_COMMAND_DATA_5+1)]


    def SendCatCommand(self, command, data, timeout=20.0):
        ''' This method sends a Command and Test (CAT) command '''
                
        # Wait for a previous command to complete
        self._waitForCommandComplete()
       
        # Read the latest command byte
        commandByte = self._readData(numBytes=self.I2C_CMD_COMMAND+1, timeout=timeout)[self.I2C_CMD_COMMAND]
       
        # Write data
        if len(data) > 509:
            print 'ERROR: data length is too large (%d > 509)' % len(data)
        self._writeData(data=data, offset=self.I2C_CMD_DATA)
        
        # Create the command
        mask = (self.I2C_OP_COMMAND_COMPLETE_BIT | self.I2C_OP_COMMAND_TOGGLE_BIT)
        command &= ~mask
        command |= (commandByte & mask)
        command ^= self.I2C_OP_COMMAND_TOGGLE_BIT
        command &= ~self.I2C_OP_COMMAND_COMPLETE_BIT
        
        # Write the command
        self._writeData(data=command, offset=self.I2C_CMD_COMMAND)
        
        # Wait for the command to complete
        self._waitForCommandComplete(timeout)
        
        return


    def ReadCatCommand(self):
        ''' This method reads the current OP Command buffer '''
        
        # Wait for any previous commands to complete
        self._waitForCommandComplete()
        
        # Read all of the data
        data = self._readData(numBytes=(512), timeout=20.0)
        
        return data[self.I2C_CMD_COMMAND+1:512]


    def _waitForCommandComplete(self, timeout=defaultTimeout):
        ''' This "private" method waits for the command-complete bit to be set '''

        startTime = time()

         # Read the data in the command byte
        commandByte = self._readData(numBytes=self.I2C_OP_COMMAND+1, timeout=timeout)[self.I2C_OP_COMMAND]
        
        # Wait for the device to finish processing a command
        while (commandByte & self.I2C_OP_COMMAND_COMPLETE_BIT) != self.I2C_OP_COMMAND_COMPLETE_BIT:
            if time() > (startTime + timeout):
                raise Exception('Timeout', 'Waiting for command to complete')
            
            sleep(0.01)
            # Read the data in the command byte
            commandByte = self._readData(numBytes=self.I2C_OP_COMMAND+1, timeout=timeout)[self.I2C_OP_COMMAND]

        return


#-------------------------------------------------------------------------------
#                                  Main
#-------------------------------------------------------------------------------
def main():
    
    # Create a new TTHE device
    print 'Initializing the TTHE device.'
    d = TTHEDevice(applyPower=True, interruptMode=True)       
   
    # Enter OP Mode
    #print 'Switching to OP mode...'
    #d.EnterOpMode()
    
    # Enter SYS mode
    #print 'Switching to SYS mode...'
    #d.EnterSysMode()
    
    print '--------------------------------------------------------------------'
    print 'SYS Packet:'
    print d.sysData
    
    print '--------------------------------------------------------------------'
    print 'CMD Packet:'
    d.EnterCmdMode()
    
    # Get the Flash Row Size
    d.SendCatCommand(0x02, []) # Get config row size
    retVal = d.ReadCatCommand()
    rowSize = (int(retVal[0]) << 8) + int(retVal[1])
    print 'Row Size: %d' % rowSize
    
    # Read a config block
    configBlockNumber = 0x00
    ebid = 0x00 # Enumerated Block ID
    d.SendCatCommand(0x03, [(configBlockNumber & 0xFF), ((configBlockNumber >> 8) & 0xFF), (rowSize & 0xFF), ((rowSize >> 8) & 0xFF), ebid]) # Read a config block
    print d.ReadCatCommand()[0:rowSize]
    
    # Enter OP Mode
    print '--------------------------------------------------------------------'
    print 'Switching to OP mode...'
    d.EnterOpMode()
    
    # Get some data
    #print 'Displaying some OP-Mode packets...'
    #for i in range(0, 150):
    #    data = d._readData(numBytes=16, timeout=5)
    #    for char in data:
    #        print '0x%02x' % char,
    #    print ''
    #    sleep(0.05)
    
    
    for i in range(0, 1000):
        d.GetI2CEvents()
        sleep(0.01)
    
    
    # Disconnect from the TTHE Device
    print 'Disconnecting...'
    d.Disconnect()
    
    print 'Script completed...'
    
    return
    
    
if __name__ == '__main__':
    main()

# End of File