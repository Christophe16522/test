/** 
 * CheckChanges util function
 */
define(['Underscore'], function () {

    var _ = window._,
        globalCheckChanges = window.CheckChanges,

        /**
         * Checks if there are some unsaved changes in the UI. The function stores original
         * arguments and injects them to success and failure callbacks.
         * @param  {Function} success Function callback called on success (no changes unsaved)
         * @param  {Function} failure Function callback called on failure (some changes unsaved)
         */
        checkChanges = function (success, failure) {
            return function () {
                if (globalCheckChanges && !globalCheckChanges()) {
                    if (failure) {
                        return failure.apply(this, _.toArray(arguments));
                    }
                } else {
                    if (success) {
                        return success.apply(this, _.toArray(arguments));
                    }
                }
            };
        };

    return checkChanges;
});