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
                    // partial rollback to jump out of current generation branch
                    while (stack.Count > stackSize / 2)
                    {
                        Move tempMove = stack.Pop();
                        tempMove.Cell.ChangeCellValue(tempMove.OldEntry);
                        board.VariableNodes.Remove(tempMove.Cell);
                        variableNodesCopy.Add(tempMove.Cell);
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
            Move move = new(randomCell, randomCell.Entry);
            // adds any new potential domains to all affected cells on the board
            AddDomains(move, board);
            // records move
            stack.Push(move);
            randomCell.ChangeCellValue(0);
            // adds cell to VariableNodes + initialises queue and MostRecentlyChangedCell for solving
            board.VariableNodes.Add(randomCell);
            board.InitialiseQueue();
        }

        private void UndoMove(Board board, Cell randomCell, MoveStack stack)
        {
            // undoes the previous move, reverting the most recently changed variable node to fixed with a set value
            Move previousMove = stack.Pop();
            board.VariableNodes.Remove(randomCell);
            previousMove.Cell.ChangeCellValue(previousMove.OldEntry);
            ResetBoardProperties(board);
            // initialises queue and MostRecentlyChangedCell for solving
            board.InitialiseQueue();
            RemoveDomains(previousMove, board);
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
            // a foreach loop where different iterations can be run in parallel
            Parallel.ForEach(board.AdjacencyList[move.Cell], connectedNode =>
            {
                if (board.AdjacencyList[connectedNode].Any(c => c.Entry == move.OldEntry))
                {
                    connectedNode.Domain.Add(move.OldEntry);
                }
            });
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
