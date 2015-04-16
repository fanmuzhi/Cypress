#!/usr/bin/env python
'''
This module implements a basic position drawing library. The class implemented in 
this file implements a TPA-style window for representing touch data.
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

turquoise = color(red=64, green=224, blue=208)
red = color(red=255, green=0, blue=0)
green = color(red=0, green=255, blue=0)
blue = color(red=0, green=0, blue=255)
yellow = color(red=255, green=255, blue=0)
deepPink = color(red=255, green=20, blue=147)
orange = color(red=255, green=127, blue=0)
purple = color(red=155, green=50, blue=204)
aqua = color(red=0, green=255, blue=255)
tomato = color(red=255, green=99, blue=71)
gray = color(red=148, green=148, blue=148)
lightSalmon = color(red=255, green=160, blue=122)
lightGold = color(red=255, green=215, blue=0)
cornflowerBlue = color(red=100, green=149, blue=237)
blanchedAlmond = color(red=255, green=235, blue=205)
teal = color(red=0, green=128, blue=128)
yellowGreen = color(red=154, green=205, blue=50)

fingerColors = [red,
                green,
                blue,
                yellow,
                deepPink,
                orange,
                purple,
                aqua,
                tomato,
                gray,
                lightSalmon,
                lightGold,
                cornflowerBlue,
                blanchedAlmond,
                teal,
                yellowGreen]


#-------------------------------------------------------------------------------
#                              PositionFigure Class
#-------------------------------------------------------------------------------
class PositionFigure(object):
    
    def __init__(self, title='DrawPositions', size=(640,480), renderText=True, flipX=False, flipY=False, resX=640, resY=480, maxTouchSize=100, maxFingerSize=40):
        ''' Initialization method for the PositionFigure class. '''

        # Initialize class variables
        self.title = title
        self.size = size
        self.renderText = renderText
        self.SetWindowOrientation(flipX, flipY)
        self.resX = resX
        self.resY = resY
        self.maxTouchSize = maxTouchSize
        self.maxFingerSize = maxFingerSize
        
        self.hotkeyCallbacks = []
        self.running = True
        self.lastPositionData = None
        
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
        ''' Quits the display. '''
        self.running = False
        return
        
    def SetWindowOrientation(self, flipX=None, flipY=None):
        ''' Sets the window orientation (flipX, flipY). '''
        if flipX is not None:
            self.flipX = flipX
        if flipY is not None:
            self.flipY = flipY
        return
    
    def AddHotkeyCallback(self, key, callback):
        ''' Member for adding callbacks when a key is pressed. '''
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
                
                if self.lastPositionData is not None:
                    self.DrawPositions(self.lastPositionData)
            
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
        pygame.draw.rect(self.screen, clearColor.rgb, (0, 0, self.size[0], self.size[1]), 0)
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
        
    
    def _ConvertToScreenCoords(self, x=0, y=0, z=0):
        '''
        Internal method to help convert from touchscreen coordinates to 
        screen coordinates.
        '''
        # Convert X
        x = int(float(x * (self.size[0]-1)) / float(self.resX - 1))    
        if self.flipX == True:
            x = screenResX - x
        
        # Convert Y
        y = int(float(y * (self.size[1]-1)) / float(self.resY - 1))
        if self.flipY == True:
            y = screenResY - y
        
        # Convert Z
        z = int(float(z * self.maxFingerSize) / float(self.maxTouchSize))

        return (x,y,z)
        
    def DrawPositions(self, positionData):
        ''' This method draws a 2D numpy array of data. '''
        
        # Save the last set of position data
        self.lastPositionData = positionData
                
        # Clear the screen
        self._ClearScreen(white)
        
        # Setup font rendering
        '''
        if self.renderText == True:
            fontSize = int(boxWidth/3.0)
            fontColor = white
            font = pygame.font.Font(None, fontSize)
            font.set_bold(True)
            (w,h) = font.size('8')
            fontSize = int(fontSize * (float(boxWidth) / (5.*float(w)) ) )       
            font = pygame.font.Font(None, fontSize)
            font.set_bold(True)
        '''
        
        # Start with a white box
        pygame.draw.rect(self.frame, white.rgb, (0, 0, self.size[0], self.size[1]), 0)
               
        # Loop over all fingers in the touch
        for i in range(0, len(positionData)):
            # Get the touch
            touch = positionData[i]
            
            # Convert the touch coordinates to screen coordinates
            [x,y,z] = self._ConvertToScreenCoords(touch['x'], touch['y'], touch['z']);        
        
            # Get the finger id
            id = touch['id']
        
            # Get the finger color
            try:
                fingerColor = fingerColors[id]
            except:
                fingerColor = black;
                        
            # Draw the touch
            pygame.draw.circle(self.frame, fingerColor.rgb, (x,y), z, 0)
        
        
        # Show the grid
        self.showOnce()
        
        return



if __name__ == '__main__':
    
    # Create a reference to the new class object
    fig = PositionFigure(title='Test Figure', size=(640,480))

    # Create a random array
    positionData = []
    dict = {}
    dict['x'] = 255
    dict['y'] = 100
    dict['z'] = 50
    dict['id'] = 1
    positionData.append(dict)
    dict = {}
    dict['x'] = 50
    dict['y'] = 200
    dict['z'] = 20
    dict['id'] = 2
    positionData.append(dict)

    # Plot the figure
    fig.DrawPositions(positionData)
    
    # Save the figure
    #fig.save('TestFile.png')
    
    # Enter an infinite loop
    fig.show()

    print 'Script complete...'

# End of File