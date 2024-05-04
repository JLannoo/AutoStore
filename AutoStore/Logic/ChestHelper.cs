using Force.DeepCloner;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

using Object = StardewValley.Object;

namespace AutoStore.Logic;

public static class ChestHelper {
    public static void TryStackToNearbyChests(int distance, Item? hoveredItem = null) {
        var nearbyChests = GetNearbyChests(distance);
        if (nearbyChests == null) return;

        if(hoveredItem != null) {
            StackHoveredItemToChest(nearbyChests, hoveredItem);
            return;
        }

        StackAllToChests(nearbyChests);
    }

    public static void TryFetchFromNearbyChests(int distance, Item? hoveredItem = null) {
        var nearbyChests = GetNearbyChests(distance);
        if (nearbyChests == null) return;

        if (hoveredItem != null) {
            FetchFromChests(nearbyChests, hoveredItem);
            return;
        }
    }

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

    public static void StackAllToChests(List<Chest> chests) {
        foreach (var chest in chests) {
            var tempMenu = new ItemGrabMenu(chest.Items);

            // Count items in inventory
            int itemsBeforeFill = Game1.player.Items.Aggregate(0, (count, item) => count += item?.stack.Value ?? 0);
            tempMenu.FillOutStacks();
            var itemsAfterFill = Game1.player.Items.Aggregate(0, (count, item) => count += item?.stack.Value ?? 0);

            // Shake chest if any item has been transferred
            if (itemsBeforeFill > itemsAfterFill) {
                ShakeChest(chest);
            }
        }
    }

    public static bool StackHoveredItemToChest(List<Chest> chests, Item item) {
        foreach (var chest in chests) {
            var chestItem = chest.Items.FirstOrDefault(i => i.itemId == item.itemId, null);
            if (chestItem != null) {

                // If hovered item is in chest's inventory, send to player's inventory instead
                if(chest.Items.Contains(item)) {
                    FetchFromChests(new() { chest }, item);
                    return false;
                }

                chest.addItem(item);
                Game1.player.removeItemFromInventory(item);

                Game1.addHUDMessage(HUDMessage.ForItemGained(item, chestItem.Stack));

                ShakeChest(chest);
                return true;
            }
        }

        return false;
    }

    public static bool FetchFromChests(List<Chest> chests, Item item) {
        foreach (var chest in chests) {
            foreach(var chestItem in chest.Items) {
                if(item.itemId == chestItem.itemId && Game1.player.couldInventoryAcceptThisItem(chestItem)) {
                    Game1.player.addItemToInventoryBool(chestItem, true);

                    chest.GetItemsForPlayer().Remove(chestItem);
                    chest.clearNulls();

                    ShakeChest(chest);

                    return true;
                }
            }
        }

        return false;
    }

    public static void ShakeChest(Chest chest) {
        chest.shakeTimer = 500;
        Game1.currentLocation.playSound("Ship", chest.TileLocation);
    }
}