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
                return true;
            }
            Cell node = GetMRV();
            List<(int,int)> orderedDomain = SortByLCV(node);
            for (int i = 0; i < orderedDomain.Count; i++)
            {
                node.Entry = orderedDomain[i].Item2;
                Dictionary<Cell, List<int>> removed = PruneValues(node.Entry, node);   // forward check, pruning the domain early
                if (HasEmptyDomains())  // invalid board in the current state, so no point going deeper into backtracking
                {
                    RestorePrunedValues(removed);  // reverts domain restrictions
                    continue;   // moves onto the next number in the iteration (the "branch" of the tree to the side)
                }
                if (Solve())
                {
                    return true;
                }
                RestorePrunedValues(removed);   // backtrack from forward checking, removing the domain restrictions of the invalid board
            }
            node.Entry = 0;  // backtracking up the tree - resetting the most recently edited cell
            Board.Queue.Enqueue(node);  // adding the reset node back into the priority queue
            return false;
        }

        private Dictionary<Cell, List<int>> PruneValues(int number, Cell node)
        {
            Dictionary<Cell, List<int>> removedNumbers = new();
            List<Cell> changeNodes = Board.AdjacencyList[node];   // all cells that the given node is linked to
            foreach (Cell changeNode in changeNodes)
            {
                if (Board.VariableNodes.Contains(changeNode))  // does not unnecessarily remove from the domain of fixed nodes
                {
                    removedNumbers[changeNode] = new();
                    if (changeNode.Domain.Contains(number))   // if the current value of the node is in the domain of a connected cell
                    {
                        changeNode.Domain.Remove(number);  // remove the value of the connected node from the cell
                        removedNumbers[changeNode].Add(number);  // records the cell which the number has been removed from for later use
                    }
                }
            }
            return removedNumbers;
        }

        private void RestorePrunedValues(Dictionary<Cell, List<int>> removedNumbers)
        {
            foreach (KeyValuePair<Cell, List<int>> pair in removedNumbers)  // recorded cell + number combination
            {
                foreach (int number in pair.Value)
                {
                    if (!pair.Key.Domain.Contains(number)) // if the domain does not already contain the number
                    {
                        pair.Key.Domain.Add(number);  // adds the removed number back into the domain
                    }
                }
            }
        }

        private Cell GetMRV()
        {
            Cell MRVCell = Board.Queue.Dequeue();
            return MRVCell;
        }

        private List<(int,int)> SortByLCV(Cell cell)  
        {
            List<(int,int)> orderedDomain = new();
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
        

        private bool HasEmptyDomains()
        {
            foreach (Cell cell in Board.AdjacencyList.Keys)
            {
                if (cell.Domain.Count == 0)  // if a certain cell has no values that it could take on without violating a Sudoku constraint
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasUniqueSolution()  // does not use a queue as the queue would have to be reset every time the board is checked
        {
            if (CheckInvalid())
            {
                return false;
            }
            if (CheckFinished())
            {
                Board.SolutionCount++; 
                PrintBoard(Board);
                Board tempBoard = Board.Clone(Board);  // objects passed by reference not value, clone needed to store the actual values                 
                Board.Solutions.Add(tempBoard);
                return true;
            }
            Cell node = new Cell((-1, -1), -1);  // arbitrary cell to be assigned to
            for (int i=0; i<Board.VariableNodes.Count; i++)
            {
                if (Board.VariableNodes[i].Entry == 0)  // if cell currently has no value
                {
                    node = Board.VariableNodes[i];
                    break;
                }
            }
            List<(int, int)> orderedDomain = SortByLCV(node);  // iterates through domains by increasing impact
            for (int i = 0; i < orderedDomain.Count; i++)
            {
                node.Entry = orderedDomain[i].Item2;
                Dictionary<Cell, List<int>> removed = PruneValues(node.Entry, node); 
                if (HasEmptyDomains())
                {
                    RestorePrunedValues(removed);
                    continue;
                }
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
            node.Entry = 0;
            return false;
        }
    }
}
