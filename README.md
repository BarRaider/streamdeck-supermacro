# SuperMacro - Advanced keystroke macros triggered by the Elgato Stream Deck

**Author's website and contact information:** [https://barraider.com](https://barraider.com)

## New in v1.9
- Added variable support for `VARSETFROMFILE` and `OUTPUTTOFILE`
- `MouseXY` and `MouseMove` commands now support both a comma and a colon between the X and Y values (for better consistency with other commands) `{{MOUSEXY:$VarX:$VarY}}`
- Length of  the long-press in `SuperMacro` action is now customizable
- `Keystroke PTT` now has a `Run Once` option, to only run the command a single time
- You can now control the output frequency of the `Keystroke PTT` using the delay slider
- `{{MSCROLLUP}}` and `{{MSCROLLDOWN}}` now support an optional number of clicks to scroll. (See example 13 below)
- `{{MOUSEXY}}` now supports variables for moving the mouse (see example 14 below)
- `{{MSAVEPOS}}` now stores the values in variables `$MOUSE_X` and `$MOUSE_Y`
- New `REPLACE` function allows replacing parts of a variable/string (see usage in `Functions` section and example 15 below)
- New `NOW` function allows to get the current date and time and format it (see example 16 below)
- Added link to documentation inside the settings
- Fixed issue with `Output` command messing up text

## New in v1.8
- :new: ***FUNCTIONS Support!*** 
	- `RANDOM` function allows choosing a random number in a customized range
	- `ADD/SUB/MUL/DIV` allow to Add, Subtract, Multiply and Divide numbers
	- `CONCAT` function allows to concatenate multiple strings/variables into one
- Comments support: Type `{{//}}` and everything after it will be ignored, until the end of the line.
- New `{{SetKeyTitle}} ` command to dynamically change the title of the Stream Deck button. See {{SETKEYTITLE}} command below.
- New `{{OutputToFile}} ` command to store contents of a variable to file. See {{OUTPUTTOFILE}} command below.
- New `{{VarSetFromClipboard}} ` command to set a variable with the clipboard contents. See {{VARSETFROMCLIPBOARD}} command below.
- New `Mouse Location` action shows you the current mouse coordinates, to be used with the MouseXY command.
- `SuperMacro Toggle` can now be set to remember the toggle state even when switching profiles or restarting the Stream Deck :pogchamp:
- `SuperMacro PTT` action now supports sending a specific keystroke when key is released
- `SuperMacro PTT` and `StickyKeypress` action now support a customizable delay (pause) between every execution.
- {{INPUT}} Command now remembers the previous value and autofills it in the popup window

## New in v1.7
- :new: ***Loops Support!***
	- *Sticky SuperMacro* and *Sticky Keystroke* both support loops! Use the `Auto Stop After N Rounds` to create loops which will run a customizable amount of times.
- New Variable Commands! 
	- {{VarSet}} - Use `{{VARSET:VarName:Value}}` to set the value `Value` into `VarName`. You can then use `{{OUTPUT:VarName}}` to display it
	- {{VarSetFromFile}} - Use `{{VARSETFROMFILE:VarName:c:\myfile.txt}}` to read `myfile.txt` and store its contents into `VarName`. You can then use `{{OUTPUT:VarName}}` to display it
	- {{VarUnset}} - Use `{{VARUNSET:VarName}}` to clear `VarName`
	- {{VarUnsetAll}} - Clears all variables. Usage: `{{VARUNSETALL}}`
- Full support for Numpad buttons! Added a bunch of new commands to differentiate between Numpad buttons and their non-Numpad variation (Arrows, Home, End, ...). See full list below
- Storing/Restore mouse position. Use `{{MSAVEPOS}}` to store the current mouse position. Later use `{{MLOADPOS}}` to move the mouse back to that position
- Mouse Double-Clicks:
	- Left Double Click now uses the `{{MLEFTDBLCLICK}}` command.
	- Right Double Click now uses the `{{MRIGHTDBLCLICK}}` command.
