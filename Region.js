// JavaScript source code
function onRegionChange(executionContext) {
    var formContext = executionContext.getFormContext();
    debugger;
    var regionValue = formContext.getAttribute("aa_region").getValue();

    // Set the Region Toll Free Number based on Region
    if (regionValue == 954000002) { // South
        formContext.getAttribute("aa_regiontollfreenumber").setValue(1111111);
    } else if (regionValue == 954000003) { // North
        formContext.getAttribute("aa_regiontollfreenumber").setValue(2222222);
    } else if (regionValue == 954000000) { // East
        formContext.getAttribute("aa_regiontollfreenumber").setValue(3333333);
    } else if (regionValue == 954000001) { // West
        formContext.getAttribute("aa_regiontollfreenumber").setValue(4444444);
    }

}

function FieldOperations(executionContext) {
    var formContext = executionContext.getFormContext();
    var regionValue = formContext.getAttribute("aa_region").getValue();

    if (regionValue == 954000002) { // South
        formContext.getAttribute("aa_regiontollfreenumber").setRequiredLevel("required");
        formContext.getControl("aa_statecode").setDisabled(true);
        formContext.getControl("aa_regioncode").setVisible(false);
    } else {
        formContext.getAttribute("aa_regiontollfreenumber").setRequiredLevel("none");
        formContext.getControl("aa_statecode").setDisabled(false);
        formContext.getControl("aa_regioncode").setVisible(true);
    }
}

 function tabAndSection(executionContext) {
    var formContext = executionContext.getFormContext();
    var regionValue = formContext.getAttribute("aa_region").getValue();

    // Hide all tabs initially
    formContext.ui.tabs.get("tab_2").setVisible(false);
    formContext.ui.tabs.get("tab_3").setVisible(false);
    formContext.ui.tabs.get("tab_4").setVisible(false);
    formContext.ui.tabs.get("tab_5").setVisible(false);

    // Show the corresponding tab based on the selected region
    if (regionValue == 954000002) { // South
        formContext.ui.tabs.get("tab_2").setVisible(true);
    } else if (regionValue == 954000003) { // North
        formContext.ui.tabs.get("tab_3").setVisible(true);
    } else if (regionValue == 954000000) { // East
        formContext.ui.tabs.get("tab_4").setVisible(true);
    } else if (regionValue == 954000001) { // West
        formContext.ui.tabs.get("tab_5").setVisible(true);
    }
}

 // Call the pincode function here to ensure it runs after the region value is set
 function pincode(executionContext) {
        var formContext = executionContext.getFormContext();
        var regionValue = formContext.getAttribute("aa_region").getValue();

     if (regionValue == 954000002) { // South 
            formContext.getAttribute("aa_pincode").setValue([954000001]); // Example: 431604 
    }
      else if (regionValue == 954000003) { // North 
            formContext.getAttribute("aa_pincode").setValue([954000000]); // Example: 431605 
    }
     else if (regionValue == 954000000) { // East 
            formContext.getAttribute("aa_pincode").setValue([954000004, 954000003]); // Example: 431601, 431602 
    } 
      else if (regionValue == 954000001) { // West 
            formContext.getAttribute("aa_pincode").setValue([954000002]); // Example: 431603
    }
     else {
        formContext.getAttribute("aa_pincode").setValue(null); // Clear the Pincode if no region selected
    }
}

    /*
        // Initialize the toll-free number
        var tollFreeNumber = null;
        
        // Check the value of the Region field and set the appropriate toll-free number
        switch (regionValue) {
            case 954000002: // South
                tollFreeNumber = 1111111;
                break;
            case 954000003: // North
                tollFreeNumber = 2222222;
                break; 
                
            case 954000000:
                tollFreeNumber = 3333333;
                break;
            case 954000001: // West
                tollFreeNumber = 4444444;
                break;
        }
    
        // Set the Region Toll Free Number field
        formContext.getAttribute("aa_regiontollfreenumber").setValue(tollFreeNumber);
    
    }
    */

    
