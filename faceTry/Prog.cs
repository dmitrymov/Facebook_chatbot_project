using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Facebook;
using System.Collections;
//using Facebook.Properties;
using System.Dynamic;
using System.Threading;
using System.Net;
using System.IO;
using System.Windows.Forms;
using System.Timers;
using facebook;

namespace faceTry
{
	class Prog
	{
		public static WebBrowser webBrowser1 = new WebBrowser();
		public static WebBrowser webBrowser2 = new WebBrowser();
		public static bool done = false;
		public static int counter = 0;
		public static System.Timers.Timer aTimer, bTimer, cTimer, dTimer, eTimer, fTimer;
		public static bool sta = false;
		public static int cnt = 0;
		public static FacebookClient fb;
		public static Friends frnds;
		static System.Windows.Forms.Timer MyTimer, MyTimer1, MyTimer2;
		static bool exitFlagSendFrndReq, exitFlagGetFrndsOfFrnd, exitFlagCheckFriendsRequest;
		public static CreateAction createAction;
		private static string comments_path = @"D:\Visual Studio 2012\faceTry\comments_for_friends.txt";

		//[STAThread]
		public static void StartRun()
		{
			//send friend req: http://www.facebook.com/dialog/friends/?id=100003482276314&app_id=308388209194572&redirect_uri=https://www.facebook.com/connect/login_success.html
			//https://www.facebook.com/connect/login_success.html
			//http://www.facebook.com/dialog/oauth/?scope=email,user_birthday,read_friendlists,offline_access,publish_stream,read_stream&client_id=276244345758463&redirect_uri=https://www.facebook.com/connect/login_success.html&response_type=token
			//string uriString = "https://www.facebook.com/login.php?api_key=308388209194572&skip_api_login=1&display=page&cancel_url=https%3A%2F%2Fwww.facebook.com%2Fconnect%2Flogin_success.html&next=http%3A%2F%2Fwww.facebook.com%2Fdialog%2Ffriends%2F%3F_path%3Dfriends%252F%26app_id%3D308388209194572%26redirect_uri%3Dhttps%253A%252F%252Fwww.facebook.com%252Fconnect%252Flogin_success.html%26display%3Dpage%26id%3D100003482276314%26from_login%3D1%26client_id%3D308388209194572&rcount=1";
			//https://login.facebook.com/login.php?login_attempt=1                   
			fb = new FacebookClient("CAAB6TY8wt98BACRwz6AZBZA0EFNOVuCs0ZAZAEYsHzDVPB41YI6RZAsQDL2I85mZCDGyBV3wQ7JD7ZBiyFzvBXySRzvgJtpMJJd23eLSeZAFZCMfuD86DM4vPHJCpE31WJmGPRkHEUimHZCptvds96jZByWJF9LZAZB9IZCRcZD");
			//dynamic myfriend = fb.Get("1247528302/friends");
			createAction = new CreateAction();
			frnds = new Friends(fb);
			//frnds.sendFriendReq("1247528302");
			//frnds.checkFriendsRequest();
			//frnds.getFriendsOfFriend("1247528302");
			//fb.Post("http://www.facebook.com/dialog/friends/?id=100003852788229&app_id=308388209194572&redirect_uri=https://www.facebook.com/connect/login_success.html");
			//frnds.getFriendsOfFriend("1247528302");
			//sendFriendReqStart();
			//createAction.postOnFriendWall(fb, "gooog", frnds.getRandFriendId);
			createAction.publishStatus(fb);
			frnds.checkFriendsRequest();           
			
			Console.WriteLine("starting bot");
			Thread t1 = new System.Threading.Thread(publishStart);
			Thread t2 = new System.Threading.Thread(eventStart);
			Thread t3 = new System.Threading.Thread(likeStart);//postOnFriendWall           
			Thread t4 = new System.Threading.Thread(postOnFriendWallStart);
			Thread t5 = new System.Threading.Thread(checkFriendsRequestStart);
			Thread t6 = new System.Threading.Thread(getFriendsOfFriendStart);
			Thread t7 = new System.Threading.Thread(sendFriendReqStart);
			Thread t8 = new System.Threading.Thread(updateMyFriendListStart);
			Thread t9 = new System.Threading.Thread(postOnFriendCommentStart);
			t1.SetApartmentState(System.Threading.ApartmentState.STA);
			t2.SetApartmentState(System.Threading.ApartmentState.STA);
			t3.SetApartmentState(System.Threading.ApartmentState.STA);
			t4.SetApartmentState(System.Threading.ApartmentState.STA);
			t5.SetApartmentState(System.Threading.ApartmentState.STA);
			t6.SetApartmentState(System.Threading.ApartmentState.STA);
			t7.SetApartmentState(System.Threading.ApartmentState.STA);
			t8.SetApartmentState(System.Threading.ApartmentState.STA);
			t9.SetApartmentState(System.Threading.ApartmentState.STA);
			t1.Start();
			t2.Start();
			t3.Start();
			t4.Start();
			t5.Start();
			t6.Start();
			t7.Start();
			t8.Start();
			t9.Start();
			
			//var fb = new FacebookWebClient();
			//dynamic parameters = new ExpandoObject();
			//parameters.appId = "308388209194572";
			//parameters.message = "{join to my app is good!@#$}";
			//dynamic id = fb.Get("me/friendrequests");
			//dynamic idss = fb.Post("me/friendrequests",id);
			//dynamic id = fb.Post("alon.moreno.16/apprequests",parameters);
			//Console.WriteLine(id.data[0]["from"]["id"]);
			//WebClient client = new WebClient();
			//String data = client.DownloadString("https://www.facebook.com/shani.cohen.1293/friends");
			//Console.WriteLine(data);

			/*
			string uri = "https://www.facebook.com/shani.cohen.1293/friends";
			WebClient client = new WebClient();
			client.Credentials = new NetworkCredential("dandh@hotmail.co.il", "zaqwsx");
			//Encoding encode = System.Text.Encoding.GetEncoding("DOS-862");
			client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
			Stream data = client.OpenRead(uri);
			//Console.WriteLine(data);
            
			//StreamReader reader = new StreamReader(data,encode);
			StreamReader reader = new StreamReader(data);
			string s = null, t = null;
			int i = 0;
			while (i < 55)
			{
				s = reader.ReadLine();
				i++;
			}
			Console.WriteLine(s);*/

			//function example here///////////////////////////////////////// 

			//aTimer.Elapsed += new ElapsedEventHandler(aTimer_Elapsed);                                                                            
			//checkFriendsRequest();
			//publishStatus(fb);
			//createEvent(fb);done!!
			//publishFhoto(fb, "http://hschools.haifanet.org.il/hugim/DocLib16/Facebook-icon.png","the pic name");
			//like(fb, "1247528302");
			//postOnFriendComment(fb, "blah blah", "1247528302");
			//getAllFriends(fb);
			//postOnFriendWall(fb, "check check check", "1247528302");
			//like("1247528302_197169540401096");
			//getFriendsOfFriend();
			//getRandFriendId(fb);
			//sendFriendReq("100003482276314");
			//end function example//////////////////////////////////////////

			//webBrowser1.Navigate("http://www.facebook.com/dialog/friends/?id=100003482276314&app_id=308388209194572&redirect_uri=https://www.facebook.com/connect/login_success.html");
			//webBrowser1.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(WebBrowser1_DocumentCompleted);

			//Console.WriteLine();

			//---------------------------------------------------------------------------------------------

		}//main end

