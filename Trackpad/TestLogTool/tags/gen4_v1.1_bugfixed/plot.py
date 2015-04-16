import numpy as np
import pylab
#from pylab import plot, figure, show, title, xlabel, ylabel, grid, savefig

x = np.arange(1,10,.1);
y = x ** 3;

pylab.figure(1);    # Create a new figure (just like MATLAB)
pylab.plot(x,y,'b-');   # Plot the results (takes the same arguments as MATLAB)
pylab.title('Simple Parabola'); # Give the figure a title
pylab.xlabel('x');  # Label the x-axis
pylab.ylabel('y');  # Label the y-axis
pylab.grid(True);   # Turn the grid on
pylab.savefig('./Plot.pdf') # I saved as a PDF because I like vector graphics
pylab.show();
