using MaGeek.Framework.Data;
using MaGeek.Framework.Extensions;
using MageekSdk.Collection.Entities;
using MageekSdk.MtgSqlive;
using MageekSdk.MtgSqlive.Entities;

namespace MageekSdk.Collection
{

    public static class CollectionSdk
    {

        public static bool IsInitialized = false;

        public static async Task<CollectionDbContext> GetContext()
        {
            if (!IsInitialized) await Initialize();
            return IsInitialized ? new CollectionDbContext(Config.Path_Db) : null;
        }

        public static async Task<bool> Initialize()
        {
            Console.WriteLine("MageekSdk : Initialize");
            try
            {
                if (IsInitialized)
                {
                    Console.WriteLine("MageekSdk : Initialize > Already initialized.");
                    return true;
                }
                Console.WriteLine("MageekSdk : Initialize > Initialize MtgSqliveSdk");
                bool needsUpdate = await MtgSqlive.MtgSqliveSdk.Initialize();
                if (needsUpdate) await UpdateData();
                IsInitialized = true;
                Console.WriteLine(IsInitialized ? "MageekSdk : Initialize > Success." : "MageekSdk : Initialize > Fail.");
                return IsInitialized;
            }
            catch (Exception e)
            {
                Console.WriteLine("MageekSdk : Initialize > error :" + e.Message);
                return false;
            }
        }

        #region Methods

        public static async Task UpdateData()
        {
            Console.WriteLine("MageekSdk : UpdateData");
            try
            {
                List<Task> tasks = new List<Task>
                {
                    Update_Archetypes(),
                    Update_Traductions()
                };
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine("MageekSdk : UpdateData > error : " + e.Message);
            }
        }

        private static async Task Update_Traductions()
        {
            Console.WriteLine("MageekSdk : Update_Traductions");
            try
            {
                List<CardTraduction> traductions = new();
                Console.WriteLine("MageekSdk : Update_Traductions > Parse...");
                using (MtgSqliveDbContext mtgSqliveContext = await MtgSqlive.MtgSqliveSdk.GetContext())
                {
                    foreach (CardForeignData traduction in mtgSqliveContext.cardForeignData)
                    {
                        traductions.Add(
                            new CardTraduction()
                            {
                                CardUuid = traduction.Uuid,
                                Language = traduction.Language,
                                Traduction = traduction.FaceName,
                                NormalizedTraduction = traduction.Language != "Korean" && traduction.Language != "Arabic"
                                    ? StringExtension.RemoveDiacritics(traduction.FaceName).Replace('-', ' ').ToLower()
                                    : traduction.FaceName
                            }
                        );
                    }
                }
                Console.WriteLine("MageekSdk : Update_Traductions > Save...");
                using (CollectionDbContext collectionDbContext = await GetContext())
                {
                    using var transaction = collectionDbContext.Database.BeginTransaction();
                    collectionDbContext.CardTraductions.RemoveRange(collectionDbContext.CardTraductions);
                    await collectionDbContext.SaveChangesAsync();
                    await collectionDbContext.CardTraductions.AddRangeAsync(traductions);
                    await collectionDbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                Console.WriteLine("MageekSdk : Update_Traductions > Done.");
            }
            catch (Exception e)
            {
                Console.WriteLine("MageekSdk : Update_Traductions > error : " + e.Message);
            }
        }

        private static async Task Update_Archetypes()
        {
            Console.WriteLine("MageekSdk : Update_Archetypes");
            try
            {
                List<ArchetypeCard> archetypes = new();
                Console.WriteLine("MageekSdk : Update_Archetypes > Parse...");
                using (MtgSqliveDbContext mtgSqliveContext = await MtgSqlive.MtgSqliveSdk.GetContext())
                {
                    foreach (Cards card in mtgSqliveContext.cards)
                    {
                        archetypes.Add(
                            new ArchetypeCard()
                            {
                                ArchetypeId = card.Name,
                                CardUuid = card.Uuid
                            }
                        );
                    }
                }
                Console.WriteLine("MageekSdk : Update_Archetypes > Save...");
                using (CollectionDbContext collectionDbContext = await GetContext())
                {
                    using var transaction = collectionDbContext.Database.BeginTransaction();
                    collectionDbContext.CardArchetypes.RemoveRange(collectionDbContext.CardArchetypes);
                    await collectionDbContext.SaveChangesAsync();
                    await collectionDbContext.CardArchetypes.AddRangeAsync(archetypes);
                    await collectionDbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                Console.WriteLine("MageekSdk : Update_Archetypes > Done.");
            }
            catch (Exception e)
            {
                Console.WriteLine("MageekSdk : Update_Archetypes > error : " + e.Message);
            }
        }

        #endregion


    }
}
