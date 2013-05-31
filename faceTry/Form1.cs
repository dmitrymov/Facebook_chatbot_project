using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace faceTry
{
	public partial class Form1 : Form
	{
		private FacebookChatClient client;
		//private AIMLProcessor bot = new AIMLProcessor();
		private LinkedList<FacebookDoChat> chatList;

		public Form1()
		{
			InitializeComponent();
			client = new FacebookChatClient();
			chatList = new LinkedList<FacebookDoChat>();
			client.OnLoginResult = success =>
			{
				if(success)
					MessageBox.Show("Connected!!!");
			};
			client.OnLogout = () =>
			{
				// close connections
			};
			client.OnMessageReceived = (msg, user) =>
			{
				startChat(msg, user);
			};
			client.OnUserIsTyping = user =>
			{
				// user is typing. so what?
			};
			client.OnUserAdded = user =>
			{
				// may be added to list when online
				// better to add user to list when getting msg
			};
			client.OnUserRemoved = user =>
			{
				RemoveFromList(user);
			};
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			string nick = "alice.bot.90";
			string pass = "210187";
			client.Login(nick, pass);

		}

		public void startChat(agsXMPP.protocol.client.Message msg, FacebookUser user)
		{
			if (!UserExist(user))
			{
				AddUser(user);
			}
			WriteToUser(msg, user);
		}

		private bool UserExist(FacebookUser user)
		{
			foreach (var t in chatList)
			{
				if (t.User.name == user.name)
				{
					return true;
				}
			}
			return false;
		}

		private void AddUser(FacebookUser user)
		{
			FacebookDoChat ch = new FacebookDoChat(user);
			chatList.AddLast(ch);
		}

		private void RemoveFromList(FacebookUser user)
		{
			if (chatList.Count <= 0)
				return;
			FacebookDoChat temp = null;
			foreach (var t in chatList)
			{
				if(t.User.name == user.name)
				{
					temp = t;
					break;
				}
			}
			if (temp == null)
				return;
			chatList.Remove(temp);
		}

		private void WriteToUser(agsXMPP.protocol.client.Message msg, FacebookUser user)
		{
			FacebookDoChat ch = FindChat(user);
			if (ch == null)
				return;		// it cant be that user is not found
			string answer = ch.GetAnswer(msg);
			client.SendMessage(answer, user.name);
		}

		private FacebookDoChat FindChat(FacebookUser user)
		{
			FacebookDoChat ans = null;
			foreach (var t in chatList)
			{
				//if (t.User.Equals(user))
				if(t.User.name == user.name)
				{
					ans = t;
				}
			}
			return ans;
		}

	} // class end

}
