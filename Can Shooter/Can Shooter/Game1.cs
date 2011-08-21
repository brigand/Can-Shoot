using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Bagnardi.Game;
using InputManagement;

namespace Can_Shooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class CanGame : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Stores a reference to the game instance.
        /// This allows interfacing with the game propertys/methods from other
        /// classes.
        /// </summary>
        public static CanGame game;

        GraphicsDeviceManager graphics;
        SpriteBatch FirstBatch;

        // Textures
        Texture2D TexAmmo;
        Texture2D TexBg;
        Texture2D TexCan;
        Texture2D TexGun;
        Texture2D TexStand;
        Texture2D TexTarget;
        Texture2D TexWin;
        Texture2D TexLose;

        // Vectors for data used in rendering
        Vector2 PosGun;
        Rectangle PosDrawGun;
        float AngleGun;
        Rectangle PosBg;
        Rectangle PosStand;
        Rectangle PosTarget;
        Rectangle PosWinLose;
        Rectangle PosCan;
        Color ColorCan;

        // User input device states
        KeyboardState PreviousKeyboardState;
        //GamePadState Pad1;
        GamePadInput Pad1;
        
        // Fonts
        SpriteFont DebugFont;
        SpriteFont ScoreFont;

        // Sound effects
        SoundEffect Shot1;
        SoundEffect Shot2;

        // Game values
        byte AmmoCount;
        byte Score;
        GameStatus PlayingStatus;
        byte Level;
        Random Rand = new Random();
        Direction HueDirection;
        double HueFade;

        // Used for the "can jump" after getting hit
        bool CanAnimating;
        Vector2 CanSource;
        Vector2 CanDest;
        TimeSpan CanAnimateTime;

        // Screen properties
        int MaxWidth;
        int MaxHeight;

        // Used in Level2 code
        Direction CanDirection;

        // This string is show when debugging
        string DebugString;

        enum Direction { Left, Right }

        enum GameStatus { Playing, Win, Loss }

        public CanGame()
        {
            game = this;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            //graphics.IsFullScreen = true;

            // DEBUG
            /*
            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(50);
             */

            Level = 1;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// 
        /// This is called at the start of a new level.  Values that shouldn't be modified then
        /// should go in the class constructor.
        /// </summary>
        protected override void Initialize()
        {
            AmmoCount = 11;
            Score = 0;
            PlayingStatus = GameStatus.Playing;
            PosWinLose = Rectangle.Empty;

            MaxWidth = graphics.GraphicsDevice.Viewport.Width;
            MaxHeight = graphics.GraphicsDevice.Viewport.Height;

            int posCanX = Rand.Next(0, 1) == 1 ? MaxWidth * 150 / 100 : MaxWidth * -150 / 100;
            PosCan = new Rectangle(posCanX, 1, MaxWidth / 20, MaxHeight / 10);
            CanSource = new Vector2(PosCan.X, PosCan.Y);
            PosBg = new Rectangle(-10, -10, MaxWidth + 10, MaxHeight + 10);
            PosGun = new Vector2(Rand.Next(MaxWidth * 35 / 100, MaxWidth * 65 / 100), 0);
            PosStand = new Rectangle(30, MaxHeight / 10 * 6, MaxWidth + 30, MaxHeight / 10 * 4);

            NewRound();

            // Get the keyboard and gamepad states for use in the first Update call
            PreviousKeyboardState = Keyboard.GetState();
            Pad1 = new GamePadInput();

            HueDirection = Direction.Right;
            HueFade = Rand.NextDouble();

            base.Initialize();
        }

        /// <summary>
        /// Start a new round of gameplay.  This can be used in Initialize and
        /// after a shot is fired to move the elements around.
        /// </summary>
        private void NewRound()
        {
            PosTarget = Rectangle.Empty;

            CanDest.X = Rand.Next(MaxWidth * 20 / 100, MaxWidth * 80 / 100);
            CanDest.Y = Rand.Next(PosStand.Y, PosStand.Y + 15);
            PosGun.Y = MaxHeight * 105 / 100;

            float gunMaxX = PosGun.X + MaxWidth / 20 < MaxWidth * 70 / 100 ? MaxWidth * 70 / 100 : PosGun.X + MaxWidth / 20;
            float gunMinX = PosGun.X - MaxWidth / 20 < MaxWidth * 30 / 100 ? MaxWidth * 30 / 100 : PosGun.X - MaxWidth / 20;
            PosGun.X = Rand.Next((int)gunMinX, (int)gunMaxX);

            CanAnimating = true;

            AngleGun = Rand.Next(-15, 15);

            double hue = Rand.NextDouble();
            
            ColorCan = GameHelper.HSLColor(hue, 1.0, 0.5);
            CanDirection = CanDest.X > CanSource.X ? Direction.Right : Direction.Left;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            FirstBatch = new SpriteBatch(GraphicsDevice);

            TexAmmo = Content.Load<Texture2D>("Ammo");
            TexBg = Content.Load<Texture2D>("bg");
            TexCan = Content.Load<Texture2D>("Can");
            TexGun = Content.Load<Texture2D>("Gun");
            TexStand = Content.Load<Texture2D>("Stand");
            TexTarget = Content.Load<Texture2D>("Glow");
            TexWin = Content.Load<Texture2D>("Win-Graphic");
            TexLose = Content.Load<Texture2D>("Lose-Graphic");

            DebugFont = Content.Load<SpriteFont>("Courier New");
            ScoreFont = Content.Load<SpriteFont>("Segoe UI Mono");

            Shot1 = Content.Load<SoundEffect>("shot1");
            Shot2 = Content.Load<SoundEffect>("shot2");
        }
    }
}
