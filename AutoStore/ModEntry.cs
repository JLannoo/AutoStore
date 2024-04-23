using AutoStore.Components;
using AutoStore.Logic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace AutoStore;

internal class ModEntry : Mod {
    public static IMonitor monitor;
    private static IModHelper _helper;

    private AutoStackButton? _stackButton;
    /// <summary>
    /// I assign `mousePosition` in the RenderActiveMenu event because it it's an incorrect value in the ButtonPressed event.
    /// </summary>
    private Point _mousePosition;

    public override void Entry(IModHelper helper){
        monitor = Monitor;
        _helper = helper;

        helper.Events.Display.MenuChanged += OnChangeMenu;
        helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
        helper.Events.Input.ButtonReleased += OnButtonPressed;
    }

    private void OnButtonPressed(object? sender, StardewModdingAPI.Events.ButtonReleasedEventArgs e) {
        if (Game1.activeClickableMenu is GameMenu menu && menu.GetCurrentPage() is InventoryPage page) {
            if (e.Button.IsUseToolButton()) {
                _stackButton?.ReceiveLeftClick(_mousePosition.X, _mousePosition.Y);
            }
        }

        if(e.Button == SButton.Q) {
            ChestHelper.FillOutNearbyChests(10);
        }
    }

    private void OnRenderedActiveMenu(object? sender, StardewModdingAPI.Events.RenderedActiveMenuEventArgs e) {
        _mousePosition = Game1.getMousePosition();
        _stackButton?.draw(Game1.spriteBatch);
        _stackButton?.tryHover(_mousePosition.X, _mousePosition.Y);
    }

    private void OnChangeMenu(object? sender, StardewModdingAPI.Events.MenuChangedEventArgs e) {
        if(Game1.activeClickableMenu is GameMenu menu && menu.GetCurrentPage() is InventoryPage page) {
            var orgButton = page.organizeButton;
            var orgPosition = page.organizeButton.bounds.Location;
            Vector2 displacement = new(0, orgButton.bounds.Height + 10);

            Point buttonPosition = new(orgPosition.X + (int)displacement.X, orgPosition.Y + (int)displacement.Y);

            _stackButton = new AutoStackButton(buttonPosition);
        } else {
            _stackButton = null;
        }
    }
}