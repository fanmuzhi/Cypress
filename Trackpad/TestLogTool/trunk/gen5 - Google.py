#===============================================================
#
#   name:       gen4.py
#   purpose:    this script is used to read the raw gen4 test
#               logs and convert to the summary csv file.
#   author:     qibo
#   date:       2013-1-17
#
#===============================================================

import os
import types
#import sys
#import csv
import re
#import math
import numpy as np
import matplotlib.pyplot as plt
import mpl_toolkits.mplot3d
from matplotlib import cm
#from pprint import pprint
import xmlobjects
import csv
#===============================================================
# Global Settings for Test Result
#===============================================================
TEST_RESULT = "ALL"
#TEST_RESULT = "PASS"               #only look for pass logs
#TEST_RESULT = "FAIL"               #only look for fail logs


#===============================================================
# Global Settings for File Paths
#===============================================================
TEST_LOG_PATH = "./11500400/"
RESULT_LOG_PATH = TEST_LOG_PATH + "/" + "TEST_STATS" + "/"
#RESULT_LOG_PATH = "./"
#SIGNAL_LOG_PATH = "./118003 EVT/signal/"


class CaptureSignalData:

    def __init__(self, test_log_path):
        if not os.path.exists(test_log_path):
            raise Exception(test_log_path + " path doesn't exsist.")
        else:
            self.log_path = test_log_path

    def __cleanup_xml(self, file):
        '''
        clean up the "Signal Value" to "signal_value" ;
        celan up the "D" to "data";
        '''
        clean_file = CleanupSignalXML()
        clean_file.change_D_to_data(file)
        clean_file.change_signal_tag(file)

    def __get_file(self, serialnumber):
        #get file list
        for root, dirs, files in os.walk(self.log_path):
            for f in files:
                (filename, ext) = os.path.splitext(f)
                if ext == ".xml" or ext == ".XML":
                    if filename == serialnumber:
                        return os.path.join(root, f)

    def get_lists(self, serialnumber):
        '''
        get signal list from file;
        '''
        myfile = self.__get_file(serialnumber)
        self.__cleanup_xml(myfile)

        log_file = open(myfile, "r")
        log_string = log_file.read()
        data_obj = xmlobjects.fromstring(log_string)

        data_list = []

        for d in data_obj.signal_value.data:
            data_list.append(int(d))
        return data_list


class CleanupSignalXML:

    def change_D_to_data(self, file_path):
        '''
        change xml tag "D" to "data"
        '''
        log_file = open(file_path, "r+")    # open file for read and write
        log_string = log_file.read()

        regex_m = re.compile(r"<D\d+>")
        log_string = regex_m.sub('<data>', log_string)

        regex_n = re.compile(r"</D\d+>")
        log_string = regex_n.sub('</data>', log_string)

        log_file.seek(0)                     # point to the head of file
        log_file.write(log_string)           # write the string
        log_file.close()

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


