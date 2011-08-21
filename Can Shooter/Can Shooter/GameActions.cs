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
        protected void AnimateCan(GameTime time)
        {
            double passed = time.TotalGameTime.TotalMilliseconds - CanAnimateTime.TotalMilliseconds;

            // Percent of max animation time
            double percent = passed / 1500.0;

            double width = CanDest.X - CanSource.X;
            double height = CanDest.Y;

            DebugString = String.Format("passed: {0} \npercent: {1} \nwidth: {2} \nheight: {3}",
                passed,
                percent,
                width,
                height);

            PosCan.X = Convert.ToInt32(CanSource.X + (width * percent));
            PosCan.Y = Convert.ToInt32(height - height * Math.Sin(percent * MathHelper.Pi));


            if (passed > 1500.0)
                CanAnimating = false;
        }

        protected void Shoot(GameTime time)
        {
            float panAmmount = PosGun.X / MaxWidth * 2 - 1;

            if (Rand.Next(0, 1) == 0)
                Shot1.Play(1.0f, 0.0f, panAmmount);
            else
                Shot2.Play(1.0f, 0.0f, panAmmount);

            // True if the player hits the can
            bool hitTarget = PosTarget.Intersects(PosCan);

            if (hitTarget)
            {
                CanAnimating = true;
                Score++;
                if (Score >= 6)
                {
                    // Level is incremented when the user presses a key to continue
                    PlayingStatus = GameStatus.Win;
                    CanSource = new Vector2(Rand.Next(-10, MaxWidth + 10), 50);
                    return;
                }
                else
                {
                    CanSource = new Vector2(PosCan.X, PosCan.Y);
                    CanDest = new Vector2(Rand.Next(MaxWidth*95/100), PosStand.Y);   
                    NewRound();
                    CanAnimateTime = time.TotalGameTime;
                }
            }

            AmmoCount -= 1;
            if (AmmoCount <= 0)
                PlayingStatus = GameStatus.Loss;
        }
    }
}