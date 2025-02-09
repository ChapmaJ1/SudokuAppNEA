using Sudoku_Solver_NEA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public interface IBoardGenerator
    {
        public Board GenerateUniqueSolution(int dimensions , Board board);
    }
}
