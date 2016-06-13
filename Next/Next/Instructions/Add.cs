using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Instructions
{
    public class Add : IInstruction
    {
        public int Index { get; set; }
        public byte[] Data { get; set; }
    }
}
