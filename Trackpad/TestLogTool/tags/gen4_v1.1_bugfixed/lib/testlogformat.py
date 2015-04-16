import re

class TestLogFormat:
	
	def change_D_to_data(self, file_path):
		log_file=open(file_path, "r+")	#open file for read and write
		log_string=log_file.read()
		
		regex_m=re.compile(r"<D\d+>")
		log_string=regex_m.sub('<data>', log_string)
		
		regex_n=re.compile(r"</D\d+>")
		log_string=regex_n.sub('</data>', log_string)

		log_file.seek(0)			#point to the head of file
		log_file.write(log_string)  #write the string
		log_file.close()
		
	def change_serialnumber_to_string(self, file_path):
		log_file=open(file_path, "r")	#open file for read
		log_string=log_file.read()		
		
		regex = re.compile(r"<Serial_Number>(?P<SerialNumber>\w{19})</Serial_Number>")
		r = regex.search(log_string)
		
		log_file.close()
		
		if r is not None:
			serialnumber="<Serial_Number>'"+ r.group(1)+"'</Serial_Number>"	# group(1) is the second match.
			log_string=regex.sub(serialnumber, log_string)	#replace with the ' '
			
			log_file=open(file_path, "w")	#open file for write
			log_file.truncate()
			log_file.seek(0)			#point to the head of file
			log_file.write(log_string)  #write the string
		
			
	def change_time_format(self, file_path):
		log_file=open(file_path, "r") # open the read of file
		log_string=log_file.read()
		
		regex=re.compile(r"<Test_Time>(?P<TestTime>.{20})</Test_Time>")
		r=regex.search(log_string)
		
		log_file.close()	# close the read of file
		
		if r is not None:
			testtime="<Test_Time>" + r.group(1)[:-1] + "</Test_Time>"  # cut the last Z
			
			log_string = regex.sub(testtime, log_string)

			log_file=open(file_path, "w")	# open the write of file
			log_file.truncate()			
			log_file.seek(0)			#point to the head of file
			log_file.write(log_string)  #write the string
								
		
		 