using System;

namespace Luny
{
	/// <summary>
	/// Exception thrown when a lifecycle-related error occurs in the Luny framework,
	/// such as singleton duplication or unexpected adapter removal.
	/// </summary>
	public sealed class LunyLifecycleException : LunyException
	{
		public LunyLifecycleException(String message)
			: base(message) {}
	}
}
