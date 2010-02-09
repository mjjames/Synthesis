using System.Text.RegularExpressions;

namespace mjjames.AdminSystem.classes
{
	public static class SQLHelpers
	{
		public static string SQLSafe(string value)
		{
			return value.Replace("'", "''");
		}

		public static string URLSafe(string value)
		{
			string outValue = value.ToLower();

			outValue = outValue.Replace(" ", "-");
			outValue = outValue.Replace("&", "and");
			
			const string pattern = @"[^a-zA-Z0-9_\-]";
			Regex validUrlChars = new Regex(pattern, RegexOptions.IgnoreCase);

			outValue = validUrlChars.Replace(outValue, "");
			
			if(outValue.EndsWith("-")) //trim off a trailing -
			{
				outValue = outValue.Remove(outValue.Length - 1);
			}
			
			return outValue;
		}
	}
}
