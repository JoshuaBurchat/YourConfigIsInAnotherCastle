var yourConfigIsInAnotherCastleViewModels = {
    tagManager: function (
        configDataService,
        addTagSelectableCall,
        clearSelectableTagsCall,
        addTagSelectedCall,
        clearSelectedTagsCall
    ) {
        var _tags = [];
        var _selectedTags = [];

        var self = {
            onChange: function () { }
        };
        function newGuid() {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                    .toString(16)
                    .substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
                s4() + '-' + s4() + s4() + s4();
        }

        function loadTags() {
            configDataService.getTags(populateTags);
        }
        function getSelectedTags() {
            var copiedTags = [];
            for (var index in _selectedTags) {
                var currentTag = _selectedTags[index];
                var saveTag = { id: currentTag.isNew ? 0 : currentTag.id, value: currentTag.value };
                copiedTags.push(saveTag);
            }
            return copiedTags;
        }
        function getSelectedTagIds() {
            var ids = [];
            for (var index in _selectedTags) {
                ids[index] = _selectedTags[index].id;
            }
            return ids;
        }

        function find(array, findFunction) {
            if (array && findFunction) {
                for (var index in array) {
                    var current = array[index];
                    if (findFunction(current)) {
                        return current;
                    }
                }
            }
            return null;
        };
        function populateTags(tags) {
            _tags = tags || _tags || [];
            if (clearSelectableTagsCall) clearSelectableTagsCall.call(self);


            var unselectedTags = _tags.filter(function (t) {
                return !find(_selectedTags, function (s) {
                    return s.id == t.id;
                })
            })

            if (unselectedTags) {
                for (var tagIndex in unselectedTags) {
                    var tag = unselectedTags[tagIndex];
                    if (addTagSelectableCall) addTagSelectableCall.call(self, tag.id, tag.value);
                }
            }
        }
        function selectTag(tagId) {
            setSelectedTags([tagId], false);
        }
        function unselectTag(tagId) {
            for (var index in _selectedTags) {
                var selectedTag = _selectedTags[index];
                if (selectedTag.id == tagId) {
                    _selectedTags.splice(index, 1);
                    break;
                }
            }

            populateTags();
            if (self.onChange) self.onChange(self);
        }
        function setSelectedTags(tagIds, clear) {
            if (clear) {
                if (clearSelectedTagsCall) clearSelectedTagsCall.call(self);

                _selectedTags = [];
            }
            var hadAnyUpdates = false;
            for (var tagIdIndex in tagIds) {
                var currentTagId = tagIds[tagIdIndex];
                if (currentTagId) {
                    for (var index in _selectedTags) {
                        var selectedTag = _selectedTags[index];
                        if (selectedTag.id == currentTagId) {
                            return;
                        }
                    }
                    for (var index in _tags) {
                        var tag = _tags[index];
                        if (tag.id == currentTagId) {
                            _selectedTags.push(tag);
                            if (addTagSelectedCall) addTagSelectedCall.call(self, tag.id, tag.value);


                            hadAnyUpdates = true;
                            break;
                        }
                    }
                }
            }
            populateTags();
            if (hadAnyUpdates && self.onChange) self.onChange(self);
        }
        function addNewSelectedTag(value) {
            var tempId = newGuid();
            _tags.push({ value: value, id: tempId, isNew: true });
            setSelectedTags([tempId])
        }
        function addNewTag(tagValue) {
            if (tagValue) {
                var existingTag = null;
                //find existing value
                for (var index in _tags) {
                    var tag = _tags[index];
                    if (tag.value == tagValue) {
                        existingTag = tag;
                        break;
                    }
                }
                if (existingTag) {
                    setSelectedTags([existingTag.id]);
                } else {
                    addNewSelectedTag(tagValue);
                }
            }
        }

        function init() {
            setSelectedTags([], true);
            loadTags();
        }
        self.loadTags = loadTags;
        self.getSelectedTags = getSelectedTags;
        self.getSelectedTagIds = getSelectedTagIds;
        self.selectTag = selectTag;
        self.setSelectedTags = setSelectedTags;
        self.unselectTag = unselectTag;
        self.addNewTag = addNewTag;
        self.init = init;
        return self;
    },

    //displayCalls { show, hide }
    //modelCalls {set, get }
    schemaEntry: function (configDataServices, tagManagement, displayCalls, modelCalls) {
        displayCalls = displayCalls || { show: function () { }, hide: function () { } };
        modelCalls = modelCalls || { set: function () { }, get: function () { return {}; } };

        var self = {

        };

        var _currentRecord = null;
        function init(messagingSection, searchSection) {
            tagManagement.init();
            self.messagingSection = messagingSection;
            self.searchSection = searchSection;
        }
        function validate(record) {
            return true;
        }

        function save() {

            var updateData = modelCalls.get();
            updateData.tags = tagManagement.getSelectedTags();;



            if (_currentRecord) {
                updateData.id = _currentRecord.id;
            }
            if (validate(updateData)) {
                configDataServices.addUpdateSchema(
                    updateData.id,
                    updateData.name,
                    updateData.systemName,
                    updateData.xml,
                    updateData.xmlSchema,
                    updateData.jsonSchema,
                    updateData.tags,
                    saveResults);
            }
        }


        function saveResults(results) {
            if (results.successful) {
                displayCalls.hide();
                self.messagingSection.success([results.record.name + ' has been saved successfully']);
                //Refresh full list to ensure all fields are updated
                tagManagement.loadTags();
                self.searchSection.refresh(true);
            } else {
                self.messagingSection.error(results.errors, true);
            }
        }
        function recordSelected(record) {
            _currentRecord = record;

            modelCalls.set(record)

            var tagIds = [];
            for (var index in record.tags) {
                tagIds.push(record.tags[index].id);
            }
            tagManagement.setSelectedTags(tagIds, true);
            displayCalls.show();
        }
        self.init = init;
        self.recordSelected = recordSelected;
        self.save = save;
        return self;
    },
    //displayCalls { show, hide }
    //modelCalls {set, get }
    dataEntry: function (configDataServices, displayCalls, modelCalls) {
        displayCalls = displayCalls || { show: function () { }, hide: function () { } };
        modelCalls = modelCalls || { set: function () { }, getJson: function () { return {}; } };

        //TODO validation
        var _currentRecord = null;
        function init(messagingSection, searchSection) {
            self.messagingSection = messagingSection;
            self.searchSection = searchSection;
        }
        function save() {
            var jsonData = modelCalls.getJson();
            if (_currentRecord) {
                configDataServices.updateJson(_currentRecord.id, jsonData, saveResults);
            }
        }

        function saveResults(results) {
            if (results.successful) {
                _currentRecord.json = results.record.json;
                displayCalls.hide();
                self.messagingSection.success([_currentRecord.name + ' has been saved successfully']);
                self.searchSection.refresh(true);
            } else {
                self.messagingSection.error(results.errors, true);
            }
        }
        function recordSelected(record) {
            _currentRecord = record;
            modelCalls.set(record);
            displayCalls.show();
        }
        self.save = save;
        self.init = init;
        self.recordSelected = recordSelected;


        return self;
    },
    //filterCalls {get {}}
    searchSection: function (configDataServices, recordsLoadedCall, filterCalls) {
        filterCalls = filterCalls || {
            get: function () {
                return { nameCotains: '', systemName: '', tagIds : [] };
            }, set : function(model) {
            
            }
        }
        var _records = [];

        var self = {

        };
        function init(messagingSection, dataEntrySection, schemaEntrySection) {
            refresh(true);

            self.messagingSection = messagingSection;
            self.dataEntrySection = dataEntrySection;
            self.schemaEntrySection = schemaEntrySection;

        }
      
        function refresh() {
            var filterModel = filterCalls.get();
            configDataServices.search(filterModel.tagIds, function (records) {
                if (records) {
                    //TODO This is inefficient and maybe there should be server side or check that the client side filters seperately for changes
                    _records = records.filter(function (r) {
                        return (r.systemName == filterModel.systemName || !filterModel.systemName)
                            && r.name.includes(filterModel.nameCotains)
                    });
                    if (recordsLoadedCall)
                        recordsLoadedCall.call(self, _records);
                } else {
                    alert("No results found");//Error handling here
                }
            });
           
        }

        function recordSelectedEditData(id) {
            if (self.dataEntrySection) {
                for (var index in _records) {
                    var currentRecord = _records[index];
                    if (currentRecord.id == id) {
                        self.dataEntrySection.recordSelected(currentRecord);
                        break;
                    }
                }
            }
        }
        function recordSelectedEditSchema(id) {
            if (self.schemaEntrySection) {
                for (var index in _records) {
                    var currentRecord = _records[index];
                    if (currentRecord.id == id) {
                        self.schemaEntrySection.recordSelected(currentRecord);
                        break;
                    }
                }
            }
        }

        function addRecord() {
            if (self.schemaEntrySection) {
                self.schemaEntrySection.recordSelected({
                    name: '',
                    systemName: '',
                    xml: '<example id="newRecord"></example>',
                    xmlSchema: '<xs:schema attributeFormDefault="unqualified" elementFormDefault="qualified" xmlns:xs="http://www.w3.org/2001/XMLSchema">\n<xs:element name="example">\n<xs:complexType>\n<xs:simpleContent>\n<xs:extension base="xs:string">\n<xs:attribute type="xs:string" name="id"/>\n</xs:extension>\n</xs:simpleContent>\n</xs:complexType>\n</xs:element>\n</xs:schema>',
                    jsonSchema: '{\n\t  "definitions": {},\n\t"required": [\n\t"example"\n\t\t], \n\t"properties": {\n\t"example": {\n\t\t"id": "/properties/example",\n\t\t\t"required": [\n\t\t\t"@id"\n\t\t\t\t],\n\t\t\t"properties": {\n\t\t\t"@id": {\n\t\t\t\t"type": "string",\n\t\t\t\t\t"title": "id"\n\t\t\t\t\t} \n\t\t\t\t},\n\t\t\t"type": "object"\n\t\t\t}\n\t\t},\n\t"type": "object"\n\t}'
                });
            }
        }
        function removeRecord(id) {
            configDataServices.remove(id, removeResults);
        }
        function removeResults(results) {
            if (results.successful) {
                self.messagingSection.success(['Record has been removed successfully']);
                refresh(true);
            } else {
                self.messagingSection.error(results.errors, true);
            }
        }

        self.init = init;
        self.refresh = refresh;
        self.recordSelectedEditData = recordSelectedEditData;
        self.recordSelectedEditSchema = recordSelectedEditSchema;
        self.addRecord = addRecord;
        self.removeRecord = removeRecord;
        return self;
    }
};