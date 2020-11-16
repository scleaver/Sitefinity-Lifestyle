(function ($) {
    $(document).ready(function () {

        var contentFilter = $('[data-sf-role="contentFilter"]');

        for (var i = 0; i < contentFilter.length; i++) {
            var cfCont = $(contentFilter[i]);
            var tempPrefix = $('[data-sf-role="cfPrefix"]', cfCont).first().val();
            var controlServerData = {
                prefix: tempPrefix ? tempPrefix : "cf",
                listPageUrl: $('[data-sf-role="cfListPageUrl"]', cfCont).first().val(),
                indexCatalogue: $('[data-sf-role="cfIndexCatalogue"]', cfCont).first().val(),
                language: $('[data-sf-role="cfLanguage"]', cfCont).first().val(),
                cfContainerId: cfCont.attr('id'),
                searchTextBox: $('[data-sf-role="cfSearchTextBox"]', cfCont).first(),
                resetButton: $('[data-sf-role="cfReset"]', cfCont).first(),
                filterButton: $('[data-sf-role="cfButton"]', cfCont).first(),
                sortBySelect: $('[data-sf-role="cfSortBySelect"]', cfCont).first(),
                rangeSelect: $('[data-sf-role="cfDateRangeSelect"]', cfCont).first(),
                categoriesList: $('[data-sf-role="cfCategoriesList"]', cfCont).first(),
                tagsList: $('[data-sf-role="cfTagsList"]', cfCont).first()
            };
            contentFilterWidget(controlServerData);
        }

        function contentFilterWidget(serverData) {

            serverData.filterButton.click(navigateToResults);
            serverData.resetButton.click(resetFilter);

            /* Initialization */
            if (serverData.cfContainerId) {

                var params = $.parseParams(window.location)[serverData.prefix];

                if (params) {
                    if (params.hasOwnProperty("squery") && serverData.searchTextBox) {
                        serverData.searchTextBox.val(params.squery);
                    }
                    if (params.hasOwnProperty("sortby") && serverData.sortBySelect) {
                        serverData.sortBySelect.val(params.sortby);
                    }
                    if (params.hasOwnProperty("categories") && serverData.categoriesList.length > 0) {
                        var cats = params.categories.split(',');
                        $('[type="checkbox"]', serverData.categoriesList).prop("checked", function () {
                            return $.inArray(this.value, cats) >= 0;
                        });
                    }
                    if (params.hasOwnProperty("tags") && serverData.tagsList.length > 0) {
                        var tags = params.tags.split(',');
                        $('[type="checkbox"]', serverData.tagsList).prop("checked", function () {
                            return $.inArray(this.value, tags) >= 0;
                        });
                    }
                    if (params.hasOwnProperty("range") && serverData.rangeSelect) {
                        serverData.rangeSelect.val(params.range);
                    }
                }
            }


            /* Helper methods */

            function navigateToResults(e) {
                if (!e)
                    e = window.event;

                if (e.stopPropagation) {
                    e.stopPropagation();
                }
                else {
                    e.cancelBubble = true;
                }
                if (e.preventDefault) {
                    e.preventDefault();
                }
                else {
                    e.returnValue = false;
                }

                var location = getLocation();

                if (location) {
                    window.location = location;
                }
            }

            function resetFilter(e) {
                if (!e)
                    e = window.event;

                if (e.stopPropagation) {
                    e.stopPropagation();
                }
                else {
                    e.cancelBubble = true;
                }
                if (e.preventDefault) {
                    e.preventDefault();
                }
                else {
                    e.returnValue = false;
                }

                window.location = serverData.listPageUrl;
            }

            function getLocation() {
                var separator = (serverData.listPageUrl.indexOf("?") === -1) ? "?" : "&";

                filterObj = {};

                if (serverData.searchTextBox.length > 0 && serverData.searchTextBox.val()) {
                    filterObj.sindex = serverData.indexCatalogue;
                    filterObj.squery = serverData.searchTextBox.val().trim().toLowerCase();
                }

                if (serverData.sortBySelect.length > 0 && serverData.sortBySelect.val()) {
                    filterObj.sortby = serverData.sortBySelect.val().trim();
                }

                if (serverData.rangeSelect.length > 0 && serverData.rangeSelect.val()) {
                    filterObj.range = serverData.rangeSelect.val().trim();
                }

                if (serverData.categoriesList.length > 0) {
                    var cats = [];
                    $('input:checkbox:checked', serverData.categoriesList).each(function () {
                        cats.push($(this).val());
                    });
                    if (Array.isArray(cats) && cats.length) {
                        filterObj.categories = cats.join(',');
                    }
                }

                if (serverData.tagsList.length > 0) {
                    var tags = [];
                    $('input:checkbox:checked', serverData.tagsList).each(function () {
                        tags.push($(this).val());
                    });
                    if (Array.isArray(tags) && tags.length) {
                        filterObj.tags = tags.join(',');
                    }
                }

                if (Object.keys(filterObj).length > 0) {
                    var url = serverData.listPageUrl + separator + $.encodeURL({ [serverData.prefix]: filterObj });

                    return url;
                }
            }
        }
    });


    // Add an URL parser to JQuery that returns an object
    // This function is meant to be used with an URL like the window.location
    // Use: $.parseParams('http://mysite.com/?var=string') or $.parseParams() to parse the window.location
    // Simple variable:  ?var=abc                        returns {var: "abc"}
    // Simple object:    ?var.length=2&var.scope=123     returns {var: {length: "2", scope: "123"}}
    // Simple array:     ?var[]=0&var[]=9                returns {var: ["0", "9"]}
    // Array with index: ?var[0]=0&var[1]=9              returns {var: ["0", "9"]}
    // Nested objects:   ?my.var.is.here=5               returns {my: {var: {is: {here: "5"}}}}
    // All together:     ?var=a&my.var[]=b&my.cookie=no  returns {var: "a", my: {var: ["b"], cookie: "no"}}
    // You just cant have an object in an array, ?var[1].test=abc DOES NOT WORK
    var re = /([^&=]+)=?([^&]*)/g;
    var decode = function (str) {
        return decodeURIComponent(str.replace(/\+/g, ' '));
    };
    $.parseParams = function (query) {
        // recursive function to construct the result object
        function createElement(params, key, value) {
            key = key + '';
            // if the key is a property
            if (key.indexOf('.') !== -1) {
                // extract the first part with the name of the object
                var list = key.split('.');
                // the rest of the key
                var new_key = key.split(/\.(.+)?/)[1];
                // create the object if it doesnt exist
                if (!params[list[0]]) params[list[0]] = {};
                // if the key is not empty, create it in the object
                if (new_key !== '') {
                    createElement(params[list[0]], new_key, value);
                } else console.warn('parseParams :: empty property in key "' + key + '"');
            }
            else
                // if the key is an array    
                if (key.indexOf('[') !== -1) {
                    // extract the array name
                    var list = key.split('[');
                    key = list[0];
                    // extract the index of the array
                    list = list[1].split(']');
                    var index = list[0];
                    // if index is empty, just push the value at the end of the array
                    if (index == '') {
                        if (!params) params = {};
                        if (!params[key] || !$.isArray(params[key])) params[key] = [];
                        params[key].push(value);
                    } else
                    // add the value at the index (must be an integer)
                    {
                        if (!params) params = {};
                        if (!params[key] || !$.isArray(params[key])) params[key] = [];
                        params[key][parseInt(index)] = value;
                    }
                } else
                // just normal key
                {
                    if (!params) params = {};
                    params[key] = value;
                }
        }
        // be sure the query is a string
        query = query + '';
        if (query === '') query = window.location + '';
        var params = {}, e;
        if (query) {
            // remove # from end of query
            if (query.indexOf('#') !== -1) {
                query = query.substr(0, query.indexOf('#'));
            }

            // remove ? at the begining of the query
            if (query.indexOf('?') !== -1) {
                query = query.substr(query.indexOf('?') + 1, query.length);
            } else return {};
            // empty parameters
            if (query == '') return {};
            // execute a createElement on every key and value
            while (e = re.exec(query)) {
                var key = decode(e[1]);
                var value = decode(e[2]);
                createElement(params, key, value);
            }
        }
        return params;
    };

    // Encode an object to an url string
    // This function return the search part, begining with "?"
    // Use: $.encodeURL({var: "test", len: 1}) returns ?var=test&len=1
    $.encodeURL = function (object) {

        // recursive function to construct the result string
        function createString(element, nest) {
            if (element === null) return '';
            if ($.isArray(element)) {
                var count = 0,
                    url = '';
                for (var t = 0; t < element.length; t++) {
                    if (count > 0) url += '&';
                    url += nest + '[]=' + element[t];
                    count++;
                }
                return url;
            } else if (typeof element === 'object') {
                var count = 0,
                    url = '';
                for (var name in element) {
                    if (element.hasOwnProperty(name)) {
                        if (count > 0) url += '&';
                        url += createString(element[name], nest + '.' + name);
                        count++;
                    }
                }
                return url;
            } else {
                return nest + '=' + element;
            }
        }

        var url = "", count = 0;

        // execute a createString on every property of object
        for (var name in object) {
            if (object.hasOwnProperty(name)) {
                if (count > 0) url += '&';
                url += createString(object[name], name);
                count++;
            }
        }

        return url;
    };
}(jQuery));