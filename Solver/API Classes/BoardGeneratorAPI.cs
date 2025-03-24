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
        private readonly HttpClient _client = new HttpClient();
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<List<Board>> GenerateBoard(int boardNumber)
        {
            List<Board> boards = new List<Board>();
            // sends GET request, fetching some number of boards along with their associated difficulties
            string url = "https://sudoku-api.vercel.app/api/dosuku?query={newboard(limit:boardNumber){grids{value,difficulty}}}";
            url = url.Replace("boardNumber", boardNumber.ToString());
            HttpResponseMessage response = await _client.GetAsync(url);
            // if request is successful + response is received
            if (!response.IsSuccessStatusCode)
            {
                throw new FieldAccessException();
            }
            // serialises the data into a string
            string data = await response.Content.ReadAsStringAsync();
            // deserialises the data a single ResponseData object
            var result = JsonSerializer.Deserialize<ResponseData>(data, _options);
            for (int i=0; i<boardNumber; i++)
            {
                boards.Add(ConvertToBoard(result, i));
            }
            return boards;
        }

        public Board GenerateUniqueSolution(int dimensions, Board board)
        {
            ForwardChecker solver = new ForwardChecker(board);
            bool unique = false;
            while (unique == false)
            {
                board.InitialiseQueue();
                solver!.HasUniqueSolution();
                // if board does not have a unique solution, and hence is not a valid Sudoku
                if (board.SolutionCount >= 2)
                {
                    for (int i = 0; i < dimensions; i++)
                    {
                        for (int j = 0; j < dimensions; j++)
                        {
                            // indicates a square which has a different value between the 2 solution boards
                            if (board.Solutions[0].BoardSketch[i, j] != board.Solutions[1].BoardSketch[i, j])
                            {
                                Cell cell = board.GetCellLocation(i, j);
                                cell.ChangeCellValue(Convert.ToInt32(board.Solutions[0].BoardSketch[i, j]));
                                i = dimensions;
                                j = dimensions;
                                // sets the cell to be fixed in the set starting arrangement with it taking on the value from the first solution
                                board.VariableNodes.Remove(cell);
                                board.Reset();
                                // resets the board so the solving process can be repeated to find a unique solution
                            }
                        }
                    }
                    // resets solver metrics for next solving iteration
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

        private Board ConvertToBoard(ResponseData data, int index)
        {
            // represents the board sketch as a jagged array
            int[][] twoDimensionalSketch = data.NewBoard.Grids[index].Value;
            string[,] boardSketch = new string[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (twoDimensionalSketch[i][j] == 0)
                    {
                        boardSketch[i, j] = "0v";
                    }
                    else
                    {
                        // converts the jagged array API output into a 2D array
                        boardSketch[i, j] = twoDimensionalSketch[i][j].ToString();
                    }
                }
            }
            return new Board(data.NewBoard.Grids[index].Difficulty, boardSketch, 9);
            // converts the API data into a Board object
        }
    }
}   