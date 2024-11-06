using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA.API_Classes
{
    public class ResponseData
    {
        public NewBoard NewBoard { get; set; }
        public ResponseData()
        {
            NewBoard = new NewBoard();
        }
    }
}
