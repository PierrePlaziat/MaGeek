#pragma warning disable CS8618 // Un champ non-nullable doit contenir une valeur non-null lors de la fermeture du constructeur. Envisagez de déclarer le champ comme nullable.

using System.ComponentModel.DataAnnotations;

namespace MageekSdk.Data.Mtg.Entities
{
    public class Sets
    {

        public string Svg
        {
            get
            {
                string s = Path.Combine(MageekFolders.SetIcon, Code + ".svg");
                if (File.Exists(s)) return s;
                else return @"\Resources\wut.svg";
            }
        }

        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
        public int TotalSetSize { get; set; }
        public string Type { get; set; }
        public int BaseSetSize { get; set; }

        public string? Block { get; set; }
        public bool? IsFoilOnly { get; set; }
        public bool? IsForeignOnly { get; set; }
        public bool? IsNonFoilOnly { get; set; }
        public bool? IsOnlineOnly { get; set; }
        public bool? IsPartialPreview { get; set; }
        public string? KeyruneCode { get; set; }
        public string? Languages { get; set; }
        public int? McmId { get; set; }
        public int? McmIdExtras { get; set; }
        public string? McmName { get; set; }
        public string? MtgoCode { get; set; }
        public string? ParentCode { get; set; }
        public string ReleaseDate { get; set; }
        public int? TcgplayerGroupId { get; set; }
        public string? TokenSetCode { get; set; }

    }

}
