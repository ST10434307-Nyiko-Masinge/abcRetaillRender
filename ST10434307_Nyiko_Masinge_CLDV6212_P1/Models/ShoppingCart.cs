namespace ST10434307_Nyiko_Masinge_CLDV6212_P1.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Total => Items.Sum(item => item.Subtotal);
        public int ItemCount => Items.Sum(item => item.Quantity);
    }
}

