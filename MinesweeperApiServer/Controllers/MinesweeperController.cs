using Microsoft.AspNetCore.Mvc;
using MinesweeperApiServer.Models;

namespace MinesweeperApiServer.Controllers
{
    [ApiController]
    [Route("api/")]
    public class MinesweeperController : Controller
    {
        static Dictionary<string, GameInfoResponse> Games = [];

        [HttpPost("new")]
        public ActionResult<GameInfoResponse> NewGame([FromBody] NewGameRequest settings)
        {
            int acceptableValueMines = settings.Width * settings.Height - 1;

            if (settings.MinesCount <= acceptableValueMines)
            {
                string gameId;

                do
                {
                    gameId = Guid.NewGuid().ToString();
                } while (Games.ContainsKey(gameId));

                var game = new GameInfoResponse(gameId, settings.Width, settings.Height, settings.MinesCount);
                Games.Add(gameId, game);

                return Ok(game);
            }

            return BadRequest(new ErrorResponse("Game settings is wrong"));
        }

        [HttpPost("turn")]
        public ActionResult<GameInfoResponse> Turn([FromBody] GameTurnRequest request)
        {
            if (!Games.TryGetValue(request.GameId, out GameInfoResponse? value))
                return BadRequest(new ErrorResponse("Game is may not created, and not found"));

            var currentGame = value;

            if (currentGame.Completed)
                return BadRequest(new ErrorResponse("Game is ended"));

            if (request.Row >= 0 && request.Row < currentGame.Height && request.Col >= 0 && request.Col < currentGame.Width)
            {
                currentGame.MakeAnAttempt(ref request);
                return Ok(currentGame);
            }

            return BadRequest(new ErrorResponse("You have made an attempt to invalid values"));
        }

    }
}