using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class ForwardChecker : BacktrackingSolver
    {
        public List<Cell> VisitedNodes { get; private set; }
        public HeapPriorityQueue Queue { get; private set; }
        public ForwardChecker(Board board, List<Cell> variableNodes, HeapPriorityQueue queue) : base(board, variableNodes)
        {
            VisitedNodes = new();
            Queue = queue;
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
            VisitedNodes.Add(node);
            List<(int,int)> orderedDomain = SortByLCV(node);
            for (int i = 0; i < orderedDomain.Count; i++)
            {
                node.Entry = orderedDomain[i].Item2;
                //PrintBoard(Board);
                //Console.WriteLine("\n");
                Dictionary<Cell, List<int>> removed = RemoveRemainingNumbers(node.Entry, node);   // forward check, pruning the domain early
                /*if (HasEmptyDomains())  // invalid board in the current state, so no point going deeper into backtracking
                {
                    AddBackRemainingNumbers(removed);
                    continue;
                }*/
                if (Solve())
                {
                    return true;
                }
                AddBackRemainingNumbers(removed);   // backtrack from forward checking, removing the domain restrictions of the invalid board
            }
            VisitedNodes.Remove(node);  // backtrack when a node in VisitedNodes has 0 remaining nodes
            node.Entry = 0;
            Queue.Enqueue(node);
            return false;
        }

        private Dictionary<Cell, List<int>> RemoveRemainingNumbers(int number, Cell node)
        {
            Dictionary<Cell, List<int>> removedNumbers = new();
            List<Cell> changeNodes = Board.AdjacencyList[node];   // all cells that the given node is linked to
            foreach (Cell changeNode in changeNodes)
            {
                removedNumbers[changeNode] = new();
                if (changeNode.Domain.Contains(number))   // if the current value of the node is in the domain of a connected cell
                {
                    changeNode.Domain.Remove(number);
                    removedNumbers[changeNode].Add(number);
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
                    pair.Key.Domain.Add(number);
                }
            }
        }

        private Cell GetMRV()
        {
            Cell MRVCell = Queue.Dequeue();
            return MRVCell;
        }
        /*private List<Cell> GetMRV()    // change to calculation algorithm using priority queue, updating it each iteration
        {
            Dictionary<int, List<Cell>> domainCounts = new Dictionary<int, List<Cell>>();
            for (int i=1; i<10; i++)
            {
                domainCounts[i] = new List<Cell>();
            }
            foreach (Cell cell in Board.VariableNodes)
            {
                if (cell.Domain.Count != 0)
                {
                    domainCounts[cell.Domain.Count].Add(cell);
                }
            }
            foreach (int number in domainCounts.Keys)
            {
                if (domainCounts[number].Count != 0)
                {
                    return domainCounts[number];
                }
            }
            return new List<Cell>();
        }*/

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
                    return true;
                }
            }
            return false;
        }

        public void HasUniqueSolution()
        {
            if (CheckInvalid())
            {
                return;
            }
            if (CheckFinished())
            {
                PrintBoard(Board);
                Console.WriteLine("\n");
                Board tempBoard = Board.Clone(Board);   // objects passed by reference not value, clone needed to store the actual values                 
                Board.Solutions.Add(tempBoard);
                return;
            }
            Cell node = new Cell((9, 9), -1);
            for (int i = 0; i < VariableNodes.Count; i++)
            {
                if (VariableNodes[i].Entry == 0)
                {
                    node = VariableNodes[i];
                    break;
                }
            }
            VisitedNodes.Add(node);
            for (int i = 0; i < node.Domain.Count; i++)
            {
                node.Entry = node.Domain[i];
                Dictionary<Cell, List<int>> removed = RemoveRemainingNumbers(node.Entry, node);
                HasUniqueSolution();
                AddBackRemainingNumbers(removed);
            }
            //VisitedNodes.Remove(node);  // backtrack when a node in VisitedNodes has 0 remaining nodes - come back
            //Board.BoardSketch[node.Item2, node.Item1] = 0;
            node.Entry = 0;
            return;
        }
    }
}
