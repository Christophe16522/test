function hideModalBackground(backgroundid) {
    var backgroundElem = $j('#' + backgroundid);
    if (backgroundElem != null) {
        backgroundElem.hide();
        if (resizeBackgroundHandler != null) {
            $j(window).off('resize', resizeBackgroundHandler);
            resizeBackgroundHandler = null;
        }
    }
}
var resizeMaxedWindowHandler = new Object();
var keyPressHandler = null;

var resizeBackgroundHandler = null;
var keepDialogAccessibleHandler = new Object();

function getMSIEMajorVersion(browser) {
	if (!browser) {
		browser = $j.browser;
	}

	return browser.version.substr(0, browser.version.indexOf('.'));
}

function hideModalPopup(popupid, backgroundid) {
	if ($j.browser.msie && getMSIEMajorVersion() < 7) {
        $j(window).off('scroll');
    }

    if (keyPressHandler != null) {
        $j(window).off('keydown', keyPressHandler);
    }
    if (resizeMaxedWindowHandler[popupid] != null) {
        $j(window).off('resize', resizeMaxedWindowHandler[popupid]);
    }

    var popupObj = $j('#' + popupid);
    if (popupObj != null) {
        popupObj.hide();
    }
    if (keepDialogAccessibleHandler[popupid] != null) {
        $j(window).off('resize', keepDialogAccessibleHandler[popupid]);
        keepDialogAccessibleHandler[popupid] = null;
    }
    // Hide background
    hideModalBackground(backgroundid);

    // Show hidden drop down for IE6
    if ($j.browser.msie && getMSIEMajorVersion() < 7) {
        var hiddenSelects = $j('select[popuphide="true"]');
        hiddenSelects.removeAttr('popuphide');
        hiddenSelects.show();
    }
}

function showModalBackground(backgroundid) {
    var backgroundElem = $j('#' + backgroundid);
    if (backgroundElem != null) {
        backgroundElem.width(GetWinWidth());
        backgroundElem.height(GetWinHeight());
        if ($j.browser.msie && getMSIEMajorVersion() < 7) {
            backgroundElem.css('position', 'absolute');
        }
        else {
            backgroundElem.css('position', 'fixed');
        }
        backgroundElem.css('overflow', 'hidden');
        backgroundElem.css('top', '0px');
        backgroundElem.css('left', '0px');

        if (resizeBackgroundHandler != null) {
            $j(window).off('resize', resizeBackgroundHandler);
            resizeBackgroundHandler = null;
        }
        resizeBackgroundHandler = function () { backgroundElem.width(GetWinWidth()); backgroundElem.height(GetWinHeight()); };

        $j(window).on('resize', resizeBackgroundHandler);
        backgroundElem.show();
    }
}

var imgsToLoad = 0;

function showModalMaximizedPopup(popupid, backgroundid, border, handler) {
    var popupObj = $j('#' + popupid);
    setModalPopupBoundaries(popupObj, backgroundid, border, handler);

    resizeMaxedWindowHandler[popupid] = function () { setModalPopupBoundaries(popupObj, backgroundid, border, handler); };
    keyPressHandler = function (e) {
        if (e.keyCode == 27) {
            hideModalPopup(popupObj.attr('id'), backgroundid);
        }
    };

    $j(window).on('resize', resizeMaxedWindowHandler[popupid]);
    $j(window).on('keydown', keyPressHandler);
}

function setModalPopupBoundaries(popupObj, backgroundid, border, handler) {
    popupObj.css('left', border);
    popupObj.css('top', border);
    popupObj.css('width', $j(window).width() - border * 2);
    popupObj.css('height', $j(window).height() - border * 2);
    popupObj.AlignCenter();
    showModalBackground(backgroundid);
    popupObj.show();
    if (handler != null) {
        handler();
    }
}

