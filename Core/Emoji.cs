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
		public static String IsEnabled(Boolean value) => value ? Enabled : Disabled;

#pragma warning disable 0414 // assigned but its value is never used
		public static readonly String True = "✅";
		public static readonly String False = "❌";

		public static readonly String Satisfied = "🟢";
		public static readonly String Unsatisfied = "🔴";

		public static readonly String Day = "☀️";
		public static readonly String Night = "🌑";

		public static readonly String On = "🔌";
		public static readonly String Off = "🚫";

		public static readonly String Active = "⚔️";
		public static readonly String Passive = "🛡️";

		public static readonly String Enabled = "⚡";
		public static readonly String Disabled = "💤";

		public static readonly String Full = "🔋";
		public static readonly String Empty = "🪫";

		public static readonly String Known = "💡";
		public static readonly String Unknown = "🤔";

		public static readonly String Agreed = "👍";
		public static readonly String Disagreed = "👎";

		public static readonly String Approved = "🤝";
		public static readonly String Disapproved = "🙅";

		public static readonly String Found = "🎯";
		public static readonly String NotFound = "🤷";

		public static readonly String Searching = "🔍";

		public static readonly String Position = "📍";
		public static readonly String Rotation = "🌀"; //⟳🔄
		public static readonly String Scale = "📐";

		public static readonly String Folder = "📂";
		public static readonly String Dependency = "🔗";

		public static readonly String Parent = "🐔";
		public static readonly String Child = "🐣";

		public static readonly String NullReference = "✖️"; //🕳️
		public static readonly String Destroyed = "💀";

		public static readonly String Negation = "❕";
		public static readonly String LogicalAnd = " <color=\"grey\"><b>AND</b></color> ";
		public static readonly String LogicalOr = " <color=\"grey\"><b>OR</b></color> ";


#pragma warning restore 0414
	}
}
