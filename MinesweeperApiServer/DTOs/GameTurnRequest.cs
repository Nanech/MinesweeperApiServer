using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MinesweeperApiServer.Models
{
    public class GameTurnRequest
    {
        [Required,DefaultValue("01234567-89AB-CDEF-0123-456789ABCDEF"), JsonProperty("game_id"), JsonPropertyName("game_id")]
        public required string GameId { get; set; }

        /// <summary>
        /// Номер колонки
        /// </summary>
        [Required, DefaultValue(5)]
        public required int Col { get; set; }

        /// <summary>
        /// Номер ряда
        /// </summary>
        [Required, DefaultValue(5)]
        public required int Row { get; set; }
    }
}
