using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CommonData;

namespace Sprites
{
    public class OtherPlayerSprite : DrawableGameComponent
    {
        public Texture2D Image;
        public Point Position;
        public Rectangle BoundingRect;
        public bool Visible = true;
        public Color tint = Color.White;
		public PlayerData pData;
		
        // Constructor epects to see a loaded Texture
        // and a start position
        public OtherPlayerSprite(Game game, PlayerData data, Texture2D spriteImage,
                            Point startPosition) : base(game)
        {
            pData = data;
            game.Components.Add(this);
            // Take a copy of the texture passed down
            Image = spriteImage;
            // Take a copy of the start position
            Position = startPosition;
            // Calculate the bounding rectangle
            BoundingRect = new Rectangle(startPosition.X, startPosition.Y, Image.Width, Image.Height);

        }

        public override void Update(GameTime gameTime)
        {
            BoundingRect = new Rectangle(Position.X, Position.Y, Image.Width, Image.Height);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch sp = Game.Services.GetService<SpriteBatch>();
            SpriteFont font = Game.Services.GetService<SpriteFont>();

            if (Image != null && Visible)
            {
                sp.Begin();
                sp.Draw(Image, BoundingRect, tint);
                sp.DrawString(font, pData.GamerTag, new Vector2(Position.X + 20, Position.Y - (Image.Height / 4)), Color.White);//Draws the player gamerTag
                sp.End();
            }

            base.Draw(gameTime);
        }


    }
}
