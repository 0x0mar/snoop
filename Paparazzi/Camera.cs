using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Paparazzi
{
	public class Camera
	{
		/// <summary>
		/// Takes a screenshot of the entire virtual desktop and saves it to the given file location
		/// </summary>
		/// <param name="filename"></param>
		public void TakeScreenshot(string filename)
		{
			int screenWidth = Convert.ToInt32(SystemParameters.VirtualScreenWidth);
			int screenHeight = Convert.ToInt32(SystemParameters.VirtualScreenHeight);
			int screenLeft = Convert.ToInt32(SystemParameters.VirtualScreenLeft);
			int screenTop = Convert.ToInt32(SystemParameters.VirtualScreenTop);
			double scale = 0.2;
			using (var bmp = new Bitmap(screenWidth, screenHeight))
			{
				using (var g = Graphics.FromImage(bmp))
				{
					g.CopyFromScreen(screenLeft, screenTop, 0, 0, bmp.Size);
					var convertedBmp = ConvertImage(bmp, new Size((int)(screenWidth * scale), (int)(screenHeight * scale)), true);
					convertedBmp.Save(filename);
					convertedBmp.Dispose();
				}
			}
		}

		#region Helper Functions

		private static Bitmap ConvertImage(Bitmap img, Size size, bool grayscale)
		{
			if (grayscale)
			{
				return new Bitmap(MakeGrayscale(img), size);
			}
			else
			{
				return new Bitmap(img, size);
			}
		}

		private static Bitmap MakeGrayscale(Bitmap original)
		{
			//create a blank bitmap the same size as original
			Bitmap newBitmap = new Bitmap(original.Width, original.Height);

			//get a graphics object from the new image
			using (var g = Graphics.FromImage(newBitmap))
			{
				//create the grayscale ColorMatrix
				ColorMatrix colorMatrix = new ColorMatrix(new float[][] {
					new float[] {.3f, .3f, .3f, 0, 0},
					new float[] {.59f, .59f, .59f, 0, 0},
					new float[] {.11f, .11f, .11f, 0, 0},
					new float[] {0, 0, 0, 1, 0},
					new float[] {0, 0, 0, 0, 1}
				});

				//create some image attributes
				ImageAttributes attributes = new ImageAttributes();

				//set the color matrix attribute
				attributes.SetColorMatrix(colorMatrix);

				//draw the original image on the new image
				//using the grayscale color matrix
				g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
				   0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

				return newBitmap;
			}
		}

		#endregion
	}
}
