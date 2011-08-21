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

namespace InputManagement
{
    /// <summary>
    /// GamePadInput holds the status of this frame's and the previous frame's 
    /// GamePadState objects.
    /// </summary>
    public class GamePadInput
    {
        public GamePadState previous;
        public GamePadState current;
        protected PlayerIndex player;

        /// <summary>
        /// Create a new GamepadInput object
        /// </summary>
        /// <param name="player">Which controller you want to watch</param>
        public GamePadInput(PlayerIndex player)
        {
            this.player = player;
            previous = GamePad.GetState(player);
            current = GamePad.GetState(player);

        }

        public GamePadInput()
        {
            this.player = PlayerIndex.One;
            previous = GamePad.GetState(player);
            current = GamePad.GetState(player);
        }

        public bool FramePress(Buttons button)
        {
            bool pressedNow = previous.IsButtonDown(button);
            bool upBefore = current.IsButtonUp(button);
            return pressedNow && upBefore;
        }

        public bool FrameRelease(Buttons button)
        {
            bool upNow = previous.IsButtonUp(button);
            bool pressedBefore = current.IsButtonDown(button);
            return pressedBefore && upNow;
        }

        public bool IsDown(Buttons button)
        {
            return current.IsButtonDown(button);
        }

        public bool IsUp(Buttons button)
        {
            return current.IsButtonDown(button);
        }

        /// <summary>
        /// Call this at the begining of each Update call
        /// </summary>
        public void Update()
        {
            previous = current;
            current = GamePad.GetState(player);
        }
    }
}
