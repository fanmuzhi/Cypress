#!/usr/bin/env python
'''
This file contains classes which define the format and structure of data 
contained in the Gen4 flash groups.

Make sure to use all "setters" and "getters" to write specific parameter
values. Directly writing to the objects memory without calling the "Set" method 
will result in the data not being retained.
'''

RevisionControlNumber = 0

def SetRevisionControlNumber(revCtrl=0):
    global RevisionControlNumber
    RevisionControlNumber = revCtrl
    return

#-------------------------------------------------------------------------------
#                                 Group 0
#-------------------------------------------------------------------------------
class Group0Data(object):

    # Settings
    groupNumber = 0
    
    if RevisionControlNumber == 0:
        startOffset = 0
        endOffset = 3
    elif RevisionControlNumber <= 272188:
        startOffset = 0
        endOffset = 3
    else:
        startOffset = 0
        endOffset = 3
    

    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-0 Data '''
        
        # Get the total CRC data
        self.CONFIG_CRC = 0
        for i in range(0, 4):
            self.CONFIG_CRC += (int(data[i]) << (8*i))
        
        # Get the individual CRC components
        self.crcCheckValue = data[0] + (int(data[1]) << 8)
        self.crcCheckValueCompliment = data[2] + (int(data[3]) << 8)
        
        # Perform a check to make sure the CRC is correct
        self._checkCompliment()
        
        return
        
    def _checkCompliment(self):
        ''' Returns True if the compliment is valid. '''
        
        complimentCheck = (self.crcCheckValue ^ self.crcCheckValueCompliment)
        if complimentCheck != 0xFFFF:
            print 'ERROR: CRC is invalid. %04x' % complimentCheck
            validCompliment = False
        else:
            validCompliment = True
            
        return validCompliment
        
        
    def SetCRC(self, crcValue):
        
        # Make sure the CRC value is the right size
        crcValue &= 0xFFFF
        
        # Calculate the compliment
        compliment = self._calculateCompliment(crcValue)
        
        # Set the CRC and CRC Compliment variables
        self.crcCheckValue = crcValue
        self.crcCheckValueCompliment = compliment
        
        # Store the values in the data variable
        compliment_LSB = compliment & 0xFF
        compliment_MSB = (compliment >> 8) & 0xFF
        crc_LSB = crcValue & 0xFF
        crc_MSB = (crcValue >> 8) & 0xFF
        self.CONFIG_CRC = (compliment_MSB << 24) | \
                          (compliment_LSB << 16) | \
                          (crc_MSB << 8) | \
                          (crc_LSB)
        
        self.data[0] = crc_LSB
        self.data[1] = crc_MSB
        self.data[2] = compliment_LSB
        self.data[3] = compliment_MSB
        
        return
        
    def _calculateCompliment(self, crcValue):
        return ((crcValue ^ 0xFFFF) & 0xFFFF)
        
#-------------------------------------------------------------------------------
#                                 Group 1
#-------------------------------------------------------------------------------
class Group1Data(object):

    # Settings
    groupNumber = 1
    
    if RevisionControlNumber == 0:
        startOffset = 4
        endOffset = 127
    elif RevisionControlNumber <= 272188:
        startOffset = 4
        endOffset = 76
    else:
        startOffset = 4
        endOffset = 127
    
    # Values
    FlashRows = None

    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-1 Data '''
        
        # Initialize the offset value
        self.offset = 0
        
        # Parse the SDK_CTRL_CFG_SIZE            
        self.SDK_CTRL_CFG_SIZE = self._parseNextValue(numBytes=4)
            
        # Parse the X_LEN_PHY value
        self.X_LEN_PHY = self._parseNextValue(numBytes=2)
            
        # Parse the Y_LEN_PHY value
        self.Y_LEN_PHY = self._parseNextValue(numBytes=2)
        
        # Parse the HST_MODE0 value
        self.HST_MODE0 = self._parseNextValue(numBytes=1)
        
        # Parse ACT_DIST0
        self.ACT_DIST0 = self._parseNextValue(numBytes=1)
        
        # Parse SCAN_TYP0
        self.SCAN_TYP0 = self._parseNextValue(numBytes=1)
         
        # Parse ACT_INTRVL0
        self.ACT_INTRVL0 = self._parseNextValue(numBytes=1)
         
        # Parse ACT_LFT_INTRVL0
        self.ACT_LFT_INTRVL0 = self._parseNextValue(numBytes=1)
         
        # Parse Reserved
        Reserved = self._parseNextValue(numBytes=1)
        
        # Parse TCH_TMOUT0
        self.TCH_TMOUT0 = self._parseNextValue(numBytes=2)
        
        # Parse LP_INTRVL0
        self.LP_INTRVL0 = self._parseNextValue(numBytes=2)
        
        # Parse PWR_CFG
        self.PWR_CFG = self._parseNextValue(numBytes=1)
        
        # Parse INT_CFG
        self.INT_CFG = self._parseNextValue(numBytes=1)
        
        # Parse INT_PULSE_DATA
        self.INT_PULSE_DATA = self._parseNextValue(numBytes=1)
        
        # Parse OPMODE_CFG
        self.OPMODE_CFG = self._parseNextValue(numBytes=1)
        
        # Parse HANDSHAKE_TIMEOUT
        self.HANDSHAKE_TIMEOUT = self._parseNextValue(numBytes=2)
        
        # Parse TIMER_CAL_INTERVAL
        self.TIMER_CAL_INTERVAL = self._parseNextValue(numBytes=1)
        
        # Parse Reserved
        Reserved = self._parseNextValue(numBytes=1)
        
        # Parse RP2P_MIN
        self.RP2P_MIN = self._parseNextValue(numBytes=2)
        
        # Parse RFB_EXT
        self.RFB_EXT = self._parseNextValue(numBytes=2)
        
        # Parse IDACOPEN_LOW
        self.IDACOPEN_LOW = self._parseNextValue(numBytes=1)
        
        # Parse IDACOPEN_HIGH
        self.IDACOPEN_HIGH = self._parseNextValue(numBytes=1)
        
        # Parse GIDAC_OPEN
        self.GIDAC_OPEN = self._parseNextValue(numBytes=1)
        
        # Parse GAIN_OPEN
        self.GAIN_OPEN = self._parseNextValue(numBytes=1)
        
        # Parse POST_CFG
        self.POST_CFG = self._parseNextValue(numBytes=1)
        
        # Parse GESTURE_CFG
        self.GESTURE_CFG = self._parseNextValue(numBytes=1)
        
        # Create an array of GEST_EN because I am lazy...
        self.GEST_EN = []
        for i in range(0, 32):
            gest = self._parseNextValue(numBytes=1)
            self.GEST_EN.append(gest)
        
        # Don't bother parsing the 52 reserved bytes...
        
        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val
        

