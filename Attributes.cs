using System;

namespace Luny
{
	// Applying [NeedsReview] triggers compiler warning CS0618.
	// Use on any existing API entry point not yet reviewed under the new design.
	// Apply at method level (not class) so each method can be cleared individually.
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
	[Obsolete("==> needs a review.")]
	public sealed class NeedsReviewAttribute : Attribute {}

	// Applying [NeedsSmokeTest] triggers CS0618 as a reminder to write a smoke test.
	// Remove the attribute once a corresponding smoke test exists in LunyScript-Test.
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
	[Obsolete("==> needs a smoke test.")]
	public sealed class NeedsSmokeTestAttribute : Attribute {}
}
