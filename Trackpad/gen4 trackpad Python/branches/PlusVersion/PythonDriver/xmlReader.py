import xmlobjects

class captureXMLConfigFile:
    
    def get_data(self, configFile):
        
        try:
            log_file = open(configFile, "r")
            log_string = unicode(log_file.read())
            data_obj = xmlobjects.fromstring(log_string)
            
            return data_obj
        except Exception, e:
            print configFile + " get_data: " + str(e)
            return 0
    
    def get_item(self,item,configFile):
        config=self.get_data(configFile)
        for testItem in config.testItemDefinition.testDefinition:
            if testItem.Test==item:
                return testItem
        return item + "is not found"

    def get_other(self,item,configFile):
        config=self.get_data(configFile)
        for testItem in config:
            if testItem==item:
                return testItem
        return item + "is not found"
    
def main():
    configFile_data=captureXMLConfigFile()
    config=configFile_data.get_data("d:/Gen4ConfigFile.xml")
    
    configTest=configFile_data.get_item("VCOM")
    print configTest.Max
    #print config.testItemDefinition.VCOM.Test
    #print config.testItemDefinition.testDefinition[0].Test
    
if __name__ == "__main__":
    main() 
