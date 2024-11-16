using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class Cell
    {
        public (int, int) Position { get; private set; }
        public int Entry { get; set; }
        public List<int> Domain { get; private set; }

        public Cell((int,int) position, int entry)
        {
            Position = position;
            Entry = entry;
            Domain = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        public void ChangeValue(int value)
        {
            Entry = value;
        }
        public bool IsEmpty()
        {
            return Entry == 0;
        }
    }
}
