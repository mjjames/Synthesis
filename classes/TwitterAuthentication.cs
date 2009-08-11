using System;
using Dimebrain.TweetSharp.Extensions;
using Dimebrain.TweetSharp.Fluent;
using Dimebrain.TweetSharp.Model;

namespace mjjames.AdminSystem.classes
{
	public class TwitterAuthentication
	{
		private OAuthToken _requestToken = new OAuthToken();
		private readonly string _consumerKey;
		private readonly string _consumerSecret;
		
		public TwitterAuthentication(string consumerKey, string consumerSecret)
		{
			_consumerKey = consumerKey;
			_consumerSecret = consumerSecret;
		}
		
		public string GetToken()
		{
			_requestToken = GetRequestToken(_consumerKey, _consumerSecret);
			return _requestToken.Token;
		}
	
		public string BeginAuthenticate()
		{
			// get an authenticated request token from twitter
			
			string oauthURL = FluentTwitter.CreateRequest()
								.Authentication
								.GetAuthorizationUrl(_requestToken.Token);

			return oauthURL;
		}

		public TwitterAuth FinishAuthenticate(string pin, string token)
		{
			OAuthToken accessToken = GetAccessToken(_consumerKey, _consumerSecret, token, pin);
			return GetAuthenticatedUser(accessToken);
		}
		
		private static TwitterAuth GetResponse(string response)
		{
			TwitterAuth auth = new TwitterAuth {User = response.AsUser()};

			if (auth.User == null)
			{
				auth.Error = response.AsError();
			}

			return auth;
		}

		private static OAuthToken GetRequestToken(string consumerKey, string consumerSecret)
		{
			var requestToken = FluentTwitter.CreateRequest()
				.Authentication.GetRequestToken(consumerKey, consumerSecret);

			var response = requestToken.Request();
			var result = response.AsToken();

			if (result == null)
			{
				var error = response.AsError();
				if (error != null)
				{
					throw new Exception(error.ErrorMessage);
				}
			}

			return result;
		}

		private static OAuthToken GetAccessToken(string consumerKey, string consumerSecret, string token, string pin)
		{
			var accessToken = FluentTwitter.CreateRequest()
											.Authentication
											.GetAccessToken(consumerKey, consumerSecret,token, pin);

			var response = accessToken.Request();
			var result = response.AsToken();

			if (result == null)
			{
				var error = response.AsError();
				if (error != null)
				{
					throw new Exception(error.ErrorMessage);
				}
			}

			return result;
		}

		public TwitterAuth GetAuthenticatedUser(OAuthToken accessToken)
		{
			// make an authenticated call to Twitter with the token and secret
			var verify = FluentTwitter.CreateRequest()
				.AuthenticateWith(_consumerKey, _consumerSecret, accessToken.Token, accessToken.TokenSecret)
				.Account().VerifyCredentials().AsJson();

			var response = verify.Request();
			TwitterAuth auth = GetResponse(response);
			auth.AccessToken = accessToken;
			return auth;
		}
	}
}
