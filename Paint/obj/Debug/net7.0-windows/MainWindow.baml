   M S B A M L   `   `   `  ����@  <Paint, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:PaintPaint  5      N JFluent, Version=10.0.0.0, Culture=neutral, PublicKeyToken=3e436e32a8c5546f/$clr-namespace:Fluent;assembly=FluentFluent 5          Paint.MainWindow   5      W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5      R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      %localclr-namespace:Paint     5      7Fluent$clr-namespace:Fluent;assembly=Fluent     5      -   5          � Title$  
MainWindow��5	      $	��900��5
      $
��1200��6"    � 	Resources 5         Paint.PercentConverter   percentConverter��%   &	          5   
   6!   5      .��5      ]� $
��True��6   .+�5   
     Fluent.Ribbon  -   5          Name$ ribbon��5      $	��Top=�5      
�������   Menu 5        Fluent.Backstage  5      -   5          Name$ 	backStage��5   2     Fluent.RibbonControl   Header$
 File��6#      Content. 5         Fluent.BackstageTabControl  .@�5        Fluent.BackstageTabItem  -   5           Header$	 New��5   2   6?     5      -   5        $
 Load��5   2   6@     5      -   5        $
 Save��5   2   6@     5      -   5        $ Save as��5   2   6C     5      -   5        $
 Quit��5   2   6@   5      5          Tabs. 5#        Fluent.RibbonTabItem  	   Header$
	 Home��6#   
   Groups.
 5%        Fluent.RibbonGroupBox  $��	Clipboard��6(   .@�5&      	  Fluent.Button	  -	   5         	  Header$ Paste��5&   $    	  	LargeIcon 5'      �� 5(      $��/Images/Paste.png��6$    �� BitmapScalingMode$ HighQuality=�6?   5*      	  5+      -
   5        $	 Cut��5+   $    5,      �� 5-      $��/Images/cut.png��6$   $ HighQuality=�6=   5/      	  50      -   5        $
 Copy��50   $    51      �� 52      $��/Images/copy.png��6$   $ HighQuality=�6>   54      55        57      $
��Tool��6(   .@�58      	  -   5         	 Name$ 	btnSelect��58   $   $ Select��67    59      �� 5:      $��/Images/selection.png��6$   $ HighQuality=�6C   5<      	  5=      -   5        $
 Undo��5=   $    5>      �� 5?      $��/Images/undo.png��6$   $ HighQuality=�6>   5A      	  5B      -   5        $
 Redo��5B   $    5C      �� 5D      $��/Images/redo.png��6$   $ HighQuality=�6>   5F      	  5G      -   5        $ Delete��5G   $    5H      �� 5I      $��/Images/delete.png��6$   $ HighQuality=�6@   5K      	  5L      -   5        $
 Fill��5L   $    5M      �� 5N      $��/Images/fill.png��6$   $ HighQuality=�6>   5P      5Q        5S      $��Insert��6(   .@�5T      	  -   5        $ Image Insert��5T   $    5U      �� 5V      $��/Images/image.png��6$   $ HighQuality=�6?   5X      5Y        5[      -   5          Name$ insertShape��5[   (   $��Shape��6=     5]      $��Style��6(      State$ 	Collapsed=�67   .@�5^      
  Fluent.DropDownButton
   
  Header$ Thick��6,    
  	LargeIcon 5_      �� 5`      $ ��/Images/line-thickness.png��6$   $ HighQuality=�6H   .@�5c        Fluent.RadioButton  -   5          Name$ onePx��5c   -      Header$	 1px��6P     5d      -   5        $ twoPx��5d   -   $	 2px��6P     5e      -   5        $ fivePx��5e   -   $	 5px��6R     5f      -   5        $ tenPx��5f   -   $
 10px��6P   5h      �� 5i      
  5k      $ Dashed style��6,    5l      �� 5m      -   5         ��Name$ lineStyleIcon��5m   $   $��/Images/dotline0.png��6;   $ HighQuality=�6Y   .@�5p        Fluent.ToggleButton  -   5          Name$ dotline0��5p   .      Icon 5q      �� 5r   "   $��/Images/dotline0.png��6(   $ HighQuality=�6F   5t        5v      -   5        $ dotline1��5v   .    5w      �� 5x   "   $��/Images/dotline1.png��6(   $ HighQuality=�6F   5z        5|      -   5        $ dotline2��5|   .    5}      �� 5~   "   $��/Images/dotline2.png��6(   $ HighQuality=�6F   5�      5�      5�        5�      $��Color��6(   .@�5�      �� $
��Auto=�6#   .��5�        Fluent.ColorGallery  -   5          Name$ colorGallery��5�   .   5�      5�      5�        5�      $
��Zoom��6(   .@�5�      ��  �� Orientation$ 
Horizontal=�6!   .+�5�      �� $
��Bold"�5�   $   $��Center=�5�   $   $��5q�5�   $   ��5�   $   ��  �� ElementName$ scaleSlider�� �� Path$ Value� �� 	Converter# [  5�   $   �� 5�      -   5         ��Name$ scaleSlider��5�   !   $	��100��66   $
��5 20q�6B    � Maximum$	 200Q�5�   !    � Minimum$ 50Q�6/     �� IsSnapToTickEnabled$
  True��5�   !   ! �� TickFrequency$! 50Q�5�   !   " � Value$	" 100Q�5�   !   5�   !   5�      5�        5�      $��Layer��6(   .@�5�      �� -   5        # ��Name$# myListLayer��5�      $��80��6c   $��70��6n   ��5�      �� 5�      .R�5�   "   �� $ 
Horizontal=�6-   .+�5�   &   �� �� 5�   *   �6.   �� $
 Name� - �� 6N   �6R   �� $ ZIndex�5�   '   5�   #   5�      5�      �� 5�      $ Vertical=�6!   .+�5�      �� -   5        $	��Add��5�   8   $��5q�6F   $��5q�6Q   
�������6]   �� 5�      -   5        $��Delete��5�   ;   $��5 0 5 5q�6M   $��5q�6^   
�������6j   5�      5�        5�      $��Rotation��6(   .@�5�      �� $ 
Horizontal=�6!   .+�5�      �� $
��Bold"�5�   $   $��Center=�5�   $   $��5q�5�   $   ��5�   $   �� $ rotationSlider��$ Value�# [  5�   $   �� 5�      -    5        $ rotationSlider��5�   !   $	��100��69   $
��5 20q�6E   $	 360Q�5�   !   $ 0Q�6/   $
  True��5�   !   $! 1Q�5�   !   5�   !   5�      5�      5�      5�      � 5�   
   -!   5        $ �Name$$ myGrid��5�      % � RowDefinitions% 5�      �� 5�      $��* �6    �� 5�      $��50 �6    .+�5�      �� -"   5        & ��Name$& drawingCanvas��5�      $��0��5�      ' J� IsHitTestVisible$
' True��5�      $
}�True��5�      
�������5�      �� 5�      -#   5        $& drawingBorder��5�      $��0��5�      $
}�True��5�      
������ 5�      �� 5�      -$   5        ( ��Name$( 
positionXY��5�      $��1��5�      $��My text��5�      $��10 0 0 0q�5�      $
��Bold"�5�      $��Center=�5�      5�      5�      5�      