using Sudoku_Solver_NEA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class Annealer : BacktrackingSolver
    {
        public MoveStack Stack { get; private set; }
        public Dictionary<int, List<Cell>> BoxGroupings { get; private set; }
        public Annealer(Board board) : base(board)
        {
            Stack = new MoveStack(100);
            BoxGroupings = new();
        }

        public override bool Solve()
        {
            InitialiseBoard();
            Random random = new Random();
            double initialTemperature = 40;   // JUSTIFY LATER
            (Dictionary<Cell, List<Cell>>, int) conflictData = GetInitialConflicts();
            for (int i = 0; i < 1000000; i++)
            {
                double temperature;
                int swapNumber;
                if (i < 3000)
                {
                    swapNumber = BoxGroupings.Count;
                    // temperature = initialTemperature / Math.Log(3 + i);
                }
                else if (i > 10000)
                {
                    swapNumber = 1;
                    // temperature = initialTemperature * Math.Pow(Math.E, -0.1 * i);
                }
                else
                {
                    swapNumber = BoxGroupings.Count - 3;
                    // temperature = initialTemperature / Math.Pow(i, 0.5);
                }
                List<Cell> changedCells = ChangeRandomCells(swapNumber);
                // solver.PrintBoard(Board);
                // temperature = initialTemperature / Math.Log(3 + i); // temperature calculation, logarithmic decay
                //temperature = initialTemperature - 0.1 * i;
                temperature = initialTemperature * Math.Pow(Math.E, -0.05 * i);
                (List<(Cell, Cell, int)>, int) changedConflicts = UpdateConflicts(conflictData, changedCells);
                conflictData.Item2 += changedConflicts.Item2; 
                if (conflictData.Item2 == 0)
                {
                    PrintBoard(Board);
                    break;
                }
                if (changedConflicts.Item2 > 0)  // if less than 0, always accept change
                {
                    double acceptanceProbability = CalculateAcceptanceProbability(changedConflicts.Item2, temperature);
                    int number = random.Next(1000);
                    if (number >= acceptanceProbability * 1000)  // if new number not accepted, revert conflict data to previous state
                    {
                        conflictData.Item2 -= changedConflicts.Item2;
                        for (int j = 0; j < swapNumber * 2; j++)
                        {
                            Move move = Stack.Pop();
                            move.Cell.ChangeCellValue(move.OldEntry);
                        }
                        ReinstateConflicts(changedConflicts.Item1, conflictData.Item1);
                    }
                }
            }
            Board.VariableNodes.Clear();
            foreach (KeyValuePair<Cell, List<Cell>> pair in conflictData.Item1)
            {
                if (pair.Value.Count != 0)
                {
                    pair.Key.ChangeCellValue(0);
                    Board.VariableNodes.Add(pair.Key);
                }
            }
            return true;
        }

        private void InitialiseBoard()
        {
            Random random = new Random();
            int squareDimensions = Convert.ToInt32(Math.Sqrt(Board.Dimensions));
            for (int i = 0; i < Board.Dimensions; i++)  // selects sub-box
            {
                List<int> remainingNumbersInSquare = new();
                for (int x = 1; x <= Board.Dimensions; x++)
                {
                    remainingNumbersInSquare.Add(x);
                }
                List<Cell> cellsInSquare = new();
                int boxI = i / squareDimensions;
                int boxJ = i % squareDimensions;
                for (int j = 0; j < squareDimensions; j++)
                {
                    for (int k = 0; k < squareDimensions; k++)
                    {
                        Cell cell = Board.GetCellLocation(boxI * squareDimensions + j, boxJ * squareDimensions + k);
                        if (cell.Entry == 0)
                        {
                            cellsInSquare.Add(cell);
                        }
                        else
                        {
                            remainingNumbersInSquare.Remove(cell.Entry);
                        }
                    }
                }
                if (cellsInSquare.Count != 0)
                {
                    BoxGroupings.Add(i, cellsInSquare);
                    foreach (Cell cell in cellsInSquare)
                    {
                        int indexToRemove = random.Next(remainingNumbersInSquare.Count);
                        cell.ChangeCellValue(remainingNumbersInSquare[indexToRemove]);
                        remainingNumbersInSquare.RemoveAt(indexToRemove);
                    }
                }
            }
        }

            private (Dictionary<Cell, List<Cell>>, int) GetInitialConflicts()
        {
            Dictionary<Cell, List<Cell>> conflictedCells = new();
            foreach (KeyValuePair<Cell, List<Cell>> pair in Board.AdjacencyList)
            {
                conflictedCells.Add(pair.Key, new());
                foreach (Cell cell in pair.Value)
                {
                    if (pair.Key.Entry == cell.Entry)
                    {
                        conflictedCells[pair.Key].Add(cell);
                    }
                }
            }
            return (conflictedCells, conflictedCells.Values.Sum(l => l.Count));  // undirected graph, therefore each conflict will be counted twice: (cell1, cell2) and (cell2, cell1)
        }

        private (List<(Cell, Cell, int)>, int) UpdateConflicts((Dictionary<Cell, List<Cell>>, int) oldConflictData, List<Cell> changedCells)
        {
            List<(Cell, Cell, int)> changedConflicts = new();
            int deltaConflicts = 0;
            List<(Cell, Cell)> tempAddValues = new();
            List<(Cell, Cell)> tempRemoveValues = new();
            foreach (Cell cell in changedCells)
            {
                List<Cell> cells = oldConflictData.Item1[cell];
                foreach (Cell conflictCell in oldConflictData.Item1[cell])
                {
                    if (cell.Entry != conflictCell.Entry)
                    {
                        if (!tempRemoveValues.Contains((conflictCell, cell)))
                        {
                            deltaConflicts -= 2;
                            tempRemoveValues.Add((cell, conflictCell));
                            changedConflicts.Add((cell, conflictCell, 0));  // 0 represents removed conflict (add back)
                        }
                    }
                }
                foreach (Cell neighbour in Board.AdjacencyList[cell])
                {
                    if (cell.Entry == neighbour.Entry)
                    {
                        if (!oldConflictData.Item1[cell].Contains(neighbour))
                        {
                            if (!tempAddValues.Contains((neighbour, cell)))
                            {
                                deltaConflicts += 2;
                                tempAddValues.Add((cell, neighbour));
                                changedConflicts.Add((cell, neighbour, 1));  // 1 represents added conflict (remove)
                            }
                        }
                    }
                }
            }
            foreach ((Cell, Cell) pair in tempRemoveValues)   // why are tempAddValues and tempRemoveValues so small
            {
                oldConflictData.Item1[pair.Item1].Remove(pair.Item2);
                oldConflictData.Item1[pair.Item2].Remove(pair.Item1);
            }
            foreach ((Cell, Cell) pair in tempAddValues)
            {
                oldConflictData.Item1[pair.Item1].Add(pair.Item2);
                oldConflictData.Item1[pair.Item2].Add(pair.Item1);
            }
            return (changedConflicts, deltaConflicts);
        }

        private void ReinstateConflicts(List<(Cell, Cell, int)> changedConflicts, Dictionary<Cell, List<Cell>> conflictData)
        {
            foreach ((Cell, Cell, int) conflictChange in changedConflicts)
            {
                if (conflictChange.Item3 == 0)
                {
                    conflictData[conflictChange.Item1].Add(conflictChange.Item2);
                    conflictData[conflictChange.Item2].Add(conflictChange.Item1);
                }
                else
                {
                    conflictData[conflictChange.Item1].Remove(conflictChange.Item2);
                    conflictData[conflictChange.Item2].Remove(conflictChange.Item1);
                }
            }
        }

        private List<Cell> ChangeRandomCells(int swapNumber)
        {
            List<Cell> swappedCells = new();
            List<int> usedBoxes = new();
            Random random = new Random();
            for (int i = 0; i < swapNumber; i++)
            {
                int box = random.Next(Board.Dimensions);
                while (BoxGroupings[box].Count < 2 || usedBoxes.Contains(box))
                {
                    box = random.Next(Board.Dimensions);
                }
                usedBoxes.Add(box);
                List<Cell> boxCells = BoxGroupings[box];
                int firstRandomCell = random.Next(boxCells.Count);
                int secondRandomCell = random.Next(boxCells.Count);
                while (secondRandomCell == firstRandomCell)
                {
                    secondRandomCell = random.Next(boxCells.Count);
                }
                Stack.Push(new Move(boxCells[firstRandomCell], boxCells[firstRandomCell].Entry));
                Stack.Push(new Move(boxCells[secondRandomCell], boxCells[secondRandomCell].Entry));
                int tempEntry = boxCells[firstRandomCell].Entry;
                boxCells[firstRandomCell].ChangeCellValue(boxCells[secondRandomCell].Entry);
                boxCells[secondRandomCell].ChangeCellValue(tempEntry);
                swappedCells.Add(boxCells[firstRandomCell]);
                swappedCells.Add(boxCells[secondRandomCell]);
            }
            return swappedCells;
        }

        private double CalculateAcceptanceProbability(int change, double temperature)  // uses Metropolis criterion
        {
            double probability = Math.Pow(Math.E, -(change / temperature));
            if (probability > 1)  // checks for underflow error
            {
                return 0;
            }
            return Math.Round(probability, 3);
        }
    }
}
