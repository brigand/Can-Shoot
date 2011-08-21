namespace Can_Shooter
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public partial class Game1 : Microsoft.Xna.Framework.Game
    {

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyState = Keyboard.GetState();
            Pad1.Update();
            int maxWidth = graphics.GraphicsDevice.Viewport.Width;
            int maxHeight = graphics.GraphicsDevice.Viewport.Height;

            // Allows the game to exit
            if (Pad1.IsDown(Buttons.Back) ||
                currentKeyState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (gameStatus == GameStatus.Playing)
            {
                UpdatePlaying(gameTime, currentKeyState);
                if (level >= 2)
                {
                    UpdateLevel2(gameTime);
                }
            }
            else // Win or loss
            {
                UpdatePause();
            }

            // Update these for the next call
            PreviousKeyboardState = currentKeyState;

            base.Update(gameTime);
        }


        private void UpdatePlaying(GameTime gameTime, KeyboardState currentKeyState)
        {
            int maxWidth = graphics.GraphicsDevice.Viewport.Width;
            int maxHeight = graphics.GraphicsDevice.Viewport.Height;

            double frameTime = gameTime.ElapsedGameTime.TotalMilliseconds;
            double stickPos = Pad1.current.ThumbSticks.Right.X / 15;

            // If the new position is within bounds we allow it
            float newPos = (float)(stickPos * frameTime + angleGun);
            if (newPos > -50 && newPos < 50)
                angleGun = newPos;

            // Limit to 70% of the area to either it's right or left
            float maxPercentWidth = 0.7f;
            if (angleGun > 0)
            { // to the left
                float variation = posGun.X;
                float percent = angleGun / 50 * maxPercentWidth;
                drawGun.Width = (int)(variation * percent);
                drawGun.X = (int)posGun.X - drawGun.Width;
            }
            else
            { // to the right
                float variation = maxWidth - posGun.X;
                float percent = angleGun / 50 * maxPercentWidth;
                drawGun.Width = (int)(variation * percent);
                drawGun.X = (int)posGun.X; // -drawGun.Width * -1;
            }


            //drawGun.Width = (int)(maxWidth * 50 / 100 * (angleGun / 100));
            drawGun.Height = maxHeight * 75 / 100;
            //drawGun.X = (int)posGun.X - drawGun.Width;
            drawGun.Y = (int)posGun.Y - drawGun.Height;

            if (angleGun > 0) // To the left
                posTarget.X = (int)(posGun.X - angleGun / 50 * posGun.X);
            else // To the right
                posTarget.X = (int)((maxWidth - posGun.X) * (angleGun / -50) + posGun.X);

            posTarget.Width = 15;
            posTarget.Height = 15;

            posTarget.Y = posCan.Y + posCan.Height / 2;

            bool F12Pressed = Pad1.FrameRelease(Buttons.Start);
            bool StartPressed = Pad1.FrameRelease(Buttons.Start);

            // Allow F12 to reload the game
            if (F12Pressed || StartPressed)
            {
                level = 1;
                this.Initialize();
            }

            if (Pad1.FrameRelease(Buttons.RightTrigger))
                Shoot();

            if (score == score) // DEBUG: should say (score == 5)
            {
                double hueIncrement = gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0;
                if (hueDirection == Direction.Right)
                    hueFade += hueIncrement;
                else
                    hueFade -= hueIncrement;

                // Reverse the directions at the end of the legal values
                // We allow 1% margin of error
                if (hueFade >= 0.99)
                {
                    hueFade = 0.99;
                    hueDirection = Direction.Left;
                }
                else if (hueFade <= 0.01)
                {
                    hueFade = 0.01;
                    hueDirection = Direction.Right;
                }

                //canColor = GameHelper.HSLColor(hueFade, 1.0, 0.5);

                //     ----------------------            DEBUG      -----------------------------
                //     ----------------------            DEBUG      -----------------------------
                //     ----------------------            DEBUG      -----------------------------


                canColor = GameHelper.HSLColor(hueFade, debug_s, debug_l);
            }
        }

        float debug_s = 0.5f;
        float debug_l = 0.5f;


        private void UpdateLevel2(GameTime gameTime)
        {
            int maxWidth = graphics.GraphicsDevice.Viewport.Width;
            int maxHeight = graphics.GraphicsDevice.Viewport.Height;

            if (posCan.X < maxWidth * 10 / 100)
                canDir = Direction.Right;
            else if (posCan.X > maxWidth * 80 / 100)
                canDir = Direction.Left;

            if (canDir == Direction.Right)
                posCan.X += (int)(gameTime.ElapsedGameTime.Milliseconds * maxWidth / 14000.0 * level);
            else if (canDir == Direction.Left)
                posCan.X -= (int)(gameTime.ElapsedGameTime.Milliseconds * maxWidth / 14000.0 * level);
        }

        private void UpdatePause()
        {
            bool aPress = Pad1.FrameRelease(Buttons.A);
            bool xPress = Pad1.FrameRelease(Buttons.X);
            bool bPress = Pad1.FrameRelease(Buttons.B);
            bool yPress = Pad1.FrameRelease(Buttons.Y);
            bool startPress = Pad1.FrameRelease(Buttons.Start);

            if (aPress || xPress || bPress || yPress || startPress)
            {
                // Only go to the next level if you win
                if (gameStatus == GameStatus.Win)
                    level += 1;

                // Start completly over except for loading resources
                Initialize();
            }
            int maxWidth = graphics.GraphicsDevice.Viewport.Width;
            int maxHeight = graphics.GraphicsDevice.Viewport.Height;

            posWinLose.X = maxWidth * 5 / 100;
            posWinLose.Y = maxHeight * 5 / 100;
            posWinLose.Width = maxWidth * 90 / 100;
            posWinLose.Height = maxHeight * 90 / 100;
        }
    }

}