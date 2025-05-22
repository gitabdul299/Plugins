function validateSerialNumber(executionContext) {
    var formContext = executionContext.getFormContext();

    // Get the Device Lookup value from the Service Request entity
    var deviceLookup = formContext.getAttribute("aa_device").getValue();
    console.log("Device Lookup: ", deviceLookup);  // Log the device lookup for debugging

    if (!deviceLookup) {
        // If Device Lookup is empty, prevent save and alert the user
        var message = "Device is not selected. Please select a valid Device.";
        executionContext.getEventArgs().preventDefault();
        formContext.ui.setFormNotification(message, "ERROR", "deviceError");
        return;
    }

    // Clear any existing notifications
    formContext.ui.clearFormNotification("deviceError");

    // Fetch the device details (including serial number and device type) using the Device Lookup
    var deviceId = deviceLookup[0].id;  // Get the device ID from the lookup
    console.log("Device ID: " + deviceId);  // Log the device ID for debugging

    // Call the function to get the device details
    fetchDeviceDetails(deviceId, formContext, executionContext);
}

function fetchDeviceDetails(deviceId, formContext, executionContext) {
    // Build the Web API query to fetch the device details, including serial number and device type
    var query = "/api/data/v9.2/device_names(" + deviceId.replace("{", "").replace("}", "") + ")?$select=aa_serialnumber,aa_devicetype,aa_name";
    console.log("Web API Query: " + query);  // Log the query for debugging

    Xrm.WebApi.retrieveRecord("device_name", deviceId, query).then(
        function success(result) {
            console.log("Device Details Retrieved: ", result);  // Log the device details

            // Check if serial number exists and is valid
            var serialNumber = result.aa_serialnumber;
            if (!serialNumber) {
                // If Serial Number is empty, prevent save and show an error
                var message = "The selected device does not have a serial number. Please select a valid device.";
                executionContext.getEventArgs().preventDefault();
                formContext.ui.setFormNotification(message, "ERROR", "serialNumberError");
                return;
            }

            // If device is found, update the Serial Number field in the Service Request entity
            formContext.getAttribute("aa_serialnumber").setValue(serialNumber);  // Set the serial number on Service Request

            // Optionally, handle device type if you need to use it (for example, if it's important for validation)
            var deviceType = result.aa_devicetype;
            console.log("Device Type: ", deviceType);  // Log the device type for debugging

            // Optionally validate based on device type or other conditions here if needed

            formContext.ui.setFormNotification("Device validated successfully.", "INFO", "deviceValidationSuccess");
        },
        function error(error) {
            // Log any errors during the Web API call
            console.log("API Call Failed: ", error.message);

            // Provide a user-friendly message
            var message = "An error occurred while validating the device. Please try again later.";
            executionContext.getEventArgs().preventDefault();
            formContext.ui.setFormNotification(message, "ERROR", "deviceValidationError");
        }
    );
}
