namespace MLBTheShowSharp.Models
{
    public class Listing
    {
        public string listing_name { get; set; }

        public int best_sell_price { get; set; }

        public int best_buy_price { get; set; }

        public PlayerItem item { get; set; }

    }
}