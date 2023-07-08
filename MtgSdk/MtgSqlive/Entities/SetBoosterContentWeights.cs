namespace MageekSdk.MtgSqlive.Entities
{
    public class SetBoosterContentWeights
    {
        public int BoosterIndex { get; set; }
        public string BoosterName { get; set; }
        public string SetCode { get; set; }
        public string SheetName { get; set; }
        public int SheetPicks { get; set; }
    }
}
