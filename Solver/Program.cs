using Sudoku_Solver_NEA.API_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/

            int[,] boardSketch = { {8,0,0,0,2,0,0,6,0},
                            { 2,0,0,0,3,8,5,1,4 },
                            { 0,0,0,6,0,0,3,8,0 },
                            { 0,7,0,5,0,2,1,0,0 },
                            { 0,0,6,0,4,1,0,9,0 },
                            { 0,0,8,0,0,0,2,5,6 },
                            { 6,0,2,4,5,0,8,7,9 },
                            { 0,0,0,7,0,0,4,2,1 },
                            { 7,8,0,2,1,0,6,0,0} };
            Board board = new Board("Hard", boardSketch);
            board.InitialiseGraph();

            /*foreach (KeyValuePair<Cell, List<Cell>> pair in board.AdjacencyList)
            {
                Console.Write($"{pair.Key.Position}:    ");
                foreach (Cell cell in pair.Value)
                {
                    Console.Write($"{cell.Position}, ");
                }
                Console.WriteLine("\n");
            }
            Console.ReadLine();*/

            //BacktrackingSolver solver = new BacktrackingSolver(board, board.GetVariableNodes());
            /*foreach (KeyValuePair<(int,int), List<int>> pair in board.RemainingNumbers)
            {
                Console.WriteLine($"{pair.Key}:   {string.Join(",", pair.Value)}");
            }
            Console.WriteLine(string.Join(",", board.GetVariableNodes()));*/
            //solver.Solve();
            /* ForwardChecker solver2 = new ForwardChecker(board);
             solver2.Solve();
             //Console.WriteLine($"{board.Solutions.Count} solutions");
             board.Reset();
             Board boardTemp = board.Solutions[0];
             Console.WriteLine($"{board.Solutions.Count} solutions");
             Console.WriteLine("Solved"); */





            /*BoardGeneratorAPI generator = new BoardGeneratorAPI();
            
            ResponseData response = await generator.GenerateBoard();
            Board board2 = generator.ConvertToBoard(response, 0);
            board2.InitialiseGraph();
            ForwardChecker solver3 = new ForwardChecker(board2);*/
            ForwardChecker solver = new ForwardChecker(board);
            bool unique = false;
            while (unique == false)
            {
                solver!.HasUniqueSolution();
                if (board.SolutionCount >= 2)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            if (board.Solutions[0].BoardSketch[i, j] != board.Solutions[1].BoardSketch[i, j])
                            {
                                Cell cell = board.GetCellLocation(i, j);
                                cell.Entry = board.Solutions[0].BoardSketch[i, j];
                                i = 9;
                                j = 9;
                                board.VariableNodes.Remove(cell);
                                board.Reset();
                            }
                        }
                    }
                    board.Solutions = new List<Board>();
                    board.SolutionCount = 0;
                }
                else  // solutions = 0 for some reason
                {
                    unique = true;
                }
            }
            // board2.SetQueue();
            // solver3.Solve();
            // Console.WriteLine(board2.Solutions);
            board.SetQueue();
            solver.Solve();
            Console.WriteLine(board.Solutions);
            Console.ReadLine(); 
        }
    }
}
