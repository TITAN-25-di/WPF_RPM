using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace WPF_RPM
{
    public partial class ProductEditWindow : Window
    {
        // Репозиторий — вся работа с бд
        private readonly ProductRepository _repo;

        // Результат - заполняется при успешной валидации
        public Product Result { get; private set; }

        // id для добавления или редактирования
        private readonly int _editId;

        // Пары (Id, Name) для поиска CategoryId по выбранному названию
        private List<(int Id, string Name)> _categories = new List<(int, string)>();

        // Конструктор без параметров — режима добавления
        public ProductEditWindow(ProductRepository repo)
        {
            InitializeComponent();
            _repo = repo;
            _editId = 0;
            Title = "Добавить товар";
            LoadCategories();
        }

        // Конструктор с Product — режим редактирования
        public ProductEditWindow(ProductRepository repo, Product product) : this(repo)
        {
            _editId = product.Id;
            Title = "Редактировать";

            // текущие значения товара
            product_name.Text = product.Name;
            product_price.Text = product.Price.ToString();
            product_quantity.Text = product.Quantity.ToString();
            product_description.Text = product.Description;

            //текущая категорию по названию
            if (!string.IsNullOrEmpty(product.Category))
                product_category.SelectedItem = product.Category;
        }

        // Заполняет ComboBox категориями из бд
        private void LoadCategories()
        {
            try
            {
                var names = _repo.Get_Categories(); // названия для ComboBox

                _categories = _repo.Get_Categories_With_Id(); // Пары (Id, Name)

                product_category.ItemsSource = names; // отправка в ComboBox
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка загрузки категорий:\n" + ex.Message,
                    "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // валидация и сохранение
        private void save_click(object sender, RoutedEventArgs e)
        {
            // Название
            if (string.IsNullOrWhiteSpace(product_name.Text))
            {
                MessageBox.Show("Введите название товара.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                product_name.Focus();
                return;
            }

            // Цена (decimal > 0)
            if (!decimal.TryParse(product_price.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Цена должна быть числом больше 0.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                product_price.Focus();
                return;
            }

            // Количество (int >= 0)
            if (!int.TryParse(product_quantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Количество должно быть целым числом >= 0.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                product_quantity.Focus();
                return;
            }

            // Категория
            if (product_category.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                product_category.Focus();
                return;
            }
            // CategoryId по выбранному названию
            string selectedName = product_category.SelectedItem.ToString();
            int categoryId = _categories.FirstOrDefault(c => c.Name == selectedName).Id;

            // Формируем Result
            Result = new Product
            {
                Id = _editId,
                Name = product_name.Text.Trim(),
                Price = price,
                Quantity = quantity,
                CategoryId = categoryId,
                Category = selectedName,
                Description = string.IsNullOrWhiteSpace(product_description.Text)
                    ? null
                    : product_description.Text.Trim()
            };
            DialogResult = true;
        }
    }
}
