using Facebook;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace faceTry
{
	class CreateAction
	{

		private static string log_path = @"D:\Visual Studio 2012\faceTry\log_file.txt";

		private static string status_path = @"D:\Visual Studio 2012\faceTry\status_file.txt";

		public CreateAction()
		{
		}
		public static void Log(string logMessage, TextWriter w)
		{
			w.Write("\r\nLog Entry : ");
			w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
				DateTime.Now.ToLongDateString());
			w.WriteLine("  :{0}", logMessage);
			w.WriteLine("-------------------------------");
			// Update the underlying file.
			w.Flush();
		}
		//select randomly between statues from file or from ynet.news
		public string publishStatus(FacebookClient fb)
		{
			int statusLinesNumber = 16;
			System.Random RandNum = new System.Random();
			int rand = RandNum.Next(0, 2);
			dynamic parameters = new ExpandoObject();
			if (rand == 0)
			{
				using (StreamWriter w = File.AppendText(log_path))
				{
					Log("start publish status", w);
					System.Random RandNumber = new System.Random();
					int randomStatus = RandNumber.Next(statusLinesNumber);
					parameters.message = getLine(status_path, randomStatus);
					dynamic result = fb.Post("me/feed", parameters);
					var id = result.id;
					Log("end publish status", w);
					w.Close();
					return id;
				}
			}
			else
			{
				using (StreamWriter w = File.AppendText(log_path))
				{
					Log("start publish status from ynet", w);
					string uri = "http://www.ynetnews.com";
					WebClient client = new WebClient();
					//webClient.Credentials = new NetworkCredential(username, password);
					//Encoding encode = System.Text.Encoding.GetEncoding("DOS-862");
					client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
					Stream data = client.OpenRead(uri);
					//StreamReader reader = new StreamReader(data,encode);
					StreamReader reader = new StreamReader(data);
					int i = 0, j = 0, k = 0;
					string s = null, t = null;
					while (i < 158)
					{
						s = reader.ReadLine();
						i++;
					}
					//get the head news
					j = s.IndexOf("19px;'>");
					k = s.IndexOf("</span>");
					int sum = k - j;
					t = s.Substring(j + 7, sum - 7);
					data.Close();
					reader.Close();
					parameters.message = t;
					dynamic result = fb.Post("me/feed", parameters);
					var id = result.id;
					Log("end publish status from ynet", w);
					w.Close();
					return id;
				}
			}
		}

		public string getLine(string fileName, int line)
		{
			using (var sr = new StreamReader(fileName))
			{
				for (int i = 1; i < line; i++)
					sr.ReadLine();
				return sr.ReadLine();
			}
		}
		//create an event (The International Standard for the representation of dates and times is ISO 8601)"2012-05-06T20:20:30", "2012-06-07T22:15:30"
		public void createEvent(FacebookClient fb)
		{
			using (StreamWriter w = File.AppendText(log_path))
			{
				Log("start createEvent", w);
				string startTime = DateTime.UtcNow.ToString("s");
				char rightDigit = startTime[9];
				int b = Convert.ToInt32(new string(rightDigit, 1));
				string endTime = startTime.Remove(8, 2);
				System.Random RandNum = new System.Random();
				int num = RandNum.Next(1, 3);
				endTime = endTime.Insert(8, num.ToString());
				num = RandNum.Next(b + 1, 10);
				endTime = endTime.Insert(9, num.ToString());
				dynamic parameters = new ExpandoObject();
				parameters.name = "party-details later";
				parameters.start_time = startTime;
				parameters.end_time = endTime;
				parameters.location = "somewhere on earth";
				parameters.privacy = "OPEN";
				fb.Post("me/events", parameters);
				Log("end createEvent", w);
				w.Close();
			}
		}

		public void postOnFriendWall(FacebookClient fb, string msg, string idfrnd)
		{
			using (StreamWriter w = File.AppendText(log_path))
			{
				Log("start postOnFriendWall", w);
				dynamic parameters = new ExpandoObject();
				parameters.message = msg;
				fb.Post(string.Format("{0}/feed?fields=message", idfrnd), parameters);
				Log("end postOnFriendWall", w);
				w.Close();
			}
		}

		public void like(FacebookClient fb, string idfrnd)//need to use this like function
		{
			string postid = null;
			dynamic parameters = new ExpandoObject();
			JsonObject myfriend = (JsonObject)fb.Get(string.Format("{0}/feed?fields=likes", idfrnd));//get only likes            
			using (StreamWriter w = File.AppendText(log_path))
			{
				Log("start like", w);
				foreach (JsonObject f in (JsonArray)myfriend[0])
				{
					if (f.ContainsKey("likes"))
					{
						postid = (string)f[1];//get the recent like id
						break;
					}
				}
				fb.Post(string.Format("{0}/likes", postid), parameters);
				Log("end like", w);
				w.Close();
			}
		}

		public static void publishFhoto(FacebookClient fb, string link, string picname)
		{
			using (StreamWriter w = File.AppendText(log_path))
			{
				Log("start publishFhoto", w);
				dynamic parameters = new ExpandoObject();
				parameters.picture = link;
				parameters.name = picname;
				dynamic result = fb.Post("me/feed", parameters);
				Log("end publishFhoto", w);
				w.Close();
			}
		}

	}
}
