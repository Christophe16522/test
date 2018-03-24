// CMSPanel.js

if(window.cmsfixpanelheight == null)
{
    var cmsfixpanelheight = 0;
}

function CMSFixPosition(id, plcId) {
    var oldHeight = 0;
    var plc = $j('#plc_' + id);
    if (plc.length > 0) {
        oldHeight = plc.outerHeight(true);
    }
    var elm = $j('#' + id);
    cmsfixpanelheight -= oldHeight;
    var top = cmsfixpanelheight;
    cmsfixpanelheight += elm.outerHeight(true);
    if (plc.length > 0) {
        plc.remove();
    }
    var plc = $j('<div></div>').attr('id', 'plc_' + id).addClass('CMSFixPanel').css('height', elm.outerHeight(true));
    if (plcId != '') {
        $j('#' + plcId).prepend(plc);
    }
    else {
        elm.before(plc);
    }
    elm.css('position', 'fixed').css('left', 0).css('top', top).css('width', '100%').css('z-index', 20000);
}