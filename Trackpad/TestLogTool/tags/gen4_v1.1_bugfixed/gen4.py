#===============================================================
#
#	name:		gen4.py 
#	purpose:	this script is used to read the raw gen4 test
#				logs and convert to the summary csv file.  
#	author:		qibo
#	date:		2013-1-17		
#
#===============================================================


import os
import sys
import csv
import re
import math
import numpy as np
import matplotlib.pyplot as plt

#===============================================================
# Global Test Result
#===============================================================
TEST_RESULT="PASS"		#only look for pass logs 
#TEST_RESULT="FAIL"


#===============================================================
# Global File Paths
#===============================================================
TEST_LOG_PATH = "./test_logs/"
RESULT_LOG_PATH = TEST_LOG_PATH + "/" + TEST_RESULT + "/"


CSV_DUT_PATH = RESULT_LOG_PATH + "dut.csv"
CSV_RAWDATA_PATH = RESULT_LOG_PATH + "dut_rawdata.csv"
CSV_NOISE_PATH = RESULT_LOG_PATH + "dut_noise.csv"
CSV_LOCALIDAC_PATH = RESULT_LOG_PATH + "dut_localidac.csv"
CSV_BASELINE_PATH = RESULT_LOG_PATH + "dut_baseline.csv"
CSV_SIGNAL_PATH = RESULT_LOG_PATH + "dut_signal.csv"
CSV_SELFCAPNOISE_PATH = RESULT_LOG_PATH + "dut_selfcapnoise.csv"
CSV_SELFCAPRAWDATA_PATH = RESULT_LOG_PATH + "dut_selfcaprawdata.csv"
CSV_SELFCAPBASELINE_PATH = RESULT_LOG_PATH + "dut_selfcapbaseline.csv"
CSV_SELFCAPSIGNAL_PATH = RESULT_LOG_PATH + "dut_selfcapsignal.csv"


#===============================================================
# Global Regular Expression Pattern
#===============================================================
regex_head = re.compile(r",\s\.header(.*?),\s\.end", re.UNICODE)
regex_content = re.compile(r",\s\.engineering\sdata(.*?),\s\.end", re.UNICODE)

regex_summary=re.compile(r",\sDATE,\s(?P<TestDate>\d{4}(?:\-|\/|\.)\d{1,2}(?:\-|\/|\.)\d{1,2}),\sTIME,\s(?P<TestTime>\d{1,2}:\d{1,2}:\d{1,2})\s*,\sSW\sVERSION,\s(?P<SWVersion>(?:\d{1,2}.){3}\d{1,4}),\sOPERATOR,\s(?P<Operator>\d{1,5}),\sTEST\sSTATION,\s(?P<TestStaion>\b\w*\b),\sTEST\sFACILITY,\s(?P<TestFacility>\b\w*\b)\s*,\sCONFIG\sFILE,\s(?P<ConfigFile>[^,]*),\sEXECUTION\sMODE,\s(?P<TestMode>\b\w*\b),\sSENSOR\sROWS,\s(?P<RowNumber>\d{1,2}),\sSENSOR\sCOLUMNS,\s(?P<ColumnNumber>\d{1,2}),\s(?P<TestResult>\b\w*\b)", re.UNICODE)
regex_engineerdataline1 = re.compile(r"\s(?P<SerialNumber>\w{19}),\s(?P<ChipID>[^,]*),\s(?:ERRORS,\s)?(?P<ErrorCode>[^:]*):\s(?P<ErrorMessage>[^,]*)", re.UNICODE)
regex_gidac = re.compile(r"\sSensor\sGlobal\siDAC,\s+(?P<Global_IDAC>\d{1,3}),", re.UNICODE)
regex_fw_ver = re.compile(r"\sFW\sVersion,([^,]*,){4}\sVersion,\s(?P<FW_Version>\d{1,2}.\d{1,2})", re.UNICODE)
regex_fw_rev = re.compile(r"\sFW\sRevision\sControl,([^,]*,){4}\sRevision,\s(?P<FW_Revision>[^,]*)", re.UNICODE)
regex_elapsedtime = re.compile("\sELAPSED\sTIME,\s(?P<ElapsedTime>\d+\.?\d?)", re.UNICODE)

