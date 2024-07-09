using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MinesweeperApiServer.Models
{
    public class GameInfoResponse : NewGameRequest
    {
        [Required, JsonProperty("game_id"), JsonPropertyName("game_id")]
        public string GameId { get; set; }

        [DefaultValue(false), JsonPropertyName("completed"), JsonProperty("completed")]
        public bool Completed { get; private set; }

        [Required]
        public List<List<char>> Field { get; set; } = new List<List<char>>();

        List<List<char>> completedField = new();
        private List<List<char>> CompletedField { get => completedField; set => completedField = value; }

        bool IsFirstAttempt { get; set; } = true;

        private HashSet<string> RelivedCells { get; set; } = [];

        HashSet<string> Mines = [];

        public GameInfoResponse(string gameId, int width, int height, int minesCount) : base(width, height, minesCount)
        {
            GameId = gameId;
            Width = width;
            Height = height;
            MinesCount = minesCount;
            Field = InitializeBoard(height, width);
            CompletedField = InitializeBoard(height, width);

        }

        List<List<char>> InitializeBoard(int height, int width)
        {
            var field = new List<List<char>>();
            for (int i = 0; i < height; i++)
            {
                var row = new List<char>();
                for (int j = 0; j < width; j++)
                {
                    row.Add(' ');
                }
                field.Add(row);
            }
            return field;
        }

        public void MakeAnAttempt(ref GameTurnRequest request)
        {
            if (IsFirstAttempt)
            {
                FirstTimeActions(ref request);
                return;
            }

            string position = request.Col.ToString() + request.Row.ToString();
            if (Mines.Contains(position))
            {
                PlaceMines('X', CompletedField);
                Field = CompletedField;
                Completed = true;
                return; // game over
            }

            if (CompletedField[request.Row][request.Col] == '0')
                RevealEmptyCells(request.Row, request.Col);
            else
                Field[request.Row][request.Col] = CompletedField[request.Row][request.Col];

            RelivedCells.Add(request.Col.ToString() + request.Row.ToString());
            IsGameWon();
        }

        private void RevealEmptyCells(int startRow, int startCol)
        {
            var stack = new Stack<(int row, int col)>();
            var visited = new HashSet<(int row, int col)>();
            stack.Push((startRow, startCol));

            while (stack.Count > 0)
            {
                var (row, col) = stack.Pop();

                if (row < 0 || row >= Height || col < 0 || col >= Width || visited.Contains((row, col)))
                    continue;

                visited.Add((row, col));

                if (!RelivedCells.Contains(col.ToString() + row.ToString()))
                {
                    Field[row][col] = CompletedField[row][col];
                    RelivedCells.Add(col.ToString() + row.ToString());
                }

                if (CompletedField[row][col] == '0')
                {
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i != 0 || j != 0)
                            {
                                stack.Push((row + i, col + j));
                            }
                        }
                    }
                }
            }

        }


        private void FirstTimeActions(ref GameTurnRequest request)
        {
            Random random = new();

            int placedMines = 0;

            while (placedMines != MinesCount)
            {
                int row = random.Next(Height);
                int col = random.Next(Width);

                if (request.Col == col && request.Row == row)
                    continue;
                else
                {
                    if (Mines.Contains(col.ToString() + row.ToString()))
                        continue;

                    Mines.Add(col.ToString() + row.ToString());
                    placedMines++;
                }
            }

            PlaceMines('M', completedField);
            CalculateNumbers();

            IsFirstAttempt = false;

            if (completedField[request.Row][request.Col] == '0')
                RevealEmptyCells(request.Row, request.Col);
            else
                Field[request.Row][request.Col] = CompletedField[request.Row][request.Col];

            RelivedCells.Add(request.Col.ToString() + request.Row.ToString());
            IsGameWon();
        }

        private bool IsInBounds(int row, int col) => row >= 0 && row < Height && col >= 0 && col < Width;

        private void IsGameWon()
        {
            if (MinesCount + RelivedCells.Count == Height * Width)
            {
                Completed = true;
                Field = CompletedField;
            }
        }

        private void PlaceMines(char currentChar, List<List<char>> board)
        {
            foreach (var mine in Mines)
            {
                int[] colAndRow = mine.Select(x => int.Parse(x.ToString())).ToArray();
                board[colAndRow[1]][colAndRow[0]] = currentChar;
            }
        }

        private void CalculateNumbers()
        {
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (Mines.Contains(j.ToString() + i.ToString()))
                        continue;

                    int minesAround = CountMinesAround(i, j);
                    CompletedField[i][j] = minesAround > 0 ? (char)('0' + minesAround) : '0';
                }
            }
        }

        /// <summary>
        /// Считаем мины вокруг
        /// </summary>
        /// <param name="row">it is I</param>
        /// <param name="col">it is J</param>
        /// <returns>Количество мин</returns>
        private int CountMinesAround(int row, int col)
        {
            int count = 0;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newRow = row + i;
                    int newCol = col + j;

                    if (IsInBounds(newRow, newCol) && Mines.Contains(newCol.ToString() + newRow.ToString()))
                        count++;
                }
            }

            return count;
        }
    }
}