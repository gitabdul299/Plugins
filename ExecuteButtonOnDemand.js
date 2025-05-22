function triggerFlowContact()
{
    debugger;

    // Get Username and UserId
    var Username = Xrm.Utility.getGlobalContext().userSettings.userName;
    var UserId = Xrm.Utility.getGlobalContext().userSettings.userId;

    // Replace this with your valid Power Automate HTTP endpoint
    var flowEndpoint = "https://prod-31.centralindia.logic.azure.com:443/workflows/3619706fe5a0468aabd375ae38e8794d/triggers/manual/paths/invoke?api-version=2016-06-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=UuOlfLG5OYLpko4napapcK5g18jW5vNYneOV4RBCFD0";

    // Create a new HTTP request
    var req = new XMLHttpRequest();
    req.open("POST", flowEndpoint, true);
    req.setRequestHeader("Content-Type", "application/json");

    // Handle the response
    req.onreadystatechange = function () {
        if (this.readyState === 4) {
            if (this.status === 200) {
                console.log("Flow triggered successfully.");

                // Show alert message
                var alertStrings = {
                    confirmButtonLabel: "Okay",
                    text: "Synchronization is in progress",
                    title: "INFORMATION"
                };
                var alertOptions = { height: 120, width: 260 };
                Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
                    function () {
                        console.log("Alert dialog closed");
                    },
                    function (error) {
                        console.log("Error showing alert: " + error.message);
                    }
                );

                // Remove notification after 5 seconds
                setTimeout(function () {
                    Xrm.App.clearGlobalNotification();
                }, 5000);
            } else {
                console.error("Error triggering the flow: " + this.responseText);

                // Show error notification
                Xrm.App.addGlobalNotification({
                    level: "ERROR",
                    message: "Failed to trigger the flow. Please try again.",
                    notificationId: "flowError"
                });

                // Remove error notification after 5 seconds
                setTimeout(function () {
                    Xrm.App.clearGlobalNotification("flowError");
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