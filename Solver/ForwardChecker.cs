using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class ForwardChecker : BacktrackingSolver
    {
        public ForwardChecker(Board board) : base(board)
        {
        }
        public override bool Solve()
        {
            if (CheckInvalid())
            {
                return false;
            }
            if (CheckFinished())
            {
                PrintBoard(Board);
                Board tempBoard = Board.Clone(Board);  // objects passed by reference not value, clone needed to store the actual values                 
                Board.Solutions.Add(tempBoard);
                return true;
            }
            Cell node = GetMRV();
            List<(int,int)> orderedDomain = SortByLCV(node);
            for (int i = 0; i < orderedDomain.Count; i++)
            {
                node.Entry = orderedDomain[i].Item2;
                Dictionary<Cell, List<int>> removed = RemoveRemainingNumbers(node.Entry, node);   // forward check, pruning the domain early
                if (HasEmptyDomains())  // invalid board in the current state, so no point going deeper into backtracking
                {
                    AddBackRemainingNumbers(removed);
                    continue;   // moves onto the next number in the iteration (the "branch" of the tree to the side)
                }
                if (Solve())
                {
                    return true;
                }
                AddBackRemainingNumbers(removed);   // backtrack from forward checking, removing the domain restrictions of the invalid board
            }
            node.Entry = 0;
            Board.Queue.Enqueue(node);
            return false;
        }

        private Dictionary<Cell, List<int>> RemoveRemainingNumbers(int number, Cell node)
        {
            Dictionary<Cell, List<int>> removedNumbers = new();
            List<Cell> changeNodes = Board.AdjacencyList[node];   // all cells that the given node is linked to
            foreach (Cell changeNode in changeNodes)
            {
                if (Board.VariableNodes.Contains(changeNode))
                {
                    removedNumbers[changeNode] = new();
                    if (changeNode.Domain.Contains(number))   // if the current value of the node is in the domain of a connected cell
                    {
                        changeNode.Domain.Remove(number);
                        removedNumbers[changeNode].Add(number);
                    }
                }
            }
            return removedNumbers;
        }

        private void AddBackRemainingNumbers(Dictionary<Cell, List<int>> removedNumbers)
        {
            foreach (KeyValuePair<Cell, List<int>> pair in removedNumbers)
            {
                foreach (int number in pair.Value)
                {
                    if (!pair.Key.Domain.Contains(number))
                    {
                        pair.Key.Domain.Add(number);
                    }
                }
            }
        }

        private Cell GetMRV()
        {
            Cell MRVCell = Board.Queue.Dequeue();
            return MRVCell;
        }

        private List<(int,int)> SortByLCV(Cell cell)    // if MRV is not 1, run LCV
        {
            List<(int,int)> orderedDomain = new();
            foreach (int number in cell.Domain)
            {
                int impact = 0;
                foreach (Cell connectedNode in Board.AdjacencyList[cell])
                {
                    if (connectedNode.Domain.Contains(number))
                    {
                        impact++;
                    }
                }
                orderedDomain.Add((impact, number));
            }
            orderedDomain.Sort();    // implement sorting algorithm?
            return orderedDomain;
        }
        

        private bool HasEmptyDomains()
        {
            foreach (Cell cell in Board.AdjacencyList.Keys)
            {
                if (cell.Domain.Count == 0)
                {
                    Console.WriteLine($"{cell.Position.Item1},{cell.Position.Item2}");
                    return true;
                }
            }
            return false;
        }

        public void HasUniqueSolution()  // does not use a queue as the queue would have to be reset every time the board is checked
        {
            if (CheckInvalid())
            {
                return;
            }
            if (CheckFinished())
            {
                Board.SolutionCount++;
                PrintBoard(Board);
                Board tempBoard = Board.Clone(Board);  // objects passed by reference not value, clone needed to store the actual values                 
                Board.Solutions.Add(tempBoard);
                return;
            }
            Cell node = new Cell((9, 9), -1);  // arbitrary cell to be assigned to
            for (int i=0; i<Board.VariableNodes.Count; i++)
            {
                if (Board.VariableNodes[i].Entry == 0)  // if cell currently has no value
                {
                    node = Board.VariableNodes[i];
                    break;
                }
            }
            for (int i = 0; i < node.Domain.Count; i++)
            {
                node.Entry = node.Domain[i];
                /*Dictionary<Cell, List<int>> removed = RemoveRemainingNumbers(node.Entry, node);   // incorrectly removing + adding back - RETURN TO THIS LATER
                if (HasEmptyDomains())
                {
                    AddBackRemainingNumbers(removed);
                    continue;
                }
                HasUniqueSolution();
                AddBackRemainingNumbers(removed);*/
                if (Board.SolutionCount >= 2)   // if multiple solutions have already been found, stop early
                {
                    node.Entry = 0;
                    return;
                }
            }
            node.Entry = 0;
            return;
        }
    }
}
