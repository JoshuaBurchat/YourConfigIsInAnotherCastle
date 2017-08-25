


function yourConfigIsInAnotherCastleDataService(objectFormatter) {

    var self = {
        url: '/Api/Config'
    }
    //Use the object formatter that comes with in another castle, or use a default, this prevents code change
    //required below if you decide to change the camel case features server side.
    objectFormatter = objectFormatter || {
        format: function (value) { return value; }
    };

    var _errorListeners = [];
    function addErrorListener(listener) {
        _errorListeners.push(listener);
    }
    function ajax(requestOptions) {
        var finalRequestOptions = {
            type: "GET",
            contentType: 'application/json',
            data: {},
            crossDomain: true,
            cache: false,
            url: self.url,
            error: function (r) {
                for (var index in _errorListeners) {
                    var currentListener = _errorListeners[index];
                    if (currentListener)
                        currentListener({
                            sourceType: 'jquery-ajax',
                            error: r
                        });
                }
            }
        };
        for (var property in requestOptions) {
            finalRequestOptions[property] = requestOptions[property];
        }
        if (requestOptions.success) {
            //Stored to prevent reference changing issues
            var requestedSuccess = requestOptions.success;
            finalRequestOptions.success = function (data) {
                var data = objectFormatter.format(data);
                requestedSuccess(data);
            }
        }

        return $.ajax(finalRequestOptions);
    }
    var _currentSearch = null;
    function search(tagIds, callback) {
        if (_currentSearch) _currentSearch.abort();
        _currentSearch = ajax({
            type: "GET",
            contentType: '',
            data: { tagIds: tagIds },
            success: callback
        });
    }
    function getTags(callback) {
        ajax({
            type: "GET",
            url: self.url + '/tags',
            success: callback
        });
    }

    function get(id, callback) {
        ajax({
            type: "GET",
            url: self.url + '/' + id,
            success: callback
        });
    }
    function remove(id, callback) {
        ajax({
            type: "DELETE",
            data: JSON.stringify({ id: id }),
            success: callback
        });
    }
    function update(id, value, format, callback) {
        ajax({
            type: "PUT",
            data: JSON.stringify({ id: id, value: value, format: format }),
            success: callback
        });
    }
    function updateJson(id, json, callback) {
        update(id, json, 'json', callback);
    }
    function updateXml(id, xml, callback) {
        update(id, xml, 'xml', callback);
    }
    function addUpdateSchema(id, name, systemName, xml, xmlSchema, jsonSchema, tags, callback) {
        ajax({
            type: "POST",
            data: JSON.stringify({ id: id, xml: xml, xmlSchema: xmlSchema, jsonSchema: jsonSchema, name: name, systemName: systemName, tags: tags }),
            success: callback
        });
    }
    self.search = search;
    self.get = get;
    self.getTags = getTags;
    self.remove = remove;
    self.updateJson = updateJson;
    self.updateXml = updateXml;
    self.addUpdateSchema = addUpdateSchema;
    self.addErrorListener = addErrorListener;

    return self;
};