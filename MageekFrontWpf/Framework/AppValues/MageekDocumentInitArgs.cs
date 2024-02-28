using MageekCore.Data.Collection.Entities;
using MageekCore.Data;
using System.Collections.Generic;
using System;
using MageekFrontWpf.Framework.BaseMvvm;

namespace MageekFrontWpf.Framework.AppValues
{

    public class MageekDocumentInitArgs : DocumentInitArgs
    {

        public Deck deck;
        public Preco preco;
        public List<DeckCard> import;

        public MageekDocumentInitArgs(Deck deck = null, Preco preco = null, List<DeckCard> import = null)
        {
            this.deck = deck;
            this.preco = preco;
            this.import = import;
            if (deck != null)
            {
                preco = null;
                import = null;
                documentId = deck.DeckId;
                documentTitle = deck.Title;
                validated = true;
            }
            if (preco != null)
            {
                import = null;
                string s = string.Concat("[", preco.Code, "] ", preco.Title);
                documentId = documentTitle = s;
                validated = true;
            }
            if (import != null)
            {
                documentId = documentTitle = DateTime.Now.ToString();
                validated = true;
            }
        }

    }

}