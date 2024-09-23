using Grpc.Core;
using MageekCore.Data;
using MageekCore.Data.Collection.Entities;
using MageekCore.Services;
using MageekProtocol;
using PlaziatTools;
using PlaziatIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace MageekServer.Services
{

    public class MageekGrpcService : MageekProtocolService.MageekProtocolServiceBase
    {

        private readonly IMageekService mageek;
        private readonly IUserService userService;

        public MageekGrpcService(IMageekService mageek, IUserService userService)
        {
            this.mageek = mageek;
            this.userService = userService;
        }

        public override async Task<Reply_Empty> Users_Handshake(Request_Empty request, ServerCallContext context)
        {
            Logger.Log(context.Peer);
            return new Reply_Empty();
        }

        public override async Task<Reply_Token> Users_Register(Request_Identity request, ServerCallContext context)
        {
            Logger.Log($"Registering user: {request.User}");
            var result = await userService.RegisterUser(request.User, request.Pass);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Logger.Log($"User registration failed: {errors}");
                return new Reply_Token { Token = "false" };
            }
            Logger.Log($"User registered: {request.User}");
            return new Reply_Token { Token = "true" };
        }
        
        public override async Task<Reply_Token> Users_Identify(Request_Identity request, ServerCallContext context)
        {
            Logger.Log(context.Peer + " - " + request.User + " - " + request.Pass);
            string token = await userService.AuthenticateUser(request.User,request.Pass);
            if (token == null) token = string.Empty;
            return new Reply_Token()
            {
                Token = token,
            };
        }

        [Authorize]
        public override async Task<Reply_Txt> CardLists_FromDeck(Request_DeckToTxt request, ServerCallContext context)
        {
            var item = await mageek.CardLists_FromDeck(request.User, request.DeckId, request.WithSetCode);
            return new Reply_Txt()
            {
                Txt = item
            };
        }

        [Authorize]
        public override async Task<Reply_TxtImportResult> CardLists_Parse(Request_Txt request, ServerCallContext context)
        {
            var data = await mageek.CardLists_Parse(request.Input);
            var reply = new Reply_TxtImportResult()
            {
                Detail = data.Detail,
                Status = data.Status,
            };
            if (data.Cards.IsNullOrEmpty()) reply.Cards.IsNull = true;
            else
            {
                foreach (var item in data.Cards)
                reply.Cards.Items.Add(new Reply_DeckCard()
                {
                    CardUuid = item.CardUuid,
                    DeckId = item.DeckId,
                    Quantity = item.Quantity,
                    RelationType = item.RelationType
                });
                reply.Cards.IsNull = false;
            }
            return reply;
        }

        [Authorize]
        public override async Task<Reply_CardData> Cards_GetData(Request_CardUuid request, ServerCallContext context)
        {
            var item = await mageek.Cards_GetData(request.CardUuid);
            return new Reply_CardData()
            {
                Uuid = item.Uuid,
                Artist = item.Artist,
                AsciiName = item.AsciiName,
                AttractionLights = item.AttractionLights,
                Availability = item.Availability,
                BoosterTypes = item.BoosterTypes,
                BorderColor = item.BorderColor,
                CardParts = item.CardParts,
                ColorIdentity = item.ColorIdentity,
                ColorIndicator = item.ColorIndicator,
                Colors = item.Colors,
                Defense = item.Defense,
                DuelDeck = item.DuelDeck,
                ManaValue = item.ManaValue,
                FaceName = item.FaceName,
                Finishes = item.Finishes,
                FlavorName = item.FlavorName,
                FlavorText = item.FlavorText,
                FrameEffects = item.FrameEffects,
                FrameVersion = item.FrameVersion,
                Hand = item.Hand,
                HasFoil = item.HasFoil,
                HasNonFoil = item.HasNonFoil,
                Keywords = item.Keywords,
                Language = item.Language,
                Layout = item.Layout,
                LeadershipSkills = item.LeadershipSkills,
                Life = item.Life,
                Loyalty = item.Loyalty,
                ManaCost = item.ManaCost,
                Name = item.Name,
                Number = item.Number,
                OriginalPrintings = item.OriginalPrintings,
                OriginalReleaseDate = item.OriginalReleaseDate,
                OriginalText = item.OriginalText,
                OriginalType = item.OriginalType,
                OtherFaceIds = item.OtherFaceIds,
                Power = item.Power,
                Printings = item.Printings,
                PromoTypes = item.PromoTypes,
                Rarity = item.Rarity,
                RebalancedPrintings = item.RebalancedPrintings,
                RelatedCards = item.RelatedCards,
                SecurityStamp = item.SecurityStamp,
                SetCode = item.SetCode,
                Side = item.Side,
                Signature = item.Signature,
                Subsets = item.Subsets,
                Subtypes = item.Subtypes,
                Supertypes = item.Supertypes,
                Text = item.Text,
                Toughness = item.Toughness,
                Type = item.Type,
                Types_ = item.Types,
                Variations = item.Variations,
                Watermark = item.Watermark,
                EdhrecRank = item.EdhrecRank,
                EdhrecSaltiness = item.EdhrecSaltiness,
                FaceConvertedManaCost = item.FaceConvertedManaCost,
                FaceFlavorName = item.FaceFlavorName,
                FaceManaValue = item.FaceManaValue,
                HasAlternativeDeckLimit = item.HasAlternativeDeckLimit,
                HasContentWarning = item.HasContentWarning,
                IsAlternative = item.IsAlternative,
                IsFullArt = item.IsFullArt,
                IsFunny = item.IsFunny,
                IsOnlineOnly = item.IsOnlineOnly,
                IsOversized = item.IsOversized,
                IsPromo = item.IsPromo,
                IsRebalanced = item.IsRebalanced,
                IsReprint = item.IsReprint,
                IsReserved = item.IsReserved,
                IsStarter = item.IsStarter,
                IsStorySpotlight = item.IsStorySpotlight,
                IsTextless = item.IsTextless,
                IsTimeshifted = item.IsTimeshifted,
            };
        }

        [Authorize]
        public override async Task<Reply_Uri> Cards_GetIllustration(Request_CardIllu request, ServerCallContext context)
        {
            var item = await mageek.Cards_GetIllustration(request.CardUuid, (CardImageFormat)request.Format);
            return new Reply_Uri()
            {
                Uri = item.ToString(),
            };
        }

        [Authorize]
        public override async Task<Reply_CardName> Cards_GetScryfallId(Request_CardUuid request, ServerCallContext context)
        {
            var item = await mageek.Cards_GetScryfallId(request.CardUuid);
            return new Reply_CardName()
            {
                CardName = item,
            };
        }

        [Authorize]
        public override async Task<Reply_CardLegalities> Cards_GetLegalities(Request_CardUuid request, ServerCallContext context)
        {
            var item = await mageek.Cards_GetLegalities(request.CardUuid);
            return new Reply_CardLegalities()
            {
                Alchemy = item.Alchemy,
                Brawl = item.Brawl,
                Commander = item.Commander,
                Duel = item.Duel,
                Explorer = item.Explorer,
                Future = item.Future,
                Gladiator = item.Gladiator,
                Historic = item.Historic,
                Legacy = item.Legacy,
                Modern = item.Modern,
                Oathbreaker = item.Oathbreaker,
                Oldschool = item.Oldschool,
                Pauper = item.Pauper,
                Paupercommander = item.Paupercommander,
                Penny = item.Penny,
                Pioneer = item.Pioneer,
                Predh = item.Predh,
                Premodern = item.Premodern,
                Standard = item.Standard,
                Standardbrawl = item.Standardbrawl,
                Timeless = item.Timeless,
                Uuid = item.Uuid,
                Vintage = item.Vintage,
            };
        }

        [Authorize]
        public override async Task<Reply_PriceData> Cards_GetPrice(Request_CardUuid request, ServerCallContext context)
        {
            var item = await mageek.Cards_GetPrice(request.CardUuid);
            return new Reply_PriceData()
            {
                CardUuid = item.CardUuid,
                LastPriceEur = item.LastPriceEur,
                LastPriceUsd = item.LastPriceUsd,
                PriceEurAccrossTime = item.PriceEurAccrossTime,
                PriceUsdAccrossTime = item.PriceUsdAccrossTime
            };
        }

        [Authorize]
        public override async Task<Reply_CardRelations> Cards_GetRelations(Request_CardUuid request, ServerCallContext context)
        {
            var data = await mageek.Cards_GetRelations(request.CardUuid);
            var reply = new Reply_CardRelations();
            reply.Relations = new();
            if (data.IsNullOrEmpty()) reply.Relations.IsNull = true;
            else
            {
                foreach (var item in data)
                    reply.Relations.Items.Add(new Reply_CardRelation()
                    {
                        CardUuid = item.CardUuid,
                        Role = (int)item.Role,
                        TokenUuid = item.TokenUuid,
                    });
            }
            return reply;
        }

        [Authorize]
        public override async Task<Reply_CardRulings> Cards_GetRulings(Request_CardUuid request, ServerCallContext context)
        {
            var data = await mageek.Cards_GetRulings(request.CardUuid);
            var reply = new Reply_CardRulings();
            reply.Rulings = new();
            if (data.IsNullOrEmpty()) reply.Rulings.IsNull = true;
            else
            {
                foreach (var item in data)
                reply.Rulings.Items.Add(new Reply_CardRuling()
                {
                    Date = item.Date,
                    Text = item.Text,
                    Uuid = item.Uuid,
                });
            }
            return reply;
        }

        [Authorize]
        public override async Task<Reply_CardTranslation> Cards_GetTranslation(Request_CardTranslation request, ServerCallContext context)
        {
            var item = await mageek.Cards_GetTranslation(request.CardUuid, request.Lang);
            return new Reply_CardTranslation()
            {
                FaceName = item.FaceName,
                Uuid = item.Uuid,
                FlavorText = item.FlavorText,
                Language = item.Language,
                MultiverseId = item.MultiverseId,
                Name = item.Name,
                Text = item.Text,
                Type = item.Type
            };
        }

        [Authorize]
        public override async Task<Reply_CardName> Cards_NameForGivenCardUuid(Request_CardUuid request, ServerCallContext context)
        {
            var item = await mageek.Cards_NameForGivenCardUuid(request.CardUuid);
            return new Reply_CardName()
            {
                CardName = item
            };
        }

        [Authorize]
        public override async Task<Reply_SearchedCardList> Cards_Search(Request_CardSearch request, ServerCallContext context)
        {
            var data = await mageek.Cards_Search(request.CardName, request.Lang, request.Page, request.PageSize, request.CardType, request.Keyword, request.Text, request.Color, request.Tag, request.OnlyGot, request.ColorisOr);
            var reply = new Reply_SearchedCardList();
            reply.SearchedCardList = new Wrapper_Reply_SearchedCardList();
            if (data.IsNullOrEmpty()) reply.SearchedCardList.IsNull = true;
            else
            {
                foreach (var item in data)
                    reply.SearchedCardList.Items.Add(new Reply_SearchedCard()
                    {
                        CardUuid = item.CardUuid,
                        Collected = item.Collected,
                        Translation = item.Translation,
                    });
            }
            return reply;
        }

        [Authorize]
        public override async Task<Reply_ListCardUuid> Cards_UuidsForGivenCardName(Request_CardName request, ServerCallContext context)
        {
            var item = await mageek.Cards_UuidsForGivenCardName(request.CardName);
            var reply = new Reply_ListCardUuid();
            reply.CardUuidList = new();
            if (item.IsNullOrEmpty()) reply.CardUuidList.IsNull = true;
            else
            {
                foreach (var v in item) reply.CardUuidList.Items.Add(v);
            }
            return reply;
        }

        [Authorize]
        public override async Task<Reply_ListCardUuid> Cards_UuidsForGivenCardUuid(Request_CardUuid request, ServerCallContext context)
        {
            var item = await mageek.Cards_UuidsForGivenCardUuid(request.CardUuid);
            var reply = new Reply_ListCardUuid();
            reply.CardUuidList = new();
            if (item.IsNullOrEmpty()) reply.CardUuidList.IsNull = true;
            {
                foreach (var v in item) reply.CardUuidList.Items.Add(v);
            }
            return reply;
        }

        [Authorize]
        public override async Task<Reply_Empty> Collec_Move(Request_CollecMove request, ServerCallContext context)
        {
            await mageek.Collec_Move(request.User, request.CardUuid, request.Quantity);
            return new Reply_Empty();
        }

        [Authorize]
        public override async Task<Reply_Quantity> Collec_OwnedCombined(Request_CardNameUser request, ServerCallContext context)
        {
            var item = await mageek.Collec_OwnedCombined(request.User, request.CardName);
            return new Reply_Quantity()
            {
                Quantity = item,
            };
        }

        [Authorize]
        public override async Task<Reply_Quantity> Collec_OwnedVariant(Request_CardUuidUser request, ServerCallContext context)
        {
            var item = await mageek.Collec_OwnedVariant(request.User, request.CardUuid);
            return new Reply_Quantity()
            {
                Quantity = item,
            };
        }

        [Authorize]
        public override async Task<Reply_Empty> Collec_SetFavCardVariant(Request_Fav request, ServerCallContext context)
        {
            await mageek.Collec_SetFavCardVariant(request.User, request.CardName, request.CardUuid);
            return new Reply_Empty();
        }

        [Authorize]
        public override async Task<Reply_Quantity> Collec_TotalDifferentExisting(Request_combinedVariant request, ServerCallContext context)
        {
            var item = await mageek.Collec_TotalDifferentExisting(request.Combined);
            return new Reply_Quantity()
            {
                Quantity = item
            };
        }

        [Authorize]
        public override async Task<Reply_Quantity> Collec_TotalDifferentOwned(Request_combinedVariant request, ServerCallContext context)
        {
            var item = await mageek.Collec_TotalDifferentOwned(request.User, request.Combined);
            return new Reply_Quantity()
            {
                Quantity = item,
            };
        }

        [Authorize]
        public override async Task<Reply_Quantity> Collec_TotalOwned(Request_User request, ServerCallContext context)
        {
            var item = await mageek.Collec_TotalOwned(request.User);
            return new Reply_Quantity()
            {
                Quantity = item,
            };
        }

        [Authorize]
        public override async Task<Reply_DeckList> Decks_All(Request_User request, ServerCallContext context)
        {
            var data = await mageek.Decks_All(request.User);
            var reply = new Reply_DeckList();
            reply.DeckList = new Wrapper_Reply_DeckList();
            if (data.IsNullOrEmpty()) reply.DeckList.IsNull = true;
            else
            {
                reply.DeckList.IsNull = false;
                foreach (var item in data)
                    reply.DeckList.Items.Add(new Reply_Deck()
                    {
                        CardCount = item.CardCount,
                        DeckColors = item.DeckColors,
                        DeckId = item.DeckId,
                        Description = item.Description,
                        Title = item.Title
                    });
            }
            return reply;
        }

        [Authorize]
        public override async Task<Reply_DeckContent> Decks_Content(Request_DeckId request, ServerCallContext context)
        {
            var data = await mageek.Decks_Content(request.User, request.DeckId);
            var reply = new Reply_DeckContent();
            reply.DeckContent = new();
            if (data.IsNullOrEmpty()) reply.DeckContent.IsNull = true;
            else
            {
                foreach (var item in data)
                    reply.DeckContent.Items.Add(new Reply_DeckCard()
                    {
                        DeckId = item.DeckId,
                        CardUuid = item.CardUuid,
                        Quantity = item.Quantity,
                        RelationType = item.RelationType
                    });
            }
            return reply;
        }

        [Authorize]
        public override async Task<Reply_Empty> Decks_Create(Request_CreateDeck request, ServerCallContext context)
        {
            var lines = new List<DeckCard>();
            if (!request.Cards.IsNull)
            {
                foreach (var v in request.Cards.Items)
                {
                    lines.Add(new DeckCard()
                    {
                        CardUuid = v.CardUuid,
                        DeckId = v.DeckId,
                        Quantity = v.Quantity,
                        RelationType = v.RelationType
                    });
                }
            }
            await mageek.Decks_Create(request.User, request.Title, request.Description, request.CardCount, request.DeckColors, lines);
            return new Reply_Empty();
        }

        [Authorize]
        public override async Task<Reply_Empty> Decks_Delete(Request_DeckId request, ServerCallContext context)
        {
            await mageek.Decks_Delete(request.User, request.DeckId);
            return new Reply_Empty();
        }

        [Authorize]
        public override async Task<Reply_Empty> Decks_Duplicate(Request_DeckId request, ServerCallContext context)
        {
            await mageek.Decks_Duplicate(request.User, request.DeckId);
            return new Reply_Empty();
        }

        [Authorize]
        public override async Task<Reply_Deck> Decks_Get(Request_DeckId request, ServerCallContext context)
        {
            var item = await mageek.Decks_Get(request.User, request.DeckId);
            return new Reply_Deck()
            {
                DeckId = item.DeckId,
                CardCount = item.CardCount,
                DeckColors = item.DeckColors,
                Description = item.Description,
                Title = item.Title
            };
        }

        [Authorize]
        public override async Task<Reply_ListPreco> Decks_Precos(Request_Empty request, ServerCallContext context)
        {
            var data = await mageek.Decks_Precos();
            var reply = new Reply_ListPreco();
            reply.PrecoList = new Wrapper_Reply_PrecoList();
            if (data.IsNullOrEmpty()) reply.PrecoList.IsNull = true;
            else
            {
                foreach (var item in data)
                {
                    var preco = new Reply_Preco()
                    {
                        Title = item.Title,
                        Code = item.Code,
                        Kind = item.Kind,
                        ReleaseDate = item.ReleaseDate,
                    };
                    preco.Cards = new Wrapper_Reply_DeckCardList();
                    if (item.Cards.IsNullOrEmpty()) preco.Cards.IsNull = true;
                    else
                    {
                        foreach (var v in item.Cards)
                        {
                            if (v!=null)
                            {
                                preco.Cards.Items.Add(new Reply_DeckCard()
                                {
                                    CardUuid = v.CardUuid,
                                    DeckId = v.DeckId == null ? "PrecoDecks": v.DeckId,
                                    Quantity = v.Quantity,
                                    RelationType = v.RelationType,
                                });

                            }
                        }
                    }
                    reply.PrecoList.Items.Add(preco);
                }
            }
            return reply;
        }

        [Authorize]
        public override async Task<Reply_Empty> Decks_Rename(Request_RenameDeck request, ServerCallContext context)
        {
            await mageek.Decks_Rename(request.User, request.DeckId, request.Title);
            return new Reply_Empty();
        }

        [Authorize]
        public override async Task<Reply_Empty> Decks_Save(Request_SaveDeck request, ServerCallContext context)
        {
            Deck deck = new Deck()
            {
                Title = request.Title,
                DeckId = request.DeckId,
                Description = request.Description,
                CardCount = request.CardCount,
                DeckColors = request.DeckColors,
            };
            List<DeckCard> cards = new List<DeckCard>();
            if(!request.Lines.IsNull)
            {
                foreach (var item in request.Lines.Items)
                {
                    cards.Add(new DeckCard()
                    {
                        CardUuid = item.CardUuid,
                        DeckId = item.DeckId,
                        Quantity = item.Quantity,
                        RelationType = item.RelationType
                    });
                }
            }
            await mageek.Decks_Save(request.User, deck, cards);
            return new Reply_Empty();
        }

        [Authorize]
        public override async Task<Reply_ListSet> Sets_All(Request_Empty request, ServerCallContext context)
        {
            var data = await mageek.Sets_All();
            Logger.Log("count: " + data.Count);
            var sets = new Reply_ListSet();
            sets.SetList = new Wrapper_Reply_SetList();
            if (data.IsNullOrEmpty()) sets.SetList.IsNull = true;
            else
            {
                foreach (var item in data)
                    sets.SetList.Items.Add(new Reply_Set()
                    {
                        BaseSetSize = item.BaseSetSize,
                        Block = item.Block,
                        Code = item.Code,
                        Name = item.Name,
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
                        ParentCode = item.ParentCode,
                        ReleaseDate = item.ReleaseDate,
                        TcgplayerGroupId = item.TcgplayerGroupId,
                        TokenSetCode = item.TokenSetCode,
                        TotalSetSize = item.TotalSetSize,
                        Type = item.Type
                    });
            }
            return sets;
        }

        [Authorize]
        public override async Task<Reply_Percentage> Sets_Completion(Request_SetCompletion request, ServerCallContext context)
        {
            var item = await mageek.Sets_Completion(request.User, request.Code, request.Strict);
            return new Reply_Percentage()
            {
                Percentage = item
            };
        }

        [Authorize]
        public override async Task<Reply_ListCardUuid> Sets_Content(Request_SetCode request, ServerCallContext context)
        {
            var data = await mageek.Sets_Content(request.SetCode);
            var reply = new Reply_ListCardUuid();
            reply.CardUuidList = new();
            if (data.IsNullOrEmpty()) reply.CardUuidList.IsNull = true;
            else
            {
                foreach (var item in data) reply.CardUuidList.Items.Add(item);
            }
            return reply;
        }

        [Authorize]
        public override async Task<Reply_Set> Sets_Get(Request_SetCode request, ServerCallContext context)
        {
            var item = await mageek.Sets_Get(request.SetCode);
            return new Reply_Set()
            {
                BaseSetSize = item.BaseSetSize,
                Block = item.Block,
                Code = item.Code,
                KeyruneCode = item.KeyruneCode,
                Languages = item.Languages,
                McmName = item.McmName,
                MtgoCode = item.MtgoCode,
                Name = item.Name,
                ParentCode = item.ParentCode,
                ReleaseDate = item.ReleaseDate,
                TokenSetCode = item.TokenSetCode,
                TotalSetSize = item.TotalSetSize,
                Type = item.Type,
                IsFoilOnly = item.IsFoilOnly,
                IsForeignOnly = item.IsForeignOnly,
                IsNonFoilOnly = item.IsNonFoilOnly,
                IsOnlineOnly = item.IsOnlineOnly,
                IsPartialPreview = item.IsPartialPreview,
                McmId = item.McmId,
                McmIdExtras = item.McmIdExtras,
                TcgplayerGroupId = item.TcgplayerGroupId,
            };
        }

        [Authorize]
        public override async Task<Reply_TagList> Tags_All(Request_User request, ServerCallContext context)
        {
            var data = await mageek.Tags_All(request.User);
            var reply = new Reply_TagList();
            if (data.IsNullOrEmpty()) reply.TagList.IsNull = true;
            else
            {
                foreach (var item in data)
                {
                    reply.TagList.Items.Add(new Reply_Tag()
                    {
                        ArchetypeId = "",
                        TagContent = item,
                        TagId = "",
                    });
                }
            }
            return reply;
        }

        [Authorize]
        public override async Task<Reply_HasTag> Tags_CardHasTag(Request_CardTag request, ServerCallContext context)
        {
            var item = await mageek.Tags_CardHasTag(request.User, request.CardName, request.Tag);
            return new Reply_HasTag()
            {
                HasTag = item
            };
        }

        [Authorize]
        public override async Task<Reply_TagList> Tags_GetCardTags(Request_CardNameUser request, ServerCallContext context)
        {
            var data = await mageek.Tags_GetCardTags(request.User, request.CardName);
            var reply = new Reply_TagList();
            reply.TagList = new();
            if (data.IsNullOrEmpty()) reply.TagList.IsNull = true; 
            else
            {
                foreach (var item in data)
                {
                    reply.TagList.Items.Add(new Reply_Tag()
                    {
                        ArchetypeId = item.ArchetypeId,
                        TagContent = item.TagContent,
                        TagId = item.TagId,
                    });
                }
            }
            return reply;
        }

        [Authorize]
        public override async Task<Reply_Empty> Tags_TagCard(Request_CardTag request, ServerCallContext context)
        {
            await mageek.Tags_TagCard(request.User, request.CardName, request.Tag);
            return new Reply_Empty();
        }

        [Authorize]
        public override async Task<Reply_Empty> Tags_UntagCard(Request_CardTag request, ServerCallContext context)
        {
            await mageek.Tags_UntagCard(request.User, request.CardName, request.Tag);
            return new Reply_Empty();
        }

    }

}
