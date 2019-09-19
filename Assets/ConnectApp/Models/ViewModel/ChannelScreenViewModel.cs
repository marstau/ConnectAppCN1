using System.Collections.Generic;
using ConnectApp.Models.Model;

namespace ConnectApp.Models.ViewModel {
    public class ChannelScreenViewModel {
        public ChannelView channelInfo;
        public List<ChannelMessageView> messages;
        public int newMessageCount;
        public string me;
        public bool messageLoading;
        public bool hasMore;
    }
}