#-------------------------------------------------------------------------------
#                                 Group 2
#-------------------------------------------------------------------------------
class Group2Data(object):

    # Settings
    groupNumber = 2
    
    if RevisionControlNumber == 0:
        startOffset = 128
        endOffset = 255
    elif RevisionControlNumber <= 272188:
        startOffset = 128
        endOffset = 220
    else:
        startOffset = 128
        endOffset = 255

    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-2 Data '''
        
        # Initialize the offset value
        self.offset = 0
        
        # Parse the SDK_CTRL_CFG_SIZE            
        self.TRUETOUCH_CFG_SIZE = self._parseNextValue(numBytes=4)
            
        # Parse the MAX_SELF_SCAN_INTERVAL
        self.MAX_SELF_SCAN_INTERVAL = self._parseNextValue(numBytes=4)
        
        # Parse the MAX_MUTUAL_SCAN_INTERVAL
        self.MAX_MUTUAL_SCAN_INTERVAL = self._parseNextValue(numBytes=4)
        
        # Parse the MAX_BALANCED_SCAN_INTERVAL
        self.MAX_BALANCED_SCAN_INTERVAL = self._parseNextValue(numBytes=4)
        
        # Parse the SELF_Z_THRSH
        self.SELF_Z_THRSH = self._parseNextValue(numBytes=4)
        
        # Parse the SELF_Z_MODE
        self.SELF_Z_MODE = self._parseNextValue(numBytes=4)
        
        # Parse the SMART_SCAN_ENABLE
        self.SMART_SCAN_ENABLE = self._parseNextValue(numBytes=4)
        
        # Parse the T_COMP_ENABLE
        self.T_COMP_ENABLE = self._parseNextValue(numBytes=4)
        
        # Parse the T_COMP_SENSORS_ACTIVE
        self.T_COMP_SENSORS_ACTIVE = self._parseNextValue(numBytes=4)
        
        # Parse the T_COMP_INTERVAL
        self.T_COMP_INTERVAL = self._parseNextValue(numBytes=4)
        
        # Parse the T_COMP_SENSORS_RECAL
        self.T_COMP_SENSORS_RECAL = self._parseNextValue(numBytes=4)
        
        # Parse the T_COMP_HIGH
        self.T_COMP_HIGH = self._parseNextValue(numBytes=4)
        
        # Parse the T_COMP_LOW
        self.T_COMP_LOW = self._parseNextValue(numBytes=4)
        
        # Parse the AFH_ENABLE
        self.AFH_ENABLE_OFFSET = self.offset
        self.AFH_ENABLE = self._parseNextValue(numBytes=4)
        
        # Parse the AFH_TX_PERIOD_CENTER_MUTUAL
        self.AFH_TX_PERIOD_CENTER_MUTUAL = self._parseNextValue(numBytes=4)
        
        # Parse the AFH_ALT_TX_PERIOD1_MUTUAL 
        self.AFH_ALT_TX_PERIOD1_MUTUAL = self._parseNextValue(numBytes=4)
        
        # Parse the AFH_ALT_TX_PERIOD2_MUTUAL
        self.AFH_ALT_TX_PERIOD2_MUTUAL = self._parseNextValue(numBytes=4)
        
        # Parse the AFH_NUM_SUB_CONV_BASE_MUTUAL
        self.AFH_NUM_SUB_CONV_BASE_MUTUAL = self._parseNextValue(numBytes=4)
        
        # Parse the AFH_ALT_NUM_SUB_CONV_MUTUAL
        self.AFH_ALT_NUM_SUB_CONV_MUTUAL = self._parseNextValue(numBytes=4)
        
        # Parse the AFH_LISTENING_SCAN_COUNT
        self.AFH_LISTENING_SCAN_COUNT = self._parseNextValue(numBytes=4)
        
        # Parse the AFH_LISTEN_SCAN_CYCLE_REPEATS
        self.AFH_LISTEN_SCAN_CYCLE_REPEATS = self._parseNextValue(numBytes=4)
        
        # Parse the AFH_BLOCK_NOISE_THRESHOLD
        self.AFH_BLOCK_NOISE_THRESHOLD = self._parseNextValue(numBytes=4)
        
        # Parse the AFH_DEFAULT_REVERT_TIME
        self.AFH_DEFAULT_REVERT_TIME = self._parseNextValue(numBytes=4)
        
        # Don't bother parsing the 36 reserved bytes

        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val
        
    def GetAFHEnable(self):
        ''' Get the Adaptive Frequency Hopping enabled/disabled status '''
        if (self.AFH_ENABLE & 0x00000001) == 1:
            return True
        else:
            return False
    
    def SetAFHEnable(self, enabled=True):
        ''' Set the Adaptive Frequency Hopping status. Does not write to flash.'''
        # Set the object's internal variable
        if enabled == True:
            self.AFH_ENABLE |= 0x00000001
        else:
            self.AFH_ENABLE &= ~0x00000001
        
        # Set the "data" value which will eventually be written back to the device
        self.data[self.AFH_ENABLE_OFFSET+0] = (self.AFH_ENABLE & 0x000000FF)
        self.data[self.AFH_ENABLE_OFFSET+1] = (self.AFH_ENABLE & 0x0000FF00) >> 8
        self.data[self.AFH_ENABLE_OFFSET+2] = (self.AFH_ENABLE & 0x00FF0000) >> 16
        self.data[self.AFH_ENABLE_OFFSET+3] = (self.AFH_ENABLE & 0xFF000000) >> 24
        
        return


#-------------------------------------------------------------------------------
#                                 Group 3
#-------------------------------------------------------------------------------
class Group3Data(object):

    # Settings
    groupNumber = 3
    
    if RevisionControlNumber == 0:
        startOffset = 256
        endOffset = 383
    elif RevisionControlNumber <= 272188:
        startOffset = 256
        endOffset = 297
    else:
        startOffset = 256
        endOffset = 383
        

    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-3 Data '''
        
        # Initialize the offset value
        self.offset = 0
        
        # Parse the GEST_CFG_SIZE            
        self.GEST_CFG_SIZE = self._parseNextValue(numBytes=1)
            
        # Parse the PAN_ACT_DSTX
        self.PAN_ACT_DSTX = self._parseNextValue(numBytes=1)
        
        # Parse the PAN_ACT_DSTY
        self.PAN_ACT_DSTY = self._parseNextValue(numBytes=1)
        
        # Parse the ZOOM_ACT_DSTX
        self.ZOOM_ACT_DSTX = self._parseNextValue(numBytes=1)
        
        # Parse the ZOOM_ACT_DSTY
        self.ZOOM_ACT_DSTY = self._parseNextValue(numBytes=1)
        
        # Parse the FLICK_ACT_DISTX
        self.FLICK_ACT_DISTX = self._parseNextValue(numBytes=1)
        
        # Parse the FLICK_ACT_DISTY
        self.FLICK_ACT_DISTY = self._parseNextValue(numBytes=1)
        
        # Parse the FLICK_TIME
        self.FLICK_TIME = self._parseNextValue(numBytes=1)
        
        # Parse the ST_DEBOUNCE
        self.ST_DEBOUNCE = self._parseNextValue(numBytes=1)
        
        # Parse the MT_DEBOUNCE_PAN
        self.MT_DEBOUNCE_PAN = self._parseNextValue(numBytes=1)
        
        # Parse the MT_DEBOUNCE_ZOOM
        self.MT_DEBOUNCE_ZOOM = self._parseNextValue(numBytes=1)
        
        # Parse the MT_DEBOUNCE_P2Z
        self.MT_DEBOUNCE_P2Z = self._parseNextValue(numBytes=1)
        
        # Parse the ROT_DEBOUNCE
        self.ROT_DEBOUNCE = self._parseNextValue(numBytes=1)
        
        # Parse the COMPL_DEBOUNCE
        self.COMPL_DEBOUNCE = self._parseNextValue(numBytes=1)
        
        # Parse the MT_TIMEOUT
        self.MT_TIMEOUT = self._parseNextValue(numBytes=2)
        
        # Parse the ST_DBLCLK_RMAX
        self.ST_DBLCLK_RMAX = self._parseNextValue(numBytes=1)
        
        # Parse the ST_CLICK_DISTX
        self.ST_CLICK_DISTX = self._parseNextValue(numBytes=1)
        
        # Parse the ST_CLICK_DISTY
        self.ST_CLICK_DISTY = self._parseNextValue(numBytes=1)
        
        # Parse the reserved byte
        Reserved = self._parseNextValue(numBytes=1)
        
        # Parse the MT_CLICK_TMAX
        self.MT_CLICK_TMAX = self._parseNextValue(numBytes=2)
        
        # Parse the MT_CLICK_TMIN
        self.MT_CLICK_TMIN = self._parseNextValue(numBytes=2)
        
        # Parse the ST_CLICK_TMAX
        self.ST_CLICK_TMAX = self._parseNextValue(numBytes=2)
        
        # Parse the ST_CLICK_TMIN
        self.ST_CLICK_TMIN = self._parseNextValue(numBytes=2)
        
        # Parse the ST_DBLCLK_TMAX
        self.ST_DBLCLK_TMAX = self._parseNextValue(numBytes=2)
        
        # Parse the ST_DBLCLK_TMIN
        self.ST_DBLCLK_TMIN = self._parseNextValue(numBytes=2)
        
        # Parse the GESTURE_GROUP_MASK
        self.GESTURE_GROUP_MASK = self._parseNextValue(numBytes=1)
        
        # Parse the GESTURE_GROUP1_START
        self.GESTURE_GROUP1_START = self._parseNextValue(numBytes=1)
        
        # Parse the GESTURE_GROUP1_END
        self.GESTURE_GROUP1_END = self._parseNextValue(numBytes=1)
        
        # Parse the GESTURE_GROUP2_START
        self.GESTURE_GROUP2_START = self._parseNextValue(numBytes=1)
        
        # Parse the GESTURE_GROUP2_END
        self.GESTURE_GROUP2_END = self._parseNextValue(numBytes=1)
        
        # Parse the GESTURE_GROUP3_START
        self.GESTURE_GROUP3_START = self._parseNextValue(numBytes=1)
        
        # Parse the GESTURE_GROUP3_END
        self.GESTURE_GROUP3_END = self._parseNextValue(numBytes=1)
        
        # Parse the GESTURE_GROUP4_START
        self.GESTURE_GROUP4_START = self._parseNextValue(numBytes=1)
        
        # Parse the GESTURE_GROUP4_END
        self.GESTURE_GROUP4_END = self._parseNextValue(numBytes=1)
        
        # Don't bother parsing the 87 reserved bytes

        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val        
        

