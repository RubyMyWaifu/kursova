using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using A;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace WpfApp1
{
    public partial class ChartWindow : Window
    {
        public ChartWindow(List<Anime> animeData)
        {
            InitializeComponent();

            // Отримуємо дати виходу, рейтинги та назви аніме
            var dates = animeData.Select(a => a.ReleaseDate.ToString("yyyy-MM-dd")).ToList();
            var ratings = animeData.Select(a => a.Rating).ToList();
            var animeTitles = animeData.Select(a => a.Title).ToList();

            // Створюємо серію з ToolTip для відображення назви та рейтингу при наведенні
            var series = new LineSeries
            {
                Title = "Rating",
                Values = new ChartValues<double>(ratings),
                DataLabels = false, // Вимкнено постійне відображення міток
                LabelPoint = point =>
                {
                    int index = (int)point.X;
                    return $"Назва: {animeTitles[index]}\nРейтинг: {point.Y}";
                }
            };

            releaseDateRatingChart.Series = new SeriesCollection { series };

            // Призначення осі X з мітками для дат виходу
            releaseDateRatingChart.AxisX[0].Labels = dates;
            releaseDateRatingChart.AxisX[0].Title = "Дата виходу";
            releaseDateRatingChart.AxisY[0].Title = "Рейтинг";
        }
    }
}

