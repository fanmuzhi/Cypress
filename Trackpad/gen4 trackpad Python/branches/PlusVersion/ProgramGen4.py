
# Imports
from PythonDriver import ppcom as pp
#import ppcom as pp
from PythonDriver.ppcom import enumInterfaces, enumI2Cspeed, Voltages, I2CPins, enumFrequencies
from time import sleep
import array
import json

FWPath="d:/101-20059-01R05V0.9.hex"
returnData={}
returnData['TestItem']='program'
returnData['TestResult']='Pass'
returnData['ErrorCode']=0
returnData['ErrorMessage']=""

#Error constants (Gen4 Programming)
S_OK = 0
E_FAIL = -1

#Chip Level Protection constants (Gen4 Programming)
CHIP_PROT_VIRGIN = 0x00
CHIP_PROT_OPEN = 0x01
CHIP_PROT_PROTECTED = 0x02
CHIP_PROT_KILL = 0x04
CHIP_PROT_MASK = 0x0F

# Programming return value index
kRetVal = 0
kStrError = 1
kSuccess = 2

# Settings
maxRetries = 5  # number of times to try to connect to the bridge


class Gen4Device(object):
    
    def __init__(self):
        ''' 
        Object constructor. This method automatically detects a bridge and 
        connects to it.
        '''
        
        retries = 0
        while True:
        
            # Create a reference to the bridge object
            self.bridge = pp.BridgeDevice()
    
            # Check for errors
            errors = self.bridge.GetCriticalErrors()
            if len(errors) > 0:
                self.bridge.PrintMessages(errorList = errors)
                self.bridge = None
                returnData['ErrorMessage']="Could not connect to COM object"
                returnData['TestResult']='Fail'
                #print 'ERROR: Could not connect to COM object.'
                raise Exception('Bridge Exception', 'Could not connect to COM object.')
                return
            
            # Open Port - get last (connected) port in the ports list
            hResult = self.bridge.GetPorts()                  
            hr = hResult
            portArray = hResult        
            
            if (len(portArray) <= 0):
                returnData['ErrorMessage']="Port Open Error"
                returnData['TestResult']='Fail'
                raise Exception('Port Open Error', 2, hr, 'Connect a programmer to the PC.')
                return -1
                
            bFound = 0
            for i in range(0, len(portArray)):
                if (portArray[i].startswith("MiniProg3") or portArray[i].startswith("TrueTouchBridge")):
                    portName = portArray[i]            
                    bFound = 1
                    break
                    
            if(bFound == 0):
                returnData['ErrorMessage']="Port Open Error"
                returnData['TestResult']='Fail'
                raise Exception('Port Open Error', 3, hr, 'Connect a MiniProg3 or TrueTouchBridge to the PC.')
                return -1
                    
            # Open the port
            self.bridge.OpenPort(portName)
        
            # See if there were any critical errors opening the port
            errors = self.bridge.GetCriticalErrors()
            if len(errors) > 0:
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
                if (retries == maxRetries):
                    returnData['ErrorMessage']="Could not connect to the bridge."
                    returnData['TestResult']='Fail'
                    print 'ERROR: Could not connect to the bridge. Failed with %d retries.' % retries
                    raise Exception('Bridge Exception', 'Could not open bridge port')
                    return
            else:
                break
    
        # Remove Power
        self.bridge.PowerOff()
        
        # Initialize the port
        self._InitializePortForProgramming()
        
        # Apply power
        self.bridge.SetVoltage(Voltages.VOLT_33V)
        self.bridge.PowerOn()
        sleep(2.0)
        
        return
        
    def Disconnect(self):
        ''' Disconnect from the bridge. '''
        self.bridge.PowerOff()
        self.bridge.ClosePort()
        self.bridge = None
        return

    def ToggleXRES(self, polarity=pp.ResetPolarity.XRES_POLARITY_NEGATIVE, duration=2):
        ''' Toggle the XRES line. Specify the polarity and duration (in ms). '''
        retVal = self.bridge.ToggleXRES(int(polarity), int(duration))        
        return
            
    def ProgramDevice(self, hexFilePath):
        ''' Performs programming of the Gen4 device.'''
        
        # Read the hex file
        hResult = self.bridge._bridge.ReadHexFile(hexFilePath)          
        hr = hResult[kSuccess]        
        hexImageSize = int(hResult[kRetVal])
        strLastError = hResult[kStrError]
        if (not self._ProgrammingSucceeded(hr)):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            raise Exception('Programming Error', 1, hr, strLastError)
        
        # Read the chip protection level
        hResult = self.bridge._bridge.HEX_ReadChipProtection()
        hr = hResult[kSuccess]
        hex_chipProt = hResult[kRetVal]
        strLastError = hResult[kStrError]
        if (not self._ProgrammingSucceeded(hr)):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            raise Exception('Programming Error', 2, hr, strLastError)
                
        if (hex_chipProt[0] == CHIP_PROT_VIRGIN):
            strLastError = 'Transition to VIRGIN is not allowed. It will destroy the chip. Please contact Cypress if you need this specifically.'
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            raise Exception('Programming Error', 3, hr, strLastError)
          
        # Set acquire mode to reset
        self.bridge._bridge.SetAcquireMode('Power')
        
        # Acquire the device
        hResult = self.bridge._bridge.DAP_Acquire()        
        hr = hResult[1]
        strLastError = hResult[0]
        if (not self._ProgrammingSucceeded(hr)):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            raise Exception('Programming Error', 4, hr, strLastError)
        
        # Erase All
        hr = self._PSoC4_EraseAll()
        if (not self._ProgrammingSucceeded(hr)):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            #raise Exception('Programming Error', 5, hr, 'Failed to erase flash.')
        
        #Find checksum of Privileged Flash. Will be used in calculation of User CheckSum later    
        hResult = self.bridge._bridge.PSoC4_CheckSum(0x8000) #CheckSum All Flash ("Privileged + User" Rows)
        hr = hResult[kSuccess]
        checkSum_Privileged = hResult[kRetVal]
        strLastError = hResult[kStrError]
        if (not self._ProgrammingSucceeded(hr)):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            raise Exception('Programming Error', 6, hr, strLastError)
        
        #Program Flash
        hr = self._ProgramFlash(hexImageSize)
        if (not self._ProgrammingSucceeded(hr)):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            raise Exception('Programming Error', 7, hr, 'Error programming flash.')
        
        #Verify Rows
        hr = self._PSoC4_VerifyFlash(hexImageSize)
        if (not self._ProgrammingSucceeded(hr)):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            raise Exception('Programming Error', 8, hr, 'Error verifying rows.')
        
        #Protect All arrays
        hResult = self.bridge._bridge.PSoC4_ProtectAll()
        hr = hResult[1]
        strLastError = hResult[0]
        if (not self._ProgrammingSucceeded(hr)):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            raise Exception('Programming Error', 9, hr, strLastError)
        
        #Verify protection ChipLevelProtection and Protection data
        hResult = self.bridge._bridge.PSoC4_VerifyProtect()        
        hr = hResult[1]
        strLastError = hResult[0]
        if (not self._ProgrammingSucceeded(hr)):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            raise Exception('Programming Error', 10, hr, strLastError)
        
        #CheckSum verification
        hResult = self.bridge._bridge.PSoC4_CheckSum(0x8000) #CheckSum All Flash (Privileged + User)        
        hr = hResult[1]
        checkSum_UserPrivileged = hResult[kRetVal]
        strLastError = hResult[0]
        if (not self._ProgrammingSucceeded(hr)):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            raise Exception('Programming Error', 11, hr, strLastError)
        checkSum_User = checkSum_UserPrivileged - checkSum_Privileged #find checksum of User Flash rows
        
        hResult = self.bridge._bridge.ReadHexChecksum()
        hr = hResult[kSuccess]
        hexChecksum = hResult[kRetVal]
        strLastError = hResult[kStrError]
        if (not self._ProgrammingSucceeded(hr)):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            raise Exception('Programming Error', 11, hr, strLastError)
        checkSum_User = checkSum_User & 0xFFFF
        
        if (checkSum_User != hexChecksum):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            strLastError = "Mismatch of Checksum: Expected 0x%x, Got 0x%x" % (checkSum_User, hexChecksum)        
            raise Exception('Programming Error', 12, E_FAIL, strLastError)
        else:
            returnData['Data']=checkSum_User
            #print "Checksum 0x%x" % (checkSum_User)
    
        #Release PSoC3 device
        hResult = self.bridge._bridge.DAP_ReleaseChip()        
        hr = hResult[1]
        strLastError = hResult[0]
        if (not self._ProgrammingSucceeded(hr)):
            returnData['ErrorMessage']='Programming Error' + strLastError
            returnData['TestResult']='Fail'
            raise Exception('Programming Error', 13, hr, strLastError)
        
        # Remove Power
        self.bridge.PowerOff()
        
        # Print a confirmation message
        #print 'Programming succeeded...'
        
        return
        
    def _ProgrammingSucceeded(self, hr):
        ''' Internal method used to check for success/failure. '''
        return hr >= 0

    def _InitializePortForProgramming(self):
        ''' Internal method to configure the port for programming. '''
        
        # Set the protocol to SWD
        self.bridge._bridge.SetProtocol(enumInterfaces.SWD)
        
        # Set the connector
        self.bridge._bridge.SetProtocolConnector(1)
        
        # Set the Clock Frequency
        self.bridge._bridge.SetProtocolClock(enumFrequencies.FREQ_03_0)
        
        return

    def _PSoC4_IsChipNotProtected(self):
        '''
        Chip Level Protection reliably can be read by below API (available in VIRGIN, OPEN, PROTECTED modes)
        This API uses SROM call - to read current status of CPUSS_PROTECTION register (privileged)
        This register contains current protection mode loaded from SFLASH during boot-up.
        '''
        hResult = self.bridge._bridge.PSoC4_ReadProtection()
        
        hr = hResult[3]
        strError = hResult[2]
        
        flashProt = hResult[0]
        chipProt = hResult[1]

        if (not self._ProgrammingSucceeded(hr)):
            return E_FAIL #consider chip as protected if any communication failure
        
        if ((chipProt[0] & CHIP_PROT_PROTECTED) == CHIP_PROT_PROTECTED):
            #print "Chip is in PROTECTED mode. Any access to Flash is suppressed."        
            return E_FAIL
    
        return S_OK

    def _PSoC4_EraseAll(self):
        '''
        Check chip level protection here. If PROTECTED then move to OPEN by PSoC4_WriteProtection() API.
        Otherwise use PSoC4_EraseAll() - in OPEN/VIRGIN modes.
        '''
        
        hr = self._PSoC4_IsChipNotProtected()
        
        if (self._ProgrammingSucceeded(hr)): #OPEN mode
            #Erase All - Flash and Protection bits. Still be in OPEN mode.
            hResult = self.bridge._bridge.PSoC4_EraseAll()
            
            hr = hResult[1]
            if (not self._ProgrammingSucceeded(hr)): 
                returnData['ErrorMessage']='Programming Error'
                returnData['TestResult']='Fail'               
                #print hResult[0]
       
        else:
            #Move to OPEN from PROTECTED. It automatically erases Flash and its Protection bits.
            flashProt = [] #do not care in PROTECTED mode
            chipProt = []
            for i in range(0, 1):
                chipProt.append(CHIP_PROT_OPEN)
            data1 = array.array('B',flashProt) #do not care in PROTECTED mode
            data2 = array.array('B',chipProt)  #move to OPEN
    
            #hResult = self.bridge._bridge.PSoC4_WriteProtection(buffer(data1), buffer(data2))
            hResult = self.bridge._bridge.PSoC4_WriteProtection(data1, data2)
            #print hResult
            #print kSuccess
            hr = hResult[kSuccess-1]
            strLastError  = hResult[kStrError]        
            if (not self._ProgrammingSucceeded(hr)):
                #print strLastError
                return hr
                
            #Need to reacquire chip here to boot in OPEN mode.
            #ChipLevelProtection is applied only after Reset.
            hResult = self.bridge._bridge.DAP_Acquire()
            hr = hResult[0]
           
        return hr
    
    def _ProgramFlash(self, flashSize):
        ''' Internal helper method for programming flash.'''

        hResult = self._PSoC4_GetTotalFlashRowsCount(flashSize)
        
        hr = hResult[kSuccess]
        totalRows = hResult[1]
        rowSize = hResult[2]
        if (not self._ProgrammingSucceeded(hr)):
            return hr
             
        #Program Flash array
        for i in range(0, totalRows):
            hResult = self.bridge._bridge.PSoC4_ProgramRowFromHex(i)            
            hr = hResult[1]
            strLastError = hResult[0]
            if (not self._ProgrammingSucceeded(hr)):
                #print strLastError
                return hr            
        return hr

    def _PSoC4_GetTotalFlashRowsCount(self, flashSize):      
        ''' Internal helper method for getting flash row count. '''  
        hResult = self.bridge._bridge.PSoC4_GetFlashInfo()
        
        hr = hResult[3]
        rowsPerArray = hResult[0]
        rowSize = hResult[1]
        strLastError = hResult[2]
        if (not self._ProgrammingSucceeded(hr)):
                #print strLastError
                return hr
    
        totalRows = flashSize / rowSize
    
        return (hr,totalRows,rowSize)
        
    def _PSoC4_VerifyFlash(self, flashSize):
        ''' Internal helper method for verifying flash.'''
        hResult = self._PSoC4_GetTotalFlashRowsCount(flashSize)
        hr = hResult[0]
        totalRows = hResult[1]
        rowSize = hResult[2]
        if (not self._ProgrammingSucceeded(hr)):
            return hr
                  
        #Verify Flash array
        for i in range(0, totalRows):        
            hResult = self.bridge._bridge.PSoC4_VerifyRowFromHex(i)            
            hr = hResult[2]
            verResult = hResult[0]
            strLastError = hResult[1]
            
            if (not self._ProgrammingSucceeded(hr)):
                #print strLastError
                return hr
                
            if (verResult == 0):
                #print "Verification failed on %d row." % (i)
                return E_FAIL
                
        return hr
        
def main():
    ''' 
    This function is used to test the Gen4Device class. This function also allows 
    a user to drag & drop a hex file on top of this script and the Gen4 device 
    will automatically be programmed.
    '''
    
    # Program the device with a new hex file
    import sys
    
    # Create a connection to the Gen4 Device
    g4 = Gen4Device()
    
    # Program the device
    g4.ProgramDevice(FWPath)
    
    # Disconnect    
    g4.Disconnect()
    
    #print 'Programming completed...'
    jsonReturn=json.dumps(returnData, True)
    print jsonReturn
    return jsonReturn
    
        
if __name__ == '__main__':
    main()
    
# End of File