using StardewModdingAPI.Utilities;

namespace AutoStore;

public sealed class ModConfig {
    public int DistanceThreshold { get; set; } = 10;
    public KeybindList Keybind { get; set; } = KeybindList.Parse("Q");
}