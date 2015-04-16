import xmlobjects

class captureXMLConfigFile(object):
    configFile=""
    
    def __init__(self, configFile):
        self.configFile=configFile
        return
        
    def get_data(self):
        
        try:
            log_file = open(self.configFile, "r")
            log_string = unicode(log_file.read())
            data_obj = xmlobjects.fromstring(log_string)
            
            return data_obj
        except Exception, e:
            print self.configFile + " get_data: " + str(e)
            return 0
    
    def get_item(self,item):
        config=self.get_data()
        for testItem in config.testItemDefinition.testDefinition:
            if testItem.Test==item:
                return testItem
        return item + "is not found"

#     def get_other(self,item):
#         config=self.get_data()
#         print config.getElementsByTagName("SensorColumns")        
#         for testItem in config:
#             print testItem
#             if testItem==item:
#                 return testItem
#         return item + "is not found"
    
def main():
    configFile_data=captureXMLConfigFile("d:/Gen4ConfigFile.xml")
    
    configTest=configFile_data.get_item("VCOM")
    print configTest.Max
    print configFile_data.get_data().SensorColumns
    print configFile_data.get_data().I2CAdress
    #print config.testItemDefinition.VCOM.Test
    #print config.testItemDefinition.testDefinition[0].Test
    
if __name__ == "__main__":
    main() 
