#!/usr/bin/env python
'''
This module reads the *ResourceList.xml and *FlashMap.xml files created for 
TTHE. The purpose of this module is to read the locations of specific 
configurable registers to be able to read and modify their values.
'''

# Imports
from xml.etree.ElementTree import ElementTree
import os
from copy import deepcopy
import CalculateCRC

# Defines
TT_CFG_FLASH_OFFSET =   0xcb00
CAL_FLASH_OFFSET =      0xe200

fileName = os.path.split(__file__)[-1]


class BitValue(object):
    order = 0
    value = 0
    name = ''

class BitField(object):
    name = ''
    affectBaseline = False # This is a "Yes"/"" entry in the bitfield
    min = 0
    max = 0
    default = 0
    mask = 0
    bitValues = []
    type = 'INT'

class Register(object):
    name = ''
    bitFields = []
    datasheetName = ''
    address = 0x00
    relativeAddress = 0x00
    size = 0

class ResourceValue(object):
    name = ''
    displayName = ''
    index = 0
    comment = ''
    registers = []
    
class Resource(object):
    name = ''
    displayName = ''
    index = 0
    comment = ''
    resourceValues = []
    
def flatten(lst):
    ''' Helper function to flatten a list. '''
    flatList = []
    
    try:
        if len(lst) > 1:
            for item in lst:
                flatList += flatten(item)
        else:
            flatList += lst
    except:
        flatList = [lst]
    
    return flatList


def GetUniqueList(nonUniqueList=[]):
    ''' Takes a list object and returns a list with only unique entries. '''
    uniqueList = []
    for element in nonUniqueList:
        if uniqueList.__contains__(element) == False:
            uniqueList.append(element)
    return uniqueList
    

