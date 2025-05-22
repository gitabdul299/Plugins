// JavaScript source code
function openLossReasonDialog(executionContext) {
    var formContext = executionContext.getFormContext();
    var opportunityId = formContext.data.entity.getId().replace("{", "").replace("}", "");

    var pageInput = {
        pageType: "webresource",
        webresourceName: "OpportunityLossReason",  // Web resource name (without .html)
        data: encodeURIComponent("id=" + opportunityId)
    };

    var navigationOptions = {
        target: 2,
        width: 400,
        height: 250,
        position: 1
    };

    Xrm.Navigation.navigateTo(pageInput, navigationOptions);
}
