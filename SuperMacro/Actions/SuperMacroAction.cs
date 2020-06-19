using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperMacro.Backend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace SuperMacro.Actions
{
    [PluginActionId("com.barraider.supermacro")]
    public class SuperMacroAction : SuperMacroBase
    {
        protected class PluginSettings : MacroSettingsBase
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings
                {
                    InputText = String.Empty,
                    LongPressInputText = String.Empty,
                    Delay = 10,
                    EnterMode = false,
                    ForcedMacro = false,
                    KeydownDelay = false,
                    IgnoreNewline = false,
                    LoadFromFiles = false,
                    PrimaryInputFile = String.Empty,
                    SecondaryInputFile = String.Empty
                };

                return instance;
            }

            [FilenameProperty]
            [JsonProperty(PropertyName = "secondaryInputFile")]
            public string SecondaryInputFile { get; set; }

            [JsonProperty(PropertyName = "longPressInputText")]
            public string LongPressInputText { get; set; }
        }

        protected PluginSettings Settings
        {
            get
            {
                var result = settings as PluginSettings;
                if (result == null)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, "Cannot convert MacroSettingsBase to PluginSettings");
                }
                return result;
            }
            set
            {
                settings = value;
            }
        }

        #region Private Members

        private const int LONG_KEYPRESS_LENGTH = 600;

        private bool keyPressed = false;
        private DateTime keyPressStart;
        private bool longKeyPressed = false;
        private string secondaryMacro;

        #endregion

        #region Public Methods

        public SuperMacroAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                Settings = PluginSettings.CreateDefaultSettings();
                Connection.SetSettingsAsync(JObject.FromObject(Settings));
            }
            else
            {
                Settings = payload.Settings.ToObject<PluginSettings>();
            }
            LoadMacros();
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Key Pressed {this.GetType()}");

            keyPressed = true;
            longKeyPressed = false;
            keyPressStart = DateTime.Now;

            if (!InputRunning)
            {
                LoadMacros(); // Refresh the macros, relevant for if you're reading from a file
            }
        }

        public override void KeyReleased(KeyPayload payload)
        {
            keyPressed = false;
            if (!longKeyPressed) // Take care of the short keypress
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, $"Short Keypress {this.GetType()}");
                if (InputRunning)
                {
                    ForceStop = true;
                }
                else
                {
                    ForceStop = false;
                    SendInput(primaryMacro, CreateWriterSettings());
                }
            }
        }

        public override void OnTick()
        {
            base.OnTick();

            if (keyPressed)
            {
                int timeKeyWasPressed = (int)(DateTime.Now - keyPressStart).TotalMilliseconds;
                if (timeKeyWasPressed >= LONG_KEYPRESS_LENGTH)
                {
                    LongKeyPress();
                }
            }
        }


        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Destructor called");
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            bool prevKeydownDelay = Settings.KeydownDelay;
            // New in StreamDeck-Tools v2.0:
            Tools.AutoPopulateSettings(Settings, payload.Settings);
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Settings loaded: {payload.Settings}");

            if (Settings.KeydownDelay && !prevKeydownDelay && Settings.Delay < CommandTools.RECOMMENDED_KEYDOWN_DELAY)
            {
                Settings.Delay = CommandTools.RECOMMENDED_KEYDOWN_DELAY;
                Connection.SetSettingsAsync(JObject.FromObject(Settings));
            }
            LoadMacros();
            SaveSettings();
        }

        #endregion

        #region Private Methods

        private async void LongKeyPress()
        {
            longKeyPressed = true;
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Long Keypress {this.GetType()}");
            ForceStop = false;
            SendInput(secondaryMacro, CreateWriterSettings());
            await Connection.ShowOk();
        }

        protected override Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(Settings));
        }

        protected override void LoadMacros()
        {
            base.LoadMacros();

            // Handle the secondary
            secondaryMacro = String.Empty;
            if (settings.LoadFromFiles)
            {
                secondaryMacro = ReadFile(Settings.SecondaryInputFile);
            }
            else
            {
                secondaryMacro = Settings.LongPressInputText;
            }
        }

        private WriterSettings CreateWriterSettings()
        {
            return new WriterSettings(settings.IgnoreNewline, settings.EnterMode, false, settings.KeydownDelay, settings.ForcedMacro, false, settings.Delay, 0);
        }

        #endregion
    }
}
