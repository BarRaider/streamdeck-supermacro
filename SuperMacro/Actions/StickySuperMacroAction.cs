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
    [PluginActionId("com.barraider.supermacrostickymacro")]
    public class StickySuperMacroAction : SuperMacroBase
    {
        #region Private members

        protected StickyMacroSettings Settings
        {
            get
            {
                var result = settings as StickyMacroSettings;
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

        private string enabledFile = null;
        private string disabledFile = null;
        private int autoStopNum = StickyMacroSettings.DEFAULT_AUTO_STOP_NUM;

        #endregion

        #region Public Methods

        public StickySuperMacroAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                Settings = StickyMacroSettings.CreateDefaultSettings();
                Connection.SetSettingsAsync(JObject.FromObject(Settings));
            }
            else
            {
                Settings = payload.Settings.ToObject<StickyMacroSettings>();
                HandleFilenames();
            }
            LoadMacros();
            InitializeSettings();
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Key Pressed {this.GetType()}");
            StickyEnabled = !StickyEnabled;
            if (StickyEnabled)
            {
                LoadMacros(); // Refresh the macros, relevant for if you're reading from a file
                Logger.Instance.LogMessage(TracingLevel.INFO, $"Command Started");
                SendStickyInput(primaryMacro, CreateWriterSettings());
            }
        }

        public async override void OnTick()
        {
            string imgBase64;
            if (StickyEnabled)
            {
                imgBase64 = Properties.Settings.Default.StickyEnabled;

                if (!String.IsNullOrWhiteSpace(enabledFile))
                {
                    imgBase64 = enabledFile;
                }
                await Connection.SetImageAsync(imgBase64);
            }
            else
            {
                imgBase64 = Properties.Settings.Default.StickyDisabled;
                if (!String.IsNullOrWhiteSpace(disabledFile))
                {
                    imgBase64 = disabledFile;
                }
                await Connection.SetImageAsync(imgBase64);
            }
        }


        public override void Dispose()
        {
            StickyEnabled = false;
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
            }
            HandleFilenames();
            LoadMacros();
            InitializeSettings();
        }

        #endregion

        #region Private Methods

        private void HandleFilenames()
        {
            enabledFile = Tools.FileToBase64(Settings.EnabledImageFilename, true);
            disabledFile = Tools.FileToBase64(Settings.DisabledImageFilename, true);
            Connection.SetSettingsAsync(JObject.FromObject(Settings));
        }

        private void InitializeSettings()
        {
            if (!Int32.TryParse(Settings.AutoStopNum, out autoStopNum))
            {
                Settings.AutoStopNum = StickyMacroSettings.DEFAULT_AUTO_STOP_NUM.ToString();
                SaveSettings();
            }
        }

        private WriterSettings CreateWriterSettings()
        {
            return new WriterSettings(settings.IgnoreNewline, settings.EnterMode, Settings.RunUntilEnd, settings.KeydownDelay, settings.ForcedMacro, false, settings.Delay, autoStopNum);
        }

        #endregion
    }
}
