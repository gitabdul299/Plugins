
function openHtmlResource() {
    // Get the first name and last name from the form and concatenate them
    var firstName = Xrm.Page.getAttribute("firstname").getValue();
    var lastName = Xrm.Page.getAttribute("lastname").getValue();
    var contactName = firstName + " " + lastName;

    // Get the account name
    var accountName = Xrm.Page.getAttribute("aa_acountname").getValue() ? Xrm.Page.getAttribute("aa_acountname").getValue().name : "";

    // Construct the full URL for the web resource
    var clientUrl = Xrm.Utility.getGlobalContext().getClientUrl();
    var webResourceName = "aa_contactHtmlResource.html";
    var url = clientUrl + "/WebResources/" + webResourceName + "?contactName=" + encodeURIComponent(contactName) + "&accountName=" + encodeURIComponent(accountName);

    // Options for the popup window
    var windowOptions = "width=600,height=400";

    // Open the web resource in a new window
    window.open(url, "_blank", windowOptions);
}


