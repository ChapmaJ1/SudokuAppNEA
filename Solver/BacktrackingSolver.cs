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
        public BacktrackingSolver(Board board)
        {
            Board = board;
            VisitedCells = new List<Cell>();
        }

        public virtual bool Solve()
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
            Cell node = new Cell((9, 9), -1);   // new arbitrary cell to be assigned to
            for (int i = 0; i < Board.VariableNodes.Count; i++)
            {
                if (Board.VariableNodes[i].Entry == 0)   // unassigned cell
                {
                    node = Board.VariableNodes[i];
                    break;
                }
            }
            for (int i=1; i<10; i++)
            {
                Console.WriteLine("\n");
                node.Entry = i;   // set board cell equal to a value and backtrack with this value
                PrintBoard(Board);
                if (Solve())
                {
                    return true;
                }
            }
            node.Entry = 0;  // moving back up the tree - resetting the most recently edited cell
            return false;
        }

        public bool CheckFinished()
        {
            foreach (Cell node in Board.AdjacencyList.Keys)
            {
                if (node.Entry == 0)  // no empty values and board is valid (guaranteed by CheckInvalid() function)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckInvalid()
        {
            foreach (KeyValuePair<Cell, List<Cell>> link in Board.AdjacencyList)
            {
                foreach (Cell node in link.Value)
                {
                    if (link.Key.Entry == node.Entry && link.Key.Entry != 0)   // 2 linked nodes share the same, non-empty value
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void PrintBoard(Board board)
        {
            foreach (Cell cell in board.AdjacencyList.Keys)
            {
                board.BoardSketch[cell.Position.Item1, cell.Position.Item2] = cell.Entry;   // update board sketch to reflect the numerical entries of the cell objects
            }
            for (int i=0; i<board.BoardSketch.GetLength(0); i++)
            {
                for (int j=0; j<board.BoardSketch.GetLength(0); j++)
                {
                    Console.Write(board.BoardSketch[i, j] + " ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}