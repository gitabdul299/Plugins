// JavaScript source code
function hideMiscOption(executionContext) {
    var formContext = executionContext.getFormContext();
    var categoryField = formContext.getAttribute("aa_category"); // Update with actual logical name

    if (categoryField) {
        var selectedValue = categoryField.getValue();
        var options = categoryField.getOptions();

        // Check if the record is newly created
        if (selectedValue === null) {
            options.forEach(function (option) {
                if (option.text === "Misc") {
                    formContext.getControl("aa_category").removeOption(option.value);
                }
            });
        }
    }
}

