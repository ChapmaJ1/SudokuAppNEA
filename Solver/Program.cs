//using Solver;
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

           /* int[,] boardSketch = { {8,0,0,0,2,0,0,6,0},
                             { 2,0,0,0,3,8,5,1,4 },
                             { 0,0,0,6,0,0,3,8,0 },
                             { 0,7,0,5,0,2,1,0,0 },
                             { 0,0,6,0,4,1,0,9,0 },
                             { 0,0,8,0,0,0,2,5,6 },
                             { 6,0,2,4,5,0,8,7,9 },
                             { 0,0,0,7,0,0,4,2,1 },
                             { 7,8,0,2,1,0,6,0,0} };  */
            int[,] boardSketch = new int[16, 16];
            for (int i = 0; i<boardSketch.GetLength(0); i++)
            {
                for (int j=0; j<boardSketch.GetLength(0); j++)
                {
                    boardSketch[i, j] = 0;
                }
            } 
            Board board = new Board("Hard", boardSketch, boardSketch.GetLength(0));
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
            // ForwardChecker solver = new ForwardChecker(board);
            // board2.SetQueue();
            // solver3.Solve();
            // Console.WriteLine(board2.Solutions);
            // Annealer annealer = new Annealer(board, boardSketch.GetLength(0));
            // annealer.Solve();
            DateTime launchTime = DateTime.Now;
           /* BacktrackingSolver solver = new BacktrackingSolver(board);
            solver.Solve(); */
            ForwardChecker solver2 = new(board);
            UniqueBoardGenerator generator = new UniqueBoardGenerator(board);
            generator.GenerateUniqueSolution(16);
           // board.SetQueue();
           // solver2.Solve();
            Console.WriteLine($"{(DateTime.Now - launchTime).Seconds} seconds {(DateTime.Now - launchTime).Milliseconds} milliseconds");
            solver2.PrintBoard(board);
            Console.ReadLine(); 
        }
    }
}
