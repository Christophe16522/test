// MessagesPlaceHolder.js
var cmsLbls = cmsLbls || [];

function CMSHandleLabel(id, description, delay, close, options) {
    options = CMSMessageLoadOptions(options);
    var elm = $j('#' + id);
    window.cmsLbls[id] = elm;
    var lblOp = (options.lblsOpacity / 100.0);
    elm.css('opacity', (delay == 0) ? lblOp : '1');

    CMSRfrLblPos(elm, options);
    $j(window).resize(function () {
        CMSRfrLblPos(elm, options);
        if (options.useRelPlc) {
            CMSRfrRelPlcSize(elm);
        }
    });

    // Prepares empty div to adjust his height and move with unigrid up and down
    var plc = null;
    if (options.useRelPlc) {
        plc = $j('<div></div>').addClass('lblPlc').addClass(elm.attr('id'));
        elm.after(plc);
    }

    if (description !== '') {
        var desc = $j('<div class="alert-description">' + description + '</div>').hide();
        var lnk = $j('<a>' + options.lblDetails + '</a>').addClass('alert-link');
        elm.find("div").append($j('<span>&nbsp;(</span>')).append(lnk).append($j('<span>)</span>')).append(desc);
        lnk.click(function () {
            desc.slideToggle('slow', function () {
                lnk.text(desc.is(':visible') ? options.lblLess : options.lblDetails);

                // After clicking on "see more details" set plc(empty div) a new height to move with unigrid, elm(message)
                CMSRfrRelPlcSize(elm, plc);
            });

        });
    }
    if (delay > 0) {
        elm.mouseenter(function () { elm.stop(true, true).fadeIn('fast'); });
        elm.mouseleave(function () { elm.delay(delay).fadeOut('fast'); });
        elm.delay(delay).fadeOut('fast');
        if (description === '') {
            elm.click(function () { elm.hide(); });
        }
    }
    else {
        if (options.useRelPlc) {
            CMSRfrRelPlcSize(elm, plc);
        }

        if (close) {
            var closeElem = $j('<span></span>').addClass('alert-close');
            closeElem.append($j('<i></i>').addClass('close icon-modal-close').click(function () { elm.hide(); if (plc !== null) { plc.hide(); } }));
            closeElem.append($j('<span">Close</span>').addClass('sr-only'));

            elm.append(closeElem);
        }
        elm.hover(function () { elm.addClass("hover"); }, function () { elm.removeClass("hover"); });
    }
}

function CMSMessageLoadOptions(options) {
    options = options || {};
    if (typeof (options.wrpCtrlid) === "undefined") { options.wrpCtrlid = wrpCtrlid; }
    if (typeof (options.lblDetails) === "undefined") { options.lblDetails = lblDetails; }
    if (typeof (options.lblLess) === "undefined") { options.lblLess = lblLess; }
    if (typeof (options.posOffsetX) === "undefined") { options.posOffsetX = posOffsetX; }
    if (typeof (options.posOffsetY) === "undefined") { options.posOffsetY = posOffsetY; }
    if (typeof (options.lblsOpacity) === "undefined") { options.lblsOpacity = lblsOpacity; }
    if (typeof (options.useRelPlc) === "undefined") { options.useRelPlc = useRelPlc; }

    return options;
}

function CMSRfrRelPlcSize(elm, plc) {
    plc = plc || $j('.' + elm.attr('id'));
    if (plc) {
        // Adjust message's width without margin to the placeholder's width.
        elm.css('max-width', plc.outerWidth());

        // Adjust placeholder's height to message's width with margin.
        plc.css('height', elm.outerHeight(true));
    }
}

function CMSRfrLblPos(elm, options) {
    if (elm.is(':hidden') || elm.hasClass("hover")) { return; }
    var opacity = elm.css('opacity');
    elm.css('opacity', 0).css('position', 'static').css('top', 'auto');
    var offset = elm.offset();
    elm.css('position', 'fixed');
    var top = CMSGetPlcPos();

    //Wrapper object
    var ctlrwrpobj = null;
    if (options.wrpCtrlid !== '') {
        ctlrwrpobj = $j('#' + options.wrpCtrlid);
    }

    // Offset top
    var otop = offset.top;
    if (ctlrwrpobj !== null) {
        otop = ctlrwrpobj.offset().top;
    }

    // Ensure wrapper max width
    if (ctlrwrpobj !== null) {
        elm.css('max-width', ctlrwrpobj.width() - 100);
    }

    elm.prevAll(".alert").each(function () {
        var sib = $j(this);
        if (sib.offset().top >= top) {
            top += sib.outerHeight(true);
        }
    });

    top = (top > otop) ? top : otop;

    // Add Y offset if defined
    top = top + options.posOffsetY;

    elm.css('top', top).css('opacity', opacity);
    var isrtl = $j('body').hasClass('RTL');
    var xpos = isrtl ? 'right' : 'left';

    // Wrapper X-axis offset
    var wrapperOfsetX = offset.left;
    if ((ctlrwrpobj !== null)) {
        if (isrtl) {
            wrapperOfsetX = $j(window).width() - (ctlrwrpobj.offset().left + ctlrwrpobj.outerWidth());
        }
        else {
            wrapperOfsetX = ctlrwrpobj.offset().left;
        }
    }
    elm.css(xpos, options.posOffsetX + wrapperOfsetX);
}

function CMSGetPlcPos() {
    var pos = 0;
    $j('.CMSFixPanel, #CMSHeaderPad, .PreviewMenu, .SplitToolbar.Vertical').each(function () { pos += $j(this).height(); });
    return pos;
}

function CMSRfrLblsPos() {
    if (window.cmsLbls !== null) {
        var options = CMSMessageLoadOptions(options);

        for (var key in window.cmsLbls) {
            CMSRfrLblPos(window.cmsLbls[key], options);
        }
    }
}

function CMSHndlLblAnchor(ctrlId, name, options) {
    options = CMSMessageLoadOptions(options);
    var ctrl = $j('#' + ctrlId);
    var parent = ctrl.parent();
    var top = CMSGetPlcPos();
    if (options.useRelPlc) {
        $j('.lblPlc').each(function () { top += $j(this).outerHeight(true); });
    }
    else {
        $j('.alert').each(function () { top += $j(this).outerHeight(true); });
    }
    $j(window).scrollTop(parent.offset().top - top);
    $j('.AnchorFocus').each(function () { $j(this).removeClass('AnchorFocus'); });
    parent.addClass('AnchorFocus');
    var inEl = null;
    var forId = ctrl.prop('for');
    if (typeof (forId) !== "undefined") {
        inEl = $j('#' + forId);
    }
    else {
        inEl = $j('input[name*="$' + name + '$"],textarea[name*="$' + name + '$"]');
    }

    if ((inEl !== null) && (inEl.length > 0)) {
        if (typeof (inEl.focus) === 'function') {
            inEl.focus();
        };

        var ckEditor = window.CKEDITOR;
        if ((typeof (ckEditor) !== 'undefined') && (ckEditor.instances !== null)) {
            var oEditor = ckEditor.instances[inEl.attr("id")];
            if (oEditor !== null) {
                oEditor.focus();
            }
        }
    }
}