function showModalPopup(popupid, backgroundid) {
    var popupObj = $j('#' + popupid);

    // Hide drop down fields for ie6
    if ($j.browser.msie && getMSIEMajorVersion() < 7) {
        var visibleSelects = $j("select:visible");
        visibleSelects.attr('popuphide', 'true');
        visibleSelects.hide();
    }

    // Clear dialog initial size
    dialogInitWidth = -1;
    if (popupObj != null) {
        // Show background
        showModalBackground(backgroundid);
        // Default setting due to IE6 support
        popupObj.css('position', 'absolute');

        keepDialogAccessibleHandler[popupid] = function () { keepDialogAccesible(popupid); };

        popupObj.css('height', 'auto');
        popupObj.css('width', 'auto');

        $j(window).on('resize', keepDialogAccessibleHandler[popupid]);

        var images = popupObj.find("img");
        imgsToLoad = images.length;
        if (imgsToLoad > 0) {
            images.each(function () {
                $j(this).error(function () { imgsToLoad--; });
                waitForImageToLoad(popupid, this);
            });
        }
        else {
            showDialog(popupid);
        }

        if ($j.browser.msie && getMSIEMajorVersion() < 7) {
            $j(window).off('scroll');
            $j(window).scroll(function () { popupObj.AlignCenter(); });
        }
    }
}

function waitForImageToLoad(popupid, img) {
    if ((imgsToLoad <= 0) || isImageLoaded(img) || isInputTypeImageLoaded(img)) {
        imgsToLoad--;
        if (imgsToLoad <= 0) {
            showDialog(popupid);
        }
    }
    else {
        setTimeout(function () { waitForImageToLoad(popupid, img); }, 20);
    }
}

function isImageLoaded(img) {
    if (!img.complete) {
        return false;
    }
    if (typeof img.naturalWidth != 'undefined' && img.naturalWidth == 0) {
        return false;
    }
    return true;
}

function isInputTypeImageLoaded(img) {
    if ((typeof img.size != 'undefined') && (img.size != 0) && (img.tagName.toLowerCase() == "input")) {
        return true;
    }
    return false;
}

function showDialog(popupid) {
    var popupObj = $j('#' + popupid);
    popupObj.AlignCenter();
    popupObj.show();
    keepDialogAccesible(popupid);
}

var dialogInitWidth = -1;
var dialogInitHeight = -1;
var scrollableInitWidth = -1;
var scrollableInitHeight = -1;
var resizableDialog = false;

function keepDialogAccesible(popupid) {
    var popupObj = $j('#' + popupid);
    if (popupObj.is(":visible")) {

        var scrollable = popupObj.find('.DialogScrollableContent');
        var isChanged = false;
        var winWidth = $j(window).width();
        var winHeight = $j(window).height();

        if (scrollable.length > 0) {

            if (dialogInitWidth == -1) {
                dialogInitWidth = popupObj.width();
                dialogInitHeight = popupObj.height();
                scrollableInitWidth = scrollable.width();
                scrollableInitHeight = scrollable.height();
            }

            scrollable.css('overflow', '');
            popupObj.css('height', dialogInitHeight + 'px');
            popupObj.css('width', dialogInitWidth + 'px');

            if (resizableDialog) {
                scrollable.css('height', '');
                scrollable.css('height', scrollable.height() + 'px');
            }
            else {
                scrollable.css('height', scrollableInitHeight + 'px');
            }
            scrollable.css('width', scrollableInitWidth + 'px');

            var dialogWidth = popupObj.width();
            var dialogHeight = popupObj.height();
            var scrollableWidth = scrollable.width();
            var scrollableHeight = scrollable.height();
            var poswi = winWidth - (dialogWidth - scrollableWidth) - 20;
            var poshei = winHeight - (dialogHeight - scrollableHeight) - 20;

            if (scrollableHeight < dialogHeight) {
                scrollable.css('overflow', 'auto');                
            }

            if (scrollableWidth < dialogWidth) {
                scrollable.css('overflow', 'auto');                
            }

            if (winHeight < dialogHeight) {
                scrollable.height(poshei + 'px');
                scrollable.css('overflow', 'auto');
                popupObj.height(winHeight - 20 + 'px');
                isChanged = true;
            }

            if (winWidth < dialogWidth) {
                scrollable.width(poswi + 'px');
                scrollable.css('overflow', 'auto');
                popupObj.width(winWidth - 20 + 'px');
                isChanged = true;
            }
        }
        else {
            var dialogWidth = popupObj.width();
            var dialogHeight = popupObj.height();

            if (popupObj.css('overflow') == 'auto') {
                var popObjBasic = popupObj.get(0);
                popupObj.width(popObjBasic.scrollWidth + 'px');
                popupObj.height(popObjBasic.scrollHeight + 'px');
                popupObj.css('overflow', "");
            }

            if (winHeight < dialogHeight) {
                popupObj.height(winHeight - 20 + 'px');
                popupObj.css('overflow', 'auto');
                isChanged = true;
            }
            if (winWidth < dialogWidth) {
                popupObj.width($winWidth - 20 + 'px');
                popupObj.css('overflow', 'auto');
                isChanged = true;
            }
        }
        if (isChanged) {
            popupObj.AlignCenter();
        }
    }
}

