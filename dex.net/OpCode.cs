using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Dex.NET - Mario Kosmiskas
/// 
/// Provided under the Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0
/// Commercial use requires attribution
/// </summary>
namespace dex.net
{
	/// <summary>
	/// Parent class for all Android opcodes
	/// </summary>
	public abstract class OpCode
	{
		/// <summary>
		/// Gets or sets the instruction name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

		/// <summary>
		/// Gets or sets the instruction type.
		/// </summary>
		/// <value>The instruction.</value>
		public Instructions Instruction { get; private set; }

		public long OpCodeOffset { get; internal set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Name">Name.</param>
		/// <param name="insn">Insn.</param>
		public OpCode(string name, Instructions insn)
		{
			Name = name;
			Instruction = insn;
		}

		/// <summary>
		/// Utility method to parse two values encoded in a single byte.
		/// </summary>
		/// <param name="value">Byte to be decoded</param>
		/// <param name="high">Value of the most significant 4 bits</param>
		/// <param name="low">Value of the least significant 4 bits</param>
		protected void SplitByte(byte value, out byte high, out byte low)
		{
			high = (byte)((value & 0xf0) >> 4);
			low = (byte)(value & 0x0f);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="dex.net.OpCode"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="dex.net.OpCode"/>.</returns>
		public override string ToString ()
		{
			return Name;
		}
	}

	/// <summary>
	/// Instructions that implement this interface use a data table
	/// at the end of the method which must not be decoded when
	/// parsing instructions 
	/// </summary>
	interface DataExtendedOpCode
	{
		long DataTableOffset { get; }
	}

	/// <summary>
	/// Instruction which can branch, this interface exposes
	/// the address to jump to
	/// </summary>
	public interface JumpOpCode
	{
		long GetTargetAddress ();
	}

	/// <summary>
	/// An opcode in which the first byte is the
	/// destination register
	/// </summary>
	public abstract class Register8OpCode : OpCode
	{
		public byte Destination;

		internal Register8OpCode (BinaryReader reader, string name, Instructions insn) : base (name, insn)
		{
			Destination = reader.ReadByte();
		}
	}
	
	/// <summary>
	/// An unsupported opcode
	/// </summary>
	public class UnknownOpCode : OpCode
	{
		readonly int Bytecode;

		internal UnknownOpCode (int bytecode) : base ("unknown", Instructions.Unknown)
		{
			Bytecode = bytecode;
		}

		public override string ToString ()
		{
			return string.Format("{0} {1:X}", Name, Bytecode);
		}
	}

	/// <summary>
	/// Nop op code.
	/// </summary>
	public class NopOpCode : OpCode
	{
		internal NopOpCode () : base ("nop", Instructions.Nop) {}
	}

	/// <summary>
	/// Parent class for MOVE operations which
	/// move a value from <F> to <T>
	/// </summary>
	public abstract class AbsMoveOpCode<T,F> : OpCode
	{
		public T To;
		public F From;

		public AbsMoveOpCode(string name, Instructions insn) : base (name, insn) {}

		public override string ToString ()
		{
			return string.Format("{0} v{1}, v{2}", Name, To, From);
		}
	}

	/// <summary>
	/// Move the contents of one non-object register to another.
	/// </summary>
	public class MoveOpCode : AbsMoveOpCode<byte,byte>
	{
		internal MoveOpCode (BinaryReader reader) : base ("move", Instructions.Move)
		{
			SplitByte(reader.ReadByte(), out From, out To);
		}
	}

	/// <summary>
	/// Move the contents of one non-object register to another.
	/// </summary>
	public class MoveFrom16OpCode : AbsMoveOpCode<byte,ushort>
	{
		internal MoveFrom16OpCode (BinaryReader reader) : base ("move/from16", Instructions.MoveFrom16)
		{
			To = reader.ReadByte();
			From = reader.ReadUInt16();
		}
	}

	/// <summary>
	/// Move the contents of one non-object register to another.
	/// </summary>
	public class Move16OpCode : AbsMoveOpCode<ushort,ushort>
	{
		internal Move16OpCode (BinaryReader reader) : base ("move/16", Instructions.Move16)
		{
			To = reader.ReadUInt16();
			From = reader.ReadUInt16();
		}
	}

	/// <summary>
	/// Move the contents of one register-pair to another.
	/// Note: It is legal to move from vN to either vN-1 or vN+1, 
	/// so implementations must arrange for both halves of a 
	/// register pair to be read before anything is written.
	/// </summary>
	public class MoveWideOpCode : AbsMoveOpCode<byte,byte>
	{
		internal MoveWideOpCode (BinaryReader reader) : base("move-wide", Instructions.MoveWide)
		{
			SplitByte(reader.ReadByte(), out From, out To);
		}
	}

	/// <summary>
	/// Move the contents of one register-pair to another.
	/// Note: Implementation considerations are the same as move-wide, above.
	/// </summary>
	public class MoveWideFrom16OpCode : AbsMoveOpCode<byte,ushort>
	{
		internal MoveWideFrom16OpCode (BinaryReader reader) : base("move-wide/from16", Instructions.MoveWideFrom16)
		{
			To = reader.ReadByte();
			From = reader.ReadUInt16();
		}
	}

	/// <summary>
	/// Move the contents of one register-pair to another.
	/// Note: Implementation considerations are the same as move-wide, above.
	/// </summary>
	public class MoveWide16OpCode : AbsMoveOpCode<ushort,ushort>
	{
		internal MoveWide16OpCode (BinaryReader reader) : base("move-wide/16", Instructions.MoveWide16)
		{
			To = reader.ReadUInt16();
			From = reader.ReadUInt16();
		}
	}

	/// <summary>
	/// Move the contents of one object-bearing register to another.
	/// </summary>
	public class MoveObjectOpCode : AbsMoveOpCode<byte,byte>
	{
		internal MoveObjectOpCode (BinaryReader reader) : base("move-object", Instructions.MoveObject)
		{
			SplitByte(reader.ReadByte(), out From, out To);
		}
	}

	/// <summary>
	/// Move the contents of one object-bearing register to another.
	/// </summary>
	public class MoveObjectFrom16OpCode : AbsMoveOpCode<byte,ushort>
	{
		internal MoveObjectFrom16OpCode (BinaryReader reader) : base("move-object/from16", Instructions.MoveObjectFrom16)
		{
			To = reader.ReadByte();
			From = reader.ReadUInt16();
		}
	}

	/// <summary>
	/// Move the contents of one object-bearing register to another.
	/// </summary>
	public class MoveObject16OpCode : AbsMoveOpCode<ushort,ushort>
	{
		internal MoveObject16OpCode (BinaryReader reader) : base("move-object/16", Instructions.MoveObject16)
		{
			To = reader.ReadUInt16();
			From = reader.ReadUInt16();
		}
	}

	/// <summary>
	/// Parent class for move-result operations
	/// </summary>
	public class AbsMoveResult : Register8OpCode
	{
		public AbsMoveResult(BinaryReader reader, string name, Instructions insn) : base (reader, name, insn) {}

		public override string ToString ()
		{
			return string.Format("{0} v{1}", Name, Destination);
		}
	}

	/// <summary>
	/// Move the single-word non-object result of the most recent invoke-kind into 
	/// the indicated register. This must be done as the instruction immediately 
	/// after an invoke-kind whose (single-word, non-object) result is not to be 
	/// ignored; anywhere else is invalid.
	/// </summary>
	public class MoveResultOpCode : AbsMoveResult
	{
		internal MoveResultOpCode (BinaryReader reader) : base (reader, "move-result", Instructions.MoveResult) {}
	}

	/// <summary>
	/// Move the double-word result of the most recent invoke-kind into the indicated 
	/// register pair. This must be done as the instruction immediately after an 
	/// invoke-kind whose (double-word) result is not to be ignored; anywhere else is invalid.
	/// </summary>
	public class MoveResultWideOpCode : AbsMoveResult
	{
		internal MoveResultWideOpCode (BinaryReader reader) : base (reader, "move-result-wide", Instructions.MoveResultWide) {}
	}

	/// <summary>
	/// Move the object result of the most recent invoke-kind into the indicated 
	/// register. This must be done as the instruction immediately after an 
	/// invoke-kind or filled-new-array whose (object) result is not to be ignored; 
	/// anywhere else is invalid.
	/// </summary>
	public class MoveResultObjectOpCode : AbsMoveResult
	{
		internal MoveResultObjectOpCode (BinaryReader reader) : base (reader, "move-result-object", Instructions.MoveResultObject) {}
	}

	/// <summary>
	/// Save a just-caught exception into the given register. This must be the 
	/// first instruction of any exception handler whose caught exception is 
	/// not to be ignored, and this instruction must only ever occur as the 
	/// first instruction of an exception handler; anywhere else is invalid.
	/// </summary>
	public class MoveExceptionOpCode : AbsMoveResult
	{
		internal MoveExceptionOpCode (BinaryReader reader) : base (reader, "move-exception", Instructions.MoveException) {}
	}

	/// <summary>
	/// Return from a void method.
	/// </summary>
	public class ReturnVoidOpCode : OpCode
	{
		internal ReturnVoidOpCode (BinaryReader reader) : base ("return-void", Instructions.ReturnVoid)
		{
			// must read another byte to complete the 16 bit operation size
			reader.ReadByte();
		}
	}

	/// <summary>
	/// Return from a single-width (32-bit) non-object value-returning method.
	/// </summary>
	public class ReturnValueOpCode : OpCode
	{
		// return value register-pair (8 bits)
		internal byte Value;

		/// <summary>
		/// Constructor for all return-* operations
		/// </summary>
		/// <param name="reader">Reader.</param>
		/// <param name="name">Name.</param>
		/// <param name="insn">Insn.</param>
		protected ReturnValueOpCode (BinaryReader reader, string name, Instructions insn) : base (name, insn)
		{
			Value = reader.ReadByte();
		}
		
		internal ReturnValueOpCode (BinaryReader reader) : this (reader, "return", Instructions.ReturnValue) {}

		public override string ToString ()
		{
			return string.Format("{0} v{1}", Name, Value);
		}
	}

	/// <summary>
	/// Return from a double-width (64-bit) value-returning method.
	/// </summary>
	public class ReturnWideOpCode : ReturnValueOpCode
	{
		internal ReturnWideOpCode (BinaryReader reader) : base (reader, "return-wide", Instructions.ReturnWide) {}
	}

	/// <summary>
	/// Return from an object-returning method.
	/// </summary>
	public class ReturnObjectOpCode : ReturnValueOpCode
	{
		internal ReturnObjectOpCode (BinaryReader reader) : base (reader, "return-object", Instructions.ReturnObject) {}
	}

	/// <summary>
	/// Parent class for const operations.
	/// Move the value <V> into the destination register <D>
	/// </summary>
	public abstract class AbsConstOpCode<D,V> : OpCode
	{
		public D Destination { get; protected set; }
		public V Value { get; protected set; }

		public AbsConstOpCode(string name, Instructions insn) : base (name, insn) {}

		public override string ToString ()
		{
			return string.Format("{0} v{1}, #{2}", Name, Destination, Value);
		}
	}

	/// <summary>
	/// Move the given literal value (sign-extended to 32 bits) into the specified register.
	/// </summary>
	public class Const4OpCode : AbsConstOpCode<byte,sbyte>
	{
		internal Const4OpCode (BinaryReader reader) : base ("const/4", Instructions.Const4)
		{
			byte value;
			byte dest;
			SplitByte(reader.ReadByte(), out value, out dest);
			Value = (sbyte)value;
			Destination = dest;
		}
	}

	/// <summary>
	/// Move the given literal value (sign-extended to 32 bits) into the specified register.
	/// </summary>
	public class Const16OpCode : AbsConstOpCode<ushort,short>
	{
		internal Const16OpCode (BinaryReader reader) : base ("const/16", Instructions.Const16)
		{
			Destination = reader.ReadByte();
			Value = reader.ReadInt16();
		}
	}

	/// <summary>
	/// Move the given literal value into the specified register.
	/// </summary>
	public class ConstOpCode : AbsConstOpCode<ushort,int>
	{
		internal ConstOpCode (BinaryReader reader) : base ("const", Instructions.Const)
		{
			Destination = reader.ReadByte();
			Value = reader.ReadInt32();
		}
	}

	/// <summary>
	/// Move the given literal value (right-zero-extended to 32 bits) into the specified register.
	/// </summary>
	public class ConstHighOpCode : AbsConstOpCode<ushort,int>
	{
		internal ConstHighOpCode (BinaryReader reader) : base ("const/high16", Instructions.ConstHigh)
		{
			Destination = reader.ReadByte();
			Value = (reader.ReadUInt16()) << 16;
		}
	}

	/// <summary>
	/// Move the given literal value (sign-extended to 64 bits) into the specified register-pair.
	/// </summary>
	public class ConstWide16OpCode : AbsConstOpCode<ushort,long>
	{
		internal ConstWide16OpCode (BinaryReader reader) : base ("const-wide/16", Instructions.ConstWide16)
		{
			Destination = reader.ReadByte();
			Value = reader.ReadInt16 ();
		}
	}

	/// <summary>
	/// Move the given literal value (sign-extended to 64 bits) into the specified register-pair.
	/// </summary>
	public class ConstWide32OpCode : AbsConstOpCode<ushort,long>
	{
		internal ConstWide32OpCode (BinaryReader reader) : base ("const-wide/32", Instructions.ConstWide32)
		{
			Destination = reader.ReadByte();
			Value = reader.ReadInt32 ();
		}
	}

	/// <summary>
	/// Move the given literal value into the specified register-pair.
	/// </summary>
	public class ConstWideOpCode : AbsConstOpCode<ushort,long>
	{
		internal ConstWideOpCode (BinaryReader reader) : base ("const-wide", Instructions.ConstWide)
		{
			Destination = reader.ReadByte();
			Value = reader.ReadInt64 ();
		}
	}

	/// <summary>
	/// Move the given literal value (right-zero-extended to 64 bits) into the specified register-pair.
	/// </summary>
	public class ConstWideHighOpCode : AbsConstOpCode<ushort,long>
	{
		internal ConstWideHighOpCode (BinaryReader reader) : base ("const-wide/high16", Instructions.ConstWideHigh)
		{
			Destination = reader.ReadByte();
			Value = (reader.ReadUInt16()) << 48;
		}
	}

	/// <summary>
	/// Parent class for const-string operations
	/// </summary>
	public class AbsConstStringOpCode<T> : OpCode
	{
		internal ushort Destination;
		internal T StringIndex;

		public AbsConstStringOpCode(string name, Instructions insn) : base (name, insn) {}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}, string@{2}", Name, Destination, StringIndex);
		}
	}

