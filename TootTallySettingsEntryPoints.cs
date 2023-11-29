using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TootTallySettings
{
    public class TootTallySettingsEntryPoints
    {
        public virtual void AddToTootTallyPage(TootTallySettingPage toottallyPage) { }
        public virtual void AddToModulePage(ITootTallyModuleManager modulePage) { }
        public virtual void InitializeSettings() { }
    }
}
