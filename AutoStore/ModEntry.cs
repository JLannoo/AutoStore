using StardewModdingAPI;

namespace AutoStore;

internal class ModEntry : Mod {
    public static IMonitor monitor;

    public override void Entry(IModHelper helper){
        monitor = Monitor;
    }


}