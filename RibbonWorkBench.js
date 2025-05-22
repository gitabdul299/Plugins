// JavaScript source code
function OnCreate(primaryControl) {
    var formContext = primaryControl;
    var name = formContext.getAttribute("aa_name").getValue();
    var record = {};
    record.aa_name = name; // Text

    Xrm.WebApi.createRecord("aa_ribbon", record).then(
        function success(result) {
            var newId = result.id;
            console.log(newId);
            alert("record created successfully");
        },
        function (error) {
            console.log(error.message);
        }
    );
}

 function OnUpdate(primaryControl) {
    var formContext = primaryControl;
    var recordId = formContext.data.entity.getId();
    var name = formContext.getAttribute("aa_name").getValue();
    var record = {};
    record.aa_name = name + " update from ribbon"; // Text

    Xrm.WebApi.updateRecord("aa_ribbon", recordId, record).then(
        function success(result) {
            var updatedId = result.id;
            console.log(updatedId);
            alert("updated successfully");
        },
        function (error) {
            console.log(error.message);
        }
    );
    alert("I am OnUpdate operation");
}

 function OnDelete(primaryControl) {
    var formContext = primaryControl;
    var recordId = formContext.data.entity.getId();

    Xrm.WebApi.deleteRecord("aa_ribbon", recordId).then(
        function success(result) {
            console.log(result);
            alert("deleted successfully");
        },
        function (error) {
            console.log(error.message);
        }
    );
}

function OnRetrieve(primaryControl) {
    var formContext = primaryControl;
    var lookup = formContext.getAttribute("aa_organiztion").getValue();
    var id = lookupVar[0].name;
    var Name = lookupVar[0].name;
    var logicalName = lookupVar[0].entityType;

    if (lookupVar != null &&lookupVar !=undefined) {

        Xrm.WebApi.retrieveRecord("account", id , "?$select=name,fax").then(
            function success(result) {
                console.log(result);
                // Columns
                var accountid = result["accountid"]; // Guid
                var name = result["name"]; // Text
                var fax = result["fax"]; // Text

                formContext.getAttribute("new_values").setValue("account name = " + name + "  fax value = " + fax );

                alert("account name = " + name + "  fax value = " + fax );

            },
            function (error) {
                console.log(error.message);
            }
        );
    }
    
}

// done on contact / logged in user aaplying security roles
function checkRoles(executionContext) {
    let formContext = executionContext.getFormContext();

    let hasRole = false;
    let roles = Xrm.Utility.getGlobalContext().userSettings.roles;
    //           ["AB Roles", "Sales Manager", "System Administrator"]
    roles.forEach(x => {
        if (x.name === "Basic User" || x.name === "Sales Manager") {
            hasRole = true;
            return;
        }
    });
    if (hasRole === true) {
        formContext.getControl("jobtitle").setVisible(true);
    } else {
        formContext.getControl("jobtitle").setVisible(false);
    }
}
//  get app name and make any field mandatory
function getCurrentModelDrivenAppName(executionContext) {
    let formContext = executionContext.getFormContext();
    Xrm.Utility.getGlobalContext().getCurrentAppName().then(
        function (appName) {
            if (appName == "Sales Hub") {
                formContext.getAttribute("parentcustomerid").setRequiredLevel("required");
            }
            else {
                formContext.getAttribute("parentcustomerid").setRequiredLevel("none");
            }
        },
        function (error) {
            alert("app name is not found");
        }
    );
}


function HideLeadQualifyButtonBasedOnRoles() {
    debugger;
    let hasRole = false;
    let roles = Xrm.Utility.getGlobalContext().userSettings.roles;

    for (let i = 0; i < roles.length; i++) {
        let role = roles[i];
        if (role.name === "Basic User" || role.name === "Sales Manager") {
            hasRole = true;
            break;  // Exiting the loop once the role is found
        }
    }

    return hasRole;
}

function checkUserRoles() {
    var userRoles = Xrm.Utility.getGlobalContext().userSettings.roles;
    var isVisible = false;

    userRoles.forEach(function (role) {
        if (role.name === "Basic User" || role.name === "Sales Manager") {
            isVisible = true;
        }
    });

    return isVisible;
}
function hideNotificationSection(executionContext) {
    var formContext = executionContext.getFormContext();

    // Get the tab by name
    var tab = formContext.ui.tabs.get("{258efb95-a79c-4c82-9b5c-92df064248b9}");

    if (tab) {
        // Get the section by name within the tab
        var section = tab.sections.get("notification");

        if (section) {
            // Hide the section
            section.setVisible(false);
        }
    }
}

function showNeedApprovalButton(primaryControl) {
    var formContext = primaryControl.getFormContext(); // Get the form context

    // Check if the Parent Account field (parentcustomerid) is populated
    var parentAccount = formContext.getAttribute("parentcustomerid");
    if (parentAccount && parentAccount.getValue()) {
        var accountName = parentAccount.getValue()[0].name; // Get the Account Name
        console.log("Parent Account Name:", accountName);

        // Check if the account name is "TCS"
        if (accountName === "TCS") {
            return true; // Show the button
        }
    }
    return false; // Hide the button if conditions are not met
}
function showNeedApprovalButton(primaryControl) {
    // Ensure the primaryControl (form context) is available
    if (!primaryControl) return false;

    // Get the form context from the primaryControl parameter
    var formContext = primaryControl;

    // Get the Parent Account lookup field
    var parentAccount = formContext.getAttribute("parentcustomerid"); // Lookup field

    if (parentAccount && parentAccount.getValue() != null) {
        // Get the name of the Parent Account
        var parentAccountName = parentAccount.getValue()[0].name;

        // Return true if the Parent Account name is "TCS"
        return parentAccountName === "Trey Research";
    }

    // Hide the button if no Parent Account is selected or it’s not "TCS"
    return false;
}







/*
function OnRetrieve(primaryControl) {
    debugger;
    formContext = primaryControl
    var name = formContext.getAttribute("aa_name").getValue();
    var recordId = formContext.data.entity.getId();
    alert("I am OnRetrieve operation" + name + "  Guid " + recordId);
    // alert(name);
    //alert(recordId);
}*/