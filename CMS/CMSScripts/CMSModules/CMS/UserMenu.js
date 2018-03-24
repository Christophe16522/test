define(['CMS/checkChanges', 'jQuery'], function (checkChanges) {

    var $ = window.jQuery,

        UserMenu = function (serverData) {
            var that = this,
                $userMenuWrapper = $(serverData.wrapperSelector),
                $myProfileLink;

            if ($userMenuWrapper) {
                $myProfileLink = $(serverData.checkChangesLinksSelector, $userMenuWrapper);
            } else {
                $myProfileLink = $(serverData.checkChangesLinksSelector);
            }

            $myProfileLink.on('click', function(e) {
                that.onMyProfileClick(e);
            });
        };

    // Prevent button default behavior when there are some
    // unsaved changes, otherwise do nothing.
    UserMenu.prototype.onMyProfileClick = checkChanges(null, function (e) {
        e.preventDefault();
    });


    return UserMenu;

});