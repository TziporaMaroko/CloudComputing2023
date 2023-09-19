namespace ZeldaWebsite.Models
{
    public class CartView
    {
        public List<CartItem> CartItems { get; set; }
        public List<Flavour> Flavours { get; set; }

        public double Total()
        {
            double total = 0;
            foreach (var item in CartItems) { total += item.Price; }
            return total;
        }
    }
}
