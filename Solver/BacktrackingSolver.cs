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
            // new arbitrary cell to be assigned to
            Cell node = null;
            // selects a cell which is currently empty + has no value
            for (int i = 0; i < Board.VariableNodes.Count; i++)
            {
                if (Board.VariableNodes[i].Entry == 0)
                {
                    node = Board.VariableNodes[i];
                    break;
                }
            }
            if (node != null)
            {
                for (int i = 1; i <= Board.Dimensions; i++)
                {
                    // sets board cell equal to a certain value and performs backtracking tree traversal with this value
                    node.ChangeCellValue(i);
                    ChangeMostRecentCell(node);
                    // recursive loop - if the series of board cell changes leads to a solution, return true
                    if (Solve())
                    {
                        return true;
                    }
                }
                // backtracks back up the tree by resetting the most recently edited cell
                node.ChangeCellValue(0);
            }
            return false;
        }

        public bool CheckInvalidFull()
        {
            // invalid board state if 2 linked nodes share the same, non-empty value, violating the Sudoku constraint
            foreach (KeyValuePair<Cell, List<Cell>> link in Board.AdjacencyList)
            {
                foreach (Cell node in link.Value)
                {
                    if (link.Key.Entry == node.Entry && link.Key.Entry != 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckFinished()
        {
            // checks that the board has no cells with empty values
            // the validity of the board in terms of Sudoku constraints is guaranteed by the CheckInvalid() function
            foreach (Cell node in Board.AdjacencyList.Keys)
            {
                if (node.Entry == 0)
                {                     
                    return false;
                }
            }
            return true;
        }

        // same functionality as CheckInvalidFull(), but speeds up the process by only checking nodes linked to the last changed cell
        public bool CheckInvalid(Cell cell)
        {
            // if null no cells have been changed yet
            if (cell == null)
            {
                return false;
            }
            // if a node shares the same, non-empty value with a connected node
            foreach (Cell connectedNode in Board.AdjacencyList[cell])
            {
                if (connectedNode.Entry == cell.Entry)
                {
                    return true;
                }
            }
            return false;
        }

        internal void ChangeMostRecentCell(Cell cell)
        {
            MostRecentlyChangedCell = cell;
        }

        protected void PrintBoard(Board board)
        {
            foreach (Cell cell in board.AdjacencyList.Keys)
            {
                // updates board sketch to reflect the numerical entries of the cell objects
                board.BoardSketch[cell.Position.Item1, cell.Position.Item2] = cell.Entry.ToString();
            } 
        }
    }
}