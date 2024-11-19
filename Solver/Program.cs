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

            int[,] boardSketch = { {7,1,0,0,0,0,2,9,0},
                            { 0,0,0,0,0,0,0,5,6 },
                            { 9,0,0,0,0,8,0,0,3 },
                            { 0,5,0,8,0,0,0,0,0 },
                            { 8,0,0,0,0,0,5,0,2 },
                            { 6,0,0,0,0,0,8,0,0 },
                            { 0,0,0,9,0,0,6,0,0 },
                            { 4,6,0,0,0,0,0,0,0 },
                            { 0,0,9,6,8,0,0,0,0} };
            Board board = new Board("Hard", boardSketch);
            board.InitialiseGraph();
            board.SetQueue();

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
            ForwardChecker solver2 = new ForwardChecker(board, board.VariableNodes);
            solver2.Solve();
            //Console.WriteLine($"{board.Solutions.Count} solutions");
            board.Reset();
            Board boardTemp = board.Solutions[0];
            Console.WriteLine($"{board.Solutions.Count} solutions");
            Console.WriteLine("Solved");
          
            
            
            
            
           /* BoardGeneratorAPI generator = new BoardGeneratorAPI();
            
            ResponseData response = await generator.GenerateBoard();
            Board board2 = generator.ConvertToBoard(response);
            board2.InitialiseGraph();
            HeapPriorityQueue queue2 = new HeapPriorityQueue(board2.VariableNodes, 9);
            foreach (Cell node in board2.VariableNodes)
            {
                queue2.Enqueue(node);
            }
            ForwardChecker solver3 = new ForwardChecker(board2, board2.VariableNodes, queue2);
            //solver3.Solve();
            solver3.HasUniqueSolution();
            Console.WriteLine(board2.Solutions.Count);
            Console.ReadLine(); */
        }
    }
}
