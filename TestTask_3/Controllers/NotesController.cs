using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task3.Data;
using Task3.Models;

namespace Task3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// Данный класс представляет из себя класс контроллер,
    /// который позволяет обрабатывать все действия с заметками
    /// </summary>
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _authContext; // Контекст базы данных
        private readonly IMemoryCache _memoryCache; // Кэширование
        private readonly ILog _logger; // Логгер

        public NotesController(AppDbContext appDbContext, IMemoryCache memoryCache)
        {
            _authContext = appDbContext;
            _memoryCache = memoryCache;
            _logger = LogManager.GetLogger(typeof(NotesController)); // Инициализация логгера
            Initialize().Wait();
        }

        private async Task Initialize()
        {
            if (!await CheckDatabaseConnectionAsync())
            {
                // Обработка ошибки подключения к базе данных
                _logger.Error("Database connection error");
            }
        }

        private async Task<bool> CheckDatabaseConnectionAsync() // Обработка подключения к бд
        {
            try
            {
                await _authContext.Database.OpenConnectionAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpGet("GetNotes")]
        public IActionResult GetNotes()
        {
            try
            {
                // Получить список всех заметок из базы данных и вернуть их как ответ с кодом 200.
                var notes = _authContext.Notes.ToList();
                // Логирование
                _logger.Info($"GetNotes: withdrawn {notes.Count()} notes"); // Логирование
                return Ok(notes);
            }
            catch (Exception ex)
            {
                _logger.Error("GetNotes Error:" + ex); // Логирование
                // В случае ошибки вернуть ответ с кодом 500
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("AddNote")]
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
                note.UpdatedAt = DateTime.Now.ToString();
                note.UpdatedAt = DateTime.Now.ToString();
                _authContext.Notes.Add(note);
                _authContext.SaveChanges();
                _logger.Info($"AddNote: added one note where id = {note.Id}"); // Логирование
                // Вернуть ответ с кодом 201 и заголовком, указывающим на созданную заметку.
                return CreatedAtRoute("GetNote", new { id = note.Id }, note);
            }
            catch (Exception ex)
            {
                _logger.Error("AddNote Error:" + ex); // Логирование
                // В случае возникновения ошибки вернуть ответ с кодом 500.
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("ChangeNote/{id}", Name = "ChangeNote")]
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
                // Сохранить изменения в базе данных.
                _authContext.SaveChanges();
                _logger.Info($"ChangeNote: change one note where id = {existingNote.Id}"); // Логирование

                // Вернуть ответ с кодом 204, без возвращения данных.
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error("ChangeNote Error:" + ex); // Логирование
                // В случае возникновения ошибки вернуть ответ с кодом 500 и сообщением об ошибке.
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpDelete("DeleteNote/{id}", Name = "DeleteNote")]
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
                _logger.Info($"DeleteNote: delete one note where id = {existingNote.Id}"); // Логирование
                // Вернуть ответ с кодом 204, без возвращения данных.
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error("DeleteNote Error:" + ex); // Логирование
                // В случае возникновения ошибки вернуть ответ с кодом 500.
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetNote/{id}", Name = "GetNote")]
        public IActionResult GetNote(int id)
        {
            try
            {
                // Попытка найти заметку в базе данных по заданному id.
                var note = _authContext.Notes.FirstOrDefault(n => n.Id == id);

                if (note == null)
                {
                    // Если заметка не найдена, вернуть ответ с кодом 404.
                    // Если заметка не найдена, вернуть ответ с кодом 404.
                    return NotFound();
                }

                // Вернуть найденную заметку как ответ с кодом 200.
                _logger.Info($"GetNote: withdrawn one notes where id = {note.Id}"); // Логирование

                return Ok(note);
            }
            catch (Exception ex)
            {
                _logger.Error("GetNote Error:" + ex); // Логирование
                // В случае возникновения ошибки вернуть ответ с кодом 500.
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
