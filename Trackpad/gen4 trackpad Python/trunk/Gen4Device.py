#!/usr/bin/env python
import os
from TTHE_I2C import *
import TTHE_I2C
import Gen4FlashMap
import pickle
import CalculateCRC
import numpy
import array

# "Defines"
kTotalGroups = 14   # Total number of flash config groups

fileName = os.path.split(__file__)[-1]


class PanelDataTypes(object):
    RawData     = 0     # Mutual Cap
    Baseline    = 1     # Mutual Cap
    Difference  = 2     # Mutual Cap
    #ScanData    = 3     # Mutual Cap
    
    SelfRawData = 3
    SelfBaseline = 4
    SelfDifference = 5
    
    BalancedRawData = 6
    BalancedBaseline = 7
    BalancedDifference = 8
    
    Buttons = 9
    
    
class SensorDataTypes(object):
    mutual      = 0     # Mutual cap data
    button      = 1     # Button data
    selfCap     = 2     # Self-capacitance data
    balanced    = 3     # Balanced-Cap data
    
    
#-------------------------------------------------------------------------------
#                   Gen4 Device Class (TTHEDevice Subclass)
#-------------------------------------------------------------------------------
class Gen4Device(TTHE_I2C.TTHEDevice):

    def __init__(self):
        
        self.bigEndian = True # Assume this is true
        
        self.systemFrequency = 12000000.    # WARNING! THIS IS HARD-CODED. WE HAVE NO WAY TO READ THIS FROM THE DEVICE FIRMWARE

        # Initialize the superclass
        TTHE_I2C.TTHEDevice.__init__(self, bridgeID=None, \
                                            deviceAddress=None, \
                                            applyPower=True, \
                                            voltage=TTHE_I2C.Voltages.VOLT_50V, \
                                            interruptMode=False)
                        
        # Make sure we enter CMD mode
        print 'Entering Command and Test Mode...'
        self.EnterCmdMode()
        
        # Determine the flash row size
        print 'Reading the flash row size...'
        self.flashRowSize = self.GetFlashRowSize()
        
        # Read all flash data (REPLACES InitializeGroupData METHOD!!!)
        print 'Initializing flash data...'
        self._InitializeFlashData()
        
        # Execute a full panel scan (updates class variables)
        print 'Performing a full panel scan...'
        self.ScanPanel()
        
        # Leave the device in OP mode
        print 'Entering OP Mode...'
        self.EnterOpMode()
        
        return

    def _InitializeFlashData(self):
        ''' 
        This method reads the entire TT_CFG flash space from the Gen4 device
        and passes it to the Gen4FlashMap object to interpret the data.
        '''
        
        # Create a flash map object
        self.flash = Gen4FlashMap.Gen4FlashMap(flashRowSize=self.flashRowSize, device='CY8CTMA440')
        
        # Get the maximum flash row (CAL start - TT_CFG start)
        flashRows = self.flash.GetListOfConfigFlashRows()
        
        # Read all flash rows and store the data
        flashData = []
        for row in flashRows:
            # Read the existing flash row
            data = self._ReadDataForFlashBlock(flashBlockNumber=row, numBytes=self.flashRowSize)

            # Append the data to the flash data list
            flashData.append(data)
        
        # Verify the config block CRC
        [calculated, stored] = self.GetConfigBlockCRC(ebid=0)
        
        #print 'Calculated CRC:', calculated
        #print 'Stored CRC:', stored
        
        # Initialize the flash map
        self.flash.SetFlashData(flashData, calculated, stored)
        
        return
        
    def GetConfigBlockCRC(self, ebid=0):
        
        # Make sure we are in CAT mode
        self.EnterCmdMode()
        
        # Send the "Verify Config Block" command
        self.SendCatCommand(0x11, [ebid])
        
        # Read the response from the command
        data = self.ReadCatCommand()[0:5]
        
        # See if the data read was successful
        success = (data[0] & 0x01) == 0x00
        #if success == False:
        #    raise Exception(fileName, 'GetConfigBlockCRC', 1, 'ERROR verifying config block CRC for ebid %d.' % ebid)
        
        # Get the calculated CRC
        calculated = (data[1] << 8) + (data[2])
        
        # Get the stored CRC
        stored = (data[3] << 8) + (data[4])
        
        return (calculated, stored)
        
    def GetFlashRowSize(self):
        ''' Use the built-in command to read the flash row size. (CAT mode) '''
        #self.EnterCmdMode()
        self.SendCatCommand(0x02, []) # Get config row size
        retVal = self.ReadCatCommand()        
        rowSize = (int(retVal[0]) << 8) + int(retVal[1])
        return rowSize

    def _ReadDataForFlashBlock(self, flashBlockNumber=0, numBytes=0, ebid=0):
        ''' Read the data for a specified flash block number. '''
        
        # Make sure we are in CAT mode
        self.EnterCmdMode()
        
        # Send the read command
        self.SendCatCommand(0x03, [((flashBlockNumber >> 8) & 0xFF), (flashBlockNumber & 0xFF), \
                                   ((numBytes >> 8) & 0xFF), (numBytes & 0xFF), \
                                    ebid])
        
        # Get the return value     
        data = self.ReadCatCommand()
        
        # See if the data read was successful
        success = (data[0] & 0x01) == 0x00
        if success == False:
            raise Exception(fileName, '_ReadDataForFlashBlock', 1, 'ERROR reading flash block %d.' % flashBlockNumber)
        
        # Get the EBID associated with the read
        ebidRead = data[1]
        if ebidRead != ebid:
            raise Exception(fileName, '_ReadDataForFlashBlock', 2, 'ERROR: the EBID read from the config block does not match the specified ebid (%d != %d).' % (ebidRead, ebid))
        
        # Extract the read length
        readLength = (data[2] << 8) + (data[3])
                
        # Get the data
        readData = data[5:readLength+5]
        
        # Get the CRC
        CRC = (data[readLength+5] << 8) + (data[readLength+6])
        
        # TODO: CHECK THE CRC
               
        return readData
          
    def _GetUniqueList(self, nonUniqueList=[]):
        ''' Takes a list object and returns a list with only unique entries. '''
        uniqueList = []
        for element in nonUniqueList:
            if uniqueList.__contains__(element) == False:
                uniqueList.append(element)
        return uniqueList

    def _CountOccurancesInList(self, list, item):
        ''' Counts the number of occurances of a specific item in a list object. '''
        occurances = 0
        for entry in list:
            if entry == item:
                occurances += 1
        return occurances
        

    def SaveConfigurableParameters(self, filePath='./params.g4'):
        '''
        This method takes the current flash configuration for the device and 
        saves it to a specified file path.  This method is useful to capture 
        the state of a device during tests, or saving a device configuration 
        after the device has been reprogrammed.
        '''
        
        # Open the output file
        file = open(filePath, 'wb')
        
        dict = {}
        
        # Create a dictionary to hold SYS data
        dict['sysData'] = self.sysData
        
        # Create a dictionary to hold the contents of flash
        dict['flash'] = self.flash
        
        # Save the dictionary to file
        pickle.dump(dict, file)
        
        # Close the file
        file.close()
        
        return
        

    def LoadConfigurableParameters(self, filePath='./params.g4'):
        '''
        This method takes a set of parameters which have been saved to a file
        and restores a Gen4 device to the saved configuration.
        '''
        
        # Open the pickle file
        file = open(filePath, 'rb')
        
        # Read the dictionary
        dict = pickle.load(file)
                
        # Close the file
        file.close()
        
        # Overwrite the current device's flash config settings
        self.flash = flash.SetFlashData(dict['flash'])
        
        # Commit the changes to the device
        self.CommitParametersToDevice()
        
        # Reinitialize the group data
        self._InitializeGroupData()
        
        return
        
    def CommitParametersToDevice(self):
        '''
        This method commits any changes to the runtime configurable parameters 
        (stored in the self.flash - Gen4FlashMap object - variable).
        '''
               
        # Calculate the CRC on the uncommited data
        self.flash.UpdateCRC()
        
        # Get the updated flash data
        flashData = self.flash.GetUncommitedBlockData()
                
        # Come up with a list of flash rows to write
        flashRowsToWrite = self.flash.GetListOfModifiedFlashRows()
                
        # Write the necessary flash rows
        for row in flashRowsToWrite:
            data = flashData[row]
            errorCode = -1
            attempts = 0
            while errorCode != 0:
                [errorCode, errorMessage] = self._WriteConfigBlock(row, data)
                if errorCode != 0:
                    print 'ERROR (%d): %s' % (errorCode, errorMessage)
                    print '    Trying to write the block again.'
                    attempts += 1
                
                if attempts >= 5:
                    print 'FAILED AFTER 5 ATTEMPTS, QUITTING!'
                    break
        
        # Re-Initialize the flash data (Uncommitted -> Device Data)
        self.flash.SaveUncommittedData()
        
        return

    def _CalculateCRC(self, flashData):

        # Flatten the flash data
        allData = []
        for row in flashData:
            for byte in row:
                allData.append(byte)

        # Get the offset and size for the CRC register
        [offset, size] = self.flash.GetOffsetForRegister('CONFIG_CRC')

        # Make sure we don't include the CRC data
        allData = allData[offset+size:]

        # Calculate the CRC
        crc = CalculateCRC.calculate(allData)

        return crc

    def _WriteConfigBlock(self, rowNumber=0, data=[], securityKey=[0xA5, 0x01, 0x02, 0x03, 0xFF, 0xFE, 0xFD, 0x5A], ebid=0):
        '''
        This method writes a row of flash data wrapped in the config block
        format. 
        '''        
        
        # Start with the flash row number
        rowNumberMSB = (rowNumber >> 8) & 0xff
        rowNumberLSB = (rowNumber) & 0xff
        dataToWrite = [rowNumberMSB, rowNumberLSB]
        
        # Then send the flash row size
        flashSizeMSB = (self.flashRowSize >> 8) & 0xff
        flashSizeLSB = (self.flashRowSize) & 0xff
        dataToWrite.append(flashSizeMSB)
        dataToWrite.append(flashSizeLSB)
        
        # Set the EBID
        dataToWrite.append(ebid)
        
        # Then send the flash block data
        for i in range(0, self.flashRowSize):
            if i < len(data):
                dataToWrite.append(data[i])
            else:
                dataToWrite.append(0)
        
        # Follow everything with the 8-byte security key
        for value in securityKey:
            dataToWrite.append(value)

        # End the whole packet with a CRC
        crc = CalculateCRC.calculate_flash_config(data)
        crcMSB = (crc >> 8) & 0xff
        crcLSB = (crc) & 0xff
        dataToWrite.append(crcMSB)
        dataToWrite.append(crcLSB)

        # Make sure we are in CAT mode
        self.EnterCmdMode()
        
        # Send the write command
        self.SendCatCommand(0x04, dataToWrite)
        
        # Get the return value     
        data = self.ReadCatCommand()[0:5]
        
        # Get the error code
        errorCode = data[0]
        
        if errorCode == 1:
            errorMessage = 'Invalid security code.'
        elif errorCode == 2:
            errorMessage = 'Invalid block number.'
        elif errorCode == 3:
            errorMessage = 'Failed flash write.'
        else:
            errorMessage = ''

        return (errorCode, errorMessage)
        
        
    def ExecutePanelScan(self):
        ''' 
        This method forces the device to execute a full panel scan. 
        Make sure the device is in CAT mode to execute.
        '''
        
        # Make sure we are in CAT mode
        #self.EnterCmdMode()
        
        # Send the CAT command
        self.SendCatCommand(0x0B, [])
        
        # Read the response
        retVal = self.ReadCatCommand()[0]
        
        # Create a success flag
        success = (retVal == 0) 
        
        return success


    def RetrievePanelScan(self, dataType=PanelDataTypes.Difference):
        '''
        This command retrieve the last panel scan. 
        Make sure the device is in CAT mode to execute.
        '''
        
        # Make sure we are in CAT mode
        #self.EnterCmdMode()
        
        # Get the number of rows and columns we expect
        numRows = self.sysData['PCFG']['ElectrodesY']
        numCols = self.sysData['PCFG']['ElectrodesX']
                        
        # Create a 2D array to contain the panel scan
        frame = numpy.zeros((numRows, numCols))
        
        # Create a variables to keep track of the processing
        readOffset = 0
        row = 0
        col = 0
        
        numElements = numRows * numCols # Request all elements (if possible)
        
        # Loop until all of the data has been collected
        while True:            
            # Send the CAT command
            self.SendCatCommand(0x0C, [(readOffset >> 8), readOffset & 0xFF, \
                                       (numElements >> 8), numElements & 0xFF, \
                                       int(dataType)])
            
            # Read the response
            retVal = self.ReadCatCommand()
            
            # Create a success flag
            success = ((retVal[0] & 0x01) == 0) 
            
            if success == False:
                print 'ERROR: Could not retrieve the panel data.'
                return None
            
            # Get the ID of the data returned
            dataID = retVal[1]
            
            # Get the number of elements
            returnedElements = (retVal[2] << 8) + retVal[3]
            
            # Get the format of the data
            dataFormat = retVal[4]
            elementSize = dataFormat & 0x07
            self.elementSize = elementSize
            rowMajor = not ((dataFormat & 0x08) == 0)
            self.rowMajor = rowMajor
            self.columnMajor = not rowMajor
            bigEndian = ((dataFormat & 0x10) == 0)
            self.bigEndian = bigEndian
            self.littleEndian = not bigEndian
            unsigned = ((dataFormat & 0x20) == 0)
            self.unsigned = unsigned
            
            # calculate bytes returned
            bytesReturned = returnedElements * elementSize
            
            # Make sure we have enough data
            if len(retVal) < (bytesReturned + 5):
                print 'ERROR: Too few bytes returned (%d < %d).' % (len(retVal), bytesReturned+5)
                return None
            
            # Get the returned data
            returnedData = retVal[5:5+bytesReturned]
                
            # Process the returned data
            for i in range(0, bytesReturned, elementSize):
                value = 0
                for j in range(0, elementSize):
                    if bigEndian == True:
                        value += returnedData[i+j] << (((elementSize-1)*8) - (8*j))
                    else:
                        value += returnedData[i+j] << (8*j)
                
                # See if we need to convert the value to a signed value
                if unsigned == False:
                    signBit = 1 << ((elementSize * 8) - 1)
                    mask = 0
                    for m in range(0, elementSize*8):
                        mask |= (1 << m)
                    if (value & signBit) > 0:
                        value ^= mask
                        value += 1
                
                # Store the value in the correct location
                frame[row, col] = value
                
                # Increment the row/column
                if rowMajor == True:
                    row += 1
        
                    # Make sure we wrap properly
                    if row >= numRows:
                        row = 0
                        col += 1
                else:
                    col += 1
                    
                    # Make sure we wrap properly
                    if col >= numCols:
                        col = 0
                        row += 1
        
            if returnedElements == 0:
                break
        
            # Increment the read offset
            readOffset += returnedElements
        
        return frame

    def ScanPanel(self, dataType=PanelDataTypes.Difference):
        ''' 
        This method wraps the execute & retrieve panel scan functions into one
        easy-to-use method.
        '''
        
        # Make sure we are in CAT mode
        self.EnterCmdMode()

        # Execute the panel scan
        self.ExecutePanelScan()
        
        # Retrieve the panel scan
        frame = self.RetrievePanelScan(dataType)

        return frame

    def CalibrateIDACs(self):
        ''' This method is used to calibrate the device IDACs '''
        
        # Make sure we are in CAT mode
        self.EnterCmdMode()

        # Send the CAT command
        self.SendCatCommand(0x09, [], timeout=20)
        
        # Read the response
        retVal = self.ReadCatCommand()[0]
        
        # Create a success flag
        success = (retVal == 0) 
        
        return success
        
    def InitializeBaselines(self, bitmask=0x0F):
        ''' This method is used to initialize all baselines. '''
        
        # Make sure we are in CAT mode
        self.EnterCmdMode()

        # Send the CAT command
        self.SendCatCommand(0x0A, [bitmask], timeout=10)
        
        # Read the response
        retVal = self.ReadCatCommand()[0]
        
        # Create a success flag
        success = (retVal == 0) 
        
        return success    
        
    def StartSensorDataMode(self, sensorList=[[0,0,SensorDataTypes.mutual]]):
        ''' 
        This method starts "Sensor Data Mode" which is described in the 
        Gen4 firmware IROS (001-66682). Sensor data mode allows up to 25 sensors
        to provide additional information while in OP mode.
        
        sensorList is a list object with the following format:
        [row, col, <output data type (see SensorDataTypes object>]
        '''

        # Save the sensor list and data type
        self._sensorList = sensorList

        # Get some system parameters
        bigEndian = True 
        #bigEndian = self.bigEndian

        # Make sure we are in CAT mode
        self.EnterCmdMode()

        # Form the OP command
        data = []
        
        for sensor in self._sensorList:
            [row,col,sensorDataType] = sensor
            
            offset = self._GetSensorOffset(row, col)
            val = 0
            val += ((sensorDataType & 0x3) << 14)
            val += offset
            msb = (val >> 8) & 0xff
            lsb = (val & 0xff)
                        
            if bigEndian == True:
                byte0 = msb  
                byte1 = lsb 
            else:
                byte0 = lsb 
                byte1 = msb 
            
            data.append(byte0)
            data.append(byte1)
        
        # Terminate the sensor list
        if bigEndian == True:
            data.append(0x7F)
            data.append(0xFF)
        else:
            data.append(0xFF)
            data.append(0x7F)
        
        # Send the OP command
        self.SendCatCommand(0x0D, data)

        return
        
    def _GetSensorOffset(self, row=0, col=0):
        '''
        If the element size is unknown, a full panel scan should be executed 
        and retrieved first to ensure the "self.elementSize" class variable is 
        correct. The default value for the element size is 1 byte.
        
        The same comment applies to row-major and column-major format.
        
        The ScanPanel method was added to the initialization routine to ensure 
        the class variables are correct.
        '''
        
        # Get some system parameters
        #rowMajor = self.rowMajor
        rowMajor = False
        columnMajor = self.columnMajor
        elementSize = self.elementSize
        
        # Get the number of rows and columns we expect
        numRows = self.sysData['PCFG']['ElectrodesY']
        numCols = self.sysData['PCFG']['ElectrodesX']
        
        # TODO: THIS NEEDS TO BE VARIFIED!!!!
        if rowMajor == True:
            offset = numCols * row + col
        else:
            offset = numRows * col + row
        # END TODO
        
        offset *= elementSize
        
        if offset < 0:
            print 'ERROR in offset calculation. Offset = %d' % offset
            offset = 0
        
        return offset
        
    def StopSensorDataMode(self):
        '''
        This method stops streaming the additional sensor data while in OP mode.
        '''
        
        # Make sure we are in CAT mode
        self.EnterCmdMode()

        # Send the OP command
        self.SendOpCommand(0x0E, [])
        
        return

    def RetrieveSensorData(self):
        '''
        Retrieves data resulting from SENSOR DATA MODE being enabled.
        Sensor Data Mode is the mode where up to 25 sensors can be monitored 
        during normal device operation. The data is reported via OP mode.
        '''
        
        # Make sure we are in OP mode
        #self.EnterOpMode()
        
        # Get the number of sensors we are monitoring
        numSensors = len(self._sensorList)
        
        # Define the number of bytes per sensor
        bytesPerSensor = 2  #self.elementSize
        bigEndian = True   #self.bigEndian
        unsigned = False    #self.unsigned
        
        # Get the start and end location in the register map
        start = I2C_OP_TOUCH_10+I2C_OP_TOUCH_PKT_SIZE-1
        end = start + 150
        
        # Get OP data
        processedData = self.GetOPData(bytes=end)
        
        # Use the unprocessed data to parse out the sensor details
        data = processedData['data'][start:]
        
        # Get the raw data
        raw_unprocessed = data[0:50]
        baseline_unprocessed = data[50:100]
        difference_unprocessed = data[100:150]
                       
        # process the raw, baseline and difference data
        raw = []
        baseline = []
        difference = []
        for i in range(0, numSensors):
            index = i * bytesPerSensor
            
            rawVal = 0
            baselineVal = 0
            differenceVal = 0
            
            for j in range(0, bytesPerSensor):
                if bigEndian == True:
                    shiftAmount = ((bytesPerSensor-1) - j) * 8
                else:
                    shiftAmount = j * 8
                
                rawVal += (raw_unprocessed[index + j] << shiftAmount)
                baselineVal += (baseline_unprocessed[index+j] << shiftAmount)
                differenceVal += (difference_unprocessed[index+j] << shiftAmount)
            
            if unsigned == False:
                # Convert the values to signed values
                rawVal = self._ConvertToSigned(rawVal, bytesPerSensor)
                baselineVal = self._ConvertToSigned(baselineVal, bytesPerSensor)
                differenceVal = self._ConvertToSigned(differenceVal, bytesPerSensor)
            
            raw.append(rawVal)
            baseline.append(baselineVal)
            difference.append(differenceVal)
        
        return (raw, baseline, difference)
        
        
    def _ConvertToSigned(self, value, numBytes=2):
        ''' Helper method for performing conversion to signed integers. '''
        
        # Get the sign bit
        signBit = 1 << (numBytes * 8 - 1)

        # Create an inversion mask
        mask = 0
        for i in range(0, numBytes*8):
            mask |= (1 << i)

        if (value & signBit) > 0:
            # Invert
            value ^= mask
            
            # Add one
            value += 1
            
            # Multiply by -1
            value *= -1

        return value


    def SetFrequency(self, frequency=133000.0):
        '''
        Sets the RAMP_INT_UP and RAMP_INT_DN parameters based on a floating
        point frequency value.
        '''
        # Calculate the ramp up/down time from the desired frequency
        ramp = int(self.systemFrequency / (2.0 * frequency))
        
        # Set the ramp up/down value       
        self.flash.SetBitFieldValue('TSS_SEQ_CONFIG4_MUT', 'RAMP_INT_UP', ramp)
        self.flash.SetBitFieldValue('TSS_SEQ_CONFIG4_MUT', 'RAMP_INT_DN', ramp)
        
        return

    def GetPanelShape(self):
        ''' Gets the panel dimensions. '''
        numRows = self.sysData['PCFG']['ElectrodesY']
        numCols = self.sysData['PCFG']['ElectrodesX']
        return (numRows, numCols)
  


