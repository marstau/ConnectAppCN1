using System;
using System.Collections.Generic;
using ConnectApp.components;
using ConnectApp.constants;
using ConnectApp.models;
using ConnectApp.redux;
using ConnectApp.redux.actions;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using TextStyle = Unity.UIWidgets.painting.TextStyle;

namespace ConnectApp.screens {
    public class EventsScreen : StatefulWidget {
        public EventsScreen(Key key = null) : base(key) {
        }

        public override State createState() {
            return new _EventsScreen();
        }
    }

    internal class _EventsScreen : State<EventsScreen> {
        private const float headerHeight = 80;
        private PageController _pageController;
        private int _selectedIndex;

        private float _offsetY = 0;


        public override void initState() {
            base.initState();
            if (StoreProvider.store.state.eventState.events.Count == 0)
                StoreProvider.store.Dispatch(new FetchEventsAction {pageNumber = 1});
            _pageController = new PageController();
            _selectedIndex = 0;
        }


        private bool _onNotification(ScrollNotification notification, BuildContext context) {
            var pixels = notification.metrics.pixels;
            if (pixels >= 0) {
                if (pixels <= headerHeight) setState(() => { _offsetY = pixels / 2; });
            }
            else {
                if (_offsetY != 0) setState(() => { _offsetY = 0; });
            }

            return true;
        }

        private Widget _buildContentList(BuildContext context) {
            return new NotificationListener<ScrollNotification>(
                onNotification: (ScrollNotification notification) => {
                    _onNotification(notification, context);
                    return true;
                },
                child: new Flexible(
                    child: new Container(
                        child: new StoreConnector<AppState, Dictionary<string, object>>(
                            converter: (state, dispatch) => new Dictionary<string, object> {
                                {"loading", state.eventState.eventsLoading},
                                {"events", state.eventState.events}
                            },
                            builder: (context1, viewModel) => {
                                var loading = (bool) viewModel["loading"];
                                var events = viewModel["events"] as List<IEvent>;
                                var cardList = new List<Widget>();
                                if (!loading)
                                    events.ForEach(model => { cardList.Add(new EventCard(model)); });
                                else
                                    cardList.Add(new Container());

                                return new ListView(
                                    physics: new AlwaysScrollableScrollPhysics(),
                                    children: cardList
                                );
                            }
                        )
                    )
                )
            );
        }

        public override Widget build(BuildContext context) {
            return new Container(
                color: CColors.White,
                child: new Column(
                    children: new List<Widget> {
                        new CustomNavigationBar(
                            new Text("活动", style: CTextStyle.H2),
                            new List<Widget> {
                                new CustomButton(
                                    onPressed: () => { Navigator.pushNamed(context, "/search"); },
                                    child: new Container(
                                        child: new Icon(
                                            Icons.search,
                                            size: 28,
                                            color: Color.fromRGBO(181, 181, 181, 0.8f)
                                        )
                                    )
                                )
                            },
                            CColors.White,
                            _offsetY
                        ),
                        buildSelectView(),
                        buildContentView()
                    }
                )
            );
        }

        private Widget buildSelectItem(BuildContext context, string title, int index) {
            var textColor = CColors.TextTitle;
            Widget lineView = new Positioned(new Container());
            if (index == _selectedIndex) {
                textColor = CColors.PrimaryBlue;
                lineView = new Positioned(
                    bottom: 0,
                    left: 0,
                    right: 0,
                    child: new Row(
                        mainAxisAlignment: MainAxisAlignment.center,
                        children: new List<Widget> {
                            new Container(
                                width: 80,
                                height: 2,
                                decoration: new BoxDecoration(
                                    CColors.PrimaryBlue
                                )
                            )
                        }
                    )
                );
            }

            return new Container(
                child: new Stack(
                    children: new List<Widget> {
                        new CustomButton(
                            onPressed: () => {
                                if (_selectedIndex != index) setState(() => _selectedIndex = index);
                                _pageController.animateToPage(
                                    index,
                                    new TimeSpan(0, 0,
                                        0, 0, 250),
                                    Curves.ease
                                );
                            },
                            child: new Container(
                                height: 44,
                                width: 96,
                                alignment: Alignment.center,
                                child: new Text(
                                    title,
                                    style: new TextStyle(
                                        fontSize: 16,
                                        fontWeight: FontWeight.w400,
                                        color: textColor
                                    )
                                )
                            )
                        ),
                        lineView
                    }
                )
            );
        }

        private Widget buildSelectView() {
            return new Container(
                child: new Container(
                    height: 44,
                    child: new Row(
                        mainAxisAlignment: MainAxisAlignment.start,
                        children: new List<Widget> {
                            buildSelectItem(context, "即将开始", 0), buildSelectItem(context, "往期活动", 1)
                        }
                    )
                )
            );
        }

        private Widget mineList() {
            return new Container(
                child: new StoreConnector<AppState, Dictionary<string, object>>(
                    converter: (state, dispatch) => new Dictionary<string, object> {
                        {"loading", state.eventState.eventsLoading},
                        {"events", state.eventState.events},
                        {"eventDict", state.eventState.eventDict}
                    },
                    builder: (context1, viewModel) => {
                        var loading = (bool) viewModel["loading"];
                        var events = viewModel["events"] as List<string>;
                        var eventDict = viewModel["eventDict"] as Dictionary<string, IEvent>;
                        var cardList = new List<Widget>();
                        var eventObjs = new List<IEvent>();
                        if (events != null && events.Count > 0)
                            events.ForEach(eventId => {
                                if (eventDict != null && eventDict.ContainsKey(eventId))
                                    eventObjs.Add(eventDict[eventId]);
                            });
                        if (!loading)
                            eventObjs.ForEach(model => {
                                cardList.Add(new EventCard(
                                    model,
                                    () => {
                                        StoreProvider.store.Dispatch(new NavigatorToEventDetailAction()
                                            {eventId = model.id});
                                        Navigator.pushNamed(context, "/event-detail");
                                    }));
                            });
                        else
                            cardList.Add(new Container());

                        return new ListView(
                            physics: new AlwaysScrollableScrollPhysics(),
                            children: cardList
                        );
                    }
                )
            );
        }

        private Widget buildContentView() {
            return new Flexible(
                child: new Container(
                    padding: EdgeInsets.only(bottom: 49),
                    child: new PageView(
                        physics: new BouncingScrollPhysics(),
                        controller: _pageController,
                        onPageChanged: index => { setState(() => { _selectedIndex = index; }); },
                        children: new List<Widget> {
                            mineList(), mineList()
                        }
                    )
                )
            );
        }
    }
}