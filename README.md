# SuperMacro - Advanced keystroke macros triggered by the Elgato Stream Deck

**Author's website and contact information:** [https://barraider.github.io](https://barraider.github.io)

## New in v1.6
- Macros can now be loaded directly from a text file (instead of being edited in the Stream Deck App)
    - Plugin will read the file on keypress to ensure the latest version of the macro is executed
- New option to ignore "new line" in textbox and only react to `{{enter}}`. Allows you to write macros on multiple lines to make them more readable

## New in v1.5
- The long awaited ***Variable support*** has now arrived!!!
:new: You can now get input from the user and store it in variables.
For example: Using `{{INPUT:YourName}}Hello {{OUTPUT:YourName}}` will first ask the user for a value, store it in `YourName` and then type it after the **Hello** part. You can use this many times,  and with any different variable names.
- SuperMacro now supports putting a `:` between the command name and value, which allows better readability of your macros. You can now type `{{PAUSE:1000}}`, `{{KEYDOWN:SHIFT}}`,`{{MOUSEMOVE:1,1}}` etc.

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
4. Move the mouse to a certain position on the screen
```
{{MOUSEPOS:40000,15000}}
```
5. Move the mouse by 10 pixels left and 20 pixels down on every press
```
{{MOUSEMOVE:-10,20}}
```
6. Get input from user and then use it later on.
```
{{INPUT:Name}}Hello {{OUTPUT:Name}}, Nice to meet you!
```


## Download
https://github.com/BarRaider/streamdeck-supermacro/releases

## I found a bug, who do I contact?
For support please contact the developer. Contact information is available at https://barraider.github.io

## I have a feature request, who do I contact?
Please contact the developer. Contact information is available at https://barraider.github.io

