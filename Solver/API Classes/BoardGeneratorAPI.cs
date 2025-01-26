using Sudoku_Solver_NEA.API_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sudoku_Solver_NEA
{
    public class BoardGeneratorAPI: IBoardGenerator
    {
        private readonly HttpClient client = new HttpClient();
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<List<Board>> GenerateBoard()
        {
            List<Board> boards = new List<Board>();
            string url = "https://sudoku-api.vercel.app/api/dosuku?query={newboard(limit:10){grids{value,difficulty}}}";
            HttpResponseMessage response = await client.GetAsync(url);    // sends GET request, fetching 10 boards along with their associated difficulties
            while (!response.IsSuccessStatusCode)   // until request is successful + response is received
            {
                response = await client.GetAsync(url);
            } 
            string data = await response.Content.ReadAsStringAsync();     // serialises the data into a string
            var result = JsonSerializer.Deserialize<ResponseData>(data, _options);   // deserialises the data a single ResponseData object
            for (int i=0; i<10; i++)
            {
                boards.Add(ConvertToBoard(result, i));
            }
            return boards;
        }

        private Board ConvertToBoard(ResponseData data, int index)
        {
            int[][] twoDimensionalSketch = data.NewBoard.Grids[index].Value;  // represents the board sketch as a jagged array
            int[,] boardSketch = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    boardSketch[i, j] = twoDimensionalSketch[i][j];  // converts the jagged array API output into a 2D array
                }
            }
            return new Board(data.NewBoard.Grids[index].Difficulty, boardSketch, 9);  // creates a board object using the API data
        }

        public Board GenerateUniqueSolution(int dimensions, Board board)
        {
            ForwardChecker solver = new ForwardChecker(board);
            bool unique = false;
            while (unique == false)
            {
                board.SetQueue();
                solver!.HasUniqueSolution();
                if (board.SolutionCount >= 2)  // if board does not have a unique solution, and hence is not a valid Sudoku
                {
                    for (int i = 0; i < dimensions; i++)
                    {
                        for (int j = 0; j < dimensions; j++)
                        {
                            if (board.Solutions[0].BoardSketch[i, j] != board.Solutions[1].BoardSketch[i, j])  // a square which has a different value between the 2 solutions
                            {
                                Cell cell = board.GetCellLocation(i, j);
                                cell.ChangeCellValue(board.Solutions[0].BoardSketch[i, j]);
                                i = dimensions;
                                j = dimensions;
                                board.VariableNodes.Remove(cell);  // sets the cell to be fixed, taking on the value from the first solution
                                board.Reset();  // resets the board so the solving process can be repeated to find a unique solution
                            }
                        }
                    }
                    solver.ChangeMostRecentCell(null);
                    board.Solutions.Clear();
                    board.SetSolutionCount(0);
                }
                else
                {
                    unique = true;
                }
            }
            return board;
        }
    }
}   