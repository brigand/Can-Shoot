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
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyState = Keyboard.GetState();
            Pad1.Update();
            MaxWidth = graphics.GraphicsDevice.Viewport.Width;
            MaxHeight = graphics.GraphicsDevice.Viewport.Height;

            // Allows the game to exit
            if (Pad1.IsDown(Buttons.Back) ||
                currentKeyState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (PlayingStatus == GameStatus.Playing)
            {
                UpdatePlaying(gameTime, currentKeyState);
                if (CanAnimating)
                {
                    AnimateCan(gameTime);
                }

                // If the can is moved in the above if, we don't need
                // the side-to-side animation now.
                // If it's not Animating, we do that here (assuming level 2 or more)
                else if (Level >= 2)
                {
                    UpdateLevel2(gameTime);
                }
            }
            else // Win or loss
            {
                UpdatePause(gameTime);
            }

            // Update these for the next call
            PreviousKeyboardState = currentKeyState;

            base.Update(gameTime);
        }

        private void UpdatePlaying(GameTime gameTime, KeyboardState currentKeyState)
        {
            double frameTime = gameTime.ElapsedGameTime.TotalMilliseconds;
            double stickPos = Pad1.current.ThumbSticks.Right.X / 15;

            // If the new position is within bounds we allow it
            float newPos = (float)(stickPos * frameTime + AngleGun);
            if (newPos > -50 && newPos < 50)
                AngleGun = newPos;

            // Limit to 70% of the area to either it's right or left
            float maxPercentWidth = 0.7f;
            if (AngleGun > 0)
            { // to the left
                float variation = PosGun.X;
                float percent = AngleGun / 50 * maxPercentWidth;
                PosDrawGun.Width = (int)(variation * percent);
                PosDrawGun.X = (int)PosGun.X - PosDrawGun.Width;
            }
            else
            { // to the right
                float variation = MaxWidth - PosGun.X;
                float percent = AngleGun / 50 * maxPercentWidth;
                PosDrawGun.Width = (int)(variation * percent);
                PosDrawGun.X = (int)PosGun.X; // -PosDrawGun.Width * -1;
            }


            //PosDrawGun.Width = (int)(MaxWidth * 50 / 100 * (AngleGun / 100));
            PosDrawGun.Height = MaxHeight * 75 / 100;
            //PosDrawGun.X = (int)PosGun.X - PosDrawGun.Width;
            PosDrawGun.Y = (int)PosGun.Y - PosDrawGun.Height;

            if (AngleGun > 0) // To the left
                PosTarget.X = (int)(PosGun.X - AngleGun / 50 * PosGun.X);
            else // To the right
                PosTarget.X = (int)((MaxWidth - PosGun.X) * (AngleGun / -50) + PosGun.X);

            PosTarget.Width = 15;
            PosTarget.Height = 15;

            PosTarget.Y = PosCan.Y + PosCan.Height / 2;

            bool F12Pressed = Pad1.FrameRelease(Buttons.Start);
            bool StartPressed = Pad1.FrameRelease(Buttons.Start);

            // Allow F12 to reload the game
            if (F12Pressed || StartPressed)
            {
                Level = 1;
                this.Initialize();
            }

            if (Pad1.FrameRelease(Buttons.RightTrigger))
                Shoot(gameTime);

            if (Score == 5) // DEBUG: should say (Score == 5)
            {
                double hueIncrement = gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0;
                if (HueDirection == Direction.Right)
                    HueFade += hueIncrement;
                else
                    HueFade -= hueIncrement;

                // Reverse the directions at the end of the legal values
                // We allow 1% margin of error
                if (HueFade >= 0.99)
                {
                    HueFade = 0.99;
                    HueDirection = Direction.Left;
                }
                else if (HueFade <= 0.01)
                {
                    HueFade = 0.01;
                    HueDirection = Direction.Right;
                }

                ColorCan = GameHelper.HSLColor(HueFade, 1.0, 0.5);
            }
        }

        /// <summary>
        /// In every Level after the first the can moves constantly.
        /// This code is only run when the player has passed the first Level.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateLevel2(GameTime gameTime)
        {
            if (PosCan.X < MaxWidth * 10 / 100)
                CanDirection = Direction.Right;
            else if (PosCan.X > MaxWidth * 80 / 100)
                CanDirection = Direction.Left;

            if (CanDirection == Direction.Right)
                PosCan.X += (int)(gameTime.ElapsedGameTime.Milliseconds * MaxWidth / 14000.0 * Level);
            else if (CanDirection == Direction.Left)
                PosCan.X -= (int)(gameTime.ElapsedGameTime.Milliseconds * MaxWidth / 14000.0 * Level);
        }

        /// <summary>
        /// Called when the game is paused.
        /// For example, after the end of a Level (Win or loss).
        /// </summary>
        private void UpdatePause(GameTime time)
        {
            bool aPress = Pad1.FrameRelease(Buttons.A);
            bool xPress = Pad1.FrameRelease(Buttons.X);
            bool bPress = Pad1.FrameRelease(Buttons.B);
            bool yPress = Pad1.FrameRelease(Buttons.Y);
            bool startPress = Pad1.FrameRelease(Buttons.Start);

            if (aPress || xPress || bPress || yPress || startPress)
            {
                // Only go to the next Level if you win
                if (PlayingStatus == GameStatus.Win)
                    Level += 1;

                // Start completly over except for loading resources
                Initialize();
                CanAnimateTime = time.TotalGameTime;
                CanAnimating = true;
            }

            PosWinLose.X = MaxWidth * 5 / 100;
            PosWinLose.Y = MaxHeight * 5 / 100;
            PosWinLose.Width = MaxWidth * 90 / 100;
            PosWinLose.Height = MaxHeight * 90 / 100;
        }
    }
}