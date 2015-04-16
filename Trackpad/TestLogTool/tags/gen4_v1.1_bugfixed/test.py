
import numpy as np
import matplotlib.pyplot as plt
import math

#draw ICOM distribution
y = np.array([1,2,3,4,5,6,5,4,5,7,5,7,9,4,5,4,3,5,5,6,4,4])
plt.hist(y, bins=50, range=None, normed=True)
plt.title("ICOM value")
plt.grid(True)
plt.savefig('./ICOM_Distribution.pdf')
plt.show()



print len(y)
bin=1+3.332*(math.log10(len(y)))
print bin