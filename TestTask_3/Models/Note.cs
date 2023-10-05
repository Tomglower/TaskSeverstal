using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Task3.Models
{
    /// <summary>
    /// Класс модели заметки, содержащий различные свойства
    /// </summary>
    public class Note
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public override string ToString()
        {
            return $"Id: {Id}\n Title: {Title}\n Description: {Description}\n CreatedAt: {CreatedAt}\n UpdatedAt: {UpdatedAt}";
        }
    }
}
