using System;
using System.IO;

/// <summary>
/// Dex.NET - Mario Kosmiskas
/// 
/// Provided under the Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0
/// Commercial use requires attribution
/// </summary>
namespace dex.net
{
	public class Disassembler
	{
		private Disassembler ()
		{
		}
		
		static internal OpCode Decode (BinaryReader reader)
		{
			OpCode opcode = null;
			var bytecode = reader.ReadByte ();

			switch (bytecode) 
			{
			case 0x00:
				opcode = new NopOpCode ();
				break;

			case 0x01:
				opcode = new MoveOpCode (reader);
				break;

			case 0x02:
				opcode = new MoveFrom16OpCode (reader);
				break;

			case 0x03:
				opcode = new Move16OpCode (reader);
				break;

			case 0x04:
				opcode = new MoveWideOpCode (reader);
				break;

			case 0x05:
				opcode = new MoveWideFrom16OpCode (reader);
				break;
					
			case 0x06:
				opcode = new MoveWide16OpCode (reader);
				break;

			case 0x07:
				opcode = new MoveObjectOpCode (reader);
				break;
					
			case 0x08:
				opcode = new MoveObjectFrom16OpCode (reader);
				break;
					
			case 0x09:
				opcode = new MoveObject16OpCode (reader);
				break;
				
			case 0x0a:
				opcode = new MoveResultOpCode (reader);
				break;
				
			case 0x0b:
				opcode = new MoveResultWideOpCode (reader);
				break;
					
			case 0x0c:
				opcode = new MoveResultObjectOpCode (reader);
				break;
					
			case 0x0d:
				opcode = new MoveExceptionOpCode (reader);
				break;
					
			case 0x0e:
				opcode = new ReturnVoidOpCode (reader);
				break;
				
			case 0x0f:
				opcode = new ReturnValueOpCode (reader);
				break;
				
			case 0x10:
				opcode = new ReturnWideOpCode (reader);
				break;
					
			case 0x11:
				opcode = new ReturnObjectOpCode (reader);
				break;
					
			case 0x12:
				opcode = new Const4OpCode (reader);
				break;
					
			case 0x13:
				opcode = new Const16OpCode (reader);
				break;
				
			case 0x14:
				opcode = new ConstOpCode (reader);
				break;
				
			case 0x15:
				opcode = new ConstHighOpCode (reader);
				break;
				
			case 0x16:
				opcode = new ConstWide16OpCode (reader);
				break;
				
			case 0x17:
				opcode = new ConstWide32OpCode (reader);
				break;
				
			case 0x18:
				opcode = new ConstWideOpCode (reader);
				break;
				
			case 0x19:
				opcode = new ConstWideHighOpCode (reader);
				break;
				
			case 0x1a:
				opcode = new ConstStringOpCode (reader);
				break;
				
			case 0x1b:
				opcode = new ConstStringJumboOpCode (reader);
				break;
				
			case 0x1c:
				opcode = new ConstClassOpCode (reader);
				break;
				
			case 0x1d:
				opcode = new MonitorEnterOpCode (reader);
				break;
				
			case 0x1e:
				opcode = new MonitorExitOpCode (reader);
				break;
				
			case 0x1f:
				opcode = new CheckCastOpCode (reader);
				break;
				
			case 0x20:
				opcode = new InstanceOfOpCode (reader);
				break;
				
			case 0x21:
				opcode = new ArrayLengthOpCode (reader);
				break;
				
			case 0x22:
				opcode = new NewInstanceOpCode (reader);
				break;
				
			case 0x23:
				opcode = new NewArrayOfOpCode (reader);
				break;
				
			case 0x24:
				opcode = new FilledNewArrayOfOpCode (reader);
				break;
				
			case 0x25:
				opcode = new FilledNewArrayRangeOpCode (reader);
				break;
				
			case 0x26:
				opcode = new FillArrayDataOpCode (reader);
				break;
				
			case 0x27:
				opcode = new ThrowOpCode (reader);
				break;
				
			case 0x28:
				opcode = new GotoOpCode (reader);
				break;
				
			case 0x29:
				opcode = new Goto16OpCode (reader);
				break;
				
			case 0x2a:
				opcode = new Goto32OpCode (reader);
				break;
				
			case 0x2b:
				opcode = new PackedSwitchOpCode (reader);
				break;
				
			case 0x2c:
				opcode = new SparseSwitchOpCode (reader);
				break;
				
			case 0x2d:
				opcode = new CmplFloatOpCode (reader);
				break;
				
			case 0x2e:
				opcode = new CmpgFloatOpCode (reader);
				break;
				
			case 0x2f:
				opcode = new CmplDoubleOpCode (reader);
				break;
				
			case 0x30:
				opcode = new CmpgDoubleOpCode (reader);
				break;
				
			case 0x31:
				opcode = new CmpLongOpCode (reader);
				break;
				
			case 0x32:
				opcode = new IfEqOpCode (reader);
				break;
				
			case 0x33:
				opcode = new IfNeOpCode (reader);
				break;
				
			case 0x34:
				opcode = new IfLtOpCode (reader);
				break;
				
			case 0x35:
				opcode = new IfGeOpCode (reader);
				break;
				
			case 0x36:
				opcode = new IfGtOpCode (reader);
				break;
				
			case 0x37:
				opcode = new IfLeOpCode (reader);
				break;
				
			case 0x38:
				opcode = new IfEqzOpCode (reader);
				break;
				
			case 0x39:
				opcode = new IfNezOpCode (reader);
				break;
				
			case 0x3a:
				opcode = new IfLtzOpCode (reader);
				break;
				
			case 0x3b:
				opcode = new IfGezOpCode (reader);
				break;
				
			case 0x3c:
				opcode = new IfGtzOpCode (reader);
				break;
				
			case 0x3d:
				opcode = new IfLezOpCode (reader);
				break;
				
			case 0x44:
				opcode = new AgetOpCode (reader);
				break;
				
			case 0x45:
				opcode = new AgetWideOpCode (reader);
				break;
				
			case 0x46:
				opcode = new AgetObjectOpCode (reader);
				break;
				
			case 0x47:
				opcode = new AgetBooleanOpCode (reader);
				break;
				
			case 0x48:
				opcode = new AgetByteOpCode (reader);
				break;
				
			case 0x49:
				opcode = new AgetCharOpCode (reader);
				break;
				
			case 0x4a:
				opcode = new AgetShortOpCode (reader);
				break;
				
			case 0x4b:
				opcode = new AputOpCode (reader);
				break;
				
			case 0x4c:
				opcode = new AputWideOpCode (reader);
				break;
				
			case 0x4d:
				opcode = new AputObjectOpCode (reader);
				break;
				
			case 0x4e:
				opcode = new AputBooleanOpCode (reader);
				break;
				
			case 0x4f:
				opcode = new AputByteOpCode (reader);
				break;
				
			case 0x50:
				opcode = new AputCharOpCode (reader);
				break;
				
			case 0x51:
				opcode = new AputShortOpCode (reader);
				break;
				
			case 0x52:
				opcode = new IgetOpCode (reader);
				break;
				
			case 0x53:
				opcode = new IgetWideOpCode (reader);
				break;
				
			case 0x54:
				opcode = new IgetObjectOpCode (reader);
				break;
				
			case 0x55:
				opcode = new IgetBooleanOpCode (reader);
				break;
				
			case 0x56:
				opcode = new IgetByteOpCode (reader);
				break;
				
			case 0x57:
				opcode = new IgetCharOpCode (reader);
				break;
				
			case 0x58:
				opcode = new IgetShortOpCode (reader);
				break;
				
			case 0x59:
				opcode = new IputOpCode (reader);
				break;
				
			case 0x5a:
				opcode = new IputWideOpCode (reader);
				break;
				
			case 0x5b:
				opcode = new IputObjectOpCode (reader);
				break;
				
			case 0x5c:
				opcode = new IputBooleanOpCode (reader);
				break;
				
			case 0x5d:
				opcode = new IputByteOpCode (reader);
				break;
				
			case 0x5e:
				opcode = new IputCharOpCode (reader);
				break;
				
			case 0x5f:
				opcode = new IputShortOpCode (reader);
				break;
				
			case 0x60:
				opcode = new SgetOpCode (reader);
				break;
				
			case 0x61:
				opcode = new SgetWideOpCode (reader);
				break;
				
			case 0x62:
				opcode = new SgetObjectOpCode (reader);
				break;
				
			case 0x63:
				opcode = new SgetBooleanOpCode (reader);
				break;
				
			case 0x64:
				opcode = new SgetByteOpCode (reader);
				break;
				
			case 0x65:
				opcode = new SgetCharOpCode (reader);
				break;
				
			case 0x66:
				opcode = new SgetShortOpCode (reader);
				break;
				
			case 0x67:
				opcode = new SputOpCode (reader);
				break;
				
			case 0x68:
				opcode = new SputWideOpCode (reader);
				break;
				
			case 0x69:
				opcode = new SputObjectOpCode (reader);
				break;
				
			case 0x6a:
				opcode = new SputBooleanOpCode (reader);
				break;
				
			case 0x6b:
				opcode = new SputByteOpCode (reader);
				break;
				
			case 0x6c:
				opcode = new SputCharOpCode (reader);
				break;
				
			case 0x6d:
				opcode = new SputShortOpCode (reader);
				break;
				
			case 0x6e:
				opcode = new InvokeVirtualOpCode (reader);
				break;
				
			case 0x6f:
				opcode = new InvokeSuperOpCode (reader);
				break;
				
			case 0x70:
				opcode = new InvokeDirectOpCode (reader);
				break;
				
			case 0x71:
				opcode = new InvokeStaticOpCode (reader);
				break;
				
			case 0x72:
				opcode = new InvokeInterfaceOpCode (reader);
				break;
				
			case 0x74:
				opcode = new InvokeVirtualRangeOpCode (reader);
				break;
				
			case 0x75:
				opcode = new InvokeSuperRangeOpCode (reader);
				break;
				
			case 0x76:
				opcode = new InvokeDirectRangeOpCode (reader);
				break;
				
			case 0x77:
				opcode = new InvokeStaticRangeOpCode (reader);
				break;
				
			case 0x78:
				opcode = new InvokeInterfaceRangeOpCode (reader);
				break;
				
			case 0x7b:
				opcode = new NegIntOpCode (reader);
				break;
				
			case 0x7c:
				opcode = new NotIntOpCode (reader);
				break;
				
			case 0x7d:
				opcode = new NegLongOpCode (reader);
				break;
				
			case 0x7e:
				opcode = new NotLongOpCode (reader);
				break;
				
			case 0x7f:
				opcode = new NegFloatOpCode (reader);
				break;
				
			case 0x80:
				opcode = new NegDoubleOpCode (reader);
				break;
				
			case 0x81:
				opcode = new IntToLongOpCode (reader);
				break;
				
			case 0x82:
				opcode = new IntToFloatOpCode (reader);
				break;
				
			case 0x83:
				opcode = new IntToDoubleOpCode (reader);
				break;
				
			case 0x84:
				opcode = new LongToIntOpCode (reader);
				break;
				
			case 0x85:
				opcode = new LongToFloatOpCode (reader);
				break;
				
			case 0x86:
				opcode = new LongToDoubleOpCode (reader);
				break;
				
			case 0x87:
				opcode = new FloatToIntOpCode (reader);
				break;
				
			case 0x88:
				opcode = new FloatToLongOpCode (reader);
				break;
				
			case 0x89:
				opcode = new FloatToDoubleOpCode (reader);
				break;
				
			case 0x8a:
				opcode = new DoubleToIntOpCode (reader);
				break;
				
			case 0x8b:
				opcode = new DoubleToLongOpCode (reader);
				break;
				
			case 0x8c:
				opcode = new DoubleToFloatOpCode (reader);
				break;
				
			case 0x8d:
				opcode = new IntToByteOpCode (reader);
				break;
				
			case 0x8e:
				opcode = new IntToCharOpCode (reader);
				break;
				
			case 0x8f:
				opcode = new IntToShortOpCode (reader);
				break;
				
			case 0x90:
				opcode = new AddIntOpCode (reader);
				break;
				
			case 0x91:
				opcode = new SubIntOpCode (reader);
				break;
				
			case 0x92:
				opcode = new MulIntOpCode (reader);
				break;
				
			case 0x93:
				opcode = new DivIntOpCode (reader);
				break;
				
			case 0x94:
				opcode = new RemIntOpCode (reader);
				break;
				
			case 0x95:
				opcode = new AndIntOpCode (reader);
				break;
				
			case 0x96:
				opcode = new OrIntOpCode (reader);
				break;
				
			case 0x97:
				opcode = new XorIntOpCode (reader);
				break;
				
			case 0x98:
				opcode = new ShlIntOpCode (reader);
				break;
				
			case 0x99:
				opcode = new ShrIntOpCode (reader);
				break;
				
			case 0x9a:
				opcode = new UshrIntOpCode (reader);
				break;
				
			case 0x9b:
				opcode = new AddLongOpCode (reader);
				break;
				
			case 0x9c:
				opcode = new SubLongOpCode (reader);
				break;
				
			case 0x9d:
				opcode = new MulLongOpCode (reader);
				break;
				
			case 0x9e:
				opcode = new DivLongOpCode (reader);
				break;
				
			case 0x9f:
				opcode = new RemLongOpCode (reader);
				break;
				
			case 0xa0:
				opcode = new AndLongOpCode (reader);
				break;
				
			case 0xa1:
				opcode = new OrLongOpCode (reader);
				break;
				
			case 0xa2:
				opcode = new XorLongOpCode (reader);
				break;
				
			case 0xa3:
				opcode = new ShlLongOpCode (reader);
				break;
				
			case 0xa4:
				opcode = new ShrLongOpCode (reader);
				break;
				
			case 0xa5:
				opcode = new UshrLongOpCode (reader);
				break;
				
			case 0xa6:
				opcode = new AddFloatOpCode (reader);
				break;
				
			case 0xa7:
				opcode = new SubFloatOpCode (reader);
				break;
				
			case 0xa8:
				opcode = new MulFloatOpCode (reader);
				break;
				
			case 0xa9:
				opcode = new DivFloatOpCode (reader);
				break;
				
			case 0xaa:
				opcode = new RemFloatOpCode (reader);
				break;
				
			case 0xab:
				opcode = new AddDoubleOpCode (reader);
				break;
				
			case 0xac:
				opcode = new SubDoubleOpCode (reader);
				break;
				
			case 0xad:
				opcode = new MulDoubleOpCode (reader);
				break;
				
			case 0xae:
				opcode = new DivDoubleOpCode (reader);
				break;
				
			case 0xaf:
				opcode = new RemDoubleOpCode (reader);
				break;

			case 0xb0:
				opcode = new AddInt2AddrOpCode (reader);
				break;
				
			case 0xb1:
				opcode = new SubInt2AddrOpCode (reader);
				break;
				
			case 0xb2:
				opcode = new MulInt2AddrOpCode (reader);
				break;
				
			case 0xb3:
				opcode = new DivInt2AddrOpCode (reader);
				break;
				
			case 0xb4:
				opcode = new RemInt2AddrOpCode (reader);
				break;
				
			case 0xb5:
				opcode = new AndInt2AddrOpCode (reader);
				break;
				
			case 0xb6:
				opcode = new OrInt2AddrOpCode (reader);
				break;
				
			case 0xb7:
				opcode = new XorInt2AddrOpCode (reader);
				break;
				
			case 0xb8:
				opcode = new ShlInt2AddrOpCode (reader);
				break;
				
			case 0xb9:
				opcode = new ShrInt2AddrOpCode (reader);
				break;
				
			case 0xba:
				opcode = new UshrInt2AddrOpCode (reader);
				break;
				
			case 0xbb:
				opcode = new AddLong2AddrOpCode (reader);
				break;
				
			case 0xbc:
				opcode = new SubLong2AddrOpCode (reader);
				break;
				
			case 0xbd:
				opcode = new MulLong2AddrOpCode (reader);
				break;
				
			case 0xbe:
				opcode = new DivLong2AddrOpCode (reader);
				break;
				
			case 0xbf:
				opcode = new RemLong2AddrOpCode (reader);
				break;
				
			case 0xc0:
				opcode = new AndLong2AddrOpCode (reader);
				break;
				
			case 0xc1:
				opcode = new OrLong2AddrOpCode (reader);
				break;
				
			case 0xc2:
				opcode = new XorLong2AddrOpCode (reader);
				break;
				
			case 0xc3:
				opcode = new ShlLong2AddrOpCode (reader);
				break;
				
			case 0xc4:
				opcode = new ShrLong2AddrOpCode (reader);
				break;
				
			case 0xc5:
				opcode = new UshrLong2AddrOpCode (reader);
				break;
				
			case 0xc6:
				opcode = new AddFloat2AddrOpCode (reader);
				break;
				
			case 0xc7:
				opcode = new SubFloat2AddrOpCode (reader);
				break;
				
			case 0xc8:
				opcode = new MulFloat2AddrOpCode (reader);
				break;
				
			case 0xc9:
				opcode = new DivFloat2AddrOpCode (reader);
				break;
				
			case 0xca:
				opcode = new RemFloat2AddrOpCode (reader);
				break;
				
			case 0xcb:
				opcode = new AddDouble2AddrOpCode (reader);
				break;
				
			case 0xcc:
				opcode = new SubDouble2AddrOpCode (reader);
				break;
				
			case 0xcd:
				opcode = new MulDouble2AddrOpCode (reader);
				break;
				
			case 0xce:
				opcode = new DivDouble2AddrOpCode (reader);
				break;
				
			case 0xcf:
				opcode = new RemDouble2AddrOpCode (reader);
				break;
				
			case 0xd0:
				opcode = new AddIntLit16OpCode (reader);
				break;
				
			case 0xd1:
				opcode = new RsubIntOpCode (reader);
				break;
				
			case 0xd2:
				opcode = new MulIntLit16OpCode (reader);
				break;
				
			case 0xd3:
				opcode = new DivIntLit16OpCode (reader);
				break;
				
			case 0xd4:
				opcode = new RemIntLit16OpCode (reader);
				break;
				
			case 0xd5:
				opcode = new AndIntLit16OpCode (reader);
				break;
				
			case 0xd6:
				opcode = new OrIntLit16OpCode (reader);
				break;
				
			case 0xd7:
				opcode = new XorIntLit16OpCode (reader);
				break;

			case 0xd8:
				opcode = new AddIntLit8OpCode (reader);
				break;
				
			case 0xd9:
				opcode = new RsubIntLit8OpCode (reader);
				break;
				
			case 0xda:
				opcode = new MulIntLit8OpCode (reader);
				break;
				
			case 0xdb:
				opcode = new DivIntLit8OpCode (reader);
				break;
				
			case 0xdc:
				opcode = new RemIntLit8OpCode (reader);
				break;
				
			case 0xdd:
				opcode = new AndIntLit8OpCode (reader);
				break;
				
			case 0xde:
				opcode = new OrIntLit8OpCode (reader);
				break;
				
			case 0xdf:
				opcode = new XorIntLit8OpCode (reader);
				break;
				
			case 0xe0:
				opcode = new ShlIntLit8OpCode (reader);
				break;
				
			case 0xe1:
				opcode = new ShrIntLit8OpCode (reader);
				break;
				
			case 0xe2:
				opcode = new UshrIntLit8OpCode (reader);
				break;

			default:
				opcode = new UnknownOpCode (bytecode);
				break;
			}

			return opcode;
		}
		
	}
}

