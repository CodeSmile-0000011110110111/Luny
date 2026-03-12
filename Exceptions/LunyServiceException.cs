using System;

namespace Luny
{
	/// <summary>
	/// Exception thrown when a service-related error occurs in the Luny framework,
	/// such as service not found or invalid service interface implementation.
	/// </summary>
	public sealed class LunyServiceException : LunyException
	{
		public LunyServiceException(String message)
			: base(message) {}
	}
}
