using AutoStore.Components;
using AutoStore.Logic;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace AutoStore;

internal class ModEntry : Mod {
    public static IMonitor monitor;
    private static IModHelper _helper;

    public static ModConfig config;

    private AutoStackButton? _stackButton;
    /// <summary>
    /// I assign `mousePosition` in the RenderActiveMenu event because it it's an incorrect value in the ButtonPressed event.
    /// </summary>
    private Point _mousePosition;
    /// <summary>
    /// I assign `hoveredItem` in the RenderingHud event because it's set to null for Toolbar in other events
    /// </summary>
    private Item? _hoveredItem;

    public override void Entry(IModHelper helper){
        monitor = Monitor;
        _helper = helper;

        config = helper.ReadConfig<ModConfig>();

        helper.Events.Display.MenuChanged += OnChangeMenu;
        helper.Events.Input.ButtonPressed += OnButtonPressed;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;

        helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;
        helper.Events.Display.RenderingHud += OnRenderingHud;
    }

    #region Events
    private void OnRenderingHud(object? sender, RenderingHudEventArgs e) {
        _hoveredItem = ItemHelper.GetHoveredItem();
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e) {
        RegisterConfigMenu();
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e) {
        if (Game1.activeClickableMenu is GameMenu menu && menu.GetCurrentPage() is InventoryPage) {
            if (e.Button.IsUseToolButton()) {
                _stackButton?.ReceiveLeftClick(_mousePosition.X, _mousePosition.Y);
            }
        }

        if(Context.IsWorldReady) {
            if (config.FetchKeybind.JustPressed()) {
                ChestHelper.TryFetchFromNearbyChests(config.DistanceThreshold, _hoveredItem);
                return;
            }

            if (config.StackKeybind.JustPressed()) {
                ChestHelper.TryStackToNearbyChests(config.DistanceThreshold, _hoveredItem);
                return;
            }
        }
    }

    private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e) {
        if(Game1.activeClickableMenu is GameMenu menu && menu.GetCurrentPage() is InventoryPage page) {
            _mousePosition = Game1.getMousePosition();
            _stackButton?.draw(e.SpriteBatch);
            _stackButton?.tryHover(_mousePosition.X, _mousePosition.Y);
        }
    }

    private void OnChangeMenu(object? sender, MenuChangedEventArgs e) {
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
    #endregion

    #region Methods
    private void RegisterConfigMenu() {
        // get Generic Mod Config Menu's API (if it's installed)
        var configMenu = _helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        // register mod
        configMenu.Register(
            mod: ModManifest,
            reset: () => config = new ModConfig(),
            save: () => _helper.WriteConfig(config)
        );

        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: () => "AutoStore"
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "Chest detection distance",
            tooltip: () => "How far away the mod will look for chests to stack to.",
            getValue: () => config.DistanceThreshold,
            setValue: value => config.DistanceThreshold = value,
            min: 1,
            max: 100
        );

        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => "Auto stack keybinds",
            tooltip: () => "Press to auto stack items to nearby chests",
            getValue: () => config.StackKeybind,
            setValue: value => config.StackKeybind = value
        );

        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => "Auto fetch keybinds",
            tooltip: () => "Press to pull the hovered item from nearby chests",
            getValue: () => config.FetchKeybind,
            setValue: value => config.FetchKeybind = value
        );
    }
    #endregion
}