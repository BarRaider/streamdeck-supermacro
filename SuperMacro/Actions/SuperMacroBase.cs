using BarRaider.SdTools;
using Newtonsoft.Json.Linq;
using SuperMacro.Backend;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace SuperMacro.Actions
{
    public abstract class SuperMacroBase : PluginBase
    {
        #region Protected Members

        protected MacroSettingsBase settings;
        protected string primaryMacro;
        private SuperMacroWriter textWriter = new SuperMacroWriter();

        protected bool InputRunning
        {
            get
            {
                return textWriter.InputRunning;
            }
            set
            {
                textWriter.InputRunning = value;
            }
        }

        protected bool ForceStop
        {
            get
            {
                return textWriter.ForceStop;
            }
            set
            {
                textWriter.ForceStop = value;
            }
        }

        protected bool StickyEnabled
        {
            get
            {
                return textWriter.StickyEnabled;
            }
            set
            {
                textWriter.StickyEnabled = value;
            }
        }

        #endregion

        public SuperMacroBase(SDConnection connection, InitialPayload payload) : base(connection, payload) { }

        #region PluginBase Methods

        public override void KeyReleased(KeyPayload payload) { }

        public override void OnTick() { }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        #endregion
       

        protected virtual Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        protected virtual void LoadMacros()
        {
            primaryMacro = String.Empty;
            if (settings.LoadFromFiles)
            {
                primaryMacro = ReadFile(settings.PrimaryInputFile);
            }
            else
            {
                primaryMacro = settings.InputText;
            }
        }

        protected string ReadFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, $"ReadFile called but no file {fileName}");
                return null;
            }
            else
            {
                return File.ReadAllText(fileName);
            }
        }

        protected async void SetKeyTitle(string title)
        {
            await Connection.SetTitleAsync(title);
        }

        protected void SendInput(string inputText, WriterSettings writerSettings)
        {
            _ = textWriter.SendInput(inputText, writerSettings, SetKeyTitle);
        }

        protected void SendStickyInput(string inputText, WriterSettings writerSettings)
        {
            textWriter.SendStickyInput(inputText, writerSettings, SetKeyTitle);
        }
    }
}
