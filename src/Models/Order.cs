
namespace PoorFoodie.Models
{
    public class Order
    {
        public Order(string number, string client, string soup, string mainDish, string basePrice, string discountPrice)
        {
            this.Number = number;
            this.Client = client;
            this.Soup = soup;
            this.MainDish = mainDish;
            this.BasePrice = basePrice;
            this.DiscountPrice = discountPrice;

        }

        public readonly string Number;
        public readonly string Client;
        public readonly string Soup;
        public readonly string MainDish;
        public readonly string BasePrice;
        public readonly string DiscountPrice;
    }
}