- `MouseXY` command which is superseding the `MousePos` command. Use it to move the mouse cursor to a specific place on your desktop. Use together with the new `Mouse Location` action to quickly determine the coordinates you want.
- Support for mouse Button 4 and Button 5 - Use `{{XBUTTON1}}` for Button 4 click. Use `{{XBUTTON2}}` for Button 5 click.
- New `Let macro complete on stop` option available for Sticky SuperMacro. Will ensure the macro completes fully when the button is pressed

## Current functionality
### 5 Plugins built into one:
#### Super Macro
This is the basic implementation. Create a macro and run it on keypress. Examples can be seen in the ***Usage Examples*** section below.

#### Super Macro Toggle
Toggle between two different macros.

#### Sticky Super Macro
Click once to enable, the macro will run again and again until the button is pressed again

#### Keystroke PTT
This action limits the action to either one command (such as {{ctrl}{c}}) or one character. The command will be run again and again as long as you continue to press the key.

#### Sticky Keystroke
This action limits the action to either one command (such as {{ctrl}{c}}) or one character. The command will be run again and again until the button is pressed again.

## How do I get started using it?
SuperMacro knows to deal with both *Commands* and normal text. A command is either one special key (like F5 or Winkey) or a keystroke (like Ctrl-C). A command is always enclosed in {} and each individual key in the command is also inclosed in {} so you should always see two `{{` at the beginning and two `}}` at the end. For instance: `{{f5}}` or `{{ctrl}{c}}`

### Usage Examples
1. Open Windows Explorer and got to C:\Program Files  
Note: Delay should be ~20 ms  
```
{{win}{e}}{{pause:400}}{{alt}{d}}c:\Program Files\{{enter}}
```

2. Open notepad and play with the settings  
Note: Delay should be ~20 ms  
Note2: This will not work correctly if your Windows (and notepad) are not in English  
```
{{win}{r}}{{pause:500}}notepad.exe{{enter}}{{pause:1000}}Ok... Let's see what this plugin can do...{{alt}{f}}{{right}}{{PAUSE:400}}{{right}}{{PAUSE:400}}f{{pause:400}}times{{down}}{{PAUSE:400}}{{tab}}{{PAUSE:400}}{{down}}{{PAUSE:400}}{{down}}{{PAUSE:400}}{{ENTER}}{{ENTER}}For more information visit: https://barider.g1thubio{{ctrl}{shift}{left}}{{PAUSE:400}}https://barraider.github.io{{ENTER}}{{alt}{o}}f{{PAUSE:100}}Lucida Console{{tab}}Regular{{Tab}}12{{ENTER}}
```

3. Calculate something  
Note: Delay should be ~20 ms  
```
{{win}{r}}{{pause:300}}calc{{enter}}{{pause:1000}}1*2*3*4*5=
```
4. Move the mouse to a certain position on the screen, then press Double-Click left mouse button.  
Note: To find the correct position you can use the Mouse Location action.
```
{{MOUSEXY:1000,15}}{{MLEFTDBLCLICK}}
```
5. Move the mouse by 10 pixels left and 20 pixels down on every press
```
{{MOUSEMOVE:-10,20}}
```
6. Variables: Get input from user and then use it later on.
```
{{INPUT:Name}}Hello {{OUTPUT:Name}}, Nice to meet you!
```

7. Variables: Read text from file into MyVar variable
```
{{VarSetFromFile:MyVar:C:\filename.txt}}
```

8. Functions: Choose a random number between 1 (inclusive) to 10 (exclusive) and store it in MyVar:
```
{{FUNC:RANDOM:MyVar:1:10}}
```

9. Functions: Input 2 numbers from the user. Choose a random number between firstNum variable (inclusive) to secondNum variable (exclusive) and store it in MyVar:
```
{{INPUT:firstNum}}
{{INPUT:secondNum}}
{{FUNC:RANDOM:MyVar:$firstNum:$secondNum}}
```

