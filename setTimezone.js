// JavaScript source code
function setTimeZone(executionContext) {
    var formContext = executionContext.getFormContext(); // Get the form context

    // Check if the record is new (this will be true if the record is being created)
    if (formContext.ui.getFormType() === 1) { // 1 means New Record
        // Get the current user's time zone
        var userTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

        // Set the TimeZone field with the user's time zone
        formContext.getAttribute("aa_timezone").setValue(userTimeZone); // 'new_timezone' should be the schema name of your field
    }
}

function getIPAddress() {
    var client = new XMLHttpRequest();
    client.open("GET", "https://api.ipify.org?format=json", true);
    client.onreadystatechange = function () {
        if (client.readyState == 4 && client.status == 200) {
            var response = JSON.parse(client.responseText);
            var ipAddress = response.ip;

            // Set the IP Address field
            var ipField = Xrm.Page.getAttribute("aa_ipaddress"); // Ensure this matches the logical name of your IP Address field
            if (ipField) {
                ipField.setValue(ipAddress);
            }
        }
    };
    client.send();
}

// Execute the function on the creation of a new record
if (Xrm.Page.ui.getFormType() == 1) { // FormType 1 means it's a create form
    getIPAddress();
}

function setTimeZoneAndConvertDateTime(executionContext) {
    var formContext = executionContext.getFormContext();  // Get the form context
    var isNewRecord = formContext.ui.getFormType() === 1; // Check if the form is new (1 represents New Record)

    if (isNewRecord) {
        // Retrieve the user's time zone from the browser
        var userTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

        // Set the TimeZone field with the retrieved time zone
        formContext.getAttribute("aa_timezone").setValue(userTimeZone);

        // Convert current UTC datetime to the user's time zone datetime
        var currentDate = new Date(); // Current date and time in UTC
        var options = {
            weekday: 'long',
            year: 'numeric',
            month: 'long',
            day: 'numeric',
            hour: 'numeric',
            minute: 'numeric',
            second: 'numeric',
            timeZone: userTimeZone,
            timeZoneName: 'short'
        };

        var formattedDateTime = currentDate.toLocaleString('en-US', options);  // Convert to user's time zone format
        console.log("Converted DateTime in User's TimeZone: " + formattedDateTime);

        formContext.getAttribute("aa_timezone").setValue(formattedDateTime);  // Set formatted date-time into the "TimeZone" field

        // If you want to set this formatted datetime in another field, use the following code:
        // formContext.getAttribute("datetimefield").setValue(formattedDateTime);
    }
}

