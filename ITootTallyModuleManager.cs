using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TootTallyCore.Utils.TootTallyModules;

namespace TootTallySettings
{
    public interface ITootTallyModuleManager
    {
        void AddModuleToSettingPage(ITootTallyModule module);
    }
}
