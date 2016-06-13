using Next.Instructions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Next
{
    public class Compressor
    {
        public byte[] Compress(byte[] source, byte[] target)
        {
            var instructions = ParseInstructions(source, target);

            var serializer = new Serializer();

            return serializer.Serialize(instructions);
        }

        public byte[] Uncompress(byte[] source, byte[] delta)
        {
            var serializer = new Serializer();
            var instructions = serializer.Deserialize(delta);

            return ApplyInstructions(source, instructions);
        }

        private ICollection<IInstruction> ParseInstructions(byte[] source, byte[] target)
        {
            List<IInstruction> instructions = new List<IInstruction>();

            int offset = 0;
            bool increment = true;
            for (int i = 0; i < source.Length; )
            {
                increment = true;
                if (i + offset > target.Length)
                {
                    break;
                }

                byte one = source[i];
                byte two = target[i + offset];

                if (one == two)
                {
                    var copy = new Copy()
                    {
                        Index = i,
                        Length = 1
                    };

                    for (int j = i + 1; j < source.Length; j++)
                    {
                        one = source[j];
                        two = target[j + offset];

                        if (one == two)
                        {
                            copy.Length++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    i = copy.Index + copy.Length;
                    instructions.Add(copy);   
                    increment = false;
                }
                else
                {
                    //Todo: run goes here
                    var add = new Add();
                    add.Index = i;

                    using (MemoryStream stream = new MemoryStream())
                    {
                        for (int j = i + offset; j < target.Length; j++)
                        {
                            two = target[j];

                            if (one != two)
                            {
                                stream.WriteByte(two);
                            }
                            else
                            {
                                increment = false;
                                break;
                            }
                        }
                        add.Data = stream.ToArray();
                    }
                    offset = Convert.ToInt32(add.Data.Length);
                    instructions.Add(add);
                }

                if (increment)
                {
                    i++;
                }
            }
            

            return instructions;
        }

        private byte[] ApplyInstructions(byte[] source, ICollection<IInstruction> instructions)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                foreach (var instruction in instructions)
                {
                    if (instruction is Copy)
                    {
                        var copy = instruction as Copy;
                        
                        
                        var data = source.Skip(copy.Index).Take(copy.Length).ToArray();
                        var test = Encoding.UTF8.GetString(data);
                        stream.Write(data, 0, data.Length);
                    }
                    else if (instruction is Add)
                    {
                        var add = instruction as Add;
                        var test = Encoding.UTF8.GetString(add.Data);
                        stream.Write(add.Data, 0, add.Data.Length);
                    }
                    else if (instruction is Run)
                    {
                        var run = instruction as Run;

                        for (int i = 0; i < run.Length; i++)
                        {
                            stream.WriteByte(run.Value);
                        }
                    }
                }
                return stream.ToArray();
            }
        }
    }
}