class CaptureData:
    #===============================================================
    # Global Regular Expression Pattern
    #===============================================================
    regex_head = re.compile(r",\s\.header(.*?),\s\.end",
                            re.UNICODE)
    regex_content = re.compile(r",\s\.engineering\sdata(.*?),\s\.end",
                               re.UNICODE)

    # Regex for single value
    regex_single = []
    regex_summary = re.compile(r",\sDATE,\s(?P<TestDate>[^,]*),\sTIME,\s(?P<TestTime>[^,]*)\s*,\sSW\sVERSION,\s(?P<SWVersion>(?:\d{1,2}.){3}\d{1,8}),\sOPERATOR,\s(?P<Operator>\d{1,5}),\sTEST\sSTATION,\s(?P<TestStaion>\b\w*\b),\sTEST\sFACILITY,\s(?P<TestFacility>\b\w*\b)\s*,\sCONFIG\sFILE,\s(?P<ConfigFile>[^,]*),\sEXECUTION\sMODE,\s(?P<TestMode>\b\w*\b),\sSENSOR\sROWS,\s(?P<RowNumber>\d{1,2}),\sSENSOR\sCOLUMNS,\s(?P<ColumnNumber>\d{1,2}),\s(?P<TestResult>\b\w*\b)", re.UNICODE)
    regex_engineerdataline1 = re.compile(r"\s(?P<SerialNumber>\w{19}),\s(?P<ChipID>[^,]*),\s(?:ERRORS,\s)?(?P<ErrorCode>[^:]*):\s(?P<ErrorMessage>[^,]*)", re.UNICODE)
        
    regex_fw_ver = re.compile(r"\sFW\sVersion,([^,]*,){4}\sVersion,\s(?P<FW_Version>\d{1,2}.\d{1,2})", re.UNICODE)
    regex_fw_rev = re.compile(r"\sFW\sRevision\sControl,([^,]*,){4}\sRevision,\s(?P<FW_Revision>[^,]*)", re.UNICODE)
    regex_elapsedtime = re.compile("\sELAPSED\sTIME,\s(?P<ElapsedTime>\d+\.?\d?)", re.UNICODE)
    regex_vcom = re.compile("\sVCOM\sVoltage,(?:[^,]*,){5}\s(?P<VCOMVoltage>\d{1,2}.\d{1,2})", re.UNICODE)
    regex_vaux = re.compile("\sVAUX\sVoltage,(?:[^,]*,){5}\s(?P<VAUXVoltage>\d{1,2}.\d{1,2})", re.UNICODE)
    regex_icom = re.compile("\sICOM\sCurrent,(?:[^,]*,){5}\s(?P<ICOMCurrent>\d{1,3}.\d{1,2})", re.UNICODE)
    regex_iaux = re.compile("\sIAUX\sCurrent,(?:[^,]*,){5}\s(?P<IAUXCurrent>\d{1,3}.\d{1,2})", re.UNICODE)

    regex_single.append(regex_summary)
    regex_single.append(regex_engineerdataline1)
    #regex_single.append(regex_gidac)
    regex_single.append(regex_fw_ver)
    regex_single.append(regex_fw_rev)
    regex_single.append(regex_elapsedtime)
    regex_single.append(regex_vcom)
    regex_single.append(regex_vaux)
    regex_single.append(regex_icom)
    regex_single.append(regex_iaux)

    # Regex for list value
    regex_dict = dict()
    regex_gidac = re.compile("\sGlobal\siDAC,\s+ROW\d{2},(?P<Global_IDAC>(?:\s{2,4}\d{1,3},)*\s{2,4}\d{1,3})", re.UNICODE)
    regex_lidac = re.compile("\sLocal\sPWC,\s+ROW\d{2},(?P<Local_IDAC>(?:\s{2,4}\d{1,2},)*\s{2,4}\d{1,2})", re.UNICODE)
    regex_noise = re.compile("\sNoise,\s+ROW\d{2},(?P<Noise>(?:\s{2,4}\d{1,2},)*\s{2,4}\d{1,2})", re.UNICODE)
    regex_rawdata = re.compile("\sRaw\sData,\s+ROW\d{2},(?P<RawData>(?:\s{0,4}\-?\d{1,5},)*\s{0,4}\-?\d{1,5})", re.UNICODE)
    #regex_baseline = re.compile("\sBaseline,\s+ROW\d{2},(?P<Baseline>(?:\s{2,4}\-?\d{1,3},)*\s{2,4}\-?\d{1,3})", re.UNICODE)
    regex_signal = re.compile("\sSignal,\s+ROW\d{2},(?P<Signal>(?:\s{2,4}\-?\d{1,3},)*\s{2,4}\-?\d{1,3})", re.UNICODE)
    regex_selfcapnoise = re.compile("\sSelf-cap\sNoise,\s(?:COLS|ROWS),(?P<SelfCapNoise>(?:\s{2,4}\d{1,3},)*\s{2,4}\d{1,3})", re.UNICODE)
    regex_selfcaplidac = re.compile("\sSelf-cap\sLocal\sPWC,\s(?:COLS|ROWS),(?P<SelfCaplidac>(?:\s{2,4}\d{1,3},)*\s{2,4}\d{1,3})", re.UNICODE)
    regex_selfcapgidac = re.compile("\sSelf-cap\sGlobal\siDAC,\s(?:Self\sRx|Self\sTx),(?P<SelfCapgidac>\s{1,4}\d{1,3})", re.UNICODE)
    regex_selfcaprawdata = re.compile("\sSelf-cap\sRaw\sData,\s(?:COLS|ROWS),(?P<SelfCapRawData>(?:\s{0,4}\-?\d{1,4},)*\s{0,4}\-?\d{1,4})", re.UNICODE)
    regex_selfcapbaseline = re.compile("\sSelf-cap\sBaseline,\s(?:COLS|ROWS),(?P<SelfCapBaseLine>(?:\s{2,4}\-?\d{1,3},)*\s{2,4}\-?\d{1,3})", re.UNICODE)
    regex_selfcapsignal = re.compile("\sSelf-cap\sSignal,\s(?:COLS|ROWS),(?P<SelfCapSignal>(?:\s{2,4}\-?\d{1,3},)*\s{2,4}\-?\d{1,3})", re.UNICODE)

    regex_dict.update({"Global_IDAC": regex_gidac})
    regex_dict.update({"Local_IDAC": regex_lidac})
    regex_dict.update({"Noise": regex_noise})
    regex_dict.update({"RawData": regex_rawdata})
    #regex_dict.update({"Baseline": regex_baseline})
    regex_dict.update({"Signal": regex_signal})
    regex_dict.update({"SelfCapNoise": regex_selfcapnoise})
    regex_dict.update({"SelfCapRawData": regex_selfcaprawdata})
    regex_dict.update({"SelfCaplidac": regex_selfcaplidac})
    regex_dict.update({"SelfCapgidac": regex_selfcapgidac})
    regex_dict.update({"SelfCapBaseline": regex_selfcapbaseline})
    regex_dict.update({"SelfCapSignal": regex_selfcapsignal})

    def __init__(self, test_log_path):
        if not os.path.exists(test_log_path):
            raise Exception(test_log_path + " path doesn't exsist.")
        else:
            self.log_path = test_log_path

    def __get_files(self):
        #file_list = []
        for root, dirs, files in os.walk(self.log_path):
            for f in files:
                (filename, ext) = os.path.splitext(f)
                if ext == ".csv" or ext == ".CSV":
                    #file_list.append(os.path.join(root, f))
                    yield os.path.join(root, f)

    def __get_list_from_pattern(self, regex_pattern, source_string):
        returnLists = []
        tmpList = regex_pattern.findall(source_string)

        for item in tmpList:
            stringList = item.split(",")

            tmpList = []
            for i in stringList:
                tmpList.append(int(i))
            returnLists.append(tmpList)
        return returnLists

    def get_datas(self):
        #dut_results = []
        for file_path in self.__get_files():
            log_file = open(file_path, "r+")
            log_string = ''

            for line in log_file.readlines():
                log_string += line.strip() + "kaka"     # remove the \r\n of line
                #print log_string

            log_file.close()

            headerlist = CaptureData.regex_head.findall(log_string)
            #print headerlist
            contentlist = CaptureData.regex_content.findall(log_string)
            #print contentlist

            for i in range(0, len(headerlist)):
                log_header = headerlist[i]
                #print log_header
                log_engineer = contentlist[i]

                # search summary info
                r = CaptureData.regex_summary.search(log_header)
                if r:
                    #print r
                    result = r.groupdict()
                                        

                # search for all single value
                for my_regex in CaptureData.regex_single:
                    r = my_regex.search(log_engineer)
                    if r:                        
                        result.update(r.groupdict())
                        

                # search for all list value
                for key, r in CaptureData.regex_dict.items():
                    dataLists = self.__get_list_from_pattern(r, log_engineer)
                    #print key
                    #print dataLists
                    if dataLists:                        
                        result.update({key: dataLists})
                yield result


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
        plt.figure(self.picture_number, figsize=(8, 4))    # Create a new figure
        plt.hist(y, bins=50, range=None, normed=False)
        plt.title(data_name + " Histogram Plot")     # Give the figure a title
        plt.xlabel('value')          # Label the x-axis
        plt.ylabel('count')          # Label the y-axis
        plt.grid(True)               # Turn the grid on
        plt.savefig(self.path + data_name + '_hist.pdf')
        plt.close()

    def draw_hist_no_zero(self, data_list, data_name):
        self.picture_number += 1
        y = np.array(data_list, dtype=float)
        y = y.ravel()
        #y, = np.nonzero(y)
        y = y[y>8]
        print y
        plt.figure(self.picture_number, figsize=(8, 4))    # Create a new figure
        plt.hist(y, bins=50, range=None, normed=False)
        plt.title(data_name + " Histogram Plot")     # Give the figure a title
        plt.xlabel('value')          # Label the x-axis
        plt.ylabel('count')          # Label the y-axis
        plt.grid(True)               # Turn the grid on
        plt.savefig(self.path + data_name + '_hist.pdf')
        plt.close()

    def draw_runchart(self, data_list, data_name):
        self.picture_number += 1
        data_array = np.array(data_list, dtype=int)       # 2D Matrix, duts * sensors
        std_data = np.std(data_array, axis=0)             # caculate stdev of data, output 1D Matrix, rows + columns
        #print data_array

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

    def draw_3Dplot(self, data_list, data_name):
        self.picture_number += 1
        data_array = np.array(data_list, dtype=int)     # 3D Matrix, duts * rows * columns
        std_data = np.std(data_array, axis=0)           # caculate stdev of duts, output 2D Matrix, rows * columns

        x_arange, y_arange = std_data.shape
        x, y = np.mgrid[:x_arange, :y_arange]

        plt.figure(self.picture_number, figsize=(8, 4))
        plt.title(data_name + " StdDev Surface")
        ax = plt.subplot(111, projection='3d')  # 111 means 1 row 1 column and the No.1 subplot
        ax.plot_surface(x, y, std_data, rstride=1, cstride=1, cmap=cm.coolwarm)
        ax.set_xlabel("sensor_x")
        ax.set_ylabel("sensor_y")
        ax.set_zlabel(data_name + " StdDev")
        plt.savefig(self.path + data_name + '_stdDev_3D.pdf')
        plt.close()



