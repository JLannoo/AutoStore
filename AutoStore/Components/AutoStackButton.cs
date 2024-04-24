using AutoStore.Logic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace AutoStore.Components;

public class AutoStackButton : ClickableTextureComponent {
    private bool active = true;
    public AutoStackButton(Point position) 
        : base("", new Rectangle(position, new Point(64)), "", Game1.content.LoadString("Strings\\UI:ItemGrab_FillStacks"), Game1.mouseCursors, new Rectangle(103, 469, 16, 16), 4f, true) {
                
    }

    public override void tryHover(int x, int y, float maxScaleIncrease = 0.1F) {
        base.tryHover(x, y, maxScaleIncrease);
        
        if(containsPoint(x, y)) {
            IClickableMenu.drawHoverText(Game1.spriteBatch, hoverText, Game1.smallFont);
        }
    }

    public void ReceiveLeftClick(int x, int y) {
        if (active && containsPoint(x, y)) {
            ChestHelper.FillOutNearbyChests(ModEntry.config.DistanceThreshold);
        }
    }

    public override void draw(SpriteBatch b) {
        active = ChestHelper.GetNearbyChests(ModEntry.config.DistanceThreshold).Count > 0;

        float opacity = active ? 1 : 0.5f;
        base.draw(b, Color.White * opacity, 1);
    }
}