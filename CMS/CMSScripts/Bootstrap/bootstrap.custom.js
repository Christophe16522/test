jQuery(function () {
    (function (jQuery) {
        // When clicking the dropdown menu, it will NOT disappear, unless a link was clicked.
        jQuery(".dropdown-menu").click(function (event) {
            if (!jQuery(event.target).closest('a').length) {
                event.stopPropagation();
            }
        });

        // Prevent hiding anchor dropup menu after clicking a link inside of dropup.
        jQuery(".anchor-dropup .dropdown-menu a").click(function (event) {
            event.stopPropagation();
        });

        // Disable hiding anchor dropup menu when clicking outside of it.
        var allowClose = false;
        jQuery('.anchor-dropup').on({
            "shown.bs.dropdown": function () { allowClose = false; },
            "click": function () { allowClose = true; },
            "hide.bs.dropdown": function () { if (!allowClose) return false; }
        });

        // This prevents IE9 bug when caching styles causes bad resolving media queries when opening iframe with lesser width.
        jQuery('iframe').load(function () {
            if (jQuery('body').hasClass('IE9')) {
                var iframeContent = jQuery(this).contents();
                iframeContent.find('link[rel="stylesheet"]').each(function () {

                    // Add a 'nocache' random num query string to stylesheet's url for disabling the caching.
                    var cssURL = jQuery(this).attr('href');
                    cssURL += (cssURL.indexOf('?') != -1) ? '&' : '?';
                    cssURL += 'nocache=' + (Math.random());
                    jQuery(this).attr('href', cssURL);
                });
            }
        });

        // On/off switcher
        jQuery('.has-switch').click(function () {
            jQuery(this).find('.switch').toggle();
            var $switch = jQuery(this).find('input[type=checkbox]');
            if ($switch.prop('checked')) {
                $switch.prop('checked', false);
            } else {
                $switch.prop('checked', true);
            }
        });

        // Fix position of localization flag icon for scrolling textareas   
        jQuery('.cms-input-group').each(function () {
            jQuery(this).on('checkScrollbar', function () {
                var $textarea = jQuery(this).find('textarea');
                if ($textarea.length > 0) {
                    if ($textarea[0].clientHeight < $textarea[0].scrollHeight) {
                        jQuery(this).addClass("has-scroller");
                    } else {
                        jQuery(this).removeClass("has-scroller");
                    }
                }
            });
        });
        jQuery('textarea').bind('keyup mouseup mouseout', function () {
            jQuery(this).parent('.input-localized').trigger('checkScrollbar');
        });
        jQuery('.input-localized').each(function () {
            jQuery(this).trigger('checkScrollbar');
        });
    }((function ($) {

        var $wrapper = $('.cms-bootstrap'),
            scopedjQuery = function (selector) {
                return $(selector, $wrapper);
            };

        // Copy all jQuery properties into
        // scopedjQuery so they can be used later
        for (var k in $) {
            if ($.hasOwnProperty(k)) {
                scopedjQuery[k] = $[k];
            }
        }

        return scopedjQuery;

    }(window.jQuery.noConflict()))));
});