def write_list_to_file(dut_results, file_path, data_name):

    csvfile = open(file_path + data_name + ".csv", 'wb')
    
    csvwriter = csv.writer(csvfile, delimiter=',', quotechar='/', quoting=csv.QUOTE_MINIMAL)
    tempList=[]
    dut = []
    data = []
    tt = []
    #print type(dut_results)
    #write content
    #print dut_results.__len__()
        
    for dut in dut_results:
        if type(dut) is types.ListType:        
            tempList = []
            for data in dut:                
                if type(data) is types.ListType:
                    #tempList = []
                    for tt in data:
                        tempList.append(tt)
                    #csvwriter.writerow(tempList)
                    #tempList = [] 
                else:                    
                    tempList.append(data)
            if len(tempList) > 0:
                csvwriter.writerow(tempList)
                tempList = []
        else:            
            tempList.append(dut)
    if len(tempList) > 0:
        #print len(tempList)
        csvwriter.writerow(tempList)        

    csvfile.close()

def main():
    gidac = []
    icom = []
    

    noise_lists = []
    baseline_lists = []
    lidac_lists = []
    rawdata_lists = []
    #signal_lists = []
    #snr_lists = []

    SelfCapNoise_lists = []
    SelfCapRawData_lists = []
    SelfCapBaseline_lists = []
    SelfCaplidac_lists = []
    SelfCapgidac_lists = []

    capture_data = CaptureData(TEST_LOG_PATH)
    #capture_signal = CaptureSignalData(SIGNAL_LOG_PATH)
    
    for dut in capture_data.get_datas():
        #print dut.keys()
        if dut["TestResult"] == TEST_RESULT or TEST_RESULT == "ALL":
            if("Global_IDAC" in dut.keys()):
                gidac.append(dut["Global_IDAC"])
            if("ICOMCurrent" in dut.keys()):
                icom.append(dut["ICOMCurrent"])
            if("Noise" in dut.keys()):                
                noise_lists.append(dut["Noise"])
            #if("Baseline" in dut.keys()):
                #baseline_lists.append(dut["Baseline"])
            if("Local_IDAC" in dut.keys()):
                lidac_lists.append(dut["Local_IDAC"])
            if("RawData" in dut.keys()):                
                rawdata_lists.append(dut["RawData"])
            if("SelfCapNoise" in dut.keys()):
                tempList = []
                for tmp in dut["SelfCapNoise"]:
                    tempList += tmp
                SelfCapNoise_lists.append(tempList)
            if("SelfCapRawData" in dut.keys()):
                tempList = []
                for tmp in dut["SelfCapRawData"]:
                    tempList += tmp
                SelfCapRawData_lists.append(tempList)
            if("SelfCaplidac" in dut.keys()):
                tempList = []
                for tmp in dut["SelfCaplidac"]:
                    tempList += tmp
                SelfCaplidac_lists.append(tempList)
            if("SelfCapgidac" in dut.keys()):
                tempList = []
                for tmp in dut["SelfCapgidac"]:
                    tempList += tmp
                SelfCapgidac_lists.append(tempList)
            #if("SelfCapBaseline" in dut.keys()):
             #   tempList = []
              #  for tmp in dut["SelfCapBaseline"]:
               #     tempList += tmp
                #SelfCapBaseline_lists.append(tempList)

            #signal_temp_list = capture_signal.get_lists(dut["SerialNumber"])
            #signal_lists.append(signal_temp_list)

            #snr_temp_list = []
            #temp_list = zip(signal_temp_list, noise_temp_list)
            #for i in range(len(temp_list)):
            #    signal, noise = temp_list[i]
            #    if noise == 0:
            #        noise = 1
            #    snr = signal / noise
            #    snr_temp_list.append(snr)
            #print snr_temp_list
            #snr_lists.append(snr_temp_list)

    draw_plot = DrawPlot(RESULT_LOG_PATH)
    write_list_to_file(gidac, RESULT_LOG_PATH, "Global_IDAC")
    write_list_to_file(lidac_lists, RESULT_LOG_PATH, "Local_IDAC")
    write_list_to_file(icom, RESULT_LOG_PATH, "ICOM")
    write_list_to_file(noise_lists, RESULT_LOG_PATH, "NOISE")
    write_list_to_file(SelfCapNoise_lists, RESULT_LOG_PATH, "SelfCapNoise")
    write_list_to_file(SelfCapRawData_lists, RESULT_LOG_PATH, "SelfCapRawData")
    write_list_to_file(SelfCaplidac_lists, RESULT_LOG_PATH, "SelfCapLocalIDAC")
    write_list_to_file(SelfCapgidac_lists, RESULT_LOG_PATH, "SelfCapGlobalIDAC")
    write_list_to_file(rawdata_lists, RESULT_LOG_PATH, "RAWDATA")
    #print SelfCapRawData_lists
    draw_plot.draw_hist(gidac, "Global_IDAC")
    draw_plot.draw_hist(lidac_lists, "Local_IDAC")
    draw_plot.draw_hist(icom, "ICOM")
    draw_plot.draw_hist(noise_lists, "NOISE")    
    #draw_plot.draw_hist(rawdata_lists, "RAWDATA")
    draw_plot.draw_hist(SelfCapNoise_lists, "SelfCapNoise")
    draw_plot.draw_hist(SelfCapRawData_lists, "SelfCapRawData")    
    draw_plot.draw_hist(SelfCaplidac_lists, "SelfCapLocalIDAC")
    draw_plot.draw_hist(SelfCapgidac_lists, "SelfCapGlobalIDAC")

    #draw_plot.draw_hist_no_zero(noise_lists, "NOISE_OutOfSpec")
    #draw_plot.draw_hist_no_zero(SelfCapNoise_lists, "SelfCapNoise_OutOfSpec")

    #draw_plot.draw_3Dplot(noise_lists, "NOISE")
    #draw_plot.draw_3Dplot(baseline_lists, "BaseLine")
    #draw_plot.draw_3Dplot(lidac_lists, "LocalIDAC")
    #draw_plot.draw_3Dplot(rawdata_lists, "RAWDATA")

    #draw_plot.draw_runchart(SelfCapNoise_lists, "SelfCapNoise")
    #draw_plot.draw_runchart(SelfCapRawData_lists, "SelfCapRawData")
    #draw_plot.draw_runchart(SelfCapBaseline_lists, "SelfCapBaseline")

    #draw_plot.draw_runchart(SelfCapNoise_lists, "SelfCapNoise")
    #draw_plot.draw_runchart(SelfCapRawData_lists, "SelfCapRawData")
    #draw_plot.draw_runchart(SelfCapBaseline_lists, "SelfCapBaseline")
    #draw_plot.draw_hist(signal_lists, "SIGNAL")
    #draw_plot.draw_hist(snr_lists, "SNR")
    print("Finish.")

if __name__ == "__main__":
    main()
