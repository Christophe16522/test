/**
 * Class image selector module
 */
define(["CMS/EventHub"], function (hub) {
    "use strict";

    /**
     * Class image selector
     * @constructor
     * @param {Object} data - Data passed from the server
     */
    var ClassImageSelector = function (data) {
        var itemSelected = function () {
            var guid = $j(".FlatSelectedItem img").data("metafile-guid");

            if (guid) {
                hub.publish(data.eventId, { metafileguid: guid });
            }

            CloseDialog();
        };

        // Publish guid of the selected metafile on submit
        $j("#" + data.okButtonId).click(itemSelected);

        $j(data.itemsCSSSelector).each(function () {
            $j(this).bind('dblclick', itemSelected);
        });
    };

    return ClassImageSelector;
});