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
        public string[,] BoardSketch { get; private set; }
        public List<Cell> VariableNodes { get; private set; }
        public Dictionary<Cell, List<Cell>> AdjacencyList { get; private set; }
        public HeapPriorityQueue Queue { get; private set; }
        public int Dimensions { get; private set; }

        public Board(string difficulty, string[,] boardSketch, int dimensions)
        {
            Difficulty = difficulty;
            Solutions = new List<Board>();
            BoardSketch = boardSketch;
            // a dictionary of nodes (key) and the cells which the node links to via an edge (value)
            AdjacencyList = new Dictionary<Cell, List<Cell>>();
            VariableNodes = new List<Cell>();
            Dimensions = dimensions;
        }

        public void InitialiseGraph()
        {
            for (int i = 0; i < Dimensions; i++)
            {
                for (int j = 0; j < Dimensions; j++)
                {
                    string entry = "";
                    // omits the 'v' character from the end of the string if required
                    if (BoardSketch[i, j].Contains('v'))
                    {
                        entry = BoardSketch[i, j].Substring(0, BoardSketch[i, j].Length - 1);
                    }
                    else
                    {
                        entry = BoardSketch[i, j];
                    }
                    // initialises all cells with their location and initial entry
                    Cell cell = new Cell((i, j), Convert.ToInt32(entry));
                    cell.InitialiseDomain(Dimensions);
                    // creates a dictionary entry for every cell on the board
                    AdjacencyList.Add(cell, new List<Cell>());
                    // if the 'v' character is present, it represents the cell being variable
                    if (BoardSketch[i, j].Contains('v'))
                    {
                        VariableNodes.Add(cell);
                    }
                }
            }
            // adds the cells which each node links to to the dictionary
            foreach (Cell cell in AdjacencyList.Keys)
            {
                AddEdges(cell);
            }
            // initialises the numbers which each cell could potentially take on, based on the values of fixed nodes on the board
            InitialiseStartingDomains(GetFixedNodes());
        }

        public void InitialiseQueue()
        {
            // adds all variable nodes to the priority queue
            Queue = new HeapPriorityQueue(VariableNodes);
            foreach (Cell cell in VariableNodes)
            {
                Queue.Enqueue(cell);
            }
        }

        public void SetSolutionCount(int count)
        {
            SolutionCount = count;
        }

        public void Reset()
        {
            foreach (Cell cell in VariableNodes)
            {
                // sets all cells that were not part of the fixed starting arrangement to empty, with a value of 0
                cell.ChangeCellValue(0);
            } 
        }
        internal Cell GetCellLocation(int x, int y)
        {
            // returns a cell based on its (x,y) coordinates on the board
            foreach (Cell cell in AdjacencyList.Keys)
            {
                if (cell.Position == (x, y))
                {
                    return cell;
                }
            }
            throw new InvalidOperationException("Cell does not exist");
        }

        // creates a copy of an input board
        // when a solution is added to the solutions list, it cannot be changed by future operations as the new object has its own reference
        internal Board Clone(Board inputBoard)
        {
            // creates a new board with identical structure
            Board newBoard = new Board("", new string[inputBoard.BoardSketch.GetLength(1), inputBoard.BoardSketch.GetLength(0)], inputBoard.Dimensions);
             // clones the board sketch of the original input board by the value of each index
            for (int i = 0; i < newBoard.BoardSketch.GetLength(0); i++)
            {
                for (int j = 0; j < newBoard.BoardSketch.GetLength(1); j++)
                {
                    newBoard.BoardSketch[i, j] = inputBoard.BoardSketch[i, j];
                }
            }
            return newBoard;
        }

        private void AddEdges(Cell cell)
        {
            int squareDimensions = Convert.ToInt32(Math.Sqrt(Dimensions));
            // correspond to the "first" and "second" dimensions of a typical array (i,j)
            int iDimension = cell.Position.Item1;
            int jDimension = cell.Position.Item2;
            // correspond to different sub-boxes, in terms of the "first" and "second" dimensions
            int boxI = iDimension / squareDimensions;
            int boxJ = jDimension / squareDimensions;

            for (int i = 0; i < Dimensions; i++)
            {
                // ensures that a cell is not shown to link to itself
                if (i != jDimension) 
                {
                    // finds all the squares in the same row and adds them to the dictionary of the given key
                    AdjacencyList[cell].Add(GetCellLocation(iDimension, i));
                }
            }
            for (int i = 0; i < Dimensions; i++)
            {
                if (i != iDimension)
                {
                    // finds all the squares in the same column and adds them to the dictionary of the given key
                    AdjacencyList[cell].Add(GetCellLocation(i, jDimension));
                }
            }
            for (int i = 0; i < squareDimensions; i++)
            {
                for (int j = 0; j < squareDimensions; j++)
                {
                    // finds which sub-box the square lies in, and therefore the corresponding nodes it must be linked to
                    int addI = i + boxI * squareDimensions;
                    int addJ = j + boxJ * squareDimensions;
                    // prevents the cell linking to itself as well as duplicate linking (the same cell added to the value dictionary twice)
                    if (!(addI == iDimension && addJ == jDimension) && !AdjacencyList[cell].Contains(GetCellLocation(addI, addJ)))
                    {
                        AdjacencyList[cell].Add(GetCellLocation(addI, addJ));
                    }
                }
            }
        }

        private List<Cell> GetFixedNodes()
        {
            List<Cell> fixedNodes = new List<Cell>();
            // finds all cells that are assigned in the starting board - cannot be changed during solving process. Call this a "fixed node"
            foreach (Cell cell in AdjacencyList.Keys)
            {
                if (!VariableNodes.Contains(cell)) 
                {
                    fixedNodes.Add(cell);
                }
            }
            return fixedNodes;
        }

        private void InitialiseStartingDomains(List<Cell> fixedNodes)
        {
            for (int i = 0; i < fixedNodes.Count; i++)
            {
                // for each cell on the board
                foreach (KeyValuePair<Cell, List<Cell>> pair in AdjacencyList)
                {
                    foreach (Cell cell in pair.Value)
                    {
                        // indicates a fixed starting node
                        if (!VariableNodes.Contains(cell)) 
                        {
                            // removes values of fixed starting numbers from the domain of a certain square that links to a fixed node
                            // if a fixed node has some value, no other connected nodes can take on that same value in any valid board state
                            pair.Key.Domain.Remove(cell.Entry);
                        }
                    }
                }
            }
        }
    }
}
