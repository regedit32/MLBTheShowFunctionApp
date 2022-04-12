namespace MLBTheShowSharp.Models
{
    public class PlayerItem
    {
        public string uuid { get; set; }
        public string type { get; set; }
        public string img { get; set; }
        public string name { get; set; }
        public string rarity { get; set; }
        public string team { get; set; }
        public string team_short_name { get; set; }
        public int ovr { get; set; }
        public string series { get; set; }
        public string series_texture_name { get; set; }
        public int series_year { get; set; }
        public string display_position { get; set; }
        public bool has_augment { get; set; }
    }
}