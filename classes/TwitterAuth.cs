using Dimebrain.TweetSharp.Model;

namespace mjjames.AdminSystem.classes
{
	public class TwitterAuth
	{
		public TwitterUser User{ get;set; }
		public TwitterError Error { get; set; }
		public OAuthToken AccessToken { get; set;}
	}
}
