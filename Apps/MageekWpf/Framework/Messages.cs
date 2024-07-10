using CommunityToolkit.Mvvm.Messaging.Messages;
using System;

namespace MageekDesktop.Framework
{

    public class LaunchAppMessage : ValueChangedMessage<string>
    {
        public LaunchAppMessage(string _) : base(_) { }
    }

    public class CardSelectedMessage : ValueChangedMessage<string>
    {
        public CardSelectedMessage(string uuid) : base(uuid) { }
    }

    public class UpdateDeckListMessage : ValueChangedMessage<string>
    {
        public UpdateDeckListMessage(string data) : base(data) { }
    }

    public class AddCardToDeckMessage : ValueChangedMessage<Tuple<string, string>>
    {
        public AddCardToDeckMessage(Tuple<string, string> data) : base(data) { }
    }

}
