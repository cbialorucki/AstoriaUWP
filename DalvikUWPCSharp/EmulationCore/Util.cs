using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace DalvikUWPCSharp.Classes
{
    public static class Util
    {


        /*public async static void ExecuteOperation(VMOperation instruction, DalvikCPU cpu)
        {
            switch (instruction.OpCode)
            {
                case 0x0:
                    Debug.WriteLine("nop");
                    break;
                case 0x1:
                    //move vA, Vb
                    Debug.WriteLine("move " + instruction.RegA + ", " + instruction.RegB);
                    cpu.Registers[instruction.RegB] = cpu.Registers[instruction.RegA];
                    break;
                case 0x2:
                    //move/from16 vAA, vBBBB
                    Debug.WriteLine("move/from16 vAA, vBBBB");
                    break;
                case 0x3:
                    //move/16 vAAAA, vBBBB
                    Debug.WriteLine("move/16 vAAAA, vBBBB");
                    break;
                case 0x4:
                    //move-wide
                    Debug.WriteLine("move-wide vA, vB");
                    break;
                case 0x5:
                    //move-wide/from16 vAA, vBBBB
                    Debug.WriteLine("move-wide/from16 vAA, vBBBB");
                    break;
                case 0x6:
                    //move-wide/16 vAA, vBBBB
                    Debug.WriteLine("move-wide/16 vAAAA, vBBBB");
                    break;
                case 0x0e:
                    Debug.WriteLine("return void");
                    break;
                case 0x90:
                    //Add-Int vAA, vBB, vCC
                    cpu.Registers[instruction.RegA] = cpu.Registers[instruction.RegB] + cpu.Registers[instruction.RegC];
                    Debug.WriteLine("RegA = Add RegB and RegC" + cpu.Registers[instruction.RegA] + cpu.Registers[instruction.RegB] + cpu.Registers[instruction.RegC]);
                    break;
                default:
                    //Halt
                    Debug.WriteLine("HALT!");
                    var dialog = new MessageDialog("Application halted.");
                    await dialog.ShowAsync();
                    break;
            }
        }

        public static uint[] GetSampleCode()
        {
            uint[] code = { 0x1014, 0x110A, 0x9000, 0x0000 };
            return code;
        }

        public static VMOperation DecodeInstruction(uint Instruction)
        {
            VMOperation operation = new VMOperation();
            operation.OpCode = (Instruction & 0xF000) >> 12;
            operation.RegA = (Instruction & 0x0F00) >> 8;
            operation.RegB = (Instruction & 0x00F0) >> 4;
            operation.RegC = (Instruction & 0x000F);
            operation.Scalar = (Instruction & 0x00FF);

            return operation;
        }*/

    }
}
