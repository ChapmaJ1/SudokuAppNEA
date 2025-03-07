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
            // if the board is invalid in the current state - backtrack
            if (CheckInvalid(MostRecentlyChangedCell))
            {
                return false;
            }
            // if the board is valid and finished - complete and can be returned
            if (CheckFinished())
            {
                PrintBoard(Board);
                return true;
            }
            // gets MRV node using MRV heuristic
            Cell node = GetMRV();
            // creates a list of domains in order of increasing impact
            List<(int,int)> orderedDomain = SortByLCV(node);
            // iterates through domains by increasing impact
            for (int i = 0; i < orderedDomain.Count; i++)
            {
                node.ChangeCellValue(orderedDomain[i].Item2);
                // prunes domains of cells affected by the change, removing values which are no longer possible
                List<(Cell,int)> removed = PruneValues(node);
                // invalid board in the current state, so no point going deeper into backtracking
                if (EmptyDomains)
                {
                    // reverts domain restrictions
                    RestorePrunedValues(removed);
                    // moves onto the next number in the iteration (the "branch" of the tree to the side)
                    continue; 
                }
                ChangeMostRecentCell(node);
                if (Solve())
                {
                    return true;
                }
                // backtrack from forward checking, removing the domain restrictions of the invalid board
                RestorePrunedValues(removed);
            }
            // backtracks up the tree - resetting the most recently edited cell
            node.ChangeCellValue(0);
            // adds the reset node back into the priority queue
            Board.Queue.Enqueue(node);
            return false;
        }
        
        // queue must be re-initialised every recursive loop, however this makes the algorithm run much faster
        // operates the same as Solve(), except the algorithm only stops after finding 2 solutions
        public bool HasUniqueSolution()
        {
            if (CheckInvalid(MostRecentlyChangedCell))
            {
                return false;
            }
            if (CheckFinished())
            {
                // increment solution count
                Board.SetSolutionCount(Board.SolutionCount + 1);
                PrintBoard(Board);
                // clones the board and adds it to the list of solutions
                // objects passed by reference, so a clone needed to store the actual values   
                Board tempBoard = Board.Clone(Board);              
                Board.Solutions.Add(tempBoard);
                return true;
            }
            Cell node = GetMRV();
            List<(int, int)> orderedDomain = SortByLCV(node);
            for (int i = 0; i < orderedDomain.Count; i++)
            {
                ChangeMostRecentCell(node);
                node.ChangeCellValue(orderedDomain[i].Item2);
                List<(Cell,int)> removed = PruneValues(node);
                // if a cell has no potential values it can take on in future recursive loops without violating Sudoku constraitns
                if (EmptyDomains)
                {
                    RestorePrunedValues(removed);
                    continue;
                }
                // if the series of board cell changes leads to a solution
                if (HasUniqueSolution())
                {
                    // reverts domain restrictions to allow exploration of a different tree branch
                    RestorePrunedValues(removed);
                    // if multiple solutions have now been found, terminate
                    if (Board.SolutionCount >= 2)
                    {
                        return true;
                    }
                    // if this is the first solution to be found, continue iterating through the for loop
                    continue;
                }
                RestorePrunedValues(removed);
            }
            node.ChangeCellValue(0);
            Board.Queue.Enqueue(node);
            return false;
        }

        private List<(Cell, int)> PruneValues(Cell node)
        {
            List<(Cell, int)> removedNumbers = new();
            // all cells that the given node is linked to
            List<Cell> changeNodes = Board.AdjacencyList[node];
            foreach (Cell changeNode in changeNodes)
            {
                // if the current value of the node is in the domain of a connected cell
                if (changeNode.Domain.Contains(node.Entry))
                {
                    // removes the value of the connected node from the cell's domain
                    changeNode.Domain.Remove(node.Entry);
                    // records the cell which the number has been removed from for later use
                    removedNumbers.Add((changeNode, node.Entry));
                    // if an unassigned cell has no potential values it can take on in future recursive loops without violating Sudoku constraints
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
            // iterates through each recorded cell + number combination in the previous loop
            foreach ((Cell, int) pair in removedNumbers)
            {
                // if the domain of the cell does not already contain the stated number
                if (!pair.Item1.Domain.Contains(pair.Item2))
                {
                    // adds the removed number back into the domain
                    pair.Item1.Domain.Add(pair.Item2); 
                }
            }
            EmptyDomains = false;
        }

        private Cell GetMRV()
        {
            // returns MRV cell based on highest priority (lowest domain count)
            Cell MRVCell = Board.Queue.Dequeue();
            return MRVCell;
        }

        private List<(int, int)> SortByLCV(Cell cell)
        {
            // if the cell's domain only has one value it can simply be returned, there is no need for sorting
            if (cell.Domain.Count == 1)
            {
                return new List<(int, int)> { (0, cell.Domain[0]) };
            }
            List<(int, int)> orderedDomain = new();
            // iterates through each number in the cell's domain
            foreach (int number in cell.Domain)
            {
                int impact = 0;
                foreach (Cell connectedNode in Board.AdjacencyList[cell])
                {
                    // records how many cells that are connected to a particular node have a specific number in their domain - the impact
                    if (connectedNode.Domain.Contains(number))
                    {
                        impact++;
                    }
                }
                orderedDomain.Add((impact, number));
            }
            // sorts values by increasing impact on neighouring cells - lowest impact tested first
            orderedDomain.Sort();
            return orderedDomain;
        }
    }
}
