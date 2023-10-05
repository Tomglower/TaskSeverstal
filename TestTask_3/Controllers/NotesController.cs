using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using log4net;
using System.Data;
using Task3.Data;
using Task3.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Task3.Controllers
{
    [Route("[controller]")]
    [ApiController]

    /// <summary>
    ///   Данный класс представляет из себя класс контроллер,
    ///   который позволяет обрабатывать все действия с заметками
    /// </summary>
    public class NotesController : ControllerBase
    {
        
        private readonly AppDbContext _authContext; // Контекст базы данных
        private readonly IMemoryCache _memoryCache; // Кэширование
        private readonly ILog _logger; // Логгер

        public NotesController(AppDbContext appDbContext,IMemoryCache memoryCache)
        {
            _authContext = appDbContext;
            _memoryCache = memoryCache; 
            _logger = LogManager.GetLogger(typeof(NotesController)); // Инициализация логгера

        }


        [HttpGet]
         public IActionResult GetNotes()
         {
             try
             {
                // Получить список всех заметок из базы данных и вернуть их как ответ с кодом 200.
                 var notes = _authContext.Notes.ToList();
                //Логирование
                _logger.Info($"GetNotes: withdrawn {notes.Count()} notes"); //Логирование
                return Ok(notes);
             }
             catch (Exception ex)
             {
                 _logger.Error("GetNotes Error:" + ex); //Логирование
                // В случае ошибки вернуть ответ с кодом 500
                return StatusCode(500, "Internal Server Error");
             }
         }


        /// <summary>
        ///   Пример написания метода GetNotes с использованием кеширования. Но кеширование в этом проекте особо и не нужно.
        /// </summary>

        /*[HttpGet]
        public IActionResult GetNotes()
        {
            try
            {
                // Получение данных из кеша
                if (_memoryCache.TryGetValue("notes", out var cachedNotes))
                {
                    return Ok(cachedNotes);
                }

                // Если данные не найдены в кеше, получаем данные из бд 
                var notes = _authContext.Notes.ToList();

                // Помещение данных в кеш
                _memoryCache.Set("notes", notes, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) 
                });

                return Ok(notes);
            }
            catch (Exception ex)
            {
                // В случае ошибки вернуть ответ с кодом 500
                return StatusCode(500, "Internal Server Error");
            }
        }*/

        [HttpPost]
        public IActionResult AddNote([FromBody] Note note)
        {
            try
            {
                if (note is null)
                {
                    // Если данные заметки недействительны, вернуть ответ с кодом 400.
                    return BadRequest("Invalid data");
                }

                // Добавить новую заметку в базу данных и сохранить изменения.
                note.CreatedAt = DateTime.Now.ToString();
                _authContext.Notes.Add(note);
                _authContext.SaveChanges();
                _logger.Info($"AddNote: added  one note where id = {note.Id}"); //Логирование
                // Вернуть ответ с кодом 201 и заголовком, указывающим на созданную заметку.
                return CreatedAtRoute("GetNote", new { id = note.Id }, note);
            }
            catch (Exception ex)
            {
                _logger.Error("AddNote Error:" + ex); //Логирование
                // В случае возникновения ошибки вернуть ответ с кодом 500.
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("{id}")]
        public IActionResult ChangeNote(int id, [FromBody] Note updatedNote)
        {
            try
            {
                if (updatedNote is null || updatedNote.Id != id)
                {
                    // Если данные заметки недействительны или id не соответствует, вернуть ответ с кодом 400.
                    return BadRequest("Invalid data");
                }

                // Найти существующую заметку в базе данных.
                var existingNote = _authContext.Notes.Find(id);

                if (existingNote == null)
                {
                    // Если заметка не найдена, вернуть ответ с кодом 404.
                    return NotFound();
                }

                // Обновить поля заметки.
                existingNote.Title = updatedNote.Title;
                existingNote.Description = updatedNote.Description;
                existingNote.UpdatedAt = DateTime.Now.ToString();
                _logger.Info($"ChangeNote: change  one note where id = {existingNote.Id}"); //Логирование
                // Сохранить изменения в базе данных.
                _authContext.SaveChanges();

                // Вернуть ответ с кодом 204, без возвращения данных.
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error("ChangeNote Error:" + ex); //Логирование
                // В случае возникновения ошибки вернуть ответ с кодом 500 и сообщением об ошибке.
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteNote(int id)
        {
            try
            {
                // Попытка найти существующую заметку в базе данных по заданному id.
                var existingNote = _authContext.Notes.Find(id);

                if (existingNote == null)
                {
                    // Если заметка не найдена, вернуть ответ с кодом 404.
                    return NotFound();
                }

                // Удалить существующую заметку из базы данных и сохранить изменения.
                _authContext.Notes.Remove(existingNote);
                _authContext.SaveChanges();
                _logger.Info($"DeleteNote: delete  one note where id = {existingNote.Id}"); //Логирование
                // Вернуть ответ с кодом 204 ,без возвращения данных.
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error("DeleteNote Error:" + ex); //Логирование
                // В случае возникновения ошибки вернуть ответ с кодом 500.
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{id}", Name = "GetNote")]
        public IActionResult GetNote(int id)
        {
            try
            {
                // Попытка найти заметку в базе данных по заданному id.
                var note = _authContext.Notes.FirstOrDefault(n => n.Id == id);

                if (note == null)
                {
                    // Если заметка не найдена, вернуть ответ с кодом 404.
                    return NotFound();
                }

                // Вернуть найденную заметку как ответ с кодом 200.
                return Ok(note);
            }
            catch (Exception ex)
            { 
                _logger.Error("GetNote Error:" + ex); //Логирование
                // В случае возникновения ошибки вернуть ответ с кодом 500.
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
