using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperMacro.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace SuperMacro.Actions
{
    public abstract class KeystrokeBase : PluginBase
    {
        protected abstract class PluginSettingsBase
        {
            [JsonProperty(PropertyName = "command")]
            public string Command { get; set; }

            [JsonProperty(PropertyName = "forcedKeydown")]
            public bool ForcedKeydown { get; set; }

            [JsonProperty(PropertyName = "autoStopNum")]
            public string AutoStopNum { get; set; }

            [JsonProperty(PropertyName = "delay")]
            public string Delay { get; set; }

            [JsonProperty(PropertyName = "isMomentary")]
            public bool IsMomentary { get; set; }
            
        }

        #region Private Members
        protected const int DEFAULT_AUTO_STOP_NUM = 0;
        protected const int DEFAULT_DELAY_MS = 30;

        private readonly InputSimulator iis = new InputSimulator();

        protected int autoStopNum = DEFAULT_AUTO_STOP_NUM;
        protected int delay = DEFAULT_DELAY_MS;
        private int counter;
        #endregion


        #region Protected Members

        protected bool keyPressed = false;
        protected bool forceOneRound = false;
        protected PluginSettingsBase settings;

        #endregion

        #region Public Methods

        public KeystrokeBase(SDConnection connection, InitialPayload payload) : base(connection, payload) { }

        public virtual Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        protected void RunCommand(string commandString)
        {
            try
            {
                if (string.IsNullOrEmpty(commandString))
                {
                    Logger.Instance.LogMessage(TracingLevel.WARN, $"Command not configured");
                    return;
                }
                counter = autoStopNum;

                if (commandString.Length == 1)
                {
                    Task.Run(() => SimulateTextEntry(commandString[0]));
                }
                else // KeyStroke
                {
                    List<VirtualKeyCodeContainer> keyStrokes = CommandTools.ExtractKeyStrokes(commandString);

                    // Actually initiate the keystrokes
                    if (keyStrokes.Count > 0)
                    {
                        VirtualKeyCodeContainer keyCode = keyStrokes.Last();
                        keyStrokes.Remove(keyCode);

                        if (keyStrokes.Count > 0)
                        {
                            Task.Run(() => SimulateKeyStroke(keyStrokes.Select(ks => ks.KeyCode).ToArray(), keyCode.KeyCode));
                        }
                        else
                        {
                            if (keyCode.IsExtended)
                            {
                                Task.Run(() => SimulateExtendedMacro(keyCode));
                            }
                            else
                            {
                                Task.Run(() => SimulateKeyDown(keyCode.KeyCode));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"RunCommand Exception: {ex}");
            }
}

        protected string ValidateKeystroke(string keystroke)
        {
            if (String.IsNullOrEmpty(keystroke))
            {
                return keystroke;
            }

            if (keystroke.Length == 1) // 1 Character is fine
            {
                return keystroke;
            }

            string macro = CommandTools.ExtractMacro(keystroke, 0);
            if (string.IsNullOrEmpty(macro)) // Not a macro, save only first character
            {
                return keystroke[0].ToString();
            }
            else
            {
                // Only returns one macro if there is more than one
                return macro;
            }
        }

        public override void Dispose()
        {
            keyPressed = false;
            Logger.Instance.LogMessage(TracingLevel.INFO, "Destructor called");
        }

        #endregion

        #region Private Methods

        protected virtual void InitializeSettings()
        {
            settings.Command = ValidateKeystroke(settings.Command);
            if (!Int32.TryParse(settings.Delay, out delay))
            {
                settings.Delay = DEFAULT_DELAY_MS.ToString();
            }
            SaveSettings();
        }

        private void SimulateKeyDown(VirtualKeyCode keyCode)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"{this.GetType()} SimulateKeyDown");
            while (keyPressed || forceOneRound)
            {
                forceOneRound = false;
                if (!MouseHandler.HandleMouseMacro(iis, keyCode))
                {
                    iis.Keyboard.KeyDown(keyCode);
                }
                Thread.Sleep(delay);
                HandleAutoStop();
            }
            iis.Keyboard.KeyUp(keyCode); // Release key at the end
        }

        private void SimulateKeyStroke(VirtualKeyCode[] keyStrokes, VirtualKeyCode keyCode)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"{this.GetType()} SimulateKeyStroke. ForcedKeyDown: {settings.ForcedKeydown}");
            while (keyPressed || forceOneRound)
            {
                forceOneRound = false;
                if (settings.ForcedKeydown)
                {
                    foreach(var keystroke in keyStrokes)
                    {
                        iis.Keyboard.KeyDown(keystroke);
                    }
                    iis.Keyboard.KeyDown(keyCode);
                }
                else
                {
                    iis.Keyboard.ModifiedKeyStroke(keyStrokes, keyCode);
                }
                Thread.Sleep(delay);
                HandleAutoStop();
            }

            if (settings.ForcedKeydown)
            {
                iis.Keyboard.KeyUp(keyCode);
                foreach (var keystroke in keyStrokes)
                {
                    iis.Keyboard.KeyUp(keystroke);
                }
            }
        }

        private void SimulateExtendedMacro(VirtualKeyCodeContainer keyCode)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"{this.GetType()} SimulateExtendedMacro");
            while (keyPressed || forceOneRound)
            {
                forceOneRound = false;
                ExtendedMacroHandler.HandleExtendedMacro(iis, keyCode, CreateWriterSettings(), null);
                Thread.Sleep(delay);
                HandleAutoStop();
            }
        }

        private void SimulateTextEntry(char character)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"{this.GetType()} SimulateTextEntry");
            while (keyPressed || forceOneRound)
            {
                forceOneRound = false;
                iis.Keyboard.TextEntry(character);
                Thread.Sleep(delay);
                HandleAutoStop();
            }
        }

        private void HandleAutoStop()
        {
            if (settings.IsMomentary)
                keyPressed = false;
            else if (autoStopNum > 0)
            {
                counter--;
                if (counter <= 0)
                {
                    keyPressed = false;
                }
            }   
        }

        private WriterSettings CreateWriterSettings()
        {
            return new WriterSettings(false, false, false, false, false, false, delay, autoStopNum);
        }



        #endregion
    }
}
