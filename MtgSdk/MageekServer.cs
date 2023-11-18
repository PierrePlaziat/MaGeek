using MageekService.Data;
using MageekService.Data.Collection.Entities;
using MageekService.Data.Mtg.Entities;
using ScryfallApi.Client.Models;
using System;

namespace MageekService
{

    public interface IMageekServer
    {

        public abstract void CollecMove(string cardUuid, int quantityModification);
        public abstract int Collected(string cardUuid, bool onlyThisVariant);
        public abstract IEnumerable<Deck> GetDecks();
        public abstract Deck GetDeckInfo(string deckId);
        public abstract IEnumerable<DeckCard> GetDeckContent(string deckId);
        public abstract void CreateDeck(string title, string description, IEnumerable<DeckCard> content);
        public abstract void UpdateDeck(string deckId, string title, string description, IEnumerable<DeckCard> content);
        public abstract void DeleteDeck(string deckId);
        public abstract IEnumerable<Tag> GetExistingTags();
        public abstract IEnumerable<Tag> GetCardTags(string archetypeId);
        public abstract void TagCard(string archetypeId, string tag);
        public abstract void UnTagCard(string archetypeId, string tag);
        public abstract bool HasTag(string archetypeId, string tag);

    }

    public class MageekServer : IMageekServer
    {

        public void CollecMove(string cardUuid, int quantityModification)
        {
            MageekService.CollecMove(cardUuid,quantityModification).ConfigureAwait(false);
        }

        public int Collected(string cardUuid, bool onlyThisVariant)
        {
            return MageekService.Collected(cardUuid, onlyThisVariant).Result;
        }

        // TODO

        public void CreateDeck(string title, string description, IEnumerable<DeckCard> content)
        {
            throw new NotImplementedException();
        }

        public void DeleteDeck(string deckId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tag> GetCardTags(string archetypeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DeckCard> GetDeckContent(string deckId)
        {
            throw new NotImplementedException();
        }

        public Deck GetDeckInfo(string deckId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Deck> GetDecks()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tag> GetExistingTags()
        {
            throw new NotImplementedException();
        }

        public bool HasTag(string archetypeId, string tag)
        {
            throw new NotImplementedException();
        }

        public void TagCard(string archetypeId, string tag)
        {
            throw new NotImplementedException();
        }

        public void UnTagCard(string archetypeId, string tag)
        {
            throw new NotImplementedException();
        }

        public void UpdateDeck(string deckId, string title, string description, IEnumerable<DeckCard> content)
        {
            throw new NotImplementedException();
        }

    }

}