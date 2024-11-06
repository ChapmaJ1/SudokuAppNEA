using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA.API_Classes
{
    public class Grids
    {
        public int[][] Value { get; set; }
        public string Difficulty { get; set; }
        public Grids()
        {
            Value = new int[9][];
            Difficulty = string.Empty;
        }
    }
}