10. Functions: Select a number from the user and multiply it by 10. Then save it to a file named c:\temp\result.txt:  
```
{{INPUT:myNumber}}
{{FUNC:MUL:MyResult:$myNumber:10}}
{{OUTPUTTOFILE:MyResult:c:\temp\result.txt}}
```

11. Add comments in the code using `{{//}}` command
```
{{INPUT:myNumber}} {{//}} Input a number from the user
{{FUNC:MUL:MyResult:$myNumber:10}} {{//}} Multiply number by 10
{{OUTPUTTOFILE:MyResult:c:\temp\result.txt}} {{//}} Save result in file
```

12. Read text from a file and show it on the Stream Deck Key:
```
{{VARSETFROMFILE:MyVar:c:\counter.txt}}
{{SETKEYTITLE:$MyVar}}
```

13. Read text from a clipboard and show it on the Stream Deck Key:
```
{{VARSETFROMCLIPBOARD:MyVar}}
{{SETKEYTITLE:$MyVar}}
```

14. Scroll the mouse up by 5 clicks and then down by 3 clicks:
```
{{MSCROLLUP:5}}
{{MSCROLLDOWN:3}}
```

15. Move the mouse to coordinates set from variables:
```
{{VARSET:X:100}}
{{VARSET:Y:400}}
{{MOUSEXY:$X,$Y}}
```

16. Repalce all "l"'s with "Z"'s in the string `Hello World` and show it on key
```
{{VARSET:XX:Hello World}}
{{VARSET:A:l}}
{{VARSET:B:Z}}
{{FUNC:REPLACE:MyVar:$XX:$A:$B}}
{{SETKEYTITLE:$MyVar}}
```

17. Show the current date and time on the key:
```
{{FUNC:NOW:MyVar:yyyy-MM-dd 
HH:mm:ss}}
{{SETKEYTITLE:$MyVar}}
```

*** More commands below ***


### Download

