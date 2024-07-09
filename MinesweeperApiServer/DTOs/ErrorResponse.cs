using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MinesweeperApiServer.Models
{
    public class ErrorResponse(string error)
    {
        [Required, DefaultValue("Произошла непредвиденная ошибка")]
        public string Error { get; set; } = error;
    }
}
