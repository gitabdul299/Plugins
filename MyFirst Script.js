// JavaScript source code
/*
function myTestFunction(executionContext){
    var formContext = executionContext.getFormContext();
}
function FunctionOnSave(executionContext){
    var formContext = executionContext.getFormContext();
    alert("I am on Save function");
}

function FunctionOnLoad(executionContext){
    var formContext = executionContext.getFormContext();
    alert("I am on Load function");
}

function FunctionOnChange(executionContext){
    var formContext = executionContext.getFormContext();
    alert("I am on Change function");
}
*/

function CRUDOperations(executionContext) {
    var formContext = executionContext.getFormContext();
    debugger;

    function getAttributeValue(attributeName) {
        var attribute = formContext.getAttribute(attributeName);
        return attribute ? attribute.getValue() : null;
    }
    function setAttributeValue(attributeName, value) {
        var attribute = formContext.getAttribute(attributeName);
        if (attribute && value !== null) { attribute.setValue(value); }
    }

    var username = formContext.getAttribute("new_name").getValue();
    formContext.getAttribute("new_name1").setValue(username);

    var wholeNumber = formContext.getAttribute("new_phoneno").getValue();
    formContext.getAttribute("new_phoneno1").setValue(wholeNumber);

    var twoOptions = formContext.getAttribute("new_areyoudeveloper").getValue();
    formContext.getAttribute("aa_areyoydeveloper1").setValue(twoOptions);

    var multipleLinesText = formContext.getAttribute("new_multiline").getValue();
    formContext.getAttribute("aa_multiline1").setValue(multipleLinesText);

    var dateTime = formContext.getAttribute("new_dob").getValue();
    formContext.getAttribute("new_dob1").setValue(dateTime);

    var getAddress = formContext.getAttribute("new_address").getValue();
    formContext.getAttribute("new_address1").setValue(getAddress);

    var getResult = formContext.getAttribute("new_result").getValue();
    formContext.getAttribute("new_result1").setValue(getResult);

    
    var getResult = formContext.getAttribute("new_result").getText();
    formContext.getAttribute("new_result1").setValue(getResult);

    var getSubject = formContext.getAttribute("new_subject").getValue();
    formContext.getAttribute("new_subject1").setValue(getSubject);

    var decimalNumber = formContext.getAttribute("aa_age").getValue();
    formContext.getAttribute("aa_age1").setValue(decimalNumber);

    var floatNumber = formContext.getAttribute("new_percentage").getValue();
    formContext.getAttribute("new_percentage1").setValue(floatNumber);

    // Ensuring the Two Optionset values are set correctly 
    if (typeof twoOptions === "number") {
        setAttributeValue("aa_areyoydeveloper1", twoOptions);
    } else {
        console.error("Value for Two Optionset should be a number");
    }
    // Ensuring the Multi Optionset values are set correctly
    var multiOptionset = getAttributeValue("new_multioptionset");
    if (Array.isArray(multiOptionset) && multiOptionset.every(val => typeof val === "number")) {
        setAttributeValue("new_multioptionset1", multiOptionset);
    } else {
        console.error("Values for Multi Optionset should be an array of numbers");
    }
}

    // Retrieve the lookup field value (Account entity)
    // get values of LookUp field 

    var lookupVar = formContext.getAttribute("new_organization");
    if (lookupVar && lookupVar.length > 0) {
        var ID = lookupVar[0].id;
        var Name = lookupVar[0].name;
        var LogicalName = lookupVar[0].entityType;
        console.log("Lookup ID:", ID, "Name:", Name, "LogicalName:", LogicalName);
    }


    // Lookup Field (Set value) 
    //var lookupValue = [{
    //  id: "record-guid", // Replace with actual GUID of the record 
    //name: "record-name", // Optional 
    //entityType: "account" // Replace with actual entity type 
    //}];
    //setAttributeValue("aa_organization1", lookupValue);


    // Or using an object array 
    var objectArray = new Array();
    objectArray[0] = new Object();
    objectArray[0].id = "83883308-7ad5-ea11-a813-000d3a33f3b4"; // Replace with actual account ID 
    objectArray[0].name = "name"; // Replace with actual account name 
    objectArray[0].entityType = "account"; // Replace with actual entity type 
formContext.getAttribute("aa_organization1").setValue(objectArray);

