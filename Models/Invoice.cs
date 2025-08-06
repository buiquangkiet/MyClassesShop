namespace MyClassesShop.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }

        public int OrderId { get; set; }

        public string FilePath { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Order Order { get; set; }
    }

}
