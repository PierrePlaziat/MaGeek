namespace MageekService
{

    public interface IMageekServer
    {

        abstract Task<Tuple<int, int>> CollecMove(string cardUuid, int quantityModification);
        abstract Task<int> Collected(string cardUuid, bool onlyThisVariant);

        #region Later

        //abstract IEnumerable<Deck> GetDecks();
        //abstract Deck GetDeckInfo(string deckId);
        //abstract IEnumerable<DeckCard> GetDeckContent(string deckId);
        //abstract void CreateDeck(string title, string description, IEnumerable<DeckCard> content);
        //abstract void UpdateDeck(string deckId, string title, string description, IEnumerable<DeckCard> content);
        //abstract void DeleteDeck(string deckId);

        //abstract IEnumerable<Tag> GetExistingTags();
        //abstract IEnumerable<Tag> GetCardTags(string archetypeId);
        //abstract void TagCard(string archetypeId, string tag);
        //abstract void UnTagCard(string archetypeId, string tag);
        //abstract bool HasTag(string archetypeId, string tag);

        #endregion

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

        #region Later

        //public void CreateDeck(string title, string description, IEnumerable<DeckCard> content)
        //{
        //    throw new NotImplementedException();
        //}
        //public void DeleteDeck(string deckId)
        //{
        //    MageekService.DeleteDeck(deckId).ConfigureAwait(false);
        //}
        //public IEnumerable<Tag> GetCardTags(string archetypeId)
        //{
        //    throw new NotImplementedException();
        //}
        //public IEnumerable<DeckCard> GetDeckContent(string deckId)
        //{
        //    throw new NotImplementedException();
        //}
        //public Deck GetDeckInfo(string deckId)
        //{
        //    throw new NotImplementedException();
        //}
        //public IEnumerable<Deck> GetDecks()
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<Tag> GetExistingTags()
        //{
        //    throw new NotImplementedException();
        //}
        //public bool HasTag(string archetypeId, string tag)
        //{
        //    throw new NotImplementedException();
        //}
        //public void TagCard(string archetypeId, string tag)
        //{
        //    throw new NotImplementedException();
        //}
        //public void UnTagCard(string archetypeId, string tag)
        //{
        //    throw new NotImplementedException();
        //}
        //public void UpdateDeck(string deckId, string title, string description, IEnumerable<DeckCard> content)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion

    }

}