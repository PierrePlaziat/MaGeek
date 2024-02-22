using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MageekFrontWpf.AppValues
{

    public class CardSelectedMessage : ValueChangedMessage<string>
    {
        public CardSelectedMessage(string uuid) : base(uuid) {}
    }

    public class DeckSelectMessage : ValueChangedMessage<string>
    {
        public DeckSelectMessage(string id) : base(id) {}
    }

    public class UpdateCardCollecMessage : ValueChangedMessage<string>
    {
        public UpdateCardCollecMessage(string data) : base(data) {}
    }

    public class UpdateDeckMessage : ValueChangedMessage<string>
    {
        public UpdateDeckMessage(string data) : base(data) {}
    }

    public class UpdateDeckListMessage : ValueChangedMessage<string>
    {
        public UpdateDeckListMessage(string data) : base(data) {}
    }
        
    public class LoadLayoutMessage : ValueChangedMessage<string>
    {
        public LoadLayoutMessage(string data) : base(data) {}
    }
        
    public class SaveLayoutMessage : ValueChangedMessage<string>
    {
        public SaveLayoutMessage(string data) : base(data) {}
    }

}
