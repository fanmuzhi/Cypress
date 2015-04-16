import bridgeDriver
from time import sleep
import json

returnData={}
returnData['TestItem']='OpenPort'
returnData['TestResult']='Pass'
returnData['ErrorCode']=0
returnData['ErrorMessage']=""
returnData['Data']=0

bridge=bridgeDriver.BridgeDevice()   

def openPort():    
    
    # Get a list of ports
    portList = bridge.GetPorts()
    print 'Port list:', portList
    
    if portList is None:
        returnData['TestResult']='Fail'
        returnData['ErrorMessage']='NO BRIDGES AVAILABLE'
        print 'NO BRIDGES AVAILABLE'
        jsonReturn=json.dumps(returnData, True)
        return jsonReturn
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
        
        # Open the port
        openPortStatus=bridge.OpenPort(port)
        bridge.PrintMessages()
       
        if openPortStatus:
            print "port opened"
            returnData['TestResult']='Pass'
            returnData['ErrorMessage']=''        
            jsonReturn=json.dumps(returnData, True)
            print jsonReturn
            return jsonReturn
        else:
            print "port not opened"
            returnData['TestResult']='Fail'
            returnData['ErrorMessage']='port not opened'        
            jsonReturn=json.dumps(returnData, True)        
            return jsonReturn      
    
    
    
if __name__=="__main__":
    openPort()
        
        