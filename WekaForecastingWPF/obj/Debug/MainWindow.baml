   M S B A M L   `   `   `  ����M  IWekaForecastingWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:WekaForecastingWPFWekaForecastingWPF  5   	   U QLiveCharts.Wpf, Version=0.9.7.0, Culture=neutral, PublicKeyToken=0bc1f845d1ebb8dfG4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.WpfLiveCharts.Wpf 5   	   #    WekaForecastingWPF.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:WekaForecastingWPF     5   	   Dlvc4clr-namespace:LiveCharts.Wpf;assembly=LiveCharts.Wpf     5   	     � Title$  Forecasting��5	   	   $	��682��6   $
��1165��6*   .��5
      � $��
0,10,11,31q�6    � RowDefinitions 5   
   �� 5      $
��auto �6    � ColumnDefinitions 5   
   �� 5      $
��auto �6   .+�5   
   #  LiveCharts.Wpf.CartesianChart  -   5          Name$ Chart��5      &   LiveCharts.Wpf.Charts.Base.Chart   LegendLocation$ Bottom=�6,   $��0,10,142,-27q�6D      Series 5      "  LiveCharts.Wpf.ScatterSeries    LiveCharts.Wpf.Series   Title$ Current Data��6$      Values 69   �� *Current_Data+  5      $ 	Next Data��6$    66   �� *	Next_Data+  LiveCharts.Wpf.LineSeries  5      $ Predicted Data��6!    68   �� *Predicted_Data+  5      $ Negetive Predicted Data��6$      Fill �	#FF37474F	   Stroke	 �	#FF37474F 6D   �� *Negetive_Predicted_Data+6�   5      �� 5   
   -   5        
 ��Name$
 listBox��5      $
��Left=�6#   $	��172��6>   $��
985,40,0,0q�6K   $	��Top=�6_   $	��135��6w   �� 5   
   -   5         ��Name$ Forecast��5      $��Forecast��6#   $
��Left=�66   $��987,367,0,0q�6Q   $	��Top=�6f   $	��135��6~    �� IsCancel . $��38��6�   � 5   
   -   5         �Name$ Popup��5      $��Mouse=�6   .��5      � 
�����_�$	��214��6*   .+�5      �� -   5         ��Name$ PopupMassege��5      $��Center=�62   $��Center=�6O   
�������6j   �� 5      -   5        $ ok_btn��5      $��
68,51,72,5q�6)   .��6R   Ok6V   5      5       � 5!   
   $
��auto��6   $
��auto��6   .+�5"      � -   5         �Name$ ProgressBar��5"      $
��Left=�6/   $��25��6J   $��988,507,0,0q�6V   $	��Top=�6k   $	��132��6�   
�����_�6�   �� 5#       �� TextAlignment$ Center=�5$      $��873,507,-99,40q�6&    J� RenderTransformOrigin$ 0.562,0.563&�6>   $��25��6b   ��5#      ��  �� ElementName$ ProgressBar�� �� Path$ Value� �� StringFormat$ {0:0}%��5$   b   �� 5%      $�� Hashtags list:��6   $��1008,0,9,540q�6.   �� 5&      -   5        $ Current_Data_btn��5&      $��Current Data��6/   $
��Left=�6F   $��988,231,0,0q�6a   $	��Top=�6v   $	��135��6�   $ 0.7,0.35&�6�   $��44��6�   �� 5'      $��Steps to forecast:��6   $
��Left=�61   $��988,302,0,0q�6L   $	��Top=�6a   $	��113��6y   �� 5(      -	   5         ��Name$ TextBox��5(      $
��Left=�6'   $��27��6B   $��988,333,0,0q�6N    �� TextWrapping$
 Wrap=�6c   $	��Top=�6w   $	��135��6�   �� 5)      -
   5         ��Name$ Standard_Error��5)      $
��Left=�6,   $��988,543,0,0q�6G   $	��Top=�6\   $��30��6t   $	��135��6�   $��Standard Error:��6�   5*      �� 5+   
   $��Process Progress:��6   $
��Left=�6,   $��985,476,0,0q�6G   $	��Top=�6\   $	��114��6t   $��28��6�   5,      5-      