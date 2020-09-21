using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    [PluginActionId("com.barraider.keystrokeptt")]
    public class KeystrokePTTAction : KeystrokeBase
    {
        private class PluginSettings : PluginSettingsBase
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings
                {
                    Command = String.Empty,
                    ReleaseCommand = String.Empty,
                    RunCommandOnce = false
                };
                return instance;
            }

            [JsonProperty(PropertyName = "releaseCommand")]
            public string ReleaseCommand { get; set; }

            [JsonProperty(PropertyName = "runCommandOnce")]
            public bool RunCommandOnce { get; set; }
        }

        private PluginSettings Settings
        {
            get
            {
                var result = settings as PluginSettings;
                if (result == null)
                {
                    Logger.Instance.LogMessage(TracingLevel.ERROR, "Cannot convert PluginSettingsBase to PluginSettings");
                }
                return result;
            }
            set
            {
                settings = value;
            }
        }

        #region Public Methods

        public KeystrokePTTAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                Settings = PluginSettings.CreateDefaultSettings();
                Connection.SetSettingsAsync(JObject.FromObject(Settings));
            }
            else
            {
                this.settings = payload.Settings.ToObject<PluginSettings>();
            }
            InitializeSettings();
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Key Pressed {this.GetType()}");
            forceOneRound = false;
            keyPressed = true;

            // If should only run once, change the flags
            if (Settings.RunCommandOnce)
            {
                keyPressed = false;
                forceOneRound = true;
            }
            RunCommand(Settings.Command);
        }

        public override void KeyReleased(KeyPayload payload)
        {
            keyPressed = false;
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Key Released {this.GetType()}");

            if (!String.IsNullOrEmpty(Settings.ReleaseCommand))
            {
                Task.Run(async () =>
                {
                    await Task.Delay(delay);
                    forceOneRound = true;
                    RunCommand(Settings.ReleaseCommand);
                });
            }
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            // New in StreamDeck-Tools v2.0:
            Tools.AutoPopulateSettings(Settings, payload.Settings);
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Settings loaded: {payload.Settings}");
            InitializeSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        public override void OnTick() { }

        public override Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(Settings));
        }

        protected override void InitializeSettings()
        {
            Settings.ReleaseCommand = ValidateKeystroke(Settings.ReleaseCommand);
            base.InitializeSettings();
        }


        #endregion
    }
}
