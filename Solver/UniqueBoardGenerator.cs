using Sudoku_Solver_NEA;
using System;
using System.Collections;
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
                    // generation gets much slower after this point for 16x16 boards (by experimentation)
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
            // solver interacts with the board's variable nodes when solving - the initial variable nodes are all removed + added gradually
            board.VariableNodes.Clear();
            while (!validBoard)
            {
                // if the number of variable nodes is greater than the upper limit
                if (board.VariableNodes.Count > stackSize)
                {
                    // partial rollback, resetting some cells to fixed to jump out of current generation branch
                    while (stack.Count > stackSize / 2)
                    {
                        (Cell, int) tempMove = stack.Pop();
                        tempMove.Item1.ChangeCellValue(tempMove.Item2);
                        board.VariableNodes.Remove(tempMove.Item1);
                        variableNodesCopy.Add(tempMove.Item1);
                    }
                }
                // chooses a random cell from the selection list
                Cell randomCell = variableNodesCopy[random.Next(variableNodesCopy.Count)];
                SetUpInitialSolvingConditions(board, randomCell, stack);
                solver.ChangeMostRecentCell(null);
                solver.HasUniqueSolution();
                // if the board no longer has a unique solution, end the generation process
                if (board.SolutionCount != 1)
                {
                    UndoMove(board, randomCell, stack);
                    solver.ChangeMostRecentCell(null);
                    // gets the single unique solution + stores it in solutions
                    solver.HasUniqueSolution();
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

        private void SetUpInitialSolvingConditions(Board board, Cell randomCell, MoveStack stack)
        {
            stack.Push(randomCell, randomCell.Entry);
            // adds any new potential domains to all affected cells on the board
            AddDomains((randomCell, randomCell.Entry), board);
            // records move
            randomCell.ChangeCellValue(0);
            // adds cell to VariableNodes + initialises queue and MostRecentlyChangedCell for solving
            board.VariableNodes.Add(randomCell);
            board.InitialiseQueue();
        }

        private void UndoMove(Board board, Cell randomCell, MoveStack stack)
        {
            // undoes the previous move, reverting the most recently changed variable node to fixed with a set value
            (Cell, int) previousMove = stack.Pop();
            board.VariableNodes.Remove(randomCell);
            previousMove.Item1.ChangeCellValue(previousMove.Item2);
            ResetBoardProperties(board);
            // initialises queue and MostRecentlyChangedCell for solving
            board.InitialiseQueue();
            RemoveDomains(previousMove, board);
        }

        // used when converting a node from fixed to variable
        private void AddDomains((Cell,int) move, Board board)
        {
            Cell cell = move.Item1;
            int oldEntry = move.Item2;
            if (!move.Item1.Domain.Contains(move.Item2))
            {
                cell.Domain.Add(oldEntry);
            }
            // for each cell that is connected to the altered node
            // if no connected nodes contain the previous value of the altered node
            // add this previous value to its domain
            Parallel.ForEach(board.AdjacencyList[cell], connectedNode =>
            {
                if (board.AdjacencyList[connectedNode].Any(c => c.Entry == oldEntry))
                {
                    connectedNode.Domain.Add(oldEntry);
                }
            });
        }

        // used when converting a node from variable to fixed
        private void RemoveDomains((Cell, int) move, Board board)
        {
            // for each connected node to a cell
            foreach (Cell cell in board.AdjacencyList[move.Item1])
            {
                // if the cell's domain contains the previous, fixed value of the base cell, remove the value from the cell's domain
                if (cell.Domain.Contains(move.Item2))
                {
                    cell.Domain.Remove(move.Item2);
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