#-------------------------------------------------------------------------------
#                                 Group 4
#-------------------------------------------------------------------------------
class Group4Data(object):

    # Settings
    groupNumber = 4
    
    if RevisionControlNumber == 0:
        startOffset = 384
        endOffset = 396
    elif RevisionControlNumber <= 272188:
        startOffset = 384
        endOffset = 396
    else:
        startOffset = 384
        endOffset = 396
        

    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-4 Data '''
        
        # Initialize the offset value
        self.offset = 0
        
        # Parse the XY_FILT_CFG_SIZE
        self.XY_FILT_CFG_SIZE = self._parseNextValue(numBytes=4)
        
        # Parse the XY_FILTER_MASK
        self.XY_FILTER_MASK = self._parseNextValue(numBytes=4)
        
        # Parse the XY_FILT_IIR_COEFF
        self.XY_FILT_IIR_COEFF = self._parseNextValue(numBytes=4)

        # Don't bother parsing the 20 reserved bytes

        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val


#-------------------------------------------------------------------------------
#                                 Group 5
#-------------------------------------------------------------------------------
class Group5Data(object):

    # Settings
    groupNumber = 5

    if RevisionControlNumber == 0:
        startOffset = 416
        endOffset = 447
    elif RevisionControlNumber <= 272188:
        startOffset = 416
        endOffset = 428
    else:
        startOffset = 416
        endOffset = 447
        

    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-5 Data '''
        
        # Initialize the offset value
        self.offset = 0
        
        # Parse the FINGER_ID_CFG_SIZE
        self.FINGER_ID_CFG_SIZE = self._parseNextValue(numBytes=4)
        
        # Parse the FINGER_ID_ACT_DIST2
        self.FINGER_ID_ACT_DIST2 = self._parseNextValue(numBytes=4)
        
        # Parse the FINGER_ID_MAX_FINGER_VELOCITY2
        self.FINGER_ID_MAX_FINGER_VELOCITY2 = self._parseNextValue(numBytes=4)

        # Don't bother parsing the 20 reserved bytes

        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val


#-------------------------------------------------------------------------------
#                                 Group 6
#-------------------------------------------------------------------------------
class Group6Data(object):

    # Settings
    groupNumber = 6
    
    if RevisionControlNumber == 0:
        startOffset = 448
        endOffset = 511
    elif RevisionControlNumber <= 272188:
        startOffset = 448
        endOffset = 472
    else:
        startOffset = 448
        endOffset = 511

    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-6 Data '''
        
        # Initialize the offset value
        self.offset = 0
        
        # Parse the CENTROID_SH_CFG_SIZE
        self.CENTROID_SH_CFG_SIZE = self._parseNextValue(numBytes=4)

        # Parse the STYLUS_THRSH
        self.STYLUS_THRSH = self._parseNextValue(numBytes=4)

        # Parse the STYLUS_HYST
        self.STYLUS_HYST = self._parseNextValue(numBytes=4)

        # Parse the S2F_THRESHOLD
        self.S2F_THRESHOLD = self._parseNextValue(numBytes=4)

        # Parse the HOVER_THRSH
        self.HOVER_THRSH = self._parseNextValue(numBytes=4)

        # Parse the HOVER_HYST
        self.HOVER_HYST = self._parseNextValue(numBytes=4)

        # Don't bother parsing the 40 reserved bytes

        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val


#-------------------------------------------------------------------------------
#                                 Group 7
#-------------------------------------------------------------------------------
class Group7Data(object):

    # Settings
    groupNumber = 7
    
    if RevisionControlNumber == 0:
        startOffset = 512
        endOffset = 575
    elif RevisionControlNumber <= 272188:
        startOffset = 512
        endOffset = 544
    else:
        startOffset = 512
        endOffset = 575

    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-7 Data '''
        
        # Initialize the offset value
        self.offset = 0
                
        # Parse the CENTROID_CFG_SIZE
        self.CENTROID_CFG_SIZE = self._parseNextValue(numBytes=4)
        
        # Parse the X_RESOLUTION
        self.X_RESOLUTION = self._parseNextValue(numBytes=4)
        
        # Parse the Y_RESOLUTION
        self.Y_RESOLUTION = self._parseNextValue(numBytes=4)
        
        # Parse the X_MULT
        self.X_MULT = self._parseNextValue(numBytes=4)
        
        # Parse the Y_MULT
        self.Y_MULT = self._parseNextValue(numBytes=4)
        
        # Parse the INNER_EDGE_GAIN
        self.INNER_EDGE_GAIN = self._parseNextValue(numBytes=4)
        
        # Parse the OUTER_EDGE_GAIN
        self.OUTER_EDGE_GAIN = self._parseNextValue(numBytes=4)
        
        # Parse the SENSOR_ASSIGNMENT
        self.SENSOR_ASSIGNMENT = self._parseNextValue(numBytes=4)

        # Don't bother parsing the 32 reserved bytes

        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val


