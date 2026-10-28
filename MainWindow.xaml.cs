using Microsoft.Data.SqlClient;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace WPF_RPM
{
    public partial class MainWindow : Window
    {
        // Строка подключения
        private const string ConnectionString =
            @"Server=(localdb)\MSSQLLocalDB;Database=ShopDB;Integrated Security=true;";

        // Коллекция товаров для DataGrid
        private ObservableCollection<Product> _products = new ObservableCollection<Product>();

        // Репозиторий — вся работа с бд
        private readonly ProductRepository _repo;

        public MainWindow()
        {
            InitializeComponent();

            // Создаём репозиторий со строкой подключения
            _repo = new ProductRepository(ConnectionString);

            // Первая загрузка данных
            LoadProducts();
        }

        // Загружает товары и отображает
        private void LoadProducts()
        {
            try
            {
                // обновление товаров
                var list = _repo.Get_All();

                // Очищаем коллекцию
                _products.Clear();

                // Наполняем коллекцию
                foreach (var p in list)
                    _products.Add(p);

                // Привязываем коллекцию к DataGrid
                table_Products.ItemsSource = _products;
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
                // Обновляем счётчик Загруженных записей
                status.Text = $"Загружено записей: {_products.Count}";
            }
        }

        // Обработчик кнопки Обновить
        private void refresh_click(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        // Обработчик кнопки Добавить
        private void add_click(object sender, RoutedEventArgs e)
        {
            try
            {
                // открываем окно добавления
                var new_dialog = new ProductEditWindow(_repo) { Owner = this };

                // если нажали Сохранить
                if (new_dialog.ShowDialog() == true)
                {
                    // Сохраняем в БД и перезагружаем таблицу
                    _repo.Add(new_dialog.Result);
                    LoadProducts();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Ошибка при добавлении:\n" + ex.Message,
                    "Ошибка БД",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Обработчик кнопки Изменить
        private void edit_click(object sender, RoutedEventArgs e)
        {
            // Пытаемся получить выбранный товар
            Product selected = table_Products.SelectedItem as Product;

            // Проверяем, что строка выбрана
            if (selected == null)
            {
                MessageBox.Show(
                    "Выберите товар для редактирования.",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            try
            {
                // открываем окно редактирования
                var new_dialog = new ProductEditWindow(_repo, selected) { Owner = this };

                if (new_dialog.ShowDialog() == true)
                {
                    // Сохраняем изменения в БД и перезагружаем таблицу
                    _repo.Update(new_dialog.Result);
                    LoadProducts();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Ошибка при редактировании:\n" + ex.Message,
                    "Ошибка БД",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Обработчик кнопки Удалить
        private void delete_click(object sender, RoutedEventArgs e)
        {
            // Пытаемся получить выбранный товар
            Product selected = table_Products.SelectedItem as Product;

            // Проверяем, что строка выбрана
            if (selected == null)
            {
                MessageBox.Show(
                    "Выберите товар для удаления.",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            // Подтверждение с именем товара
            var answer = MessageBox.Show(
                $"Удалить товар «{selected.Name}»?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (answer != MessageBoxResult.Yes)
                return;

            try
            {
                // Удаляем из БД и перезагружаем таблицу
                _repo.Delete(selected.Id);
                LoadProducts();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Ошибка при удалении:\n" + ex.Message,
                    "Ошибка БД",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}

