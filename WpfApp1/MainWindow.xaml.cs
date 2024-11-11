using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using A;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using OfficeOpenXml;
using System.IO;
using Microsoft.Win32;
using LiveCharts;
using LiveCharts.Wpf;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _context;
        public MainWindow()
        {
            InitializeComponent();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=C:\\Users\\Komp\\source\\repos\\AnimeTrackingApp\\AnimeTrackingApp\\your_database_name.db");
            _context = new AppDbContext(optionsBuilder.Options);

             _ =LoadData();
        }
        private async Task LoadData()
        {
                dataGrid.ItemsSource = await _context.Animes.ToListAsync();
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_context == null)
            {
                MessageBox.Show("не ну воно ж працює ", "пон", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string searchTerm = txtSearch.Text.ToLower();

            var filteredData = _context.Animes
                                       .Where(a => a.Title.ToLower().Contains(searchTerm))
                                       .ToList();

            dataGrid.ItemsSource = filteredData;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var newAnime = new Anime
            {
                Title = "Введіть назву",       // Додайте відповідні дані
                Genre = "Введіть жанр",        // Ви можете створити діалог для введення даних
                MangaAuthor = "Введіть автора манги",
                ReleaseDate = DateTime.Now,    // Задайте дату
                Rating = 1                    // Початковий рейтинг
            };
            if (newAnime.Rating < 1 || newAnime.Rating > 10)
            {
                // Виводимо повідомлення, якщо рейтинг не в діапазоні від 1 до 10
                MessageBox.Show("Будь ласка, введіть значення рейтингу від 1 до 10.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Якщо все правильно, додаємо новий запис до бази даних
            _context.Animes.Add(newAnime);
            await _context.SaveChangesAsync();  // Використовуємо async/await

            // Оновлюємо дані на екрані
            await LoadData();
        }
        private async void btnDelete_Click_1(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem is Anime selectedAnime)
            {
                var result = MessageBox.Show($"Ви впевнені, що хочете видалити '{selectedAnime.Title}'?", "Підтвердження видалення", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    // Лог для перевірки видалення
                    MessageBox.Show($"Видаляється '{selectedAnime.Title}' з бази даних.");

                    // Видалення з контексту
                    _context.Animes.Remove(selectedAnime);

                    // Збереження змін в базі даних
                    await _context.SaveChangesAsync();

                    // Оновіть джерело даних
                    dataGrid.ItemsSource = await _context.Animes.ToListAsync();
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть аніме для видалення.");
            }

        }
        private void ValidateRatingInput()
        {
            // Отримуємо вибраний рядок в DataGrid
            var selectedRow = dataGrid.SelectedItem as DataRowView; // або інший тип, який ви використовуєте для вашої моделі

            if (selectedRow != null)
            {
                // Отримуємо значення з клітинки, припустимо, що Rating знаходиться в стовпці з індексом 5
                var ratingValue = selectedRow[5]?.ToString(); // 5 — це індекс стовпця, де знаходиться рейтинг

                if (double.TryParse(ratingValue, out double enteredRating))
                {
                    if (enteredRating < 1.0 || enteredRating > 10.0)
                    {
                        MessageBox.Show("Будь ласка, введіть рейтинг від 1 до 10.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Будь ласка, введіть коректне число.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column.Header.ToString() == "Rating") // Якщо редагується стовпець Rating
            {
                var cell = e.EditingElement as TextBox;
                if (cell != null && int.TryParse(cell.Text, out int enteredRating))
                {
                    if (enteredRating < 1 || enteredRating > 10)
                    {
                        MessageBox.Show("Будь ласка, введіть рейтинг від 1 до 10.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Будь ласка, введіть коректне число.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private async void btnRefresh_Click(object sender, RoutedEventArgs e) 
        {
            try
            {
                // Оновлення даних із бази
                var updatedData = await _context.Animes.ToListAsync();

                // Оновлюємо ItemsSource для DataGrid
                dataGrid.ItemsSource = updatedData;

                // Збереження змін у базі даних
                await _context.SaveChangesAsync();

                MessageBox.Show("Зміни успішно збережені в базі даних.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Обробка помилок
                MessageBox.Show($"Сталася помилка при збереженні даних: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

    
        }

        private async void btnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            if (dataGrid.ItemsSource == null || !(dataGrid.ItemsSource is IEnumerable<Anime>))
            {
                MessageBox.Show("Немає даних для експорту.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var animeList = dataGrid.ItemsSource;
            if (animeList is IEnumerable<Anime> list)
            {
                try
                {
                    // Створення Excel-пакету
                    using (var package = new ExcelPackage())
                    {
                        // Додавання аркуша в Excel
                        var worksheet = package.Workbook.Worksheets.Add("Animes");

                        // Створення заголовків
                        worksheet.Cells[1, 1].Value = "ID";
                        worksheet.Cells[1, 2].Value = "Title";
                        worksheet.Cells[1, 3].Value = "Genre";
                        worksheet.Cells[1, 4].Value = "Manga Author";
                        worksheet.Cells[1, 5].Value = "Release Date";
                        worksheet.Cells[1, 6].Value = "Rating";

                        int row = 2;

                        // Заповнення даними з DataGrid

                        foreach (var anime in list)
                        {
                            worksheet.Cells[row, 1].Value = anime.Id;
                            worksheet.Cells[row, 2].Value = anime.Title;
                            worksheet.Cells[row, 3].Value = anime.Genre;
                            worksheet.Cells[row, 4].Value = anime.MangaAuthor;
                            worksheet.Cells[row, 5].Value = anime.ReleaseDate.ToString("yyyy-MM-dd");
                            worksheet.Cells[row, 6].Value = anime.Rating;
                            row++;
                        }

                        // Збереження файлу Excel
                        var fileDialog = new SaveFileDialog
                        {
                            Filter = "Excel Files (*.xlsx)|*.xlsx",
                            DefaultExt = ".xlsx"
                        };

                        if (fileDialog.ShowDialog() == true)
                        {
                            // Зберігаємо файл
                            var filePath = fileDialog.FileName;
                            FileInfo fileInfo = new FileInfo(filePath);
                            await Task.Run(() => package.SaveAs(fileInfo));

                            MessageBox.Show("Дані успішно експортовані в Excel!", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Обробка помилок
                    MessageBox.Show($"Сталася помилка при експорті: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Невірний тип даних у DataGrid.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
            private void btnShowChart_Click(object sender, RoutedEventArgs e)
            {
                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite("Data Source=C:\\Users\\Komp\\source\\repos\\AnimeTrackingApp\\AnimeTrackingApp\\your_database_name.db")
                    .Options;

                using (var context = new AppDbContext(options))
                {
                    var animeData = context.Animes
                                           .OrderBy(a => a.ReleaseDate)
                                           .ToList();
                    var chartWindow = new ChartWindow(animeData);
                    chartWindow.Show();
                }
            }

        private void genreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = genreComboBox.SelectedItem as ComboBoxItem;

            if (selectedItem != null)
            {
                string selectedGenre = selectedItem.Content.ToString();

                // Оновлюємо жанр для вибраної аніме
                UpdateGenreForSelectedAnime(selectedGenre);
            }

        }
        private void UpdateGenreForSelectedAnime(string newGenre)
        {
            var selectedAnime = dataGrid.SelectedItem as Anime; // Отримуємо вибрану аніме з DataGrid

            if (selectedAnime != null)
            {
                // Оновлюємо жанр для вибраної аніме
                selectedAnime.Genre = newGenre;

                var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite("Data Source=C:\\Users\\Komp\\source\\repos\\AnimeTrackingApp\\AnimeTrackingApp\\your_database_name.db")
                    .Options;

                using (var context = new AppDbContext(options))
                {
                    context.Animes.Update(selectedAnime); // Оновлюємо запис
                    context.SaveChanges(); // Зберігаємо зміни в базі даних
                }

                // Оновлення DataGrid
                dataGrid.Items.Refresh();
                MessageBox.Show("Жанр успішно оновлено!");
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть аніме для оновлення жанру.");
            }
        }

        private void genre1ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedGenre = (genre1ComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (!string.IsNullOrEmpty(selectedGenre))
            {
                // Виконуємо фільтрацію
                FilterAnimeByGenre(selectedGenre);
            }

        }
        private void FilterAnimeByGenre(string genre)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlite("Data Source=C:\\Users\\Komp\\source\\repos\\AnimeTrackingApp\\AnimeTrackingApp\\your_database_name.db")
                    .Options;
            using (var context = new AppDbContext(options))
            {
                // Якщо вибрано "Усі жанри", відображаємо всі записи
                if (genre == "Усі жанри")
                {
                    dataGrid.ItemsSource = context.Animes.ToList(); // Відображаємо всі аніме
                }
                else
                {
                    // Фільтруємо за жанром
                    dataGrid.ItemsSource = context.Animes.Where(a => a.Genre == genre).ToList();
                }
            }
        }


    }
}