		public static void publishStart()
		{
			Console.WriteLine("starting t1");
			System.Random RandNum = new System.Random();
			int timeinter = 0, max = 10800000;//3 hr
			while (true)
			{
				aTimer = new System.Timers.Timer();
				aTimer.Elapsed += (sender, e) => createAction.publishStatus(fb);
				timeinter = RandNum.Next(10000, max);
				aTimer.Interval = timeinter;
				Console.WriteLine("time interval publishStart:{0}", timeinter);
				aTimer.AutoReset = false;
				aTimer.Enabled = true;
				GC.KeepAlive(aTimer);
				Thread.Sleep(max - timeinter);
				aTimer.Dispose();
			}
		}

		public static void eventStart()
		{
			Console.WriteLine("starting t2");
			System.Random RandNum = new System.Random();
			int timeinter = 0, max = 604800000;
			while (true)
			{
				bTimer = new System.Timers.Timer();
				bTimer.Elapsed += (sender, e) => createAction.createEvent(fb);
				timeinter = RandNum.Next(10000, max);
				bTimer.Interval = timeinter;
				Console.WriteLine("time interval eventStart:{0}", timeinter);
				bTimer.Enabled = true;
				GC.KeepAlive(bTimer);
				Thread.Sleep(max - timeinter);
				bTimer.Dispose();
			}
		}

