// JavaScript source code
function validateVehicleMake(primaryControl) {
    debugger;
    var formContext = primaryControl; // Get form context

    // Get values from the Lead form
    var makeId = formContext.getAttribute("aa_makeid") ? formContext.getAttribute("aa_makeid").getValue() : null;
    var makeName = formContext.getAttribute("aa_makename") ? formContext.getAttribute("aa_makename").getValue() : null;

    // Check if both fields are populated
    if (!makeId || !makeName) {
        Xrm.Navigation.openAlertDialog({ text: "Both Make Id and Make Name are required." });
        return;
    }

    var apiUrl = "https://vpic.nhtsa.dot.gov/api/vehicles/getallmakes?format=json";

    // Make an API call
    var req = new XMLHttpRequest();
    req.open("GET", apiUrl, true);
    req.onreadystatechange = function () {
        if (req.readyState === 4) {
            if (req.status === 200) {
                var response = JSON.parse(req.responseText);
                var isValid = false;

                // Check if Make Id and Make Name exist in API data
                if (response.Results) {
                    for (var i = 0; i < response.Results.length; i++) {
                        var make = response.Results[i];
                        if (make.Make_ID.toString() === makeId && make.Make_Name.toLowerCase() === makeName.toLowerCase()) {
                            isValid = true;
                            break;
                        }
                    }
                }

                // Show validation result
                if (isValid) {
                    Xrm.Navigation.openAlertDialog({ text: "Validation Successful: The Make Id and Make Name are valid!" });
                } else {
                    Xrm.Navigation.openAlertDialog({ text: "Validation Failed: The Make Id and Make Name do not match any records in the database." });
                }
            } else {
                Xrm.Navigation.openAlertDialog({ text: "Error fetching vehicle data from API." });
            }
        }
    };
    req.send();
}
