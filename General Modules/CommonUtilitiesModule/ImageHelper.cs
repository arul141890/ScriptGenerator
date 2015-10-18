// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageHelper.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The image helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule
{
    public static class ImageHelper
    {

        public static void GetAspectRatio(int x, int y, out int ax, out int ay)
        {
            ax = x/Gcd(x, y);
            ay = y/Gcd(x, y);
        }



        private static int Gcd(int a, int b)
        {
            while (b != 0)
            {
                var remainder = a%b;
                a = b;
                b = remainder;
            }

            return a;
        }

    }
}