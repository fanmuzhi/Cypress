'''
Module Name:    PWRDriver.py
Author(s):      PZHO
Description:    This module implements communications with power supply PWR1600L via
                USB interface.
Dependencies:   win32com or comtypes
'''

import comtypes
import comtypes.client
from time import sleep
import os
import traceback


class DeviceError(Exception):    
    def __init__(self, **kvargs):
        self.code = kvargs.get('code', 0)
        self.message = kvargs.get('message', "undefined error message")
    def __str__(self):
        return repr(self.message)

class DeviceErrors(object):
    createCOMObject=DeviceError(code=0x01, message="can not create COM object for Kikusui4800")
    open=DeviceError(code=0x02, message="can not open device")
    close=DeviceError(code=0x03, message="can not close device")
    reset=DeviceError(code=0x03, message="can not reset device")
    openChannel=DeviceError(code=0x03, message="can not open channel")
    setVoltage=DeviceError(code=0x03, message="can not set voltage")
    setCurrentLimit=DeviceError(code=0x03, message="can not set current")
    activeOutput=DeviceError(code=0x03, message="can not enable output")
    De_activeOutput=DeviceError(code=0x03, message="can not disable output")
    setOVP=DeviceError(code=0x03, message="can not set OVP")
    setOCP=DeviceError(code=0x03, message="can not set OCP")
    measureVolt=DeviceError(code=0x03, message="can not measure Voltage")
    measureCurrent=DeviceError(code=0x03, message="can not measure Current")
    queryState=DeviceError(code=0x03, message="can not query device status")
    

class PowerSupplyDevice(object):
    
    voltage=5
    current=10
    
    
    def __init__(self):
        
        self.m_output=None
        
        try:
            self._PWR=comtypes.client.CreateObject("Kikusui4800.Kikusui4800")
            
            # See if there was an error connecting to the bridge
            if self._PWR is None:
                raise DeviceErrors.createCOMObject
            
        except Exception as ex:
            print DeviceErrors.createCOMObject
        
    def open(self,port="USB0::0x0B3E::0x1014::NB003730::0::INSTR",option=""):
        Status=self._PWR.Initialize(port,True,False,option)
                
        if Status!=0:
            raise DeviceErrors.open       
        
        #print self._PWR.Identity.InstrumentModel + " Ver" + self._PWR.Identity.InstrumentFirmwareRevision
        errorCode=0
        StrErrMessage=""
        self._PWR.Utility.ErrorQuery(errorCode,StrErrMessage)       
        while errorCode!=0:
            self._PWR.Utility.ErrorQuery(errorCode,StrErrMessage)        
        
        return
    
    def close(self):
        self._PWR.close()
        return
    
    def reset(self):
        if not self._PWR.Initialized:
            return
        
        try:
            self._PWR.Reset()
            errorCode=0
            StrErrMessage=""
            self._PWR.Utility.ErrorQuery(errorCode,StrErrMessage)
            #print errorCode
            while errorCode!=0:
                self._PWR.Utility.ErrorQuery(errorCode,StrErrMessage) 
        except Exception as ex: 
            raise DeviceErrors.reset
        
        return
            
    def openChannel(self,channelName="N5!C1"):
        try:
            self.m_output=self._PWR.Outputs.Item["N5!C1"]
            #print self.m_output
        except Exception as ex: 
            raise DeviceErrors.openChannel
        return
                
    def setVoltage(self,volt):
        #set output voltage
        if not self._PWR.Initialized:
            return
        try:
            self.m_output.VoltageLevel=volt
            
        except Exception as ex:
            raise DeviceErrors.setVoltage       
        return
    
    def setCurrentLimit(self,currentLimit):
        #set current output
        if not self._PWR.Initialized:
            return
        try:
            self.m_output.CurrentLimit=currentLimit
            
        except Exception as ex:
            raise DeviceErrors.setCurrentLimit     
        
        return
    
    def activeOutput(self):
        #enable voltage output
        if not self._PWR.Initialized:
            return
        try:
            self.m_output.Enabled=True
            
        except Exception as ex:
            raise DeviceErrors.activeOutput
        
        return
    
    def De_activeOutput(self):
        #disable voltage output
        if not self._PWR.Initialized:
            return
        try:
            self.m_output.Enabled=False
            
        except Exception as ex:
            raise DeviceErrors.De_activeOutput
        return
    
    def setOVP(self,volt):
        #set OVP limit
        if not self._PWR.Initialized:
            return
        try:
            self.m_output.OVPLimit=volt
            
        except Exception as ex:
            raise DeviceErrors.setOVP  
        return
    
    def setOCP(self,current):
        #set OCP limit
        if not self._PWR.Initialized:
            return
        try:
            self.m_output.OCPLimit=current
            
        except Exception as ex:
            raise DeviceErrors.setOCP      
        return
    
    def measureVolt(self):
        #measure output voltage
        if not self._PWR.Initialized:
            return
        try:
            measuredVolt=self.m_output.Measure(1)            
        except Exception as ex:
            raise DeviceErrors.measureVolt   
           
        return measuredVolt
        
    def measureCurrent(self):
        #measure output current
        if not self._PWR.Initialized:
            return
        try:
            measuredCurrent=self.m_output.Measure(0)            
        except Exception as ex:
            raise DeviceErrors.measureCurrent
           
        return measuredCurrent
    
    def QueryState(self):
        #Query the state of output
        if not self._PWR.Initialized:
            return
        try:
            if self.m_output.QueryState(0):
                return "CV"
            if self.m_output.QueryState(1):
                return "CC"
            if self.m_output.QueryState(2):
                return "OVP"
            if self.m_output.QueryState(3):
                return "OCP"
            if self.m_output.QueryState(4):
                return "UNR"
            if self.m_output.QueryState(1001):
                return "CP"
                        
        except Exception as ex:
            raise DeviceErrors.queryState   
           
        

        
def main():
    PS=PowerSupplyDevice()
    PS.open()    
    PS.openChannel() 
    PS.setVoltage(12)
    PS.activeOutput()
    sleep(5)
    print PS.measureVolt()
    print PS.QueryState()
    #print PS.measureCurrent()
    PS.close()
    
if __name__ == '__main__':
    main()       
    
       
            
            
            