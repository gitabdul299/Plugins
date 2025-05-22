// JavaScript source code


function closeOpportunityAsLost(primaryControl) {
    var formContext = primaryControl;

    // Define loss reasons
    var lossReasons = [
        { id: "1", text: "Invalid Contact Details" },
        { id: "2", text: "Lost to Other Vendor" },
        { id: "3", text: "Prospect Not Interested" },
        { id: "4", text: "Long Delivery Time" },
        { id: "5", text: "Other Reasons" }
    ];

    // Show a selection prompt using a Window.confirm (basic) or Custom Dialog (advanced)
    var selectedReason = prompt(
        "Select Loss Reason:\n1. Invalid Contact Details\n2. Lost to Other Vendor\n3. Prospect Not Interested\n4. Long Delivery Time\n5. Other Reasons",
        "1"
    );

    if (!selectedReason || selectedReason < 1 || selectedReason > 5) {
        alert("Please select a valid loss reason.");
        return;
    }

    var selectedText = lossReasons[selectedReason - 1].text;

    // Prepare data to update the Opportunity record
    var opportunityId = formContext.data.entity.getId().replace("{", "").replace("}", "");
    var entity = {};
    entity["aa_lostreasons"] = selectedText; // Assuming 'new_lossreason' is the schema name of the custom field

    // Update the Opportunity record with the selected reason
    Xrm.WebApi.updateRecord("opportunity", opportunityId, entity).then(
        function success(result) {
            console.log("Lost reasons updated successfully.");

            // Now Close the Opportunity as Lost
            var closeOpportunity = {
                "opportunityid@odata.bind": "/opportunities(" + opportunityId + ")",
                "statuscode": 2, // "Lost" status code
                "statecode": 2, // Closed
                "description": "Opportunity Closed as Lost"
            };

            Xrm.WebApi.createRecord("opportunityclose", closeOpportunity).then(
                function success(result) {
                    alert("Opportunity successfully closed as lost.");
                    Xrm.Page.data.refresh(true); // Refresh the form
                },
                function error(error) {
                    console.log(error.message);
                    alert("Error while closing opportunity.");
                }
            );
        },
        function error(error) {
            console.log(error.message);
            alert("Error updating loss reason.");
        }
    );
}


/*function promptLostReason(executionContext) {
    var formContext = executionContext.getFormContext();

    // Ensure attributes exist before accessing them
    var stateAttr = formContext.getAttribute("statecode");
    var statusAttr = formContext.getAttribute("statuscode");

    if (!stateAttr || !statusAttr) {
        console.error("Statecode or Statuscode attribute not found.");
        return; // Stop execution if attributes are missing
    }

    var state = stateAttr.getValue();
    var statusReason = statusAttr.getValue();

    // 1 = Closed, 2 = Lost
    if (state === 1 && statusReason === 2) {
        var options = [
            { text: "Invalid Contact Details", value: 954000000 },
            { text: "Lost to Other Vendor", value: 954000001 },
            { text: "Prospect Not Interested", value: 954000002 },
            { text: "Long Delivery Time", value: 954000003 },
            { text: "Other Reasons", value: 954000004 }
        ];

        Xrm.Navigation.openConfirmDialog({
            title: "Select Lost Reason",
            text: "Please select a reason for losing this opportunity before closing:",
            confirmButtonLabel: "Proceed",
            cancelButtonLabel: "Cancel"
        }).then(function (response) {
            if (response.confirmed) {
                Xrm.Navigation.openDialog("Mscrm.SelectOption", { options: options })
                    .then(function (result) {
                        if (result && result.selectedOption) {
                            var selectedValue = result.selectedOption.value;
                            var lostReasonAttr = formContext.getAttribute("aa_lostreason");

                            if (lostReasonAttr) {
                                lostReasonAttr.setValue(selectedValue);
                                formContext.data.entity.save();
                            } else {
                                console.error("Lost Reason field (aa_lostreason) not found.");
                            }
                        }
                    });
            } else {
                executionContext.getEventArgs().preventDefault(); // Stop save if user cancels
            }
        });
    }
}
*/

