﻿using System;

namespace Manatee.Json.Internal
{
#if !NET45
	[CompilerAttributes.GeneratesWarning("This functionality is experimental.  It may change or be removed completely in future versions.")]
#endif
	internal class ExperimentalAttribute : Attribute
	{
	}

#if !NET45
	[CompilerAttributes.GeneratesWarning("Feedback is welcome on the Manatee Slack workspace (https://manateeopensource.slack.com/)")]
#endif
	internal class FeedbackWelcomeAttribute : Attribute
	{
	}
}