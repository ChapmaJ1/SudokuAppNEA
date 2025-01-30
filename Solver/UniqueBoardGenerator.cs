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
        public Board GenerateUniqueSolution(int dimensions, Board board)  // if 1.5s passed since starting, reset
        {
            bool validBoard = false;
            MoveStack stack = new MoveStack(5);
            List<Cell> variableNodesCopy = new();
            Random random = new Random();
            board.InitialiseQueue();
            ForwardChecker solver = new ForwardChecker(board);
            if (solver.HasUniqueSolution())
            {
                return board;
            }
            foreach (Cell cell in board.VariableNodes)
            {
                variableNodesCopy.Add(cell);
            }
            board.VariableNodes.Clear();  // solver interacts with the board's variable nodes when solving - remove all + add gradually
            RemoveInitialNumbers(dimensions, variableNodesCopy, board);
            while (!validBoard)
            {
                Cell randomCell = variableNodesCopy[random.Next(variableNodesCopy.Count)];
                Move move = new(randomCell, randomCell.Entry);
                AddDomains(move, board);
                stack.Push(move);
                randomCell.ChangeCellValue(0);
                board.VariableNodes.Add(randomCell);
                board.InitialiseQueue();
                solver.ChangeMostRecentCell(null);
                bool unique = solver.HasUniqueSolution();
                if (!unique)   // weird notation bc function returns true when board is not unique 
                {
                    Move previousMove = stack.Pop();
                    board.VariableNodes.Remove(randomCell);
                    previousMove.Cell.ChangeCellValue(previousMove.OldEntry);
                    board.Reset();
                    RemoveDomains(previousMove, board);
                    validBoard = true;
                }
                else
                {
                    variableNodesCopy.Remove(randomCell);
                    board.SetSolutionCount(0);
                    board.Solutions.Clear();
                }
            }
            return board;
        }

        private void AddDomains(Move move, Board board)
        {
            move.Cell.Domain.Add(move.OldEntry);
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
            while (board.VariableNodes.Count <= cap)
            {
                int randomCellIndex = random.Next(variableNodesCopy.Count);
                Cell randomCell = variableNodesCopy[randomCellIndex];
                AddDomains(new Move(randomCell, randomCell.Entry), board);
                randomCell.ChangeCellValue(0);
                variableNodesCopy.Remove(randomCell);
                board.VariableNodes.Add(randomCell);
            }
        }
    }
}
