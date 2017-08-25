"use strict";


function yourConfigIsInAnotherCastleJsonSchemaForm(elementContainer) {
    var Form = JSONSchemaForm.default;


    //I had major issues with JSONSchemaForm and definitions within my schema, sometimes it likes it and other times it doesnt.
    //It might be browser related. I have taken some of the code that parses the definitions and modified it slightly, so the schema
    //will be resolved before it is used.
    function mapDefinitionReferences(schema) {
        var currentProperty = schema.properties;
        var definitionBase = schema.definitions;

        function findPropertyValue(definitionPath) {
            var match = /^#\/definitions\/(.*)$/.exec(definitionPath);
            if (match && match[1]) {
                var parts = match[1].split("/");
                var current = definitionBase;
                for (var partIndex in parts) {
                    var part = parts[partIndex];
                    part = part.replace(/~1/g, "/").replace(/~0/g, "~");
                    if (current.hasOwnProperty(part)) {
                        current = current[part];
                    } else {
                        // No matching definition found, that's an error (bogus schema?)
                        throw new Error("Could not find a definition for " + definitionPath);
                    }
                }
                return current;
            }
            throw new Error("Could not find a definition for " + definitionPath);
        }

        function replaceDefinitions(root) {

            for (var property in root) {
                var currentPropertyValue = root[property];
                //Note replace lowest first
                if (typeof currentPropertyValue == 'object') {
                    root[property] = replaceDefinitions(currentPropertyValue);
                } else if (property.toLowerCase() == "$ref") {
                    return findPropertyValue(currentPropertyValue);
                }
            }
            return root;
        }
        definitionBase = replaceDefinitions(definitionBase);
        currentProperty = replaceDefinitions(currentProperty);

        schema.definitions = {};
    }

    var log = function log(type) {
        return console.log.bind(console, type);
    };
    var create = React.createElement(Form, {
        schema: {},
        //TODO add listener and browse away
        //onChange: log("changed"),
        onSubmit: function () {
            for (var index in _listeners) {
                var listener = _listeners[index];
                listener(schemaFormRenderedComponent.state.formData);
            }
        },
        //TODO add listener
        onError: log("errors")
    });
    function getFormData() {
        return schemaFormRenderedComponent.state.formData;
    }
    //TODO add init with the id for the element in it
    var schemaFormRenderedComponent = ReactDOM.render(create, elementContainer);
  
    function changeSelectedDetails(json, schema) {
        mapDefinitionReferences(schema);

        schemaFormRenderedComponent.state.schema = schema;
        schemaFormRenderedComponent.state.formData = json;

        schemaFormRenderedComponent.forceUpdate(function () {});
    }
    var _listeners = [];
    function addListener(listener) {
        _listeners.push(listener);
    }

    return {
        changeSelectedDetails: changeSelectedDetails,
        addListener: addListener,
        getFormData: getFormData
    };
};
