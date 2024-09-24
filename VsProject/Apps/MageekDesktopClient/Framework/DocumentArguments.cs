using MageekCore.Data.Collection.Entities;
using MageekCore.Data;
using System.Collections.Generic;
using System;
using PlaziatWpf.Docking;
using PlaziatWpf.Mvvm;
using PlaziatWpf.Services;
using MageekCore.Services;

namespace MageekDesktopClient.Framework
{

    public class DocumentArguments : AbstractDocumentArguments
    {

        public Deck deck;
        public Preco preco;
        public List<DeckCard> import;

        public DocumentArguments(Deck deck = null, Preco preco = null, List<DeckCard> import = null)
        {
            this.preco = preco;
            this.deck = deck;
            this.import = import;
            if (preco != null) InitWithPreco(preco);
            if (deck != null) InitFromCollec(deck);
            if (import != null) InitFromImport(import);
        }

        private void InitWithPreco(Preco preco)
        {
            import = null;
            string s = string.Concat("[", preco.Code, "] ", preco.Title);
            documentId = documentTitle = s;
            validated = true;
        }

        private void InitFromCollec(Deck deck)
        {
            preco = null;
            import = null;
            documentId = deck.DeckId;
            documentTitle = deck.Title;
            validated = true;
        }

        private void InitFromImport(List<DeckCard> import)
        {
            documentId = documentTitle = DateTime.Now.ToString();
            validated = true;
        }

    }

}