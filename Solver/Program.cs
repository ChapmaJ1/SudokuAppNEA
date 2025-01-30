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
                             { 7,8,0,2,1,0,6,0,0} };    */
            string[,] boardSketch = new string[16, 16];
            for (int i = 0; i<boardSketch.GetLength(0); i++)
            {
                for (int j=0; j<boardSketch.GetLength(0); j++)
                {
                    boardSketch[i, j] = "0v";
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
            UniqueBoardGenerator generator = new UniqueBoardGenerator();
            generator.GenerateUniqueSolution(16, board); 
            //board.SetQueue();
            //solver2.Solve(new Cell((-1,-1),-1));
            Console.WriteLine($"{(DateTime.Now - launchTime).Seconds} seconds {(DateTime.Now - launchTime).Milliseconds} milliseconds");
            //solver2.PrintBoard(board);
            Console.WriteLine("😊😊");
            Console.ReadLine(); 
        }
    }
}

/* 0 9 0 13 0 6 3 0 14 12 0 8 0 0 0 16
0 10 3 0 0 13 9 8 16 0 0 4 6 2 1 0
15 7 16 0 0 12 5 4 6 0 1 0 11 0 9 0
0 5 0 0 1 7 16 2 0 0 10 0 8 14 0 12
10 6 12 9 5 0 0 3 4 0 8 0 16 0 0 7
0 0 5 0 0 1 7 11 9 3 0 16 10 0 8 0
0 11 0 0 0 0 0 0 12 5 0 0 14 15 0 0
16 3 7 15 0 0 8 0 0 0 14 11 0 0 2 9
0 0 15 0 12 8 6 0 3 0 0 10 4 11 0 1
14 0 10 3 0 2 11 1 5 9 0 6 15 0 0 8
9 0 1 11 4 0 0 7 2 8 15 12 0 6 16 0
7 8 6 0 0 16 0 0 13 11 0 0 2 9 0 14
3 12 4 0 2 0 1 16 0 0 13 0 0 0 15 11
11 16 0 0 0 15 12 14 0 0 0 2 1 4 7 6
0 0 0 7 8 11 4 6 0 16 0 5 0 3 0 0
0 0 0 0 0 0 13 10 11 4 9 15 0 0 5 2 */
