using Microsoft.EntityFrameworkCore;
using Task3.Models;
using System;
using System.Reflection.PortableExecutable;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Task3.Data
{
    /// <summary>
    /// Класс для инициализации бд 
    /// </summary>
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        // Таблица заметок в бд.
        public virtual DbSet<Note> Notes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions)
             : base(dbContextOptions)
        {
           
        }

      
    }
}
