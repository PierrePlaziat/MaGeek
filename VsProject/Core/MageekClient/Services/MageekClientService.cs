using Grpc.Net.Client;
using MageekCore.Services;
using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.Mtg.Entities;
using MageekProtocol;
using PlaziatTools;
using MageekCore.Data.MtgFetched.Entities;

namespace MageekClient.Services
{

    public class MageekClientService : IMageekService
    {

        GrpcChannel channel;
        MageekProtocolService.MageekProtocolServiceClient mageekClient;
        private readonly ScryfallHelper scryfall = new ScryfallHelper();

        bool connected = false;
        private string token;
        const int timeout = 5;

        public MageekClientService()
        {
            scryfall.FetchSets().ConfigureAwait(false);
        }

        #region Connexion

        public async Task<MageekConnectReturn> Client_Connect(string user, string pass, string serverAddress)
        {
            if (connected) return MageekConnectReturn.Success;
            try
            {
                Logger.Log("Established channel...");
                try
                {
                    channel = await Client_Connect_Method1(serverAddress);
                    mageekClient = new(channel);
                }
                catch
                {
                    try
                    {
                        channel = await Client_Connect_Method2(serverAddress);
                        mageekClient = new(channel);
                    }
                    catch
                    {
                        return MageekConnectReturn.Failure;
                    }
                }
                Logger.Log("Handshake...");
                var call = await mageekClient.Users_HandshakeAsync(new Request_Empty());
                connected = true;
                return MageekConnectReturn.Success;
            }
            catch (Exception e)
            {
                Logger.Log(e, inner: true);
                return MageekConnectReturn.Failure;
            }
        }
        private async Task<GrpcChannel> Client_Connect_Method1(string serverAddress)
        {
            GrpcChannel channel = null;
            try
            {
                Logger.Log("Trying...");
                await Task.Run(() =>
                {
                    var handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    channel = GrpcChannel.ForAddress(
                        serverAddress,
                        new GrpcChannelOptions()
                        {
                            HttpHandler = handler,
                        }
                    );
                });
                Logger.Log("Success");
                return channel;
            }
            catch(Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }
        private async Task<GrpcChannel> Client_Connect_Method2(string serverAddress)
        {
            try
            {
                GrpcChannel channel = null;
                Logger.Log("Trying...");
                await Task.Run(() =>
                {
                    channel = GrpcChannel.ForAddress(
                        serverAddress,
                        new GrpcChannelOptions()
                        {
                            HttpClient = new HttpClient()
                            {
                                DefaultRequestVersion = new Version(2, 0)
                            }
                        }
                    );
                });
                Logger.Log("Success");
                return channel;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        public async Task<MageekConnectReturn> Client_Register(string user, string pass)
        {
            if (!connected) return MageekConnectReturn.Failure;
            try
            {
                Logger.Log("Registering...");
                var result = await mageekClient.Users_RegisterAsync(new Request_Identity()
                {
                    User = user,
                    Pass = pass,
                });
                if (string.IsNullOrEmpty(result.Token))
                {
                    return MageekConnectReturn.Failure;
                }
                //Logger.Log("token : " + result.Token);
                return MageekConnectReturn.Success;
            }
            catch (Exception e)
            {
                Logger.Log(e, inner: true);
                return MageekConnectReturn.Failure;
            }
        }

        public async Task<MageekConnectReturn> Client_Authentify(string user, string pass)
        {
            if (!connected) return MageekConnectReturn.Failure;
            try
            {
                Logger.Log("Identifying...");
                var result = await mageekClient.Users_IdentifyAsync(new Request_Identity()
                {
                    Pass = pass,
                    User = user,
                });
                if (string.IsNullOrEmpty(result.Token))
                {
                    return MageekConnectReturn.Failure;
                }
                //Logger.Log("token : " + result.Token);
                token = result.Token;
                return MageekConnectReturn.Success;
            }
            catch (Exception e)
            {
                Logger.Log(e, inner: true);
                return MageekConnectReturn.Failure;
            }
        }

        public async Task<MageekInitReturn> Server_Initialize()
        {
            return MageekInitReturn.NotImplementedForClient;
        }

        public async Task<MageekUpdateReturn> Server_Update()
        {
            return MageekUpdateReturn.NotImplementedForClient;
        }

        #endregion

        #region Implementation

        public async Task<List<SearchedCards>> Cards_Search(string cardName, string lang, int page, int pageSize, string? cardType = null, string? keyword = null, string? text = null, string? color = null, string? tag = null, bool onlyGot = false, bool colorisOr = false)
        {
            var reply = await mageekClient.Cards_SearchAsync(new Request_CardSearch()
            {
                CardName = cardName,
                Lang = lang,
                Page = page,
                PageSize = pageSize,
                CardType = cardType,
                Keyword = keyword,
                Text = text,
                Color = color,
                Tag = tag,
                OnlyGot = onlyGot,
                ColorisOr = colorisOr
            });
            List<SearchedCards> parsed = new();
            if (!reply.SearchedCardList.IsNull)
            {
                foreach (var item in reply.SearchedCardList.Items)
                {
                    parsed.Add(new SearchedCards(
                        item.CardUuid,
                        item.Translation,
                        item.Collected)
                    );
                }
            }
            return parsed;
        }

        public async Task<List<string>> Cards_UuidsForGivenCardName(string cardName)
        {
            var reply = await mageekClient.Cards_UuidsForGivenCardNameAsync(new Request_CardName()
            {
                CardName = cardName
            });
            List<string> parsed = new();

            if (!reply.CardUuidList.IsNull)
            {
                foreach (var item in reply.CardUuidList.Items)
                {
                    parsed.Add(item);
                }
            }
            return parsed;
        }

        public async Task<string> Cards_NameForGivenCardUuid(string cardUuid)
        {
            var reply = await mageekClient.Cards_NameForGivenCardUuidAsync(new Request_CardUuid()
            {
                CardUuid = cardUuid
            });
            string parsed = string.Empty;
            parsed = reply.CardName;
            return parsed;
        }

        public async Task<List<string>> Cards_UuidsForGivenCardUuid(string cardUuid)
        {
            var reply = await mageekClient.Cards_UuidsForGivenCardUuidAsync(new Request_CardUuid()
            {
                CardUuid = cardUuid
            });
            List<string> parsed = new();
            if (!reply.CardUuidList.IsNull)
                foreach (var item in reply.CardUuidList.Items)
            {
                parsed.Add(item);
            }
            return parsed;
        }

        public async Task<Cards> Cards_GetData(string cardUuid)
        {
            var reply = await mageekClient.Cards_GetDataAsync(new Request_CardUuid()
            {
                CardUuid = cardUuid
            });
            Cards parsed = MakeCard(reply);
            return parsed;
        }

        public async Task<CardForeignData> Cards_GetTranslation(string cardUuid, string lang)
        {
            var reply = await mageekClient.Cards_GetTranslationAsync(new Request_CardTranslation()
            {
                CardUuid = cardUuid,
                Lang = lang
            });
            CardForeignData parsed = new CardForeignData()
            {
                Uuid = reply.Uuid,
                Type = reply.Type,
                Text = reply.Text,
                FaceName = reply.FaceName,
                FlavorText = reply.FlavorText,
                Language = reply.Language,
                MultiverseId = reply.MultiverseId,
                Name = reply.Name
            };
            return parsed;
        }

        public async Task<CardLegalities> Cards_GetLegalities(string cardUuid)
        {
            var reply = await mageekClient.Cards_GetLegalitiesAsync(new Request_CardUuid()
            {
                CardUuid = cardUuid
            });
            CardLegalities parsed = new CardLegalities()
            {
                Uuid = reply.Uuid,
                Alchemy = reply.Alchemy,
                Brawl = reply.Brawl,
                Commander = reply.Commander,
                Duel = reply.Duel,
                Explorer = reply.Explorer,
                Future = reply.Future,
                Gladiator = reply.Gladiator,
                Historic = reply.Historic,
                Legacy = reply.Legacy,
                Modern = reply.Modern,
                Oathbreaker = reply.Oathbreaker,
                Oldschool = reply.Oldschool,
                Pauper = reply.Pauper,
                Paupercommander = reply.Paupercommander,
                Penny = reply.Penny,
                Pioneer = reply.Pioneer,
                Predh = reply.Predh,
                Premodern = reply.Premodern,
                Standard = reply.Standard,
                Standardbrawl = reply.Standardbrawl,
                Timeless = reply.Timeless,
                Vintage = reply.Vintage
            };
            return parsed;
        }

        public async Task<List<CardRulings>> Cards_GetRulings(string cardUuid)
        {
            var reply = await mageekClient.Cards_GetRulingsAsync(new Request_CardUuid()
            {
                CardUuid = cardUuid
            });
            List<CardRulings> parsed = new();
            if (!reply.Rulings.IsNull)
            {
                foreach (var item in reply.Rulings.Items)
                {
                    parsed.Add(new CardRulings()
                    {
                        Date = item.Date,
                        Text = item.Text,
                        Uuid = item.Uuid,
                    });
                }
            }
            return parsed;
        }

        public async Task<List<CardRelation>> Cards_GetRelations(string cardUuid)
        {
            var reply = await mageekClient.Cards_GetRelationsAsync(new Request_CardUuid()
            {
                CardUuid = cardUuid
            });
            List<CardRelation> parsed = new();
            if (!reply.Relations.IsNull)
                foreach (var item in reply.Relations.Items)
            {
                parsed.Add(new CardRelation()
                {
                    Role = (CardRelationRole)item.Role,
                    CardUuid = item.CardUuid,
                    TokenUuid = item.TokenUuid
                });
            }
            return parsed;
        }

        public async Task<Uri> Cards_GetIllustration(string cardUuid, CardImageFormat format, bool back = false)
        {
            try
            {
                string localFileName = Path.Combine(
                    MageekCore.Data.Paths.Folder_Illustrations,
                    string.Concat(cardUuid, "_", format.ToString())
                );
                if (!File.Exists(localFileName))
                {
                    string scryfallId = await Cards_GetScryfallId(cardUuid);
                    await scryfall.CacheIllustration(scryfallId, format, localFileName, back);
                }
                return new("file://" + Path.GetFullPath(localFileName), UriKind.Absolute);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        public async Task<string> Cards_GetScryfallId(string cardUuid)
        {
            var reply = await mageekClient.Cards_GetScryfallIdAsync(new Request_CardUuid()
            {
                CardUuid = cardUuid
            });
            return reply.CardName;
        }

        public async Task<PriceLine> Cards_GetPrice(string cardUuid)
        {
            var reply = await mageekClient.Cards_GetPriceAsync(new Request_CardUuid()
            {
                CardUuid = cardUuid
            });
            PriceLine parsed = new PriceLine()
            {
                CardUuid = reply.CardUuid,
                LastPriceEur = reply.LastPriceEur,
                LastPriceUsd = reply.LastPriceUsd,
                PriceEurAccrossTime = reply.PriceEurAccrossTime,
                PriceUsdAccrossTime = reply.PriceUsdAccrossTime
            };
            return parsed;
        }

        public async Task<List<Sets>> Sets_All()
        {
            Logger.Log("Calling...");
            var reply = await mageekClient.Sets_AllAsync(new Request_Empty());
            List<Sets> parsed = new();
            if (!reply.SetList.IsNull)
            {
                foreach (var item in reply.SetList.Items)
                {
                    parsed.Add(new Sets()
                    {
                        BaseSetSize = item.BaseSetSize,
                        Block = item.Block,
                        Code = item.Code,
                        IsFoilOnly = item.IsFoilOnly,
                        IsForeignOnly = item.IsForeignOnly,
                        IsNonFoilOnly = item.IsNonFoilOnly,
                        IsOnlineOnly = item.IsOnlineOnly,
                        IsPartialPreview = item.IsPartialPreview,
                        KeyruneCode = item.KeyruneCode,
                        Languages = item.Languages,
                        McmId = item.McmId,
                        McmIdExtras = item.McmIdExtras,
                        McmName = item.McmName,
                        MtgoCode = item.MtgoCode,
                        Name = item.Name,
                        ParentCode = item.ParentCode,
                        ReleaseDate = item.ReleaseDate,
                        TcgplayerGroupId = item.TcgplayerGroupId,
                        TokenSetCode = item.TokenSetCode,
                        TotalSetSize = item.TotalSetSize,
                        Type = item.Type
                    });
                }
            }
            Logger.Log("Done, found " + parsed.Count + " sets");
            return parsed;
        }

        public async Task<Sets> Sets_Get(string setCode)
        {
            var reply = await mageekClient.Sets_GetAsync(new Request_SetCode()
            {
                SetCode = setCode
            });
            Sets parsed = new Sets()
            {
                BaseSetSize = reply.BaseSetSize,
                Block = reply.Block,
                ReleaseDate = reply.ReleaseDate,
                TcgplayerGroupId = reply.TcgplayerGroupId,
                TokenSetCode = reply.TokenSetCode,
                TotalSetSize = reply.TotalSetSize,
                Type = reply.Type,
                ParentCode = reply.ParentCode,
                Name = reply.Name,
                Code = reply.Code,
                IsFoilOnly = reply.IsFoilOnly,
                IsForeignOnly = reply.IsForeignOnly,
                IsNonFoilOnly = reply.IsNonFoilOnly,
                IsOnlineOnly = reply.IsOnlineOnly,
                IsPartialPreview = reply.IsPartialPreview,
                KeyruneCode = reply.KeyruneCode,
                Languages = reply.Languages,
                McmId = reply.McmId,
                McmIdExtras = reply.McmIdExtras,
                McmName = reply.McmName,
                MtgoCode = reply.MtgoCode
            };
            return parsed;
        }

        public async Task<List<string>> Sets_Content(string setCode)
        {
            var reply = await mageekClient.Sets_ContentAsync(new Request_SetCode()
            {
                SetCode = setCode
            });
            List<string> parsed = new();
            if (!reply.CardUuidList.IsNull)
            {
                parsed = [.. reply.CardUuidList.Items];
            }
            return parsed;
        }

        public async Task<int> Sets_Completion(string user, string setCode, bool strict)
        {
            var reply = await mageekClient.Sets_CompletionAsync(new Request_SetCompletion()
            {
                User = user,
                Code = setCode,
                Strict = strict
            });
            int parsed = -1;
            parsed = reply.Percentage;
            return parsed;
        }

        public async Task Collec_SetFavCardVariant(string user, string cardName, string cardUuid)
        {
            var reply = await mageekClient.Collec_SetFavCardVariantAsync(new Request_Fav()
            {
                User = user,
                CardName = cardName,
                CardUuid = cardUuid
            });
        }

        public async Task Collec_Move(string user, string cardUuid, int quantity)
        {
            var reply = await mageekClient.Collec_MoveAsync(new Request_CollecMove()
            {
                User = user,
                CardUuid = cardUuid,
                Quantity = quantity
            });
        }

        public async Task<int> Collec_OwnedVariant(string user, string cardUuid)
        {
            var reply = await mageekClient.Collec_OwnedVariantAsync(new Request_CardUuidUser()
            {
                User = user,
                CardUuid = cardUuid
            });
            int parsed = -1;
            parsed = reply.Quantity;
            return parsed;
        }

        public async Task<int> Collec_OwnedCombined(string user, string cardName)
        {
            var reply = await mageekClient.Collec_OwnedCombinedAsync(new Request_CardNameUser()
            {
                User = user,
                CardName = cardName
            });
            int parsed = -1;
            parsed = reply.Quantity;
            return parsed;
        }

        public async Task<int> Collec_TotalOwned(string user)
        {
            var reply = await mageekClient.Collec_TotalOwnedAsync(new Request_User()
            {
                User = user,
            });
            int parsed = -1;
            parsed = reply.Quantity;
            return parsed;
        }

        public async Task<int> Collec_TotalDifferentOwned(string user, bool combined = true)
        {
            var reply = await mageekClient.Collec_TotalDifferentOwnedAsync(new Request_combinedVariant()
            {
                User = user,
                Combined = combined
            });
            int parsed = -1;
            parsed = reply.Quantity;
            return parsed;
        }

        public async Task<int> Collec_TotalDifferentExisting(bool combined = true)
        {
            var reply = await mageekClient.Collec_TotalDifferentExistingAsync(new Request_combinedVariant()
            {
                Combined = combined
            });
            int parsed = -1;
            parsed = reply.Quantity;
            return parsed;
        }

        public async Task<List<Deck>> Decks_All(string user)
        {
            var reply = await mageekClient.Decks_AllAsync(new Request_User()
            {
                User = user,
            });
            List<Deck> parsed = new();
            if (!reply.DeckList.IsNull)
            {
                foreach (var item in reply.DeckList.Items)
                {
                    parsed.Add(new Deck()
                    {
                        CardCount = item.CardCount,
                        DeckColors = item.DeckColors,
                        DeckId = item.DeckId,
                        Description = item.Description,
                        Title = item.Title,
                    });
                }
            }
            return parsed;
        }

        public async Task<Deck> Decks_Get(string user, string deckId)
        {
            var reply = await mageekClient.Decks_GetAsync(new Request_DeckId()
            {
                User = user,
                DeckId = deckId
            });
            Deck parsed = new Deck()
            {
                CardCount = reply.CardCount,
                DeckColors = reply.DeckColors,
                DeckId = reply.DeckId,
                Description = reply.Description,
                Title = reply.Title,
            };
            return parsed;
        }

        public async Task<List<DeckCard>> Decks_Content(string user, string deckId)
        {
            var reply = await mageekClient.Decks_ContentAsync(new Request_DeckId()
            {
                User = user,
                DeckId = deckId
            });
            List<DeckCard> parsed = new();
            if (!reply.DeckContent.IsNull)
            {
                foreach (var item in reply.DeckContent.Items)
                {
                    parsed.Add(new DeckCard()
                    {
                        DeckId = item.DeckId,
                        CardUuid = item.CardUuid,
                        Quantity = item.Quantity,
                        RelationType = item.RelationType
                    });
                }
            }
            return parsed;
        }

        public async Task Decks_Create(string user, string title, string description, IEnumerable<DeckCard> deckLines)
        {
            var req = new Request_CreateDeck()
            {
                User = user,
                Description = description,
                Title = title,
                Cards = new Wrapper_Reply_DeckCardList()
            };
            foreach (var item in deckLines)
            {
                    req.Cards.Items.Add(new Reply_DeckCard()
                    {
                        CardUuid = item.CardUuid,
                        //DeckId = header.DeckId,
                        Quantity = item.Quantity,
                        RelationType = item.RelationType,
                    });
            }
            if (!req.Cards.Items.Any())
            {
                req.Cards.IsNull = true;
            }
            var reply = await mageekClient.Decks_CreateAsync(req);
        }

        public async Task Decks_Rename(string user, string deckId, string title)
        {
            var reply = await mageekClient.Decks_RenameAsync(new Request_RenameDeck()
            {
                User = user,
                DeckId = deckId,
                Title = title
            });
        }

        public async Task Decks_Duplicate(string user, string deckId)
        {
            var reply = await mageekClient.Decks_DuplicateAsync(new Request_DeckId()
            {
                User = user,
                DeckId = deckId
            });
        }

        public async Task Decks_Save(string user, Deck header, List<DeckCard> lines)
        {
            var req = new Request_SaveDeck()
            {
                User = user,
                DeckId = header.DeckId,
                Description = header.Description,
                Title = header.Title,
            };
            foreach (var item in lines)
            {
                if (!req.Lines.IsNull)
                {
                    req.Lines.Items.Add(new Reply_DeckCard()
                    {
                        CardUuid = item.CardUuid,
                        DeckId = header.DeckId,
                        Quantity = item.Quantity,
                        RelationType = item.RelationType,
                    });
                }
            }
            var reply = await mageekClient.Decks_SaveAsync(req);
        }

        public async Task Decks_Delete(string user, string deckId)
        {
            var reply = await mageekClient.Decks_DeleteAsync(new Request_DeckId()
            {
                User = user,
                DeckId = deckId
            });
        }

        public async Task<List<Preco>> Decks_Precos()
        {
            var reply = await mageekClient.Decks_PrecosAsync(new Request_Empty());
            List<Preco> parsed = new();
            if (!reply.PrecoList.IsNull)
            {
                foreach (var item in reply.PrecoList.Items)
                {
                    var preco = new Preco()
                    {
                        Code = item.Code,
                        Kind = item.Kind,
                        ReleaseDate = item.ReleaseDate,
                        Title = item.Title,
                        Cards = new()
                    };
                    if (!item.Cards.IsNull)
                    {
                        foreach (var v in item.Cards.Items)
                        {
                            preco.Cards.Add(new DeckCard()
                            {
                                CardUuid = v.CardUuid,
                                DeckId = v.DeckId,
                                Quantity = v.Quantity,
                                RelationType = v.RelationType,
                            });
                        }
                    }
                    parsed.Add(preco);
                }
            }
            return parsed;
        }

        public async Task<List<string>> Tags_All(string user)
        {
            var reply = await mageekClient.Tags_AllAsync(new Request_User()
            {
                User = user,
            });
            List<string> parsed = new();
            if (!reply.TagList.IsNull)
                foreach (var item in reply.TagList.Items)
            {
                parsed.Add(item.TagContent);
            }
            return parsed;
        }

        public async Task<bool> Tags_CardHasTag(string user, string cardName, string tag)
        {
            var reply = await mageekClient.Tags_CardHasTagAsync(new Request_CardTag()
            {
                User = user,
                CardName = cardName,
                Tag = tag
            });
            bool parsed = false;
            parsed = reply.HasTag;
            return parsed;
        }

        public async Task Tags_TagCard(string user, string cardName, string tag)
        {
            var reply = await mageekClient.Tags_UntagCardAsync(new Request_CardTag()
            {
                User = user,
                CardName = cardName,
                Tag = tag
            });
        }

        public async Task Tags_UntagCard(string user, string cardName, string tag)
        {
            var reply = await mageekClient.Tags_UntagCardAsync(new Request_CardTag()
            {
                User = user,
                CardName = cardName,
                Tag = tag
            });
        }

        public async Task<List<Tag>> Tags_GetCardTags(string user, string cardName)
        {
            var reply = await mageekClient.Tags_GetCardTagsAsync(new Request_CardNameUser()
            {
                User = user,
                CardName = cardName
            });
            List<Tag> parsed = new();
            if(!reply.TagList.IsNull)
            {
                foreach (var item in reply.TagList.Items)
                {
                    parsed.Add(new Tag()
                    {
                        ArchetypeId = item.ArchetypeId,
                        TagContent = item.TagContent,
                        TagId = item.TagId,
                    });
                }
            }
            return parsed;
        }

        public async Task<CardList> CardLists_Parse(string input)
        {
            var reply = await mageekClient.CardLists_ParseAsync(new Request_Txt()
            {
                Input = input
            });
            CardList parsed = new CardList()
            {
                Detail = reply.Detail,
                Status = reply.Status,
                Cards = new(),
            };
            if (!reply.Cards.IsNull)
            {
                foreach (var item in reply.Cards.Items)
                {
                    parsed.Cards.Add(new DeckCard()
                    {
                        CardUuid = item.CardUuid,
                        DeckId = item.DeckId,
                        Quantity = item.Quantity,
                        RelationType = item.RelationType,
                    });
                }
            }
            return parsed;
        }

        public async Task<string> CardLists_FromDeck(string user, string deckId, bool withSetCode = false)
        {
            var reply = await mageekClient.CardLists_FromDeckAsync(new Request_DeckToTxt()
            {
                User = user,
                DeckId = deckId,
                WithSetCode = withSetCode
            });
            string parsed = string.Empty;
            parsed = reply.Txt;
            return parsed;
        }

        #endregion

        #region Methods

        private Cards? MakeCard(Reply_CardData reply)
        {
            return new Cards()
            {
                Artist = reply.Artist,
                AsciiName = reply.AsciiName,
                AttractionLights = reply.AttractionLights,
                Availability = reply.Availability,
                BoosterTypes = reply.BoosterTypes,
                BorderColor = reply.BorderColor,
                CardParts = reply.CardParts,
                ColorIdentity = reply.ColorIdentity,
                ColorIndicator = reply.ColorIndicator,
                Colors = reply.Colors,
                Defense = reply.Defense,
                DuelDeck = reply.DuelDeck,
                EdhrecRank = reply.EdhrecRank,
                EdhrecSaltiness = reply.EdhrecSaltiness,
                FaceConvertedManaCost = reply.FaceConvertedManaCost,
                FaceFlavorName = reply.FaceFlavorName,
                FaceManaValue = reply.FaceManaValue,
                FaceName = reply.FaceName,
                Finishes = reply.Finishes,
                FlavorName = reply.FlavorName,
                FlavorText = reply.FlavorText,
                FrameEffects = reply.FrameEffects,
                FrameVersion = reply.FrameVersion,
                Hand = reply.Hand,
                HasAlternativeDeckLimit = reply.HasAlternativeDeckLimit,
                HasContentWarning = reply.HasContentWarning,
                HasFoil = reply.HasFoil,
                HasNonFoil = reply.HasNonFoil,
                IsAlternative = reply.IsAlternative,
                IsFullArt = reply.IsFullArt,
                IsFunny = reply.IsFunny,
                IsOnlineOnly = reply.IsOnlineOnly,
                IsOversized = reply.IsOversized,
                IsPromo = reply.IsPromo,
                IsRebalanced = reply.IsRebalanced,
                IsReprint = reply.IsReprint,
                IsReserved = reply.IsReserved,
                IsStarter = reply.IsStarter,
                IsStorySpotlight = reply.IsStorySpotlight,
                IsTextless = reply.IsTextless,
                IsTimeshifted = reply.IsTimeshifted,
                Keywords = reply.Keywords,
                Language = reply.Language,
                Layout = reply.Layout,
                LeadershipSkills = reply.LeadershipSkills,
                Life = reply.Life,
                Loyalty = reply.Loyalty,
                ManaCost = reply.ManaCost,
                ManaValue = reply.ManaValue,
                Name = reply.Name,
                Number = reply.Number,
                OriginalPrintings = reply.OriginalPrintings,
                OriginalReleaseDate = reply.OriginalReleaseDate,
                OriginalText = reply.OriginalText,
                OriginalType = reply.OriginalType,
                OtherFaceIds = reply.OtherFaceIds,
                Power = reply.Power,
                Printings = reply.Printings,
                PromoTypes = reply.PromoTypes,
                Rarity = reply.Rarity,
                RebalancedPrintings = reply.RebalancedPrintings,
                RelatedCards = reply.RelatedCards,
                SecurityStamp = reply.SecurityStamp,
                SetCode = reply.SetCode,
                Side = reply.Side,
                Signature = reply.Signature,
                Subsets = reply.Subsets,
                Subtypes = reply.Subtypes,
                Supertypes = reply.Supertypes,
                Text = reply.Text,
                Toughness = reply.Toughness,
                Type = reply.Type,
                Types = reply.Types_,
                Uuid = reply.Uuid,
                Variations = reply.Variations,
                Watermark = reply.Watermark
            };
        }

        #endregion

    }

}
