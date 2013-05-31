using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook;
using System.IO;
using System.Net;
using System.Web;
using System.Dynamic;
using agsXMPP;
using agsXMPP.Xml.Dom;
using agsXMPP.protocol.iq.roster;
using agsXMPP.sasl;
using agsXMPP.Collections;
using agsXMPP.protocol.client;
using Newtonsoft.Json;

namespace faceTry
{
	class MyFacebookClient
	{

		private static string accessToken;

		public MyFacebookClient()
		{
			//accessToken = "CAAB6TY8wt98BAF22ZCHTc36rCWklpVr94LfwZAJOsdzmxAZA5rV5TdtpnIIZB4T1kOxm3OFMSPuTjcDz1hqELpK1ZBor47bU5duGWhKCPW8ShHUc3NSiyzMTvUoC8OGmtuZCbz48cCoMiOwVtuzdB8ZA3lzHovZBouMZD";
			accessToken = GetAccessToken();
		}

		public MyFacebookClient(string token)
		{
			accessToken = token;
		}

		public static void GetSampleWithoutAccessToken()
		{
			try
			{
				var fb = new FacebookClient();

				var result = (IDictionary<string, object>)fb.Get("/4");

				var id = (string)result["id"];
				var name = (string)result["name"];
				var firstName = (string)result["first_name"];
				var lastName = (string)result["last_name"];

				Console.WriteLine("Id: {0}", id);
				Console.WriteLine("Name: {0}", name);
				Console.WriteLine("First Name: {0}", firstName);
				Console.WriteLine("Last Name: {0}", lastName);
				Console.WriteLine();

				// Note: This json result is not the original json string as returned by Facebook.
				Console.WriteLine("Json: {0}", result.ToString());
			}
			catch (FacebookApiException)
			{
				// Note: make sure to handle this exception.
				throw;
			}
		}

		public static void GetSampleWithAccessToken(string accessToken)
		{
			try
			{
				var fb = new FacebookClient(accessToken);

				var result = (IDictionary<string, object>)fb.Get("/me");

				var id = (string)result["id"];
				var name = (string)result["name"];
				var firstName = (string)result["first_name"];
				var lastName = (string)result["last_name"];

				Console.WriteLine("Id: {0}", id);
				Console.WriteLine("Name: {0}", name);
				Console.WriteLine("First Name: {0}", firstName);
				Console.WriteLine("Last Name: {0}", lastName);
				Console.WriteLine();

				// Note: This json result is not the original json string as returned by Facebook.
				Console.WriteLine("Json: {0}", result.ToString());
			}
			catch (FacebookApiException)
			{
				// Note: make sure to handle this exception.
				throw;
			}
		}

		public static string PostToFriendsWall(string accessToken, string message)
		{
			try
			{
				var fb = new FacebookClient(accessToken);
				var id = "100001453014158";	// friends id
				var result = (IDictionary<string, object>)fb.Post("/" + id + "/feed",
											new Dictionary<string, object> { { "message", message } });
				var postId = (string)result["id"];

				Console.WriteLine("Post Id: {0}", postId);

				// Note: This json result is not the original json string as returned by Facebook.
				Console.WriteLine("Json: {0}", result.ToString());

				return postId;
			}
			catch (FacebookApiException)
			{
				// Note: make sure to handle this exception.
				return null;
			}


		}


		public static string PostToMyWall(string accessToken, string message)
		{
			try
			{
				var fb = new FacebookClient(accessToken);

				var result = (IDictionary<string, object>)fb.Post("/me/feed",
											new Dictionary<string, object> { { "message", message } });
				var postId = (string)result["id"];

				Console.WriteLine("Post Id: {0}", postId);

				// Note: This json result is not the original json string as returned by Facebook.
				Console.WriteLine("Json: {0}", result.ToString());

				return postId;
			}
			catch (FacebookApiException)
			{
				// Note: make sure to handle this exception.
				return null;
			}


		}

		public static void Delete(string accessToken, string id)
		{
			try
			{
				var fb = new FacebookClient(accessToken);

				var result = fb.Delete(id);

				// Note: This json result is not the original json string as returned by Facebook.
				Console.WriteLine("Json: {0}", result.ToString());
			}
			catch (FacebookApiException)
			{
				// Note: make sure to handle this exception.
				throw;
			}
		}

		public static string UploadPictureToWall(string accessToken, string filePath)
		{
			// sample usage: UploadPictureToWall(accessToken, @"C:\Users\Public\Pictures\Sample Pictures\Penguins.jpg");

			var mediaObject = new FacebookMediaObject
			{
				FileName = System.IO.Path.GetFileName(filePath),
				ContentType = "image/jpeg"
			};

			mediaObject.SetValue(System.IO.File.ReadAllBytes(filePath));

			try
			{
				var fb = new FacebookClient(accessToken);

				var result = (IDictionary<string, object>)fb.Post("me/photos", new Dictionary<string, object>
                                       {
                                           { "source", mediaObject },
                                           { "message","photo" }
                                       });

				var postId = (string)result["id"];

				Console.WriteLine("Post Id: {0}", postId);

				// Note: This json result is not the original json string as returned by Facebook.
				Console.WriteLine("Json: {0}", result.ToString());

				return postId;
			}
			catch (FacebookApiException)
			{
				// Note: make sure to handle this exception.
				throw;
			}
		}

		public static string GetPageAccessToken(string accessToken, string pageId)
		{
			try
			{
				var fb = new FacebookClient(accessToken);

				var parameters = new Dictionary<string, object>();

				// Note that the access_token field is a non-default field and
				// must be requested explicitly via the fields URL parameter.
				// In addition, you must use a user access_token with the 
				// manage_pages permission to make this request, 
				// where the user is an administrator of the Page. 
				parameters["fields"] = "access_token";
				var result = (IDictionary<string, object>)fb.Get(pageId, parameters);

				var pageAccessToken = (string)result["access_token"];

				Console.WriteLine("Access Token: {0}", accessToken);
				Console.WriteLine();

				// Note: This json result is not the original json string as returned by Facebook.
				Console.WriteLine("Json: {0}", result.ToString());

				return pageAccessToken;
			}
			catch (FacebookApiException)
			{
				// Note: make sure to handle this exception.
				throw;
			}
		}

		// get access token for app
		public string GetAccessToken()
		{
			var fb = new FacebookClient();
			dynamic result = fb.Get("oauth/access_token", new
			{
				client_id = "134473533405151",
				client_secret = "bd26d713bf4a96e0e745101da0785665",
				grant_type = "client_credentials"
			});
			return (string)result["access_token"];
		}

		// get List<id> of all friends
		public List<int> GetFriendsIdList(string accessToken)
		{
			List<int> ans = new List<int>();

			var fb = new FacebookClient(accessToken);
			dynamic myfriend = fb.Get("me/friends");
			foreach (dynamic friend in myfriend.data)
			{
				ans.Add(friend.id);
			}
			return ans;
		}
	}
}
