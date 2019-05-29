using ConnectApp.api;
using ConnectApp.Components;
using ConnectApp.constants;
using ConnectApp.Models.State;
using Unity.UIWidgets.Redux;
using UnityEngine;

namespace ConnectApp.redux.actions {
    public class StartReportItemAction : RequestAction {
        public string itemId;
        public string itemType;
        public string reportContext;
    }

    public class ReportItemSuccessAction : BaseAction {
    }

    public class ReportItemFailureAction : BaseAction {
    }

    public static partial class Actions {
        public static object reportItem(string itemId, string itemType, string reportContext) {
            return new ThunkAction<AppState>((dispatcher, getState) => {
                return ReportApi.ReportItem(itemId, itemType, reportContext)
                    .Then(() => {
                        dispatcher.dispatch(new MainNavigatorPopAction());
                        CustomDialogUtils.showToast("举报成功", Icons.sentiment_satisfied);
                        dispatcher.dispatch(new ReportItemSuccessAction());
                    })
                    .Catch(error => {
                        CustomDialogUtils.showToast("举报失败", Icons.sentiment_dissatisfied);
                        dispatcher.dispatch(new ReportItemFailureAction());
                        Debug.Log(error);
                    });
            });
        }
    }
}