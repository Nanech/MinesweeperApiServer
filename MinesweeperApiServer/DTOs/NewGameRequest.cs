using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MinesweeperApiServer.Models
{
    public class NewGameRequest(int width, int height, int minesCount)
    {
        /// <summary>
        /// Ширина поля
        /// </summary>
        [Required, DefaultValue(10), Range(2, 30, ErrorMessage = "Вы вышли из допустимых значений")]
        public int Width { get; set; } = width;
        /// <summary>
        /// Высота поля
        /// </summary>
        [Required, DefaultValue(10), Range(2, 30, ErrorMessage = "Вы вышли из допустимых значений")]
        public int Height { get; set; } = height;
        /// <summary>
        /// Количество мин 
        /// </summary>
        [Required, DefaultValue(10), JsonPropertyName("mines_count"), JsonProperty("mines_count")]
        public int MinesCount { get; set; } = minesCount;
    }
}
