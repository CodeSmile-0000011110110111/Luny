using System;

namespace Luny
{
	public static class Emoji
	{
		public static String IsTrue(Boolean value) => value ? True : False;
		public static String IsSatisfied(Boolean value) => value ? Satisfied : Unsatisfied;
		public static String IsKnown(Boolean value) => value ? Known : Unknown;
		public static String IsAgreed(Boolean value) => value ? Agreed : Disagreed;
		public static String IsApproved(Boolean value) => value ? Approved : Disapproved;
		public static String IsFound(Boolean value) => value ? Found : NotFound;
#pragma warning disable 0414 // assigned but its value is never used
		private static readonly String True = "✅";
		private static readonly String False = "❌";

		private static readonly String Satisfied = "🟢";
		private static readonly String Unsatisfied = "🔴";

		private static readonly String Known = "💡";
		private static readonly String Unknown = "🤔";

		private static readonly String Agreed = "👍";
		private static readonly String Disagreed = "👎";

		private static readonly String Approved = "🤝";
		private static readonly String Disapproved = "🙅";

		private static readonly String Found = "🎯";
		private static readonly String NotFound = "🤷";

		private static readonly String Searching = "🔍";
#pragma warning restore 0414
	}
}
