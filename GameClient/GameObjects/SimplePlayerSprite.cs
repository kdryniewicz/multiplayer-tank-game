using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CommonData;
using Engine.Engines;
using Microsoft.Xna.Framework.Input;
using Microsoft.AspNet.SignalR.Client;
using CameraNS;

namespace Sprites
{
    public class SimplePlayerSprite :DrawableGameComponent
    {
        public Texture2D Image;
        public Vector2 Position;
        public Rectangle BoundingRect;
        public Rectangle CollisionRect;
        public bool Visible = true;
        public Color tint = Color.White;
		public PlayerData pData;
        public Vector2 previousPosition;		
        public int speed = 5;
        public float delay = 500;
        public float rotation;
        public float previousRoation;
        public Turret turret;
        public bool fired;
        public Vector2 origin;
        public Game g;
        Rectangle worldCoords;
        public int Health;


        // Constructor epects to see a loaded Texture
        // and a start position
        public SimplePlayerSprite(Game game, PlayerData data, Texture2D spriteImage,
                            Texture2D turretImage,Texture2D projectileImage,Point startPosition,Rectangle world,int hp) :base(game)
        {
            worldCoords = world;
            new Camera(game, Vector2.Zero, new Vector2(worldCoords.Width,worldCoords.Height)/*, player.playerID*/);
            g = game;
            pData = data;
            DrawOrder = 1;
            game.Components.Add(this);
            // Take a copy of the texture passed down
            Image = spriteImage;
            // Take a copy of the start position
            previousPosition = Position = (startPosition.ToVector2());
            // Calculate the bounding rectangle
            BoundingRect = new Rectangle((int)Position.X, (int)Position.Y, Image.Width, Image.Height);
            CollisionRect = BoundingRect;
            turret = new Turret(Position,turretImage,projectileImage, game);
            origin = new Vector2(Image.Width / 2, Image.Height / 2);
            Health = hp;

        }

        public override void Update(GameTime gameTime)
        {

            //On Pressing Escape button to exit game, a message is sent to server informing that this player has left then the game is quit for the client.
            if (InputEngine.IsKeyPressed(Keys.Escape))
            {
                IHubProxy proxy = Game.Services.GetService<IHubProxy>();
                Visible = false;
                proxy.Invoke("Left", new Object[]
                {
                    pData.playerID
                });

                Game.Exit();
            }



            turret.BoundingRect = new Rectangle((int)Position.X, (int)Position.Y, turret._tx.Width, turret._tx.Height);

            BoundingRect.X = BoundingRect.X + Image.Width / 2;
            BoundingRect.Y = BoundingRect.Y + Image.Height / 2;

            Vector2 direction = new Vector2((float)Math.Cos(rotation),
                                  (float)Math.Sin(rotation));
            direction.Normalize();
            turret.previousRotation = turret.rotation;
            if (InputEngine.IsKeyHeld(Keys.A))
                turret.rotation -= 0.05f;
            if (InputEngine.IsKeyHeld(Keys.D))
                turret.rotation += 0.05f;

            previousPosition = Position;
            if(InputEngine.IsKeyHeld(Keys.Up))
                Position += direction * speed;
            if (InputEngine.IsKeyHeld(Keys.Down))
                Position -= direction * speed;
            previousRoation = rotation;
            if (InputEngine.IsKeyHeld(Keys.Left))
                rotation -= 0.05f;
            if (InputEngine.IsKeyHeld(Keys.Right))
                rotation += 0.05f;
            delay -= gameTime.ElapsedGameTime.Milliseconds;

            if (InputEngine.IsKeyPressed(Keys.Space) && delay <= 0)
            {
                delay = 500;
                turret.CreateProjectile(Position,pData.playerID,Guid.NewGuid().ToString());
                fired = true;
                
                

            }
            if(turret.projectiles.Count > 0)
                foreach (SimpleProjectile p in turret.projectiles)
                {
                    p.Update(gameTime);
                    if (p.CollisionDetect(g))
                    {
                        //p.data.position = new Position { X = (int)p.Position.X, Y = (int)p.Position.Y };
                        IHubProxy proxy = Game.Services.GetService<IHubProxy>();
                        proxy.Invoke("Hit", new Object[]
                        {
                            p.data,
                            p.target
                    });
                    }


                }
            //foreach (SimpleProjectile p in turret.projectiles)
            //{
            //    if (!p.visible)
            //        turret.projectiles.Remove(p);
            //    break;
            //}

            // if we have moved pull back the proxy reference and send a message to the hub
            if (Position != previousPosition || rotation != previousRoation || turret.rotation != turret.previousRotation)
            {
                pData.playerPosition = new Position { X = (int)Position.X, Y = (int)Position.Y};
                pData.playerPosition.angle = rotation;
                pData.playerPosition.TurretAngle = turret.rotation;
                IHubProxy proxy = Game.Services.GetService<IHubProxy>();
                proxy.Invoke("Moved", new Object[] 
                {
                    pData.playerID,
                    pData.playerPosition,
                });

            }
            if (fired)
            {
                IHubProxy proxy = Game.Services.GetService<IHubProxy>();
                proxy.Invoke("Fired", new Object[]
                {
                   turret.projectiles.Last().data
            });
                fired = false;
            }

            BoundingRect = new Rectangle((int)Position.X, (int)Position.Y, Image.Width, Image.Height);
            CollisionRect = new Rectangle((int)Position.X, (int)Position.Y, Image.Width / 2, Image.Height / 2);



            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteFont font = Game.Services.GetService<SpriteFont>();
            SpriteBatch sp = Game.Services.GetService<SpriteBatch>();
            if (sp == null) return;
            if (Image != null && Visible)
            {
                sp.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Camera.CurrentCameraTranslation);
                sp.Draw(Image, BoundingRect, null, Color.White, rotation, origin, SpriteEffects.None, 0);
                sp.DrawString(font, pData.GamerTag, new Vector2(Position.X + 20, Position.Y - (Image.Height / 4)), Color.White);//Draws the player gamerTag
                sp.Draw(turret._tx, turret.BoundingRect, null, Color.White, turret.rotation, turret.origin, SpriteEffects.None, 1);
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
