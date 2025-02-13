using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class BacktrackingSolver
    {
        public Board Board { get; private set; }
        public List<Cell> VisitedCells { get; private set; }
        public Cell MostRecentlyChangedCell { get; private set; }
        public BacktrackingSolver(Board board)
        {
            Board = board;
            VisitedCells = new List<Cell>();
            MostRecentlyChangedCell = null;
        }

        public virtual bool Solve()
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
            Cell node = null;   // new arbitrary cell to be assigned to
            for (int i = 0; i < Board.VariableNodes.Count; i++)
            {
                if (Board.VariableNodes[i].Entry == 0)   // cell which is currently empty + has no value
                {
                    node = Board.VariableNodes[i];
                    break;
                }
            }
            if (node != null)
            {
                for (int i = 1; i <= Board.Dimensions; i++)
                {
                    node.ChangeCellValue(i);   // sets board cell equal to a value and performs backtracking tree traversal with this value
                    ChangeMostRecentCell(node);
                    if (Solve())  // recursive loop - if the series of board cell changes leads to a solution, return true
                    {
                        return true;
                    }
                }
                node.ChangeCellValue(0);  // backtracking back up the tree - resetting the most recently edited cell
            }
            return false;
        }

        public bool CheckInvalidFull()
        {
            foreach (KeyValuePair<Cell, List<Cell>> link in Board.AdjacencyList)
            {
                foreach (Cell node in link.Value)
                {
                    if (link.Key.Entry == node.Entry && link.Key.Entry != 0)   // 2 linked nodes share the same, non-empty value, hence violating the Sudoku constraint
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckFinished()
        {
            foreach (Cell node in Board.AdjacencyList.Keys)
            {
                if (node.Entry == 0)  // checks that the board has no empty values
                {                     // validity of the board in terms of Sudoku constraints is guaranteed by the CheckInvalid() function
                    return false;
                }
            }
            return true;
        }

        internal void ChangeMostRecentCell(Cell cell)
        {
            MostRecentlyChangedCell = cell;
        }
        protected bool CheckInvalid(Cell cell)
        {
            if (cell == null)
            {
                return false;
            }
            foreach (Cell connectedNode in Board.AdjacencyList[cell])
            {
                if (connectedNode.Entry == cell.Entry)
                {
                    return true;
                }
            }
            return false;
        }

        protected void PrintBoard(Board board)
        {
            foreach (Cell cell in board.AdjacencyList.Keys)
            {
                board.BoardSketch[cell.Position.Item1, cell.Position.Item2] = cell.Entry.ToString();   // updates board sketch to reflect the numerical entries of the cell objects
            }                                                                               // board sketch can then be used for output
          /*  for (int i=0; i<board.BoardSketch.GetLength(0); i++)
            {
                for (int j=0; j<board.BoardSketch.GetLength(0); j++)
                {
                    Console.Write(board.BoardSketch[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();  */
        }
    }
}