regex_vcom = re.compile("\sVCOM\sVoltage,(?:[^,]*,){5}\s(?P<VCOMVoltage>\d{1,2}.\d{1,2})", re.UNICODE)
regex_vaux = re.compile("\sVAUX\sVoltage,(?:[^,]*,){5}\s(?P<VAUXVoltage>\d{1,2}.\d{1,2})", re.UNICODE)
regex_icom = re.compile("\sICOM\sCurrent,(?:[^,]*,){5}\s(?P<ICOMCurrent>\d{1,3}.\d{1,2})", re.UNICODE)
regex_iaux = re.compile("\sIAUX\sCurrent,(?:[^,]*,){5}\s(?P<IAUXCurrent>\d{1,3}.\d{1,2})", re.UNICODE)


regex_lidac = re.compile("\sLocal\siDAC,\s+ROW\d{2},(?P<Local_IDAC>(?:\s{3,4}\d{1,3},)*\s{3,4}\d{1,3})", re.UNICODE)
regex_noise = re.compile("\sNoise,\s+ROW\d{2},(?P<Noise>(?:\s{3,4}\d{1,3},)*\s{3,4}\d{1,3})", re.UNICODE)
regex_rawdata = re.compile("\sRaw\sData,\s+ROW\d{2},(?P<RawData>(?:\s{2,4}\-?\d{1,3},)*\s{2,4}\-?\d{1,3})", re.UNICODE)
regex_baseline = re.compile("\sBaseline,\s+ROW\d{2},(?P<Baseline>(?:\s{2,4}\-?\d{1,3},)*\s{2,4}\-?\d{1,3})", re.UNICODE)
regex_signal = re.compile("\sSignal,\s+ROW\d{2},(?P<Signal>(?:\s{2,4}\-?\d{1,3},)*\s{2,4}\-?\d{1,3})", re.UNICODE)

regex_selfcapnoise = re.compile("\sSelf-cap\sNoise,\s(?:COLS|ROWS),(?P<SelfCapNoise>(?:\s{2,4}\d{1,3},)*\s{2,4}\d{1,3})", re.UNICODE)
regex_selfcaprawdata = re.compile("\sSelf-cap\sRaw\sData,\s(?:COLS|ROWS),(?P<SelfCapRawData>(?:\s{2,4}\-?\d{1,3},)*\s{2,4}\-?\d{1,3})", re.UNICODE)
regex_selfcapbaseline = re.compile("\sSelf-cap\sBaseline,\s(?:COLS|ROWS),(?P<SelfCapBaseLine>(?:\s{2,4}\-?\d{1,3},)*\s{2,4}\-?\d{1,3})", re.UNICODE)
regex_selfcapsignal= re.compile("\sSelf-cap\sSignal,\s(?:COLS|ROWS),(?P<SelfCapSignal>(?:\s{2,4}\-?\d{1,3},)*\s{2,4}\-?\d{1,3})", re.UNICODE)


def draw_pdfPlots():
	global dut_results
	
	#Gidac
	gidac_list = []
	icom_list = []
	
	for dut in dut_results:
		
		if("Global_IDAC" in dut.keys()):
			gidac_list.append(dut["Global_IDAC"])
		
		if("ICOMCurrent" in dut.keys()):
			icom_list.append(dut["ICOMCurrent"])
	
	#draw Global IDAC dot Plot
	x = np.arange(len(gidac_list))
	y = np.array(gidac_list, dtype=float)
	
	plt.figure(1);    # Create a new figure (just like MATLAB)
	plt.plot(x,y,'ro');   # Plot the results (takes the same arguments as MATLAB)
	plt.title("Global IDAC"); # Give the figure a title
	plt.xlabel('sensor');  # Label the x-axis
	plt.ylabel('value');  # Label the y-axis
	plt.grid(True);   # Turn the grid on
	plt.savefig('./Gidac_Plot.pdf') # I saved as a PDF because I like vector graphics
	plt.show();

	#draw ICOM distribution
	y = np.array(icom_list, dtype=float)
	plt.hist(y, bins=50, range=None, normed=False)
	plt.title("ICOM value")
	plt.grid(True)
	plt.savefig('./ICOM_Distribution.pdf')
	plt.show()