#-------------------------------------------------------------------------------
#                                  MAIN
#-------------------------------------------------------------------------------
if __name__ == '__main__':
    ''' This is self-test code. '''
    
    # Create a Gen4 Device
    g4 = Gen4Device()
    
    # Print the flash row size
    print '\nFlash Row Size: %d\n' % g4.flashRowSize
    
    '''
    # MEASURE DIFFERENCE COUNTS
    
    # Make sure we are in CAT mode
    g4.EnterCmdMode()
    
    for i in range(0, 10):
    
        # Execute a panel scan
        g4.ExecutePanelScan()
        
        # Retrieve the panel scan
        data = g4.RetrievePanelScan(PanelDataTypes.Difference)
        
        print data
        print '----------------------------------------------------------------'
    '''
    
        
    '''
    # Make sure we are in CAT mode to send the command
    g4.EnterCmdMode()
    
    sensorList = []
    #sensorList.append([0,0,SensorDataTypes.mutual])
    sensorList.append([12,20,SensorDataTypes.mutual])
    #sensorList.append([1,1,SensorDataTypes.mutual])
    #sensorList.append([12,20,SensorDataTypes.mutual])
    #sensorList.append([0,20,SensorDataTypes.mutual])
    #sensorList.append([12,0,SensorDataTypes.mutual])
    
    g4.StartSensorDataMode(sensorList)
    
    # Make sure we are in OP mode
    g4.EnterOpMode()
    
    import pylab
    fig = pylab.figure()
    pylab.ylim([-5, 110])
    pylab.title('Difference Counts [12,20]')
    pylab.xlabel('time')
    pylab.ylabel('Difference Counts')
    pylab.grid(True)
    pylab.ion()
    
    x = []
    y = []
    xVal = 0
    for i in range(0, 200):
        [raw, baseline, diff] = g4.RetrieveSensorData()
        y.append(diff[0])
        x.append(xVal)
        xVal += 1
        pylab.plot(x,y,'r-')
        pylab.draw()
    
    pylab.ioff()
    
    # Make sure we are in CAT mode to send the command
    g4.EnterCmdMode()
    
    # Stop sending the extra sensor data
    g4.StopSensorDataMode()
    '''
    
    
    print '\nREADING CURRENT VALUES...'
    
    # Print the TX Voltage
    txVoltage = g4.flash.GetBitFieldValue('TSS_TX_CONFIG_MUT', 'TX_LVL_CTRL')
    print 'TX Voltage:', txVoltage
    
    # Print the number of subconversions
    bf = g4.flash.GetBitFieldValue('TSS_LENGTH_MUT', 'SUBCONVERSIONS')
    print 'Subconversions:', bf
    
    # Print the number of tx clocks
    bf = g4.flash.GetBitFieldValue('TSS_LENGTH_MUT', 'TXPULSES')
    print 'Tx Clocks:', bf
    
    
    

    print '\nMODIFYING VALUES...'

    # Set the Tx Voltage
    g4.flash.SetBitFieldValue('TSS_TX_CONFIG_MUT', 'TX_LVL_CTRL', 'VTX_10_V')
    
    # Set the number of subconversions
    g4.flash.SetBitFieldValue('TSS_LENGTH_MUT', 'SUBCONVERSIONS', 4)
    
    # Set the number of Tx Clocks
    g4.flash.SetBitFieldValue('TSS_LENGTH_MUT', 'TXPULSES', 2)
    
    # Commit the changes to flash
    g4.CommitParametersToDevice()
    
    
    print '\nREADING MODIFIED VALUES...'
    
    # Print the TX Voltage
    txVoltage = g4.flash.GetBitFieldValue('TSS_TX_CONFIG_MUT', 'TX_LVL_CTRL')
    print 'TX Voltage:', txVoltage
    
    # Print the number of subconversions
    bf = g4.flash.GetBitFieldValue('TSS_LENGTH_MUT', 'SUBCONVERSIONS')
    print 'Subconversions:', bf
    
    # Print the number of tx clocks
    bf = g4.flash.GetBitFieldValue('TSS_LENGTH_MUT', 'TXPULSES')
    print 'Tx Clocks:', bf


    
    # Get the CRC value
    crc = g4.flash.GetBitFieldValue('CONFIG_CRC', 'CRC Check Value')
    print 'CRC Value:', crc
    
    # Disconnect
    g4.Disconnect()
    
    print 'Script Complete...'

# End of File