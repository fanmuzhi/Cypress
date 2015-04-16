#!/usr/bin/env python
'''
This module implements a basic grid-drawing library. The class implemented in 
this file implements a TPA-style 2D grid for representing touchscreen data.
The low-level drawing functions were done with PyGame, but have been abstracted 
so the high level user does not need to be aware of specific drawing features.
'''

# Imports
import pygame
from pygame.locals import *
import numpy


#-------------------------------------------------------------------------------
#                              color Class
#-------------------------------------------------------------------------------
class color(object):
    def __init__(self, red=0, green=0, blue=0, alpha=255):       
        self.red = red
        self.green = green
        self.blue = blue
        self.alpha = alpha
        self.rgb = (red, green, blue)
        self.rgba = (red, green, blue, alpha)
        return

black = color(red=0, green=0, blue=0)
white = color(red=255, green=255, blue=255)


#-------------------------------------------------------------------------------
#                              GridFigure Class
#-------------------------------------------------------------------------------
class GridFigure(object):
    
    def __init__(self, title='DrawGrid', size=(640,480), renderText=True, flipX=False, flipY=False, colorMode='hotcold', showGrid=True):
        ''' Initialization method for the DrawGrid class. '''

        # Initialize class variables
        self.title = title
        self.size = size
        self.renderText = renderText
        self.SetGridOrientation(flipX, flipY)
        self.colorMode = colorMode
        self.showGrid=showGrid
        
        self.hotkeyCallbacks = []
        self.running = True
        self.lastFrame = None
        
        # Initialize pygame
        pygame.init()
        pygame.key.set_repeat()  # Disable key-repeating
        
        # Set the window caption
        pygame.display.set_caption(self.title)
        
        # Hide the mouse
        pygame.mouse.set_visible(0)
        
        # Create a reference to the screen
        self.screen = pygame.display.set_mode(self.size, (RESIZABLE))
        
        # Create a frame to draw to
        self.frame = pygame.Surface(self.screen.get_size(), pygame.SRCALPHA)
        
        # Add the ability to quit pygame
        self.AddHotkeyCallback(K_ESCAPE, self._quitCallback)
        self.AddHotkeyCallback(K_q, self._quitCallback)
        
        return
        
    def _quitCallback(self):
        self.running = False
        return
        
    def SetGridOrientation(self, flipX=None, flipY=None):
        if flipX is not None:
            self.flipX = flipX
        if flipY is not None:
            self.flipY = flipY
        return


    def _WavelengthToRGB(self, wl):
        ''' A helper method to convert a wavelength into an RGB value. '''
        #http://codingmess.blogspot.com/2009/05/conversion-of-wavelength-in-nanometers.html
    
        w = float(numpy.uint16(wl));
        
        # colour
        if w >= 380 and w < 440:
            R = -(w - 440.) / (440. - 350.);
            G = 0.0;
            B = 1.0;
        elif w >= 440 and w < 490:
            R = 0.0;
            G = (w - 440.) / (490. - 440.);
            B = 1.0;
        elif w >= 490 and w < 510:
            R = 0.0;
            G = 1.0;
            B = -(w - 510.) / (510. - 490.);
        elif w >= 510 and w < 580:
            R = (w - 510.) / (580. - 510.);
            G = 1.0;
            B = 0.0;
        elif w >= 580 and w < 645:
            R = 1.0;
            G = -(w - 645.) / (645. - 580.);
            B = 0.0;
        elif w >= 645 and w <= 780:
            R = 1.0;
            G = 0.0;
            B = 0.0;
        else:
            R = 0.0;
            G = 0.0;
            B = 0.0;
    
        # intensity correction
        if w >= 380 and w < 420:
            SSS = 0.3 + 0.7*(w - 350) / (420 - 350);
        elif w >= 420 and w <= 700:
            SSS = 1.0;
        elif w > 700 and w <= 780:
            SSS = 0.3 + 0.7*(780 - w) / (780 - 700);
        else:
            SSS = 0.0;
        
        SSS = SSS * 255;
        
        r = int(SSS*R);
        g = int(SSS*G);
        b = int(SSS*B);
           
        return (r,g,b)


    def _GetColor(self, value, colorRange='hotcold', maxVal=None, minVal=None):
        '''
            Method Name:    GetColor
            Arguments:      value - current count value
                            color range - string to define the colors
                                o 'hotcold' - only "hot" and "cold" values
                                o 'fullspectrum' - full color spectrum
                                o 'red' - red colors only
            Returns:        image - pygame image object drawn to.
            Description:    This function draws the 2D raw counts array. Typically,
                            this function will be used to draw raw counts, 
                            difference counts or baseline data.
        '''
                
        if colorRange == 'fullspectrum':
        
            # Settings
            minWavelength = 380
            maxWavelength = 780
            zeroWavelength = 550
            
            # Just map the wavelength linearly for now
            try:
                wlRange = maxWavelength - minWavelength
                valRange = maxVal - minVal
                valStep = float(wlRange) / float(valRange)
                wl = int((value-minVal) * valStep + minWavelength)
            except:
                wl = zeroWavelength
        
            # Convert wavelength to an RGB color
            (r,g,b) = self._WavelengthToRGB(wl)
        
        elif colorRange == 'hotcold':
            
            if (value == 0):
                r = 255
                g = 255
                b = 255
            else:
                # Define some functions for calculating r,g,and b
                def constrainColor(v):
                    if v < 0.:
                        return 0.
                    elif v > 255.:
                        return 255.
                    else:
                        return v
                def red(v):
                    if v < 256.:
                        #return v
                        return 0
                    else:
                        return 255
                def green(v):
                    if v < 256.:
                        return v
                    else:
                        return constrainColor(-v+512.)
                def blue(v):
                    if v < 256.:
                        return 255.
                    else:
                        return 0.
                        #return constrainColor(-v+512.)
                
                # Determine a value between 0 and 512
                if (maxVal != minVal):
                    value = (value-minVal) * 511. / (maxVal-minVal)
                else:
                    value = 0
                
                if value > 511:
                    value = 511
                elif value < 0:
                    value = 0
                            
                # Get the r,g,and b values
                r = int(red(value))
                g = int(green(value))
                b = int(blue(value))
            
        elif colorRange == 'red':
            try:
                r = int(float((value-minVal)*255.)/float(maxVal-minVal))
            except:
                r = 0    
            g = 0
            b = 0
        
        
        # Create a color object
        returnColor = color(red=r, green=g, blue=b)
        
        return returnColor


    def AddHotkeyCallback(self, key, callback):
        dict = {}
        dict['key'] = key
        dict['callback'] = callback
        self.hotkeyCallbacks.append(dict)
        return

    def _ProcessPyGameEvents(self):
        ''' Helper method for processing PyGame events. '''
        
        # Get all current PyGame Events
        allEvents = pygame.event.get()
        
        # Process all events
        for event in allEvents:
            if event.type == VIDEORESIZE:
                self.size = event.size
                self.screen = pygame.display.set_mode(event.size, (RESIZABLE))
                self.frame = pygame.Surface(self.screen.get_size(), pygame.SRCALPHA)
                
                if self.lastFrame is not None:
                    self.DrawGrid(self.lastFrame, self.lastMinVal, self.lastMaxVal)
            
            elif event.type == KEYDOWN:
                
                for key in self.hotkeyCallbacks:
                    if event.key == key['key']:
                        # Call the callback function
                        key['callback']()
                
            elif (event.type == QUIT):
                self.running = False    
        
        return

    def _ClearScreen(self, clearColor=black):
        ''' Helper class to clear the PyGame screen object. '''
        pygame.draw.rect(self.screen, black.rgb, (0, 0, self.size[0], self.size[1]), 0)
        return
        
    def show(self):
        '''
        This method is an infinite loop which shows the pygame screen and 
        handles pygame events until the window is closed.
        '''
        
        while self.running == True:
            self.showOnce()
        
        return

    def showOnce(self):
        '''
        This is a helper method which just handles the screen refresh one time.
        This method is useful when multiple frames are drawn sequentially.
        '''
        
        # Move the frame image over to the screen image
        self.screen.blit(self.frame, self.frame.get_rect())
        
        # Redraw the screen
        pygame.display.flip()
        
        # Handle pygame events
        self._ProcessPyGameEvents()

        return
        
    def save(self, outputFile='saveframe.png'):
        ''' This method saves the current figure to a file. '''
        pygame.image.save(self.screen, outputFile)
        return

    def DrawGrid(self, frame, minVal=None, maxVal=None):
        ''' This method draws a 2D numpy array of data. '''
        
        self.lastFrame = frame
        self.lastMinVal = minVal
        self.lastMaxVal = maxVal
        
        # Get the shape of the frame
        [numR, numC] = numpy.shape(frame)
        
        # See if max and min are valid
        if minVal is None:
            minVal = numpy.min(numpy.min(frame))
        if maxVal is None:
            maxVal = numpy.max(numpy.max(frame))
        
        # Compute the box size
        boxWidth = self.size[0] / numC
        boxHeight = self.size[1] / numR
        
        # Setup font rendering
        if self.renderText == True:
            fontSize = int(boxWidth/3.0)
            fontColor = white
            font = pygame.font.Font(None, fontSize)
            font.set_bold(True)
            (w,h) = font.size('8')
            fontSize = int(fontSize * (float(boxWidth) / (5.*float(w)) ) )       
            font = pygame.font.Font(None, fontSize)
            font.set_bold(True)
        
        # Loop through each value
        for r in range(0, numR):
            for c in range(0, numC):
                # Get the value
                counts = frame[r,c]
                
                # Determine the color based on the count value
                boxColor = self._GetColor(counts, self.colorMode, maxVal, minVal)               
                
                # Get the box position
                if self.flipX == True:
                    x1 = (numC-1-c) * boxWidth
                else:
                    x1 = c * boxWidth
                x2 = x1 + boxWidth
                
                if self.flipY == True:
                    y1 = (numR-1-r) * boxHeight
                else:
                    y1 = r * boxHeight
                y2 = y1 + boxHeight
                                    
                # Draw the box
                pygame.draw.rect(self.frame, boxColor.rgb, (x1,y1,boxWidth,boxHeight), 0)
                if self.showGrid == True:
                    pygame.draw.rect(self.frame, black.rgb, (x1,y1,boxWidth,boxHeight), 1)
    
                # Draw the counts value in the box
                if self.renderText == True:
                    text = font.render('%d' % counts, True, fontColor.rgba, boxColor.rgb)  # decimal
                    #text = font.render('%04x' % counts, True, fontColor, boxColor)  # hex
                    textRect = text.get_rect()
                    textRect.centerx = int((x2+x1)/2.0)
                    textRect.centery = int((y2+y1)/2.0)
                    self.frame.blit(text, textRect)
        
        # Show the grid
        self.showOnce()
        
        return



if __name__ == '__main__':
    
    # Create a reference to the new class object
    fig = GridFigure(title='Test Figure', size=(640,480), colorMode='hotcold')

    # Create a random array
    array = numpy.random.rand(10,20)
    array *= 100
    array = numpy.array(array, dtype=int)

    # Plot the figure
    fig.DrawGrid(array, minVal=0, maxVal=100)
    
    # Save the figure
    #fig.save('TestFile.png')
    
    # Enter an infinite loop
    fig.show()

    print 'Script complete...'

# End of File