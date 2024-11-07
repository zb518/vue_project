using SkiaSharp;

namespace PPE.Core
{

    public class ValidatorCodeHelper
    {
        public static string CreateCode(int length = 4)
        {
            string sources = "ABCDEFGHJKLMNPRSTWXYabcdefghjklmnprstwxy123456789";
            Random random = new Random();
            string code = "";
            for (int i = 0; i < length; ++i)
            {
                code += sources[random.Next(sources.Length)];
            }
            return code;
        }

        public static byte[] CreatePng(string code)
        {
            int width = 120;
            int height = 50;
            Random random = new Random();
            SKBitmap image = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            SKCanvas canvas = new(image);
            canvas.DrawColor(SKColors.White);
            SKColor[] colors = new[] { SKColors.Red, SKColors.Black, SKColors.DarkRed, SKColors.DarkBlue, SKColors.Green, SKColors.Orange, SKColors.Brown, SKColors.DarkCyan, SKColors.Purple };
            //画线
            for (int i = 0; i < (width * height * 0.012); ++i)
            {
                SKPaint drawStyle = new();
                drawStyle.Color = new(Convert.ToUInt32(random.Next(Int32.MaxValue)));
                canvas.DrawLine(random.Next(0, width), random.Next(0, height), random.Next(0, width), random.Next(0, height), drawStyle);
            }

            for (int i = 0; i < code.Length; ++i)
            {
                using (SKPaint drawStyle = new())
                {
                    drawStyle.Color = colors[random.Next(colors.Length)];
                    drawStyle.TextSize = height - 10;
                    drawStyle.StrokeWidth = 1.5f;

                    float emHeight = height - (float)height * (float)0.17;
                    float emWidth = ((float)width * (float).13) * (((float)i) + (float).5) * (float)1.5;
                    canvas.DrawText(code[i].ToString(), emWidth, emHeight, drawStyle);
                }
            }
            //画噪点
            for (int i = 0; i < (width * height * .3); ++i)
            {
                image.SetPixel(random.Next(0, width), random.Next(0, height), new SKColor(Convert.ToUInt32(random.Next(Int32.MaxValue))));
            }

            using var img = SKImage.FromBitmap(image);
            using SKData p = image.Encode(SKEncodedImageFormat.Png, 100);
            return p.ToArray();
        }
    }
}