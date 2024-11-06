using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA.API_Classes
{
    public class NewBoard
    {
        public Grids[] Grids { get; set; }
        public NewBoard()
        {
            Grids = new Grids[0];
        }
    }
}
