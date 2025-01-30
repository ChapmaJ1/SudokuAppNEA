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
        public int Entry { get; private set; }
        public List<int> Domain { get; private set; }
        public List<int> NoteEntries { get; private set; }

        public Cell((int,int) position, int entry)
        {
            Position = position;  // location (i,j) of the cell
            Entry = entry;  // the value of the cell at any given moment
            Domain = new List<int>(); 
            NoteEntries = new List<int>();  // the "note" values of the cell assigned by the user while playing
        }

        public void ChangeCellValue(int value)
        {
            Entry = value;
        }

        internal void InitialiseDomain(int dimensions) // potential values that the cell could take without violating constraints. initially every number is possible
        {
            for (int i=1; i<=dimensions; i++)
            {
                Domain.Add(i);
            }
        }
    }
}
