using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace faceTry
{
	class FacebookDoChat
	{

		private FacebookUser user;
		private AIMLProcessor bot;

		public FacebookDoChat(FacebookUser usr)
		{
			user = usr;
			bot = new AIMLProcessor();
			String name = usr.first_name;
			String gender = usr.gender;
			bot.Load();
			bot.SetName(name);
			bot.SetGender(gender);
			AIMLProcessor.Thats.Add("*");
		}

		public FacebookUser User
		{
			get
			{
				return user;
			}
			set
			{
				user = value;
			}
		}

		//public AIMLProcessor Bot { get; set; }

		public string GetAnswer(agsXMPP.protocol.client.Message msg)
		{
			AIMLProcessor.Inputs.Insert(0, msg.Body);
			string tempCh = bot.FindTemplate(msg.Body, AIMLProcessor.Thats[0], "*");
			if(string.IsNullOrEmpty(tempCh))
			{
				Random rand = new Random();
				int num = rand.Next(0, 2);
				switch (num)
				{
					case 0:
						tempCh = "Are you talking about an animal, vegetable or mineral??";
						break;
					case 1:
						tempCh = "Be more specific";
						break;
					case 2:
						tempCh = "What is it?";
						break;
					default:
						tempCh = "Can you say it in other way?";
						break;
				}
			}
			AIMLProcessor.Thats.Insert(0, tempCh);
			//Console.WriteLine(AIMLProcessor.Thats[0]);
			//chat = Console.ReadLine();
			//client.SendMessage(AIMLProcessor.Thats[0], user.name);
			return AIMLProcessor.Thats[0];
		}

	}
}
