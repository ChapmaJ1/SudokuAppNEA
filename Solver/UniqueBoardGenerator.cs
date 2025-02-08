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
        public async Task<Board> GenerateUniqueSolution(int dimensions, Board board)
        {
            bool validBoard = false;
            MoveStack stack = new MoveStack(20);
            List<Cell> variableNodesCopy = new();
            Random random = new Random();
            ForwardChecker solver = new ForwardChecker(board);
            board.InitialiseQueue();
            await solver.HasUniqueSolution();
            if (board.SolutionCount == 1)
            {
                return board;
            }
            else
            {
                board.Reset();
                board.InitialiseQueue();
                board.Solutions.Clear();
                board.SetSolutionCount(0);
            }
            solver.Solve();   // fills all cells with valid initial conditions
            foreach (Cell cell in board.VariableNodes)
            {
                variableNodesCopy.Add(cell);
            }
            board.VariableNodes.Clear();  // solver interacts with the board's variable nodes when solving - remove all + add gradually
          //  RemoveInitialNumbers(dimensions, variableNodesCopy, board);
            while (!validBoard)
            {
                if (board.VariableNodes.Count > 100)    // generation gets exponentially slower after this point, experimentally
                {
                    while (stack.Count > 0)
                    {
                        Move tempMove = stack.Pop();
                        tempMove.Cell.ChangeCellValue(tempMove.OldEntry);
                        board.VariableNodes.Remove(tempMove.Cell);
                    }
                }
                Cell randomCell = variableNodesCopy[random.Next(variableNodesCopy.Count)];
                Move move = new(randomCell, randomCell.Entry);
                AddDomains(move, board);
                stack.Push(move);
                randomCell.ChangeCellValue(0);
                board.VariableNodes.Add(randomCell);
                board.InitialiseQueue();
                solver.ChangeMostRecentCell(null);
                await solver.HasUniqueSolution();
                if (board.SolutionCount != 1)   // function returns true when solution is not unique, so this checks if the solution is not unique
                {
                    Move previousMove = stack.Pop();
                    board.VariableNodes.Remove(randomCell);
                    previousMove.Cell.ChangeCellValue(previousMove.OldEntry);
                    ResetBoardProperties(board);
                    board.InitialiseQueue();
                    RemoveDomains(previousMove, board);
                    solver.ChangeMostRecentCell(null);
                    await solver.HasUniqueSolution();  // gets single unique solution + stores it in solutions
                    validBoard = true;
                }
                else   // if solution not unique, keep increasing the number of variable nodes 
                {
                    variableNodesCopy.Remove(randomCell);
                    ResetBoardProperties(board);
                }
            }
            return board;
        }

        private void AddDomains(Move move, Board board)   // faulty, could use CheckInvalidFull instead
        {
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
        private void RemoveDomains(Move move, Board board)
        {
            foreach (Cell cell in board.AdjacencyList[move.Cell])
            {
                if (cell.Domain.Contains(move.OldEntry))
                {
                    cell.Domain.Remove(move.OldEntry);
                }
            }
        }

        private void RemoveInitialNumbers(int dimensions, List<Cell> variableNodesCopy, Board board)
        {
            ForwardChecker solver = new(board);
            Random random = new Random();
            int cap = 80;
            if (dimensions == 9)
            {
                cap = 20;
            }
            while (board.VariableNodes.Count < cap)
            {
                int randomCellIndex = random.Next(variableNodesCopy.Count);
                Cell randomCell = variableNodesCopy[randomCellIndex];
                AddDomains(new Move(randomCell, randomCell.Entry), board);
                randomCell.ChangeCellValue(0);
                variableNodesCopy.Remove(randomCell);
                board.VariableNodes.Add(randomCell);
            }
        }

        private void ResetBoardProperties(Board board)
        {
            board.SetSolutionCount(0);
            board.Solutions.Clear();
            board.Reset();
        }
    }
}
