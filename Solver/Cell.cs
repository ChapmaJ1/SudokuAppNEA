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
        public List<int> NoteEntries { get; private set; }

        public Cell((int,int) position, int entry)
        {
            Position = position;  // location (i,j) of the cell
            Entry = entry;  // the value of the cell at any given moment
            Domain = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };  // potential values that the cell could take without violating constraints. initially every number is possible
            NoteEntries = new List<int>();  // the "note" values of the cell assigned by the user while playing
        }
    }
}
