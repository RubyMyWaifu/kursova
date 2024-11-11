using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using OfficeOpenXml;
using System.IO;
using Microsoft.Win32;
using System.Windows;


namespace A; 


public class Anime : INotifyDataErrorInfo 
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Genre { get; set; }
    public string MangaAuthor { get; set; }
    public DateTime ReleaseDate { get; set; }

    private double _rating;

    [Range(1, 10, ErrorMessage = "Рейтинг повинен бути в діапазоні від 1 до 10")]
    public double Rating
    {
        get => _rating;
        set
        {
            _rating = value;
            ValidateProperty(value, nameof(Rating));
        }
    }
    private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    // Метод для перевірки наявності помилок
    public bool HasErrors => _errors.Count > 0;

    // Отримання помилок для певної властивості
    public IEnumerable GetErrors(string propertyName)
    {
        return _errors.ContainsKey(propertyName) ? _errors[propertyName] : null;
    }

    // Метод для перевірки властивостей
    private void ValidateProperty(object value, string propertyName)
    {
        _errors.Remove(propertyName);

        var validationContext = new ValidationContext(this) { MemberName = propertyName };
        var results = new List<ValidationResult>();

        // Перевірка за допомогою атрибутів DataAnnotations
        if (!Validator.TryValidateProperty(value, validationContext, results))
        {
            _errors[propertyName] = new List<string>();
            foreach (var validationResult in results)
            {
                _errors[propertyName].Add(validationResult.ErrorMessage);
            }
        }

        // Викликаємо подію для оновлення UI
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }
}
