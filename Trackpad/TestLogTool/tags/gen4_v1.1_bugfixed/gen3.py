import xmlobjects
import os
import sys
import csv

from lib import testlogformat   #import testlogformat.py from the lib folder

TEST_LOG_PATH="/home/qibo/workspace/Test Log Tool/sample test log/"

def object_to_xml():
    xobj = xmlobjects.XMLObject('root_elem') 
    xobj.sub_element_string = 'test string'
    xobj.sub_element_int = 89
    xobj.sub_element_float = 6.7
    xobj.sub_element_empty = None
    
    print xmlobjects.tostring(xobj)

def xml_to_object():
    global file_paths
    
    try:
        csvfile=open('result.csv', 'wb')
        csvwriter = csv.writer(csvfile, delimiter=',', quotechar='|', quoting=csv.QUOTE_MINIMAL)
    
        for file_path in file_paths:
        
            log_file=open(file_path, "r")
            log_string=log_file.read()
            xobj=xmlobjects.fromstring(log_string)
       
            #for raw_count in xobj.Raw_Count_Averages.data:
            #    print raw_count
        
            #print xobj.Raw_Count_Averages.data

            csvwriter.writerow([xobj.Serial_Number, str(xobj.Test_Time), str(xobj.Error_Code) , xobj.Test_Station])
          
            print xobj.Serial_Number+"\t"+xobj.Test_Time
            
        csvfile.close()
            
    except TypeError as e:
        print file_path + ": " + e.args[0]
    except Exception as e:
        print e
    except :
        print file_path + " Unexpected error:", sys.exc_info()[0]

def format_cleanup():
    global file_paths
    
    try:
        for file_path in file_paths:
            logupdate=testlogformat.TestLogFormat()
            logupdate.change_D_to_data(file_path)
            logupdate.change_serialnumber_to_string(file_path)
            logupdate.change_time_format(file_path)
    except Exception as e:
        print e
 
def get_files():
    global file_paths
    
    try:
        file_paths=[]   #declare a list
        for root, dirs, files in os.walk(TEST_LOG_PATH):
            #for d in dirs:
            #    print os.path.join(root, d)
            for f in files:
                file_paths.append(os.path.join(root, f))    #add file path to a list
    except Exception as e:
        print e


if __name__ == "__main__":
    get_files()
    format_cleanup()
    xml_to_object()
    
    #logtest=testlogformat.TestLogFormat()
    #logtest.change_serialnumber_to_string("/home/qibo/workspace/Test Log Tool/sample test log/0120050005125210D91.xml")