* [Download plugin](https://github.com/BarRaider/streamdeck-supermacro/releases/)

## I found a bug, who do I contact?
For support please contact the developer. Contact information is available at https://barraider.com

## I have a feature request, who do I contact?
Please contact the developer. Contact information is available at https://barraider.com

## Dependencies
* Uses StreamDeck-Tools by BarRaider: [![NuGet](https://img.shields.io/nuget/v/streamdeck-tools.svg?style=flat)](https://www.nuget.org/packages/streamdeck-tools)
* Uses [Easy-PI](https://github.com/BarRaider/streamdeck-easypi) by BarRaider - Provides seamless integration with the Stream Deck PI (Property Inspector) 


## List of supported keystroke commands

<table id="commands" border="1">
    <tbody>
        <tr>
            <th align="center">Keyboard Key</th>
            <th align="center">Macro Command</th>
        </tr>
		<tr>
            <td>Letters A-Z</td>
            <td>{VK_XXXX} (XXXX = the letter - e.g. VK_A / VK_B ...)</td>
        </tr>
		<tr>
            <td>Numbers 0-9</td>
            <td>{VK_XXXX} (XXXX = the number - e.g. VK_0 / VK_1 ...)</td>
        </tr>
		<tr>
            <td>These characters:<br/><b>;/`[\]':?~{|}"</b></td>
            <td>Exact command changes between keyboard layouts:<br/>Try the following macros to figure out the correct command:<br/> <b>{{oem_1}}{{oem_2}}{{oem_3}}{{oem_4}}{{oem_5}} {{oem_6}}{{oem_7}}{{oem_8}}
			{{shift}{oem_1}}{{shift}{oem_2}}{{shift}{oem_3}} {{shift}{oem_4}}{{shift}{oem_5}} {{shift}{oem_6}}{{shift}{oem_7}}{{shift}{oem_8}}</b></td>
        </tr>
		<tr>
            <td>Numpad 0</td>
            <td>{NUMPAD0}</td>
        </tr>
        <tr>
            <td>Numpad 1</td>
            <td>{NUMPAD1}</td>
        </tr>
        <tr>
            <td>Numpad 2</td>
            <td>{NUMPAD2}</td>
        </tr>
        <tr>
            <td>Numpad 3</td>
            <td>{NUMPAD3}</td>
        </tr>
        <tr>
            <td>Numpad 4</td>
            <td>{NUMPAD4}</td>
        </tr>
        <tr>
            <td>Numpad 5</td>
            <td>{NUMPAD5}</td>
        </tr>
        <tr>
            <td>Numpad 6</td>
            <td>{NUMPAD6}</td>
        </tr>
        <tr>
            <td>Numpad 7</td>
            <td>{NUMPAD7}</td>
        </tr>
        <tr>
            <td>Numpad 8</td>
            <td>{NUMPAD8}</td>
        </tr>
        <tr>
            <td>Numpad 9</td>
            <td>{NUMPAD9}</td>
        </tr>
		<tr>
            <td>Numpad *</td>
            <td>{MULTIPLY}</td>
        </tr>
		<tr>
            <td>Numpad +</td>
            <td>{ADD}</td>
        </tr>
		<tr>
            <td>Numpad -</td>
            <td>{SUBTRACT}</td>
        </tr>
		<tr>
            <td>Numpad .</td>
            <td>{DECIMAL}</td>
        </tr>
		<tr>
            <td>Numpad /</td>
            <td>{DIVIDE}</td>
        </tr>
        <tr>
            <td>BACKSPACE</td>
            <td>{BACK}</td>
        </tr>
        <tr>
            <td>TAB</td>
            <td>{TAB}</td>
        </tr>
        <tr>
            <td>CLEAR</td>
            <td>{CLEAR}</td>
        </tr>
        <tr>
            <td>ENTER</td>
            <td>{RETURN} or {ENTER}</td>
        </tr>
        <tr>
            <td>SHIFT</td>
            <td>{SHIFT}</td>
        </tr>
        <tr>
            <td>Left SHIFT</td>
            <td>{LSHIFT}</td>
        </tr>
        <tr>
            <td>Right SHIFT</td>
            <td>{RSHIFT}</td>
        </tr>
        <tr>
            <td>CTRL</td>
            <td>{CONTROL} or {CTRL}</td>
        </tr>
        <tr>
            <td>Left CONTROL</td>
            <td>{LCONTROL} or {LCTRL}</td>
        </tr>
        <tr>
            <td>Right CONTROL</td>
            <td>{RCONTROL} or {RCTRL}</td>
        </tr>
        <tr>
            <td>ALT</td>
            <td>{ALT} or {MENU}</td>
        </tr>
        <tr>
            <td>Left ALT</td>
            <td>{LALT} or {LMENU}</td>
        </tr>
        <tr>
            <td>Right ALT</td>
            <td>{RALT} or {RMENU}</td>
        </tr>
		<tr>
            <td>PAUSE/BREAK</td>
            <td>{BREAK}</td>
        </tr>
        <tr>
            <td>CAPS LOCK</td>
            <td>{CAPITAL}</td>
        </tr>
        <tr>
            <td>ESC</td>
            <td>{ESCAPE}</td>
        </tr>
        <tr>
            <td>SPACEBAR</td>
            <td>{SPACE}</td>
        </tr>
        <tr>
            <td>PAGE UP</td>
            <td>{PAGEUP} or {PGUP} or {PRIOR}</td>
        </tr>
		<tr>
            <td>Numpad PAGE UP</td>
            <td>{NUMPAD_PAGEUP}</td>
        </tr>
        <tr>
            <td>PAGE DOWN</td>
            <td>{PAGEDOWN} or {PGDN} or {NEXT}</td>
        </tr>
		<tr>
            <td>Numpad PAGE DOWN</td>
            <td>{NUMPAD_PAGEDOWN}</td>
        </tr>
        <tr>
            <td>HOME</td>
            <td>{HOME}</td>
        </tr>
		<tr>
            <td>Numpad HOME</td>
            <td>{NUMPAD_HOME}</td>
        </tr>
		<tr>
            <td>END</td>
            <td>{END}</td>
        </tr>
		<tr>
            <td>Numpad END</td>
            <td>{NUMPAD_END}</td>
        </tr>
		<tr>
            <td>UP ARROW</td>
            <td>{UP}</td>
        </tr>
		<tr>
            <td>Numpad UP ARROW</td>
            <td>{NUMPAD_UP}</td>
        </tr>
        <tr>
            <td>LEFT ARROW</td>
            <td>{LEFT}</td>
        </tr>
         <tr>
            <td>Numpad LEFT ARROW</td>
            <td>{NUMPAD_LEFT}</td>
        </tr>
        <tr>
            <td>RIGHT ARROW</td>
            <td>{RIGHT}</td>
        </tr>
		<tr>
            <td>Numpad RIGHT ARROW</td>
            <td>{NUMPAD_RIGHT}</td>
        </tr>
        <tr>
            <td>DOWN ARROW</td>
            <td>{DOWN}</td>
        </tr>
		<tr>
            <td>Numpad DOWN ARROW</td>
            <td>{NUMPAD_DOWN}</td>
        </tr>
        <tr>
            <td>SELECT</td>
            <td>{SELECT}</td>
        </tr>
        <tr>
            <td>PRINT SCREEN</td>
            <td>{SNAPSHOT}</td>
        </tr>
        <tr>
            <td>PRINT</td>
            <td>{PRINT}</td>
        </tr>
        <tr>
            <td>EXECUTE</td>
            <td>{EXECUTE}</td>
        </tr>
        <tr>
            <td>INS</td>
            <td>{INSERT}</td>
        </tr>
		 <tr>
            <td>Numpad INS</td>
            <td>{NUMPAD_INSERT}</td>
        </tr>
        <tr>
            <td>DEL</td>
            <td>{DELETE}</td>
        </tr>
		<tr>
            <td>Numpad DEL</td>
            <td>{NUMPAD_DEL}</td>
        </tr>
        <tr>
            <td>HELP</td>
            <td>{HELP}</td>
        </tr>
        <tr>
            <td>Left Windows</td>
            <td>{LWIN} or {WIN} or {WINDOWS}</td>
        </tr>
        <tr>
            <td>Right Windows</td>
            <td>{RWIN}</td>
        </tr>
        <tr>
            <td>F1</td>
            <td>{F1}</td>
        </tr>
        <tr>
            <td>F2</td>
            <td>{F2}</td>
        </tr>
        <tr>
            <td>F3</td>
            <td>{F3}</td>
        </tr>
        <tr>
            <td>F4</td>
            <td>{F4}</td>
        </tr>
        <tr>
            <td>F5</td>
            <td>{F5}</td>
        </tr>
        <tr>
            <td>F6</td>
            <td>{F6}</td>
        </tr>
        <tr>
            <td>F7</td>
            <td>{F7}</td>
        </tr>
        <tr>
            <td>F8</td>
            <td>{F8}</td>
        </tr>
        <tr>
            <td>F9</td>
            <td>{F9}</td>
        </tr>
        <tr>
            <td>F10</td>
            <td>{F10}</td>
        </tr>
        <tr>
            <td>F11</td>
            <td>{F11}</td>
        </tr>
        <tr>
            <td>F12</td>
            <td>{F12}</td>
        </tr>
        <tr>
            <td>F13</td>
            <td>{F13}</td>
        </tr>
        <tr>
            <td>F14</td>
            <td>{F14}</td>
        </tr>
        <tr>
            <td>F15</td>
            <td>{F15}</td>
        </tr>
        <tr>
            <td>F16</td>
            <td>{F16}</td>
        </tr>
        <tr>
            <td>F17</td>
            <td>{F17}</td>
        </tr>
        <tr>
            <td>F18</td>
            <td>{F18}</td>
        </tr>
        <tr>
            <td>F19</td>
            <td>{F19}</td>
        </tr>
        <tr>
            <td>F20</td>
            <td>{F20}</td>
        </tr>
        <tr>
            <td>F21</td>
            <td>{F21}</td>
        </tr>
        <tr>
            <td>F22</td>
            <td>{F22}</td>
        </tr>
        <tr>
            <td>F23</td>
            <td>{F23}</td>
        </tr>
        <tr>
            <td>F24</td>
            <td>{F24}</td>
        </tr>
		<tr>
            <td>Plus: +=</td>
            <td>{OEM_PLUS} / {{SHIFT}{OEM_PLUS}}</td>
        </tr>
		<tr>
            <td>Minus: -_</td>
            <td>{OEM_MINUS} / {{SHIFT}{OEM_MINUS}}</td>
        </tr>
		<tr>
            <td>Period: .&gt;</td>
            <td>{OEM_PERIOD} / {{SHIFT}{OEM_PERIOD}}</td>
        </tr>
		<tr>
            <td>Comma: ,&lt;</td>
            <td>{OEM_COMMA} / {{SHIFT}{OEM_COMMA}}</td>
        </tr>
        <tr>
            <td>NUM LOCK</td>
            <td>{NUMLOCK}</td>
        </tr>
        <tr>
            <td>SCROLL LOCK</td>
            <td>{SCROLL}</td>
        </tr>     
    </tbody>
</table>

## Advanced Commands
Note: Use a `:` between the command name and the arguments

<table id="advanced" border="1">
    <tbody>
		<tr>
            <td>{{//}}</td>
            <td><b>Comments Support</b>: Anything after the {{//}} sign will be ignored until end of line.</td>
        </tr>
		<tr>
            <td>PAUSE</td>
            <td>{{PAUSE:XXXX}} (XXXX = length in miliseconds)</td>
        </tr>
		<tr>
            <td>KeyDown</td>
            <td>{{KeyDown:XXXX}} (XXXX = name of key, example {{KeyDown:F1}})</td>
        </tr>
		<tr>
            <td>KeyUp</td>
            <td>{{KeyUp:XXXX}} (XXXX = name of key, example {{KeyUp:SHIFT}})</td>
		</tr>
		<tr>
			<td>Input</td>
			<td>{{Input:VarName}} Get input from the user and store it in 'VarName'.</td>
		</tr>
		<tr>
			<td>Output</td>
			<td>{{Output:MyVar}} Output the input previously gathered into 'MyVar'.</td>
		</tr>
		<tr>
			<td>VarSet</td>
			<td>{{VarSet:MyVar:MyValue}} set the value `MyValue` into `MyVar`.</td>
		</tr>
		<tr>
			<td>OutputToFile</td>
			<td>{{OutputToFile:MyVar:C:\filename.txt}} write the contents of the `MyVar` variable into `c:\filename.txt` file.</td>
		</tr>
		<tr>
			<td>VarSetFromFile</td>
			<td>{{VarSetFromFile:MyVar:C:\filename.txt}} read the contents of the file specified and store into `MyVar`.</td>
		</tr>
		<tr>
			<td>VarSetFromClipboard</td>
			<td>{{VarSetFromClipboard:MyVar}} read the contents of the clipboard and store into `MyVar`.</td>
		</tr>
		<tr>
			<td>VarUnset</td>
			<td>{{VarUnset:MyVar}} clears `MyVar`.</td>
		</tr>
		<tr>
			<td>VarUnsetAll</td>
			<td>{{VARUNSETALL}} clears all variables.</td>
		</tr>
		<tr>
			<td>MSavePos</td>
			<td>{{MSAVEPOS}} stores the current mouse cursor position. <br/>The position is stored in variables: $MOUSE_X and $MOUSE_Y</td>
		</tr>
		<tr>
			<td>MLoadPos</td>
			<td>{{MLOADPOS}} moves the mouse to the previous set position (when `{MSAVEPOS}` was called).</td>
		</tr>
		<tr>
			<td>SetKeyTitle</td>
			<td>{{SetKeyTitle:$MyVar}} Sets the text on the Stream Deck key to the contents of `MyVar`.</td>
		</tr>
	</tbody>
</table>

## Functions
### Syntax: 
```
{{FUNC:NameOfFunction:OutputVariable:InputParam1:InputParam2:InputParam3...}}

Where 'InputParamX' can either be text (10) or another variable ($MyVar)
```

<table id="functions" border="1">
    <tbody>
        <tr>
            <th align="center">Function Name</th>
            <th align="center">Number of Input variables</th>
			<th align="center">Example</th>
			<th align="center">Comments</th>
        </tr>
		<tr>
            <td>ADD</td>
            <td>2</td>
			<td>{{FUNC:ADD:MyVar:10:20}} (10+20 and store in MyVar)<br/>
			{{FUNC:ADD:Var1:10:$Var2}} (Add 10 to Var2 and store in Var1)<br/>
			{{FUNC:ADD:Result:$Var1:$Var2}} (Sum Var1 and Var2 and store in Result)</td>
        </tr>
		<tr>
            <td>SUB</td>
            <td>2</td>
			<td>{{FUNC:ADD:MyVar:20:10}} (20-10 and store in MyVar)
			</td>
			<td>
			<i>(Additional examples similar to ADD above)</i>
			</td>
        </tr>
		<tr>
            <td>MUL</td>
            <td>2</td>
			<td>{{FUNC:MUL:MyVar:10:20}} (10*20 and store in MyVar)
			</td>
			<td>
			<i>(Additional examples similar to ADD above)</i></td>
        </tr>
		<tr>
            <td>DIV</td>
            <td>2</td>
			<td>{{FUNC:DIV:MyVar:&#8203;100:50}} (100/50 and store in MyVar).
			</td>
			<td>
			<i>(Additional examples similar to ADD above)</i></td>
        </tr>
		<tr>
            <td>RANDOM</td>
            <td>2</td>
			<td>{{FUNC:RANDOM:MyVar:1:20}} (Find a random number between 1 (inclusive) and 20 (exclusive) and store in MyVar.<br/>
			{{FUNC:RANDOM:MyVar:$FirstVal:$SecondVal}} (Find a random number between FirstVal variable (inclusive) and SecondVal variable (exclusive) and store in MyVar.<br/>
			</td>
			<td>
			<b>Note:</b> First value must be LOWER than Second value.</td>
        </tr>
		<tr>
            <td>CONCAT</td>
            <td>Unlimited</td>
			<td>{{FUNC:CONCAT:MyVar:Hello:World:$Var1:Hi:$Var2}}<br/>
			MyVar will have the string: <b>HelloWorldXXXXHiYYYY</b>
			Where XXXX is the contents of Var1 and YYYY is the contents of Var2
			</td>
			<td>
			</td>
        </tr>
		<tr>
            <td>REPLACE</td>
            <td>3</td>
			<td>{{FUNC:REPLACE:MyVar:Hello:He:Y}}<br/>
			MyVar will have the string: <b>Yello</b><br/><br/>
			{{VARSET:XX:Hello World}} {{VARSET:A:l}}{{VARSET:B:Z}} {{FUNC:REPLACE:MyVar:$XX:$A:$B}}<br/>
			MyVar will have the string: <b>HeZZo WorZd</b>
			</td>
			<td>
			</td>
        </tr>
		<tr>
            <td>NOW</td>
            <td>1</td>
			<td>{{FUNC:NOW:MyVar:yyyy-MM-dd HH:mm:ss}}<br/>
			MyVar will have the current date and time.
			</td>
			<td>
			</td>
        </tr>
	</tbody>
</table>


## Mouse Commands

<table id="mouse" border="1">
    <tbody>
        <tr>
            <th align="center">Mouse Key</th>
            <th align="center">Macro Command</th>
        </tr>
		<tr>
            <td>Mouse Left-Click</td>
            <td>{{LBUTTON}}</td>
        </tr>
		<tr>
            <td>Mouse Left Double-Click</td>
            <td>{{MLEFTDBLCLICK}}</td>
        </tr>
		<tr>
            <td>Mouse Left Button Down</td>
            <td>{{MLEFTDOWN}}</td>
        </tr>
		<tr>
            <td>Mouse Left Button Up</td>
            <td>{{MLEFTUP}}</td>
        </tr>	
		<tr>
            <td>Mouse Right-Click</td>
            <td>{{RBUTTON}}</td>
        </tr>
		<tr>
            <td>Mouse Right Double-Click</td>
            <td>{{MRIGHTDBLCLICK}}</td>
        </tr>
		<tr>
            <td>Mouse Right Button Down</td>
            <td>{{MRIGHTDOWN}}</td>
        </tr>
		<tr>
            <td>Mouse Right Button Up</td>
            <td>{{MRIGHTUP}}</td>
        </tr>
		<tr>
            <td>Mouse Middle Click</td>
            <td>{{MBUTTON}}</td>
        </tr>
		<tr>
            <td>Mouse Middle Button Down</td>
            <td>{{MMIDDLEDOWN}}</td>
        </tr>
		<tr>
            <td>Mouse Middle Button Up</td>
            <td>{{MMIDDLEUP}}</td>
        </tr>
		<tr>
            <td>Mouse Button 4 Click</td>
            <td>{{XBUTTON1}}</td>
        </tr>
		<tr>
            <td>Mouse Button 5 Click</td>
            <td>{{XBUTTON2}}</td>
        </tr>
		<tr>
            <td>Mouse Scroll Wheel Up</td>
            <td>{{MSCROLLUP}}<br/>Optional: Set the number of 'clicks' to scroll up: {{MSCROLLUP:10}} will scroll up 10 clicks</td>
        </tr>
		<tr>
            <td>Mouse Scroll Wheel Down</td>
            <td>{{MSCROLLDOWN}}<br/>Optional: Set the number of 'clicks' to scroll down: {{MSCROLLDOWN:3}} will scroll down 3 clicks</td>
        </tr>
		<tr>
            <td>Mouse Horizontal Scroll Left</td>
            <td>{{MSCROLLLEFT}}<br/>Optional: Set the number of 'clicks' to scroll left: {{MSCROLLLEFT:3}} will scroll left 3 clicks</td>
        </tr>
		<tr>
            <td>Mouse Horizontal Scroll Right</td>
            <td>{{MSCROLLRIGHT}}<br/>Optional: Set the number of 'clicks' to scroll right: {{MSCROLLRIGHT:3}} will scroll right 3 clicks</td>
        </tr>
		<tr>
            <td>Mouse Move based on CURRENT position</td>
            <td>{{MOUSEMOVE:X,Y}} (Move the cursor by X,Y from current position)</td>
        </tr>
		<tr>
            <td>Mouse Move based on multi-screen resolutions</td>
            <td>{{MOUSEXY:X,Y}} (Move the cursor to the X,Y position on the screen. 0,0 is the [top-left] of your primary monitor. Supports both positive and negative values. Use along with the `Mouse Location` action to easily find the right coordinates on your PC<br/>
			Supports variables too: {{MOUSEXY:$Var1,$Var2}}</td>
        </tr>
		<tr>
            <td><b>(DEPRICATED)</b> Mouse Move based on ABSOLUTE position <b>(DEPRICATED)</b> </td>
            <td>{{MOUSEPOS:X,Y}} (Move the cursor to the X,Y position on the screen. Values from 0,0 [top-left] to 65535,65535 [bottom-right])</td>
        </tr>
  </tbody>
</table>
