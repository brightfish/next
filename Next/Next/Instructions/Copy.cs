using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Instructions
{
    public class Copy : IInstruction
    {
        public int Index { get; set; }
        public int Length { get; set; }
    }
}
