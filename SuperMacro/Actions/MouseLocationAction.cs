using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Threading.Tasks;

namespace WinTools
{
    [PluginActionId("com.barraider.supermacro.mouselocation")]
    public class MouseLocationAction : PluginBase
    {
        public enum MouseCoordinatesType
        {
            SuperMacro = 0,
            Windows = 1
        }

        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings
                {
                    CoordinatesType = MouseCoordinatesType.Windows
                };
                return instance;
            }

            [JsonProperty(PropertyName = "coordinatesType")]
            public MouseCoordinatesType CoordinatesType { get; set; }
        }

        #region Private Members
        private readonly PluginSettings settings;
        private readonly System.Timers.Timer tmrShowMouseLocation;

        private bool shownMousePosition = false;

        #endregion
        public MouseLocationAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                this.settings = PluginSettings.CreateDefaultSettings();
                SaveSettings();
            }
            else
            {
                this.settings = payload.Settings.ToObject<PluginSettings>();
            }

            tmrShowMouseLocation = new System.Timers.Timer
            {
                Interval = 250
            };
            tmrShowMouseLocation.Elapsed += TmrShowMouseLocation_Elapsed;
        }

        public override void Dispose()
        {
            tmrShowMouseLocation.Stop();
            tmrShowMouseLocation.Dispose();
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Destructor called");
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Key Pressed {this.GetType()}");

            shownMousePosition = true;
            if (tmrShowMouseLocation.Enabled)
            {
                tmrShowMouseLocation.Stop();
            }
            else
            {
                tmrShowMouseLocation.Start();
            }
           
        }

        public override void KeyReleased(KeyPayload payload) 
        {
        }

        public async override void OnTick()
        {
            if (!tmrShowMouseLocation.Enabled && !shownMousePosition)
            {
                await Connection.SetTitleAsync("Press\nto\nstart");
            }
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        #region Private Methods

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        private void TmrShowMouseLocation_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Point currentLocation;
            currentLocation = System.Windows.Forms.Cursor.Position;
            /*
            if (settings.CoordinatesType == MouseCoordinatesType.SuperMacro)
            {
                currentLocation = MouseLocation.ConvertScreenPointToAbsolutePoint(System.Windows.Forms.Cursor.Position);
            }
            else
            {
                currentLocation = System.Windows.Forms.Cursor.Position;
            }
            */
            Connection.SetTitleAsync($"X: {currentLocation.X}\nY: {currentLocation.Y}");
        }

        #endregion
    }
}