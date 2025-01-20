using Sudoku_Solver_NEA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class UniqueBoardGenerator
    {
        public Board Board { get; private set; }
        public UniqueBoardGenerator(Board board)
        {
            Board = board;
        }

        public void GenerateUniqueSolution()
        {
            ForwardChecker solver = new ForwardChecker(Board);
            bool unique = false;
            while (unique == false)
            {
                solver!.HasUniqueSolution();
                if (Board.SolutionCount >= 2)  // if board does not have a unique solution, and hence is not a valid Sudoku
                {
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            if (Board.Solutions[0].BoardSketch[i, j] != Board.Solutions[1].BoardSketch[i, j])  // a square which has a different value between the 2 solutions
                            {
                                Cell cell = Board.GetCellLocation(i, j);
                                cell.Entry = Board.Solutions[0].BoardSketch[i, j];
                                i = 9;
                                j = 9;
                                Board.VariableNodes.Remove(cell);  // sets the cell to be fixed, taking on the value from the first solution
                                Board.Reset();  // resets the board so the solving process can be repeated to find a unique solution
                            }
                        }
                    }
                    Board.Solutions.Clear();
                    Board.SolutionCount = 0;
                }
                else
                {
                    unique = true;
                }
            }
        }
    }
}