class Gen4FlashMap(object):

    def __init__(self, flashData=[], flashRowSize=128, device='CY8CTMA440'):

        # Save the flash row size
        self.flashRowSize = flashRowSize
     
        # Save the flash data
        self.SetFlashData(flashData)

        # Save the device string
        self._device = device
        
        # Parse the ResourceList XML file
        self._ParseResourceList()
        
        # Parse the FlashMap XML file
        self._ParseFlashMap()
        
        return
        
    def SetFlashData(self, flashData, calculatedCRC=0, storedCRC=0):
        
        # Save the flash data
        self._flashData = deepcopy(flashData)
        
        # Parse the config block data
        self._blockData = self._ParseConfigBlock(flashData, calculatedCRC, storedCRC)
        
        self._DeviceData = deepcopy(self._blockData)        # register values currently stored in the device
        self._UncommittedData = deepcopy(self._blockData)   # modified register values not yet committed to the device
        
        return
        
    def SaveUncommittedData(self):
        self._DeviceData = deepcopy(self._UncommittedData)
        return

    def GetDeviceData(self):
        return self._DeviceData
    
    def GetUncommittedData(self):
        return self._UncommittedData
        
    def GetUncommitedBlockData(self):
        ''' Returns data in a block format. '''
                
        # Create a list to contain all blocks
        blocks = []
        
        for i in range(0, len(self._UncommittedData)):
            # Get the block that this data is associated with
            blockNum = self._GetFlashRowForByteOffset(i)
                        
            # See if the blocks list is long enough
            if len(blocks) <= blockNum:
                blocks.append([])
            
            # Append to the correct list
            blocks[blockNum].append(self._UncommittedData[i])
            
        return blocks
        
        
    def _ParseConfigBlock(self, data=[], calculatedCRC=0, storedCRC=0):
        ''' Parses the config block from a flash row read. '''
        
        blockData = None
        
        if len(data) > 0:
        
            # Flatten the row data into one massive array
            data = flatten(data)
                       
            # Extract the data size
            dataSize = (data[1] << 8) + (data[0])
                       
            if dataSize > 0:
            
                # Get the maximum block size
                maximumSize = (data[3] << 8) + (data[2])
                                          
                # Get the block data
                blockData = data[0:maximumSize+4]
                
                # Calculate the CRC based on the block data
                blockCRC = CalculateCRC.calculate_flash_config(data[0:dataSize])
                                                
                # Get the CRC
                try:
                    CRC = (data[maximumSize+1] << 8) + (data[maximumSize])
                except:
                    pass
                
                '''
                # Make sure the CRC matches the stored CRC
                if (CRC != storedCRC):
                    print 'CRC:', CRC
                    print 'Stored CRC:', storedCRC
                    print 'Block CRC:', blockCRC
                    print 'Calculated CRC:', calculatedCRC
                    raise Exception(fileName, '_ParseConfigBlock', 1, 'ERROR: The stored CRC does not match the returned CRC value (%d != %d).' % (CRC, storedCRC))
                
                # Make sure the calculated CRC matches the internal calculated CRC
                if blockCRC != calculatedCRC:
                    raise Exception(fileName, '_ParseConfigBlock', 2, 'ERROR: The calculated CRC does not match the internally calculated CRC value (%d != %d).' % (blockCRC, calculatedCRC))
                '''
                      
        return blockData
                
    def _ParseResourceList(self):
        ''' Parse the *ResourceList.xml file. '''
        
        # Create a path to the resource file
        path = os.path.split(__file__)
        path = path[0] + '\\' + self._device + 'ResourceList.xml'
    
        # Make sure the file exists
        if not os.path.exists(path):
            raise Exception('Gen4FlashMap', '_ParseResourceList', 0, 'Resource list XML file could not be found (%s)' % path)
        
        # Parse the XML file
        tree = ElementTree()
        tree.parse(path)
        
        # Get the PSOC_DB node
        element = tree.getroot()
    
        # Get all resource lists
        resourceLists = element.findall('RESOURCE_LIST')
    
        # Find the 'TUNNING_PARAM' resource list
        tuningParam = None
        for resourceList in resourceLists:
            attributes = resourceList.attrib
            if attributes['NAME'] == 'TUNNING_PARAM':
                tuningParam = resourceList
                break
    
        # Check tuningParam
        if tuningParam is None:
            raise Exception('Gen4FlashMap', '_ParseResourceList', 1, 'Could not find the TUNNING_PARAM resource list in the XML file.')
    
        # Parse all resources within the resource list
        resources = tuningParam.findall('RESOURCE')
    
        self._resources = []
        for resource in resources:
            # Create a resource object
            r = Resource()
            
            # Parse the attributes from the XML
            attributes = resource.attrib
            r.name = attributes['NAME']
            r.displayName = attributes['DISPLAY_NAME']
            r.index = int(attributes['INDEX'])
            r.comment = attributes['COMMENT']
            
            # Parse the individual resource values
            resourceValueList = resource.findall('RESOURCE_VALUE_LIST')
            resourceValues = resourceValueList[0].findall('RESOURCE_VALUE')
            rvList = []
            for resourceValue in resourceValues:
                # Create a resource value object
                rv = ResourceValue()

                # Parse the attributes from the XML
                attributes = resourceValue.attrib
                rv.name = attributes['NAME']
                rv.displayName = attributes['DISPLAY_NAME']
                rv.index = int(attributes['INDEX'])
                if attributes.has_key('COMMENT'):
                    rv.comment = attributes['COMMENT']
                else:
                    rv.comment = ''

                # Parse the individual registers
                registerList = resourceValue.findall('REGISTER_LIST')
                registers = registerList[0].findall('REGISTER')
                regList = []
                for register in registers:
                    
                    # Create a register object
                    reg = Register()
                    
                    # Parse the attributes from the XML
                    attributes = register.attrib
                    reg.name = attributes['NAME']
                    
                    # Parse individual bitfields
                    bitfieldList = register.findall('BITFIELD_LIST')
                    bitfields = bitfieldList[0].findall('BITFIELD')
                    bfList = []
                    for bitfield in bitfields:
                        # Create a bitfield object
                        bf = BitField()
                    
                        # Parse the attributes from the XML
                        attributes = register.attrib
                        bf.name = attributes['NAME']
                    
                        # Append the bitfield to the bitfield list
                        bfList.append(bf)
                    
                    # Save the bitfieds to the register
                    reg.bitFields = bfList
                    
                    # Save the register object to the list
                    regList.append(reg)

                # Save the register list
                rv.registers = regList

                # Save the resouce value to the list
                rvList.append(rv)
            
            # Save the list of resource values
            r.resourceValues = rvList
            
            # Save the resource object to the list
            self._resources.append(r)
    
        return


    def _ParseFlashMap(self):
        ''' Parse the *FlashMap.xml file. '''
        
        # Create a path to the resource file
        path = os.path.split(__file__)
        path = path[0] + '\\' + self._device + 'FlashMap.xml'
    
        # Make sure the file exists
        if not os.path.exists(path):
            raise Exception('Gen4FlashMap', '_ParseFlashMap', 0, 'Flash Map XML file could not be found (%s)' % path)
        
        # Parse the XML file
        tree = ElementTree()
        tree.parse(path)
        
        # Get the PSOC_DB node
        element = tree.getroot()
    
        # Get all resource lists
        deviceLists = element.findall('PSOC_DEVICE_LIST')
        psocDevices = deviceLists[0].findall('PSOC_DEVICE')
        
        # Find our psoc device within the list
        deviceTree = None
        for psocDevice in psocDevices:
            # Parse the attributes from the XML
            attributes = psocDevice.attrib
            
            if attributes['NAME'] == self._device:
                deviceTree = psocDevice
                break
                
        # Check the device tree
        if deviceTree is None:
            raise Exception('Gen4FlashMap', '_ParseFlashMap', 1, 'PSoC Device (%s) could not be found in the XML file.' % self._device)
        
        # Parse out each flash map section
        flashMapSections = deviceTree.findall('FLASH_MEMORY_MAP')
        
        # Find the 'TT_CFG_FLASH' section
        ttConfigTree = None
        for flashMap in flashMapSections:
            attributes = flashMap.attrib
            if attributes['NAME'] == 'TT_CFG_FLASH':
                ttConfigTree = flashMap
                break
        
        # Check the ttConfigTree
        if ttConfigTree is None:
            raise Exception('Gen4FlashMap', '_ParseFlashMap', 2, 'Could not find TT_CFG_FLASH memory map section within the XML file.')
        
        # Loop through and parse out each register
        self._registers = []
        for memory in ttConfigTree:
            # Create a register object
            reg = Register()
            
            # Parse out the individual attributes
            attributes = memory.attrib
            if attributes['ATTRIBUTES'] == 'RES':
                continue
            reg.name = attributes['NAME']
            reg.datasheetName = attributes['DATASHEET_NAME']
            reg.address = eval(attributes['ADDR'])
            reg.relativeAddress = reg.address - TT_CFG_FLASH_OFFSET
            reg.size = int(attributes['SIZE'])
            
            # Get the bitfield list
            bitfieldList = memory.findall('BITFIELD_LIST')
            if len(bitfieldList) > 0:
                bitfields = bitfieldList[0].findall('BITFIELD')
                bfList = []
                for bitfield in bitfields:
                    # Create a bitfield object
                    bf = BitField()
                    
                    # Populate the bitfield from the XML attributes
                    attributes = bitfield.attrib
                    bf.affectBaseline = (attributes['AFFECT_BASELINE'] == 'Yes')
                    bf.name = attributes['NAME']
                    if attributes.has_key('DEFAULT'):
                        bf.default = int(eval(attributes['DEFAULT']))
                    else:
                        bf.default = None
                    if attributes.has_key('MIN'):
                        bf.min = int(eval(attributes['MIN']))
                    else:
                        bf.min = None
                    if attributes.has_key('MAX'):
                        bf.max = int(eval(attributes['MAX']))
                    else:
                        bf.max = None
                    bf.mask = int(eval(attributes['MASK']))
                    bf.type = attributes['TYPE']
                    
                    # Get the specific bit values
                    bitValueList = bitfield.findall('BITVALUE_LIST')
                    if len(bitValueList) > 0:
                        bitValues = bitValueList[0].findall('BITVALUE')
                        bvList = []
                        for bitValue in bitValues:
                            # Create a bitvalue object
                            bv = BitValue()
                            
                            # Parse the individual attributes
                            attributes = bitValue.attrib
                            bv.order = int(attributes['ORDER'])
                            bv.value = int(attributes['VALUE'])
                            bv.name = attributes['NAME']
                            
                            # Save the bitvalue to the list
                            bvList.append(bv)
                            
                        # Save the bit values
                        bf.bitValues = bvList
                    
                    # Save the bitfield
                    bfList.append(bf)
            
            # Save the bitfield list to the register
            reg.bitFields = bfList
            
            # Add to the register list
            self._registers.append(reg)
        
        return

    def GetOffsetForRegister(self, name):
        # Get the register object
        r = self._GetRegister(name)
        return (r.address, r.size)
        
    def _GetRegister(self, name):
        # Find this particular register
        register = None
        for r in self._registers:
            if r.name.lower() == name.lower():
                register = r
                break
        return register
        
    def GetRegisterValue(self, name):
        ''' Gets a value for a register. Specify the datasheet name. '''
        
        # Get the register object
        register = self._GetRegister(name)
        
        # Get the address from the register object
        addr = register.relativeAddress
        
        # Get the register contents
        value = 0
        for i in range(0, register.size):
            value += self._UncommittedData[addr+i] << (8*i)
                
        return value
    
    def GetListOfConfigFlashRows(self):        
        maxSize = (CAL_FLASH_OFFSET - TT_CFG_FLASH_OFFSET)
        flashRows = range(0, self._GetFlashRowForByteOffset(maxSize))
        return flashRows
    
    def _GetFlashRowForByteOffset(self, byteOffset=0):
        ''' Determine the flash row for a specified byte offset. '''
        flashRow = int(byteOffset) / int(self.flashRowSize)
        return flashRow

    def _GetBitField(self, register, bitfield):
        ''' Method for retrieving internal object files. '''
        
        # Search the register object for the bitfield
        bitfieldObject = None
        for bf in register.bitFields:
            if bf.name.lower() == bitfield.lower():
                bitfieldObject = bf
                break
                
        return bitfieldObject

    def GetBitFieldValue(self, register, bitfield):
        ''' Pass the name of the register, and the name of the bitfield. '''
        
        # Get the register object
        r = self._GetRegister(register)
        
        # Get the bitfield object
        bf = self._GetBitField(r, bitfield)
        
        # Get the regiser value
        regVal = self.GetRegisterValue(register)
        
        # Apply the mask to the register
        mask = bf.mask
        regVal &= mask
        
        # Shift the reg value to the correct place
        shiftTimes = 0
        while (mask & 0x01) == 0:
            regVal >>= 1
            mask >>= 1
            shiftTimes += 1
            if shiftTimes > 32:
                raise Exception('Gen4FlashMap', 'GetBitFieldValue', 'Bitfield mask is invalid (0x%x).' % bf.mask)
        
        # See if the bitfield value has a corresponding bitvalue
        bitVal = None
        for bv in bf.bitValues:
            if bv.value == regVal:
                bitVal = bv.name
                break
        
        return (regVal, bitVal)

    def SetRegisterValue(self, name, value):
        ''' Sets a value for a register. Specify the datasheet name. '''
        
        # Get the register object
        register = self._GetRegister(name)
        
        # Get the address from the register object
        addr = register.relativeAddress
        
        # Write the value into the uncommited data        
        for i in range(0, register.size):
            self._UncommittedData[addr+i] = (value & 0xFF)
            value >>= 8
        
        return

    def SetBitFieldValue(self, register, bitfield, value):
        ''' Pass the name of the register, and the name of the bitfield. '''
        
        # Get the register object
        r = self._GetRegister(register)
        
        # Get the bitfield object
        bf = self._GetBitField(r, bitfield)
        
        # Get the register value
        regVal = self.GetRegisterValue(register)
               
        # Clear the bits in the bitfield
        mask = bf.mask
        regVal &= ~mask
        
        # Find the number of times to shift the value
        shiftTimes = 0
        while (mask & 0x01) == 0:
            mask >>= 1
            shiftTimes += 1
        
        # Check if value is a string
        if (type(value) == type(' ')) and (bf.type.upper() == 'ENUM'):
            # Look to see if there is a bit value to select from
            for bitValue in bf.bitValues:
                if bitValue.name.lower() == value.lower():
                    value = bitValue.value
                    break

        # OR-in the new value
        regVal |= ((value << shiftTimes) & bf.mask)

        # Set the register value
        self.SetRegisterValue(register, regVal)

        return

    def GetListOfModifiedFlashRows(self):
        ''' Get a list of flash rows modified since the device was last read. '''
                
        flashRows = []
        
        for i in range(0, len(self._UncommittedData)):
            if self._UncommittedData[i] != self._DeviceData[i]:
                flashRowIndex = self._GetFlashRowForByteOffset(i)
                flashRows.append(flashRowIndex)
        
        # Make sure the list is unique
        flashRows = GetUniqueList(flashRows)
                
        return flashRows
        
        
    def SetCRC(self, crcValue):
        
        # Make sure the CRC value is the right size
        crcValue &= 0xFFFF
        
        # Set the CRC and CRC Compliment variables        
        self.SetBitFieldValue('CONFIG_CRC', 'CRC Check Value', crcValue)
        
        return
        
    def _calculateCompliment(self, crcValue):
        return ((crcValue ^ 0xFFFF) & 0xFFFF)
    
    
    def UpdateCRC(self):
        ''' Calculates the CRC on the uncommitted data, then updates it. '''
        
        # Get the address of the CRC
        crcReg = self._GetRegister('CONFIG_CRC')
        crcAddr = crcReg.relativeAddress
        
        # Get the data to test
        data = self._UncommittedData[0:crcAddr]
        
        # Calculate the CRC
        crc = CalculateCRC.calculate_flash_config(data) 
        
        # Set the CRC
        self.SetCRC(crc)
        
        return
        