#-------------------------------------------------------------------------------
#                                 Group 8
#-------------------------------------------------------------------------------
class Group8Data(object):

    # Settings
    groupNumber = 8
    
    if RevisionControlNumber == 0:
        startOffset = 576
        endOffset = 607
    elif RevisionControlNumber <= 272188:
        startOffset = 576
        endOffset = 587
    else:
        startOffset = 576
        endOffset = 607

    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-8 Data '''
        
        # Initialize the offset value
        self.offset = 0
        
        # Parse the ID_COORDS_SIZE
        self.ID_COORDS_SIZE = self._parseNextValue(numBytes=4)
        
        # Parse the FAT_FINGER
        self.FAT_FINGER = self._parseNextValue(numBytes=1)
        
        # Parse the LRG_OBJ_CFG
        self.LRG_OBJ_CFG = self._parseNextValue(numBytes=1)
        
        # Parse the MAX_FINGERS
        self.MAX_FINGERS = self._parseNextValue(numBytes=1)
        
        # Parse the MAX_FAT_FINGER_SIZE
        self.MAX_FAT_FINGER_SIZE = self._parseNextValue(numBytes=1)
        
        # Parse the MIN_FAT_FINGER_SIZE
        self.MIN_FAT_FINGER_SIZE = self._parseNextValue(numBytes=1)
        
        # Parse the FINGER_THRESH_MUTUAL
        self.FINGER_THRESH_MUTUAL_OFFSET = self.offset
        self.FINGER_THRESH_MUTUAL = self._parseNextValue(numBytes=1)
        
        # Parse the FINGER_THRESH_SELF
        self.FINGER_THRESH_SELF = self._parseNextValue(numBytes=1)
        
        # Don't bother parsing the 21 reserved bytes

        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val

    def GetFingerThreshold(self):
        ft = self.FINGER_THRESH_MUTUAL & 0x000000FF
        return ft
        
    def SetFingerThreshold(self, threshold):
        threshold &= 0xFF
        self.FINGER_THRESH_MUTUAL &= ~0x000000FF
        self.FINGER_THRESH_MUTUAL |= threshold
        
        self.data[self.FINGER_THRESH_MUTUAL_OFFSET+0] = (self.FINGER_THRESH_MUTUAL & 0x000000FF)
        self.data[self.FINGER_THRESH_MUTUAL_OFFSET+1] = (self.FINGER_THRESH_MUTUAL & 0x0000FF00) >> 8
        self.data[self.FINGER_THRESH_MUTUAL_OFFSET+2] = (self.FINGER_THRESH_MUTUAL & 0x00FF0000) >> 16
        self.data[self.FINGER_THRESH_MUTUAL_OFFSET+3] = (self.FINGER_THRESH_MUTUAL & 0xFF000000) >> 24
        return

#-------------------------------------------------------------------------------
#                                 Group 9
#-------------------------------------------------------------------------------
class Group9Data(object):

    # Settings
    groupNumber = 9
    
    if RevisionControlNumber == 0:
        startOffset = 640
        endOffset = 767
    elif RevisionControlNumber <= 272188:
        startOffset = 640
        endOffset = 656
    else:
        startOffset = 640
        endOffset = 767

    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-9 Data '''
        
        # Initialize the offset value
        self.offset = 0
                
        # Parse the RAW_PROC_CFG_SIZE
        self.RAW_PROC_CFG_SIZE = self._parseNextValue(numBytes=4)
        
        # Parse the RAW_FILTER_MASK
        self.RAW_FILTER_MASK = self._parseNextValue(numBytes=1)
        
        # Parse the RAW_FILT_IIR_COEFF
        self.RAW_FILT_IIR_COEFF = self._parseNextValue(numBytes=1)
        
        # Parse the BL_DELAY_MUT
        self.BL_DELAY_MUT = self._parseNextValue(numBytes=1)
        
        # Parse the BL_DELAY_SELF
        self.BL_DELAY_SELF = self._parseNextValue(numBytes=1)
        
        # Parse the BL_DELAY_BAL
        self.BL_DELAY_BAL = self._parseNextValue(numBytes=1)
        
        # Parse the BL_DELAY_BTN
        self.BL_DELAY_BTN = self._parseNextValue(numBytes=1)
        
        # Parse the BL_THR_MUT
        self.BL_THR_MUT = self._parseNextValue(numBytes=1)
        
        # Parse the BL_THR_SELF
        self.BL_THR_SELF = self._parseNextValue(numBytes=1)
        
        # Parse the BL_THR_BAL
        self.BL_THR_BAL = self._parseNextValue(numBytes=1)
        
        # Parse the BL_THR_BTN
        self.BL_THR_BTN = self._parseNextValue(numBytes=1)
        
        # Parse the BL_H20_RJCT
        self.BL_H20_RJCT = self._parseNextValue(numBytes=1)
        
        # Parse the RAW_IIR_THRESHOLD
        self.RAW_IIR_THRESHOLD = self._parseNextValue(numBytes=1)

        # Don't bother parsing the 112 reserved bytes

        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val


