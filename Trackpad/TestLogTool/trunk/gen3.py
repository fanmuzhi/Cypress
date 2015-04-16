#===============================================================
#
#   name:       gen3.py
#   purpose:    this script is used to read the raw MTG test
#               logs and convert to the summary csv file.
#   author:     qibo
#   date:       2013-3-07
#
#===============================================================

import xmlobjects
import numpy as np
import matplotlib.pyplot as plt
import re
import os
import csv

#===============================================================
# Global Settings for File Paths
#===============================================================
TEST_LOG_PATH = "./012011/"
RESULT_LOG_PATH = TEST_LOG_PATH + "TEST_STATS" + "/"
#RESULT_LOG_PATH = "./"


class CaptureData:

    def cleanup_xml(self, singlefile):
        '''
        clean up the "Signal Value" to "signal_value" ;
        celan up the "D" to "data";
        '''
        clean_file = CleanupXML()

        clean_file.change_D_to_data(singlefile)
        clean_file.change_signal_tag(singlefile)
        clean_file.change_serialnumber_to_string(singlefile)
        clean_file.change_time_format(singlefile)

    def get_data(self, singlefile):
        '''
        get signal list from singlefile;
        '''

        try:
            log_file = open(singlefile, "r")
            log_string = unicode(log_file.read())

            data_obj = xmlobjects.fromstring(log_string)

            return data_obj
        except Exception, e:
            print singlefile + " get_data: " + str(e)
            return 0

        #(dir, filename) = os.path.split(singlefile)
        #(serialnumber, ext) = os.path.splitext(filename)

        #print serialnumber

        #data_list = ["'" + serialnumber + "'", "Signal"]

        #for d in data_obj.signal_value.data:
        #    data_list.append(int(d))

        #return data_list

    def get_files(self, test_log_path):

        #get singlefile list

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
        log_file = open(file_path, "r+")    # open singlefile for read and write
        log_string = log_file.read()

        regex_m = re.compile(r"<D\d+>")
        log_string = regex_m.sub('<data>', log_string)

        regex_n = re.compile(r"</D\d+>")
        log_string = regex_n.sub('</data>', log_string)

        log_file.seek(0)                     # point to the head of singlefile
        log_file.write(log_string)           # write the string
        log_file.close()

    def change_serialnumber_to_string(self, file_path):
        log_file = open(file_path, "r")   # open file for read
        log_string = log_file.read()

        regex = re.compile(r"<Serial_Number>(?P<SerialNumber>\w{19})</Serial_Number>")
        r = regex.search(log_string)

        log_file.close()

        if r is not None:
            # group(0) is the whole match, from <Serial_number>.
            # group(1) is the second match, the \w{19}.
            serialnumber = "<Serial_Number>'" + r.group(1) + "'</Serial_Number>"
            log_string = regex.sub(serialnumber, log_string)  # replace with the ' '

            log_file = open(file_path, "w")   # open file for write
            log_file.truncate()
            log_file.seek(0)            # point to the head of file
            log_file.write(log_string)  # write the string

    def change_time_format(self, file_path):
        log_file = open(file_path, "r")   # open the read of file
        log_string = log_file.read()

        regex = re.compile(r"<Test_Time>(?P<TestTime>.{20})</Test_Time>")
        r = regex.search(log_string)

        log_file.close()    # close the read of file

        if r is not None:
            testtime = "<Test_Time>" + r.group(1)[:-1] + "</Test_Time>"  # cut the last Z
            log_string = regex.sub(testtime, log_string)

            log_file = open(file_path, "w")   # open the write of file
            log_file.truncate()
            log_file.seek(0)                  # point to the head of file
            log_file.write(log_string)        # write the string

    def change_signal_tag(self, file_path):
        '''
        change xml tag "Signal Value" to "signal_value"
        '''
        log_file = open(file_path, "r+")
        log_string = log_file.read()

        regex_m = re.compile(r"<Signal\sValue>")
        log_string = regex_m.sub("<signal_value>", log_string)

        regex_n = re.compile(r"</Signal\sValue>")
        log_string = regex_n.sub("</signal_value>", log_string)

        log_file.seek(0)
        log_file.write(log_string)
        log_file.close()


