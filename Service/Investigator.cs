using Paparazzi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace service
{
	public class Investigator
	{
		public void Investigate()
		{
			TakePictures();
			var programs = GatherPrograms();
			var history = CollectWebHistory();
		}

		private List<WebHistoryItem> CollectWebHistory()
		{
			return null;
		}

		private List<ProgramInfo> GatherPrograms()
		{
			return null;
		}

		public void TakePictures()
		{
			var camera = new Camera();
			var directory = "screenshots";
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
			camera.TakeScreenshot(Path.Combine(directory, DateTime.Now.ToString("MM-dd-yy_HH-mm-ss") + ".png"));
		}
	}
}
