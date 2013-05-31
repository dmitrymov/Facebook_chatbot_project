using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace faceTry
{
	public class AIMLProcessor
	{
		public static readonly List<string> Thats = new List<string>();
		public static readonly List<string> Inputs = new List<string>();
		private readonly Dictionary<string, string> BotData = new Dictionary<string, string>();
		private readonly Dictionary<string, string> Predicates = new Dictionary<string, string>();
		private readonly AIMLData data = new AIMLData(null, string.Empty);
		private readonly Random rand = new Random(DateTime.Now.Millisecond);

		public void Load()
		{
			foreach (string file in Directory.GetFiles(@"D:\Visual Studio 2012\AIMLbotConsole\alice", "*.aiml"))
			{
				var doc = XDocument.Load(file);
				var first = doc.Element("aiml");
				var topics = first.Elements("topic");
				var cats = first.Elements("category");
				if (topics != null && topics.Count() > 0)
				{
					foreach (var topic in topics)
					{
						var topicName = topic.Attribute("name") == null ? "*" : topic.Attribute("name").Value;
						foreach (var cat in topic.Elements())
							ProcessCategory(cat, topicName);
					}
				}
				if (cats == null || cats.Count() <= 0)
					continue;
				foreach (var cat in cats)
					ProcessCategory(cat, "*");
			}
			Predicates.Add("job", "unknown");
			Predicates.Add("name", "unknown");
			Predicates.Add("gender", "unknown");
			Predicates.Add("birthday", "unknown");
			Predicates.Add("age", "unknown");
			Predicates.Add("birthplace", "unknown");
			Predicates.Add("location", "unknown");
			Predicates.Add("lenguage", "English");
			Predicates.Add("religion", "unknown");
			LoadInitialPredicates();
		}

		public void SetName(string name)
		{
			if(!string.IsNullOrEmpty(name))
				Predicates["name"] = name;
		}

		public void SetGender(string gender)
		{
			if(!string.IsNullOrEmpty(gender))
				Predicates["gender"] = gender;
		}

		private void ProcessCategory(XElement cat, string topic)
		{
			var patt = cat.Element("pattern");
			var temp = cat.Element("template");
			var that = cat.Element("that");
			if (patt == null || temp == null)
				return;
			var t = that != null ? that.InnerText() : "*";
			string pattern = string.Format("{0} <THAT> {1} <TOPIC> {2}",
				patt.Nodes().First(), t.Trim(), topic.Trim());
			LoadWord(data, new List<string>(pattern.Split(' ')), temp.InnerText());
		}

		public string FindTemplate(string text, string that, string topic)
		{
			try
			{
				var pattern = string.Format("{0} <THAT> {1} <TOPIC> {2}", Normalise(text).Trim(), "*", topic.Trim());
				var temp = FindTemplateRecursive(data, new List<string>(pattern.Split(' ')), new PatternData(), 0);
				var result = InterpretTemplate(temp.Template, temp.WildTexts, text, that, topic);
				return result.Trim().Replace("  ", " ");
			}
			catch(Exception){
				return "";
			}
		}

		public string Normalise(string text)
		{
			try
			{
				return Regex.Replace(text, @"[^\w\ ]", "").Trim().Replace("  ", " ");
			}
			catch (Exception)
			{
				return "";
			}
		}

		private string InterpretTemplate(string template, List<WildData> wilds,
			string text, string that, string topic)
		{
			var xmltemplate = string.Format("<process>{0}</process>", template);
			var doc = XElement.Parse(xmltemplate);
			if (doc.Elements().Count() == 0)
				return doc.Value;
			string response = "";

			foreach (var node in doc.Nodes())
			{
				if (node.NodeType != XmlNodeType.Element)
				{
					response += " " + node;
					continue;
				}
				var element = node as XElement;
				switch (element.Name.LocalName.ToUpper())
				{
					case "INPUT":
						response += " " + ProcessInput(element).Trim();
						break;
					case "THAT":
						response += " " + ProcessThat(element).Trim();
						break;
					case "THATSTAR":
						response += " " + ProcessThatStar(element, wilds).Trim();
						break;
					case "TOPICSTAR":
						response += " " + ProcessTopicStar(element, wilds).Trim();
						break;
					case "STAR":
						response += " " + ProcessStar(element, wilds).Trim();
						break;
					case "PERSON":
						response += " " + ProcessPerson(wilds).Trim();
						break;
					case "SET":
						response += " " + ProcessSet(element, wilds, text, that, topic).Trim();
						break;
					case "GET":
						response += " " + ProcessGet(element).Trim();
						break;
					case "THINK":
						response += " " + ProcessThink(element, wilds, text, that, topic).Trim();
						break;
					case "RANDOM":
						response += " " + ProcessRandom(element, wilds, text, that, topic).Trim();
						break;
					case "BOT":
						response += " " + ProcessBot(element).Trim();
						break;
					case "SR":
						response += " " + ProcessSR(element, wilds, text, that, topic).Trim();
						break;
					case "SRAI":
						response += " " + ProcessSRAI(element, wilds, text, that, topic).Trim();
						break;
					default:
						return element.ToString().Trim();
				}
			}
			return response.Trim();
		}

		private static string ProcessInput(XElement element)
		{
			var num = 0;
			var index = element.Attribute("index");
			if (index != null)
				num = int.Parse(index.Value.Split(',')[0]) - 1;
			if (num >= Inputs.Count)
				return string.Empty;
			return Inputs[num];
		}

		private static string ProcessThat(XElement element)
		{
			var num = 0;
			var index = element.Attribute("index");
			if (index != null)
				num = int.Parse(index.Value.Split(',')[0]) - 1;
			if (num >= Thats.Count)
				return string.Empty;
			return Thats[num];
		}

		private static string ProcessThatStar(XElement element, IEnumerable<WildData> wilds)
		{
			var w = wilds.Where(a => a.WildType == StarType.That).ToList();
			if (w.Count() == 0)
				return string.Empty;
			var num = 0;
			var index = element.Attribute("index");
			if (index != null)
				num = int.Parse(index.Value.Split(',')[0]) - 1;
			if (num >= w.Count)
				return string.Empty;
			return w[num].WildText;
		}

		private static string ProcessTopicStar(XElement element, IEnumerable<WildData> wilds)
		{
			var w = wilds.Where(a => a.WildType == StarType.Topic).ToList();
			if (w.Count() == 0)
				return string.Empty;
			var num = 0;
			var index = element.Attribute("index");
			if (index != null)
				num = int.Parse(index.Value.Split(',')[0]) - 1;
			if (num >= w.Count)
				return string.Empty;
			return w[num].WildText;
		}

		private static string ProcessStar(XElement element, IEnumerable<WildData> wilds)
		{
			var w = wilds.Where(a => a.WildType == StarType.Pattern).ToList();
			if (w.Count() == 0)
				return string.Empty;
			var num = 0;
			var index = element.Attribute("index");
			if (index != null)
				num = int.Parse(index.Value) - 1;
			if (num >= w.Count)
				return string.Empty;
			return w[num].WildText;
		}

		private static string ProcessPerson(IList<WildData> wilds)
		{
			if (wilds.Count == 0)
				return string.Empty;
			var words = wilds[0].WildText.Split(' ');
			for (int i = 0; i < words.Count(); i++)
			{
				if (words[i].Trim().ToUpper() == "I")
					words[i] = "you";
				if (words[i].Trim().ToUpper() == "MY")
					words[i] = "your";
			}
			var response = string.Join(" ", words);
			response = response.Replace("you am", "you are");
			return response;
		}

		private string ProcessSet(XElement element, List<WildData> wilds, string text,
			string that, string topic)
		{
			if (element.Attribute("name") == null)
				return string.Empty;
			var att = element.Attribute("name").Value;
			if (!Predicates.ContainsKey(att))
				Predicates.Add(att, "");
			Predicates[att] = InterpretTemplate(element.InnerText(), wilds, text, that, topic);
			return Predicates[att];
		}

		private string ProcessGet(XElement element)
		{
			if (element.Attribute("name") == null)
				return string.Empty;
			var att = element.Attribute("name").Value;
			if (!Predicates.ContainsKey(att))
				return string.Empty;
			return Predicates[att];
		}


		private string ProcessThink(XElement element, List<WildData> wilds, string text,
			string that, string topic)
		{
			InterpretTemplate(element.InnerText(), wilds, text, that, topic);
			return string.Empty;
		}

		private string ProcessRandom(XElement element, List<WildData> wilds, string text,
			string that, string topic)
		{
			var num = rand.Next(0, element.Elements().Count());
			var ret = element.Elements().ToList()[num].InnerText();
			return InterpretTemplate(ret, wilds, text, that, topic);
		}

		private string ProcessSRAI(XElement element, List<WildData> wilds, string text,
			string that, string topic)
		{
			var t = InterpretTemplate(element.InnerText(), wilds, text, that, topic);
			return FindTemplate(t, that, topic);
		}

		private string ProcessSR(XElement element, List<WildData> wilds, string text,
			string that, string topic)
		{
			var w = wilds.Where(a => a.WildType == StarType.Pattern).ToList();
			if (w.Count() == 0)
				return string.Empty;
			var num = 0;
			var index = element.Attribute("index");
			if (index != null)
				num = int.Parse(index.Value) - 1;
			if (num >= w.Count)
				return string.Empty;
			var t = InterpretTemplate(w[num].WildText, wilds, text, that, topic);
			return FindTemplate(t, that, topic);
		}

		private string ProcessBot(XElement element)
		{
			return BotData[element.Attribute("name").Value.ToUpper()];
		}


		private static PatternData FindTemplateRecursive(AIMLData ai, List<string> text,
			PatternData data, int searchPos)
		{
			var key = text[searchPos];
			if (data.IsInWildcard && searchPos < text.Count - 1 && !ai.Data.ContainsKey("_") &&
				!ai.Data.ContainsKey(key.ToUpper()) && !ai.Data.ContainsKey("*"))
			{
				data.WildTexts[data.WildTexts.Count - 1].WildText += key + " ";
				return FindTemplateRecursive(ai, text, data, searchPos + 1);
			}
			if (ai.Data.ContainsKey("_"))
			{
				if (searchPos == text.Count - 1)
				{
					data.IsAnswer = true;
					data.Template = ai.Data["_"].Template;
					return data;
				}
				data.WildTexts.Add(new WildData { WildText = key + " ", WildType = data.WildType });
				data.IsInWildcard = true;
				data = FindTemplateRecursive(ai.Data["_"], text, data, searchPos + 1);
				if (data.IsAnswer)
					return data;
				data.WildTexts.RemoveAt(data.WildTexts.Count - 1);
				data.WildType = data.WildTexts.Count == 0 ? StarType.Pattern :
					data.WildTexts[data.WildTexts.Count - 1].WildType;
			}
			if (ai.Data.ContainsKey(key.ToUpper()))
			{
				if (searchPos == text.Count - 1)
				{
					data.IsAnswer = true;
					data.Template = ai.Data[key.ToUpper()].Template;
					return data;
				}
				if (key.ToUpper() == "<THAT>")
					data.WildType = StarType.That;
				if (key.ToUpper() == "<TOPIC>")
					data.WildType = StarType.Topic;
				data.IsInWildcard = false;
				data = FindTemplateRecursive(ai.Data[key.ToUpper()], text, data, searchPos + 1);
				if (data.IsAnswer)
					return data;
			}
			if (!data.IsAnswer && ai.Data.ContainsKey("*"))
			{
				if (searchPos == text.Count - 1)
				{
					data.IsAnswer = true;
					data.Template = ai.Data["*"].Template;
					return data;
				}
				data.WildTexts.Add(new WildData { WildText = key + " ", WildType = data.WildType });
				data.IsInWildcard = true;
				data = FindTemplateRecursive(ai.Data["*"], text, data, searchPos + 1);
				if (data.IsAnswer)
					return data;
				data.WildTexts.RemoveAt(data.WildTexts.Count - 1);
				data.WildType = data.WildTexts.Count == 0 ? StarType.Pattern :
					data.WildTexts[data.WildTexts.Count - 1].WildType;
			}
			return data;
		}

		private static void LoadWord(AIMLData parent, List<string> pattern, string template)
		{
			var key = pattern[0].ToUpper().Trim();
			pattern.RemoveAt(0);
			if (!parent.Data.ContainsKey(key))
				parent.Data.Add(key, new AIMLData(parent, key));
			if (pattern.Count > 0)
				LoadWord(parent.Data[key], pattern, template);
			else
				parent.Data[key].Template = template;
		}

		private void LoadInitialPredicates()
		{
			BotData.Add("NAME", "Alice");
			BotData.Add("GENDER", "female");
			BotData.Add("BIRTHDAY", "10/4/2001");
			BotData.Add("AGE", "12");
			BotData.Add("BIRTHPLACE", "Cruel place named Earth");
			BotData.Add("LOCATION", "World");
			BotData.Add("LENGUAGE", "English");
			BotData.Add("RELIGION", "trust in God");
			BotData.Add("PARTY", "love party");
			BotData.Add("SIGN", "X");
			BotData.Add("ARCH", "Windows");
			BotData.Add("SPECIES", "scholar");
			BotData.Add("GENUS", "DNA");
			BotData.Add("FOAVOURITEFOOD", "entrecote steak");
			BotData.Add("BOTMASTER", "The Programmer");
			BotData.Add("MASTER", "The Programmer");
			BotData.Add("FRIENDS", "All the people");
			BotData.Add("House", "I live in big nice house");
		}
	}

	public class AIMLData
	{
		public AIMLData(AIMLData parent, string key)
		{
			Parent = parent;
			Key = key;
			Data = new Dictionary<string, AIMLData>();
		}

		public Dictionary<string, AIMLData> Data { get; private set; }
		public AIMLData Parent { get; private set; }
		public string Template { get; set; }
		public string Key { get; set; }
	}

	public class PatternData
	{
		public PatternData()
		{
			WildTexts = new List<WildData>();
			WildType = StarType.Pattern;
		}

		public string Template { get; set; }
		public bool IsAnswer { get; set; }
		public bool IsInWildcard { get; set; }
		public StarType WildType { get; set; }
		public List<WildData> WildTexts { get; private set; }
	}

	public class WildData
	{
		public string WildText { get; set; }
		public StarType WildType { get; set; }
	}

	public enum StarType
	{
		Pattern,
		That,
		Topic
	}

	public static class ExtensionMethods
	{
		public static string InnerText(this XElement element)
		{
			return element.Nodes().Aggregate("", (current, node) => current + node.ToString());
		}
	}
}
