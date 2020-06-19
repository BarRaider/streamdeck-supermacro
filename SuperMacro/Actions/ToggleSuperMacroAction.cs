using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuperMacro.Backend;
using SuperMacro.Wrapper;
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
    [PluginActionId("com.barraider.supermacrotoggle")]
    public class ToggleSuperMacroAction : SuperMacroBase
    {
        protected class PluginSettings : MacroSettingsBase
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings
                {
                    InputText = String.Empty,
                    SecondaryText = String.Empty,
                    PrimaryImageFilename = String.Empty,
                    SecondaryImageFilename = string.Empty,
                    Delay = 10,
                    EnterMode = false,
                    ForcedMacro = false,
                    KeydownDelay = false,
                    IgnoreNewline = false,
                    LoadFromFiles = false,
                    PrimaryInputFile = String.Empty,
                    SecondaryInputFile = String.Empty,
                    RememberState = false
                };

                return instance;
            }

            [FilenameProperty]
            [JsonProperty(PropertyName = "secondaryInputFile")]
            public string SecondaryInputFile { get; set; }

            [JsonProperty(PropertyName = "secondaryText")]
            public string SecondaryText { get; set; }

            [FilenameProperty]
            [JsonProperty(PropertyName = "primaryImage")]
            public string PrimaryImageFilename { get; set; }

            [FilenameProperty]
            [JsonProperty(PropertyName = "secondaryImage")]
            public string SecondaryImageFilename { get; set; }

            [JsonProperty(PropertyName = "rememberState")]
            public bool RememberState { get; set; }           
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

        #region Private members

        string primaryFile = null;
        string secondaryFile = null;
        bool isPrimary = false;
        string secondaryMacro = String.Empty;
        private GlobalSettings global;

        #endregion

        #region Public Methods

        public ToggleSuperMacroAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                Settings = PluginSettings.CreateDefaultSettings();
                Connection.SetSettingsAsync(JObject.FromObject(Settings));
            }
            else
            {
                Settings = payload.Settings.ToObject<PluginSettings>();
                HandleFilenames();
            }
            Connection.OnSendToPlugin += Connection_OnSendToPlugin;
            Connection.GetGlobalSettingsAsync();
            LoadMacros();          
        }

        public async override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Key Pressed {this.GetType()}");
            if (InputRunning)
            {
                ForceStop = true;
                return;
            }

            LoadMacros(); // Refresh the macros, relevant for if you're reading from a file
            ForceStop = false;
            isPrimary = !isPrimary;
            string text = isPrimary ? primaryMacro : secondaryMacro;
            await SaveToggleState();
            SendInput(text, CreateWriterSettings());
        }

        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Destructor called");
            Connection.OnSendToPlugin -= Connection_OnSendToPlugin;
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
            if (payload?.Settings != null && payload.Settings.Count > 0)
            {
                global = payload.Settings.ToObject<GlobalSettings>();
                LoadToggleState();
            }

            base.ReceivedGlobalSettings(payload);
        }

        public async override void OnTick()
        {
            string imgBase64;
            if (isPrimary)
            {
                imgBase64 = Properties.Settings.Default.TogglePrimary;

                if (!String.IsNullOrWhiteSpace(primaryFile))
                {
                    imgBase64 = primaryFile;
                }
                await Connection.SetImageAsync(imgBase64);
            }
            else
            {
                imgBase64 = Properties.Settings.Default.ToggleSecondary;
                if (!String.IsNullOrWhiteSpace(secondaryFile))
                {
                    imgBase64 = secondaryFile;
                }
                await Connection.SetImageAsync(imgBase64);
            }
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
            }
            HandleFilenames();
            LoadMacros();
        }

        #endregion

        #region Private Methods

        private void HandleFilenames()
        {
            primaryFile = Tools.FileToBase64(Settings.PrimaryImageFilename, true);
            secondaryFile = Tools.FileToBase64(Settings.SecondaryImageFilename, true);
            Connection.SetSettingsAsync(JObject.FromObject(Settings));
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
                secondaryMacro = Settings.SecondaryText;
            }
        }

        private void LoadToggleState()
        {
            if (!Settings.RememberState)
            {
                return;
            }

            if (global != null && global.DictToggleStates != null && global.DictToggleStates.ContainsKey(Connection.ContextId))
            {
                Logger.Instance.LogMessage(TracingLevel.INFO, $"Global Setting DictToggleStates has {global.DictToggleStates.Keys.Count} entries");
                isPrimary = global.DictToggleStates[Connection.ContextId];
            }
        }

        private async Task SaveToggleState()
        {
            if (!Settings.RememberState)
            {
                return;
            }

            if (global == null)
            {
                Logger.Instance.LogMessage(TracingLevel.WARN, $"SaveToggleState - global is null, creating new object");
                global = new GlobalSettings();
            }

            if (global.DictToggleStates == null)
            {
                global.DictToggleStates = new Dictionary<string, bool>();
            }

            global.DictToggleStates[Connection.ContextId] = isPrimary;
            await Connection.SetGlobalSettingsAsync(JObject.FromObject(global));
        }

        private async void Connection_OnSendToPlugin(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.SendToPlugin> e)
        {
            var payload = e.Event.Payload;

            Logger.Instance.LogMessage(TracingLevel.INFO, "OnSendToPlugin called");
            if (payload["property_inspector"] != null)
            {
                switch (payload["property_inspector"].ToString().ToLowerInvariant())
                {
                    case "resetstate":
                        isPrimary = false;
                        await SaveToggleState();
                        break;
                    case "resetallstates":
                        global = null;
                        isPrimary = false;
                        await SaveToggleState();
                        break;
                }
            }
        }

        private WriterSettings CreateWriterSettings()
        {
            return new WriterSettings(settings.IgnoreNewline, settings.EnterMode, false, settings.KeydownDelay, settings.ForcedMacro, false, settings.Delay, 0);
        }

        #endregion

    }
}
