# Project Paint

<aside>
⛳ Objective: Create a program to draw graphic object

</aside>

Expected work hours: 10 hours (Not including Learning / Studying / Debugging)
# Knowledge base

> The core concept of the processing is to draw graphic objects
> 

## Technical details

- Design patterns: Singleton, Factory, Abstract factory, prototype
- Plugin architecture
- Delegate & event

# Core requirements (5 points)

1. Dynamically load all graphic objects that can be drawn from external DLL files
2. The user can choose which object to draw
3. The user can see the preview of the object they want to draw
4. The user can finish the drawing preview and their change becomes permanent with previously drawn objects
5. The list of drawn objects can be saved and loaded again for continuing later
    
    You must save in your own defined binary format
    
6. Save and load all drawn objects as an image in bmp/png/jpg format (rasterization). Just one format is fine. No need to save in all three formats.
# Basic graphic objects

1. Line: controlled by two points, the starting point, and the endpoint
2. Rectangle: controlled by two points, the left top point, and the right bottom point
3. Ellipse: controlled by two points, the left top point, and the right bottom point

# Improvements (Choose and propose as you wish)

1. Allow the user to change the color, pen width, stroke type (dash, dot, dash dot dot..._
2. Adding text to the list of drawable objects
3. Adding image to the canvas
4. Reduce flickering when drawing preview by using buffer to redraw all the canvas
    
    Upgrade: Only redraw the needed region, no fullscreen redraw
    
5. Adding Layers support
6. Select a single element for editing again
    
    Transforming horizontally and vertically
    
    Rotate the image
    
    Drag & Drop
    
7. Zooming
8. Cut / Copy / Paste
9. Undo, Redo (Command)
10. Fill color by boundaries
11. Anything that you think is suitable