		public static void likeStart()
		{
			Console.WriteLine("starting t3");
			System.Random RandNum = new System.Random();
			int timeinter = 0, max = 18000000;//5 hr
			while (true)
			{
				cTimer = new System.Timers.Timer();
				cTimer.Elapsed += (sender, e) => createAction.like(fb, frnds.getRandFriendId());
				timeinter = RandNum.Next(10000, max);
				cTimer.Interval = timeinter;
				Console.WriteLine("time interval likeStart:{0}", timeinter);
				cTimer.AutoReset = false;
				cTimer.Enabled = true;
				GC.KeepAlive(cTimer);
				Thread.Sleep(max - timeinter);
				cTimer.Dispose();
			}
		}

		public static void postOnFriendWallStart()
		{
			Console.WriteLine("starting t4");
			System.Random RandNumber = new System.Random();
			int randomStatus = RandNumber.Next(13);
			System.Random RandNum = new System.Random();
			int timeinter = 0, max = 14400000;//4 hr
			while (true)
			{
				dTimer = new System.Timers.Timer();
				dTimer.Elapsed += (sender, e) => createAction.postOnFriendWall(fb, createAction.getLine(
									comments_path, randomStatus), frnds.getRandFriendId());
				timeinter = RandNum.Next(10000, max);
				dTimer.Interval = timeinter;
				Console.WriteLine("time interval postOnFriendWallStart:{0}", timeinter);
				dTimer.AutoReset = false;
				dTimer.Enabled = true;
				GC.KeepAlive(dTimer);
				Thread.Sleep(max - timeinter);
				dTimer.Dispose();
			}
		}

		public static void checkFriendsRequestStart()
		{
			Console.WriteLine("starting t5");
			System.Random RandNum = new System.Random();
			int timeinter = 0, max = 18000000;//5 hours
			while (true)
			{
				timeinter = RandNum.Next(10000, max);
				Console.WriteLine("time interval checkFriendsRequestStart:{0}", timeinter);
				MyTimer = new System.Windows.Forms.Timer();
				MyTimer.Tick += new EventHandler(MyTimer_Tick);
				MyTimer.Interval = timeinter;
				MyTimer.Start();
				exitFlagCheckFriendsRequest = false;
				while (exitFlagCheckFriendsRequest == false)
				{
					Application.DoEvents();
				}
				GC.KeepAlive(MyTimer);
				Thread.Sleep(max - timeinter);
			}
		}
		public static void MyTimer_Tick(object sender, EventArgs e)
		{
			//MyTimer.Stop();           
			exitFlagCheckFriendsRequest = frnds.checkFriendsRequest();
		}

		public static void getFriendsOfFriendStart()
		{
			Console.WriteLine("starting t6");
			System.Random RandNum = new System.Random();
			int timeinter = 0, max = 14400000;//4 hours
			while (true)
			{
				timeinter = RandNum.Next(10000, max);
				Console.WriteLine("time interval getFriendsOfFriendStart:{0}", timeinter);
				MyTimer1 = new System.Windows.Forms.Timer();
				MyTimer1.Tick += new EventHandler(MyTimer_Tick1);
				MyTimer1.Interval = timeinter;
				MyTimer1.Start();
				exitFlagGetFrndsOfFrnd = false;
				while (exitFlagGetFrndsOfFrnd == false)
				{
					Application.DoEvents();
				}
				GC.KeepAlive(MyTimer1);
				Thread.Sleep(max - timeinter);
			}
		}

		public static void MyTimer_Tick1(object sender, EventArgs e)
		{
			//MyTimer.Stop();            
			string id = frnds.getRandFriendId();
			exitFlagGetFrndsOfFrnd = frnds.getFriendsOfFriend(id);
		}

