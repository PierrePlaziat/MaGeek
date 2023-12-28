using MageekService.Data.Collection.Entities;

namespace MageekService
{

    public interface IMageekServer
    {

        abstract Task<Tuple<int, int>> CollecMove(string cardUuid, int quantityModification);
        abstract Task<int> Collected(string cardUuid, bool onlyThisVariant);

        abstract Task<IEnumerable<Deck>> GetDecks();
        abstract Task<Deck> GetDeckInfo(string deckId);
        abstract Task<IEnumerable<DeckCard>> GetDeckContent(string deckId);
        abstract Task CreateDeck(string title, string description, IEnumerable<DeckCard> content);
        abstract Task UpdateDeck(string deckId, string title, string description, IEnumerable<DeckCard> content);
        abstract Task DeleteDeck(string deckId);

        abstract Task<IEnumerable<Tag>> GetExistingTags();
        abstract Task<IEnumerable<Tag>> GetCardTags(string archetypeId);
        abstract Task TagCard(string archetypeId, string tag);
        abstract Task UnTagCard(string archetypeId, string tag);
        abstract Task<bool> HasTag(string archetypeId, string tag);

    }

    public class MageekServer : IMageekServer
    {

        public async Task<Tuple<int, int>> CollecMove(string cardUuid, int quantityModification)
        {
            return await MageekService.CollecMove(cardUuid,quantityModification);
        }
        public async Task<int> Collected(string cardUuid, bool onlyThisVariant)
        {
            return await MageekService.Collected(cardUuid, onlyThisVariant);
        }

        public async Task CreateDeck(string title, string description, IEnumerable<DeckCard> content)
        {
            await MageekService.CreateDeck_Contructed(title, description, content);
        }
        public async Task UpdateDeck(string deckId, string title, string description, IEnumerable<DeckCard> content)
        {
            await MageekService.UpdateDeck(deckId, title, description, content);
        }
        public async Task DeleteDeck(string deckId)
        {
            await MageekService.DeleteDeck(deckId);
        }
        public async Task<IEnumerable<Tag>> GetCardTags(string archetypeId)
        {
            return await MageekService.GetTags(archetypeId);
        }
        public async Task<IEnumerable<DeckCard>> GetDeckContent(string deckId)
        {
            return await MageekService.GetDeckContent(deckId);
        }
        public async Task<Deck> GetDeckInfo(string deckId)
        {
            return await MageekService.GetDeck(deckId);
        }
        public async Task<IEnumerable<Deck>> GetDecks()
        {
            return await MageekService.GetDecks();
        }

        public async Task<IEnumerable<Tag>> GetExistingTags()
        {
            return await MageekService.GetTags();
        }
        public async Task<bool> HasTag(string archetypeId, string tag)
        {
            return await MageekService.HasTag(archetypeId,tag);
        }
        public async Task TagCard(string archetypeId, string tag)
        {
            await MageekService.TagCard(archetypeId, tag);
        }
        public async Task UnTagCard(string archetypeId, string tag)
        {
            await MageekService.UnTagCard(archetypeId, tag);
        }

    }

}