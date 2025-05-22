function showLossReasonDialog(primaryControl) {
    var formContext = primaryControl; // Get form context

    // Check if the Opportunity is already closed
    var stateCode = formContext.getAttribute("statecode").getValue();
    if (stateCode !== 0) { // 0 = Open, 1 = Won, 2 = Lost
        Xrm.Navigation.openAlertDialog({ text: "This Opportunity is already closed and cannot be modified." });
        return; // Exit function
    }

    // Define loss reason options
    var options = {
        "954000000": "Invalid Contact Details",
        "954000001": "Lost to Other Vendor",
        "954000002": "Prospect Not Interested",
        "954000003": "Long Delivery Time",
        "954000004": "Other Reasons"
    };

    // Create selection prompt
    var message = "Select a Loss Reason:\n";
    var optionKeys = Object.keys(options);
    optionKeys.forEach((key, index) => {
        message += (index + 1) + ". " + options[key] + "\n";
    });

    var userInput = prompt(message, "1"); // Default to first option

    // Validate input
    if (userInput && !isNaN(userInput)) {
        var selectedIndex = parseInt(userInput) - 1;
        if (selectedIndex >= 0 && selectedIndex < optionKeys.length) {
            var selectedValue = parseInt(optionKeys[selectedIndex]); // Get corresponding option set value

            // Set the selected value in Opportunity
            formContext.getAttribute("aa_lostreasons").setValue(selectedValue);

            // Save the record
            formContext.data.entity.save();

            // Close Opportunity
            closeOpportunity(primaryControl);
        } else {
            alert("Invalid selection. Please try again.");
        }
    } else {
        alert("No selection made. Opportunity will not be closed.");
    }
}

function closeOpportunity(primaryControl) {
    var formContext = primaryControl;
    var opportunityId = formContext.data.entity.getId().replace("{", "").replace("}", "");

    var entity = {};
    entity["statecode"] = 2; // Lost
    entity["statuscode"] = 4; // Lost status

    Xrm.WebApi.updateRecord("opportunity", opportunityId, entity).then(
        function success(result) {
            console.log("Opportunity closed successfully.");
            Xrm.Navigation.openAlertDialog({ text: "Opportunity Closed as Lost." });
        },
        function error(error) {
            console.log(error.message);

