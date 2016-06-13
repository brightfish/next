using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Instructions
{
    public class Run : IInstruction
    {
        public byte Value { get; set; }
        public int Length { get; set; }
    }
}
