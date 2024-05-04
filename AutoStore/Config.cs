using StardewModdingAPI.Utilities;

namespace AutoStore;

public sealed class ModConfig {
    public int DistanceThreshold { get; set; } = 10;
    public KeybindList StackKeybind { get; set; } = KeybindList.Parse("Q");
    public KeybindList FetchKeybind { get; set; } = KeybindList.Parse("LeftShift + Q");
}