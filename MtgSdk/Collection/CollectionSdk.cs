#pragma warning disable CS8600 // Conversion de littéral ayant une valeur null ou d'une éventuelle valeur null en type non-nullable.
#pragma warning disable CS8602 // Déréférencement d'une éventuelle référence null.

using MaGeek.Framework.Extensions;
using MageekSdk.Collection.Entities;
using MageekSdk.MtgSqlive;
using MageekSdk.MtgSqlive.Entities;
using MageekSdk.Tools;

namespace MageekSdk.Collection
{

    /// <summary>
    /// Mageek database
    /// Call Initialize() first, then you can use GetContext()
    /// to access data through entity framework.
    /// </summary>
    public static class CollectionSdk
    {

        public static bool IsInitialized { get; private set; } = false;

        /// <summary>
        /// Handles Mageek database setup
        /// </summary>
        /// <returns>True if correclty initialized</returns>
        public static async Task<bool> Initialize()
        {
            Logger.Log("Start");
            try
            {
                if (IsInitialized)
                {
                    Logger.Log("Already called");
                    return true;
                }
                bool needsUpdate = await MtgSqlive.MtgSqliveSdk.Initialize();
                if (needsUpdate) await Update();
                IsInitialized = true;
                Logger.Log(IsInitialized ? "Success!" : "Fail.");
                return IsInitialized;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return false;
            }
        }

        /// <summary>
        /// Call this when you need to access data
        /// </summary>
        /// <returns>An entity framework context representing Mageek database</returns>
        public static async Task<CollectionDbContext?> GetContext()
        {
            await Task.Delay(0);
            return new CollectionDbContext(Config.Path_Db);
        }

        #region Method s

        private static async Task Update()
        {
            Logger.Log("Fetching MtgSqlite...");
            try
            {
                List<Task> tasks = new() 
                {
                    Update_Archetypes(),
                    Update_Traductions()
                };
                await Task.WhenAll(tasks);
                Logger.Log("Done");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        private static async Task Update_Traductions()
        {
            try
            {
                List<CardTraduction> traductions = new();
                Logger.Log("Parsing...");
                using (MtgSqliveDbContext mtgSqliveContext = await MtgSqlive.MtgSqliveSdk.GetContext())
                {
                    foreach (CardForeignData traduction in mtgSqliveContext.cardForeignData)
                     {
                        if (traduction.FaceName == null)
                        {
                            traductions.Add(
                                new CardTraduction()
                                {
                                    CardUuid = traduction.Uuid,
                                    Language = traduction.Language,
                                    Traduction = traduction.Name,
                                    NormalizedTraduction = traduction.Language != "Korean" && traduction.Language != "Arabic"
                                        ? StringExtension.RemoveDiacritics(traduction.Name).Replace('-', ' ').ToLower()
                                        : traduction.Name
                                }
                            );
                        }
                        else
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
                }
                Logger.Log("Saving...");
                using (CollectionDbContext collectionDbContext = await GetContext())
                {
                    using var transaction = collectionDbContext.Database.BeginTransaction();
                    collectionDbContext.CardTraductions.RemoveRange(collectionDbContext.CardTraductions);
                    await collectionDbContext.SaveChangesAsync();
                    await collectionDbContext.CardTraductions.AddRangeAsync(traductions);
                    await collectionDbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                Logger.Log("Done");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        private static async Task Update_Archetypes()
        {
            try
            {
                List<ArchetypeCard> archetypes = new();
                Logger.Log("Parsing...");
                using (MtgSqliveDbContext mtgSqliveContext = await MtgSqlive.MtgSqliveSdk.GetContext())
                {
                    foreach (Cards card in mtgSqliveContext.cards)
                    {
                        if (card.Name == null)
                        {

                        }
                        if (card.Uuid == null)
                        {

                        }
                        archetypes.Add(
                            new ArchetypeCard()
                            {
                                ArchetypeId = card.Name != null ? card.Name : string.Empty,
                                CardUuid = card.Uuid
                            }
                        );
                    }
                }
                Logger.Log("Saving...");
                using (CollectionDbContext collectionDbContext = await GetContext())
                {
                    using var transaction = collectionDbContext.Database.BeginTransaction();
                    collectionDbContext.CardArchetypes.RemoveRange(collectionDbContext.CardArchetypes);
                    await collectionDbContext.SaveChangesAsync();
                    await collectionDbContext.CardArchetypes.AddRangeAsync(archetypes);
                    await collectionDbContext.SaveChangesAsync();
                    transaction.Commit();
                }
                Logger.Log("Done");
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        #endregion


    }
}
