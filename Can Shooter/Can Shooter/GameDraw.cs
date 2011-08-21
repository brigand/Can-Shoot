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
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // The background color shouldn't ever be seen
            GraphicsDevice.Clear(Color.CornflowerBlue);

            FirstBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            FirstBatch.Draw(TexBg, PosBg, Color.White);
            FirstBatch.Draw(TexStand, PosStand, Color.White);

            DrawAmmo();

            DrawCan();
            DrawGun();

            DrawDebugInfo();

            FirstBatch.Draw(TexTarget, PosTarget, Color.White);

            string scoreText = Score.ToString() + "/6\nLvl " + Level.ToString();
            Vector2 scorePos = new Vector2(MaxWidth * 85 / 100, 15);
            FirstBatch.DrawString(ScoreFont, scoreText, scorePos, Color.SteelBlue);


            if (PlayingStatus == GameStatus.Win)
                FirstBatch.Draw(TexWin, PosWinLose, Color.White);
            else if (PlayingStatus == GameStatus.Loss)
                FirstBatch.Draw(TexLose, PosWinLose, Color.White);



            FirstBatch.End();

            base.Draw(gameTime);
        }

        protected void DrawCan()
        {
            if (CanDirection == Direction.Left)
            {
                FirstBatch.Draw(TexCan, PosCan, Color.Gray);
                FirstBatch.Draw(TexCan, PosCan, ColorCan);
            }
            else
            {
                FirstBatch.Draw(TexCan, PosCan, null, Color.Gray, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
                FirstBatch.Draw(TexCan, PosCan, null, ColorCan, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            }
        }

        protected void DrawAmmo()
        {
            for (int i = 0; i <= AmmoCount; i++)
            {
                Rectangle posAmmoTemp = new Rectangle();
                posAmmoTemp.Width = MaxWidth * 3 / 100;
                posAmmoTemp.Height = MaxHeight * 6 / 100;
                posAmmoTemp.X = i * posAmmoTemp.Width - posAmmoTemp.Width;
                posAmmoTemp.Y = MaxHeight * 85 / 100;

                SpriteEffects effect = i % 2 == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                FirstBatch.Draw(TexAmmo, posAmmoTemp, null, Color.White, 0, Vector2.Zero, effect, 0);
            }
        }

        protected void DrawDebugInfo()
        {
#if DEBUG
            string debugInfo = String.Format("Angle: {0}\nGun Width: {1}\nGun Height: {2}\nGun Position: ({3}, {4})\n"
            + "Target X: {5}\n{6}\nAmmo: {7}\n Can Pos: ({8}, {9})\n",
                AngleGun, 
                PosDrawGun.Width, PosDrawGun.Height, PosDrawGun.X, PosDrawGun.Y, 
                PosTarget.X, PlayingStatus, AmmoCount,
                PosCan.X, PosCan.Y);

            debugInfo = DebugString + "\n" + debugInfo;

            FirstBatch.DrawString(DebugFont, debugInfo, new Vector2(1, 1), Color.Red);

            // Debug tests to show AnimateCan
            FirstBatch.Draw(TexTarget, CanSource, Color.Red);
            FirstBatch.Draw(TexTarget, CanDest, Color.Plum);

#endif
        }

        protected void DrawGun()
        {
            // Draw the gun respecting negative widths
            if (PosDrawGun.Width > 0)
            {
                FirstBatch.Draw(TexGun, PosDrawGun, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
            }
            else
            {
                Rectangle drawGunBackwords = new Rectangle(PosDrawGun.X, PosDrawGun.Y, PosDrawGun.Width * -1, PosDrawGun.Height);
                FirstBatch.Draw(TexGun, drawGunBackwords, null, Color.White, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            }
        }
    }
}