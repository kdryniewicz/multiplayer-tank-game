using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CommonData;
using Microsoft.Xna.Framework.Input;
using Microsoft.AspNet.SignalR.Client;
using Engine.Engines;
using CameraNS;

namespace Sprites
{
    public class OtherPlayerSprite : DrawableGameComponent
    {
        public Texture2D Image;
        public Point Position;
        public Rectangle BoundingRect;
        public Rectangle CollisionRect;
        public bool Visible = true;
        public Color tint = Color.White;
		public PlayerData pData;
        public Turret turret;
        public Vector2 origin;
        public float rotation;
        public Game g;
		
        // Constructor epects to see a loaded Texture
        // and a start position
        public OtherPlayerSprite(Game game, PlayerData data, Texture2D spriteImage, Texture2D turretImage,Texture2D projectileImage,
                            Point startPosition) : base(game)
        {
            g = game;
            pData = data;
            game.Components.Add(this);
            // Take a copy of the texture passed down
            Image = spriteImage;
            // Take a copy of the start position
            Position = startPosition;
            // Calculate the bounding rectangle
            BoundingRect = new Rectangle(startPosition.X, startPosition.Y, Image.Width, Image.Height);
            CollisionRect = BoundingRect;
            turret = new Turret(Position.ToVector2(), turretImage,projectileImage, game);
            origin = new Vector2(Image.Width / 2, Image.Height / 2);

        }

        public override void Update(GameTime gameTime)
        {


            if (!Visible) return;
            BoundingRect = new Rectangle(Position.X, Position.Y, Image.Width, Image.Height);
            CollisionRect = BoundingRect;
            CollisionRect.X -= Image.Width / 2;
            CollisionRect.Y -= Image.Height / 2;
            foreach (SimpleProjectile p in turret.projectiles)
            {
                p.Update(gameTime);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch sp = Game.Services.GetService<SpriteBatch>();
            if (Image != null && Visible)
            {
                sp.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Camera.CurrentCameraTranslation);
                sp.Draw(Image, BoundingRect, null, Color.White, rotation, origin, SpriteEffects.None, 0);
                sp.Draw(turret._tx, turret.BoundingRect, null, Color.White, turret.rotation, turret.origin , SpriteEffects.None, 1);
              
                sp.End();

                if (turret.projectiles.Count > 0)
                    foreach (SimpleProjectile p in turret.projectiles)
                    {
                        p.Draw(gameTime, sp);
                    }
            }

            base.Draw(gameTime);
        }


    }
}
