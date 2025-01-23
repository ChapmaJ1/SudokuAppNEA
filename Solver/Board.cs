using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class Board
    {
        public string Difficulty { get; private set; }
        public List<Board> Solutions { get; private set; }
        public int SolutionCount { get; private set; }
        public int[,] BoardSketch { get; private set; }
        public List<Cell> VariableNodes { get; private set; }
        public Dictionary<Cell, List<Cell>> AdjacencyList { get; private set; }
        public HeapPriorityQueue Queue { get; private set; }
        public int Dimensions { get; private set; }

        public Board(string difficulty, int[,] boardSketch, int dimensions)
        {
            Difficulty = difficulty;
            Solutions = new List<Board>();
            BoardSketch = boardSketch;
            AdjacencyList = new Dictionary<Cell, List<Cell>>();  // a dictionary of nodes (key) and the cells which the node links to via an edge (value)
            VariableNodes = new List<Cell>();
            Dimensions = dimensions;
        }

        public void InitialiseGraph()
        {
            int squareDimensions = Convert.ToInt32(Math.Sqrt(Dimensions));
            for (int i = 0; i < Dimensions; i++)
            {
                for (int j = 0; j < Dimensions; j++)
                {
                    Cell cell = new Cell((i, j), BoardSketch[i, j]);  // initialise all cells with their location and initial entry
                    cell.InitialiseDomain(Dimensions);
                    AdjacencyList.Add(cell, new List<Cell>());  // creates dictionary entry for every cell on the board
                }
            }
            foreach (Cell cell in AdjacencyList.Keys)
            {
                AddEdges(cell);  // adds the cells which each node links to to the dictionary
            }
            InitialiseRemainingNumbers(GetFixedNodes());
            SetVariableNodes();
        }

        public void SetQueue()
        {
            Queue = new HeapPriorityQueue(VariableNodes, BoardSketch.GetLength(0));
            foreach (Cell cell in VariableNodes)
            {
                Queue.Enqueue(cell);
            }
        }

        private void AddEdges(Cell cell)
        {
            int squareDimensions = Convert.ToInt32(Math.Sqrt(Dimensions));
            int iDimension = cell.Position.Item1;
            int jDimension = cell.Position.Item2;   // correspond to the "first" and "second" dimensions of a typical array
            int boxI = iDimension / squareDimensions;
            int boxJ = jDimension / squareDimensions;   // correspond to different boxes, in terms of the "first" and "second" dimensions

            for (int i = 0; i < Dimensions; i++)
            {
                if (i != jDimension)  // ensures that a cell is not shown to link to itself
                {
                    AdjacencyList[cell].Add(GetCellLocation(iDimension, i));   // finds all the squares in the same row
                }
            }
            for (int i = 0; i < Dimensions; i++)
            {
                if (i != iDimension)
                {
                    AdjacencyList[cell].Add(GetCellLocation(i, jDimension));  // finds all the squares in the same column
                }
            }
            for (int i = 0; i < squareDimensions; i++)
            {
                for (int j = 0; j < squareDimensions; j++)
                {
                    int addI = i + boxI * squareDimensions;
                    int addJ = j + boxJ * squareDimensions;  // finds which box the square lies in, and the corresponding nodes it must be linked to
                    if (!(addI == iDimension && addJ == jDimension) && !AdjacencyList[cell].Contains(GetCellLocation(addI, addJ)))  // prevents the cell linking to itself + duplicate linking (linking to the same cell twice)
                    {
                        AdjacencyList[cell].Add(GetCellLocation(addI, addJ));
                    }
                }
            }
        }

        public Cell GetCellLocation(int x, int y)
        {
            foreach (Cell cell in AdjacencyList.Keys)
            {
                if (cell.Position == (x, y))
                {
                    return cell;
                }
            }
            throw new InvalidOperationException("Cell does not exist");
        }

        public List<Cell> GetFixedNodes()
        {
            List<Cell> fixedNodes = new List<Cell>();
            foreach (Cell cell in AdjacencyList.Keys)
            {
                if (cell.Entry != 0)    // assigned in the starting board - cannot be changed during solving process. Call this a "fixed node"
                {
                    fixedNodes.Add(cell);
                }
            }
            return fixedNodes;
        }

        private void SetVariableNodes()
        {
            foreach (Cell cell in AdjacencyList.Keys)
            {
                if (cell.Entry == 0)  // not assigned in the starting board - can be changed during solving process. Call this a "variable node"
                {
                    VariableNodes.Add(cell);
                }
            }
        }

        public void InitialiseRemainingNumbers(List<Cell> fixedNodes)
        {
            for (int i = 0; i < fixedNodes.Count; i++)
            {
                foreach (KeyValuePair<Cell, List<Cell>> pair in AdjacencyList)
                {
                    foreach (Cell cell in pair.Value)
                    {
                        if (cell.Entry != 0)
                        {
                            pair.Key.Domain.Remove(cell.Entry);  // removes values of fixed starting numbers from the domain of a certain square that links to a fixed node
                        }
                    }
                }
            }
        }

        public Board Clone(Board inputBoard)
        {
            Board newBoard = new Board("", new int[inputBoard.BoardSketch.GetLength(1), inputBoard.BoardSketch.GetLength(0)], inputBoard.Dimensions);  // creates a new board with identical structure
            for (int i=0; i<newBoard.BoardSketch.GetLength(0); i++)
            {
                for (int j=0; j<newBoard.BoardSketch.GetLength(1); j++)
                {
                    newBoard.BoardSketch[i, j] = inputBoard.BoardSketch[i, j];  // clones the board sketch of the original input board
                }
            }
            return newBoard;
        }

        public void Reset()
        {
            foreach (Cell cell in VariableNodes)
            {
                cell.ChangeCellValue(0);   // sets all cells that were not part of the fixed starting arrangement to empty
            } 
        }

        public void SetSolutionCount(int count)
        {
            SolutionCount = count;
        }
    }
}
