using System;

namespace Can_Shooter
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CanGame game = new CanGame())
            {
                game.Run();
            }
        }
    }
#endif
}

