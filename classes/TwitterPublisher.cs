using System;
using Dimebrain.TweetSharp;
using Dimebrain.TweetSharp.Extensions;
using Dimebrain.TweetSharp.Fluent;
using Dimebrain.TweetSharp.Model;

namespace mjjames.AdminSystem.classes
{
	public class TwitterPublisher
	{
		private readonly OAuthToken _auth;
		private readonly string _consumerKey;
		private readonly string _consumerSecret;
		
		public TwitterPublisher(string consumerKey, string consumerSecret, string token, string tokenSecret)
		{
			_consumerKey = consumerKey;
			_consumerSecret = consumerSecret;
			_auth = new OAuthToken()
			        	{
							Token = token,
							TokenSecret = tokenSecret
			        	};

			if (!String.IsNullOrEmpty(_auth.Token) && !String.IsNullOrEmpty(_auth.TokenSecret)) return;
			
			System.Diagnostics.Trace.TraceWarning("Invalid Twitter OAuth Settings - Please Check and Try Again");
			_auth = null;
		}
		
		public bool PublishMessage(string message)
		{
			if(_auth == null) return false;
			try
			{
				// make an authenticated call to Twitter with the token and secret
				var publish = FluentTwitter.CreateRequest(GetClientInfo())
					.Configuration.UseUrlShortening(ShortenUrlServiceProvider.IsGd)
					.AuthenticateWith(_auth.Token, _auth.TokenSecret)
					.Statuses().Update(message).AsJson();

				var response = publish.Request();
				var tweet = response.AsStatuses();
				return true;
			}
			catch(Exception e)
			{
				System.Diagnostics.Debug.WriteLine("Twitter Publish Message Error: {0}", e.Message);
				return false;
			}
			
		}
		
		private TwitterClientInfo GetClientInfo()
		{
			return new TwitterClientInfo()
			       	{
						ClientName = "MJJames Admin Tool",
						ClientUrl = "http://mjjames.co.uk",
						ClientVersion = "1.6",
						ConsumerKey = _consumerKey,
						ConsumerSecret = _consumerSecret
			       	};
		}
	}
}