	/// <summary>
	/// Move a reference to the string specified by the given index into the specified register.
	/// </summary>
	public class ConstStringOpCode : AbsConstStringOpCode<ushort>
	{
		internal ConstStringOpCode (BinaryReader reader) : base("const-string", Instructions.ConstString)
		{
			Destination = reader.ReadByte();
			StringIndex = reader.ReadUInt16();
		}
	}

	/// <summary>
	/// Move a reference to the string specified by the given index into the specified register.
	/// </summary>
	public class ConstStringJumboOpCode : AbsConstStringOpCode<uint>
	{
		internal ConstStringJumboOpCode (BinaryReader reader) : base("const-string/jumbo", Instructions.ConstStringJumbo)
		{
			Destination = reader.ReadByte();
			StringIndex = reader.ReadUInt32 ();
		}
	}

	/// <summary>
	/// Move a reference to the class specified by the given index into the 
	/// specified register. In the case where the indicated type is primitive,
	/// this will store a reference to the primitive type's degenerate class.
	/// </summary>
	public class ConstClassOpCode : Register8OpCode
	{
		internal ushort TypeIndex;

		internal ConstClassOpCode (BinaryReader reader) : base (reader, "const-class", Instructions.ConstClass)
		{
			TypeIndex = reader.ReadUInt16();
		}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}, type@{2}", Name, Destination, TypeIndex);
		}
	}

	/// <summary>
	/// Acquire the monitor for the indicated object.
	/// </summary>
	public class MonitorEnterOpCode : Register8OpCode
	{
		internal MonitorEnterOpCode (BinaryReader reader) : base(reader, "monitor-enter", Instructions.MonitorEnter) {}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}", Name, Destination);
		}
	}

	/// <summary>
	/// Release the monitor for the indicated object.
	/// </summary>
	public class MonitorExitOpCode : Register8OpCode
	{
		internal MonitorExitOpCode (BinaryReader reader) : base(reader, "monitor-exit", Instructions.MonitorExit) {}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}", Name, Destination);
		}
	}

	/// <summary>
	/// Throw a ClassCastException if the reference in the given 
	/// register cannot be cast to the indicated type.
	/// </summary>
	public class CheckCastOpCode : Register8OpCode
	{
		public ushort TypeIndex;

		internal CheckCastOpCode (BinaryReader reader) : base (reader, "check-cast", Instructions.CheckCast)
		{
			TypeIndex = reader.ReadUInt16();
		}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}, type@{2}", Name, Destination, TypeIndex);
		}
	}

	/// <summary>
	/// Store in the given destination register 1 if the indicated 
	/// reference is an instance of the given type, or 0 if not.
	/// </summary>
	public class InstanceOfOpCode : OpCode
	{
		internal byte Destination;
		internal byte Reference;
		internal ushort TypeIndex;

		internal InstanceOfOpCode (BinaryReader reader) : base ("instance-of", Instructions.InstanceOf)
		{
			SplitByte (reader.ReadByte (), out Destination, out Reference);
			TypeIndex = reader.ReadUInt16();
		}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}, v{2}, type@{3}", Name, Destination, Reference, TypeIndex);
		}
	}

	/// <summary>
	/// Store in the given destination register the length of the indicated array, in entries
	/// </summary>
	public class ArrayLengthOpCode : OpCode
	{
		internal byte Destination;
		internal byte ArrayReference;

		internal ArrayLengthOpCode (BinaryReader reader) : base ("array-length", Instructions.ArrayLength)
		{
			SplitByte (reader.ReadByte (), out ArrayReference, out Destination);
		}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}, v{2}", Name, Destination, ArrayReference);
		}
	}

	/// <summary>
	/// Construct a new instance of the indicated type, storing a reference 
	/// to it in the destination. The type must refer to a non-array class.
	/// </summary>
	public class NewInstanceOpCode : Register8OpCode
	{
		public ushort TypeIndex;
		
		internal NewInstanceOpCode (BinaryReader reader) : base(reader, "new-instance", Instructions.NewInstance)
		{
			TypeIndex = reader.ReadUInt16();
		}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}, type@{2}", Name, Destination, TypeIndex);
		}
	}

	/// <summary>
	/// Construct a new array of the indicated type and size. The type must be an array type.
	/// </summary>
	public class NewArrayOfOpCode : OpCode
	{
		internal byte Destination;
		internal byte Size;
		internal ushort TypeIndex;
		
		internal NewArrayOfOpCode (BinaryReader reader) : base ("new-array", Instructions.NewArrayOf)
		{
			SplitByte (reader.ReadByte (), out Size, out Destination);
			TypeIndex = reader.ReadUInt16();
		}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}, v{2}, type@{3}", Name, Destination, Size, TypeIndex);
		}
	}

	/// <summary>
	/// Construct an array of the given type and size, filling it with the supplied contents. 
	/// The type must be an array type. The array's contents must be single-word (that is, 
	/// no arrays of long or double, but reference types are acceptable). The constructed 
	/// instance is stored as a "result" in the same way that the method invocation 
	/// instructions store their results, so the constructed instance must be moved to a 
	/// register with an immediately subsequent move-result-object instruction (if it is to be used).
	/// </summary>
	public class FilledNewArrayOfOpCode : OpCode
	{
		internal byte[] Values;
		internal ushort TypeIndex;
		
		internal FilledNewArrayOfOpCode (BinaryReader reader) : base ("filled-new-array", Instructions.FilledNewArrayOf)
		{
			var data = reader.ReadByte ();

			var size = ((data & 0xf0) >> 4);
			Values = new byte[size];

			TypeIndex = reader.ReadUInt16();

			// Target registers in the next 16 bits
			var byte2 = reader.ReadByte ();
			var byte3 = reader.ReadByte ();

			if (size > 0) {
				Values[0] = (byte)(byte2 & 0x0f);
			}

			if (size > 1) {
				Values[1] = (byte)((byte2 & 0xf0) >> 4);
			}

			if (size > 2) {
				Values[2] = (byte)(byte3 & 0x0f);
			}

			if (size > 3) {
				Values[3] = (byte)((byte3 & 0xf0) >> 4);
			}

			// Last register is encoded in the first byte
			if (size == 5) {
				Values[4] = (byte)(data & 0x0f);
			}
		}
		
		public override string ToString ()
		{
			return string.Format("{0} {{v{1}}}, type@{2}", Name, string.Join(", v", Values), TypeIndex);
		}
	}

	/// <summary>
	/// Construct an array of the given type and size, filling it with the supplied contents. 
	/// Clarifications and restrictions are the same as filled-new-array.
	/// </summary>
	public class FilledNewArrayRangeOpCode : OpCode
	{
		internal ushort TypeIndex;
		internal ushort[] Values;
		
		internal FilledNewArrayRangeOpCode (BinaryReader reader) : base ("filled-new-array/range", Instructions.FilledNewArrayRange)
		{
			var size = reader.ReadByte ();
			TypeIndex = reader.ReadUInt16 ();

			Values = new ushort[size];

			for (int i=0; i<size; i++) {
				Values[i] = reader.ReadUInt16();
			}
		}
		
		public override string ToString ()
		{
			return string.Format("{0} {{v{1}}}, type@{2}", string.Join(", v", Values), TypeIndex);
		}
	}

	/// <summary>
	/// Fill the given array with the indicated data. The reference must be to an array of primitives, 
	/// and the data table must match it in type and must contain no more elements than will fit in 
	/// the array. That is, the array may be larger than the table, and if so, only the initial 
	/// elements of the array are set, leaving the remainder alone.
	/// </summary>
	public class FillArrayDataOpCode : Register8OpCode, DataExtendedOpCode
	{
		internal ushort TypeIndex;
		internal ulong[] Values;
		protected ushort ValueWidth;

		public long DataTableOffset { get; private set; }

		internal FillArrayDataOpCode (BinaryReader reader) : base(reader, "fill-array-data", Instructions.FillArrayData)
		{
			var dataOffset = reader.ReadInt32() * 2;
			var streamPosition = reader.BaseStream.Position;

			DataTableOffset = streamPosition + dataOffset - 6;
			reader.BaseStream.Position = DataTableOffset;
			ParsePayload(reader);

			reader.BaseStream.Position = streamPosition;
		}

		/// <summary>
		/// Read data to fill the array
		/// </summary>
		/// <param name="reader">Reader.</param>
		private void ParsePayload (BinaryReader reader)
		{
			var id = reader.ReadUInt16();
			if (id != 0x0300)
				throw new ArgumentException (string.Format("Invalid fill-array-data-payload identifier. Got {0:X}", id));

			ValueWidth = reader.ReadUInt16();

			var size = reader.ReadUInt32();
			Values = new ulong[size];

			for (int i=0; i<size; i++) {
				Values[i] = ReadValue(reader, ValueWidth);
			}
		}

		/// <summary>
		/// Reads a single value of the payload
		/// </summary>
		/// <returns>The value.</returns>
		/// <param name="reader">Reader.</param>
		/// <param name="valueWidth">Value width.</param>
		private ulong ReadValue (BinaryReader reader, int valueWidth)
		{
			ulong value = 0;

			for (int i=0; i<valueWidth; i++) {
				value |= ((ulong)reader.ReadByte()) << (i*8);
			}

			return value;
		}

		public override string ToString ()
		{
			return string.Format("{0} v{1}, [{2}]", Name, Destination, string.Join(",", Values));
		}
	}

	/// <summary>
	/// Throw the indicated exception.
	/// </summary>
	public class ThrowOpCode : Register8OpCode
	{
		internal ThrowOpCode (BinaryReader reader) : base(reader, "throw", Instructions.Throw) {}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}", Name, Destination);
		}
	}

	public interface IGoto : JumpOpCode
	{
	}

	/// <summary>
	/// Parent class for goto operations
	/// </summary>
	public abstract class AbsGoto<T> : OpCode, IGoto where T : IComparable
	{
		internal T Offset;

		protected AbsGoto (string name, Instructions insn) : base (name, insn) {}

		public long GetTargetAddress()
		{
			return OpCodeOffset + (long)Convert.ChangeType(Offset, typeof(long)) * 2;
		}

		public override string ToString ()
		{
			return string.Format("{0} {1}", Name, Offset);
		}
	}

	/// <summary>
	/// Unconditionally jump to the indicated instruction.
	/// </summary>
	public class GotoOpCode : AbsGoto<sbyte>
	{
		internal GotoOpCode (BinaryReader reader) : base("goto", Instructions.Goto)
		{
			Offset = reader.ReadSByte();
		}
	}

	/// <summary>
	/// Unconditionally jump to the indicated instruction.
	/// </summary>
	public class Goto16OpCode : AbsGoto<short>
	{
		internal Goto16OpCode (BinaryReader reader) : base("goto/16", Instructions.Goto16)
		{
			Offset = reader.ReadInt16();
			reader.ReadByte(); // skip alignment byte
		}
	}

	/// <summary>
	/// Unconditionally jump to the indicated instruction.
	/// Note: The branch offset must not be 0. (A spin loop may be legally 
	/// constructed either with goto/32 or by including a nop as a target before the branch.)
	/// </summary>
	public class Goto32OpCode : AbsGoto<int>
	{
		internal Goto32OpCode (BinaryReader reader) : base("goto/32", Instructions.Goto16)
		{
			Offset = reader.ReadInt32();
			reader.ReadByte(); // skip alignment byte
		}
	}

	/// <summary>
	/// Parent of both switch opcodes
	/// </summary>
	public abstract class SwitchOpCode : Register8OpCode, DataExtendedOpCode
	{
		// relative branch targets
		internal int[] Targets;
		public long DataTableOffset { get; protected set; }

		internal SwitchOpCode (BinaryReader reader, string name, Instructions isns) : base(reader, name, isns) 
		{
		}

		public IEnumerable<long> GetTargetAddresses()
		{
			foreach (var target in Targets) {
				yield return OpCodeOffset + target*2;
			}
		}

		public long GetTargetAddress(int index)
		{
			return OpCodeOffset + Targets[index]*2;
		}

		public abstract int[] GetKeys ();
	}

	/// <summary>
	/// Jump to a new instruction based on the value in the given register, 
	/// using a table of offsets corresponding to each value in a particular 
	/// integral range, or fall through to the next instruction if there is no match.
	/// </summary>
	public class PackedSwitchOpCode : SwitchOpCode
	{
		internal int FirstKey;

		internal PackedSwitchOpCode (BinaryReader reader) : base(reader, "packed-switch", Instructions.PackedSwitch)
		{
			var tableOffset = reader.ReadInt32() * 2;
			var streamPosition = reader.BaseStream.Position;

			DataTableOffset = streamPosition + tableOffset - 6;
			reader.BaseStream.Position = DataTableOffset;
			ParsePayload(reader);
			
			reader.BaseStream.Position = streamPosition;
		}

		private void ParsePayload (BinaryReader reader)
		{
			var id = reader.ReadUInt16 ();
			if (id != 0x0100)
				throw new ArgumentException (string.Format("Invalid packed-switch-payload identifier. Got {0:X}", id));
			
			// The total number of code units for an instance of this table is (size * 2) + 2
			var size = reader.ReadUInt16 ();
			Targets = new int[size];

			FirstKey = reader.ReadInt32();

			for (int i=0; i<size; i++) {
				Targets[i] = reader.ReadInt32 ();
			}
		}

		public override int[] GetKeys () 
		{
			var keys = new int[Targets.Length];
			keys [0] = FirstKey;

			for (int i=1; i<Targets.Length; i++) {
				keys [i] = FirstKey + i;
			}

			return keys;
		}

		public override string ToString ()
		{
			return string.Format("{0} v{1} - First:{2} - [{3}]", Name, Destination, FirstKey, string.Join(",", Targets));
		}
	}

	/// <summary>
	/// Jump to a new instruction based on the value in the given register, 
	/// using an ordered table of value-offset pairs, or fall through to the 
	/// next instruction if there is no match.
	/// </summary>
	public class SparseSwitchOpCode : SwitchOpCode
	{
		// keys, sorted low-to-high
		internal int[] Keys;

		internal SparseSwitchOpCode (BinaryReader reader) : base(reader, "sparse-switch", Instructions.SparseSwitch)
		{
			var tableOffset = reader.ReadInt32() * 2; 
			var streamPosition = reader.BaseStream.Position;

			DataTableOffset = streamPosition + tableOffset - 6;
			reader.BaseStream.Position = DataTableOffset;
			ParsePayload(reader);
			
			reader.BaseStream.Position = streamPosition;
		}
		
		private void ParsePayload (BinaryReader reader)
		{
			var id = reader.ReadUInt16 ();
			if (id != 0x0200)
				throw new ArgumentException (string.Format("Invalid sparse-switch-payload identifier. Got {0:X}", id));

			// The total number of code units for an instance of this table is (size * 4) + 2
			var size = reader.ReadUInt16();
			Keys = new int[size];
			Targets = new int[size];
			
			for (int i=0; i<size; i++) {
				Keys[i] = reader.ReadInt32 ();
			}

			for (int i=0; i<size; i++) {
				Targets[i] = reader.ReadInt32 ();
			}
		}
		
		public override int[] GetKeys () 
		{
			return Keys;
		}

		public override string ToString ()
		{
			return string.Format("{0} v{1} - Keys:[{2}] Targets:[{3}]", Name, Destination, 
			                     string.Join(",",Keys), string.Join(",", Targets));
		}
	}

	/// <summary>
	/// Parent class for Compare operations
	/// 
	/// Perform the indicated floating point or long comparison, setting a to 0 if b == c, 
	/// 1 if b > c, or -1 if b < c. The "bias" listed for the floating point operations 
	/// indicates how NaN comparisons are treated: "gt bias" instructions return 1 for 
	/// NaN comparisons, and "lt bias" instructions return -1.
	/// For example, to check to see if floating point x < y it is advisable to use 
	/// cmpg-float; a result of -1 indicates that the test was true, and the other 
	/// values indicate it was false either due to a valid comparison or because one of 
	/// the values was NaN.
	/// </summary>
	public abstract class CmplOpCode : Register8OpCode
	{
		internal byte First;
		internal byte Second;

		internal CmplOpCode (BinaryReader reader, string name, Instructions insn) : base(reader, name, insn)
		{
			First = reader.ReadByte();
			Second = reader.ReadByte();
		}

		public override string ToString ()
		{
			return string.Format("{0} v{1}, v{2}, v{3}", Name, Destination, First, Second);
		}
	}

	public class CmplFloatOpCode : CmplOpCode
	{
		// (lt bias)
		internal CmplFloatOpCode (BinaryReader reader) : base(reader, "cmpl-float", Instructions.CmplFloat) {}
	}

	public class CmpgFloatOpCode : CmplOpCode
	{
		// (gt bias)
		internal CmpgFloatOpCode (BinaryReader reader) : base(reader, "cmpg-float", Instructions.CmpgFloat) {}
	}

	public class CmplDoubleOpCode : CmplOpCode
	{
		// (lt bias)
		internal CmplDoubleOpCode (BinaryReader reader) : base(reader, "cmpl-double", Instructions.CmplDouble) {}
	}

	public class CmpgDoubleOpCode : CmplOpCode
	{
		// (gt bias)
		internal CmpgDoubleOpCode (BinaryReader reader) : base(reader, "cmpg-double", Instructions.CmpgDouble) {}
	}

	public class CmpLongOpCode : CmplOpCode
	{
		internal CmpLongOpCode (BinaryReader reader) : base(reader, "cmp-long", Instructions.CmpLong) {}
	}

	/// <summary>
	/// Marker interface for IF operations
	/// </summary>
	public interface IIfOpCode : JumpOpCode
	{
	}

	/// <summary>
	/// Branch to the given destination if the given two registers' values compare as specified.
	/// Note: The branch offset must not be 0. (A spin loop may be legally constructed either 
	/// by branching around a backward goto or by including a nop as a target before the branch.)
	/// </summary>
	public abstract class IfOpCode : OpCode, IIfOpCode
	{
		internal byte First;
		internal byte Second;
		internal short Offset;

		internal IfOpCode (BinaryReader reader, string name, Instructions insn) : base (name, insn)
		{
			SplitByte (reader.ReadByte (), out Second, out First);
			Offset = reader.ReadInt16();
		}

		public long GetTargetAddress()
		{
			return OpCodeOffset + Offset*2;
		}

		public override string ToString ()
		{
			return string.Format("{0} v{1}, v{2}, {3}", Name, First, Second, Offset);
		}
	}

	public class IfEqOpCode : IfOpCode
	{
		internal IfEqOpCode (BinaryReader reader) : base(reader, "if-eq", Instructions.IfEq) {}
	}

	public class IfNeOpCode : IfOpCode
	{
		internal IfNeOpCode (BinaryReader reader) : base(reader, "if-ne", Instructions.IfNe) {}
	}

	public class IfLtOpCode : IfOpCode
	{
		internal IfLtOpCode (BinaryReader reader) : base(reader, "if-lt", Instructions.IfLt) {}
	}

	public class IfGeOpCode : IfOpCode
	{
		internal IfGeOpCode (BinaryReader reader) : base(reader, "if-ge", Instructions.IfGe) {}
	}

	public class IfGtOpCode : IfOpCode
	{
		internal IfGtOpCode (BinaryReader reader) : base(reader, "if-gt", Instructions.IfGt) {}
	}

	public class IfLeOpCode : IfOpCode
	{
		internal IfLeOpCode (BinaryReader reader) : base(reader, "if-le", Instructions.IfLe) {}
	}

	/// <summary>
	/// Branch to the given destination if the given register's value compares with 0 as specified.
	/// Note: The branch offset must not be 0. (A spin loop may be legally constructed either by 
	/// branching around a backward goto or by including a nop as a target before the branch.)
	/// </summary>
	public abstract class IfzOpCode : Register8OpCode, IIfOpCode
	{
		internal short Offset;

		internal IfzOpCode (BinaryReader reader, string name, Instructions insn) : base(reader, name, insn)
		{
			Offset = reader.ReadInt16();
		}
		
		public long GetTargetAddress()
		{
			return OpCodeOffset + Offset*2;
		}

		public override string ToString ()
		{
			return string.Format("{0} v{1}, {2}", Name, Destination, Offset);
		}
	}

	public class IfEqzOpCode : IfzOpCode
	{
		internal IfEqzOpCode (BinaryReader reader) : base(reader, "if-eqz", Instructions.IfEqz) {}
	}

	public class IfNezOpCode : IfzOpCode
	{
		internal IfNezOpCode (BinaryReader reader) : base(reader, "if-nez", Instructions.IfNez) {}
	}

	public class IfLtzOpCode : IfzOpCode
	{
		internal IfLtzOpCode (BinaryReader reader) : base(reader, "if-ltz", Instructions.IfLtz) {}
	}

	public class IfGezOpCode : IfzOpCode
	{
		internal IfGezOpCode (BinaryReader reader) : base(reader, "if-gez", Instructions.IfGez) {}
	}

	public class IfGtzOpCode : IfzOpCode
	{
		internal IfGtzOpCode (BinaryReader reader) : base(reader, "if-gtz", Instructions.IfGtz) {}
	}

	public class IfLezOpCode : IfzOpCode
	{
		internal IfLezOpCode (BinaryReader reader) : base(reader, "if-lez", Instructions.IfLez) {}
	}

	/// <summary>
	/// Perform the identified array operation at the identified index of the given array, 
	/// loading or storing into the value register.
	/// </summary>
	public abstract class ArrayOpOpCode : Register8OpCode
	{
		public byte Array;
		public byte Index;

		internal ArrayOpOpCode (BinaryReader reader, string name, Instructions insn) : base(reader, name, insn)
		{
			Array = reader.ReadByte();
			Index = reader.ReadByte();
		}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}, v{2}, v{3}", Name, Destination, Array, Index);
		}
	}

	public class AgetOpCode : ArrayOpOpCode
	{
		internal AgetOpCode (BinaryReader reader) : base(reader, "aget", Instructions.Aget) {}
	}

	public class AgetWideOpCode : ArrayOpOpCode
	{
		internal AgetWideOpCode (BinaryReader reader) : base(reader, "aget-wide", Instructions.AgetWide) {}
	}

	public class AgetObjectOpCode : ArrayOpOpCode
	{
		internal AgetObjectOpCode (BinaryReader reader) : base(reader, "aget-object", Instructions.AgetObject) {}
	}

	public class AgetBooleanOpCode : ArrayOpOpCode
	{
		internal AgetBooleanOpCode (BinaryReader reader) : base(reader, "aget-boolean", Instructions.AgetBoolean) {}
	}

	public class AgetByteOpCode : ArrayOpOpCode
	{
		internal AgetByteOpCode (BinaryReader reader) : base(reader, "aget-byte", Instructions.AgetByte) {}
	}

	public class AgetCharOpCode : ArrayOpOpCode
	{
		internal AgetCharOpCode (BinaryReader reader) : base(reader, "aget-char", Instructions.AgetChar) {}
	}
	
	public class AgetShortOpCode : ArrayOpOpCode
	{
		internal AgetShortOpCode (BinaryReader reader) : base(reader, "aget-short", Instructions.AgetShort) {}
	}

	public class AputOpCode : ArrayOpOpCode
	{
		internal AputOpCode (BinaryReader reader) : base(reader, "aput", Instructions.Aput) {}
	}
	
	public class AputWideOpCode : ArrayOpOpCode
	{
		internal AputWideOpCode (BinaryReader reader) : base(reader, "aput-wide", Instructions.AputWide) {}
	}
	
	public class AputObjectOpCode : ArrayOpOpCode
	{
		internal AputObjectOpCode (BinaryReader reader) : base(reader, "aput-object", Instructions.AputObject) {}
	}
	
	public class AputBooleanOpCode : ArrayOpOpCode
	{
		internal AputBooleanOpCode (BinaryReader reader) : base(reader, "aput-boolean", Instructions.AputBoolean) {}
	}
	
	public class AputByteOpCode : ArrayOpOpCode
	{
		internal AputByteOpCode (BinaryReader reader) : base(reader, "aput-byte", Instructions.AputByte) {}
	}
	
	public class AputCharOpCode : ArrayOpOpCode
	{
		internal AputCharOpCode (BinaryReader reader) : base(reader, "aput-char", Instructions.AputChar) {}
	}
	
	public class AputShortOpCode : ArrayOpOpCode
	{
		internal AputShortOpCode (BinaryReader reader) : base(reader, "aput-short", Instructions.AputShort) {}
	}

	/// <summary>
	/// Perform the identified object instance field operation with the identified field, loading 
	/// or storing into the value register.
	/// Note: These opcodes are reasonable candidates for static linking, altering the field 
	/// argument to be a more direct offset.
	/// </summary>
	public abstract class IinstanceOpOpCode : OpCode
	{
		internal byte Destination;
		internal byte Object;
		internal ushort Index;

		internal IinstanceOpOpCode (BinaryReader reader, string name, Instructions insn) : base (name, insn)
		{
			SplitByte(reader.ReadByte(), out Object, out Destination);
			Index = reader.ReadUInt16();
		}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}, v{2} field@{3}", Name, Destination, Object, Index);
		}
	}
	
	public class IgetOpCode : IinstanceOpOpCode
	{
		internal IgetOpCode (BinaryReader reader) : base(reader, "iget", Instructions.Iget) {}
	}

	public class  IgetWideOpCode : IinstanceOpOpCode
	{
		internal  IgetWideOpCode (BinaryReader reader) : base(reader, "iget-wide", Instructions.IgetWide) {}
	}
	
	public class IgetObjectOpCode : IinstanceOpOpCode
	{
		internal IgetObjectOpCode (BinaryReader reader) : base(reader, "iget-object", Instructions.IgetObject) {}
	}
	
	public class IgetBooleanOpCode : IinstanceOpOpCode
	{
		internal IgetBooleanOpCode (BinaryReader reader) : base(reader, "iget-boolean", Instructions.IgetBoolean) {}
	}
	
	public class IgetByteOpCode : IinstanceOpOpCode
	{
		internal IgetByteOpCode (BinaryReader reader) : base(reader, "iget-byte", Instructions.IgetByte) {}
	}
	
	public class IgetCharOpCode : IinstanceOpOpCode
	{
		internal IgetCharOpCode (BinaryReader reader) : base(reader, "iget-char", Instructions.IgetChar) {}
	}
	
	public class IgetShortOpCode : IinstanceOpOpCode
	{
		internal IgetShortOpCode (BinaryReader reader) : base(reader, "iget-short", Instructions.IgetShort) {}
	}
	
	public class IputOpCode : IinstanceOpOpCode
	{
		internal IputOpCode (BinaryReader reader) : base(reader, "iput", Instructions.Iput) {}
	}
	
	public class IputWideOpCode : IinstanceOpOpCode
	{
		internal IputWideOpCode (BinaryReader reader) : base(reader, "iput-wide", Instructions.IputWide) {}
	}
	
	public class IputObjectOpCode : IinstanceOpOpCode
	{
		internal IputObjectOpCode (BinaryReader reader) : base(reader, "iput-object", Instructions.IputObject) {}
	}
	
	public class IputBooleanOpCode : IinstanceOpOpCode
	{
		internal IputBooleanOpCode (BinaryReader reader) : base(reader, "iput-boolean", Instructions.IputBoolean) {}
	}
	
	public class IputByteOpCode : IinstanceOpOpCode
	{
		internal IputByteOpCode (BinaryReader reader) : base(reader, "iput-byte", Instructions.IputByte) {}
	}
	
	public class IputCharOpCode : IinstanceOpOpCode
	{
		internal IputCharOpCode (BinaryReader reader) : base(reader, "iput-char", Instructions.IputChar) {}
	}
	
	public class IputShortOpCode : IinstanceOpOpCode
	{
		internal IputShortOpCode (BinaryReader reader) : base(reader, "iput-short", Instructions.IputShort) {}
	}

	/// <summary>
	/// Perform the identified object static field operation with the identified 
	/// static field, loading or storing into the value register.
	/// </summary>
	public abstract class StaticOpOpCode : Register8OpCode
	{
		internal ushort Index;
		
		internal StaticOpOpCode (BinaryReader reader, string name, Instructions insn) : base (reader, name, insn)
		{
			Index = reader.ReadUInt16();
		}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}, field@{2}", Name, Destination, Index);
		}
	}
	
	public class SgetOpCode : StaticOpOpCode
	{
		internal SgetOpCode (BinaryReader reader) : base(reader, "sget", Instructions.Sget) {}
	}
	
	public class  SgetWideOpCode : StaticOpOpCode
	{
		internal  SgetWideOpCode (BinaryReader reader) : base(reader, "sget-wide", Instructions.SgetWide) {}
	}
	
	public class SgetObjectOpCode : StaticOpOpCode
	{
		internal SgetObjectOpCode (BinaryReader reader) : base(reader, "sget-object", Instructions.SgetObject) {}
	}
	
	public class SgetBooleanOpCode : StaticOpOpCode
	{
		internal SgetBooleanOpCode (BinaryReader reader) : base(reader, "sget-boolean", Instructions.SgetBoolean) {}
	}
	
	public class SgetByteOpCode : StaticOpOpCode
	{
		internal SgetByteOpCode (BinaryReader reader) : base(reader, "sget-byte", Instructions.SgetByte) {}
	}
	
	public class SgetCharOpCode : StaticOpOpCode
	{
		internal SgetCharOpCode (BinaryReader reader) : base(reader, "sget-char", Instructions.SgetChar) {}
	}
	
	public class SgetShortOpCode : StaticOpOpCode
	{
		internal SgetShortOpCode (BinaryReader reader) : base(reader, "sget-short", Instructions.SgetShort) {}
	}
	
	public class SputOpCode : StaticOpOpCode
	{
		internal SputOpCode (BinaryReader reader) : base(reader, "sput", Instructions.Sput) {}
	}
	
	public class SputWideOpCode : StaticOpOpCode
	{
		internal SputWideOpCode (BinaryReader reader) : base(reader, "sput-wide", Instructions.SputWide) {}
	}
	
	public class SputObjectOpCode : StaticOpOpCode
	{
		internal SputObjectOpCode (BinaryReader reader) : base(reader, "sput-object", Instructions.SputObject) {}
	}
	
	public class SputBooleanOpCode : StaticOpOpCode
	{
		internal SputBooleanOpCode (BinaryReader reader) : base(reader, "sput-boolean", Instructions.SputBoolean) {}
	}
	
	public class SputByteOpCode : StaticOpOpCode
	{
		internal SputByteOpCode (BinaryReader reader) : base(reader, "sput-byte", Instructions.SputByte) {}
	}
	
	public class SputCharOpCode : StaticOpOpCode
	{
		internal SputCharOpCode (BinaryReader reader) : base(reader, "sput-char", Instructions.SputChar) {}
	}
	
	public class SputShortOpCode : StaticOpOpCode
	{
		internal SputShortOpCode (BinaryReader reader) : base(reader, "sput-short", Instructions.SputShort) {}
	}

	/// <summary>
	/// Call the indicated method. The result (if any) may be stored with an appropriate move-result* 
	/// variant as the immediately subsequent instruction.
	/// </summary>
	public abstract class InvokeOpCode : OpCode
	{
		public byte[] ArgumentRegisters;
		public ushort MethodIndex;

		internal InvokeOpCode (BinaryReader reader, string name, Instructions insn) : base (name, insn)
		{
			var byte1 = reader.ReadByte ();
			MethodIndex = reader.ReadUInt16 ();

			var argumentCount = ((byte1 & 0xf0) >> 4);
			ArgumentRegisters = new byte[argumentCount];

			// Target registers in the next 16 bits
			var byte2 = reader.ReadByte ();
			var byte3 = reader.ReadByte ();

			if (argumentCount > 0) {
				ArgumentRegisters[0] = (byte)(byte2 & 0x0f);
			}
			
			if (argumentCount > 1) {
				ArgumentRegisters[1] = (byte)((byte2 & 0xf0) >> 4);
			}
			
			if (argumentCount > 2) {
				ArgumentRegisters[2] = (byte)(byte3 & 0x0f);
			}

			if (argumentCount > 3) {
				ArgumentRegisters[3] = (byte)((byte3 & 0xf0) >> 4);
			}

			// Last register is encoded in the first byte
			if (argumentCount == 5) {
				ArgumentRegisters[4] = (byte)(byte1 & 0x0f);
			}
		}
		
		public override string ToString ()
		{
			if (ArgumentRegisters.Length == 0) {
				return string.Format("{0} {{}}, meth@{1}", Name, MethodIndex);
			}

			return string.Format("{0} {{v{1}}}, meth@{2}", Name, string.Join(", v", ArgumentRegisters), MethodIndex);
		}
	}

	/// <summary>
	/// invoke-virtual is used to invoke a normal virtual method 
	/// (a method that is not private, static, or final, and is also not a constructor).
	/// </summary>
	public class InvokeVirtualOpCode : InvokeOpCode
	{
		internal InvokeVirtualOpCode (BinaryReader reader) : base(reader, "invoke-virtual", Instructions.InvokeVirtual) {}
	}

	/// <summary>
	/// invoke-super is used to invoke the closest superclass's virtual method 
	/// (as opposed to the one with the same method_id in the calling class). 
	/// The same method restrictions hold as for invoke-virtual.
	/// </summary>
	public class InvokeSuperOpCode : InvokeOpCode
	{
		internal InvokeSuperOpCode (BinaryReader reader) : base(reader, "invoke-super", Instructions.InvokeSuper) {}
	}

	/// <summary>
	/// invoke-direct is used to invoke a non-static direct method (that is, an 
	/// instance method that is by its nature non-overridable, namely either a 
	/// private instance method or a constructor).
	/// </summary>
	public class InvokeDirectOpCode : InvokeOpCode
	{
		internal InvokeDirectOpCode (BinaryReader reader) : base(reader, "invoke-direct", Instructions.InvokeDirect) {}
	}

	/// <summary>
	/// invoke-static is used to invoke a static method (which is always considered a direct method).
	/// </summary>
	public class InvokeStaticOpCode : InvokeOpCode
	{
		internal InvokeStaticOpCode (BinaryReader reader) : base(reader, "invoke-static", Instructions.InvokeStatic) {}
	}

	/// <summary>
	/// invoke-interface is used to invoke an interface method, that is, on an object whose concrete 
	/// class isn't known, using a method_id that refers to an interface.
	/// </summary>
	public class InvokeInterfaceOpCode : InvokeOpCode
	{
		internal InvokeInterfaceOpCode (BinaryReader reader) : base(reader, "invoke-interface", Instructions.InvokeInterface) {}
	}

	/// <summary>
	/// Call the indicated method. See first invoke-kind description for details, caveats, and suggestions.
	/// </summary>
	public abstract class InvokeRangeOpCode : OpCode
	{
		internal byte ArgumentCount;
		internal ushort MethodIndex;
		internal ushort FirstArgument;

		internal InvokeRangeOpCode (BinaryReader reader, string name, Instructions insn) : base (name, insn)
		{
			ArgumentCount = reader.ReadByte ();
			MethodIndex = reader.ReadUInt16();
			FirstArgument = reader.ReadUInt16();
		}
		
		public override string ToString ()
		{
			return string.Format("{0} {{{1}..{2}}}, method@{3}", Name, FirstArgument, FirstArgument+ArgumentCount, MethodIndex);
		}
	}
	
	public class InvokeVirtualRangeOpCode : InvokeRangeOpCode
	{
		internal InvokeVirtualRangeOpCode (BinaryReader reader) : base(reader, "invoke-virtual/range", Instructions.InvokeVirtualRange) {}
	}
	
	public class InvokeSuperRangeOpCode : InvokeRangeOpCode
	{
		internal InvokeSuperRangeOpCode (BinaryReader reader) : base(reader, "invoke-super/range", Instructions.InvokeSuperRange) {}
	}
	
	public class InvokeDirectRangeOpCode : InvokeRangeOpCode
	{
		internal InvokeDirectRangeOpCode (BinaryReader reader) : base(reader, "invoke-direct/range", Instructions.InvokeDirectRange) {}
	}
	
	public class InvokeStaticRangeOpCode : InvokeRangeOpCode
	{
		internal InvokeStaticRangeOpCode (BinaryReader reader) : base(reader, "invoke-static/range", Instructions.InvokeStaticRange) {}
	}
	
	public class InvokeInterfaceRangeOpCode : InvokeRangeOpCode
	{
		internal InvokeInterfaceRangeOpCode (BinaryReader reader) : base(reader, "invoke-interface/range", Instructions.InvokeInterfaceRange) {}
	}

	/// <summary>
	/// Perform the identified unary operation on the source register, storing the result in the destination register.
	/// </summary>
	public abstract class UnaryOpOpCode : OpCode
	{
		internal byte Destination;
		internal byte Source;

		public UnaryOpOpCode (BinaryReader reader, string name, Instructions insn) : base (name, insn)
		{
			SplitByte (reader.ReadByte (), out Source, out Destination);
		}

		public override string ToString ()
		{
			return string.Format("{0} v{1}, v{2}", Name, Destination, Source);
		}
	}
	
	public class NegIntOpCode : UnaryOpOpCode
	{
		internal NegIntOpCode (BinaryReader reader) : base(reader, "neg-int", Instructions.NegInt) {}
	}
	
	public class NotIntOpCode : UnaryOpOpCode
	{
		internal NotIntOpCode (BinaryReader reader) : base(reader, "not-int", Instructions.NotInt) {}
	}
	
	public class NegLongOpCode : UnaryOpOpCode
	{
		internal NegLongOpCode (BinaryReader reader) : base(reader, "neg-long", Instructions.NegLong) {}
	}
	
	public class NotLongOpCode : UnaryOpOpCode
	{
		internal NotLongOpCode (BinaryReader reader) : base(reader, "not-long", Instructions.NotLong) {}
	}
	
	public class NegFloatOpCode : UnaryOpOpCode
	{
		internal NegFloatOpCode (BinaryReader reader) : base(reader, "neg-float", Instructions.NegFloat) {}
	}
	
	public class NegDoubleOpCode : UnaryOpOpCode
	{
		internal NegDoubleOpCode (BinaryReader reader) : base(reader, "neg-double", Instructions.NegDouble) {}
	}
	
	public class IntToLongOpCode : UnaryOpOpCode
	{
		internal IntToLongOpCode (BinaryReader reader) : base(reader, "int-to-long", Instructions.IntToLong) {}
	}
	
	public class IntToFloatOpCode : UnaryOpOpCode
	{
		internal IntToFloatOpCode (BinaryReader reader) : base(reader, "int-to-float", Instructions.IntToFloat) {}
	}
	
	public class IntToDoubleOpCode : UnaryOpOpCode
	{
		internal IntToDoubleOpCode (BinaryReader reader) : base(reader, "int-to-double", Instructions.IntToDouble) {}
	}
	
	public class LongToIntOpCode : UnaryOpOpCode
	{
		internal LongToIntOpCode (BinaryReader reader) : base(reader, "long-to-int", Instructions.LongToInt) {}
	}
	
	public class LongToFloatOpCode : UnaryOpOpCode
	{
		internal LongToFloatOpCode (BinaryReader reader) : base(reader, "long-to-float", Instructions.LongToFloat) {}
	}
	
	public class LongToDoubleOpCode : UnaryOpOpCode
	{
		internal LongToDoubleOpCode (BinaryReader reader) : base(reader, "long-to-double", Instructions.LongToDouble) {}
	}
	
	public class FloatToIntOpCode : UnaryOpOpCode
	{
		internal FloatToIntOpCode (BinaryReader reader) : base(reader, "float-to-int", Instructions.FloatToInt) {}
	}
	
	public class FloatToLongOpCode : UnaryOpOpCode
	{
		internal FloatToLongOpCode (BinaryReader reader) : base(reader, "float-to-long", Instructions.FloatToLong) {}
	}
	
	public class FloatToDoubleOpCode : UnaryOpOpCode
	{
		internal FloatToDoubleOpCode (BinaryReader reader) : base(reader, "float-to-double", Instructions.FloatToDouble) {}
	}
	
	public class DoubleToIntOpCode : UnaryOpOpCode
	{
		internal DoubleToIntOpCode (BinaryReader reader) : base(reader, "double-to-int", Instructions.DoubleToInt) {}
	}
	
	public class DoubleToLongOpCode : UnaryOpOpCode
	{
		internal DoubleToLongOpCode (BinaryReader reader) : base(reader, "double-to-long", Instructions.DoubleToLong) {}
	}
	
	public class DoubleToFloatOpCode : UnaryOpOpCode
	{
		internal DoubleToFloatOpCode (BinaryReader reader) : base(reader, "double-to-float", Instructions.DoubleToFloat) {}
	}
	
	public class IntToByteOpCode : UnaryOpOpCode
	{
		internal IntToByteOpCode (BinaryReader reader) : base(reader, "int-to-byte", Instructions.IntToByte) {}
	}
	
	public class IntToCharOpCode : UnaryOpOpCode
	{
		internal IntToCharOpCode (BinaryReader reader) : base(reader, "int-to-char", Instructions.IntToChar) {}
	}
	
	public class IntToShortOpCode : UnaryOpOpCode
	{
		internal IntToShortOpCode (BinaryReader reader) : base(reader, "int-to-short", Instructions.IntToShort) {}
	}

	/// <summary>
	/// Perform the identified binary operation on the two source registers, storing the result in the first source register.
	/// </summary>
	public abstract class BinaryOpOpCode : Register8OpCode
	{
		internal byte First;
		internal byte Second;

		internal BinaryOpOpCode (BinaryReader reader, string name, Instructions insn) : base(reader, name, insn)
		{
			First = reader.ReadByte();
			Second = reader.ReadByte();
		}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}, v{2}, v{3}", Name, Destination, First, Second);
		}
	}
	
	public class AddIntOpCode : BinaryOpOpCode
	{
		internal AddIntOpCode (BinaryReader reader) : base(reader, "add-int", Instructions.AddInt) {}
	}
	
	public class SubIntOpCode : BinaryOpOpCode
	{
		internal SubIntOpCode (BinaryReader reader) : base(reader, "sub-int", Instructions.SubInt) {}
	}
	
	public class MulIntOpCode : BinaryOpOpCode
	{
		internal MulIntOpCode (BinaryReader reader) : base(reader, "mul-int", Instructions.MulInt) {}
	}
	
	public class DivIntOpCode : BinaryOpOpCode
	{
		internal DivIntOpCode (BinaryReader reader) : base(reader, "div-int", Instructions.DivInt) {}
	}
	
	public class RemIntOpCode : BinaryOpOpCode
	{
		internal RemIntOpCode (BinaryReader reader) : base(reader, "rem-int", Instructions.RemInt) {}
	}
	
	public class AndIntOpCode : BinaryOpOpCode
	{
		internal AndIntOpCode (BinaryReader reader) : base(reader, "and-int", Instructions.AndInt) {}
	}
	
	public class OrIntOpCode : BinaryOpOpCode
	{
		internal OrIntOpCode (BinaryReader reader) : base(reader, "or-int", Instructions.OrInt) {}
	}
	
	public class XorIntOpCode : BinaryOpOpCode
	{
		internal XorIntOpCode (BinaryReader reader) : base(reader, "xor-int", Instructions.XorInt) {}
	}
	
	public class ShlIntOpCode : BinaryOpOpCode
	{
		internal ShlIntOpCode (BinaryReader reader) : base(reader, "shl-int", Instructions.ShlInt) {}
	}
	
	public class ShrIntOpCode : BinaryOpOpCode
	{
		internal ShrIntOpCode (BinaryReader reader) : base(reader, "shr-int", Instructions.ShrInt) {}
	}
	
	public class UshrIntOpCode : BinaryOpOpCode
	{
		internal UshrIntOpCode (BinaryReader reader) : base(reader, "ushr-int", Instructions.UshrInt) {}
	}
	
	public class AddLongOpCode : BinaryOpOpCode
	{
		internal AddLongOpCode (BinaryReader reader) : base(reader, "add-long", Instructions.AddLong) {}
	}
	
	public class SubLongOpCode : BinaryOpOpCode
	{
		internal SubLongOpCode (BinaryReader reader) : base(reader, "sub-long", Instructions.SubLong) {}
	}
	
	public class MulLongOpCode : BinaryOpOpCode
	{
		internal MulLongOpCode (BinaryReader reader) : base(reader, "mul-long", Instructions.MulLong) {}
	}
	
	public class DivLongOpCode : BinaryOpOpCode
	{
		internal DivLongOpCode (BinaryReader reader) : base(reader, "div-long", Instructions.DivLong) {}
	}
	
	public class RemLongOpCode : BinaryOpOpCode
	{
		internal RemLongOpCode (BinaryReader reader) : base(reader, "rem-long", Instructions.RemLong) {}
	}
	
	public class AndLongOpCode : BinaryOpOpCode
	{
		internal AndLongOpCode (BinaryReader reader) : base(reader, "and-long", Instructions.AndLong) {}
	}
	
	public class OrLongOpCode : BinaryOpOpCode
	{
		internal OrLongOpCode (BinaryReader reader) : base(reader, "or-long", Instructions.OrLong) {}
	}
	
	public class XorLongOpCode : BinaryOpOpCode
	{
		internal XorLongOpCode (BinaryReader reader) : base(reader, "xor-long", Instructions.XorLong) {}
	}
	
	public class ShlLongOpCode : BinaryOpOpCode
	{
		internal ShlLongOpCode (BinaryReader reader) : base(reader, "shl-long", Instructions.ShlLong) {}
	}
	
	public class ShrLongOpCode : BinaryOpOpCode
	{
		internal ShrLongOpCode (BinaryReader reader) : base(reader, "shr-long", Instructions.ShrLong) {}
	}
	
	public class UshrLongOpCode : BinaryOpOpCode
	{
		internal UshrLongOpCode (BinaryReader reader) : base(reader, "ushr-long", Instructions.UshrLong) {}
	}
	
	public class AddFloatOpCode : BinaryOpOpCode
	{
		internal AddFloatOpCode (BinaryReader reader) : base(reader, "add-float", Instructions.AddFloat) {}
	}
	
	public class SubFloatOpCode : BinaryOpOpCode
	{
		internal SubFloatOpCode (BinaryReader reader) : base(reader, "sub-float", Instructions.SubFloat) {}
	}
	
	public class MulFloatOpCode : BinaryOpOpCode
	{
		internal MulFloatOpCode (BinaryReader reader) : base(reader, "mul-float", Instructions.MulFloat) {}
	}
	
	public class DivFloatOpCode : BinaryOpOpCode
	{
		internal DivFloatOpCode (BinaryReader reader) : base(reader, "div-float", Instructions.DivFloat) {}
	}
	
	public class RemFloatOpCode : BinaryOpOpCode
	{
		internal RemFloatOpCode (BinaryReader reader) : base(reader, "rem-float", Instructions.RemFloat) {}
	}
	
	public class AddDoubleOpCode : BinaryOpOpCode
	{
		internal AddDoubleOpCode (BinaryReader reader) : base(reader, "add-double", Instructions.AddDouble) {}
	}
	
	public class SubDoubleOpCode : BinaryOpOpCode
	{
		internal SubDoubleOpCode (BinaryReader reader) : base(reader, "sub-double", Instructions.SubDouble) {}
	}
	
	public class MulDoubleOpCode : BinaryOpOpCode
	{
		internal MulDoubleOpCode (BinaryReader reader) : base(reader, "mul-double", Instructions.MulDouble) {}
	}
	
	public class DivDoubleOpCode : BinaryOpOpCode
	{
		internal DivDoubleOpCode (BinaryReader reader) : base(reader, "div-double", Instructions.DivDouble) {}
	}
	
	public class RemDoubleOpCode : BinaryOpOpCode
	{
		internal RemDoubleOpCode (BinaryReader reader) : base(reader, "rem-double", Instructions.RemDouble) {}
	}

	/// <summary>
	/// Perform the identified binary operation on the two source registers, storing the result in the first source register.
	/// </summary>
	public abstract class BinaryOp2OpCode : OpCode
	{
		internal byte Destination;
		internal byte Source;

		internal BinaryOp2OpCode (BinaryReader reader, string name, Instructions insn) : base (name, insn)
		{
			var data = reader.ReadByte();
			
			Destination = (byte)((data & 0xf0) >> 4);
			Source = (byte)(data & 0x0f);
		}
		
		public override string ToString ()
		{
			return string.Format("{0} v{1}, v{2}", Name, Destination, Source);
		}
	}
	
	public class AddInt2AddrOpCode : BinaryOp2OpCode
	{
		internal AddInt2AddrOpCode (BinaryReader reader) : base(reader, "add-int/2addr", Instructions.AddInt2Addr) {}
	}
	
	public class SubInt2AddrOpCode : BinaryOp2OpCode
	{
		internal SubInt2AddrOpCode (BinaryReader reader) : base(reader, "sub-int/2addr", Instructions.SubInt2Addr) {}
	}
	
	public class MulInt2AddrOpCode : BinaryOp2OpCode
	{
		internal MulInt2AddrOpCode (BinaryReader reader) : base(reader, "mul-int/2addr", Instructions.MulInt2Addr) {}
	}
	
	public class DivInt2AddrOpCode : BinaryOp2OpCode
	{
		internal DivInt2AddrOpCode (BinaryReader reader) : base(reader, "div-int/2addr", Instructions.DivInt2Addr) {}
	}
	
	public class RemInt2AddrOpCode : BinaryOp2OpCode
	{
		internal RemInt2AddrOpCode (BinaryReader reader) : base(reader, "rem-int/2addr", Instructions.RemInt2Addr) {}
	}
	
	public class AndInt2AddrOpCode : BinaryOp2OpCode
	{
		internal AndInt2AddrOpCode (BinaryReader reader) : base(reader, "and-int/2addr", Instructions.AndInt2Addr) {}
	}
	
	public class OrInt2AddrOpCode : BinaryOp2OpCode
	{
		internal OrInt2AddrOpCode (BinaryReader reader) : base(reader, "or-int/2addr", Instructions.OrInt2Addr) {}
	}
	
	public class XorInt2AddrOpCode : BinaryOp2OpCode
	{
		internal XorInt2AddrOpCode (BinaryReader reader) : base(reader, "xor-int/2addr", Instructions.XorInt2Addr) {}
	}
	
	public class ShlInt2AddrOpCode : BinaryOp2OpCode
	{
		internal ShlInt2AddrOpCode (BinaryReader reader) : base(reader, "shl-int/2addr", Instructions.ShlInt2Addr) {}
	}
	
	public class ShrInt2AddrOpCode : BinaryOp2OpCode
	{
		internal ShrInt2AddrOpCode (BinaryReader reader) : base(reader, "shr-int/2addr", Instructions.ShrInt2Addr) {}
	}
	
	public class UshrInt2AddrOpCode : BinaryOp2OpCode
	{
		internal UshrInt2AddrOpCode (BinaryReader reader) : base(reader, "ushr-int/2addr", Instructions.UshrInt2Addr) {}
	}
	
	public class AddLong2AddrOpCode : BinaryOp2OpCode
	{
		internal AddLong2AddrOpCode (BinaryReader reader) : base(reader, "add-long/2addr", Instructions.AddLong2Addr) {}
	}
	
	public class SubLong2AddrOpCode : BinaryOp2OpCode
	{
		internal SubLong2AddrOpCode (BinaryReader reader) : base(reader, "sub-long/2addr", Instructions.SubLong2Addr) {}
	}
	
	public class MulLong2AddrOpCode : BinaryOp2OpCode
	{
		internal MulLong2AddrOpCode (BinaryReader reader) : base(reader, "mul-long/2addr", Instructions.MulLong2Addr) {}
	}
	
	public class DivLong2AddrOpCode : BinaryOp2OpCode
	{
		internal DivLong2AddrOpCode (BinaryReader reader) : base(reader, "div-long/2addr", Instructions.DivLong2Addr) {}
	}
	
	public class RemLong2AddrOpCode : BinaryOp2OpCode
	{
		internal RemLong2AddrOpCode (BinaryReader reader) : base(reader, "rem-long/2addr", Instructions.RemLong2Addr) {}
	}
	
	public class AndLong2AddrOpCode : BinaryOp2OpCode
	{
		internal AndLong2AddrOpCode (BinaryReader reader) : base(reader, "and-long/2addr", Instructions.AndLong2Addr) {}
	}
	
	public class OrLong2AddrOpCode : BinaryOp2OpCode
	{
		internal OrLong2AddrOpCode (BinaryReader reader) : base(reader, "or-long/2addr", Instructions.OrLong2Addr) {}
	}
	
	public class XorLong2AddrOpCode : BinaryOp2OpCode
	{
		internal XorLong2AddrOpCode (BinaryReader reader) : base(reader, "xor-long/2addr", Instructions.XorLong2Addr) {}
	}
	
	public class ShlLong2AddrOpCode : BinaryOp2OpCode
	{
		internal ShlLong2AddrOpCode (BinaryReader reader) : base(reader, "shl-long/2addr", Instructions.ShlLong2Addr) {}
	}
	
	public class ShrLong2AddrOpCode : BinaryOp2OpCode
	{
		internal ShrLong2AddrOpCode (BinaryReader reader) : base(reader, "shr-long/2addr", Instructions.ShrLong2Addr) {}
	}
	
	public class UshrLong2AddrOpCode : BinaryOp2OpCode
	{
		internal UshrLong2AddrOpCode (BinaryReader reader) : base(reader, "ushr-long/2addr", Instructions.UshrLong2Addr) {}
	}
	
	public class AddFloat2AddrOpCode : BinaryOp2OpCode
	{
		internal AddFloat2AddrOpCode (BinaryReader reader) : base(reader, "add-float/2addr", Instructions.AddFloat2Addr) {}
	}
	
	public class SubFloat2AddrOpCode : BinaryOp2OpCode
	{
		internal SubFloat2AddrOpCode (BinaryReader reader) : base(reader, "sub-float/2addr", Instructions.SubFloat2Addr) {}
	}
	
	public class MulFloat2AddrOpCode : BinaryOp2OpCode
	{
		internal MulFloat2AddrOpCode (BinaryReader reader) : base(reader, "mul-float/2addr", Instructions.MulFloat2Addr) {}
	}
	
	public class DivFloat2AddrOpCode : BinaryOp2OpCode
	{
		internal DivFloat2AddrOpCode (BinaryReader reader) : base(reader, "div-float/2addr", Instructions.DivFloat2Addr) {}
	}
	
	public class RemFloat2AddrOpCode : BinaryOp2OpCode
	{
		internal RemFloat2AddrOpCode (BinaryReader reader) : base(reader, "rem-float/2addr", Instructions.RemFloat2Addr) {}
	}
	
	public class AddDouble2AddrOpCode : BinaryOp2OpCode
	{
		internal AddDouble2AddrOpCode (BinaryReader reader) : base(reader, "add-double/2addr", Instructions.AddDouble2Addr) {}
	}
	
	public class SubDouble2AddrOpCode : BinaryOp2OpCode
	{
		internal SubDouble2AddrOpCode (BinaryReader reader) : base(reader, "sub-double/2addr", Instructions.SubDouble2Addr) {}
	}
	
	public class MulDouble2AddrOpCode : BinaryOp2OpCode
	{
		internal MulDouble2AddrOpCode (BinaryReader reader) : base(reader, "mul-double/2addr", Instructions.MulDouble2Addr) {}
	}
	
	public class DivDouble2AddrOpCode : BinaryOp2OpCode
	{
		internal DivDouble2AddrOpCode (BinaryReader reader) : base(reader, "div-double/2addr", Instructions.DivDouble2Addr) {}
	}
	
	public class RemDouble2AddrOpCode : BinaryOp2OpCode
	{
		internal RemDouble2AddrOpCode (BinaryReader reader) : base(reader, "rem-double/2addr", Instructions.RemDouble2Addr) {}
	}

	/// <summary>
	/// Perform the indicated binary op on the indicated register (first argument) and literal value 
	/// (second argument), storing the result in the destination register.
	/// Note: rsub-int does not have a suffix since this version is the main opcode of its family. 
	/// Also, see below for details on its semantics.
	/// </summary>
	public abstract class AbsBinaryOpLit<D,S,C> : OpCode
	{
		internal D Destination;
		internal S Source;
		internal C Constant;

		public AbsBinaryOpLit (string name, Instructions insn) : base (name, insn)
		{
		}

		public override string ToString ()
		{
			return string.Format("{0} v{1}, v{2}, #{3}", Name, Destination, Source, Constant);
		}
	}

	public abstract class BinaryOpLit16OpCode : AbsBinaryOpLit<byte,byte,short>
	{
		internal BinaryOpLit16OpCode (BinaryReader reader, string name, Instructions insn) : base (name, insn)
		{
			SplitByte (reader.ReadByte (), out Destination, out Source);
			Constant = (short)reader.ReadUInt16();
		}
	}
		
	public class AddIntLit16OpCode : BinaryOpLit16OpCode
	{
		internal AddIntLit16OpCode (BinaryReader reader) : base(reader, "add-int/lit16", Instructions.AddIntLit16) {}
	}
	
	public class RsubIntOpCode : BinaryOpLit16OpCode
	{
		internal RsubIntOpCode (BinaryReader reader) : base(reader, "rsub-int", Instructions.RsubInt) {}
	}
	
	public class MulIntLit16OpCode : BinaryOpLit16OpCode
	{
		internal MulIntLit16OpCode (BinaryReader reader) : base(reader, "mul-int/lit16", Instructions.MulIntLit16) {}
	}
	
	public class DivIntLit16OpCode : BinaryOpLit16OpCode
	{
		internal DivIntLit16OpCode (BinaryReader reader) : base(reader, "div-int/lit16", Instructions.DivIntLit16) {}
	}
	
	public class RemIntLit16OpCode : BinaryOpLit16OpCode
	{
		internal RemIntLit16OpCode (BinaryReader reader) : base(reader, "rem-int/lit16", Instructions.RemIntLit16) {}
	}
	
	public class AndIntLit16OpCode : BinaryOpLit16OpCode
	{
		internal AndIntLit16OpCode (BinaryReader reader) : base(reader, "and-int/lit16", Instructions.AndIntLit16) {}
	}
	
	public class OrIntLit16OpCode : BinaryOpLit16OpCode
	{
		internal OrIntLit16OpCode (BinaryReader reader) : base(reader, "or-int/lit16", Instructions.OrIntLit16) {}
	}
	
	public class XorIntLit16OpCode : BinaryOpLit16OpCode
	{
		internal XorIntLit16OpCode (BinaryReader reader) : base(reader, "xor-int/lit16", Instructions.XorIntLit16) {}
	}

	/// <summary>
	/// Perform the indicated binary op on the indicated register (first argument) and literal value 
	/// (second argument), storing the result in the destination register.
	/// Note: See below for details on the semantics of rsub-int.
	/// </summary>
	public abstract class BinaryOpLit8OpCode : AbsBinaryOpLit<byte,byte,sbyte>
	{
		internal BinaryOpLit8OpCode (BinaryReader reader, string name, Instructions insn) : base(name, insn)
		{
			Destination = reader.ReadByte();
			Source = reader.ReadByte();
			Constant = reader.ReadSByte();
		}
	}
	
	public class AddIntLit8OpCode : BinaryOpLit8OpCode
	{
		internal AddIntLit8OpCode (BinaryReader reader) : base(reader, "add-int/lit8", Instructions.AddIntLit8) {}
	}
	
	public class RsubIntLit8OpCode : BinaryOpLit8OpCode
	{
		internal RsubIntLit8OpCode (BinaryReader reader) : base(reader, "rsub-int/lit8", Instructions.RsubIntLit8) {}
	}
	
	public class MulIntLit8OpCode : BinaryOpLit8OpCode
	{
		internal MulIntLit8OpCode (BinaryReader reader) : base(reader, "mul-int/lit8", Instructions.MulIntLit8) {}
	}
	
	public class DivIntLit8OpCode : BinaryOpLit8OpCode
	{
		internal DivIntLit8OpCode (BinaryReader reader) : base(reader, "div-int/lit8", Instructions.DivIntLit8) {}
	}
	
	public class RemIntLit8OpCode : BinaryOpLit8OpCode
	{
		internal RemIntLit8OpCode (BinaryReader reader) : base(reader, "rem-int/lit8", Instructions.RemIntLit8) {}
	}
	
	public class AndIntLit8OpCode : BinaryOpLit8OpCode
	{
		internal AndIntLit8OpCode (BinaryReader reader) : base(reader, "and-int/lit8", Instructions.AndIntLit8) {}
	}
	
	public class OrIntLit8OpCode : BinaryOpLit8OpCode
	{
		internal OrIntLit8OpCode (BinaryReader reader) : base(reader, "or-int/lit8", Instructions.OrIntLit8) {}
	}
	
	public class XorIntLit8OpCode : BinaryOpLit8OpCode
	{
		internal XorIntLit8OpCode (BinaryReader reader) : base(reader, "xor-int/lit8", Instructions.XorIntLit8) {}
	}
	
	public class ShlIntLit8OpCode : BinaryOpLit8OpCode
	{
		internal ShlIntLit8OpCode (BinaryReader reader) : base(reader, "shl-int/lit8", Instructions.ShlIntLit8) {}
	}
	
	public class ShrIntLit8OpCode : BinaryOpLit8OpCode
	{
		internal ShrIntLit8OpCode (BinaryReader reader) : base(reader, "shr-int/lit8", Instructions.ShrIntLit8) {}
	}
	
	public class UshrIntLit8OpCode : BinaryOpLit8OpCode
	{
		internal UshrIntLit8OpCode (BinaryReader reader) : base(reader, "ushr-int/lit8", Instructions.UshrIntLit8) {}
	}
}