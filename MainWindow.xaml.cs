using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace WPF_RPM
{
    public partial class MainWindow : Window
    {
        // Строка подключения к LocalDB
        private const string ConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=ShopDB;Integrated Security=true;";

        // Коллекция товаров, которую показывает DataGrid
        // ObservableCollection уведомляет DataGrid об изменениях автоматически
        private ObservableCollection<Product> _products = new ObservableCollection<Product>();

        public MainWindow()
        {
            InitializeComponent();

            // Первичная загрузка данных при открытии окна
            LoadProducts();
        }

        // Загружает товары из БД и отображает их
        // наполняет коллекцию, обновляет счётчик
        private void LoadProducts()
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    // привязываем коллекцию к DataGrid
                    table_Products.ItemsSource = _products;

                    connection.Open(); // открыть соединение

                    // SQL-запрос выборки товаров с JOIN по категории
                    string sql = @"
                        SELECT Products.Id,
                               Products.Name,
                               Products.Price,
                               Products.Quantity,
                               Categories.Name AS Category
                               FROM Products LEFT JOIN Categories ON Products.CategoryId = Categories.Id
                               ORDER BY Products.Name";

                    //
                    using (var command = new SqlCommand(sql, connection))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Очищаем коллекцию, чтобы не дублировать
                        _products.Clear();

                        while (reader.Read()) // читаем строки по одной
                        {
                            // Создаём объект Product для текущей строки
                            var product = new Product
                            {
                                Id = reader.GetInt32(0), // столбец 0
                                Name = reader.GetString(1), // столбец 1
                                Price = reader.GetDecimal(2), // столбец 2
                                Quantity = reader.GetInt32(3), // столбец 3
                                Category = reader.GetString(4) // столбец 4
                            };

                            // Добавляем товар в коллекцию
                            _products.Add(product);
                        }
                    }
                }
            }
            // SQL исключения
            catch (SqlException ex) 
            {
                MessageBox.Show(
                    "Ошибка при работе с базой данных: " + ex.Message,
                    "Ошибка БД",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            // любые другие исключения
            catch (Exception ex) 
            {
                MessageBox.Show(
                    "Непредвиденная ошибка: " + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                // Обновляем счётчик Загруженых записей
                status.Text = $"Загружено записей: {_products.Count}";
            }
        }

        // Обработчик кнопки Обновить - перезагружает данные
        private void refresh_click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }
    }
}

