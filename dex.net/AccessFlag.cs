using System;

/// <summary>
/// Dex.NET - Mario Kosmiskas
/// 
/// Provided under the Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0
/// Commercial use requires attribution
/// </summary>
namespace dex.net
{
	[Flags]
	public enum AccessFlag
	{
		ACC_PUBLIC = 0x1,
		ACC_PRIVATE = 0x2,
		ACC_PROTECTED = 0x4,
		ACC_STATIC = 0x8,
		ACC_FINAL = 0x10,
		ACC_SYNCHRONIZED = 0x20,
		ACC_VOLATILE = 0x40,
		ACC_BRIDGE = 0x40,
		ACC_TRANSIENT = 0x80,
		ACC_VARARGS = 0x80,
		ACC_NATIVE = 0x100,
		ACC_INTERFACE = 0x200,
		ACC_ABSTRACT = 0x400,
		ACC_STRICT = 0x800,
		ACC_SYNTHETIC = 0x1000,
		ACC_ANNOTATION = 0x2000,
		ACC_ENUM = 0x4000,
		ACC_UNUSED = 0x8000,
		ACC_CONSTRUCTOR = 0x10000,
		ACC_DECLARED_SYNCHRONIZED = 0x20000
	}
}