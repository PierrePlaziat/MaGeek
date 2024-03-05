using MageekCore.Data.Collection.Entities;

namespace MageekServer
{

    public interface IMageekServer
    {

        abstract Task<Tuple<int, int>> CollecMove(string cardUuid, int quantityModification);
        abstract Task<int> Collected(string cardUuid, bool onlyThisVariant);

        abstract Task<IEnumerable<Deck>> GetDecks();
        abstract Task<Deck> GetDeckInfo(string deckId);
        abstract Task<IEnumerable<DeckCard>> GetDeckContent(string deckId);
        abstract Task CreateDeck(string title, string description, string colors, int count, IEnumerable<DeckCard> content);
        abstract Task UpdateDeck(string deckId, string title, string description, string colors, int count, IEnumerable<DeckCard> content);
        abstract Task DeleteDeck(string deckId);

        abstract Task<IEnumerable<Tag>> GetExistingTags();
        abstract Task<IEnumerable<Tag>> GetCardTags(string archetypeId);
        abstract Task TagCard(string archetypeId, string tag);
        abstract Task UnTagCard(string archetypeId, string tag);
        abstract Task<bool> HasTag(string archetypeId, string tag);

    }

    public class MageekServer : IMageekServer
    {
        private MageekCore.MageekService mageek;

        public MageekServer(MageekCore.MageekService mageek)
        {
            this.mageek = mageek;
            mageek.Initialize().ConfigureAwait(true);
        }

        public async Task<Tuple<int, int>> CollecMove(string cardUuid, int quantityModification)
        {
            return await mageek.CollecMove(cardUuid,quantityModification);
        }
        public async Task<int> Collected(string cardUuid, bool onlyThisVariant)
        {
            return await mageek.Collected(cardUuid, onlyThisVariant);
        }

        public async Task CreateDeck(string title, string description, string colors, int count, IEnumerable<DeckCard> content)
        {
            await mageek.CreateDeck(title, description,colors, count, content);
        }
        public async Task UpdateDeck(string deckId, string title, string description, string colors, int count, IEnumerable<DeckCard> content)
        {
            await mageek.UpdateDeck(deckId, title, description, colors, count, content);
        }
        public async Task DeleteDeck(string deckId)
        {
            await mageek.DeleteDeck(deckId);
        }
        public async Task<IEnumerable<Tag>> GetCardTags(string archetypeId)
        {
            return await mageek.GetTags(archetypeId);
        }
        public async Task<IEnumerable<DeckCard>> GetDeckContent(string deckId)
        {
            return await mageek.GetDeckContent(deckId);
        }
        public async Task<Deck> GetDeckInfo(string deckId)
        {
            return await mageek.GetDeck(deckId);
        }
        public async Task<IEnumerable<Deck>> GetDecks()
        {
            return await mageek.GetDecks();
        }

        public async Task<IEnumerable<Tag>> GetExistingTags()
        {
            return await mageek.GetTags();
        }
        public async Task<bool> HasTag(string archetypeId, string tag)
        {
            return await mageek.HasTag(archetypeId,tag);
        }
        public async Task TagCard(string archetypeId, string tag)
        {
            await mageek.TagCard(archetypeId, tag);
        }
        public async Task UnTagCard(string archetypeId, string tag)
        {
            await mageek.UnTagCard(archetypeId, tag);
        }

    }

}