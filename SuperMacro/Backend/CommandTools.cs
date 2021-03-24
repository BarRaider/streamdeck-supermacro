using BarRaider.SdTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace SuperMacro.Backend
{
    internal static class CommandTools
    {
        internal const char MACRO_START_CHAR = '{';
        internal const string MACRO_END = "}}";
        internal const string REGEX_MACRO = @"^\{(\{[^\{\}]+\})+\}$";
        internal const string REGEX_SUB_COMMAND = @"(\{[^\{\}]+\})";
        internal const int RECOMMENDED_KEYDOWN_DELAY = 50;

        internal static string ExtractMacro(string text, int position)
        {
            try
            {
                int endPosition = text.IndexOf(CommandTools.MACRO_END, position);

                // Found an end, let's verify it's actually a macro
                if (endPosition > position)
                {
                    // Use Regex to verify it's really a macro
                    var match = Regex.Match(text.Substring(position, endPosition - position + CommandTools.MACRO_END.Length), CommandTools.REGEX_MACRO);
                    if (match.Length > 0)
                    {
                        return match.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"ExtractMacro Exception: {ex}");
            }

            return null;
        }

        internal static List<VirtualKeyCodeContainer> ExtractKeyStrokes(string macroText)
        {
            List<VirtualKeyCodeContainer> keyStrokes = new List<VirtualKeyCodeContainer>();


            try
            {
                MatchCollection matches = Regex.Matches(macroText, CommandTools.REGEX_SUB_COMMAND);
                foreach (var match in matches)
                {
                    string matchText = match.ToString().Replace("{", "").Replace("}", "");
                    if (matchText.Length == 1)
                    {
                        char code = matchText.ToUpperInvariant()[0];
                        keyStrokes.Add(new VirtualKeyCodeContainer((VirtualKeyCode)code));
                    }
                    else
                    {
                        VirtualKeyCodeContainer stroke = CommandTools.MacroTextToKeyCode(matchText);
                        if (stroke != null)
                        {
                            keyStrokes.Add(stroke);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"ExtractKeyStrokes Exception: {ex}");
            }

            return keyStrokes;
        }

        private static VirtualKeyCodeContainer MacroTextToKeyCode(string macroText)
        {
            try
            {  
                if (ExtendedMacroHandler.IsExtendedMacro(macroText, out string macroCommand, out string extendedData))
                {
                    return new VirtualKeyCodeContainer(VirtualKeyCode.ZOOM, macroCommand, extendedData);
                }

                string text = ConvertSimilarMacroCommands(macroText.ToUpperInvariant());
                VirtualKeyCode keyCode = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), text, true);
                return new VirtualKeyCodeContainer(keyCode);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"MacroTextToInt Exception: {ex}");
                return null;
            }
        }

        internal static string ConvertSimilarMacroCommands(string macroText)
        {
            switch (macroText)
            {
                case "CTRL":
                    return "CONTROL";
                case "LCTRL":
                    return "LCONTROL";
                case "RCTRL":
                    return "RCONTROL";
                case "ALT":
                    return "MENU";
                case "LALT":
                    return "LMENU";
                case "RALT":
                    return "RMENU";
                case "ENTER":
                    return "RETURN";
                case "BACKSPACE":
                    return "BACK";
                case "WIN":
                    return "LWIN";
                case "WINDOWS":
                    return "LWIN";
                case "PAGEUP":
                case "PGUP":
                    return "PRIOR";
                case "PAGEDOWN":
                case "PGDN":
                    return "NEXT";
                case "BREAK":
                    return "PAUSE";
                case "ARROWDOWN":
                    return "DOWN";
                case "ARROWUP":
                    return "UP";
                case "ARROWRIGHT":
                    return "RIGHT";
                case "ARROWLEFT":
                    return "LEFT";
            };

            return macroText;
        }
    }
}
