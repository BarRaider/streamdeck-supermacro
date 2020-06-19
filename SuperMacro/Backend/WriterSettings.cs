using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMacro.Backend
{
    public class WriterSettings
    {
        public bool IgnoreNewline { get; private set; }

        public bool EnterMode { get; private set; }

        public bool RunUntilEnd { get; private set; }

        public bool KeydownDelay { get; private set; }

        public bool ForcedMacro { get; private set; }

        public int Delay { get; private set; }

        public int AutoStopNum { get; private set; }

        public bool IsMomentary { get; private set; }

        public WriterSettings(bool ignoreNewLine, bool enterMode, bool runUntilEnd, bool keydownDelay, bool forcedMacro, bool isMomentary, int delay, int autoStopNum)
        {
            IsMomentary = isMomentary;
            IgnoreNewline = ignoreNewLine;
            EnterMode = enterMode;
            RunUntilEnd = runUntilEnd;
            KeydownDelay = keydownDelay;
            ForcedMacro = forcedMacro;
            Delay = delay;
            AutoStopNum = autoStopNum;
        }
        
    }
}