#-------------------------------------------------------------------------------
#                                  MAIN
#-------------------------------------------------------------------------------
if __name__ == '__main__':
    
    # Create Test Data
    testData = []
    for i in range(0, 32):
        flashBlock = []
        for j in range(0, 128):
            flashBlock.append(0)
        testData.append(flashBlock)
    
    # Create a reference to the flash map object
    flashMap = Gen4FlashMap(testData)

    # Get a register value
    reg = flashMap.GetRegisterValue('TSS_INT_CONFIG1_MUT')
    print reg

    # Bitfield Value
    bf = flashMap.GetBitFieldValue('TSS_TX_CONFIG_MUT', 'TX_LVL_CTRL')
    print bf
    bf = flashMap.GetBitFieldValue('TSS_LENGTH_MUT', 'SUBCONVERSIONS')
    print bf

    
    #for v1 in range(0, 2**32-1, 10000):
    #    flashMap.SetRegisterValue('TSS_INT_CONFIG1_MUT', v1)
    #    v2 = flashMap.GetRegisterValue('TSS_INT_CONFIG1_MUT')
    #    if v2 != v1:
    #        print 'ERROR:', v1, v2

    
    # Set a bitfield value   
    flashMap.SetBitFieldValue('TSS_TX_CONFIG_MUT', 'TX_LVL_CTRL', 8)
    print flashMap.GetBitFieldValue('TSS_TX_CONFIG_MUT', 'TX_LVL_CTRL')
    
    flashMap.SetBitFieldValue('TSS_TX_CONFIG_MUT', 'TX_LVL_CTRL', 'VTX_10_V')
    print flashMap.GetBitFieldValue('TSS_TX_CONFIG_MUT', 'TX_LVL_CTRL')
    
    # Check the CRC method
    crc = 0xABCD
    flashMap.SetCRC(crc)
    print 'CRC:', flashMap.GetBitFieldValue('CONFIG_CRC', 'CRC Check Value')
    print 'CRC Compliment:', flashMap.GetBitFieldValue('CONFIG_CRC', 'CRC Check Value Complement')
    
    
    print flashMap.GetListOfModifiedFlashRows()
    

    print 'script completed...'

# End of File