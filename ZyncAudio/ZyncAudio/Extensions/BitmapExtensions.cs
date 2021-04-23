using System.Drawing;

namespace ZyncAudio.Extensions
{
    public static class BitmapExtensions
    {

        public static void Tint(this Bitmap bitmap, Color newColor)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);
                    if (pixelColor.A != 0)
                    {
                        bitmap.SetPixel(x, y, newColor);
                    }
                }
            }
        }

    }
}