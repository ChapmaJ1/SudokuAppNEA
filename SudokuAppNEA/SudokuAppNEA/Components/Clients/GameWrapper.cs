using Microsoft.AspNetCore.Components;
using Sudoku_Solver_NEA;

namespace SudokuAppNEA.Components.Clients
{
    public class GameWrapper : ComponentBase // splits the frontend + backend functionality for enhanced modularity
    {
        internal Board Board { get; private set; }
        internal int[,] CorrectNumbers { get; private set; }
        internal GameWrapper(Board board, int[,] correctNumbers)
        {
            Board = board;
            CorrectNumbers = correctNumbers;
        }

        internal void SolveBoard()
        {
            foreach (Cell cell in Board!.VariableNodes)
            {
                cell.ChangeCellValue(CorrectNumbers![cell.Position.Item1, cell.Position.Item2]); // sets the value of all cells to the correct value, ensuring they are rendered correctly 
            }
        }

        internal int ChangeCellValue(int number, MoveStack moveStack, Cell selectedCell, string noteTaking)
        {
            if (noteTaking == "Off")
            {
                if (!(selectedCell == null))
                {
                    Move move = new Move(selectedCell, selectedCell!.Entry);
                    moveStack.Push(move);  // records move + pushes it onto move stack
                    selectedCell.ChangeCellValue(number);
                    if (number != CorrectNumbers![selectedCell.Position.Item1, selectedCell.Position.Item2])  // if the input number is incorrect
                    {
                        return 1;
                    }
                }
            }
            else  // note taking mode is on
            {
                if (selectedCell!.NoteEntries.Contains(number))  // if number already noted, remove it from the list
                {
                    selectedCell.NoteEntries.Remove(number);
                }
                else
                {
                    selectedCell.NoteEntries.Add(number);  // if number not already noted, add it to the list
                }
            }
            return 0;
        }

        internal Cell OnCellSelected(int row, int column)
        {
            foreach (Cell cell in Board!.VariableNodes)  // finds the cell selected by the user
            {
                if (cell.Position.Item1 == row && cell.Position.Item2 == column)
                {
                    return cell;
                }
            }
            return null!;   // represents fixed node that is selected
        }

        internal string DisplayCell(int row, int column)
        {
            foreach (Cell cell in Board!.AdjacencyList.Keys)
            {
                if (cell.Position.Item1 == row && cell.Position.Item2 == column)
                {
                    if (cell.NoteEntries.Count != 0)
                    {
                        if (cell.Entry != 0)
                        {
                            return $"{cell.Entry.ToString()}  ({string.Join(",", cell.NoteEntries)})";
                        }
                        return $" ({string.Join(",", cell.NoteEntries)})";  // displays cell value if applicable + note entries
                    }
                    else
                    {
                        if (cell.Entry != 0)
                        {
                            return cell.Entry.ToString();
                        }
                        return $" ";  // displays the cell value if it is non-empty, otherwise displays nothing
                    }
                }
            }
            throw new InvalidOperationException("Cell does not exist");
        }
        internal string GetBackgroundColour(int row, int column)
        {
            string cellValue = DisplayCell(row, column);
            if (cellValue[0].ToString() != " " && cellValue != CorrectNumbers![row, column].ToString())  // if cell is non-empty and is filled with an incorrect value
            {
                return "red";
            }
            return "white";
        }

        internal void GetHint()
        {
            List<Cell> emptyCells = new();
            foreach (Cell cell in Board!.VariableNodes)
            {
                if (cell.Entry == 0)
                {
                    emptyCells.Add(cell);  // adds all cells that currently have no value to a list
                }
            }
            Random random = new Random();
            int randomIndex = random.Next(emptyCells.Count);
            Cell selectedCell = emptyCells[randomIndex];  // selects a random empty cell
            selectedCell.ChangeCellValue(CorrectNumbers![selectedCell.Position.Item1, selectedCell.Position.Item2]); // fills the random
        }

        internal string ChangeNotesFunctionality(string currentSetting)  // changes frontend display + functionality when changing cell values
        {
            if (currentSetting == "On")
            {
                return "Off";
            }
            return "On";
        }

        internal void Undo(MoveStack stack)
        {
            try
            {
                Move move = stack.Pop();
                move.Cell.ChangeCellValue(move.OldEntry);  // sets the given cell to its original entry before being changed
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Stack is empty");
            }
        }

        internal string FormatTime(DateTime launchTime)
        {
            TimeSpan currentTime = DateTime.Now - launchTime;
            if (currentTime.Seconds < 10)
            {
                return $"{currentTime.Minutes}:0{currentTime.Seconds}";  // eg formats as 7:06 instead of 7:6
            }
            return $"{currentTime.Minutes}:{currentTime.Seconds}";
        }
    }
}
