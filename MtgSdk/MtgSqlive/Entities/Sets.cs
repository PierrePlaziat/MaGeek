namespace MageekSdk.MtgSqlive.Entities
{
    public class Sets
    {
        public int baseSetSize { get; set; }
        public string block { get; set; }
        public int cardsphereSetId { get; set; }
        public string code { get; set; }
        public string decks { get; set; }
        public bool isFoilOnly { get; set; }
        public bool isForeignOnly { get; set; }
        public bool isNonFoilOnly { get; set; }
        public bool isOnlineOnly { get; set; }
        public bool isPartialPreview { get; set; }
        public string keyruneCode { get; set; }
        public string languages { get; set; }
        public int mcmId { get; set; }
        public int mcmIdExtras { get; set; }
        public string mcmName { get; set; }
        public string mtgoCode { get; set; }
        public string name { get; set; }
        public string parentCode { get; set; }
        public string releaseDate { get; set; }
        public int tcgplayerGroupId { get; set; }
        public string tokenSetCode { get; set; }
        public int totalSetSize { get; set; }
        public string type { get; set; }
    }
}
