# UIToolkit_fork
A fork from oddgame/UIToolkit plugin. More UIElements and bugfixes.

## Addons
- UIElements:
  * **UIKeyboard**. It interprets the keyboard writing to put it in focused UITextField.
  * **UITextField**. It haven't vertical boundaries.
  * **UITooltip**. Some elements can have tooltip which show up when pointer is over the element.
  * Can add text to some elements like buttons.
- Container:
  * **UIDraggableLayout**.
- Bugfixes:
  * UIColorPicker::getTouchTextureCoords() now returns correct coords.
  * **Touch** input events now work in WebPlayer. **Solution by https://github.com/hyakugei**.
  * **UILanguageManager**[string token] doesn't raise an exception anymore, now it returns string "null" if token doesn't exist.
  * **UILanguageManager**::loadLanguageTextsFromJSON(string language) in the foreach loop it doesn't look up for "StringToRead" cause I use other JSON format for translation texts. ```
  {
   "TranslatedStrings":{
      "A01_0_00_003":"Median.",
      "A01_0_00_004":"Coronal."...
  }
}```
 * UISlider now works and looks as expected.
 - Some minor changes all over the script for getting addons to work or new util functions.

## Usage
I followed the same code pattern as the original UIToolkit, so it's straightforward getting used with the new.

## License (same as oddgame/UIToolkit)
[Attribution-NonCommercial-ShareAlike 3.0 Unported](http://creativecommons.org/licenses/by-nc-sa/3.0/legalcode) with [simple explanation](http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_US).

## Thanks
Thanks to **prime31** for creating and **oddgame** for supporting this fantastic plugin.

## Contact
Fell free to post issues or doubts about usage or whatever at **vonflaken@gmail.com.**