def write_list_to_file(file_path, data_name, selfcap):
	global dut_results
	
	csvfile = open(file_path, 'wb')
	csvwriter = csv.writer(csvfile, delimiter=',', quotechar='|', quoting=csv.QUOTE_MINIMAL)
	
	
	#write head
	headerList=[]
	for dut in dut_results:

		if selfcap :
			sensor_number = int(dut["RowNumber"]) + int(dut["ColumnNumber"])
		else :
			sensor_number = int(dut["RowNumber"]) * int(dut["ColumnNumber"])
		for i in range(sensor_number):
			headerList.append(i+1)
		
		break
			
	csvwriter.writerow(["SerialNumber","Statistic"]+headerList)
	
	#write content
	for dut in dut_results:

		if dut["TestResult"]==TEST_RESULT:
			tempList=[dut["SerialNumber"],data_name,]
			
			for list in dut[data_name]:
				tempList +=list
	
			csvwriter.writerow(tempList)
			#print tempList	
	
	csvfile.close()


def write_CSVfiles():
	global dut_resutls
	
	try:
		
		#--------------------------
		# write main information
		#--------------------------
		csvfile = open(CSV_DUT_PATH, 'wb')
		csvwriter = csv.writer(csvfile, delimiter=',', quotechar='|', quoting=csv.QUOTE_MINIMAL)
		
		# write csv header
		csvwriter.writerow(["SerialNumber", "TestStaion", "TestResult","ErrorCode","ErrorMessage", "FW_Version", "FW_Revision", "RowNumber", "ColumnNumber", "Global_IDAC", "ChipID", "VCOMVoltage", "VAUXVoltage", "ICOMCurrent", "IAUXCurrent", "TestFacility", "TestMode", "TestDate", "TestTime", "SWVersion", "ElapsedTime", "Operator", "ConfigFile"])
		
		for dut in dut_results:
			
			dut["SerialNumber"]="'"+dut["SerialNumber"]+"'"
			
			if dut["TestResult"]==TEST_RESULT:
				csvwriter.writerow([dut["SerialNumber"], dut["TestStaion"], dut["TestResult"], dut["ErrorCode"], dut["ErrorMessage"], dut["FW_Version"], dut["FW_Revision"], dut["RowNumber"], dut["ColumnNumber"], dut["Global_IDAC"], dut["ChipID"], dut["VCOMVoltage"], dut["VAUXVoltage"], dut["ICOMCurrent"], dut["IAUXCurrent"], dut["TestFacility"], dut["TestMode"], dut["TestDate"], dut["TestTime"], dut["SWVersion"], dut["ElapsedTime"], dut["Operator"], dut["ConfigFile"]])
				#print dut["SerialNumber"]
			
		csvfile.close()
		
		#--------------------------
		# write list data information
		#--------------------------
		write_list_to_file(CSV_RAWDATA_PATH, "RawData", False)
		write_list_to_file(CSV_NOISE_PATH, "Noise", False)
		write_list_to_file(CSV_LOCALIDAC_PATH, "Local_IDAC", False)
		write_list_to_file(CSV_BASELINE_PATH, "Baseline", False)
		write_list_to_file(CSV_SIGNAL_PATH, "Signal", False)
		write_list_to_file(CSV_SELFCAPNOISE_PATH, "SelfCapNoise", True)
		write_list_to_file(CSV_SELFCAPRAWDATA_PATH, "SelfCapRawData", True)
		write_list_to_file(CSV_SELFCAPBASELINE_PATH, "SelfCapBaseLine", True)
		write_list_to_file(CSV_SELFCAPSIGNAL_PATH, "SelfCapSignal", True)
		
	except Exception as e:
		print e

