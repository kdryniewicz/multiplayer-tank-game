using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Engines;
using Microsoft.Xna.Framework.Input;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sprites
{
  public class Turret
    {

        public Texture2D _tx;
        public Texture2D projectileImage;
        public float rotation = 0.0f;
        public float previousRotation;
        public Vector2 origin;
        public List<SimpleProjectile> projectiles = new List<SimpleProjectile>();

        public Rectangle BoundingRect;

        public Turret(Vector2 p,Texture2D t,Texture2D t2,Game g)
        {
            _tx = t;
            projectileImage = t2;
           BoundingRect = BoundingRect = new Rectangle((int)p.X, (int)p.Y, _tx.Width, _tx.Height);
            origin = new Vector2(_tx.Width / 2, _tx.Height / 2);
            previousRotation = rotation;
        }

        public void CreateProjectile(Vector2 pos,string header,string p)
        {
            SimpleProjectile temp = new SimpleProjectile(projectileImage, new Vector2(pos.X - projectileImage.Width/2,pos.Y-projectileImage.Height/2), 5f, rotation,header,p);
            projectiles.Add(temp);
        }

    }
}