#-------------------------------------------------------------------------------
#                                 Group 10
#-------------------------------------------------------------------------------
class Group10Data(object):

    # Settings
    groupNumber = 10
    
    if RevisionControlNumber == 0:
        startOffset = 768
        endOffset = 1279
    elif RevisionControlNumber <= 272188:
        startOffset = 768
        endOffset = 1192
    else:
        startOffset = 768
        endOffset = 1279

    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-10 Data '''
        
        # Initialize the offset value
        self.offset = 0        
        
        # Parse the CDC_CFG_SIZE
        self.CDC_CFG_SIZE = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_CONTROL_MUT
        self.TSS_CONTROL_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_LENGTH_MUT
        self.TSS_LENGTH_MUT_OFFSET = self.offset
        self.TSS_LENGTH_MUT = self._parseNextValue(numBytes=4)
        self.GetTxPulses()
        self.GetSubconversions()
        
        # Parse the TSS_TX_CONFIG_MUT
        self.TSS_TX_CONFIG_MUT_OFFSET = self.offset
        self.TSS_TX_CONFIG_MUT = self._parseNextValue(numBytes=4)
        self.GetTxDriveControl()
        self.GetTxLevelControl()
        
        # Parse the TSS_TX_CONTROL_MUT
        self.TSS_TX_CONTROL_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG1_MUT
        self.TSS_SEQ_CONFIG1_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG2_MUT
        self.TSS_SEQ_CONFIG2_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG3_MUT
        self.TSS_SEQ_CONFIG3_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG4_MUT
        self.TSS_SEQ_CONFIG4_MUT_OFFSET = self.offset
        self.TSS_SEQ_CONFIG4_MUT = self._parseNextValue(numBytes=4)
        self.GetRampIntUp()
        self.GetRampIntDown()       
        
        # Parse the TSS_SEQ_CONFIG5_MUT
        self.TSS_SEQ_CONFIG5_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG6_MUT
        self.TSS_SEQ_CONFIG6_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG7_MUT
        self.TSS_SEQ_CONFIG7_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG8_MUT
        self.TSS_SEQ_CONFIG8_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_EXT_CONFIG1_MUT
        self.TSS_EXT_CONFIG1_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_EXT_CONFIG2_MUT
        self.TSS_EXT_CONFIG2_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_EXT_INTERVAL_MUT
        self.TSS_EXT_INTERVAL_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_INT_CONFIG1_MUT
        self.TSS_INT_CONFIG1_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_INT_CONFIG2_MUT
        self.TSS_INT_CONFIG2_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_INT_INTERVAL_MUT
        self.TSS_INT_INTERVAL_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_MCS_CONFIG_MUT
        self.TSS_MCS_CONFIG_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_RX_CONFIG_MUT
        self.TSS_RX_CONFIG_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the Reserved
        Reserved = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_CONTROL_SELF
        self.TSS_CONTROL_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_LENGTH_SELF
        self.TSS_LENGTH_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_TX_CONFIG_SELF
        self.TSS_TX_CONFIG_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_TX_CONTROL_SELF
        self.TSS_TX_CONTROL_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG1_SELF
        self.TSS_SEQ_CONFIG1_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG2_SELF
        self.TSS_SEQ_CONFIG2_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG3_SELF
        self.TSS_SEQ_CONFIG3_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG4_SELF
        self.TSS_SEQ_CONFIG4_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG5_SELF
        self.TSS_SEQ_CONFIG5_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG6_SELF
        self.TSS_SEQ_CONFIG6_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG7_SELF
        self.TSS_SEQ_CONFIG7_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG8_SELF
        self.TSS_SEQ_CONFIG8_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_EXT_CONFIG1_SELF
        self.TSS_EXT_CONFIG1_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_EXT_CONFIG2_SELF
        self.TSS_EXT_CONFIG2_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_EXT_INTERVAL_SELF
        self.TSS_EXT_INTERVAL_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_INT_CONFIG1_SELF
        self.TSS_INT_CONFIG1_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_INT_CONFIG2_SELF
        self.TSS_INT_CONFIG2_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_INT_INTERVAL_SELF
        self.TSS_INT_INTERVAL_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_MCS_CONFIG_SELF
        self.TSS_MCS_CONFIG_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_RX_CONFIG_SELF
        self.TSS_RX_CONFIG_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the Reserved
        Reserved = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_CONTROL_BAL
        self.TSS_CONTROL_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_LENGTH_BAL
        self.TSS_LENGTH_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_TX_CONFIG_BAL
        self.TSS_TX_CONFIG_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_TX_CONTROL_BAL
        self.TSS_TX_CONTROL_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG1_BAL
        self.TSS_SEQ_CONFIG1_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG2_BAL
        self.TSS_SEQ_CONFIG2_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG3_BAL
        self.TSS_SEQ_CONFIG3_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG4_BAL
        self.TSS_SEQ_CONFIG4_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG5_BAL
        self.TSS_SEQ_CONFIG5_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG6_BAL
        self.TSS_SEQ_CONFIG6_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG7_BAL
        self.TSS_SEQ_CONFIG7_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG8_BAL
        self.TSS_SEQ_CONFIG8_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_EXT_CONFIG1_BAL
        self.TSS_EXT_CONFIG1_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_EXT_CONFIG2_BAL
        self.TSS_EXT_CONFIG2_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_EXT_INTERVAL_BAL
        self.TSS_EXT_INTERVAL_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_INT_CONFIG1_BAL
        self.TSS_INT_CONFIG1_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_INT_CONFIG2_BAL
        self.TSS_INT_CONFIG2_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_INT_INTERVAL_BAL
        self.TSS_INT_INTERVAL_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_MCS_CONFIG_BAL
        self.TSS_MCS_CONFIG_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_RX_CONFIG_BAL
        self.TSS_RX_CONFIG_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the Reserved
        Reserved = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_CONTROL_BTN
        self.TSS_CONTROL_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_LENGTH_BTN
        self.TSS_LENGTH_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_TX_CONFIG_BTN
        self.TSS_TX_CONFIG_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_TX_CONTROL_BTN
        self.TSS_TX_CONTROL_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG1_BTN
        self.TSS_SEQ_CONFIG1_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG2_BTN
        self.TSS_SEQ_CONFIG2_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG3_BTN
        self.TSS_SEQ_CONFIG3_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG4_BTN
        self.TSS_SEQ_CONFIG4_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG5_BTN
        self.TSS_SEQ_CONFIG5_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG6_BTN
        self.TSS_SEQ_CONFIG6_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG7_BTN
        self.TSS_SEQ_CONFIG7_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_SEQ_CONFIG8_BTN
        self.TSS_SEQ_CONFIG8_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_EXT_CONFIG1_BTN
        self.TSS_EXT_CONFIG1_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_EXT_CONFIG2_BTN
        self.TSS_EXT_CONFIG2_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_EXT_INTERVAL_BTN
        self.TSS_EXT_INTERVAL_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_INT_CONFIG1_BTN
        self.TSS_INT_CONFIG1_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_INT_CONFIG2_BTN
        self.TSS_INT_CONFIG2_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_INT_INTERVAL_BTN
        self.TSS_INT_INTERVAL_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_MCS_CONFIG_BTN
        self.TSS_MCS_CONFIG_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_RX_CONFIG_BTN
        self.TSS_RX_CONFIG_BTN = self._parseNextValue(numBytes=4)
        
        # Parse the Reserved
        Reserved = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_RX_VREF
        self.TSS_RX_VREF = self._parseNextValue(numBytes=4)
        
        # Parse the TSS_RX_LX_CONFIG
        self.TSS_RX_LX_CONFIG = self._parseNextValue(numBytes=4)
        
        # Parse the TX_NUM
        self.TX_NUM_OFFSET = self.offset
        self.TX_NUM = self._parseNextValue(numBytes=4)
        
        # Parse the RX_NUM
        self.RX_NUM_OFFSET = self.offset
        self.RX_NUM = self._parseNextValue(numBytes=4)
        
        # Parse the SENS_NUM
        self.SENS_NUM_OFFSET = self.offset
        self.SENS_NUM = self._parseNextValue(numBytes=4)
        
        # Parse the CROSS_NUM
        self.CROSS_NUM = self._parseNextValue(numBytes=4)
        
        # Parse the BUTTON_NUM
        self.BUTTON_NUM = self._parseNextValue(numBytes=4)
        
        # Parse the SLOTS_MUT
        self.SLOTS_MUT_OFFSET = self.offset
        self.SLOTS_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the SLOTS_SELF_RX
        self.SLOTS_SELF_RX_OFFSET = self.offset
        self.SLOTS_SELF_RX = self._parseNextValue(numBytes=4)
        
        # Parse the SLOTS_SELF_TX
        self.SLOTS_SELF_TX_OFFSET = self.offset
        self.SLOTS_SELF_TX = self._parseNextValue(numBytes=4)
        
        # Parse the SLOTS_SELF
        self.SLOTS_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the SLOTS_BAL
        self.SLOTS_BAL_OFFSET = self.offset
        self.SLOTS_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the SCALE_MUT
        self.SCALE_MUT = self._parseNextValue(numBytes=4)
        
        # Parse the SCALE_SELF
        self.SCALE_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the SCALE_BAL
        self.SCALE_BAL = self._parseNextValue(numBytes=4)
        
        # Parse the SCALE_BUTTON
        self.SCALE_BUTTON = self._parseNextValue(numBytes=4)
        
        # Parse the LX_MODE
        self.LX_MODE = self._parseNextValue(numBytes=4)
        
        # Parse the LX_SCALE
        self.LX_SCALE = self._parseNextValue(numBytes=4)
        
        # Parse the MEDIAN_EN
        self.MEDIAN_EN = self._parseNextValue(numBytes=4)
        
        # Parse the SCANNING_MODE_MUTUAL
        self.SCANNING_MODE_MUTUAL_OFFSET = self.offset
        self.SCANNING_MODE_MUTUAL = self._parseNextValue(numBytes=4)
        
        # Parse the SCANNING_MODE_BUTTON
        self.SCANNING_MODE_BUTTON = self._parseNextValue(numBytes=4)       
        
        # Don't bother parsing the 88 reserved bytes

        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val


    def GetNumSensors(self):
        return (self.SENS_NUM & 0x000000FF)

    def SetNumSensors(self, numSensors=1):
        numSensors &= 0xFF
        self.SENS_NUM &= ~0x000000FF
        self.SENS_NUM |= numSensors
        
        # Update the data array with the correct value
        self.data[self.SENS_NUM_OFFSET+0] = (self.SENS_NUM & 0x000000FF)
        self.data[self.SENS_NUM_OFFSET+1] = (self.SENS_NUM & 0x0000FF00) >> 8
        self.data[self.SENS_NUM_OFFSET+2] = (self.SENS_NUM & 0x00FF0000) >> 16
        self.data[self.SENS_NUM_OFFSET+3] = (self.SENS_NUM & 0xFF000000) >> 24
        return

    def GetNumSelfCapRxSlots(self):
        return (self.SLOTS_SELF_RX & 0x000000FF)

    def SetNumSelfCapRxSlots(self, numSlots):
        ''' This method sets the SLOTS_SELF_RX value. '''
        numSlots &= 0xFF
        self.SLOTS_SELF_RX &= ~0x000000FF
        self.SLOTS_SELF_RX |= numSlots
        
        # Update the data array with the correct value
        self.data[self.SLOTS_SELF_RX_OFFSET+0] = (self.SLOTS_SELF_RX & 0x000000FF)
        self.data[self.SLOTS_SELF_RX_OFFSET+1] = (self.SLOTS_SELF_RX & 0x0000FF00) >> 8
        self.data[self.SLOTS_SELF_RX_OFFSET+2] = (self.SLOTS_SELF_RX & 0x00FF0000) >> 16
        self.data[self.SLOTS_SELF_RX_OFFSET+3] = (self.SLOTS_SELF_RX & 0xFF000000) >> 24
        
        return
        
    def GetNumSelfCapTxSlots(self):
        return (self.SLOTS_SELF_TX & 0x000000FF)

    def SetNumSelfCapTxSlots(self, numSlots):
        ''' This method sets the SLOTS_SELF_TX value. '''
        numSlots &= 0xFF
        self.SLOTS_SELF_TX &= ~0x000000FF
        self.SLOTS_SELF_TX |= numSlots
        
        # Update the data array with the correct value
        self.data[self.SLOTS_SELF_TX_OFFSET+0] = (self.SLOTS_SELF_TX & 0x000000FF)
        self.data[self.SLOTS_SELF_TX_OFFSET+1] = (self.SLOTS_SELF_TX & 0x0000FF00) >> 8
        self.data[self.SLOTS_SELF_TX_OFFSET+2] = (self.SLOTS_SELF_TX & 0x00FF0000) >> 16
        self.data[self.SLOTS_SELF_TX_OFFSET+3] = (self.SLOTS_SELF_TX & 0xFF000000) >> 24
        
        return

    def GetTxPulses(self):
        ''' This method gets the number of Tx Pulses. '''
        self.TXPULSES = self.TSS_LENGTH_MUT & 0x000000FF
        return self.TXPULSES
        
    def SetTxPulses(self, numPulses=2):
        ''' 
        This method sets the number of Tx Pulses.
        
        Number of TX pulses to issue for each single subconversion.  
        TXPULSES must be >=1 when CI_MODE=0 and >=2 when CI_MODE=1.
        
        This method DOES NOT commit changes to the Gen4 device's flash.
        '''
        
        '''
        if numPulses > 255:
            numPulses = 255
        elif numPulses < 1:
            numPulses = 1
        '''
            
        self.TXPULSES = numPulses
        self.TSS_LENGTH_MUT &= ~0x000000FF
        self.TSS_LENGTH_MUT |= numPulses
        
        # Update the data array with the correct value
        self.data[self.TSS_LENGTH_MUT_OFFSET+0] = (self.TSS_LENGTH_MUT & 0x000000FF)
        self.data[self.TSS_LENGTH_MUT_OFFSET+1] = (self.TSS_LENGTH_MUT & 0x0000FF00) >> 8
        self.data[self.TSS_LENGTH_MUT_OFFSET+2] = (self.TSS_LENGTH_MUT & 0x00FF0000) >> 16
        self.data[self.TSS_LENGTH_MUT_OFFSET+3] = (self.TSS_LENGTH_MUT & 0xFF000000) >> 24
        
        return

    def GetSubconversions(self):
        ''' This method gets the number of subconversions. '''
        self.SUBCONVERSIONS = (self.TSS_LENGTH_MUT & 0x0000FF00) >> 8
        return self.SUBCONVERSIONS
        
    def SetSubconversions(self, numSubconversions=2):
        '''
        This method sets the number of subconversions.
        This method does not commit changes to the Gen4 device's flash.
        '''
        if numSubconversions > 255:
            numSubconversions = 255
        elif numSubconversions < 1:
            numSubconversions = 1
        
        self.SUBCONVERSIONS = numSubconversions
        self.TSS_LENGTH_MUT &= ~0x0000FF00
        self.TSS_LENGTH_MUT |= (numSubconversions << 8)
        
        # Update the data array with the correct value
        self.data[self.TSS_LENGTH_MUT_OFFSET+0] = (self.TSS_LENGTH_MUT & 0x000000FF)
        self.data[self.TSS_LENGTH_MUT_OFFSET+1] = (self.TSS_LENGTH_MUT & 0x0000FF00) >> 8
        self.data[self.TSS_LENGTH_MUT_OFFSET+2] = (self.TSS_LENGTH_MUT & 0x00FF0000) >> 16
        self.data[self.TSS_LENGTH_MUT_OFFSET+3] = (self.TSS_LENGTH_MUT & 0xFF000000) >> 24
        
        return

    def GetTxDriveControl(self):
        ''' This method gets the Tx Drive Control value '''
        self.TX_DRV_CTRL = self.TSS_TX_CONFIG_MUT & 0x0000000F
        iout = 0
        if (self.TX_DRV_CTRL == 0):
            iout = 96
        elif (self.TX_DRV_CTRL == 1):
            iout = 384
        elif (self.TX_DRV_CTRL == 2):
            iout = 576
        elif (self.TX_DRV_CTRL == 3):
            iout = 768
        elif (self.TX_DRV_CTRL == 4):
            iout = 960
        elif (self.TX_DRV_CTRL == 5):
            iout = 1152
        elif (self.TX_DRV_CTRL == 6):
            iout = 1344
        elif (self.TX_DRV_CTRL == 7):
            iout = 1536
        elif (self.TX_DRV_CTRL == 8):
            iout = 1728
        elif (self.TX_DRV_CTRL == 9):
            iout = 1920
        elif (self.TX_DRV_CTRL == 10):
            iout = 2112
        elif (self.TX_DRV_CTRL == 11):
            iout = 2304
        elif (self.TX_DRV_CTRL == 12):
            iout = 2496
        elif (self.TX_DRV_CTRL == 13):
            iout = 2688
        elif (self.TX_DRV_CTRL == 14):
            iout = 2880
        elif (self.TX_DRV_CTRL == 15):
            iout = 3072
            
        return (self.TX_DRV_CTRL, iout, 'uA')

    def SetTxDriveControl(self, value):
        '''
        This method writes the Tx Drive Control value into the data object.
        This method DOES NOT commit the change to the device flash.
        '''
        self.TX_DRV_CTRL = value
        self.TSS_TX_CONFIG_MUT &= ~0x0000000F
        self.TSS_TX_CONFIG_MUT |= value
        
        # Update the data array with the correct value
        self.data[self.TSS_TX_CONFIG_MUT_OFFSET+0] = (self.TSS_TX_CONFIG_MUT & 0x000000FF)
        self.data[self.TSS_TX_CONFIG_MUT_OFFSET+1] = (self.TSS_TX_CONFIG_MUT & 0x0000FF00) >> 8
        self.data[self.TSS_TX_CONFIG_MUT_OFFSET+2] = (self.TSS_TX_CONFIG_MUT & 0x00FF0000) >> 16
        self.data[self.TSS_TX_CONFIG_MUT_OFFSET+3] = (self.TSS_TX_CONFIG_MUT & 0xFF000000) >> 24

    def GetTxLevelControl(self):
        ''' This method returns the Tx Level Control (Tx Drive Voltage) '''
        self.TX_LVL_CTRL = (self.TSS_TX_CONFIG_MUT & 0x000000F0) >> 4

        vtx = 0
        if self.TX_LVL_CTRL == 0:
            vtx = 3.
        elif self.TX_LVL_CTRL == 1:
            vtx = 3.5
        elif self.TX_LVL_CTRL == 2:
            vtx = 4.
        elif self.TX_LVL_CTRL == 3:
            vtx = 4.5
        elif self.TX_LVL_CTRL == 4:
            vtx = 5.
        elif self.TX_LVL_CTRL == 5:
            vtx = 5.5
        elif self.TX_LVL_CTRL == 6:
            vtx = 6.
        elif self.TX_LVL_CTRL == 7:
            vtx = 6.5
        elif self.TX_LVL_CTRL == 8:
            vtx = 7.
        elif self.TX_LVL_CTRL == 9:
            vtx = 7.5
        elif self.TX_LVL_CTRL == 10:
            vtx = 8.
        elif self.TX_LVL_CTRL == 11:
            vtx = 8.5
        elif self.TX_LVL_CTRL == 12:
            vtx = 9.
        elif self.TX_LVL_CTRL == 13:
            vtx = 9.5
        elif self.TX_LVL_CTRL == 14:
            vtx = 9.75
        elif self.TX_LVL_CTRL == 15:
            vtx = 10.
        
        return (self.TX_LVL_CTRL, vtx, 'V')
    
    def SetTxLevelControl(self, value=15, voltage=None):
        '''
        This method sets the Tx Level Control (Tx Drive Voltage) in the object
        memory. This methods DOES NOT commit the change to the Gen4 device's 
        flash.
        '''
        
        if voltage is not None:
            if voltage > 10.0:
                voltage = 10.0
            elif voltage < 3.0:
                voltage = 3.0
            
            value = 0
            
            if voltage == 3.0:
                value = 0
            elif voltage == 3.5:
                value = 1
            elif voltage == 4.0:
                value = 2
            elif voltage == 4.5:
                value = 3
            elif voltage == 5.0:
                value = 4
            elif voltage == 5.5:
                value = 5
            elif voltage == 6.0:
                value = 6
            elif voltage == 6.5:
                value = 7
            elif voltage == 7.0:
                value = 8
            elif voltage == 7.5:
                value = 9
            elif voltage == 8.0:
                value = 10
            elif voltage == 8.5:
                value = 11
            elif voltage == 9.0:
                value = 12
            elif voltage == 9.5:
                value = 13
            elif voltage == 9.75:
                value = 14
            elif voltage == 10.0:
                value = 15
            
        self.TX_LVL_CTRL = value
        self.TSS_TX_CONFIG_MUT &= ~0x000000F0
        self.TSS_TX_CONFIG_MUT |= (value << 4)
        
        # Update the data array with the correct value
        self.data[self.TSS_TX_CONFIG_MUT_OFFSET+0] = (self.TSS_TX_CONFIG_MUT & 0x000000FF)
        self.data[self.TSS_TX_CONFIG_MUT_OFFSET+1] = (self.TSS_TX_CONFIG_MUT & 0x0000FF00) >> 8
        self.data[self.TSS_TX_CONFIG_MUT_OFFSET+2] = (self.TSS_TX_CONFIG_MUT & 0x00FF0000) >> 16
        self.data[self.TSS_TX_CONFIG_MUT_OFFSET+3] = (self.TSS_TX_CONFIG_MUT & 0xFF000000) >> 24
        
        return

    def GetPumpEnable(self):
        pumpEnable = (self.TSS_TX_CONFIG_MUT & 0x40000000) == 0x40000000
        return pumpEnable
        
    def SetPumpEnable(self, enabled=True):
        if enabled == True:
            self.TSS_TX_CONFIG_MUT |= 0x40000000
        else:
            self.TSS_TX_CONFIG_MUT &= ~0x40000000 
        
        # Update the data array with the correct value
        self.data[self.TSS_TX_CONFIG_MUT_OFFSET+0] = (self.TSS_TX_CONFIG_MUT & 0x000000FF)
        self.data[self.TSS_TX_CONFIG_MUT_OFFSET+1] = (self.TSS_TX_CONFIG_MUT & 0x0000FF00) >> 8
        self.data[self.TSS_TX_CONFIG_MUT_OFFSET+2] = (self.TSS_TX_CONFIG_MUT & 0x00FF0000) >> 16
        self.data[self.TSS_TX_CONFIG_MUT_OFFSET+3] = (self.TSS_TX_CONFIG_MUT & 0xFF000000) >> 24
        
        return

    def GetRampIntUp(self):
        ''' This method gets the RAMP_INT_UP parameter. '''
        self.RAMP_INT_UP = self.TSS_SEQ_CONFIG4_MUT & 0x000000FF
        return self.RAMP_INT_UP
        
    def GetRampIntDown(self):
        ''' This method gets the RAMP_INT_DN parameter. '''
        self.RAMP_INT_DN = (self.TSS_SEQ_CONFIG4_MUT & 0x00FF0000) >> 16
        return self.RAMP_INT_DN
        
    def SetRampIntUp(self, value=45):
        
        self.RAMP_INT_UP = value
        self.TSS_SEQ_CONFIG4_MUT &= ~0x000000FF
        self.TSS_SEQ_CONFIG4_MUT |= value
        
        # Update the data array with the correct value
        self.data[self.TSS_SEQ_CONFIG4_MUT_OFFSET+0] = (self.TSS_SEQ_CONFIG4_MUT & 0x000000FF)
        self.data[self.TSS_SEQ_CONFIG4_MUT_OFFSET+1] = (self.TSS_SEQ_CONFIG4_MUT & 0x0000FF00) >> 8
        self.data[self.TSS_SEQ_CONFIG4_MUT_OFFSET+2] = (self.TSS_SEQ_CONFIG4_MUT & 0x00FF0000) >> 16
        self.data[self.TSS_SEQ_CONFIG4_MUT_OFFSET+3] = (self.TSS_SEQ_CONFIG4_MUT & 0xFF000000) >> 24
        
        return
        
    def SetRampIntDn(self, value=45):
        
        self.RAMP_INT_DN = value
        self.TSS_SEQ_CONFIG4_MUT &= ~0x00FF0000
        self.TSS_SEQ_CONFIG4_MUT |= (value << 16)
        
        # Update the data array with the correct value
        self.data[self.TSS_SEQ_CONFIG4_MUT_OFFSET+0] = (self.TSS_SEQ_CONFIG4_MUT & 0x000000FF)
        self.data[self.TSS_SEQ_CONFIG4_MUT_OFFSET+1] = (self.TSS_SEQ_CONFIG4_MUT & 0x0000FF00) >> 8
        self.data[self.TSS_SEQ_CONFIG4_MUT_OFFSET+2] = (self.TSS_SEQ_CONFIG4_MUT & 0x00FF0000) >> 16
        self.data[self.TSS_SEQ_CONFIG4_MUT_OFFSET+3] = (self.TSS_SEQ_CONFIG4_MUT & 0xFF000000) >> 24
        
        return
        
    def SetRampInt(self, value=45):
        '''
        This method sets the RAMP_INT_UP and RAMP_INT_DN values. 
        This method DOES NOT commit changes to the Gen4 device's flash.
        '''
        if value < 2:
            value = 2
        elif value > 255:
            value = 255
            
        self.RAMP_INT_UP = value
        self.RAMP_INT_DN = value
        self.TSS_SEQ_CONFIG4_MUT &= ~0x00FF00FF
        self.TSS_SEQ_CONFIG4_MUT |= ((value << 16) | (value))
        
        # Update the data array with the correct value
        self.data[self.TSS_SEQ_CONFIG4_MUT_OFFSET+0] = (self.TSS_SEQ_CONFIG4_MUT & 0x000000FF)
        self.data[self.TSS_SEQ_CONFIG4_MUT_OFFSET+1] = (self.TSS_SEQ_CONFIG4_MUT & 0x0000FF00) >> 8
        self.data[self.TSS_SEQ_CONFIG4_MUT_OFFSET+2] = (self.TSS_SEQ_CONFIG4_MUT & 0x00FF0000) >> 16
        self.data[self.TSS_SEQ_CONFIG4_MUT_OFFSET+3] = (self.TSS_SEQ_CONFIG4_MUT & 0xFF000000) >> 24
        
        return
        
    def GetTxPhasing(self):
        ''' Return value: 1 = single tx, 2 = 4-channel multi-tx '''
        txPhasing = self.SCANNING_MODE_MUTUAL & 0x03
        return txPhasing
        
    def SetTxPhasing(self, phaseValue):
        # Make sure we have the correct number of bits in the Phase Value
        phaseValue &= 0x03
        
        # Update the internal class variable
        self.SCANNING_MODE_MUTUAL &= ~0x03
        self.SCANNING_MODE_MUTUAL |= phaseValue
        
        # Update the data array with the correct value
        self.data[self.SCANNING_MODE_MUTUAL_OFFSET+0] = (self.SCANNING_MODE_MUTUAL & 0x000000FF)
        self.data[self.SCANNING_MODE_MUTUAL_OFFSET+1] = (self.SCANNING_MODE_MUTUAL & 0x0000FF00) >> 8
        self.data[self.SCANNING_MODE_MUTUAL_OFFSET+2] = (self.SCANNING_MODE_MUTUAL & 0x00FF0000) >> 16
        self.data[self.SCANNING_MODE_MUTUAL_OFFSET+3] = (self.SCANNING_MODE_MUTUAL & 0xFF000000) >> 24
        
        return
        
    def SetMultiTx(self, enabled=True):
        '''
        This is a wrapper method aroudn the SetTxPhasing method. Since we 
        currently only have 2 states, single tx and 4-channel multi-tx, this 
        method just enables the 4-channel multi-tx if enabled is True or enables 
        single tx if enabled = False.
        '''
        if enabled == True:
            self.SetTxPhasing(phaseValue=2)
        else:
            self.SetTxPhasing(phaseValue=1)
        return
        
    def GetNumBalancedSlots(self):
        return (self.SLOTS_BAL & 0x000000FF)
        
    def SetNumBalancedSlots(self, numSlots):
        ''' Sets the SLOTS_BAL register. '''
        numSlots &= 0xFF
        self.SLOTS_BAL &= ~0x000000FF
        self.SLOTS_BAL |= numSlots
        
        # Update the data array with the correct value
        self._UpdateDataArray(self.SLOTS_BAL_OFFSET, self.SLOTS_BAL)
    
        return
        
    def GetNumMutualSlots(self):
        return (self.SLOTS_MUT & 0x000000FF)
        
    def SetNumMutualSlots(self, numSlots):
        ''' Sets the SLOTS_MUT register. '''
        numSlots &= 0xFF
        self.SLOTS_MUT &= ~0x000000FF
        self.SLOTS_MUT |= numSlots
        
        # Update the data array with the correct value
        self._UpdateDataArray(self.SLOTS_MUT_OFFSET, self.SLOTS_MUT)
        
        return
        
    def GetNumRxElectodes(self):
        return (self.RX_NUM & 0x0000007F)
    
    def SetNumRxElectrodes(self, numElectrodes):
        numElectrodes &= 0x7F
        self.RX_NUM &= ~0x0000007F
        self.RX_NUM |= numElectrodes
        self._UpdateDataArray(self.RX_NUM_OFFSET, self.RX_NUM)
        return
    
    def GetNumTxElectodes(self):
        return (self.TX_NUM & 0x0000007F)
    
    def SetNumTxElectrodes(self, numElectrodes):
        numElectrodes &= 0x7F
        self.TX_NUM &= ~0x0000007F
        self.TX_NUM |= numElectrodes
        self._UpdateDataArray(self.TX_NUM_OFFSET, self.TX_NUM)
        return
    
    
    def _UpdateDataArray(self, offset, value):
        # Update the data array with the correct value
        self.data[offset+0] = (value & 0x000000FF)
        self.data[offset+1] = (value & 0x0000FF00) >> 8
        self.data[offset+2] = (value & 0x00FF0000) >> 16
        self.data[offset+3] = (value & 0xFF000000) >> 24
        
    
#-------------------------------------------------------------------------------
#                                 Group 11
#-------------------------------------------------------------------------------
class Group11Data(object):

    # Settings
    groupNumber = 11
    
    if RevisionControlNumber == 0:
        startOffset = 1280
        endOffset = 1375
    elif RevisionControlNumber <= 272188:
        startOffset = 1280
        endOffset = 1332
    else:
        startOffset = 1280
        endOffset = 1375
        
    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-11 Data '''
        
        # Initialize the offset value
        self.offset = 0        
        
        # Parse the CALIBRATION_PARAM_SIZE
        self.CALIBRATION_PARAM_SIZE = self._parseNextValue(numBytes=4)
        
        # Parse the Reserved data        
        Reserved = self._parseNextValue(numBytes=4)
        
        # Parse the Reserved data
        Reserved = self._parseNextValue(numBytes=4)
        
        # Parse the Reserved data
        Reserved = self._parseNextValue(numBytes=4)
        
        # Parse the Reserved data
        Reserved = self._parseNextValue(numBytes=4)
        
        # Parse the GLOBAL_IDAC_LSB_MUTUAL
        self.GLOBAL_IDAC_LSB_MUTUAL = self._parseNextValue(numBytes=4)
        
        # Parse the GLOBAL_IDAC_LSB_SELF
        self.GLOBAL_IDAC_LSB_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the GLOBAL_IDAC_LSB_BALANCED
        self.GLOBAL_IDAC_LSB_BALANCED = self._parseNextValue(numBytes=4)
        
        # Parse the GLOBAL_IDAC_LSB_BUTTON
        self.GLOBAL_IDAC_LSB_BUTTON = self._parseNextValue(numBytes=4)
        
        # Parse the TARGET_LEVEL_MUTUAL
        self.TARGET_LEVEL_MUTUAL = self._parseNextValue(numBytes=4)
        
        # Parse the TARGET_LEVEL_SELF
        self.TARGET_LEVEL_SELF = self._parseNextValue(numBytes=4)
        
        # Parse the TARGET_LEVEL_BALANCED
        self.TARGET_LEVEL_BALANCED = self._parseNextValue(numBytes=4)
        
        # Parse the TARGET_LEVEL_BUTTON
        self.TARGET_LEVEL_BUTTON = self._parseNextValue(numBytes=4)
        
        # Don't bother parsing the remaining 44 reserved bytes
        
        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val
        


