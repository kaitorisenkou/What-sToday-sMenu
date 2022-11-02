using Verse;
using System.Reflection;
using System.Linq;
using System.Xml;
using System.Text;

namespace WhatsTodaysMenu {
    [StaticConstructorOnStartup]
    public class WhatsTodaysMenu {
        static WhatsTodaysMenu() {
            Log.Message("[WhatsTodaysMenu] Now Active");
        }
    }
}