def get_list_from_pattern(regex_pattern, source_string):
	
	returnLists=[]
	
	tmpList = regex_pattern.findall(source_string)
	
	for item in tmpList:
		stringList=item.split(",")
		
		tmpList=[]
		for i in stringList:
			tmpList.append(int(i))
		returnLists.append(tmpList)
	
	return returnLists

def read_files():
	global file_paths
	global dut_results

	dut_results=[]
	
	try:
		for file_path in file_paths:
			
			log_file=open(file_path, "r+")
			log_string=''
			
			for line in log_file.readlines():
				log_string += line.strip() #remove the \r\n of line
			
			log_file.close()
			#print log_string
			
			headerlist=regex_head.findall(log_string)
			contentlist=regex_content.findall(log_string)

			for i in range(0, len(headerlist)):
				log_header = headerlist[i]
				log_engineer = contentlist[i]
				
				#print log_engineer
				
				#search summary info
				r = regex_summary.search(log_header)
				if r : result = r.groupdict()

				#search Engineering data line1
				r = regex_engineerdataline1.search(log_engineer)
				if r : result.update(r.groupdict())

				#search Global IDAC
				r = regex_gidac.search(log_engineer)
				if r : result.update(r.groupdict())

				#search FW version and revision
				r = regex_fw_ver.search(log_engineer)
				if r : result.update(r.groupdict())
				r = regex_fw_rev.search(log_engineer)
				if r : result.update(r.groupdict())
				
				#search voltage and current
				r = regex_vcom.search(log_engineer)
				if r : result.update(r.groupdict())
				r = regex_icom.search(log_engineer)
				if r : result.update(r.groupdict())
				r = regex_vaux.search(log_engineer)
				if r : result.update(r.groupdict())
				r = regex_iaux.search(log_engineer)
				if r : result.update(r.groupdict())
				
				#search elapsed time
				r = regex_elapsedtime.search(log_engineer)
				if r : result.update(r.groupdict())
				
				#search local IDAC
				lidacLists = get_list_from_pattern(regex_lidac, log_engineer)
				if r : result.update({"Local_IDAC": lidacLists})
				
				#search noise
				noiseLists = get_list_from_pattern(regex_noise, log_engineer)
				if r : result.update({"Noise": noiseLists})
				
				#search rawdata
				rawdataLists = get_list_from_pattern(regex_rawdata, log_engineer)
				if r : result.update({"RawData": rawdataLists})
				
				#search baseline
				baselineLists = get_list_from_pattern(regex_baseline, log_engineer)
				if r : result.update({"Baseline": baselineLists})
				
				#search signal
				signalLists = get_list_from_pattern(regex_signal, log_engineer)
				if r : result.update({"Signal": signalLists})
				
				#search selfcap noise
				selfcapnoiseLists = get_list_from_pattern(regex_selfcapnoise, log_engineer)
				if r : result.update({"SelfCapNoise": selfcapnoiseLists})
				
				#search selfcap rawdata
				selfcaprawdataLists = get_list_from_pattern(regex_selfcaprawdata, log_engineer)
				if r : result.update({"SelfCapRawData": selfcaprawdataLists})
				
				#search selfcap baseline
				selfcapbaselineLists = get_list_from_pattern(regex_selfcapbaseline, log_engineer)
				if r : result.update({"SelfCapBaseLine": selfcapbaselineLists})
				
				#search selfcap signal
				selfcapsignalLists = get_list_from_pattern(regex_selfcapsignal, log_engineer)
				if r : result.update({"SelfCapSignal": selfcapsignalLists})

				dut_results.append(result)
				
#				for key in result:
#					print key, result[key]
#
#				print "\n"

	except Exception as e:
		print e
		print file_path
	
	finally:
		print "duts:" + str(len(dut_results))
		#return dut_results
		

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

def check_paths():
	
	if not os.path.exists(RESULT_LOG_PATH):
		os.makedirs(RESULT_LOG_PATH)


if __name__ == "__main__":
	check_paths()
	get_files()
	read_files()
	write_CSVfiles()
	#draw_pdfPlots()
	