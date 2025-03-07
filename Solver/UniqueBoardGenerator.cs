using Sudoku_Solver_NEA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class UniqueBoardGenerator : IBoardGenerator
    {
        public Board GenerateUniqueSolution(int dimensions, Board board)
        {
            // defines the maximum number of variable nodes that should be present on a board for given dimensions
            int stackSize;
            switch (dimensions)
            {
                case 9:
                    stackSize = 70;
                    break;
                default:
                    stackSize = 90;
                    break;
            }
            bool validBoard = false;
            MoveStack stack = new MoveStack(stackSize);
            List<Cell> variableNodesCopy = new();
            Random random = new Random();
            ForwardChecker solver = new ForwardChecker(board);
            board.InitialiseQueue();
            // a board which is not completely empty indicates a board that has been regenerated from the text file
            if (board.VariableNodes.Count != Math.Pow(dimensions, 2))
            {
                // gets single solution, guaranteed by previous generation of a unique solution
                solver.HasUniqueSolution();
                return board;
            }
            // fills all cells with valid initial conditions
            solver.Solve();
            // copies all variable nodes over to a new list
            // this new list is used for the selection of random cells during the process of unique solution generation
            foreach (Cell cell in board.VariableNodes)
            {
                variableNodesCopy.Add(cell);
            }
            // solver interacts with the board's variable nodes when solving - the initial variable nodes should all be removed + added gradually
            board.VariableNodes.Clear();
            while (!validBoard)
            {
                // if the number of variable nodes is greater than the upper limit
                // generation gets exponentially slower after this point (by experimentation, for 16x16 boards)
                if (board.VariableNodes.Count > stackSize)
                {
                    // revert all changed cells back to their initial, fixed values
                    while (stack.Count > 0)
                    {
                        Move tempMove = stack.Pop();
                        tempMove.Cell.ChangeCellValue(tempMove.OldEntry);
                        board.VariableNodes.Remove(tempMove.Cell);
                        variableNodesCopy.Add(tempMove.Cell);
                    }
                }
                // chooses a random cell from the selection list
                Cell randomCell = variableNodesCopy[random.Next(variableNodesCopy.Count)];
                Move move = new(randomCell, randomCell.Entry);
                // adds any new potential domains to all affected cells on the board
                AddDomains(move, board);
                // records move
                stack.Push(move);
                randomCell.ChangeCellValue(0);
                // adds cell to VariableNodes + initialises queue and MostRecentlyChangedCell for solving
                board.VariableNodes.Add(randomCell);
                board.InitialiseQueue();
                solver.ChangeMostRecentCell(null);
                solver.HasUniqueSolution();
                // if the board does not have a unique solution
                if (board.SolutionCount != 1)
                {
                    // undo the previous move, reverting the most recently changed variable node to fixed with a set value
                    Move previousMove = stack.Pop();
                    board.VariableNodes.Remove(randomCell);
                    previousMove.Cell.ChangeCellValue(previousMove.OldEntry);
                    ResetBoardProperties(board);
                    // initialises queue and MostRecentlyChangedCell for solving
                    board.InitialiseQueue();
                    RemoveDomains(previousMove, board);
                    solver.ChangeMostRecentCell(null);
                    // gets single unique solution + stores it in solutions
                    solver.HasUniqueSolution();
                    // ends loop
                    validBoard = true;
                }
                // if board has a unique solution, keep increasing the number of variable nodes 
                else
                {
                    // removes the most recently changed cell from the selection list
                    variableNodesCopy.Remove(randomCell);
                    ResetBoardProperties(board);
                }
            }
            return board;
        }

        // used when converting a node from fixed to variable
        private void AddDomains(Move move, Board board)
        {
            // for the changed node and each connected node
            // if the domain of the node does not contain the previous fixed value of the changed cell prior to removal
            // add the value to the domain
            if (!move.Cell.Domain.Contains(move.OldEntry))
            {
                move.Cell.Domain.Add(move.OldEntry);
            }
            foreach (Cell connectedNode in board.AdjacencyList[move.Cell])
            {
                if (!connectedNode.Domain.Contains(move.OldEntry))
                {
                    connectedNode.Domain.Add(move.OldEntry);
                }
            }
        }

        // used when converting a node from variable to fixed
        private void RemoveDomains(Move move, Board board)
        {
            // for each connected node to a cell
            foreach (Cell cell in board.AdjacencyList[move.Cell])
            {
                // if the cell's domain contains the previous, fixed value of the base cell, remove the value from the cell's domain
                if (cell.Domain.Contains(move.OldEntry))
                {
                    cell.Domain.Remove(move.OldEntry);
                }
            }
        }

        private void ResetBoardProperties(Board board)
        {
            // resets board metrics so future solving iterations are not affected
            board.SetSolutionCount(0);
            board.Solutions.Clear();
            board.Reset();
        }
    }
}