class WriteCSV:
    '''
    '''
    def write_list_to_file(self, dut_results, file_path, data_name, selfcap):

        csvfile = open(file_path + data_name + ".csv", 'wb')
        csvwriter = csv.writer(csvfile, delimiter=',', quotechar='|', quoting=csv.QUOTE_MINIMAL)

        #write head
        if data_name == "Raw_Count_Averages":
            headerList = range(1, len(dut_results[0].Raw_Count_Averages.data) + 1, 1)
        if data_name == "Raw_Count_Noise":
            headerList = range(1, len(dut_results[0].Raw_Count_Noise.data) + 1, 1)
        if data_name == "Global_IDAC_Value":
            headerList = range(1, len(dut_results[0].Global_IDAC_Value.data) + 1, 1)
        csvwriter.writerow(["SerialNumber", "Statistic"] + headerList)

        #write content
        for dut in dut_results:
            try:
                tempList = [dut.Serial_Number, data_name]
                if data_name == "Raw_Count_Averages":
                    for data in dut.Raw_Count_Averages.data:
                        tempList.append(data)
                if data_name == "Raw_Count_Noise":
                    for data in dut.Raw_Count_Noise.data:
                        tempList.append(data)
                if data_name == "Global_IDAC_Value":
                    for data in dut.Global_IDAC_Value.data:
                        tempList.append(data)

                csvwriter.writerow(tempList)
                #print tempList
            except Exception, e:
                print "write_list_to_file: " + str(e)

        csvfile.close()

    def write2files(self, path, dut_results):
        self.check_paths(path)
        csvfile = open(path + "dut.csv", 'wb')
        csvwriter = csv.writer(csvfile, delimiter=',', quotechar='|',
                               quoting=csv.QUOTE_MINIMAL)

        # write csv header
        csvwriter.writerow(["Serial_Number", "Test_Station", "Error_Code",
                            "Test_Time", "IDD_Value", "Firmware_Revision"])

        for dut in dut_results:
            try:
                csvwriter.writerow([dut.Serial_Number, dut.Test_Station, dut.Error_Code,
                                    dut.Test_Time, dut.IDD_Value, dut.Firmware_Revision])
            #print dut["SerialNumber"]
            except Exception, e:
                print "write_CSVfiles: " + str(e)

        csvfile.close()

        self.write_list_to_file(dut_results, path, "Raw_Count_Averages", False)
        self.write_list_to_file(dut_results, path, "Raw_Count_Noise", False)
        self.write_list_to_file(dut_results, path, "Global_IDAC_Value", False)

    def check_paths(self, path):
        if not os.path.exists(path):
            os.makedirs(path)


class DrawPlot:
    def __init__(self, path2save):
        self.picture_number = 0
        self.path = path2save

    def draw_hist(self, data_list, data_name):
        self.picture_number += 1
        y = np.array(data_list, dtype=float)
        plt.figure(self.picture_number, figsize=(8, 4))    # Create a new figure
        plt.hist(y, bins=50, range=None, normed=False)
        plt.title(data_name)     # Give the figure a title
        plt.xlabel('value')          # Label the x-axis
        plt.ylabel('count')          # Label the y-axis
        plt.grid(True)               # Turn the grid on
        plt.savefig(self.path + data_name + '_hist.pdf')
        plt.close()

    def draw_runchart(self, data_list, data_name):
        self.picture_number += 1
        data_array = np.array(data_list, dtype=int)       # 2D Matrix, duts * sensors
        std_data = np.std(data_array, axis=0)             # caculate stdev of data, output 1D Matrix, rows + columns
        print data_array

        x = np.arange(len(std_data))
        y = std_data

        plt.figure(self.picture_number, figsize=(8, 4))
        plt.plot(x, y, 'ro')            # Plot the results (takes the same arguments as MATLAB)
        plt.title(data_name + " StdDev Plot")
        plt.xlabel("sensor")
        plt.ylabel("value")
        plt.grid(True)
        plt.savefig(self.path + data_name + '_stdDev.pdf')
        plt.close()


def main():
    # Capture data from test log path
    capture_data = CaptureData()
    files = capture_data.get_files(TEST_LOG_PATH)

    dut_results = []
    for singlefile in files:
        capture_data.cleanup_xml(singlefile)
        mydut = capture_data.get_data(singlefile) 
        
        if mydut != 0:
            dut_results.append(mydut)

    # Write data to CSV files
    write_csv = WriteCSV()
    write_csv.write2files(RESULT_LOG_PATH, dut_results)

    # Draw Plots
    rawcount = []
    noise = []
    idac = []
    rawcount_list = []
    noise_list = []
    idac_list = []
    for dut in dut_results:
        try:
            rawcount_list.append(dut.Raw_Count_Averages.data)
            noise_list.append(dut.Raw_Count_Noise.data)
            idac_list.append(dut.Global_IDAC_Value.data)
            for data in dut.Raw_Count_Averages.data:
                rawcount.append(data)
            for data in dut.Raw_Count_Noise.data:
                noise.append(data)
            for data in dut.Global_IDAC_Value.data:
                idac.append(data)
        except Exception, e:
            print e

    draw_plot = DrawPlot(RESULT_LOG_PATH)
    draw_plot.draw_hist(rawcount, "RAWCOUNT")
    draw_plot.draw_hist(noise, "NOISE")
    draw_plot.draw_hist(idac, "IDAC")

    draw_plot.draw_runchart(rawcount_list, "RAWCOUNT")
    draw_plot.draw_runchart(noise_list, "NOISE")
    draw_plot.draw_runchart(idac_list, "IDAC")
    print("finish")


if __name__ == "__main__":
    main()
