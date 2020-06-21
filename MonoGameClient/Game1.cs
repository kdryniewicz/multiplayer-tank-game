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

namespace MonoGameClient
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont sf;
        string connectionMessage = string.Empty;
        FadeTextManager FadeManager;
        Scoreboard score;
        SpriteFont font;


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
            new GetGameInputComponent(this);
            sf = Content.Load<SpriteFont>("keyboardfont");
            // TODO: Add your initialization logic here
            serverConnection = new HubConnection("http://localhost:15878");
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


            //Sets up the method that will be used when player leaves the game.
            Action<PlayerData, List<PlayerData>> left = PlayerLeft;
            proxy.On<PlayerData, List<PlayerData>>("Left", left);


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
                    break; // break out of loop as only one player position is being updated
                           // and we have found it
                }
            }
        }


        // Only called when the client joins a game
        private void clientPlayers(List<PlayerData> otherPlayers)
        {
            foreach (PlayerData player in otherPlayers)
            {
                // Create an other player sprites in this client afte
                new OtherPlayerSprite(this, player, Content.Load<Texture2D>("Textures\\" +  player.imageName),
                                        new Point(player.playerPosition.X, player.playerPosition.Y));
                connectionMessage = player.playerID + " delivered ";
                if(!score.players.Contains(player))
                score.players.Add(player);
            }
        }

        private void clientJoined(PlayerData otherPlayerData)
        {
            score.players.Add(otherPlayerData);
            // Create an other player sprite
            new OtherPlayerSprite(this, otherPlayerData, Content.Load<Texture2D>("Textures\\" + otherPlayerData.imageName),
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
                    //connectionMessage = "Disconnected.....";
                    new FadeText(this, new Vector2(10, 10), "Disconnected..");
                    if (State.OldState == ConnectionState.Connected)
                    new FadeText(this, new Vector2(10, 10), "Lost Connection..");
                    //connectionMessage = "Lost Connection.....";
                    Connected = false;
                    break;
                case ConnectionState.Connecting:
                    new FadeText(this, new Vector2(10, 10), "Connecting..");
                    //connectionMessage = "Connecting.....";
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
            score.players.Add(player);
            // Create an other player sprites in this client afte
            new SimplePlayerSprite(this, player, Content.Load<Texture2D>("Textures\\" + player.imageName),
                                    new Point(player.playerPosition.X, player.playerPosition.Y));
           // connectionMessage = player.playerID + " created ";
            new FadeText(this, new Vector2(10, 20), string.Format("Player with ID {0} has joined the game.", player.playerID));

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
            score = new Scoreboard(new List<PlayerData>(), spriteBatch, sf, new Vector2(GraphicsDevice.Viewport.Bounds.X, GraphicsDevice.Viewport.Bounds.Y), this);
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
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                //Exit();

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
            spriteBatch.Begin();
            //spriteBatch.DrawString(sf,connectionMessage,Vector2.Zero,Color.White);
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