/*function closeAsLostWithReason(primaryControl) {
    var formContext = primaryControl;

    // Define Lost Reason options
    var lostReasons = [
        { text: "Invalid Contact Details", value: 954000000 },
        { text: "Lost to Other Vendor", value: 954000001 },
        { text: "Prospect Not Interested", value: 954000002 },
        { text: "Long Delivery Time", value: 954000003 },
        { text: "Other Reasons", value: 954000004 }
    ];

    var options = lostReasons.map(r => r.text);

    // Show dialog for Lost Reason selection
    Xrm.Navigation.openConfirmDialog({
        text: "Please select a reason for the loss",
        title: "Close Opportunity as Lost",
        confirmButtonLabel: "OK",
        cancelButtonLabel: "Cancel"
    }).then(function (response) {
        if (response.confirmed) {
            var selectedReason = lostReasons.find(r => r.text === response.confirmButtonLabel);

            if (selectedReason) {
                var opportunityId = formContext.data.entity.getId().replace("{", "").replace("}", "");

                var opportunityClose = {
                    "opportunityid@odata.bind": "/opportunities(" + opportunityId + ")",
                    "subject": "Closed as Lost",
                    "aa_lostreason": selectedReason.value
                };

                var request = {
                    entity: opportunityClose,
                    entityName: "opportunityclose"
                };

                // Close Opportunity with Lost Reason
                Xrm.WebApi.online.createRecord("opportunityclose", request).then(
                    function success(result) {
                        console.log("Opportunity closed successfully!");
                        Xrm.Page.data.refresh(true);
                    },
                    function (error) {
                        console.log(error.message);
                    }
                );

                // Close related active activities
                closeRelatedActivities(opportunityId);
            }
        }
    });
}

// Function to Close Related Active Activities
function closeRelatedActivities(opportunityId) {
    var query = "/activitypointers?$filter=_regardingobjectid_value eq " + opportunityId + " and statecode eq 0";

    Xrm.WebApi.online.retrieveMultipleRecords("activitypointer", query).then(
        function success(results) {
            results.entities.forEach(function (activity) {
                var updateData = { statecode: 2, statuscode: 6 }; // Set to "Cancelled"

                Xrm.WebApi.online.updateRecord("activitypointer", activity.activityid, updateData).then(
                    function success() {
                        console.log("Activity " + activity.subject + " closed as Cancelled");
                    },
                    function error(error) {
                        console.log(error.message);
                    }
                );
            });
        },
        function error(error) {
            console.log(error.message);
        }
    );
}







function promptLossReason(e) {
    var formContext = e.getFormContext(); // Corrected typo


    /*var getStatusReason = formContext.getAttribute("opportunitystatuscode").getValue();

    if (getStatusReason === 2) {
        formContext.getControl("aa_lossreason").setVisible(true);
    } else {
        formContext.getControl("aa_lossreason").setVisible(false);

    }*/

    /*// Define loss reason options
    var lossOptions = [
        { value: 954000000, text: "Invalid Contact Details" },
        { value: 954000001, text: "Lost to Other Vendor" },
        { value: 954000002, text: "Prospect Not Interested" },
        //{ value: 100000003, text: "Long Delivery Time" },
        //{ value: 100000004, text: "Other Reasons" }
    ];

    // Convert options to display format
    var optionList = lossOptions.map(function (option, index) {
        return (index + 1) + ". " + option.text;
    }).join("\n");

    // Show prompt to user
    var selectedIndex = prompt("Select a Loss Reason:\n" + optionList);

    if (selectedIndex && selectedIndex >= 1 && selectedIndex <= lossOptions.length) {
        var selectedReason = lossOptions[selectedIndex - 1];

        // Set the selected loss reason on the Opportunity form
        formContext.getAttribute("aa_lossreason").setValue(selectedReason.value);

        // Save Opportunity
        formContext.data.entity.save();

        // Close related activities
        closeRelatedActivities(formContext.data.entity.getId());
    } else {
        alert("Please select a valid Loss Reason.");
        e.getEventArgs().preventDefault(); // Prevent form submission
    }
}

// Function to close related activities
function closeRelatedActivities(opportunityId) {
    var query = "/activitypointers?$filter=_regardingobjectid_value eq " + opportunityId + " and statecode eq 0";

    Xrm.WebApi.retrieveMultipleRecords("activitypointer", query).then(function (result) {
        var updatePromises = result.entities.map(function (activity) {
            var updateData = { statecode: 2, statuscode: 6 }; // Set status to "Cancelled"
            return Xrm.WebApi.updateRecord(activity["entitytype"], activity["activityid"], updateData);
        });

        Promise.all(updatePromises).then(function () {
            Xrm.Navigation.openAlertDialog({ title: "Success", text: "Opportunity closed and related activities cancelled." });
        }).catch(function (error) {
            console.error("Error closing activities: ", error.message);
        });
    }).catch(function (error) {
        console.error("Error retrieving activities: ", error.message);
    });
}/*
function promptLostReason(executionContext) {
    var formContext = executionContext.getFormContext();

    // Check if the Opportunity is being closed as Lost
    var status = formContext.getAttribute("statecode").getValue();
    var statusReason = formContext.getAttribute("statuscode").getValue();

    if (status === 1 && statusReason === 2) { // 1 = Closed, 2 = Lost
        var options = [
            { text: "Price too high", value: 954000000 },
            { text: "Chose a competitor", value: 954000001 },
            { text: "Lack of interest", value: 954000002 },
            { text: "Budget constraints", value: 954000003 },
            { text: "Other", value: 954000004 }
        ];

        // Prompt user to select a reason
        Xrm.Navigation.openConfirmDialog({
            title: "Select Lost Reason",
            text: "Please select a reason for losing this opportunity:",
            confirmButtonLabel: "Select",
            cancelButtonLabel: "Cancel"
        }).then(function (response) {
            if (response.confirmed) {
                Xrm.Navigation.openDialog("Mscrm.SelectOption", { options: options })
                    .then(function (result) {
                        if (result && result.selectedOption) {
                            var selectedValue = result.selectedOption.value;
                            formContext.getAttribute("aa_lostreason").setValue(selectedValue);
                            formContext.data.entity.save();
                        }
                    });
            }
        });
    }
}

function promptLostReason(executionContext) {
    var formContext = executionContext.getFormContext();

    // Check if the Opportunity is being closed as Lost
    var status = formContext.getAttribute("statecode").getValue();
    var statusReason = formContext.getAttribute("statuscode").getValue();

    if (status === 1 && statusReason === 2) { // 1 = Closed, 2 = Lost
        var options = [
            { text: "Price too high", value: 954000000 },
            { text: "Chose a competitor", value: 954000001 },
            { text: "Lack of interest", value: 954000002 },
            { text: "Budget constraints", value: 954000003 },
            { text: "Other", value: 954000004 }
        ];

        // Prompt user to select a reason
        Xrm.Navigation.openConfirmDialog({
            title: "Select Lost Reason",
            text: "Please select a reason for losing this opportunity:",
            confirmButtonLabel: "Select",
            cancelButtonLabel: "Cancel"
        }).then(function (response) {
            if (response.confirmed) {
                Xrm.Navigation.openDialog("Mscrm.SelectOption", { options: options })
                    .then(function (result) {
                        if (result && result.selectedOption) {
                            var selectedValue = result.selectedOption.value;
                            formContext.getAttribute("aa_lostreason").setValue(selectedValue);
                            formContext.data.entity.save();
                        }
                    });
            }
        });
    }
*/