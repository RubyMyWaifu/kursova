using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using OfficeOpenXml;
using System.IO;
using Microsoft.Win32;
using System.Windows;


namespace A; // Змініть на ваш фактичний простір імен

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Anime> Animes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Додайте налаштування моделей, якщо потрібно
    }
}

