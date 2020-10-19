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

    //---------------------------------------------------
    //          BarRaider's Hall Of Fame
    // Subscriber: OneMouseGaming
    // Subscriber: CyberlightGames
    // Subscriber: MagicGuitarist
    //---------------------------------------------------
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

            [JsonProperty(PropertyName = "longKeypressTime")]
            public string LongKeypressTime { get; set; }
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

        private const int LONG_KEYPRESS_LENGTH_MS = 600;

        private bool longKeyPressed = false;
        private int longKeypressTime = LONG_KEYPRESS_LENGTH_MS;
        private readonly System.Timers.Timer tmrRunLongPress = new System.Timers.Timer();
        private string secondaryMacro;

        #endregion

        #region Public Methods

        public SuperMacroAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                Settings = PluginSettings.CreateDefaultSettings();
                Connection.SetSettingsAsync(JObject.FromObject(Settings));
                SaveSettings();
            }
            else
            {
                Settings = payload.Settings.ToObject<PluginSettings>();
            }
            tmrRunLongPress.Interval = longKeypressTime;
            tmrRunLongPress.Elapsed += TmrRunLongPress_Elapsed;
            InitializeSettings();
        }

        private void TmrRunLongPress_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            LongKeyPress();
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Key Pressed {this.GetType()}");
            longKeyPressed = false;

            tmrRunLongPress.Interval = longKeypressTime > 0 ? longKeypressTime : LONG_KEYPRESS_LENGTH_MS;
            tmrRunLongPress.Start();

            if (!InputRunning)
            {
                LoadMacros(); // Refresh the macros, relevant for if you're reading from a file
            }
        }

        public override void KeyReleased(KeyPayload payload)
        {
            tmrRunLongPress.Stop();
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Key Released {this.GetType()}");
            if (!longKeyPressed) // Take care of the short keypress
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, $"Short Keypress {this.GetType()}");
                if (InputRunning)
                {
                    ForceStop = true;
                }
                else // Start the short press macro
                {
                    ForceStop = false;
                    SendInput(primaryMacro, CreateWriterSettings());
                }
            }
        }

        public override void OnTick()
        {
            base.OnTick();
        }


        public override void Dispose()
        {
            tmrRunLongPress.Stop();
            tmrRunLongPress.Elapsed -= TmrRunLongPress_Elapsed;
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
            InitializeSettings();
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
            return new WriterSettings(settings.IgnoreNewline, settings.EnterMode, false, settings.KeydownDelay, settings.ForcedMacro, settings.Delay, 0);
        }

        private void InitializeSettings()
        {
            if (!Int32.TryParse(Settings.LongKeypressTime, out longKeypressTime))
            {
                Settings.LongKeypressTime = LONG_KEYPRESS_LENGTH_MS.ToString();
                SaveSettings();
            }

            LoadMacros();
        }

        #endregion
    }
}
