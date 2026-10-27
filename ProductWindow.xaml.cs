using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace WPF_RPM
{
    /// <summary>
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        // Свойство результата
        public Product Result { get; private set; }

        // Конструктор для добавления
        public ProductWindow()
        {
            InitializeComponent();
            Title = "Добавить товар";
        }

        // Конструктор для редактирования
        public ProductWindow(Product product) : this()
        {
            Name.Text = product.Name;
            Price.Text = product.Price.ToString().Replace(',', '.');
            value.Text = product.Quantity.ToString();
            Title = "Редактировать товар";
        }

        // проверка
        private void Click_ok(object sender, RoutedEventArgs e)
        {
            // Название
            if (string.IsNullOrWhiteSpace(Name.Text)) // проверка на пустоту
            {
                MessageBox.Show("Введите название товара",
                                "Предупреждение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                Name.Focus();
                return;
            }

            // Цена
            if (!decimal.TryParse(Price.Text.Replace(',', '.'), // проверка числа
                                  out decimal price) || price <= 0)
            {
                MessageBox.Show("Цена не указана или не больше 0 или неверный формат",
                                "Предупреждение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                Price.Focus();
                return;
            }

            // Количество
            if (!int.TryParse(value.Text, out int quantity) || quantity < 0) // проверка числа
            {
                MessageBox.Show("Количество не указано или не целое число или не больше 0",
                                "Предупреждение",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                value.Focus();
                return;
            }

            // Успешная проверка
            Result = new Product
            {
                Name = Name.Text.Trim(),
                Price = price,
                Quantity = quantity
            };

            DialogResult = true;
        }
    }
}
