#===============================================================
#
#   name:       gen3.py
#   purpose:    this script is used to read the raw MTG test
#               logs with EVT data and convert to the summary csv file.
#   author:     qibo
#   date:       2013-5-21
#
#===============================================================

import xmlobjects
import numpy as np
import matplotlib.pyplot as plt
import mpl_toolkits.mplot3d
from matplotlib import cm
import re
import os
#import csv

#===============================================================
# Global Settings for File Paths
#===============================================================
TEST_LOG_PATH = "./01201100/"
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
        # open singlefile for read and write
        log_file = open(file_path, "r+")
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

        regex = re.compile(
            r"<Serial_Number>(?P<SerialNumber>\w{19})</Serial_Number>")
        r = regex.search(log_string)

        log_file.close()

        if r is not None:
            # group(0) is the whole match, from <Serial_number>.
            # group(1) is the second match, the \w{19}.
            serialnumber = "<Serial_Number>'" + \
                r.group(1) + \
                "'</Serial_Number>"
            # replace with the ' '
            log_string = regex.sub(serialnumber, log_string)

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
            # cut the last Z
            testtime = "<Test_Time>" + r.group(1)[:-1] + "</Test_Time>"
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


class DrawPlot:
    def __init__(self, path2save):
        self.picture_number = 0

        if not os.path.exists(path2save):
            os.makedirs(path2save)
        self.path = path2save

    def draw_hist(self, data_list, data_name):
        self.picture_number += 1
        y = np.array(data_list, dtype=float)
        y = y.ravel()
        plt.figure(self.picture_number, figsize=(8, 4))  # Create a new figure
        plt.hist(y, bins=50, range=None, normed=False)
        plt.title(data_name + " Histogram Plot")     # Give the figure a title
        plt.xlabel('value')          # Label the x-axis
        plt.ylabel('count')          # Label the y-axis
        plt.grid(True)               # Turn the grid on
        plt.savefig(self.path + data_name + '_hist.pdf')
        plt.close()

    def draw_runchart(self, data_list, data_name):
        self.picture_number += 1
        # 2D Matrix, duts * sensors
        data_array = np.array(data_list, dtype=int)
        # caculate stdev of data, output 1D Matrix, rows + columns
        std_data = np.std(data_array, axis=0)
        #print data_array

        x = np.arange(len(std_data))
        y = std_data

        plt.figure(self.picture_number, figsize=(8, 4))
        plt.plot(x, y, 'ro')
        plt.title(data_name + " StdDev Plot")
        plt.xlabel("sensor")
        plt.ylabel("value")
        plt.grid(True)
        plt.savefig(self.path + data_name + '_stdDev.pdf')
        plt.close()

    def draw_3Dplot(self, data_list, data_name):
        self.picture_number += 1
        # 3D Matrix, duts * rows * columns
        data_array = np.array(data_list, dtype=int)
        # caculate stdev of duts, output 2D Matrix, rows * columns
        std_data = np.std(data_array, axis=0)

        x_arange, y_arange = std_data.shape
        x, y = np.mgrid[:x_arange, :y_arange]

        plt.figure(self.picture_number, figsize=(8, 4))
        plt.title(data_name + " StdDev Surface")
        # 111 means 1 row 1 column and the No.1 subplot
        ax = plt.subplot(111, projection='3d')
        ax.plot_surface(x, y, std_data, rstride=1, cstride=1, cmap=cm.coolwarm)
        ax.set_xlabel("sensor_x")
        ax.set_ylabel("sensor_y")
        ax.set_zlabel(data_name + " StdDev")
        plt.savefig(self.path + data_name + '_stdDev_3D.pdf')
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

    # Draw Plots
    rawcount_list = []
    noise_list = []
    idac_list = []
    signal_list = []
    snr_list = []
    for dut in dut_results:
        try:
            rawcount_list.append(dut.Raw_Count_Averages.data)
            noise_list.append(dut.Raw_Count_Noise.data)
            idac_list.append(dut.Global_IDAC_Value.data)
            signal_list.append(dut.Signal_Data.data)
            snr_list.append(dut.SNR_Data.data)
        except Exception, e:
            print e

    draw_plot = DrawPlot(RESULT_LOG_PATH)
    draw_plot.draw_hist(rawcount_list, "RAWCOUNT")
    draw_plot.draw_hist(noise_list, "NOISE")
    draw_plot.draw_hist(idac_list, "IDAC")
    draw_plot.draw_hist(signal_list, "Signal")
    draw_plot.draw_hist(snr_list, "SNR")

    draw_plot.draw_runchart(rawcount_list, "RAWCOUNT")
    draw_plot.draw_runchart(noise_list, "NOISE")
    draw_plot.draw_runchart(idac_list, "IDAC")
    draw_plot.draw_runchart(signal_list, "Signal")
    draw_plot.draw_runchart(snr_list, "SNR")
    print("finish")


if __name__ == "__main__":
    main()
