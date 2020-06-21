using CommonData;
using textInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.AspNet.SignalR.Client;
using System;
using Sprites;
using GameComponentNS;
using System.Collections.Generic;
using CameraNS;
using Engine.Engines;
using Collectables;
using Microsoft.AspNet.SignalR.Messaging;

namespace GameClient
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        Camera c;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont sf;
        string connectionMessage = string.Empty;
        FadeTextManager FadeManager;
        Scoreboard score;
        SpriteFont font;

        Texture2D BG;
        Vector2 worldCoords;
        private Rectangle worldRect;
        List<Collectable> collectables = new List<Collectable>();


        HubConnection serverConnection;
        IHubProxy proxy;

        public bool Connected { get; set; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Helpers.GraphicsDevice = GraphicsDevice;
            new InputEngine(this);
            // TODO: Add your initialization logic here
            serverConnection = new HubConnection("http://localhost:2050"); // Remember to change the port here
            //serverConnection = new HubConnection("http://g-teamcasualgames.azurewebsites.net");
            serverConnection.StateChanged += severConnection_StateChanged;
            proxy = serverConnection.CreateHubProxy("GameHub");
            serverConnection.Start();



            Action<PlayerData> joined = clientJoined;
            proxy.On<PlayerData>("Joined", joined);

            Action<List<PlayerData>> currentPlayers = clientPlayers;
            proxy.On<List<PlayerData>>("CurrentPlayers", currentPlayers);

            Action<string, Position> otherMove = clientOtherMoved;
            proxy.On<string, Position>("OtherMove", otherMove);

            Action<int, int> getWorldSize = SetWorldSize;
            proxy.On<int, int>("SendWorldSize", getWorldSize);

            Action<PlayerData, List<PlayerData>> left = PlayerLeft;
            proxy.On<PlayerData, List<PlayerData>>("Left", left);

            Action<ProjectileData> hitreg = hitRegistered;
            proxy.On<ProjectileData>("HitReg", hitreg);

            Action<ProjectileData> fired = ProjectileFired;
            proxy.On<ProjectileData>("Fired", fired);

            Action<List<CollectableData>> createCols = createCollectibles;
            proxy.On<List<CollectableData>>("CreateCollectibles", createCols);

            Action checkPs = playerCheck;
            proxy.On("IsGameReady", checkPs);

            Action<string, CollectableData> collected = collectableCollected;
            proxy.On<string, CollectableData>("CollectableCollected", collected);



            FadeManager = new FadeTextManager(this);

            Services.AddService<IHubProxy>(proxy);

            FadeManager = new FadeTextManager(this);
            base.Initialize();
        }


        //Code Client-Side for leaving server.
        private void PlayerLeft(PlayerData player, List<PlayerData> otherPlayers)
        {
            //This method looks for the player that just left and hides him from other clients.
            foreach (var p in Components)
            {
                if (p.GetType() == typeof(OtherPlayerSprite) //look through otherplayers by comparing with player that was passed to it through playerID.
                    && ((OtherPlayerSprite)p).pData.playerID == player.playerID)
                {
                    OtherPlayerSprite found = ((OtherPlayerSprite)p); //Once we got it, set found to p.
                    found.Visible = false;//Hide the player that left.
                    break; //Break out of the loop as soon as player is found in the otherplayers collection as we have what we wanted.
                }
            }

            new FadeText(this, new Vector2(10, 20), string.Format("{0} has left the game.", player.GamerTag));
        }


        private void playerCheck()
        {

        }

        private void hitRegistered(ProjectileData projectile)
        {

            foreach (var player in Components)
            {
                if (player.GetType() == typeof(OtherPlayerSprite)
                    && ((OtherPlayerSprite)player).pData.playerID == projectile.ID)
                {
                    OtherPlayerSprite p = ((OtherPlayerSprite)player);

                    foreach (SimpleProjectile proj in p.turret.projectiles)
                    {
                        if (proj.data.projectileID == projectile.projectileID)
                        {
                            proj.visible = false;
                            break;
                        }
                    }
                    break;
                }
            }
        }


        private void SetWorldSize(int X, int Y)
        {
            worldCoords = new Vector2(X, Y);
            worldRect = new Rectangle(new Point(0, 0), worldCoords.ToPoint());
        }

        private void ProjectileFired(ProjectileData projectile)
        {
            foreach (var player in Components)
            {
                if (player.GetType() == typeof(OtherPlayerSprite)
                    && ((OtherPlayerSprite)player).pData.playerID == projectile.ID)
                {
                    OtherPlayerSprite p = ((OtherPlayerSprite)player);
                    p.turret.CreateProjectile(new Vector2(p.Position.X, p.Position.Y), projectile.ID, projectile.projectileID);
                }
            }
        }

        private void clientOtherMoved(string playerID, Position newPos)
        {
            // iterate over all the other player components 
            // and check to see the type and the right id
            foreach (var player in Components)
            {
                if (player.GetType() == typeof(OtherPlayerSprite)
                    && ((OtherPlayerSprite)player).pData.playerID == playerID)
                {
                    OtherPlayerSprite p = ((OtherPlayerSprite)player);
                    p.pData.playerPosition = newPos;
                    p.Position = new Point(p.pData.playerPosition.X, p.pData.playerPosition.Y);
                    p.rotation = p.pData.playerPosition.angle;
                    p.turret.rotation = p.pData.playerPosition.TurretAngle;
                    p.turret.BoundingRect = new Rectangle(p.Position.X, p.Position.Y, p.turret._tx.Width, p.turret._tx.Height);
                    if (score.players.Contains(p.pData))
                    {

                    }
                    break; // break out of loop as only one player position is being updated
                           // and we have found it
                }
            }
        }

        private void collectableCollected(string playerID, CollectableData c)
        {
            foreach (Collectable collectable in collectables)
            {
                if (collectable.collectableData.ID == c.ID)
                {
                    foreach (PlayerData p in score.players)
                    {
                        if (p.playerID == playerID)
                            p.Score += c.worth;
                    }
                    collectable.Visible = false;

                    break;
                }
            }
        }
        // Only called when the client joins a game
        private void clientPlayers(List<PlayerData> otherPlayers)
        {
            foreach (PlayerData player in otherPlayers)
            {
                // Create an other player sprites in this client afte
                new OtherPlayerSprite(this, player, Content.Load<Texture2D>("Textures\\" + player.imageName), Content.Load<Texture2D>("Textures\\" + player.turretName), Content.Load<Texture2D>("Textures\\projectile"),
                                        new Point(player.playerPosition.X, player.playerPosition.Y));
                connectionMessage = player.playerID + " delivered ";
                if (!score.players.Contains(player))
                    score.players.Add(player);
            }
        }

        private void clientJoined(PlayerData otherPlayerData)
        {
            score.players.Add(otherPlayerData);
            // Create an other player sprite
            new OtherPlayerSprite(this, otherPlayerData, Content.Load<Texture2D>("Textures\\" + otherPlayerData.imageName), Content.Load<Texture2D>("Textures\\" + otherPlayerData.turretName), Content.Load<Texture2D>("Textures\\projectile"),
                                    new Point(otherPlayerData.playerPosition.X, otherPlayerData.playerPosition.Y));
        }

        private void severConnection_StateChanged(StateChange State)
        {
            switch (State.NewState)
            {
                case ConnectionState.Connected:
                    //connectionMessage = "Connected.....";
                    new FadeText(this, new Vector2(10, 10), "Connected..");
                    Connected = true;
                    startGame();

                    break;
                case ConnectionState.Disconnected:
                    new FadeText(this, new Vector2(10, 10), "Fuck..");
                    if (State.OldState == ConnectionState.Connected)
                        connectionMessage = "Lost Connection.....";
                    Connected = false;
                    break;
                case ConnectionState.Connecting:
                    connectionMessage = "Connecting.....";
                    Connected = false;
                    break;
            }
        }
        private void startGame()
        {
            proxy.Invoke<PlayerData>("Join")
                .ContinueWith(
                    (p) => { //Do with p
                        if (p.Result == null)
                            connectionMessage = "No Player Data Returned";
                        else
                        {
                            CreatePlayer(p.Result);
                            //create the player
                        }
                    });


        }

        private void CreatePlayer(PlayerData player)
        {
            proxy.Invoke("SendWorldSize");
            score.players.Add(player);
            // Create an other player sprites in this client afte
            new SimplePlayerSprite(this, player, Content.Load<Texture2D>("Textures\\" + player.imageName), Content.Load<Texture2D>("Textures\\" + player.turretName), Content.Load<Texture2D>("Textures\\projectile"),
                                    new Point(player.playerPosition.X, player.playerPosition.Y), worldRect, 10);
            // Setup Collectables
            proxy.Invoke<bool>("IsGameReady")
               .ContinueWith(
                   (p) => { //Do with p
                       if (p.Result == false)
                           // connectionMessage = "Game is not ready";
                           new FadeText(this, new Vector2(10, 20), string.Format("Game Not Ready!"));
                       else
                       {
                           //new FadeText(this, new Vector2(10, 20), string.Format("Game IS Ready!!!!"));
                           proxy.Invoke("CreateCollectibles");
                       }
                   });

            // connectionMessage = player.playerID + " created ";
            new FadeText(this, new Vector2(10, 20), string.Format("Player with ID {0} has joined the game.", player.playerID));

        }


        private void createCollectibles(List<CollectableData> Collectables)
        {
            Texture2D tx = Content.Load<Texture2D>(@"Textures\collectable");
            foreach (var c in Collectables)
            {
                collectables.Add(new Collectable(this, c, tx, c.position));
            }
            new FadeText(this, new Vector2(10, 20), string.Format("Game is started! There are: {0} collectibles to pick up.", Collectables.Count));
        }




        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Services.AddService<SpriteBatch>(spriteBatch);

            font = Content.Load<SpriteFont>("Message");

            Services.AddService<SpriteFont>(font);
            score = new Scoreboard(new List<PlayerData>(), spriteBatch, font, new Vector2(GraphicsDevice.Viewport.Bounds.X, GraphicsDevice.Viewport.Bounds.Y), this);

            BG = Content.Load<Texture2D>("Textures\\bg");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (Connected)

            {
                DrawPlay();

            }

            else

            {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, connectionMessage, new Vector2(20, 20), Color.White);
                spriteBatch.End();

            }


            base.Draw(gameTime);
        }
        private void DrawPlay()

        {

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Camera.CurrentCameraTranslation);
            spriteBatch.Draw(BG, worldRect, Color.White);
            spriteBatch.End();

        }
    }
}
