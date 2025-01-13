using System;
using System.Collections.Generic;
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
            MoveStack = new MoveStack();
            Dimensions = dimensions;
        }

        public void Solve()
        {
            InitialiseBoard();
            Random random = new Random();
            double initialTemperature = 50;   // JUSTIFY LATER
            (List<Cell>, int) initialConflictData = GetConflicts(); 
            for (int i = 0; i < 2000; i++)
            {
                ChangeRandomCell(initialConflictData.Item1);
                //double temperature = initialTemperature / Math.Log(1+i); // temperature calculation, logarithmic decay
                // double temperature = initialTemperature - 0.05 * i;
                 double temperature = initialTemperature * Math.Pow(Math.E, -0.2 * i);
                (List<Cell>, int) newConflictData = GetConflicts();
                Console.WriteLine(newConflictData.Item2);
                if (newConflictData.Item2 == 0)
                {
                    initialConflictData = newConflictData;
                    break;
                }
                int deltaConflicts = newConflictData.Item2 - initialConflictData.Item2;
                if (deltaConflicts > 0)  // if less than 0, always accept change
                {
                    double acceptanceProbability = CalculateAcceptanceProbability(deltaConflicts, temperature);
                    int number = random.Next(1000);
                    if (number > acceptanceProbability * 1000)  // if new number not accepted, do not update conflict data
                    {
                        Move move = MoveStack.Pop();
                        move.Cell.Entry = move.OldEntry;
                    }
                    else
                    {
                        initialConflictData = newConflictData;
                    }
                }
                else
                {
                    initialConflictData = newConflictData;
                }
            }
            Console.WriteLine(initialConflictData.Item2);
            Board.VariableNodes.Clear();
            ForwardChecker solver = new(Board);
            foreach (Cell cell in initialConflictData.Item1)
            {
                cell.Entry = 0;
                Board.VariableNodes.Add(cell);
            }
             solver.Solve();
        }

        private void InitialiseBoard()
        {
            Random random = new Random();
            foreach (Cell cell in Board.VariableNodes)
            {
                cell.Entry = random.Next(1, Dimensions+1);
            }
        }

        private int CalculateAcceptanceProbability(int change, double temperature)  // uses Metropolis criterion
        {
            double probability = Math.Pow(Math.E, -(change/temperature));
            if (probability > 1)
            {
                return 0;
            }
            return Convert.ToInt32(Math.Round(probability, 3));
        }

        private void ChangeRandomCell(List<Cell> conflictCells)
        {
            Random random = new Random();
            int randomIndex = random.Next(conflictCells.Count);
            Cell chosenCell = conflictCells[randomIndex];
            List<int> potentialChangeNumbers = new List<int>();
            for (int i=1; i<=Dimensions; i++)
            {
                if (i != chosenCell.Entry)
                {
                    potentialChangeNumbers.Add(i);
                }
            }
            MoveStack.Push(new Move(chosenCell, chosenCell.Entry));
            int randomNumber = random.Next(Dimensions-1);
            chosenCell.Entry = potentialChangeNumbers[randomNumber];
        }

        private (List<Cell>, int) GetConflicts()
        {
            List<Cell> conflictedCells = new();
            int conflicts = 0;
            foreach (KeyValuePair<Cell, List<Cell>> pair in Board.AdjacencyList)
            {
                foreach (Cell cell in pair.Value)
                {
                    if (pair.Key.Entry == cell.Entry)
                    {
                        conflicts++;
                        if (!conflictedCells.Contains(pair.Key))
                        {
                            conflictedCells.Add(pair.Key);
                        }
                        if (!conflictedCells.Contains(cell))
                        {
                            conflictedCells.Add(cell);
                        }
                    }
                }
            }
            return (conflictedCells, conflicts/2);  // undirected graph, therefore each conflict will be counted twice: (cell1, cell2) and (cell2, cell1)
        }
    }
}
