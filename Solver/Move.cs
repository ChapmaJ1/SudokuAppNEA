using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class Move
    {
        public Cell Cell { get; private set; }
        public int OldEntry { get; private set; }
        public Move(Cell cell, int oldEntry)
        {
            Cell = cell;
            OldEntry = oldEntry;
        }
    }
}
