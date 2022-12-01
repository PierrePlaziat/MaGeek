using System;
using Newtonsoft.Json;
using System.Net;
using ScryfallApi.Client;
using MaGeek.Data.Entities;
using ScryfallApi.Client.Models;
using System.Windows;
using System.Linq;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace MaGeek
{

    public class Price
    {
        [Key]
        public string MultiverseId { get; set; }
        public string LastUpdate { get; set; }
        public string Value { get; set; }
    }

    public static class PriceManager
    {
        static ScryfallApiClient scryfallApi { get; } = new ScryfallApiClient(new System.Net.Http.HttpClient()) { };

        public static float GetCardPrize(MagicCardVariant variant)
        {
            if (variant == null) return 0;
            float price = 0;

            var v = App.DB.Prices.Where(x => x.MultiverseId == variant.MultiverseId).FirstOrDefault();
            if (v == null) price = RetrievePrice(variant);
            else if (IsOutDated(v.LastUpdate)) price = RetrievePrice(variant);
            else price = float.Parse(v.Value);
            return price;
        }
        
        public static float GetCardPrize(string variantId)
        {
            MagicCardVariant variant = App.DB.cardVariants.Where(x=>x.Id == variantId).FirstOrDefault();
            if (variant == null) return 0;
            return GetCardPrize(variant);
        }

        private static bool IsOutDated(string lastUpdate)
        {
            DateTime lastUp = DateTime.Parse(lastUpdate);
            if(lastUp<DateTime.Now.AddHours(-12)) return true;
            else return false;
        }

        private static float RetrievePrice(MagicCardVariant variant)
        {
            if (string.IsNullOrEmpty(variant.MultiverseId)) return -1;
            Thread.Sleep(150);
            float price = -1;
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                try
                {
                    json_data = w.DownloadString("https://api.scryfall.com/cards/multiverse/" + variant.MultiverseId);
                    var js = JsonConvert.DeserializeObject<Card>(json_data);
                    price = js.Prices.Eur.HasValue ? float.Parse(js.Prices.Eur.Value.ToString()) : -1;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Couldnt retrieve price : " + e.Message);
                }
            }
            if (price != -1)
            {
                var v = App.DB.Prices.Where(x => x.MultiverseId == variant.MultiverseId).FirstOrDefault();
                if (v != null) App.DB.Prices.Remove(v);
                App.DB.Prices.Add(
                    new Price() { 
                        MultiverseId = variant.MultiverseId, 
                        LastUpdate = DateTime.Now.ToString(), 
                        Value = price.ToString(),
                    }
                );
                App.DB.SafeSaveChanges();
            }
            return price;
        }

    }

}
