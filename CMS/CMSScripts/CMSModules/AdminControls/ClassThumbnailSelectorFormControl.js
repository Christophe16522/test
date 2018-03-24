/**
 * Class image selector opener module
 */
define(["CMS/EventHub", "CMS/UrlHelper"], function (hub, urlHelper) {
    "use strict";

    /**
     * Class image selector opener
     * @constructor
     * @param {Object} data - Data passed from the server
     *      @param {string} hiddenInputId - Id of hidden input used for storing image guid
     *      @param {string} previewImageId - Id of preview image element
     *      @param {string} previewImageAnchorId - Id of anchor element decorating the preview image
     *      @param {string} selectButtonId - Id of select button
     *      @param {string} classId - Id of CMS class of the edited object
     *      @param {string} modalUrl - Url of modal dialog for selecting image
     */
    var ClassImageSelectorOpener = function (data) {
        var eventId = "CMS.ClassImageSelector.ImageSelected." + Math.random().toString(),
            $hdnMetafileGuid = $j("#" + data.hiddenInputId),
            $preview = $j("#" + data.previewImageId),
            $button = $j("#" + data.selectButtonId);

        // Subscribe to event fired in the image selector
        hub.subscribe(eventId, function (subData) {
            var imageUrl = window.applicationUrl + "getmetafile/" + subData.metafileguid + "/";
            imageUrl += urlHelper.buildQueryString({ maxsidesize: 256 });

            $hdnMetafileGuid.val(subData.metafileguid);
            $preview.attr("src", imageUrl);
        });

        var openOnClick = function () {
            var queryString = urlHelper.buildQueryString(
                {
                    metafileguid: $hdnMetafileGuid.val(),
                    eventid: eventId,
                    classid: data.classId
                });

            modalDialog(data.modalUrl + queryString, "ClassImageSelectorWindow", 1650, 800);
            return false;
        };

        // Prevent default action after clicking on link and open selector
        $j("#" + data.previewImageAnchorId).click(function (e) {
            e.preventDefault();
            openOnClick();
        });

        // Open image selector on click on button
        $button.click(openOnClick);
    };

    return ClassImageSelectorOpener;
});