using Next.Instructions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next
{
    public class Serializer
    {
        public byte[] Serialize(ICollection<IInstruction> instructions)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                foreach (var instruction in instructions)
                {
                    if (instruction is Copy)
                    {
                        var copy = instruction as Copy;
                        writer.Write((byte)InstructionTypes.Copy);
                        writer.Write(copy.Index);
                        writer.Write(copy.Length);
                    }
                    else if (instruction is Add)
                    {
                        var add = instruction as Add;
                        writer.Write((byte)InstructionTypes.Add);
                        writer.Write(add.Index);
                        writer.Write(add.Data.Length);
                        writer.Write(add.Data);
                    }
                    else if (instruction is Run)
                    {
                        var run = instruction as Run;
                        writer.Write((byte)InstructionTypes.Run);
                        writer.Write(run.Length);
                        writer.Write(run.Value);
                    }
                }
                return stream.ToArray();
            }
        }

        public ICollection<IInstruction> Deserialize(byte[] delta)
        {
            var instructions = new List<IInstruction>();

            using (MemoryStream stream = new MemoryStream(delta))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    InstructionTypes instructionType = (InstructionTypes)reader.ReadByte();

                    switch (instructionType)
                    {
                        case InstructionTypes.Add:
                            var add = new Add();
                            add.Index = reader.ReadInt32();
                            int length = reader.ReadInt32();
                            add.Data = reader.ReadBytes(length);

                            instructions.Add(add);
                            break;
                        case InstructionTypes.Copy:
                            var copy = new Copy();
                            copy.Index = reader.ReadInt32();
                            copy.Length = reader.ReadInt32();

                            instructions.Add(copy);
                            break;
                        case InstructionTypes.Run:
                            var run = new Run();
                            run.Length = reader.ReadInt32();
                            run.Value = reader.ReadByte();

                            instructions.Add(run);
                            break;
                        default:
                            throw new FormatException();
                    }
                }
            }

            return instructions;
        }
    }
}
