// JavaScript source code
function triggerFlowContact()
{
    // Get Username and UserId
    var Username = Xrm.Utility.getGlobalContext().userSettings.userName;
    var UserId = Xrm.Utility.getGlobalContext().userSettings.userId;

    // Show notification
    var flowEndpoint = "https://prod-31.centralindia.logic.azure.com:443/workflows/3619706fe5a0468aabd375ae38e8794d/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=UuOlfLG5OYLpko4napapcK5g18jW5vNYneOV4RBCFD0https://prod-20.centralindia.logic.azure.com:443/workflows/70d432a357d2426e8c5e680144a9a3bb/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=Dj5B0071Lq3enyespKWzffUBfIbm-D2C11_UlKBFBbo";
    var req = new XMLHttpRequest();
    req.open("POST", flowEndpoint, true);
    req.setRequestHeader("Content-Type", "application/json");

    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            if (this.status === 200) {
                console.log("Flow triggered successfully.");
                // Show alert message
                var alertStrings = { confirmButtonLabel: "Okay", text: "Synchronization is in progress", title: "INFORMATION" };
                var alertOptions = { height: 120, width: 260 };
                Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                    function (success) {
                        console.log("Alert dialog closed");
                    },
                    function (error) {
                        console.log(error.message);
                    }
                );
                // Remove notification after 5 seconds
                setTimeout(function () {
                    Xrm.Page.ui.clearFormNotification();
                }, 5000);
            } else {
                console.error("Error triggering the flow: " + this.responseText);
                // Remove notification after 5 seconds
                setTimeout(function () {
                    Xrm.Page.ui.clearFormNotification();
                }, 5000);
            }
        }
    };

    // Create the request body with Username and UserId
    var requestBody = {
        Username: Username,
        UserId: UserId
    };

    // Send the request with the JSON payload
    req.send(JSON.stringify(requestBody));
}