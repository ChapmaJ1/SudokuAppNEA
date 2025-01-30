using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class ForwardChecker : BacktrackingSolver
    {
        public bool EmptyDomains { get; private set; }
        public ForwardChecker(Board board) : base(board)
        {
            EmptyDomains = false;
        }
        public override bool Solve()
        {
            if (CheckInvalid(MostRecentlyChangedCell))
            {
                return false;
            }
            if (CheckFinished())
            {
                PrintBoard(Board);
                return true;
            }
            Cell node = GetMRV();
            List<(int,int)> orderedDomain = SortByLCV(node);
            for (int i = 0; i < orderedDomain.Count; i++)
            {
                node.ChangeCellValue(orderedDomain[i].Item2);
                List<(Cell,int)> removed = PruneValues(node.Entry, node);   // forward check, pruning the domain early
                if (EmptyDomains)  // invalid board in the current state, so no point going deeper into backtracking
                {
                    RestorePrunedValues(removed);  // reverts domain restrictions
                    continue;   // moves onto the next number in the iteration (the "branch" of the tree to the side)
                }
                ChangeMostRecentCell(node);
                if (Solve())
                {
                    return true;
                }
                RestorePrunedValues(removed);   // backtrack from forward checking, removing the domain restrictions of the invalid board
            }
            node.ChangeCellValue(0);  // backtracking up the tree - resetting the most recently edited cell
            Board.Queue.Enqueue(node);  // adding the reset node back into the priority queue
            return false;
        }

        public bool HasUniqueSolution()  // does not use a queue as the queue would have to be reset every time the board is checked
        {
            if (CheckInvalid(MostRecentlyChangedCell))
            {
                return false;
            }   // WHAT HAPPENS IF THERE ARE EMPTY DOMAINS IN STARTING STATE??
            if (CheckFinished())
            {
                Board.SetSolutionCount(Board.SolutionCount + 1);
                PrintBoard(Board);
                Board tempBoard = Board.Clone(Board);  // objects passed by reference not value, clone needed to store the actual values                 
                Board.Solutions.Add(tempBoard);
                return true;
            }
            Cell node = GetMRV();
            List<(int, int)> orderedDomain = SortByLCV(node);  // iterates through domains by increasing impact
            for (int i = 0; i < orderedDomain.Count; i++)
            {
                node.ChangeCellValue(orderedDomain[i].Item2);
                List<(Cell,int)> removed = PruneValues(node.Entry, node); 
                if (EmptyDomains)
                {
                    RestorePrunedValues(removed);
                    continue;
                }
                ChangeMostRecentCell(node);
                if (HasUniqueSolution())
                {
                    RestorePrunedValues(removed);  // reverts domain restrictions to allow exploration of a different tree branch
                    if (Board.SolutionCount >= 2)   // if multiple solutions have already been found, stop early
                    {
                        return true;
                    }
                    continue;  // if this is the first solution to be found, continue iterating through the for loop
                }
                RestorePrunedValues(removed);
            }
            node.ChangeCellValue(0);
            Board.Queue.Enqueue(node);
            return false;
        }

        private List<(Cell, int)> PruneValues(int number, Cell node)
        {
            List<(Cell, int)> removedNumbers = new();
            List<Cell> changeNodes = Board.AdjacencyList[node];   // all cells that the given node is linked to
            foreach (Cell changeNode in changeNodes)
            {
                if (changeNode.Domain.Contains(number))   // if the current value of the node is in the domain of a connected cell
                {
                    changeNode.Domain.Remove(number);  // remove the value of the connected node from the cell
                    removedNumbers.Add((changeNode, number));  // records the cell which the number has been removed from for later use
                    if (changeNode.Domain.Count == 0)
                    {
                        EmptyDomains = true;
                    }
                }
            }
            return removedNumbers;
        }

        private void RestorePrunedValues(List<(Cell, int)> removedNumbers)
        {
            foreach ((Cell, int) pair in removedNumbers)  // recorded cell + number combination
            {
                if (!pair.Item1.Domain.Contains(pair.Item2)) // if the domain does not already contain the number
                {
                    pair.Item1.Domain.Add(pair.Item2);  // adds the removed number back into the domain
                }
            }
            EmptyDomains = false;
        }

        private Cell GetMRV()
        {
            Cell MRVCell = Board.Queue.Dequeue();  // gets MRV cell based on highest priority (lowest domain count)
            return MRVCell;
        }

        private List<(int, int)> SortByLCV(Cell cell)
        {
            if (cell.Domain.Count == 1)
            {
                return new List<(int, int)> { (0, cell.Domain[0]) };
            }
            List<(int, int)> orderedDomain = new();
            foreach (int number in cell.Domain)
            {
                int impact = 0;
                foreach (Cell connectedNode in Board.AdjacencyList[cell])
                {
                    if (connectedNode.Domain.Contains(number))  // records how many cells that are connected to a particular node have a specific number in their domain
                    {
                        impact++;
                    }
                }
                orderedDomain.Add((impact, number));
            }
            orderedDomain.Sort();    // sorts by increasing impact - lowest impact tested first
            return orderedDomain;
        }
    }
}
