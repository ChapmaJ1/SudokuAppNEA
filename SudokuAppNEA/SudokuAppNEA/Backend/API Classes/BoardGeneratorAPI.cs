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
    public class BoardGeneratorAPI
    {
        private readonly HttpClient client = new HttpClient();
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public async Task<ResponseData> GenerateBoard()
        {
            string url = "https://sudoku-api.vercel.app/api/dosuku?query={newboard(limit:1){grids{value,difficulty}}}";
            HttpResponseMessage response = await client.GetAsync(url);    // sends GET request
            if (response.IsSuccessStatusCode)    // if request was successful - response received
            {
                string data = await response.Content.ReadAsStringAsync();     // serialises the data into a string
                var result = JsonSerializer.Deserialize<ResponseData>(data, _options);   // deserialises the data a single ResponseData object
                return result;
            }
            else
            {
                throw new HttpRequestException("Could not fetch board");
            }
        }

        public Board ConvertToBoard(ResponseData data)
        {
            int[][] twoDimensionalSketch = data.NewBoard.Grids[0].Value;  // represents the board sketch as a jagged array
            int[,] boardSketch = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    boardSketch[i, j] = twoDimensionalSketch[i][j];
                }
            }
            return new Board(data.NewBoard.Grids[0].Difficulty, boardSketch);
        }
    }
}   