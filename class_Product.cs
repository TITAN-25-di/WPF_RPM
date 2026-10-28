
namespace WPF_RPM
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }

        // CategoryId для изменений БД
        public int CategoryId { get; set; }

        // Description — Описание
        public string Description { get; set; }


        public override string ToString() => $"{Name} ({Price:F2} руб.)";
    }
}