function RegisterClickHandler(targetcontrolid, fn) {
    $j(document).ready(function () {
        var target = $j('#' + targetcontrolid);
        if (target != null) {
            target.click(fn);
        }
    });
}

function GetWinHeight() {
    var docHeight = $j(document).height();
    var winHeight = $j(window).height();
    if ((docHeight > winHeight) && (($j.browser.msie && getMSIEMajorVersion() < 7))) {
        return docHeight;
    }
    else {
        return winHeight;
    }
}

function GetWinWidth() {
    var docWidth = $j(document).width();
    var winWidth = $j(window).width();
    if ((docWidth > winWidth) && (($j.browser.msie && getMSIEMajorVersion() < 7))) {
        return docWidth;
    }
    else {
        return winWidth;
    }
}

/* Fixes position for all browsers */

(function (F) {
    F.fn.AlignCenter = function (B) {
        var C = { ignorechildren: true, showPopup: true }; var D = F.extend({}, C, B); var E = F(this); E.css({ zIndex: 20200 }); if (D.showPopup) { E.show(); } F(document).ready(function () { PosElem(); }); F(window).resize(function () { PosElem(); }); function PosElem() {
            var a = 0; var b = 0; if (D.ignorechildren) { a = E.height(); b = E.width(); } else { var c = E.children(); for (var i = 0; i < c.length; i++) { if (c[i].style.display != 'none') { a = c[i].clientHeight; b = c[i].clientWidth; } } } var d = E.css("margin"); var e = E.css("padding"); if (d != null) { d = d.replace(/auto/gi, '0'); d = d.replace(/px/gi, ''); d = d.replace(/pt/gi, ''); } var f = ""; if (d != "" && d != null) { var g = e.split(' '); if (g.length == 1) { var h = parseInt(g[0]); f = new Array(h, h, h, h); } else if (g.length == 2) { var j = parseInt(g[0]); var k = parseInt(g[1]); f = new Array(j, k, j, k); } else if (g.length == 3) { var l = parseInt(g[0]); var m = parseInt(g[1]); var n = parseInt(g[2]); f = new Array(l, m, n, m); } else if (g.length == 4) { var l = parseInt(g[0]); var m = parseInt(g[1]); var o = parseInt(g[2]); var p = parseInt(g[3]); f = new Array(l, m, n, p); } } var k = 0; var j = 0; if (f != "NaN") { if (f.length > 0) { k = f[1] + f[3]; j = f[0] + f[2]; } } if (e != null) { e = e.replace(/auto/gi, '0'); e = e.replace(/px/gi, ''); e = e.replace(/pt/gi, ''); } var q = ""; if (e != "" && e != null) {
                var r = e.split(' '); if (r.length == 1) { var s = parseInt(r[0]); q = new Array(s, s, s, s); } else if (r.length == 2) {
                    var t = parseInt(r[0]); var u = parseInt(r[1]); q = new Array(t, u, t, u);
                } else if (r.length == 3) { var v = parseInt(r[0]); var w = parseInt(r[1]); var x = parseInt(r[2]); q = new Array(v, w, x, w); } else if (r.length == 4) { var v = parseInt(r[0]); var w = parseInt(r[1]); var x = parseInt(r[2]); var y = parseInt(r[3]); q = new Array(v, w, x, y); }
            } var u = 0; var t = 0; if (q != "NaN") { if (q.length > 0) { u = q[1] + q[3]; t = q[0] + q[2]; } } if (j == "NaN" || isNaN(j)) { j = 0; } if (t == "NaN" || isNaN(t)) { t = 0; } var z = F(window).height(); var A = F(window).width(); var rt = ((z - (a + j + t)) / 2); var rl = ((A - (b + k + u)) / 2); if (F.browser.msie &&  /* CMS */ getMSIEMajorVersion(F.browser) /* CMS end */ < 7) {
                E.css("position", "absolute"); rt = rt + F(document).scrollTop(); var rl = rl + F(document).scrollLeft();
            } else { E.css("position", "fixed"); } E.css("height", a + "px"); E.css("width", b + "px"); E.css("top", rt + "px"); E.css("left", rl + "px");
        }
    };
})(jQuery);