#-------------------------------------------------------------------------------
#                                 Group 12
#-------------------------------------------------------------------------------
class Group12Data(object):

    # Settings
    groupNumber = 12
    
    if RevisionControlNumber == 0:
        startOffset = 1376
        endOffset = 1407
    elif RevisionControlNumber <= 272188:
        startOffset = 1376
        endOffset = 1384
    else:
        startOffset = 1376
        endOffset = 1407

    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-12 Data '''
        
        # Initialize the offset value
        self.offset = 0        
        
        # Parse the SPREADER_CFG_SIZE
        self.SPREADER_CFG_SIZE = self._parseNextValue(numBytes=4)
        
        # Parse the CLK_IMO_SPREAD
        self.CLK_IMO_SPREAD = self._parseNextValue(numBytes=4)
        
        # Don't bother parsing the remaining 24 reserved bytes
        
        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val
        
        
#-------------------------------------------------------------------------------
#                                 Group 13
#-------------------------------------------------------------------------------
class Group13Data(object):

    # Settings
    groupNumber = 13
    
    if RevisionControlNumber == 0:
        startOffset = 1408
        endOffset = 1439
    elif RevisionControlNumber <= 272188:
        startOffset = 1408
        endOffset = 1439
    else:
        startOffset = 1408
        endOffset = 1439

    # Values
    FlashRows = None
    
    
    def __init__(self, data):
        
        # Save the raw data
        self.data = data
        
        # Parse the input data
        self._parseData(data)
        
        return
        
    def _parseData(self, data):
        ''' Parse out the Group-13 Data '''
        
        # Initialize the offset value
        self.offset = 0        
        
        # Parse the DES_VAL0
        self.DES_VAL0 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL1
        self.DES_VAL1 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL2
        self.DES_VAL2 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL3
        self.DES_VAL3 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL4
        self.DES_VAL4 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL5
        self.DES_VAL5 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL6
        self.DES_VAL6 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL7
        self.DES_VAL7 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL8
        self.DES_VAL8 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL9
        self.DES_VAL9 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL10
        self.DES_VAL10 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL11
        self.DES_VAL11 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL12
        self.DES_VAL12 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL13
        self.DES_VAL13 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL14
        self.DES_VAL14 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL15
        self.DES_VAL15 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL16
        self.DES_VAL16 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL17
        self.DES_VAL17 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL18
        self.DES_VAL18 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL19
        self.DES_VAL19 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL20
        self.DES_VAL20 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL21
        self.DES_VAL21 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL22
        self.DES_VAL22 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL23
        self.DES_VAL23 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL24
        self.DES_VAL24 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL25
        self.DES_VAL25 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL26
        self.DES_VAL26 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL27
        self.DES_VAL27 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL28
        self.DES_VAL28 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL29
        self.DES_VAL29 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL30
        self.DES_VAL30 = self._parseNextValue(numBytes=1)
        
        # Parse the DES_VAL31
        self.DES_VAL31 = self._parseNextValue(numBytes=1)
        
        return

    def _parseNextValue(self, numBytes=0):
        val = 0
        for shift, i in enumerate(range(self.offset, self.offset+numBytes)):
            val += (int(self.data[i]) << (8*shift))
            self.offset += 1
        return val

# End of File