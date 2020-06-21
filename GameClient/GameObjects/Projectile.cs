using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CommonData;
using CameraNS;

namespace Sprites
{
    public class SimpleProjectile
    {
        public Texture2D Image;
        public string target;
        public Rectangle BoundingRect;
        public ProjectileData data;
        public Vector2 Position;
        public float Speed;
        public Vector2 direction;
        public string header;
        public bool visible;

        public SimpleProjectile(Texture2D image, Vector2 pos, float s, float r,string h,string p)
        {
            header = h;
            Image = image;
            Position = pos;
            Speed = s;
            direction = new Vector2((float)Math.Cos(r),
                                  (float)Math.Sin(r));
            direction.Normalize();
            BoundingRect = new Rectangle((int)Position.X, (int)Position.Y, Image.Width, Image.Height);
            visible = true;
            data = new ProjectileData
            {
                ID = header,
                projectileID = p
            };

        }

        public void Update(GameTime gameTime)
        {
         
            if (!visible) return;
            BoundingRect = new Rectangle((int)Position.X, (int)Position.Y, Image.Width, Image.Height);
            Position += direction * Speed;

        }

        public void Draw(GameTime gameTime,SpriteBatch sp)
        {
            if (!visible) return;
            sp.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Camera.CurrentCameraTranslation);
            sp.Draw(Image, BoundingRect, Color.White);
            sp.End();
        }

        public bool CollisionDetect(Game game)
        {
            if (!visible) return false;
            foreach (var player in game.Components)
                if (player.GetType() == typeof(OtherPlayerSprite))
                {
                    OtherPlayerSprite p = ((OtherPlayerSprite)player);

                    if (BoundingRect.Intersects(p.CollisionRect))
                    {
                        visible = false;
                        target = p.pData.playerID;
                        return true;
                    }
                }
                    return false;
                
        }
    }
}
