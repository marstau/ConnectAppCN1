using System.Collections.Generic;
using ConnectApp.Api;
using ConnectApp.Models.Api;
using ConnectApp.Models.Model;
using ConnectApp.Models.State;
using ConnectApp.Models.ViewModel;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.Redux;
using UnityEngine;

namespace ConnectApp.redux.actions {
    public static partial class Actions {
        public static object fetchChannels(int page) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return ChannelApi.FetchChannels(page).Then(channelResponse => {
                        dispatcher.dispatch(new ChannelsAction {
                            discoverList = channelResponse.discoverList,
                            joinedList = channelResponse.joinedList,
                            discoverPage = channelResponse.discoverPage,
                            channelMap = channelResponse.channelMap
                        });
                        for (int i = 0; i < channelResponse.joinedList.Count; i++) {
                            dispatcher.dispatch(fetchChannelMessages(channelResponse.joinedList[i]));
                            dispatcher.dispatch(fetchChannelMembers(channelResponse.joinedList[i]));
                        }
                    })
                    .Catch(error => {
                        dispatcher.dispatch(new FetchPublicChannelsFailureAction());
                        Debug.Log(error);
                    });
            });
        }
        
        public static object fetchChannelMessages(string channelId, string before = null, string after = null) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return ChannelApi.FetchChannelMessages(channelId, before, after)
                    .Then(channelMessagesResponse => {
                        dispatcher.dispatch(new ChannelMessagesAction {
                            channelId = channelId,
                            messages = channelMessagesResponse.items,
                            before = before,
                            after = after
                        });
                    })
                    .Catch(error => {
                        dispatcher.dispatch(new FetchChannelMessagesFailureAction());
                        Debug.Log(error);
                    });
            });
        }
        
        public static object fetchChannelMembers(string channelId) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return ChannelApi.FetchChannelMembers(channelId).Then(channelMemberResponse => {
                        dispatcher.dispatch(new ChannelMemberAction {
                            channelId = channelId,
                            members = channelMemberResponse
                        });
                    })
                    .Catch(error => {
                        dispatcher.dispatch(new FetchChannelMemberFailureAction());
                        Debug.Log(error);
                    });
            });
        }
        
        public static object joinChannel(string channelId, string groupId = null) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return ChannelApi.JoinChannel(channelId, groupId).Then(joinChannelResponse => {
                        dispatcher.dispatch(new JoinChannelSuccessAction {
                            channelId = channelId
                        });
                    })
                    .Catch(error => {
                        dispatcher.dispatch(new JoinChannelFailureAction());
                        Debug.Log(error);
                    });
            });
        }
        
        public static object leaveChannel(string channelId, string groupId = null) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return ChannelApi.LeaveChannel(channelId, groupId).Then(leaveChannelResponse => {
                        dispatcher.dispatch(new LeaveChannelSuccessAction {
                            channelId = channelId
                        });
                        dispatcher.dispatch(new MainNavigatorPopAction());
                        dispatcher.dispatch(new MainNavigatorPopAction());
                    })
                    .Catch(error => {
                        dispatcher.dispatch(new LeaveChannelFailureAction());
                        Debug.Log(error);
                    });
            });
        }
    }

    public class ChannelsAction {
        public List<string> discoverList;
        public List<string> joinedList;
        public int discoverPage;
        public Dictionary<string, Channel> channelMap;

    }
    
    public class JoinedChannelsAction {
        public List<Channel> channels;
        public int currentPage;
        public List<int> pages;
        public int total;
    }

    public class ChannelMessagesAction {
        public string channelId;
        public List<ChannelMessage> messages;
        public string before;
        public string after;
        public bool hasMore;
        public bool hasMoreNew;
    }

    public class ChannelMemberAction {
        public string channelId;
        public List<ChannelMember> members;
    }
    
    public class FetchPublicChannelsSuccessAction : BaseAction {
    }
    
    public class FetchPublicChannelsFailureAction : BaseAction {
    }
    
    public class FetchJoinedChannelsSuccessAction : BaseAction {
    }
    
    public class FetchJoinedChannelsFailureAction : BaseAction {
    }
    
    public class FetchChannelMessagesSuccessAction : BaseAction {
    }
    
    public class FetchChannelMessagesFailureAction : BaseAction {
    }
    
    public class FetchChannelMemberSuccessAction : BaseAction {
    }
    
    public class FetchChannelMemberFailureAction : BaseAction {
    }
    
    public class JoinChannelSuccessAction : BaseAction {
        public string channelId;
    }
    
    public class JoinChannelFailureAction : BaseAction {
    }

    public class LeaveChannelSuccessAction : BaseAction {
        public string channelId;
    }
    
    public class LeaveChannelFailureAction : BaseAction {
    }

    public class StartSendChannelMessageAction : RequestAction {
    }

    public class MarkChannelMessageAsRead : RequestAction {
        public string channelId;
        public long nonce;
    }

    public class PushReadyAction : BaseAction {
        public SocketResponseSessionData readyData;
    }

    public class PushNewMessageAction : BaseAction {
        public SocketResponseMessageData messageData;
    }

    public class PushModifyMessageAction : BaseAction {
        public SocketResponseMessageData messageData;
    }

    public class PushDeleteMessageAction : BaseAction {
        public SocketResponseMessageData messageData;
    }

    public class PushPresentUpdateAction : BaseAction {
        public SocketResponsePresentUpdateData presentUpdateData;
    }

    public class PushChannelAddMemberAction : BaseAction {
        public SocketResponseChannelMemberChangeData memberData;
    }

    public class PushChannelRemoveMemberAction : BaseAction {
        public SocketResponseChannelMemberChangeData memberData;
    }
}