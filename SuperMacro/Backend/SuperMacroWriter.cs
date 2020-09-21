using BarRaider.SdTools;
using SuperMacro.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using static SuperMacro.Backend.ExtendedMacroHandler;

namespace SuperMacro.Backend
{
    internal class SuperMacroWriter
    {
        private const string COMMENT_MACRO = "{{//}}";

        public bool InputRunning { get; set; }
        public bool ForceStop { get; set;}

        public bool StickyEnabled { get; set; }


        public async Task SendInput(string inputText, WriterSettings settings, SetKeyTitle setKeyTitleFunction, bool areMacrosSupported = true)
        {
            if (String.IsNullOrEmpty(inputText))
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, $"SendInput: Text is null");
                return;
            }

            if (settings == null)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"SendInput: Settings is null");
                return;
            }

            InputRunning = true;
            await Task.Run(async () =>
            {
                InputSimulator iis = new InputSimulator();
                string text = inputText;

                if (settings.IgnoreNewline)
                {
                    text = text.Replace("\r\n", "\n").Replace("\n", "");
                }
                else if (settings.EnterMode)
                {
                    text = text.Replace("\r\n", "\n");
                }

                for (int idx = 0; idx < text.Length && !ForceStop; idx++)
                {
                    if (settings.EnterMode && text[idx] == '\n')
                    {
                        iis.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
                    }
                    else if (text[idx] == CommandTools.MACRO_START_CHAR)
                    {
                        string macro = CommandTools.ExtractMacro(text, idx);
                        if (!areMacrosSupported || String.IsNullOrWhiteSpace(macro)) // Not a macro, just input the character
                        {
                            InputChar(iis, text[idx], settings);
                        }
                        else if (macro == COMMENT_MACRO) // Comment, ignore everything until the next newline
                        {
                            var newPos = text.IndexOf('\n', idx);
                            // If no newline exists, skip the entire rest of the text
                            if (newPos < 0)
                            {
                                newPos = text.Length;
                            }
                            idx = newPos;
                        }
                        else // This is a macro, run it
                        {
                            idx += macro.Length - 1;
                            macro = macro.Substring(1, macro.Length - 2);

                            await HandleMacro(macro, settings, setKeyTitleFunction);
                        }
                    }
                    else
                    {
                        InputChar(iis, text[idx], settings);
                    }
                    Thread.Sleep(settings.Delay);
                }
            });
            InputRunning = false;
        }

        public async void SendStickyInput(string inputText, WriterSettings settings, SetKeyTitle setKeyTitleFunction, bool areMacrosSupported = true)
        {
            if (String.IsNullOrEmpty(inputText))
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, $"SendStickyInput: Text is null");
                return;
            }

            if (settings == null)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"SendStickyInput: Settings is null");
                return;
            }

            InputRunning = true;
            await Task.Run(async () =>
            {
                InputSimulator iis = new InputSimulator();
                string text = inputText;

                if (settings.IgnoreNewline)
                {
                    text = text.Replace("\r\n", "\n").Replace("\n", "");
                }
                else if (settings.EnterMode)
                {
                    text = text.Replace("\r\n", "\n");
                }

                int autoStopNum = settings.AutoStopNum;
                bool isAutoStopMode = autoStopNum > 0;
                int counter = autoStopNum;
                while (StickyEnabled)
                {
                    for (int idx = 0; idx < text.Length; idx++)
                    {
                        if (!StickyEnabled && !settings.RunUntilEnd) // Stop as soon as user presses button
                        {
                            break;
                        }
                        if (settings.EnterMode && text[idx] == '\n')
                        {
                            iis.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
                        }
                        else if (text[idx] == CommandTools.MACRO_START_CHAR)
                        {
                            string macro = CommandTools.ExtractMacro(text, idx);
                            if (!areMacrosSupported || String.IsNullOrWhiteSpace(macro)) // Not a macro, just input the character
                            {
                                iis.Keyboard.TextEntry(text[idx]);
                            }
                            else // This is a macro, run it
                            {
                                idx += macro.Length - 1;
                                macro = macro.Substring(1, macro.Length - 2);

                                await HandleMacro(macro, settings, setKeyTitleFunction);
                            }
                        }
                        else
                        {
                            iis.Keyboard.TextEntry(text[idx]);
                        }
                        Thread.Sleep(settings.Delay);
                    }
                    if (isAutoStopMode)
                    {
                        counter--; // First decrease, then check if equals zero 
                        if (counter <= 0)
                        {
                            StickyEnabled = false;
                        }
                    }

                }
            });
            InputRunning = false;
        }

        protected async Task HandleMacro(string macro, WriterSettings settings, SetKeyTitle setKeyTitleFunction)
        {
            List<VirtualKeyCodeContainer> keyStrokes = CommandTools.ExtractKeyStrokes(macro);

            // Actually initiate the keystrokes
            if (keyStrokes.Count > 0)
            {
                InputSimulator iis = new InputSimulator();
                VirtualKeyCodeContainer keyCode = keyStrokes.Last();
                keyStrokes.Remove(keyCode);

                if (keyStrokes.Count > 0)
                {
                    if (settings.KeydownDelay)
                    {
                        Logger.Instance.LogMessage(TracingLevel.INFO, $"{this.GetType()} DelayedModifiedKeyStroke");
                        iis.Keyboard.DelayedModifiedKeyStroke(keyStrokes.Select(ks => ks.KeyCode).ToArray(), new VirtualKeyCode[] { keyCode.KeyCode }, settings.Delay);
                    }
                    else
                    {
                        Logger.Instance.LogMessage(TracingLevel.INFO, $"{this.GetType()} ModifiedKeyStroke");
                        iis.Keyboard.ModifiedKeyStroke(keyStrokes.Select(ks => ks.KeyCode).ToArray(), keyCode.KeyCode);
                    }
                }
                else // Single Keycode
                {
                    if (keyCode.IsExtended)
                    {
                        Logger.Instance.LogMessage(TracingLevel.INFO, $"{this.GetType()} HandleExtendedMacro");
                        await ExtendedMacroHandler.HandleExtendedMacro(iis, keyCode, settings, setKeyTitleFunction);
                    }
                    else // Normal single keycode
                    {
                        if (!MouseHandler.HandleMouseMacro(iis, keyCode.KeyCode))
                        {
                            Logger.Instance.LogMessage(TracingLevel.INFO, $"{this.GetType()} KeyPress");
                            iis.Keyboard.KeyPress(keyCode.KeyCode);
                        }
                    }
                }
            }
        }

        protected void InputChar(InputSimulator iis, char c, WriterSettings settings)
        {
            if (settings.ForcedMacro)
            {
                VirtualKeyCode vk = VirtualKeyCode.LBUTTON;
                iis.Keyboard.KeyPress(vk.FromChar(c));
            }
            else
            {
                iis.Keyboard.TextEntry(c);
            }
        }

    }
}