		public static void sendFriendReqStart()
		{
			Console.WriteLine("starting t7");
			System.Random RandNum = new System.Random();
			int timeinter = 0, max = 21600000;//6 hr
			while (true)
			{
				timeinter = RandNum.Next(10000, max);
				Console.WriteLine("time interval sendFriendReqStart:{0}", timeinter);
				//string id = (string)frnds.getncf()[0];                 
				MyTimer2 = new System.Windows.Forms.Timer();
				MyTimer2.Tick += new EventHandler(MyTimer_Tick2);
				MyTimer2.Interval = timeinter;
				MyTimer2.Start();
				exitFlagSendFrndReq = false;
				while (exitFlagSendFrndReq == false)
				{
					Application.DoEvents();
				}
				GC.KeepAlive(MyTimer2);
				Thread.Sleep(max - timeinter);
			}
		}

		public static void MyTimer_Tick2(object sender, EventArgs e)
		{
			ArrayList ncf = new ArrayList();
			ncf = frnds.getncf();
			if (ncf != null)
			{
				string id = ncf[0].ToString();
				ncf.RemoveAt(0);
				///MyTimer.Stop();                
				exitFlagSendFrndReq = frnds.sendFriendReq(id);
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

		/*
		 public static void like(string object_id)    
		 {
			 using (StreamWriter w = File.AppendText(@"C:\Users\natanelal\Documents\Visual Studio 2010\Projects\facebook\facebook\logfile.txt"))
			 {
				 Log("start like", w);
				 WebRequest request = WebRequest.Create(string.Format("https://graph.facebook.com/{0}/likes?access_token=AAAEYejmpdkwBADHEGAqPI9ds7iAv44jHJ0PUzlTZA5tEjJQVNq4ERxo2umLmCZBlhMXo9g1enhG1SxZAcZBCgtrUP6W0IVaeGjgJYHjPwQZDZD", object_id));
				 request.Credentials = CredentialCache.DefaultCredentials;
				 request.Method = "POST";
				 Stream dataStream = request.GetRequestStream();
				 WebResponse response = request.GetResponse();
				 Console.WriteLine(((HttpWebResponse)response).StatusDescription);
				 dataStream = response.GetResponseStream();
				 StreamReader reader = new StreamReader(dataStream);
				 string responseFromServer = reader.ReadToEnd();
				 Console.WriteLine(responseFromServer);
				 Log("end like", w);
				 w.Close();
				 // Clean up the streams.
				 reader.Close();
			 }
		 }
		 */

		public static void updateMyFriendListStart()
		{
			Console.WriteLine("starting t8");
			System.Random RandNum = new System.Random();
			int timeinter = 0, max = 18000000;//5 hr
			while (true)
			{
				eTimer = new System.Timers.Timer();
				eTimer.Elapsed += (sender, e) => frnds.updateMyFriendList(fb);
				timeinter = RandNum.Next(10000, max);
				eTimer.Interval = timeinter;
				Console.WriteLine("time interval t8:{0}", timeinter);
				eTimer.AutoReset = false;
				eTimer.Enabled = true;
				GC.KeepAlive(eTimer);
				Thread.Sleep(max - timeinter);
				eTimer.Dispose();
			}
		}

		public static void postOnFriendCommentStart()
		{
			Console.WriteLine("starting t9");
			System.Random RandNum = new System.Random();
			int timeinter = 0, max = 18000000;//5 hr
			System.Random RandNumber = new System.Random();
			int randomComment = RandNumber.Next(0, 12);
			while (true)
			{
				fTimer = new System.Timers.Timer();
				fTimer.Elapsed += (sender, e) => frnds.postOnFriendComment(fb, createAction.getLine(
							comments_path, randomComment), frnds.getRandFriendId());
				timeinter = RandNum.Next(10000, max);
				fTimer.Interval = timeinter;
				Console.WriteLine("time interval t9:{0}", timeinter);
				fTimer.AutoReset = false;
				fTimer.Enabled = true;
				GC.KeepAlive(fTimer);
				Thread.Sleep(max - timeinter);
				fTimer.Dispose();
			}
		}


	}
}
