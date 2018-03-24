(function () {

    var $uiHeader;
    var $shadowSet;

    function toggleShadow(eventObject) {
        ensureUiHeader();

        if ($uiHeader.length > 0) {
            toggleShadowInternal(this);
        }
    }
    function ensureUiHeader() {
        if ($uiHeader == null || $uiHeader.length <= 0) {
            // Try to find explicit shadow holder panel
            $uiHeader = jQuery('div.shadow-holder');
            checkEmptyness();
            // Try to find other header panels
            if ($uiHeader.length <= 0) {
                $uiHeader = jQuery('div.PreviewMenu');
            }
            if ($uiHeader.length <= 0) {
                $uiHeader = jQuery('div.header-container');
                checkEmptyness();
            }
            if ($uiHeader.length <= 0) {
                $uiHeader = jQuery('div#CMSHeaderDiv');
                checkEmptyness();
            }
            if ($uiHeader.length <= 0) {
                $uiHeader = jQuery('div#CKToolbar');
                checkEmptyness();
            }
            if ($uiHeader.length <= 0) {
                $uiHeader = jQuery('div.cms-edit-menu');
            }
        }
    }
    function checkEmptyness() {
        if (($uiHeader.children().length <= 0) || ($uiHeader.height() == 0)) {
            $uiHeader = [];
        }
    }
    function toggleShadowInternal(scrollElem) {
        if (jQuery(scrollElem).scrollTop() > 0) {
            $uiHeader.first().addClass('header-shadow');
            $shadowSet = true;
        }
        else {
            $uiHeader.first().removeClass('header-shadow');
            $shadowSet = false;
        }
    }

    var doc = jQuery(document);
    doc.ready(function () {
        doc.on('scroll', toggleShadow);
        jQuery('iframe.scroll-area').on('load', function () { jQuery(this).contents().on('scroll', toggleShadow) });
        jQuery('.scroll-area,.PageContent,.PreviewBody').on('scroll', toggleShadow);
        $uiHeader = [];

        if ($shadowSet == null) {
            $shadowSet = false;
        }

        // Reset shadow after async postback
        if ($shadowSet == true) {
            ensureUiHeader();
            if (($uiHeader != null) && ($uiHeader.length > 0)) {
                $uiHeader.first().addClass('header-shadow');
            }
        }
    });

}());