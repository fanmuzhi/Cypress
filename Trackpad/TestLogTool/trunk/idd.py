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
#import re
import os
import csv

#===============================================================
# Global Settings for File Paths
#===============================================================
TEST_LOG_PATH = "./012005 IDD/"
RESULT_LOG_PATH = TEST_LOG_PATH + "TEST_STATS" + "/"
#RESULT_LOG_PATH = "./"


class CaptureData:

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


class WriteCSV:

    def write2files(self, path, dut_results):
        self.check_paths(path)
        csvfile = open(path + "dut.csv", 'wb')
        csvwriter = csv.writer(csvfile, delimiter=',', quotechar='|',
                               quoting=csv.QUOTE_MINIMAL)

        # write csv header
        csvwriter.writerow(["Serial_Number", "Test_Station", "Error_Code",
                            "Test_Time", "IDD_Sleep1_Value", "IDD_Deep_Sleep_Value"])

        for dut in dut_results:
            try:
                csvwriter.writerow([dut.Serial_Number, dut.Test_Station, dut.Error_Code,
                                    dut.Test_Time, dut.IDD_Sleep1_Value, dut.IDD_Deep_Sleep_Value])
            #print dut["SerialNumber"]
            except Exception, e:
                print "write_CSVfiles: " + str(e)

        csvfile.close()

    def check_paths(self, path):
        if not os.path.exists(path):
            os.makedirs(path)


class DrawPlot:
    def __init__(self, path2save):
        self.picture_number = 0
        self.path = path2save
        if not os.path.exists(path2save):
            os.makedirs(path2save)
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
        mydut = capture_data.get_data(singlefile)
        if mydut != 0:
            dut_results.append(mydut)

    # Write CSV
    csvFile = WriteCSV()
    csvFile.write2files(RESULT_LOG_PATH, dut_results)

    # Draw Plots
    sleep_list = []
    deepsleep_list = []
    for dut in dut_results:
        try:
            sleep_list.append(dut.IDD_Sleep1_Value)
            deepsleep_list.append(dut.IDD_Deep_Sleep_Value)
        except Exception, e:
            print e

    draw_plot = DrawPlot(RESULT_LOG_PATH)
    draw_plot.draw_hist(sleep_list, "Sleep1")
    draw_plot.draw_hist(deepsleep_list, "Deep Sleep")

    #draw_plot.draw_runchart(rawcount_list, "RAWCOUNT")
    #draw_plot.draw_runchart(noise_list, "NOISE")
    #draw_plot.draw_runchart(idac_list, "IDAC")
    print("finish")


if __name__ == "__main__":
    main()
