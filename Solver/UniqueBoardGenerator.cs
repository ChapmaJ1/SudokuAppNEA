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

        public void GenerateUniqueSolution(int dimensions)
        {
            ForwardChecker solver = new ForwardChecker(Board);
            bool unique = false;
            SetInitialNumbers(dimensions);  // if the number of fixed numbers on the board is less than the required threshold for a unique board
            solver.PrintBoard(Board);
            while (unique == false)
            {
                solver!.HasUniqueSolution();
                if (Board.SolutionCount >= 2)  // if board does not have a unique solution, and hence is not a valid Sudoku
                {
                    for (int i = 0; i < dimensions; i++)
                    {
                        for (int j = 0; j < dimensions; j++)
                        {
                            if (Board.Solutions[0].BoardSketch[i, j] != Board.Solutions[1].BoardSketch[i, j])  // a square which has a different value between the 2 solutions
                            {
                                Cell cell = Board.GetCellLocation(i, j);
                                cell.ChangeCellValue(Board.Solutions[0].BoardSketch[i, j]);
                                i = dimensions;
                                j = dimensions;
                                Board.VariableNodes.Remove(cell);  // sets the cell to be fixed, taking on the value from the first solution
                                Board.Reset();  // resets the board so the solving process can be repeated to find a unique solution
                            }
                        }
                    }
                    Board.Solutions.Clear();
                    Board.SetSolutionCount(0);
                }
                else
                {
                    unique = true;
                }
            }
        }

        private void SetInitialNumbers(int dimensions)
        {
            Random random = new Random();
            BacktrackingSolver solver = new BacktrackingSolver(Board);
            int numberCap = 17;  // minimum numbers required for a unique 9x9 Sudoku
            if (dimensions == 16)
            {
                numberCap = 100;  // minimum numbers required for a unique 16x16 Sudoku - guess
            }
            int maximumVariableNodes = Convert.ToInt32(Math.Pow(dimensions,2)) - numberCap;
            while (Board.VariableNodes.Count > maximumVariableNodes)   // while the number of fixed cells is below the required threshold
            {
                int randomCellIndex = random.Next(Board.VariableNodes.Count);
                Cell randomCell = Board.VariableNodes[randomCellIndex];
                int randomNumberIndex = random.Next(randomCell.Domain.Count);
                int randomNumber = randomCell.Domain[randomNumberIndex];
                randomCell.ChangeCellValue(randomNumber);  // selects a random cell, and changes its value to a random number in its domain
                if (solver.CheckInvalid())
                {
                    solver.PrintBoard(Board);
                    Console.WriteLine("Dead");
                }
                PruneDomains(randomCell, randomNumber);  // prunes the domains so no connected cells will take on identical values in future
                Board.VariableNodes.Remove(randomCell);  // sets the cell to a fixed node
            }
        }

        private void PruneDomains(Cell cell, int number)
        {
            foreach (Cell connectedNode in Board.AdjacencyList[cell])
            {
                if (connectedNode.Domain.Contains(number))
                {
                    connectedNode.Domain.Remove(number);
                }
            }
        }
    }
}
