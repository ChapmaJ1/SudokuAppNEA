using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class Annealer
    {
        public Board Board { get; private set; }
        public MoveStack MoveStack { get; private set; }
        public int Dimensions { get; private set; }
        public Annealer(Board board, int dimensions)
        {
            Board = board;
            MoveStack = new MoveStack(30);
            Dimensions = dimensions;
        }

        public void Solve()
        {
            InitialiseBoard();
            Random random = new Random();
            double initialTemperature = 200;   // JUSTIFY LATER
            BacktrackingSolver solver = new(Board);
            (Dictionary<Cell, List<Cell>>, int) conflictData = GetConflicts();
            for (int i = 0; i < 100000; i++)
            {
                double temperature;
                int cellsToRemove;
                if (i < 3000)
                {
                    cellsToRemove = 30;
                    temperature = initialTemperature / Math.Log(3 + i);
                }
                else if (i > 7000)
                {
                    cellsToRemove = 1;
                    temperature = initialTemperature * Math.Pow(Math.E, -0.01 * i);
                }
                else
                {
                    cellsToRemove = 20;
                    temperature = initialTemperature / Math.Pow(i, 0.5);
                }
                List<Cell> changedCells = ChangeRandomCells(conflictData.Item1, cellsToRemove);
                // double temperature = initialTemperature / Math.Log(2 + i); // temperature calculation, logarithmic decay
                // double temperature = initialTemperature - 0.01 * i;
                // double temperature = initialTemperature * Math.Pow(Math.E, -0.3 * i);
                (List<(Cell, Cell, int)>, int) changedConflicts = UpdateConflicts(conflictData, changedCells);
                conflictData.Item2 += changedConflicts.Item2;  // REMOVE LATER
                //Console.WriteLine(conflictData.Item2);
                if (conflictData.Item2 == 0)
                {
                    solver.PrintBoard(Board);
                    break;
                }
                if (changedConflicts.Item2 > 0)  // if less than 0, always accept change
                {
                    double acceptanceProbability = CalculateAcceptanceProbability(changedConflicts.Item2, temperature);
                    int number = random.Next(1000);
                    if (number > acceptanceProbability * 1000)  // if new number not accepted, revert conflict data to previous state
                    {
                        conflictData.Item2 -= changedConflicts.Item2;
                        for (int j = 0; j < cellsToRemove; j++)
                        {
                            Move move = MoveStack.Pop();
                            move.Cell.Entry = move.OldEntry;
                        }
                        ReinstateConflicts(changedConflicts.Item1, conflictData.Item1);
                    }
                }
            }
            Console.WriteLine(conflictData.Item2);
            Board.VariableNodes.Clear();
            foreach (KeyValuePair<Cell, List<Cell>> pair in conflictData.Item1)
            {
                if (pair.Value.Count != 0)
                {
                    pair.Key.Entry = GetLCVNumber(pair.Key);
                    pair.Key.Entry = 0;
                    Board.VariableNodes.Add(pair.Key);
                }
            }
          //   Board.SetQueue();
            solver.Solve(); // most cells have 0, 1 or 2 conflicts by the end of the procedure (25x25)
        }

        private void InitialiseBoard()   // greedy initialisation
        {

           foreach (Cell cell in Board.VariableNodes)
           {
               cell.Entry = GetLCVNumber(cell);
           }   
         /*  Random random = new Random();
            
           foreach (Cell cell in Board.VariableNodes)
           {
               int randomNum = random.Next(Dimensions + 1);
               cell.Entry = randomNum;
           } 
            */
        }

        private int CalculateAcceptanceProbability(int change, double temperature)  // uses Metropolis criterion
        {
            double probability = Math.Pow(Math.E, -(change / temperature));
            if (probability > 1)  // overflow
            {
                return 0;
            }
            return Convert.ToInt32(Math.Round(probability, 3));
        }

        private List<Cell> ChangeRandomCells(Dictionary<Cell, List<Cell>> conflictCells, int cellsToRemove)
        {
          //  List<Cell> cells = conflictCells.Select(kvp => kvp.Key).ToList();
            List<Cell> cells = conflictCells.OrderByDescending(kvp => kvp.Value.Count).Select(kvp => kvp.Key).ToList();
            List<Cell> changedCells = new();
         /*   Random random = new Random();
            List<int> randomIndexes = new();
            while (randomIndexes.Count < cellsToRemove)
            {
                int randomIndex = random.Next(conflictCells.Count);
                if (!(randomIndexes.Contains(randomIndex)))
                {
                    randomIndexes.Add(randomIndex);
                }
            } */

            for (int i=0; i<cellsToRemove; i++)
            {
                Cell chosenCell = cells[i];
                changedCells.Add(chosenCell);
                MoveStack.Push(new Move(chosenCell, chosenCell.Entry));
                chosenCell.Entry = GetLCVNumber(chosenCell);  // biased towards improvement - picking entry that is likely to result in the fewest new conflicts
            } 
            
            /*
            foreach (int index in randomIndexes)
            {
                Cell chosenCell = cells[index];
                changedCells.Add(chosenCell);
                MoveStack.Push(new Move(chosenCell, chosenCell.Entry));
                chosenCell.Entry = random.Next(Dimensions + 1);
            }
            */
            return changedCells;
        }

        private int GetLCVNumber(Cell cell)
        {
            int minConflicts = int.MaxValue;
            int lcvValue = 0;
            for (int i = 1; i <= Dimensions; i++)
            {
                int conflicts = Board.AdjacencyList[cell].Count(node => node.Entry == i);
                if (conflicts < minConflicts)
                {
                    minConflicts = conflicts;
                    lcvValue = i;
                }
            }
            return lcvValue;
        }

        private (Dictionary<Cell, List<Cell>>, int) GetConflicts()
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
    }
}

