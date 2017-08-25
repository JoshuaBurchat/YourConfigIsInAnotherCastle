

function camelCaseObjectFormatter() {

    function findFirstLowerCaseIndex(text) {
        var index = 0;
        for (index = 0; index < text.length; index++) {
            if (text[index] !== text[index].toUpperCase()
                && text[index] === text[index].toLowerCase()) {
                return index;
            }
        }
        return -1;
    }
    function toCamelCase(text) {
        var firstLowerCaseIndex = findFirstLowerCaseIndex(text);

        var firstLowerCaseIndex = findFirstLowerCaseIndex(text);
        if (firstLowerCaseIndex == -1) {
            return text.toLowerCase();
        } else if (firstLowerCaseIndex == 1) {
            return (text.charAt(0).toLowerCase() + text.slice(1) || text);
        } else  {
            return (text.substr(0, firstLowerCaseIndex - 1).toLowerCase() + text.slice(firstLowerCaseIndex - 1))
        }
    }
    function propertiesToCamelCase(value) {
        var newValue;
        if (value instanceof Array) {
            newValue = [];
            for (var originalProperty in value) {
                var propertyValue = value[originalProperty];
                if (typeof propertyValue === "object") {
                    propertyValue = propertiesToCamelCase(propertyValue);
                }
                newValue.push(propertyValue);
            }
        } else {
            newValue = {};
            for (var originalProperty in value) {
                if (value.hasOwnProperty(originalProperty)) {
                    var newProperty = toCamelCase(originalProperty);

                    var propertyValue = value[originalProperty];
                    if (propertyValue !== null) {
                        if (propertyValue.constructor === Object) {
                            propertyValue = propertiesToCamelCase(propertyValue);
                        } else if (propertyValue instanceof Array) {
                            propertyValue = propertiesToCamelCase(propertyValue);
                        }
                    } 
                    newValue[newProperty] = propertyValue;
                }
            }
        }
        return newValue;
    }

    function format(value) {
        var value = propertiesToCamelCase(value);
        //Any other formatting steps applied here
        return value;
    }
    return {

        format: format
    };
};


