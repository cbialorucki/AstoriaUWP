using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

/// <summary>
/// Dex.NET - Mario Kosmiskas
/// 
/// Provided under the Apache 2.0 License: http://www.apache.org/licenses/LICENSE-2.0
/// Commercial use requires attribution
/// </summary>
namespace dex.net
{
	public class Method
	{
		// Index into the Type Ids list for the definer of this method
		public ushort ClassIndex { get; internal set; }

		public readonly uint Id;

		// Index into the Type Ids list for the type of this method
		internal ushort PrototypeIndex;

		// index into the string_ids list for the name of this method
		internal uint NameIndex;

		internal ulong AccessFlags;

		internal long CodeOffset;
		internal long CodeLength;
		internal uint RegistersSize;
		internal uint ArgumentsSize;
		internal uint ReturnValuesSize;

		private Dex Dex;

		public IEnumerable<Annotation> Annotations { get; internal set; }
		public List<Annotation> ParameterAnnotations { get; internal set; }

		internal TryCatchBlock[] TryCatchBlocks;

		public string Name
		{
			get { return Dex.GetString (NameIndex); }
		}

		public uint LocalsCount {
			get { return RegistersSize - ArgumentsSize; }
		}

		internal Method (uint id, Dex dex, BinaryReader reader, uint codeOffset)
		{
			Id = id;
			Dex = dex;

			ClassIndex = reader.ReadUInt16 ();
			PrototypeIndex = reader.ReadUInt16 ();
			NameIndex = reader.ReadUInt32 ();

			Annotations = new Annotation[0];
			ParameterAnnotations = new List<Annotation>();

			// This method has no opcodes, must be abstract or native.
			// Or the method is being loaded directly from the methods list
			if (codeOffset != 0) {
				reader.BaseStream.Position = codeOffset;

				RegistersSize = reader.ReadUInt16 ();
				ArgumentsSize = reader.ReadUInt16 ();
				ReturnValuesSize = reader.ReadUInt16 ();
				var numberOfTryItems = reader.ReadUInt16 ();
				// Not parsing debug info
				/*var debugInfoOffset =*/ reader.ReadUInt32 ();
				CodeLength = reader.ReadUInt32 ();

				CodeOffset = reader.BaseStream.Position;

				// Skip the opcode block
				reader.BaseStream.Position += CodeLength*2;

				// Skip the optional padding
				if ((numberOfTryItems != 0) && (CodeLength % 2 != 0))
					reader.BaseStream.Position += 2;

				// Load the try blocks
				if (numberOfTryItems != 0) {
					TryCatchBlocks = new TryCatchBlock[numberOfTryItems];
					var handlerOffsets = new long[numberOfTryItems];

					// read Try
					for (int i=0; i<numberOfTryItems; i++) {
						var tryCatch = new TryCatchBlock (CodeOffset); 
						tryCatch.StartAddress = reader.ReadUInt32 ();
						tryCatch.InstructionCount = reader.ReadUInt16 ();

						TryCatchBlocks [i] = tryCatch;

						handlerOffsets[i] = reader.ReadUInt16 ();
					}

					var encodedCatchHandlerListOffset = reader.BaseStream.Position;
					// Size of the list, could be used to confirm the DEX is properly
					// built. For this purpose will assume DEX files are always good
					Leb128.ReadUleb (reader);

					// read Catch blocks
					for (int i=0; i<numberOfTryItems; i++) {
						//encoded_catch_handler
						if (handlerOffsets [i] > 0) {
							reader.BaseStream.Position = encodedCatchHandlerListOffset + handlerOffsets [i];

							var handlersCount = Leb128.ReadLeb (reader);
							var handlers = new CatchHandler[handlersCount<=0 ? Math.Abs(handlersCount)+1 : handlersCount];

							for (int j=0; j<Math.Abs(handlersCount); j++) {
								var catchHandler = new CatchHandler (CodeOffset);
								catchHandler.TypeIndex = Leb128.ReadUleb (reader);
								catchHandler.Address = Leb128.ReadUleb (reader);

								handlers [j] = catchHandler;
							}

							// parse the catch all block
							if (handlersCount <= 0) {
								var catchHandler = new CatchHandler (CodeOffset);
								catchHandler.TypeIndex = 0;
								catchHandler.Address = Leb128.ReadUleb (reader);

								handlers [handlers.Length-1] = catchHandler;
							}

							TryCatchBlocks [i].Handlers = handlers;
						}
					}
				}
			}

		}

		public IEnumerable<OpCode> GetInstructions ()
		{
			var offset = CodeOffset;
			var finalOffset = CodeOffset+(CodeLength*2);

			while (offset < finalOffset) {
				OpCode opcode = null;
				using (var reader = Dex.GetReader (offset)) {
					opcode = Disassembler.Decode(reader);
					opcode.OpCodeOffset = offset;

					offset = reader.BaseStream.Position;
				}

				// Instruction adds a data table at the end of the method
				// must skip all data tables since they are not instructions
				if (opcode is DataExtendedOpCode) {
					var deOpcode = (DataExtendedOpCode)opcode;

					if (deOpcode.DataTableOffset < finalOffset) {
						finalOffset = deOpcode.DataTableOffset;
					}
				}

				yield return opcode;
			}
		}

		public uint GetRegisterCount ()
		{
			return RegistersSize;
		}

		public bool IsStatic() {
			return (((AccessFlag)AccessFlags) & AccessFlag.ACC_STATIC) != 0;
		}

		public override string ToString ()
		{
			return "Method: " + Name;
		}
	}

	public class TryCatchBlock
	{
		// start address of the block of code covered by this entry
		internal uint StartAddress;

		// number of 16-bit code units covered by this entry. 
		// The last code unit covered (inclusive) is start_addr + insn_count - 1
		internal ushort InstructionCount;

		internal CatchHandler[] Handlers;

		private long _codeOffset;

		internal TryCatchBlock(long codeOffset)
		{
			_codeOffset = codeOffset;
		}

		public bool IsInBlock(long offset) 
		{
			return (offset >= StartAddress*2+_codeOffset) && (offset <= LastInstructionOffset());
		}

		public long StartInstructionOffset()
		{
			return StartAddress*2+_codeOffset;
		}

		public long LastInstructionOffset()
		{
			return (StartAddress*2+_codeOffset) + (InstructionCount * 2) - 1;
		}
	}

	public class CatchHandler
	{
		// index into the type_ids list for the type of the exception to catch
		internal uint TypeIndex;

		// bytecode address of the associated exception handler
		internal uint Address;

		private long _codeOffset;

		/// <summary>
		/// The calculated offset for the handler for this catch statement
		/// </summary>
		/// <value>The handler address.</value>
		public long HandlerOffset {
			get { return _codeOffset + Address * 2; }
		}

		internal CatchHandler(long codeOffset)
		{
			_codeOffset = codeOffset;
		}
	}
}