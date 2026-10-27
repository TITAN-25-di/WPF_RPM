-- Создать базу данных (выполнять в SQL Server Management Studio или
-- в окне «Обозреватель объектов SQL Server» в Visual Studio):
CREATE DATABASE ShopDB;
USE ShopDB;
-- Таблица категорий:
CREATE TABLE Categories (
 Id INT PRIMARY KEY IDENTITY(1,1),
 Name NVARCHAR(100) NOT NULL UNIQUE
);
-- Таблица товаров:
CREATE TABLE Products (
 Id INT PRIMARY KEY IDENTITY(1,1),
 Name NVARCHAR(200) NOT NULL,
 Price DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
 Quantity INT NOT NULL DEFAULT 0 CHECK (Quantity >= 0),
 CategoryId INT FOREIGN KEY REFERENCES Categories(Id),
 Description NVARCHAR(500) NULL
);
-- Тестовые данные:
INSERT INTO Categories (Name) VALUES
 (N'Электроника'), (N'Бытовая техника'), (N'Книги'), (N'Одежда');
INSERT INTO Products (Name, Price, Quantity, CategoryId) VALUES
 (N'Ноутбук Lenovo', 49999.00, 10, 1),
 (N'Смартфон Samsung', 34990.00, 5, 1),
 (N'Наушники Sony', 5499.00, 20, 1),
 (N'Холодильник Bosch', 54900.00, 3, 2),
 (N'Стиральная машина LG', 42000.00, 4, 2),
 (N'Микроволновая печь', 8990.00, 15, 2),
 (N'Чистый код (книга)', 1200.00, 50, 3),
 (N'Паттерны проектирования', 890.00, 30, 3),
 (N'Куртка зимняя', 7500.00, 25, 4),
 (N'Джинсы классические', 3200.00, 40, 4);