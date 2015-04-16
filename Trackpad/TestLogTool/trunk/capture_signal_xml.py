import xmlobjects
import re
import os
import csv

TEST_LOG_PATH = "./signal test results"

class CaptureSignalData:
    
    def cleanup_xml(self, file):
        '''
        clean up the "Signal Value" to "signal_value" ;
        celan up the "D" to "data";
        '''
        clean_file = CleanupXML()
        
        clean_file.change_D_to_data(file)
        clean_file.change_signal_tag(file)

    
    def get_lists(self, file):
        '''
        get signal list from file;
        '''
        
        log_file = open(file,"r")
        log_string = log_file.read()
        data_obj = xmlobjects.fromstring(log_string)
        
        (dir, filename) = os.path.split(file)
        (serialnumber, ext) = os.path.splitext(filename)
        
        #print serialnumber
        
        data_list = ["'"+serialnumber+"'", "Signal",]
        
        for d in data_obj.signal_value.data:
            data_list.append(int(d))
        
        return data_list
        
    
    def get_files(self, test_log_path):
        
        #get file list
        
        file_list = []
        for root, dirs, files in os.walk(test_log_path):
            for f in files:
                (filename, ext) = os.path.splitext(f)
                if ext == ".xml" or ext == ".XML":
                    #print f
                    file_list.append(os.path.join(root, f))
        
        
        return file_list

    
class CleanupXML:
    
    def change_D_to_data(self, file_path):
        '''
        change xml tag "D" to "data"
        '''
        log_file = open(file_path, "r+")    #open file for read and write
        log_string = log_file.read()
        
        regex_m = re.compile(r"<D\d+>")
        log_string = regex_m.sub('<data>', log_string)
        
        regex_n = re.compile(r"</D\d+>")
        log_string = regex_n.sub('</data>', log_string)
    
        log_file.seek(0)            #point to the head of file
        log_file.write(log_string)  #write the string
        log_file.close()
    
    def change_signal_tag(self, file_path):
        '''
        change xml tag "Signal Value" to "signal_value"
        '''
        log_file = open(file_path, "r+")
        log_string = log_file.read()
        
        regex_m = re.compile(r"<Signal\sValue>")
        log_string = regex_m.sub("<signal_value>", log_string)
        
        regex_n =  re.compile(r"</Signal\sValue>")
        log_string = regex_n.sub("</signal_value>", log_string)
        
        log_file.seek(0)
        log_file.write(log_string)
        log_file.close()


def temp_write_csv(data_lists):
    
    csvfile = open("./dut_signal.csv", "wb")
    csvwriter = csv.writer(csvfile, delimiter=',', quotechar='|', quoting=csv.QUOTE_MINIMAL)
    
    for data_list in data_lists:
        #write head
        headerList=[]
        for i in range((len(data_list)-2)):
            headerList.append(i+1)
    
        csvwriter.writerow(["SerialNumber","Statistic"] + headerList)
        
        break

    for data_list in data_lists:
        #write row
        csvwriter.writerow(data_list)

        
if __name__ == "__main__" :
    
    signal_process = CaptureSignalData()
    files = signal_process.get_files(TEST_LOG_PATH)
    
    data_lists = [] 
    
    for file in files:
        signal_process.cleanup_xml(file)
        data_lists.append(signal_process.get_lists(file))
    
    temp_write_csv(data_lists)
    
    print("capture_signal_xml.py finish")
    
    
    
        
        
        
        