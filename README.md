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

1. Dynamically load all graphic objects that can be drawn from external DLL files (Done)
2. The user can choose which object to draw (Done)
3. The user can see the preview of the object they want to draw (Done)
4. The user can finish the drawing preview and their change becomes permanent with previously drawn objects (Done)
5. The list of drawn objects can be saved and loaded again for continuing later (Done)
    
    You must save in your own defined binary format (Done)
    
6. Save and load all drawn objects as an image in bmp/png/jpg format (rasterization). Just one format is fine. No need to save in all three formats. (Done)
# Basic graphic objects

1. Line: controlled by two points, the starting point, and the endpoint (Done)
2. Rectangle: controlled by two points, the left top point, and the right bottom point (Done)
3. Ellipse: controlled by two points, the left top point, and the right bottom point (Done)

# Improvements (Choose and propose as you wish)

1. Allow the user to change the color, pen width, stroke type (dash, dot, dash dot dot..._ (Done)
2. Adding text to the list of drawable objects (Done)
3. Adding image to the canvas (Done)
4. Reduce flickering when drawing preview by using buffer to redraw all the canvas (Done)
    
    Upgrade: Only redraw the needed region, no fullscreen redraw (Done)
    
5. Adding Layers support (Done)
6. Select a single element for editing again
    
    Transforming horizontally and vertically (Done)
    
    Rotate the image (Done)
    
    Drag & Drop (Done)
    
7. Zooming (Done)
8. Cut / Copy / Paste (Done)
9. Undo, Redo (Command) (Done)
10. Fill color by boundaries (Done)
11. Delete shape (Done)
