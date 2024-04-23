using AutoStore.Logic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;

using Object = StardewValley.Object;

namespace AutoStore.Components;

public class AutoStackButton : ClickableTextureComponent {
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
        if (containsPoint(x, y)) {
            ChestHelper.FillOutNearbyChests(ModEntry.config.DistanceThreshold);
        }
    }
}