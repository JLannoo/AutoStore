using Force.DeepCloner;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

using Object = StardewValley.Object;

namespace AutoStore.Logic;

public static class ChestHelper {
    public static List<Chest> GetNearbyChests(int distance) {
        List<Object> objects = Game1.currentLocation.objects.Values.ToList();
        List<Object> chests = objects.FindAll(o => o is Chest);

        List<Chest> nearby = new();

        var playerPosition = Game1.player.TilePoint;

        foreach (var chest in chests) {
            var chestPosition = chest.TileLocation;
            var dist = Utility.distance(chestPosition.X, playerPosition.X, chestPosition.Y, playerPosition.Y);

            if(dist <= distance) {
                nearby.Add(chest as Chest);
            }
        }

        return nearby;
    }

    public static void FillOutNearbyChests(int distance) {
        var nearbyChest = GetNearbyChests(distance);

        foreach (var chest in nearbyChest) {
            var tempMenu = new ItemGrabMenu(chest.Items);

            // Count items in inventory
            int itemsBeforeFill = Game1.player.Items.Aggregate(0, (count, item) => count += item?.stack.Value ?? 0);
            tempMenu.FillOutStacks();
            var itemsAfterFill = Game1.player.Items.Aggregate(0, (count, item) => count += item?.stack.Value ?? 0);

            // Shake chest if any item has been transferred
            if (itemsBeforeFill > itemsAfterFill) {
                chest.shakeTimer = 500;
                Game1.playSound("Ship");
            }
        }
    }
}