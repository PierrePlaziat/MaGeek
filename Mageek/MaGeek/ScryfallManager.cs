using System;
using Newtonsoft.Json;
using System.Net;
using ScryfallApi.Client;
using MaGeek.Data.Entities;
using ScryfallApi.Client.Models;
using System.Windows;
using System.Linq;
using System.Threading;
using System.Windows.Documents;
using MaGeek.Entities;
using System.Collections.Generic;

namespace MaGeek
{

    public class ScryfallManager
    {

        static CardDatabase DB;

        public ScryfallManager(CardDatabase db)
        {
            DB = db;
        }

        // COMMON

        private bool IsOutDated(string lastUpdate, int dayLimit)
        {
            DateTime lastUp = DateTime.Parse(lastUpdate);
            if (lastUp < DateTime.Now.AddDays(-dayLimit)) return true;
            else return false;
        }

        // PRIZE

        public float GetCardPrize(string variantId)
        {
            MagicCardVariant variant = DB.cardVariants.Where(x=>x.Id == variantId).FirstOrDefault();
            if (variant == null) return 0;
            return GetCardPrize(variant);
        }
        public float GetCardPrize(MagicCardVariant variant)
        {
            if (variant == null) return -1;
            float price = 0;
            var v = DB.Prices.Where(x => x.MultiverseId == variant.MultiverseId).FirstOrDefault();

            // NO DATA
            if (v == null)
            {
                price = RetrieveCardPrize(variant);
            }
            // OUTDATED
            else if (IsOutDated(v.LastUpdate, 1))
            {
                DB.Prices.Remove(v);
                price = RetrieveCardPrize(variant);
            }
            // DATA OK 
            else price = float.Parse(v.Value);

            return price;
        }
        private float RetrieveCardPrize(MagicCardVariant variant)
        {
            if (string.IsNullOrEmpty(variant.MultiverseId)) return -1;
            float price = -1;

            // ASK API
            Thread.Sleep(150);
            var json_data = string.Empty;
            using (var w = new WebClient())
            {
                try { json_data = w.DownloadString("https://api.scryfall.com/cards/multiverse/" + variant.MultiverseId); }
                catch (Exception e) { MessageBox.Show("Couldnt retrieve price : " + e.Message); }
            }
            try
            {
                var scryCard = JsonConvert.DeserializeObject<Card>(json_data);
                price = scryCard.Prices.Eur.HasValue ? float.Parse(scryCard.Prices.Eur.Value.ToString()) : -1;
            }
            catch (Exception e) { MessageBox.Show("Couldnt parse price : " + e.Message); }

            // SAVE
            if (price != -1)
            {
                DB.Prices.Add(
                    new Entities.Price() { 
                        MultiverseId = variant.MultiverseId, 
                        LastUpdate = DateTime.Now.ToString(), 
                        Value = price.ToString(),
                    }
                );
                DB.SafeSaveChanges();
            }
            return price;
        }

        // LEGAL

        public List<Legality> GetCardLegal(string variantId)
        {
            MagicCardVariant variant = DB.cardVariants.Where(x=>x.Id == variantId).FirstOrDefault();
            if (variant == null) return new List<Legality>();
            return GetCardLegal(variant);
        }
        public List<Legality> GetCardLegal(MagicCardVariant variant)
        {
            if (variant == null) return new List<Legality>();
            List<Legality> legal = new List<Legality>();
            List<Legality> previous = DB.Legalities.Where(x => x.MultiverseId == variant.MultiverseId).ToList();

            // NO DATA
            if (previous == null || previous.FirstOrDefault()==null)
            {
                legal = RetrieveCardLegal(variant);
            }
            // OUTDATED
            else if (IsOutDated(previous.FirstOrDefault().LastUpdate, 30))
            {
                DB.Legalities.RemoveRange(previous);
                legal = RetrieveCardLegal(variant);
            }
            // DATA OK 
            else legal = previous;
            return legal;
        }
        private List<Legality> RetrieveCardLegal(MagicCardVariant variant)
        {
            if (string.IsNullOrEmpty(variant.MultiverseId)) return new List<Legality>();
            List<Legality> legal = new List<Legality>();

            // ASK API
            Thread.Sleep(150);
            var json_data = string.Empty;
            using (var w = new WebClient())
            {
                try { json_data = w.DownloadString("https://api.scryfall.com/cards/multiverse/" + variant.MultiverseId); }
                catch (Exception e) { MessageBox.Show("Couldnt retrieve legalities : " + e.Message); }
            }
            try
            {
                var scryCard = JsonConvert.DeserializeObject<Card>(json_data);
                foreach (var l in scryCard.Legalities) 
                {
                    legal.Add(new Legality()
                    {
                        Format = l.Key,
                        IsLegal = l.Value,
                        LastUpdate = DateTime.Now.ToString(),
                        MultiverseId = variant.MultiverseId,
                    });
                }
            }
            catch (Exception e) { MessageBox.Show("Couldnt parse legalities : " + e.Message); }

            // SAVE
            
            DB.Legalities.AddRange(legal);
            DB.SafeSaveChanges();
            return legal;
        }

    }

}
