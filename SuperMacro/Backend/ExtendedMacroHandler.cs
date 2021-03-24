using BarRaider.SdTools;
using SuperMacro.Actions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace SuperMacro.Backend
{
    //---------------------------------------------------
    //          BarRaider's Hall Of Fame
    // Subscriber: Geekie_Benji
    // "Anonymous" - Tip: $10.10
    // Havocxnoodles - 5 Gifted Subs
    // Subscriber: CyberlightGames
    // Subscriber: kermit_the_frog
    // Subscriber: BuSheeZy
    //---------------------------------------------------   
    internal static class ExtendedMacroHandler
    {
        private enum ExtendedCommand
        {
            EXTENDED_MACRO_PAUSE,
            EXTENDED_MACRO_KEY_DOWN,
            EXTENDED_MACRO_KEY_UP,
            EXTENDED_MACRO_MOUSE_MOVE, // Update in HandleExtendedMacro if adding more mouse commands BEFORE this one
            EXTENDED_MACRO_MOUSE_POS,
            EXTENDED_MACRO_MOUSE_XY,
            EXTENDED_MACRO_SCROLL_UP,
            EXTENDED_MACRO_SCROLL_DOWN,
            EXTENDED_MACRO_SCROLL_LEFT,
            EXTENDED_MACRO_SCROLL_RIGHT,
            EXTENDED_MACRO_MOUSE_LEFT_DOWN,
            EXTENDED_MACRO_MOUSE_LEFT_UP,
            EXTENDED_MACRO_MOUSE_RIGHT_DOWN,
            EXTENDED_MACRO_MOUSE_RIGHT_UP,
            EXTENDED_MACRO_MOUSE_MIDDLE_DOWN,
            EXTENDED_MACRO_MOUSE_MIDDLE_UP,
            EXTENDED_MACRO_MOUSE_LEFT_DOUBLE_CLICK,
            EXTENDED_MACRO_MOUSE_RIGHT_DOUBLE_CLICK,
            EXTENDED_MACRO_MOUSE_STORE_LOCATION,
            EXTENDED_MACRO_MOUSE_RESTORE_LOCATION, // Update in HandleExtendedMacro if adding more mouse commands AFTER this one
            EXTENDED_MACRO_VARIABLE_INPUT = 20,
            EXTENDED_MACRO_VARIABLE_OUTPUT = 21,
            EXTENDED_MACRO_VARIABLE_UNSETALL = 22,
            EXTENDED_MACRO_VARIABLE_UNSET = 23,
            EXTENDED_MACRO_VARIABLE_SET = 24,
            EXTENDED_MACRO_VARIABLE_SET_FROM_FILE = 25,
            EXTENDED_MACRO_VARIABLE_SET_FROM_CLIPBOARD = 26,
            EXTENDED_MACRO_VARIABLE_OUTPUT_TO_FILE = 27,
            EXTENDED_MACRO_FUNCTIONS = 28,
            EXTENDED_MACRO_STREAMDECK_SETKEYTITLE = 29,
            EXTENDED_MACRO_STREAMDECK_SETCLIPBOARD = 30
        }

        private static readonly string[] EXTENDED_COMMANDS_LIST = { "PAUSE", "KEYDOWN", "KEYUP", "MOUSEMOVE", "MOUSEPOS", "MOUSEXY", "MSCROLLUP", "MSCROLLDOWN", "MSCROLLLEFT", "MSCROLLRIGHT", "MLEFTDOWN", "MLEFTUP", "MRIGHTDOWN", "MRIGHTUP", "MMIDDLEDOWN", "MMIDDLEUP", "MLEFTDBLCLICK", "MRIGHTDBLCLICK", "MSAVEPOS", "MLOADPOS", "INPUT", "OUTPUT", "VARUNSETALL", "VARUNSET", "VARSET", "VARSETFROMFILE", "VARSETFROMCLIPBOARD", "OUTPUTTOFILE", "FUNC", "SETKEYTITLE", "SETCLIPBOARD" };
        private const string MOUSE_STORED_X_VARIABLE = "MOUSE_X";
        private const string MOUSE_STORED_Y_VARIABLE = "MOUSE_Y";
        private const char SUPERMACRO_EXTENDED_COMMAND_DELIMITER = ':';
        private const char SUPERMACRO_VARIABLE_PREFIX = '$';

        public delegate void SetKeyTitle(string title);

        private static readonly Dictionary<VirtualKeyCode, bool> dicRepeatKeydown = new Dictionary<VirtualKeyCode, bool>();
        private static readonly Dictionary<string, string> dicVariables = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> dicPresetVariables = new Dictionary<string, string>()
        {
            { "SMCOLON", ":" },
            { "SMLCB", "{" },
            { "SMRCB", "}" },
            { "SMENTER", "\n" },
            { "SMDOLLAR", "$" }
        };

        public static bool IsExtendedMacro(string macroText, out string macroCommand, out string extendedData)
        {
            extendedData = String.Empty;
            macroCommand = null;
            string[] upperMacroText = macroText.ToUpperInvariant().Split(SUPERMACRO_EXTENDED_COMMAND_DELIMITER);

            foreach (string command in EXTENDED_COMMANDS_LIST)
            {
                if (upperMacroText[0] == command)
                {
                    macroCommand = command;
                    if (macroText.Length > command.Length)
                    {
                        extendedData = macroText.Substring(command.Length);

                        // Handle delimiter
                        if (extendedData.StartsWith(SUPERMACRO_EXTENDED_COMMAND_DELIMITER.ToString()))
                        {
                            extendedData = extendedData.Substring(SUPERMACRO_EXTENDED_COMMAND_DELIMITER.ToString().Length);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if the variableString is an actual variable in the dictionary and starting with a $ sign.
        /// If it is - return that value.
        /// Otherwise, return the original string
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="dicVariables"></param>
        /// <returns></returns>
        public static string TryExtractVariable(string variableString)
        {
            if (String.IsNullOrEmpty(variableString))
            {
                return variableString;
            }

            if (variableString[0] != SUPERMACRO_VARIABLE_PREFIX)
            {
                return variableString;
            }

            // Remove the '$' sign and make it uppercase
            string variableName = variableString.Substring(1).ToUpperInvariant();

            // Check if it's a preset variable
            if (dicPresetVariables.ContainsKey(variableName))
            {
                return dicPresetVariables[variableName];
            }

            if (!dicVariables.ContainsKey(variableName))
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, $"HandleFunctionRequest TryExtractVariable: Variable does not exist {variableName}");
                return variableString;
            }

            return dicVariables[variableName];
        }

        public static async Task HandleExtendedMacro(InputSimulator iis, VirtualKeyCodeContainer macro, WriterSettings settings, SetKeyTitle SetKeyTitleFunction)
        {
            try
            {
                // Index in array
                int index = EXTENDED_COMMANDS_LIST.ToList().FindIndex(cmd => cmd == macro.ExtendedCommand);
                ExtendedCommand command = (ExtendedCommand)index;

                // Check if this is a function command
                if (command == ExtendedCommand.EXTENDED_MACRO_FUNCTIONS)
                {
                    FunctionsHandler.HandleFunctionRequest(macro.ExtendedData, dicVariables);
                }
                // Check if it's a pause command
                else if (command == ExtendedCommand.EXTENDED_MACRO_PAUSE)
                {
                    string pauseLengthParam = TryExtractVariable(macro.ExtendedData);
                    if (Int32.TryParse(pauseLengthParam, out int pauseLength))
                    {
                        Thread.Sleep(pauseLength);
                        return;
                    }
                }
                // Keyboard commands
                else if (command == ExtendedCommand.EXTENDED_MACRO_KEY_DOWN || command == ExtendedCommand.EXTENDED_MACRO_KEY_UP)
                {
                    HandleKeyboardCommand(iis, macro);
                    return;
                }
                // Mouse commands
                else if (index >= (int)ExtendedCommand.EXTENDED_MACRO_MOUSE_MOVE && index <= (int)ExtendedCommand.EXTENDED_MACRO_MOUSE_RESTORE_LOCATION)
                {
                    HandleMouseCommand(command, iis, macro);
                }
                else if (command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_INPUT || command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_OUTPUT ||
                         command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_UNSETALL || command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_UNSET ||
                         command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_SET || command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_SET_FROM_FILE ||
                         command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_SET_FROM_CLIPBOARD || command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_OUTPUT_TO_FILE)
                {
                    await HandleVariableCommand(command, macro, settings);
                }
                else if (command == ExtendedCommand.EXTENDED_MACRO_STREAMDECK_SETKEYTITLE)
                {
                    if (SetKeyTitleFunction == null)
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"SETKEYTITLE called but callback function is null");
                        return;
                    }
                    string titleString = TryExtractVariable(macro.ExtendedData).Replace(@"\n", "\n");
                    SetKeyTitleFunction(titleString);
                }
                else if (command == ExtendedCommand.EXTENDED_MACRO_STREAMDECK_SETCLIPBOARD)
                {
                    SetClipboard(TryExtractVariable(macro.ExtendedData));
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"HandleExtendedMacro - Invalid command {command}");
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"Failed to parse extended macro: {macro?.ExtendedCommand} {macro?.ExtendedData} {ex}");
            }
        }

        private static void HandleKeyboardCommand(InputSimulator iis, VirtualKeyCodeContainer macro)
        {
            string commandText = CommandTools.ConvertSimilarMacroCommands(macro.ExtendedData.ToUpperInvariant());
            if (string.IsNullOrEmpty(commandText))
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"Extended Keydown/Keyup - Missing Command");
                return;
            }

            if (!Enum.TryParse<VirtualKeyCode>(commandText, true, out VirtualKeyCode code))
            {
                if (commandText.Length > 1)
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"Extended Keydown/Keyup Shrinking {commandText} to {commandText[0]}");
                }
                code = (VirtualKeyCode)commandText[0];
            }

            if (macro.ExtendedCommand == EXTENDED_COMMANDS_LIST[(int)ExtendedCommand.EXTENDED_MACRO_KEY_DOWN])
            {
                RepeatKeyDown(iis, code);
                //iis.Keyboard.KeyDown(code);
            }
            else
            {
                dicRepeatKeydown[code] = false;
                //iis.Keyboard.KeyUp(code);
            }
        }

        private static void HandleMouseCommand(ExtendedCommand command, InputSimulator iis, VirtualKeyCodeContainer macro)
        {
            // Mouse Move commands
            if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_MOVE || command == ExtendedCommand.EXTENDED_MACRO_MOUSE_POS ||
                command == ExtendedCommand.EXTENDED_MACRO_MOUSE_XY ||
                command == ExtendedCommand.EXTENDED_MACRO_MOUSE_STORE_LOCATION || command == ExtendedCommand.EXTENDED_MACRO_MOUSE_RESTORE_LOCATION)
            {
                HandleMouseMoveCommand(command, iis, macro);
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_SCROLL_UP || command == ExtendedCommand.EXTENDED_MACRO_SCROLL_DOWN || command == ExtendedCommand.EXTENDED_MACRO_SCROLL_LEFT ||
                    command == ExtendedCommand.EXTENDED_MACRO_SCROLL_RIGHT)
            {
                HandleMouseScrollCommand(command, iis, macro);
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_LEFT_DOWN ||
                     command == ExtendedCommand.EXTENDED_MACRO_MOUSE_LEFT_UP ||
                     command == ExtendedCommand.EXTENDED_MACRO_MOUSE_RIGHT_DOWN ||
                     command == ExtendedCommand.EXTENDED_MACRO_MOUSE_RIGHT_UP ||
                     command == ExtendedCommand.EXTENDED_MACRO_MOUSE_MIDDLE_DOWN ||
                     command == ExtendedCommand.EXTENDED_MACRO_MOUSE_MIDDLE_UP ||
                     command == ExtendedCommand.EXTENDED_MACRO_MOUSE_LEFT_DOUBLE_CLICK ||
                     command == ExtendedCommand.EXTENDED_MACRO_MOUSE_RIGHT_DOUBLE_CLICK)
            {
                HandleMouseButtonCommand(command, iis);
            }
            else
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"HandleMouseCommand - Invalid command {command}");
            }

        }

        private static void HandleMouseMoveCommand(ExtendedCommand command, InputSimulator iis, VirtualKeyCodeContainer macro)
        {
            if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_MOVE || command == ExtendedCommand.EXTENDED_MACRO_MOUSE_POS ||
                command == ExtendedCommand.EXTENDED_MACRO_MOUSE_XY)  // Mouse Move
            {
                string[] mousePos = macro.ExtendedData.Split(',');
                if (mousePos.Length != 2)
                {
                    mousePos = macro.ExtendedData.Split(':');
                }
                if (mousePos.Length == 2)
                {
                    string mouseX = TryExtractVariable(mousePos[0]);
                    string mouseY = TryExtractVariable(mousePos[1]);

                    if (Double.TryParse(mouseX, out double x) && Double.TryParse(mouseY, out double y))
                    {
                        if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_POS)
                        {
                            iis.Mouse.MoveMouseToPositionOnVirtualDesktop(x, y);
                        }
                        else if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_XY)
                        {
                            System.Windows.Forms.Cursor.Position = new Point((int)x, (int)y);
                        }
                        else
                        {
                            iis.Mouse.MoveMouseBy((int)x, (int)y);
                        }
                    }
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, $"HandleMouseMoveCommand - Invalid parameter {macro.ExtendedData}");
                }
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_STORE_LOCATION) // Mouse Store
            {
                Point point = System.Windows.Forms.Cursor.Position;
                dicVariables[MOUSE_STORED_X_VARIABLE] = point.X.ToString();
                dicVariables[MOUSE_STORED_Y_VARIABLE] = point.Y.ToString();
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_RESTORE_LOCATION) // Mouse Restore
            {
                if (!dicVariables.ContainsKey(MOUSE_STORED_X_VARIABLE) || !dicVariables.ContainsKey(MOUSE_STORED_Y_VARIABLE))
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"Restore Mouse called but no variables assigned");
                    return;
                }


                int.TryParse(dicVariables[MOUSE_STORED_X_VARIABLE], out int mouseX);
                int.TryParse(dicVariables[MOUSE_STORED_Y_VARIABLE], out int mouseY);
                System.Windows.Forms.Cursor.Position = new Point(mouseX, mouseY);
            }
            else
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"HandleMouseMoveCommand - Invalid command {command}");
            }
            return;
        }

        private static void HandleMouseScrollCommand(ExtendedCommand command, InputSimulator iis, VirtualKeyCodeContainer macro)
        {
            // Scroll UP/DOWN/LEFT/RIGHT
            if (command == ExtendedCommand.EXTENDED_MACRO_SCROLL_UP || command == ExtendedCommand.EXTENDED_MACRO_SCROLL_DOWN)
            {
                int direction = (command == ExtendedCommand.EXTENDED_MACRO_SCROLL_UP) ? 1 : -1;
                if (Int32.TryParse(macro.ExtendedData, out int mouseClicks) && mouseClicks > 0)
                {
                    direction *= mouseClicks;
                }
                iis.Mouse.VerticalScroll(direction);
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_SCROLL_LEFT || command == ExtendedCommand.EXTENDED_MACRO_SCROLL_RIGHT)
            {
                int direction = (macro.ExtendedCommand == EXTENDED_COMMANDS_LIST[(int)ExtendedCommand.EXTENDED_MACRO_SCROLL_RIGHT]) ? 1 : -1;
                if (Int32.TryParse(macro.ExtendedData, out int mouseClicks) && mouseClicks > 0)
                {
                    direction *= mouseClicks;
                }
                iis.Mouse.HorizontalScroll(direction);
            }
            else
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"HandleMouseScrollCommand - Invalid command {command}");
            }
        }

        private static void HandleMouseButtonCommand(ExtendedCommand command, InputSimulator iis)
        {
            if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_LEFT_DOWN)
            {
                iis.Mouse.LeftButtonDown();
                return;
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_LEFT_UP)
            {
                iis.Mouse.LeftButtonUp();
                return;
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_RIGHT_DOWN)
            {
                iis.Mouse.RightButtonDown();
                return;
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_RIGHT_UP)
            {
                iis.Mouse.RightButtonUp();
                return;
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_MIDDLE_DOWN)
            {
                iis.Mouse.MiddleButtonDown();
                return;
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_MIDDLE_UP)
            {
                iis.Mouse.MiddleButtonUp();
                return;
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_LEFT_DOUBLE_CLICK)
            {
                iis.Mouse.LeftButtonDoubleClick();
                return;
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_MOUSE_RIGHT_DOUBLE_CLICK)
            {
                iis.Mouse.RightButtonDoubleClick();
                return;
            }
            else
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"HandleMouseButtonCommand - Invalid command {command}");
            }
        }

        private static async Task HandleVariableCommand(ExtendedCommand command, VirtualKeyCodeContainer macro, WriterSettings settings)
        {
            string upperExtendedData = macro.ExtendedData.ToUpperInvariant();

            // Variables
            if (command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_INPUT)
            {
                string defaultValue = String.Empty;
                if (!String.IsNullOrEmpty(upperExtendedData) && dicVariables.ContainsKey(upperExtendedData))
                {
                    defaultValue = dicVariables[upperExtendedData];
                }

                using InputBox input = new InputBox("Variable Input", $"Enter value for \"{upperExtendedData}\":", defaultValue);
                input.ShowDialog();

                // Value exists (cancel button was NOT pressed)
                if (!string.IsNullOrEmpty(input.Input))
                {
                    dicVariables[upperExtendedData] = input.Input;
                }
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_OUTPUT)
            {
                if (dicVariables.ContainsKey(upperExtendedData))
                {
                    SuperMacroWriter textWriter = new SuperMacroWriter();
                    await textWriter.SendInput(dicVariables[upperExtendedData], settings, null, false);
                }
                else if (dicPresetVariables.ContainsKey(upperExtendedData))
                {
                    SuperMacroWriter textWriter = new SuperMacroWriter();
                    await textWriter.SendInput(dicPresetVariables[upperExtendedData], settings, null, false);
                }
                else
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"Variable Output called for {upperExtendedData} without an Input beforehand");
                }
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_UNSETALL)
            {
                dicVariables.Clear();
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_UNSET)
            {
                if (string.IsNullOrEmpty(upperExtendedData))
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"Variable Unset called without variable name");
                    return;
                }
                if (dicVariables.ContainsKey(upperExtendedData))
                {
                    dicVariables.Remove(upperExtendedData);
                }
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_SET || command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_SET_FROM_FILE)
            {
                if (string.IsNullOrEmpty(macro.ExtendedData))
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"Variable Set called without variable name");
                    return;
                }

                // last parameter "2" ensures maximum of two splits!
                var splitData = macro.ExtendedData.Split(new char[] { ':' }, 2);
                if (splitData.Length != 2)
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"Variable Set called without incorrect extended data: {macro.ExtendedData}");
                    return;
                }
                string varInput = splitData[1];
                string fileName = TryExtractVariable(splitData[1]);

                // Set From File but file doesn't exist
                if (command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_SET_FROM_FILE && !File.Exists(fileName))
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"Variable SetFromFile called but file does not exist {fileName}");
                    return;
                }
                else if (command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_SET_FROM_FILE) // File DOES exist
                {
                    varInput = File.ReadAllText(fileName);
                }

                dicVariables[splitData[0].ToUpperInvariant()] = varInput;
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_SET_FROM_CLIPBOARD)
            {
                var value = ReadFromClipboard();

                // Value exists (cancel button was NOT pressed)
                if (!string.IsNullOrEmpty(value))
                {
                    dicVariables[upperExtendedData] = value;
                }
            }
            else if (command == ExtendedCommand.EXTENDED_MACRO_VARIABLE_OUTPUT_TO_FILE)
            {
                if (string.IsNullOrEmpty(macro.ExtendedData))
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"OutputToFile called without any params");
                    return;
                }

                var splitData = macro.ExtendedData.Split(new char[] { ':' }, 2);
                if (splitData.Length != 2)
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"OutputToFile called without incorrect extended data: {macro.ExtendedData}");
                    return;
                }

                string variableName = splitData[0].ToUpperInvariant();
                string fileName = TryExtractVariable(splitData[1]);

                // Check if variable exists
                if (!dicVariables.ContainsKey(variableName))
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"OutputToFile called without non existing variable: {variableName}");
                    return;
                }

                // Try to save the data in the variable to the filename
                try
                {
                    File.WriteAllText(fileName, dicVariables[variableName]);
                }
                catch (Exception ex)
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"OutputToFile exception: {macro.ExtendedData} {ex}");
                }
                return;
            }
            else
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"HandleVariableCommand - Invalid command {command}");
            }
        }

        private static void RepeatKeyDown(InputSimulator iis, VirtualKeyCode code)
        {
            dicRepeatKeydown[code] = true;

            Task.Run(() =>
            {
                while (dicRepeatKeydown[code])
                {
                    iis.Keyboard.KeyDown(code);
                    Thread.Sleep(30);
                }
                iis.Keyboard.KeyUp(code);
            });
        }

        private static String ReadFromClipboard()
        {
            string result = null;
            Thread staThread = new Thread(
                delegate ()
                {
                    try
                    {
                        result = Clipboard.GetText(System.Windows.Forms.TextDataFormat.Text);
                    }

                    catch (Exception ex)
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"ReadFromClipboard exception: {ex}");
                    }
                });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
            return result;
        }

        private static void SetClipboard(string text)
        {
            Thread staThread = new Thread(
                delegate ()
                {
                    try
                    {
                        Clipboard.SetText(text);
                    }

                    catch (Exception ex)
                    {
                        Logger.Instance.LogMessage(TracingLevel.ERROR, $"SetClipboard exception: {ex}");
                    }
                });
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
            staThread.Join();
        }
    }
}
