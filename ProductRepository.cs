using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace WPF_RPM
{
    public class ProductRepository
    {
        // Строка подключения
        private readonly string _connectionString;

        // Конструктор: сохраняем строку подключения
        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // обновление - загружает все товары из бд вместе с названием категории
        public List<Product> Get_All()
        {
            // привязываем коллекцию к result
            var result = new List<Product>();


            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open(); // открыть соединение

                // SQL-запрос выборки товаров с JOIN по категории
                string sql = @"
                    SELECT Products.Id,
                           Products.Name,
                           Products.Price,
                           Products.Quantity,
                           Products.CategoryId,
                           Products.Description,
                           Categories.Name AS Category
                           FROM Products LEFT JOIN Categories ON Products.CategoryId = Categories.Id
                           ORDER BY Products.Name";

                // обновление - перезапись коллекции
                using (var command = new SqlCommand(sql, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read()) // читаем строки по одной
                    {
                        // Создаём объект Product для текущей строки
                        var product = new Product
                        {
                            Id = reader.GetInt32(0), // столбец 0
                            Name = reader.GetString(1), // столбец 1
                            Price = reader.GetDecimal(2), // столбец 2
                            Quantity = reader.GetInt32(3), // столбец 3
                            CategoryId = reader.IsDBNull(4) ? 0 : reader.GetInt32(4), // столбец 4
                            Description = reader.IsDBNull(5) ? null : reader.GetString(5), // столбец 5
                            Category = reader.GetString(6) // столбец 6
                        };
                        // Добавляем товар в коллекцию
                        result.Add(product);
                    }
                }
            }
            return result;
        }

        // Добавляет товар
        public void Add(Product p)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open(); // открыть соединение

                // вставляет новую строку и возвращает ее id
                string sql = @"
                    INSERT INTO Products (Name, Price, Quantity, CategoryId, Description)
                    VALUES (@Name, @Price, @Quantity, @CategoryId, @Description);
                    SELECT SCOPE_IDENTITY();"; 

                using (var command = new SqlCommand(sql, connection))
                {
                    // Параметры — защита от инъекций
                    command.Parameters.AddWithValue("@Name", p.Name);
                    command.Parameters.AddWithValue("@Price", p.Price);
                    command.Parameters.AddWithValue("@Quantity", p.Quantity);
                    command.Parameters.AddWithValue("@CategoryId", p.CategoryId);
                    command.Parameters.AddWithValue("@Description", (object)p.Description ?? DBNull.Value); // иначе сервер может отвергнуть значение
                    p.Id = (int)(decimal)command.ExecuteScalar(); // ExecuteScalar- вставляет значения и возвращает id
                }
            }
        }

        // обновляет товар по id
        public void Update(Product p)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                // обновляет данные по id
                string sql = @"
                    UPDATE Products
                    SET Name = @Name,
                        Price = @Price,
                        Quantity = @Quantity,
                        CategoryId = @CategoryId,
                        Description = @Description
                    WHERE Id = @Id";

                using (var command = new SqlCommand(sql, connection))
                {
                    // Параметры — защита от инъекций
                    command.Parameters.AddWithValue("@Id", p.Id);
                    command.Parameters.AddWithValue("@Name", p.Name);
                    command.Parameters.AddWithValue("@Price", p.Price);
                    command.Parameters.AddWithValue("@Quantity", p.Quantity);
                    command.Parameters.AddWithValue("@CategoryId", p.CategoryId);
                    command.Parameters.AddWithValue("@Description", (object)p.Description ?? DBNull.Value); // иначе сервер может отвергнуть значение
                    
                    command.ExecuteNonQuery(); // возвращает затронутые строки
                }
            }
        }

        // Удаляет товар по Id
        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open(); // открыть соединение

                // Удаляет товар по Id
                string sql = "DELETE FROM Products WHERE Id = @Id";

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery(); // возвращает затронутые строки
                }
            }
        }

        // Названия категорий для ComboBox
        public List<string> Get_Categories()
        {
            var result = new List<string>(); // итоговый список категорий

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open(); // открыть соединение

                // категории по алфавиту
                string sql = "SELECT Name FROM Categories ORDER BY Name";

                using (var command = new SqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read()) // читаем строки по одной
                        result.Add(reader.GetString(0)); // добавляет категории по строчно
                }
            }
            return result;
        }

        // пары (id, name) чтобы по названию найти CategoryId
        public List<(int Id, string Name)> Get_Categories_With_Id()
        {
            // список пар
            var result = new List<(int, string)>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open(); // открыть соединение

                // вызов Id и Name 
                string sql = "SELECT Id, Name FROM Categories ORDER BY Name";

                using (var command = new SqlCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read()) // читаем строки по одной
                        result.Add((reader.GetInt32(0), reader.GetString(1))); // добавляем пару в список
                }
            }
            return result;
        }
    }
}
