   M S B A M L   `   `   `  ����  GameLauncher(    "GameLauncher.Views.KeyBindingsView   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      -   5          >�Name$  KeyBindingsUserControl��5      .��5      � .+�5   
   �� $��20q�6   .+�5      �� $��24(�6   $
��Bold"�6&   $��Configure Key Bindings��68   $��0,0,0,20q�6V   �� 5	      $��Input Device:��6   $��16(�6-   �� 5
      $	��120��6i   $��0,0,0,20q�6u   ��6   �� *InputDevices+ �� SelectedItem 6<   �� *SelectedInputDevice+6u   �� 5      ��6   �� *KeyBindings+��5      �� 5      .R�5      ��  �� Orientation$ 
Horizontal=�6%   $��0,0,0,10q�6>   .+�5      �� $	��100��6@   $��Center=�6L   ��6(   �� *Action+6L   �� 5      $	��120��5   %   $	��260��5   %   $
��Left=�5   %   ��5   %   �� *
DisplayKey+ �� Mode$ OneWay=���5   %   �� *DataContext.SetKeyCommand+ �� RelativeSource ��  �� AncestorType UserControl��5   %   �� *Action+
 � Tag 5   %   �� ��5   "   �� 5   &   .R�5   *   �� $��CharacterEllipsis=�6E   ��64   �� 6E   5   '   5      5      5      5!      �� 5#      
��� E��$��0,10,0,0q�6O   $
��Bold"�6a   ��6   �� *WaitingForText+6a   �� 5%      $ 
Horizontal=�6   $��Right=�62   $��0,20,0,0q�6N   .+�5&      �� $
��Save��6   $	��100��6(   $��0,0,10,0q�64   ��6F   �� *SaveCommand+�� 5'      $��Cancel��6   $	��100��6*   ��66   �� *CancelCommand+5(      5)      5*      5+      