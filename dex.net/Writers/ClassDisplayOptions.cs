using System;

namespace dex.net
{
	[Flags]
	public enum ClassDisplayOptions
	{
		ClassAnnotations = 1,
		ClassName = 2,
		ClassDetails = 4,
		MethodAnnotations = 8,
		Methods = 16,
		Fields = 32,
		OpCodes = 64
	}
}

