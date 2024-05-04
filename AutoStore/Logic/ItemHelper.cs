using StardewValley;
using StardewValley.Menus;

namespace AutoStore.Logic;

public static class ItemHelper {
    public static Item? GetHoveredItem() {
        Item? item = null;

        foreach (IClickableMenu menu in Game1.onScreenMenus) {
            if (menu is Toolbar toolbar && toolbar.hoverItem is Item toolbarItem) {
                item = toolbarItem;
            }
        }

        if (Game1.activeClickableMenu is GameMenu gameMenu) {
            switch (gameMenu.GetCurrentPage()) {
                case InventoryPage inventoryPage:
                    item = inventoryPage.hoveredItem;
                    break;
                case CraftingPage craftingPage:
                    item = craftingPage.hoverItem;
                    break;
            }
        }

        if(Game1.activeClickableMenu is ItemGrabMenu grabMenu) {
            if (grabMenu.hoveredItem is Item) {
                item = grabMenu.hoveredItem;
            }
        }

        return item;
    }
}