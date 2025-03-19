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
        // a dictionary of boxes by number (0,1,...,N) and the cells which each box contains
        public Dictionary<int, List<Cell>> BoxGroupings { get; private set; }
        public Annealer(Board board) : base(board)
        {
            Stack = new MoveStack(100);
            BoxGroupings = new();
        }

        public override bool Solve()
        {
            InitialiseBoard();
            // if initial board is already solved
            if (BoxGroupings.Count == 0)
            {
                PrintBoard(Board);
                return true;
            }
            Random random = new Random();
            double initialTemperature = 40;
            // sets up the initial dictionary of Cells taking on identical values
            (Dictionary<Cell, List<Cell>>, int) conflictData = GetInitialConflicts();
            for (int i = 0; i < 1000000; i++)
            {
                double temperature;
                int swapNumber;
                // selects the number of boxes that swaps should take place within based on iteration number
                // this allows for greater initial exploration, while narrowing in on the global minimum later on in the process
                switch (i)
                {
                    case < 5000: 
                        swapNumber = BoxGroupings.Count; 
                        break;
                    case > 20000:
                        swapNumber = 1;
                        break;
                    default:
                        swapNumber = Convert.ToInt32(Math.Ceiling((double)BoxGroupings.Count / 2));
                        break;
                }
                // performs a certain number of swaps within boxes, defined by swapNumber
                List<Cell> changedCells = ChangeRandomCells(swapNumber);
                // updates the conflicts dictionary and number of conflicts present based on the performed swap
                (List<(Cell, Cell, int)>, int) changedConflicts = UpdateConflicts(conflictData, changedCells);
                conflictData.Item2 += changedConflicts.Item2; 
                // if board is not solved
                if (conflictData.Item2 == 0)
                {
                    PrintBoard(Board);
                    break;
                }
                // if the number of conflicts decreases or stays the same, always accept the change
                if (changedConflicts.Item2 > 0)
                {
                    temperature = initialTemperature * Math.Pow(Math.E, -0.05 * i);
                    // calculate the acceptance probability based on the magnitude of the increase in conflicts and the iteration number (temperature)
                    double acceptanceProbability = CalculateAcceptanceProbability(changedConflicts.Item2, temperature);
                    int number = random.Next(1000);
                    // if the number is sufficiently high, do not accept the change and revert back the board back to its state in the previous iteration
                    if (number >= acceptanceProbability * 1000)
                    {
                        // reverts the conflict number counter
                        conflictData.Item2 -= changedConflicts.Item2;
                        // undoes all the changes of the iteration, resetting all the values of cells to their previous values
                        for (int j = 0; j < swapNumber * 2; j++)
                        {
                            Move move = Stack.Pop();
                            move.Cell.ChangeCellValue(move.OldEntry);
                        }

                        ReinstateConflicts(changedConflicts.Item1, conflictData.Item1);
                    }
                }
            }
            // if board not solved by the end of the iterations
            // for all cells which still have conflicts, set them to 0 and variable (empty)
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
            for (int i = 0; i < Board.Dimensions; i++)
            {
                List<int> remainingNumbersInSquare = new();
                // adds all N numbers on an NxN board to the list
                for (int x = 1; x <= Board.Dimensions; x++)
                {
                    remainingNumbersInSquare.Add(x);
                }
                List<Cell> cellsInSquare = new();
                // selects sub-box within the board
                int boxI = i / squareDimensions; 
                int boxJ = i % squareDimensions;
                for (int j = 0; j < squareDimensions; j++)
                {
                    for (int k = 0; k < squareDimensions; k++)
                    {
                        // gets every cell in the sub-box
                        Cell cell = Board.GetCellLocation(boxI * squareDimensions + j, boxJ * squareDimensions + k);
                        // if a cell is empty, add it to variable cells to be assigned a random value
                        if (cell.Entry == 0)
                        {
                            cellsInSquare.Add(cell);
                        }
                        // if a cell is a fixed node, prevent its entry from being assigned to other nodes - no other nodes in the sub-box can take on this value
                        else
                        {
                            remainingNumbersInSquare.Remove(cell.Entry);
                        }
                    }
                }
                // if at least 1 variable node in the sub-box - it is not already complete
                if (cellsInSquare.Count != 0)
                {
                    BoxGroupings.Add(i, cellsInSquare);
                    foreach (Cell cell in cellsInSquare)
                    {
                        // gets random number remaining in the sub-box domain, changes the entry of a particular cell to this value
                        // removes the number from the domain - no other nodes in the sub-box can take on this value
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
            // for all cells on the board
            foreach (KeyValuePair<Cell, List<Cell>> pair in Board.AdjacencyList)
            {
                conflictedCells.Add(pair.Key, new());
                foreach (Cell cell in pair.Value)
                {
                    // if a node has an identical value to a connecte node
                    // update the conflicts dictionary
                    if (pair.Key.Entry == cell.Entry)
                    {
                        conflictedCells[pair.Key].Add(cell);
                    }
                }
            }
            return (conflictedCells, conflictedCells.Values.Sum(l => l.Count));
            // returns all conflicting cells + number of conflicts
            // the dictionary represents undirected graph, therefore each conflict will be counted twice: (cell1, cell2) and (cell2, cell1) in separate key-value pairs
        }

        private (List<(Cell, Cell, int)>, int) UpdateConflicts((Dictionary<Cell, List<Cell>>, int) oldConflictData, List<Cell> changedCells)
        {
            // the final integer value represents whether a certain conflict has been removed (0) or added (1)
            List<(Cell, Cell, int)> changedConflicts = new();
            int deltaConflicts = 0;
            List<(Cell, Cell)> tempAddValues = new();
            List<(Cell, Cell)> tempRemoveValues = new();
            // iterate through all cells whose value was changed during the current annealing iteration
            foreach (Cell cell in changedCells)  
            {
                // iterate through conflicting cells as of the previous annealing iteration
                foreach (Cell conflictCell in oldConflictData.Item1[cell])
                {
                    // if the cell no longer conflicts with a given connected node
                    if (cell.Entry != conflictCell.Entry)
                    {
                        if (!tempRemoveValues.Contains((conflictCell, cell)))
                        {
                            // subtract 2 rather than 1 due to undirected graph nature
                            deltaConflicts -= 2;
                            tempRemoveValues.Add((cell, conflictCell));
                            // 0 represents removed conflict - add back if change not accepted
                            changedConflicts.Add((cell, conflictCell, 0));
                        }
                    }
                }
                foreach (Cell neighbour in Board.AdjacencyList[cell])
                {
                    // if a cell conflicts with a connected node and they were not already conflicting
                    if (cell.Entry == neighbour.Entry)
                    {
                        if (!oldConflictData.Item1[cell].Contains(neighbour))
                        {
                            // if conflict has not already been recorded the other way around (undirected graph)
                            if (!tempAddValues.Contains((neighbour, cell)))   
                            {
                                deltaConflicts += 2;
                                tempAddValues.Add((cell, neighbour));
                                changedConflicts.Add((cell, neighbour, 1));
                                // 1 represents added conflict - remove if change not accepted
                            }
                        }
                    }
                }
            }
            // removes + adds all recorded conflict changes
            foreach ((Cell, Cell) pair in tempRemoveValues)
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
            // iterates through all changed conflicts in the list
            foreach ((Cell, Cell, int) conflictChange in changedConflicts)
            {
                // if a conflict was previously removed and should be added back to revert the board to its previous state
                if (conflictChange.Item3 == 0)
                {
                    conflictData[conflictChange.Item1].Add(conflictChange.Item2);
                    conflictData[conflictChange.Item2].Add(conflictChange.Item1);
                }
                // if a conflict was previously added and should be removed to revert the board to its previous state
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
            List<int> unusedBoxes = new();
            for (int i=0; i<Board.Dimensions; i++)
            {
                unusedBoxes.Add(i);
            }
            int counter = unusedBoxes.Count;
            Random random = new Random();
            // until the specified number of swaps in different boxes have been made
            for (int i = 0; i < swapNumber; i++)
            {
                // select a random box that a swap has not already occurred in
                int box = unusedBoxes[random.Next(counter)];
                // do not select a box that only has one cell with conflicts - a conflict-reducing swap is unlikely
                while (BoxGroupings[box].Count < 2)
                {
                    box = unusedBoxes[random.Next(counter)];
                }
                unusedBoxes.Remove(box);
                counter--;
                // selects 2 random cells within the box
                List<Cell> boxCells = BoxGroupings[box];
                int firstRandomCell = random.Next(boxCells.Count);
                int secondRandomCell = random.Next(boxCells.Count);
                // ensures that the selected cells are different, and the same cell is not selected twice
                while (secondRandomCell == firstRandomCell)
                {
                    secondRandomCell = random.Next(boxCells.Count);
                }
                // records the previous values of each cell for later reversion if equired
                Stack.Push(new Move(boxCells[firstRandomCell], boxCells[firstRandomCell].Entry));
                Stack.Push(new Move(boxCells[secondRandomCell], boxCells[secondRandomCell].Entry));
                // swaps the entries of the two cells
                int tempEntry = boxCells[firstRandomCell].Entry;
                boxCells[firstRandomCell].ChangeCellValue(boxCells[secondRandomCell].Entry);
                boxCells[secondRandomCell].ChangeCellValue(tempEntry);
                // records each cell as one which has had a swap performed on it
                swappedCells.Add(boxCells[firstRandomCell]);
                swappedCells.Add(boxCells[secondRandomCell]);
            }
            return swappedCells;
        }
        
        // uses the criterion of probabilistic acceptance of a worse board state
        private double CalculateAcceptanceProbability(int change, double temperature)
        {
            // calculate and return the acceptance probability based on the temperature and how much "worse" the solution is
            double probability = Math.Pow(Math.E, -(change / temperature));
            // occurs due to an underflow , at which point take the probability as 0 since it is so small
            if (probability > 1)
            {
                return 0;
            }
            return Math.Round(probability, 3);
        }
    }
}
