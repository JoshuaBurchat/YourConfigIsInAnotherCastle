

//This code is working with a lack of knowledge of what tools you have included in your project. It assumes that you have included the packages from YourConfigIsInAnotherCastle + react-jsonschema-form by mozilla.
//There is a dependency on Jquery, but I did not want to include any further dependencies such as a templating framework like handlebars, or a more robust framework like angular
//It is recommended that you modify the code below to your projects coding standards and use it has a guide.
(function () {

    //Setup the section ViewModels with the UI components and events
    function setupTagManagementSection(configDataService, $tagSelectionDropDown, $selectedTagsContainers, $newTagEntry) {
        function clearSelectableTags() {
            $tagSelectionDropDown.html('<option>Please select one</option>');
        }
        function addTagSelectableCall(key, text) {
            $tagSelectionDropDown.append('<option value="' + key + '">' + text + '</option>');
        }
        function addNewTag() {
            _section.addNewTag($(this).val());
            $(this).val('');
        }
        function clearSelectedTags() {
            $selectedTagsContainers.html('');
        }

        function addTagSelectedCall(key, text) {
            var $tagButton = $('<button class="btn btn-default"><span class="pull-right">&nbsp;&nbsp;<span class="glyphicon glyphicon-remove"></span></span>' + text + '</button>');
            $selectedTagsContainers.append($tagButton);
            $tagButton.data('tagid', key);
            $tagButton.click(function () {
                _section.unselectTag($(this).data('tagid'));
                $(this).remove();
            });
        }

        function bindEvents() {
            $tagSelectionDropDown.change(function () { _section.selectTag($(this).val()); });
            if ($newTagEntry) {
                $newTagEntry.blur(addNewTag);
                $newTagEntry.keyup(function (e) {
                    if (e.keyCode == 13) {
                        addNewTag.call($newTagEntry);
                    }
                });
            }
        }
        var _section = yourConfigIsInAnotherCastleViewModels.tagManager(configDataService, addTagSelectableCall, clearSelectableTags, addTagSelectedCall, clearSelectedTags);

        $(document).ready(function () {
            bindEvents();
        });
        return _section;
    }
    function setupSearchSection(configDataServices) {
        var _$tableBody = $('#configurationList');
        var _$nameContainsFilter = $('#nameContainsFilter');
        var _$systemNameFilter = $('#systemNameFilter');
        var _tagManagement = setupTagManagementSection(configDataService, $('#tagSelection'), $('#selectedTagsContainers'), null);

        //debating if I have a set funtion for these in the view model
        var filterCalls = {
            get: function () {
                return {
                    nameCotains: _$nameContainsFilter.val(),
                    systemName: _$systemNameFilter.val(),
                    tagIds: _tagManagement.getSelectedTagIds()
                };
            }, set: function (model) {
                _$nameContainsFilter.val(nameCotains);
                _$systemNameFilter.val(systemName);
                _tagManagement.setSelectedTags(tagIds);
            }
        };
        function buildTagLinkList(tags) {
            var results = '';
            for (var index in tags) {
                var currentTag = tags[index];
                results += '<a href="#" data-searchresults-tagid="' + currentTag.id + '">' + currentTag.value + '</a>, ';
            }
            return results;
        }
        function buildColumnHtml(value) {
            return "<td>" + value + "</td>";
        }
        function buildRowElementHtml(record) {
            return '<tr><td><button data-edit="' + record.id + '" class="btn"><span class="glyphicon glyphicon-pencil"></span></button></td>' +
                '<td><button data-edit-schema="' + record.id + '" class="btn"><span class="glyphicon glyphicon-wrench"></span></button></td>' +
                buildColumnHtml(record.systemName) +
                buildColumnHtml(record.id) +
                buildColumnHtml(record.name) +
                buildColumnHtml(buildTagLinkList(record.tags)) +
                '<td><button data-remove="' + record.id + '" class="btn"><span class="glyphicon glyphicon-remove"></span></button></td>' +
                '</tr>';
        }
        function bindGridEvents() {
            _$tableBody.find('[data-edit]').click(function () {
                var id = $(this).data('edit');
                _section.recordSelectedEditData(id);
            });
            _$tableBody.find('[data-edit-schema]').click(function () {
                var id = $(this).data('edit-schema');
                _section.recordSelectedEditSchema(id);
            });
            _$tableBody.find('[data-add]').click(function () {
                _section.addRecord();
            });
            _$tableBody.find('[data-searchresults-tagid]').click(function () {
                var tagId = $(this).data('searchresults-tagid');
                _tagManagement.setSelectedTags([tagId]);
                _section.refresh();
            });
            _$tableBody.find('[data-remove]').click(function () {
                var id = $(this).data('remove');
                _section.removeRecord(id);
            });
        }

        function onRecordsLoaded(records) {
            _tagManagement.loadTags();
            var systemNames = [];
            var previousSystemNameValue = _$systemNameFilter.val();
            _$systemNameFilter.html('<option value="">-All-</option>');
            _$tableBody.html('');
            for (var index in records) {
                var currentRecord = records[index];
                var $element = $(buildRowElementHtml(currentRecord));
                _$tableBody.append($element);
                if (currentRecord.systemName)
                    _$systemNameFilter.append($('<option value="' + currentRecord.systemName + '">' + currentRecord.systemName + "</option>"));
            }
            _$systemNameFilter.val(previousSystemNameValue);
            _$tableBody.append('<tr><td><button data-add="" class="btn"><span class="glyphicon glyphicon-plus"></span></button></td></tr>');
            bindGridEvents();
        }

        function bindEvents() {
            var refreshTimeout = null;

            function refresh() {
                if (refreshTimeout) clearTimeout(refreshTimeout);
                refreshTimeout = setTimeout(_section.refresh(), 100);
            }

            _$nameContainsFilter.keyup(refresh);
            _$systemNameFilter.change(refresh);
            _tagManagement.onChange = refresh;

        }

        var _section = yourConfigIsInAnotherCastleViewModels.searchSection(
            configDataServices,
            onRecordsLoaded,
            filterCalls
        );



        $(document).ready(function () {
            _tagManagement.init();
            bindEvents();
        });

        return _section;
    }
    function setupSchemaEntrySection(configDataServices) {
        var _$dialog = $('#schemaEntryDialog');
        var _$title = $('#schemaEntryTitle');
        var _$name = $('#schemaEntryName');
        var _$systemName = $('#schemaEntrySystemName');
        var _$xmlData = $('#schemaEntryXmlData');
        var _$xmlSchema = $('#schemaEntryXmlSchema');
        var _$jsonSchema = $('#schemaEntryJsonSchema');
        var _$save = $('#schemaEntrySave');

        var modelCalls = {
            get: function () {
                return {
                    name: _$name.val(),
                    systemName: _$systemName.val(),
                    xml: _$xmlData.val(),
                    xmlSchema: _$xmlSchema.val(),
                    jsonSchema: _$jsonSchema.val()
                };
            },
            set: function (record) {
                _$name.val(record.name);
                _$systemName.val(record.systemName);
                _$xmlData.val(record.xml);
                _$xmlSchema.val(record.xmlSchema);
                _$jsonSchema.val(record.jsonSchema);
            }
        }

        var displayCalls = {
            show: function () {
                _$dialog.modal('show');
            },
            hide: function () {
                _$dialog.modal('hide');
            }
        };

        function bindEvents() {
            _$save.click(_section.save);
        }

        var _section = yourConfigIsInAnotherCastleViewModels.schemaEntry(
            configDataServices,
            setupTagManagementSection(configDataService, $('#schemaEntryTagSelection'), $('#schemaEntrySelectedTagsContainers'), $('#schemaEntryNewTag')),
            displayCalls,
            modelCalls
        );

        $(document).ready(function () {
            bindEvents();
        });

        return _section;
    }
    function setupDataEntrySection(configDataServices) {
        var _jsonSchemaForm = yourConfigIsInAnotherCastleJsonSchemaForm(document.getElementById("entryForm"));

        var _$dialog = $('#entryDialog');
        var _$dialogTitle = $('#entryDialogTitle');
        var modelCalls = {
            getJson: function () {
                return JSON.stringify(_jsonSchemaForm.getFormData());
            },
            set: function (record) {
                _jsonSchemaForm.changeSelectedDetails(
                    JSON.parse(record.json),
                    JSON.parse(record.jsonSchema)
                );

                _$dialogTitle.html(record.name);
            }
        }
        var displayCalls = {
            show: function () {
                _$dialog.modal('show');
            },
            hide: function () {
                _$dialog.modal('hide');
            }
        };

        function bindEvents() {
            _jsonSchemaForm.addListener(function (data) {
                _section.save();
            });
        }
        var _section = yourConfigIsInAnotherCastleViewModels.dataEntry(
            configDataServices,
            displayCalls,
            modelCalls
        );

        $(document).ready(function () {
            bindEvents();
        });

        return _section;
    }
    function setupMessageSection(configDataService) {
        var _$messageDialog = $('#messageDialog');
        var _$messageDialogTitle = $('#messageDialogTitle');
        var _$messageDialogBody = $('#messageDialogBody');
        var _$messageHeader = $('#messageHeader');
        var _$messageDetailList = $('#messageDetailList');
        var _alertClassValues = {
            success: 'alert-success',
            info: 'alert-info',
            warning: 'alert-warning',
            danger: 'alert-danger'
        };

        function buildListItems(messages) {
            _$messageDetailList.html('');
            for (var index in messages) {
                var currentMessage = messages[index];
                _$messageDetailList.append('<li>' + currentMessage + '</li>');
            }
        }

        function showDialog(title, header, messages, alertClass) {
            _$messageDialogTitle.html(title);
            _$messageHeader.html(header);
            buildListItems(messages);
            for (var index in _alertClassValues) {
                var currentAlertClassValue = _alertClassValues[index];
                _$messageDialogBody.removeClass(currentAlertClassValue);
            }
            _$messageDialogBody.addClass(alertClass);
            _$messageDialog.modal('show');
        }

        function fatalError(error) {
            //Valid aborts
            if (error.sourceType == 'jquery-ajax' && error.error.statusText == 'abort') {
                return;
            }
            showDialog(
                'Fatal Error',
                'A fatal error has occured please send the following messages to your administrators',
                [JSON.stringify(error)],
                _alertClassValues.danger
            );
        }
        function error(messages, format) {
            if (format) messages = formatErrorMessages(messages);
            showDialog(
                'Error',
                'The following errors have occurred, please validate your inputs and retry',
                messages,
                _alertClassValues.danger
            );
        }

        function formatErrorMessages(errors) {
            var messages = [];
            for (var index in errors) {
                var currentError = errors[index];
                messages.push(currentError.message + ', error code : ' + currentError.code);
            }
            return messages;
        }
        function success(messages) {
            showDialog(
                'Successful',
                '',
                messages,
                _alertClassValues.success
            );
        }
        function clear() {
            _$messageDialogTitle.html('');
            _$messageHeader.html('');
            _$messageDetailList.html('');
            _$messageDialog.modal('hide');
        }


        function init() {
            clear();
            configDataService.addErrorListener(fatalError);
        }
        return {
            init: init,
            clear: clear,
            success: success,
            error: error,
            fatalError: fatalError
        }
    }


    var configDataService = yourConfigIsInAnotherCastleDataService(camelCaseObjectFormatter());

    var searchSection = setupSearchSection(configDataService);
    var schemaEntrySection = setupSchemaEntrySection(configDataService);
    var dataEntrySection = setupDataEntrySection(configDataService);
    var messagingSection = setupMessageSection(configDataService);

    $(document).ready(function () {
        messagingSection.init();
        searchSection.init(messagingSection, dataEntrySection, schemaEntrySection);
        schemaEntrySection.init(messagingSection, searchSection);
        dataEntrySection.init(messagingSection, searchSection);

    });


    ///https://stackoverflow.com/questions/6637341/use-tab-to-indent-in-textarea
    $(document).delegate('textarea', 'keydown', function (e) {
        var keyCode = e.keyCode || e.which;

        if (keyCode == 9) {
            e.preventDefault();
            var start = $(this).get(0).selectionStart;
            var end = $(this).get(0).selectionEnd;

            // set textarea value to: text before caret + tab + text after caret
            $(this).val($(this).val().substring(0, start)
                + "\t"
                + $(this).val().substring(end));

            // put caret at right position again
            $(this).get(0).selectionStart =
                $(this).get(0).selectionEnd = start + 1;
        }
    });
}());