## Dependencies
This plugin uses the [StreamDeck-Tools](https://github.com/BarRaider/streamdeck-tools) v2.0

## List of supported keystroke commands

<table id="commands" border="1">
    <tbody>
        <tr>
            <th align="center">Keyboard Key</th>
            <th align="center">Macro Command</th>
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
            <td>{PAGEUP} or {PRIOR}</td>
        </tr>
        <tr>
            <td>PAGE DOWN</td>
            <td>{PAGEDOWN} or {NEXT}</td>
        </tr>
        <tr>
            <td>END</td>
            <td>{END}</td>
        </tr>
        <tr>
            <td>HOME</td>
            <td>{HOME}</td>
        </tr>
        <tr>
            <td>LEFT ARROW</td>
            <td>{LEFT}</td>
        </tr>
        <tr>
            <td>UP ARROW</td>
            <td>{UP}</td>
        </tr>
        <tr>
            <td>RIGHT ARROW</td>
            <td>{RIGHT}</td>
        </tr>
        <tr>
            <td>DOWN ARROW</td>
            <td>{DOWN}</td>
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
            <td>DEL</td>
            <td>{DELETE}</td>
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
            <td>Numericpad 0</td>
            <td>{NUMPAD0}</td>
        </tr>
        <tr>
            <td>Numericpad 1</td>
            <td>{NUMPAD1}</td>
        </tr>
        <tr>
            <td>Numericpad 2</td>
            <td>{NUMPAD2}</td>
        </tr>
        <tr>
            <td>Numericpad 3</td>
            <td>{NUMPAD3}</td>
        </tr>
        <tr>
            <td>Numericpad 4</td>
            <td>{NUMPAD4}</td>
        </tr>
        <tr>
            <td>Numericpad 5</td>
            <td>{NUMPAD5}</td>
        </tr>
        <tr>
            <td>Numericpad 6</td>
            <td>{NUMPAD6}</td>
        </tr>
        <tr>
            <td>Numericpad 7</td>
            <td>{NUMPAD7}</td>
        </tr>
        <tr>
            <td>Numericpad 8</td>
            <td>{NUMPAD8}</td>
        </tr>
        <tr>
            <td>Numericpad 9</td>
            <td>{NUMPAD9}</td>
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
            <td>These characters:<br/><b>;/`[\]':?~{|}"</b></td>
            <td>Exact command changes between keyboard layouts:<br/>Try the following macros to figure out the correct command:<br/> <b>{{oem_1}}{{oem_2}}{{oem_3}}{{oem_4}}{{oem_5}} {{oem_6}}{{oem_7}}{{oem_8}}
			{{shift}{oem_1}}{{shift}{oem_2}}{{shift}{oem_3}} {{shift}{oem_4}}{{shift}{oem_5}} {{shift}{oem_6}}{{shift}{oem_7}}{{shift}{oem_8}}</b></td>
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
            <td>PAUSE</td>
            <td>{PAUSE:XXXX} (XXXX = length in miliseconds)</td>
        </tr>
		<tr>
            <td>KeyDown</td>
            <td>{KeyDown:XXXX} (XXXX = name of key, example {{KeyDown:F1}})</td>
        </tr>
		<tr>
            <td>KeyUp</td>
            <td>{KeyUp:XXXX} (XXXX = name of key, example {{KeyUp:SHIFT}})</td>
		</tr>
		<tr>
			<td>Input</td>
			<td>{Input:VarName} Get input from the user and store it in 'VarName'.</td>
		</tr>
		<tr>
			<td>Output</td>
			<td>{Output:MyVar} Output the input previously gathered into 'MyVar'.</td>
		</tr>
	</tbody>
</table>


## Mouse Commands

<table id="mouse" border="1">
    <tbody>
        <tr>
            <th align="center">Keyboard Key</th>
            <th align="center">Macro Command</th>
        </tr>
		<tr>
            <td>Mouse Left-Click</td>
            <td>{LBUTTON}</td>
        </tr>
		<tr>
            <td>Mouse Left Double-Click</td>
            <td>{XBUTTON1}</td>
        </tr>
		<tr>
            <td>Mouse Left Button Down</td>
            <td>{MLEFTDOWN}</td>
        </tr>
		<tr>
            <td>Mouse Left Button Up</td>
            <td>{MLEFTUP}</td>
        </tr>	
		<tr>
            <td>Mouse Right-Click</td>
            <td>{RBUTTON}</td>
        </tr>
		<tr>
            <td>Mouse Right Double-Click</td>
            <td>{XBUTTON2}</td>
        </tr>
		<tr>
            <td>Mouse Right Button Down</td>
            <td>{MRIGHTDOWN}</td>
        </tr>
		<tr>
            <td>Mouse Right Button Up</td>
            <td>{MRIGHTUP}</td>
        </tr>
		<tr>
            <td>Mouse Middle Click</td>
            <td>{MBUTTON}</td>
        </tr>
		<tr>
            <td>Mouse Middle Button Down</td>
            <td>{MMIDDLEDOWN}</td>
        </tr>
		<tr>
            <td>Mouse Middle Button Up</td>
            <td>{MMIDDLEUP}</td>
        </tr>
		<tr>
            <td>Mouse Scroll Wheel Up</td>
            <td>{MSCROLLUP}</td>
        </tr>
		<tr>
            <td>Mouse Scroll Wheel Down</td>
            <td>{MSCROLLDOWN}</td>
        </tr>
		<tr>
            <td>Mouse Horizontal Scroll Left</td>
            <td>{MSCROLLLEFT}</td>
        </tr>
		<tr>
            <td>Mouse Horizontal Scroll Right</td>
            <td>{MSCROLLRIGHT}</td>
        </tr>
		<tr>
            <td>Mouse Move: Based on CURRENT position</td>
            <td>{MOUSEMOVE:X,Y} (Move the cursor by X,Y from current position)</td>
        </tr>
		<tr>
            <td>Mouse Move: based on ABSOLUTE position </td>
            <td>{MOUSEPOS:X,Y} (Move the cursor to the X,Y position on the screen. Values from 0,0 [top-left] to 65535,65535 [bottom-right])</td>
        </tr>
  </tbody>
</table>
