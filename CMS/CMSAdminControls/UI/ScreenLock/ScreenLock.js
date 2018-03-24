var screenLockWarningOn = false;
var screenLockWarningDialogOn = false;
var screenLockIsLocked = false;
var screenLockCounter = 0;
var screenLockIntervalCounter = null;
var screenLockTimeout = null;

var longPing = 300000;
var shortPing = 60000;
var waitForPasscodVal = false;

var ARGUMENTS_SEPARATOR = "|";


function getResultShort(result, context) {
    var resultArguments = new Array();

    if (result.indexOf('|') > -1) {
        resultArguments = result.split(ARGUMENTS_SEPARATOR);
    } else {
        resultArguments[0] = result;
    }
    
    switch (resultArguments[0]) {
        case 'logout':
            parent.location.replace(result.substring(7));
            break;
        case 'missingToken':
            if (resultArguments.length < 4) {
                break;
            }
            $j('[id$=_lblInstructions]').text(resultArguments[1]);
            $j('[id$=_lblTokenInfo]').text(resultArguments[2]);
            $j('[id$=_lblTokenID]').text(resultArguments[3]);
            $j('#tokenBox').removeClass("hide");
        case 'waitingForPasscode':
            $j('#screenLockDialogWarning').addClass("hide");
            $j('#passcodeBox').removeClass("hide");
            $j('#passwordBox').addClass("hide");
            $j('#usernameBox').addClass("hide");
            waitForPasscodVal = true;
            break;
        case 'wrongPassc':
            $j('#screenLockDialogWarning').text(resultArguments[1]);
            $j('#screenLockDialogWarning').removeClass("hide");
            break;
        case 'valbad':
            $j('#screenLockDialogWarning').removeClass("hide");
            $j('[id$=_lblScreenLockWarningLogonAttempts]').addClass("hide");
            $j('#screenLockDialog input[type="password"]').focus();
            break;
        case 'valok':
            hideModalPopup('screenLockDialog', 'screenLockBackground');
            $j('#screenLockDialogWarning').addClass("hide");
            $j('#passcodeBox').addClass("hide");
            $j('#tokenBox').addClass("hide");
            $j('[id$=_txtPasscode]').val("");
            $j('#passwordBox').removeClass("hide");
            $j('#usernameBox').removeClass("hide");
            screenLockIsLocked = false;
            CallServer(resultArguments[1] * 1000);
            waitForPasscodVal = false;
            break;
        case 'isLocked':
            if (result.substring(9) == 'True') {
                if (!screenLockIsLocked) {
                    ShowScreenLockDialog();
                }
            }
            CallServer(longPing);
            break;
        case 'lockScreen':
            if (!screenLockIsLocked) {
                ShowScreenLockDialog();
            }
            CallServer(longPing);
            break;
        case 'hideWarning':
            hideModalPopup('screenLockDialog', 'screenLockBackground');
            screenLockIsLocked = false;
            if (screenLockWarningOn) {
                HideScreenLockWarning();
            }
            CallServer(result.substring(12) * 1000);
            break;
        case 'showWarning':
            hideModalPopup('screenLockDialog', 'screenLockBackground');
            screenLockIsLocked = false;
            screenLockCounter = result.substring(12);
            ShowScreenLockWarning();
            CallServer(shortPing);
            break;
        case 'cancelOk':
            HideScreenLockWarning();
            CallServer(result.substring(9) * 1000);
            break;
        case 'disabled':
            HideScreenLockWarning();
            screenLockEnabled = false;
            clearTimeout(screenLockTimeout);
            break;
        case 'accountLocked':
            $j('#screenLockDialogWarning').addClass("hide");
            $j('[id$=_lblScreenLockWarningLogonAttempts]').removeClass("hide");
            $j('#screenLockSignInButton').show();
            $j('#screenLockUnlockButton').hide();
            $j('[id$=_btnScreenLockSignOut]').hide();
            if (!screenLockIsLocked) {
                ShowScreenLockDialog();
            }
            break;
    }
}


function ScreenLockLogoutUser() {
    serverRequest('logout');
}


function ScreenLockValidateUser() {
    if (waitForPasscodVal) {
        serverRequest('validPasscode|' + $j('[id$=_txtPasscode]').val());
        return;
    }
    serverRequest('validate|' + $j('[id$=_txtScreenLockDialogPassword]').val());
}


function ScreenLockRedirectToLogon(logonpage) {
    parent.location.replace(logonpage);
}


function HideScreenLockWarning() {
    clearInterval(screenLockIntervalCounter);
    screenLockIntervalCounter = null;

    $j('#screenLockWarningDialog').hide();

    screenLockWarningOn = false;
    screenLockWarningDialogOn = false;

    window.top.layouts[0].resizeAll();
}


function HideScreenLockWarningAndSync(timeout) {
    if (timeout > 0) {
        HideScreenLockWarning();
    }
    CallServer(timeout * 1000);
    screenLockPinging = true;
}


function ShowScreenLockWarning() {
    screenLockWarningOn = true;
    if (screenLockIntervalCounter == null) {
        screenLockIntervalCounter = setInterval(UpdateScreenLockWarning, 1000);
    }

    var dialogOn = ($j('#modalBack').is(':visible')) ? true : false;
    if (dialogOn) {
        screenLockWarningDialogOn = true;
    }

    $j('#screenLockWarningDialog').show();

    UpdateScreenLockWarning();

    window.top.layouts[0].resizeAll();
}


function CallServer(timeoutPeriod) {
    clearTimeout(screenLockTimeout);
    screenLockTimeout = setTimeout('serverRequest("isLocked");', timeoutPeriod);
}


function UpdateScreenLockWarning() {
    if (screenLockWarningOn) {
        if (screenLockCounter > 0) {
            screenLockCounter--;
        }
        else {
            clearInterval(screenLockIntervalCounter);
            screenLockIntervalCounter = null;
            CallServer(0);
        }
        $j('#screenLockTime').html(screenLockCounter);
        $j('#screenLockTimeDialog').html(screenLockCounter);

        var dialogOn = ($j('#modalBack').is(':visible')) ? true : false;
        if (!dialogOn) {
            if (screenLockWarningDialogOn) {
                $j('#screenLockWarningDialog').hide();

                screenLockWarningOn = false;
                screenLockWarningDialogOn = false;

                // Dialog was closed while countdown
                serverRequest('action');
            }
        }
    }
}


function ShowScreenLockDialog() {
    HideScreenLockWarning();
    showModalPopup('screenLockDialog', 'screenLockBackground');
    var txtPassword = $j('#screenLockDialog input[type="password"]');
    txtPassword.val('');
    txtPassword.focus();
    screenLockIsLocked = true;
}


function CancelScreenLockCountdown() {
    serverRequest('cancel');
}


function ScreenLockEnterHandler(event) {
    if (event.which == 13 || event.keyCode == 13) {
        ScreenLockValidateUser();
        return false;
    }
    return true;
}