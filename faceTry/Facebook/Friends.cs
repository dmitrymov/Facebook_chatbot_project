using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Facebook;
using System.Dynamic;
//using Facebook.Properties;
using System.Threading;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Timers;

namespace facebook
{
	class Friends
	{
		public static WebBrowser webBrowser1 = new WebBrowser();
		public static WebBrowser webBrowser2 = new WebBrowser();
		public static WebBrowser webBrowser3 = new WebBrowser();
		public static int cnt = 0, counter = 0, cnt0 = 0;
		public static ArrayList myfriendlist = new ArrayList();
		public static ArrayList notConfirmFrndsIds = new ArrayList();
		public static bool flag = false;
		public static Form form, form1, form2;
		public Friends(FacebookClient fb)
		{
			dynamic myfriend = fb.Get("me/friends");
			foreach (dynamic friend in myfriend.data)
			{
				myfriendlist.Add(friend.id);
				//Console.WriteLine("Name: " + friend.name + " Facebook id: " + friend.id);
			}
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
		public ArrayList getncf()
		{
			if (notConfirmFrndsIds.Count > 0)
				return notConfirmFrndsIds;
			return null;
		}
		public void PrintValues(IEnumerable myList)
		{
			foreach (Object obj in myList)
				Console.Write("   {0}", obj);
			Console.WriteLine();
		}
		public static ArrayList getAllMyFriendList()
		{
			return myfriendlist;
		}

		public void updateMyFriendList(FacebookClient fb)
		{
			using (StreamWriter w = File.AppendText(@"C:\Users\nati\Documents\Visual Studio 2010\Projects\facebook\facebook\logfile.txt"))
			{
				Log("start update my friend list", w);
				dynamic myfriend = fb.Get("me/friends");
				foreach (dynamic friend in myfriend.data)
				{
					if (!myfriendlist.Contains(friend.id))
						myfriendlist.Add(friend.id);
					//Console.WriteLine("Name: " + friend.name + " Facebook id: " + friend.id);
				}
				Log("end post On Friend Comment", w);
				w.Close();
			}
		}
		public string getRandFriendId()
		{
			Random rnd = new Random();
			int index = rnd.Next(0, myfriendlist.Count);
			/*
			JsonObject myfriend = (JsonObject)fb.Get("me/friends");
			int totalFriend = 0;
			foreach (JsonObject f in (JsonArray)myfriend[0])
				totalFriend++;
			System.Random RandNum = new System.Random();
			int num = RandNum.Next(totalFriend + 1);
			int tmp = 0;
			string id = null;
			foreach (JsonObject f in (JsonArray)myfriend[0])
			{
				id = (string)f[1];
				if (tmp == num)
					break;
				else
					tmp++;
			}*/
			return myfriendlist[index].ToString();
		}

		public bool checkFriendsRequest()
		{
			using (StreamWriter w = File.AppendText(@"C:\Users\nati\Documents\Visual Studio 2010\Projects\facebook\facebook\logfile.txt"))
			{
				Log("start check Friends Request", w);
				form = new Form();
				webBrowser2.Visible = true;
				webBrowser2.Dock = DockStyle.Fill;
				webBrowser2.Navigate("https://www.facebook.com/reqs.php");
				webBrowser2.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowser2_DocumentCompleted);
				form.WindowState = FormWindowState.Normal;
				form.Controls.Add(webBrowser2);
				form.ShowDialog();
				Log("end check Friends Request", w);
				w.Close();
			}
			return true;
		}
		public static void WebBrowser2_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			Console.WriteLine("document completed");
			HtmlElement ele = webBrowser2.Document.GetElementById("email");
			if (ele != null)
			{
				ele.InnerText = "dandh@hotmail.co.il";
				cnt++;
			}
			ele = webBrowser2.Document.GetElementById("pass");
			if (ele != null)
			{
				ele.InnerText = "zaqwsx";
				cnt++;
			}
			ele = webBrowser2.Document.GetElementById("loginbutton");
			if (ele != null)
			{
				ele.InvokeMember("click");
				cnt++;
			}
			Console.WriteLine(cnt);
			cnt++;
			if (cnt >= 5)
			{
				Console.WriteLine("in cnt");
				HtmlElementCollection elems = webBrowser2.Document.GetElementsByTagName("input");
				foreach (HtmlElement elem in elems)
				{
					if (elem.GetAttribute("value").Equals("Confirm"))
					{
						elem.InvokeMember("click");
						cnt = 0;
						form.Close();
					}
				}
				cnt = 0;
				form.Close();
				Console.WriteLine("after close form");
			}
		}
		public void postOnFriendComment(FacebookClient fb, string msg, string idfrnd)
		{
			string postid = null;
			dynamic parameters = new ExpandoObject();
			parameters.message = msg;
			JsonObject myfriend = (JsonObject)fb.Get(string.Format("{0}/feed?fields=comments", idfrnd));//get only comments            
			using (StreamWriter w = File.AppendText(@"C:\Users\nati\Documents\Visual Studio 2010\Projects\facebook\facebook\logfile.txt"))
			{
				Log("start post On Friend Comment", w);
				foreach (JsonObject f in (JsonArray)myfriend[0])
				{
					if (f.ContainsKey("id"))
					{
						postid = (string)f.ElementAt(1).Value;//get the recent comment id
						break;
					}
				}
				fb.Post(string.Format("{0}/comments", postid), parameters);
				Log("end post On Friend Comment", w);
				w.Close();
			}
		}
		public bool getFriendsOfFriend(string id)
		{
			using (StreamWriter w = File.AppendText(@"C:\Users\nati\Documents\Visual Studio 2010\Projects\facebook\facebook\logfile.txt"))
			{
				Log("start get friends of friend", w);
				webBrowser3.Visible = true;
				webBrowser3.AllowNavigation = true;
				webBrowser3.Height = 500;
				webBrowser3.Width = 500;
				webBrowser3.Dock = DockStyle.Fill;
				webBrowser3.Navigate(string.Format("https://www.facebook.com/profile.php?id={0}&sk=friends", id));
				webBrowser3.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowser3_DocumentCompleted);
				form1 = new Form();
				form1.WindowState = FormWindowState.Normal;
				form1.Controls.Add(webBrowser3);
				form1.ShowDialog();
				Log("end get friends of friend", w);
				w.Close();
			}
			return true;
		}

		public static void WebBrowser3_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			string sid;
			string id;
			HtmlElement ele = webBrowser3.Document.GetElementById("email");
			if (ele != null)
			{
				ele.InnerText = "dandh@hotmail.co.il";
				cnt0++;
			}
			ele = webBrowser3.Document.GetElementById("pass");
			if (ele != null)
			{
				ele.InnerText = "zaqwsx";
				cnt0++;
			}
			ele = webBrowser3.Document.GetElementById("loginbutton");
			if (ele != null)
			{
				ele.InvokeMember("click");
				cnt0++;
			}
			cnt0++;
			if (cnt0 >= 3)
			{
				HtmlElementCollection elems = webBrowser3.Document.GetElementsByTagName("a");
				foreach (HtmlElement elem in elems)
				{
					//Console.WriteLine("data hovercard:{0}", elem.GetAttribute("data-hovercard"));
					//get friend link id
					sid = elem.GetAttribute("data-hovercard");
					int i = 0;
					bool found = false;
					if (sid.Length > 0)
					{
						id = sid.Substring(29);
						if (!id[i].Equals('0'))
						{
							foreach (string frnd_id in notConfirmFrndsIds)
							{
								if (id == frnd_id)
								{
									found = true;
									break;
								}
							}
							if (found == false)
							{
								notConfirmFrndsIds.Add(id);
								using (StreamWriter sw = File.AppendText(@"C:\Users\nati\Documents\Visual Studio 2010\Projects\facebook\facebook\notConfirmedId.txt"))
								{
									sw.WriteLine(id);
								}
							}
						}
					}
				}
				form1.Close();
			}
		}

		public bool sendFriendReq(string idfrnd)
		{
			using (StreamWriter w = File.AppendText(@"C:\Users\nati\Documents\Visual Studio 2010\Projects\facebook\facebook\logfile.txt"))
			{
				Log("start send req friend", w);
				webBrowser1.Visible = true;
				webBrowser1.Dock = DockStyle.Fill;
				webBrowser1.Navigate(string.Format("http://www.facebook.com/dialog/friends/?id={0}&app_id=308388209194572&redirect_uri=https://www.facebook.com/connect/login_success.html", idfrnd));
				webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowser1_DocumentCompleted);
				form2 = new Form();
				form2.Visible = false;
				form2.WindowState = FormWindowState.Normal;
				form2.Controls.Add(webBrowser1);
				form2.ShowDialog();
				Log("end send req friend", w);
				w.Close();
			}
			return true;
		}

		private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			HtmlElement ele = webBrowser1.Document.GetElementById("email");
			if (ele != null)
			{
				ele.InnerText = "dandh@hotmail.co.il";
				counter++;
			}
			ele = webBrowser1.Document.GetElementById("pass");
			if (ele != null)
			{
				ele.InnerText = "zaqwsx";
				counter++;
			}
			ele = webBrowser1.Document.GetElementById("login");
			if (ele != null)
			{
				ele.InvokeMember("click");
				counter++;
			}
			ele = webBrowser1.Document.GetElementById("request");
			if (ele != null)
			{
				ele.InvokeMember("click");
				counter++;
			}
			if (counter == 4 || counter == 0)
			{
				counter = 0;
				form2.Close();
			}
		}
	}
}
