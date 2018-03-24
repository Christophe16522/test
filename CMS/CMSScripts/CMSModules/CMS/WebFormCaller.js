/**
 * Helper module able to perform WebForm specific calls to the server (callbacks and postbacks).
 *
 * (postback call is not implemented yet. Should be implemented when needed.)
 */
define([], function() {
    'use strict';

    /**
     * Performs WebForms Callback.
     *
     * @param {string} option.targetControlUniqueId - Required. The UniqueID of a server Control that handles the client callback. The control must implement the ICallbackEventHandler interface and provide a RaiseCallbackEvent method.
     * @param {string} option.args - An argument passed from the client script to the server RaiseCallbackEvent method.
     * @param {function} option.successCallback - The name of the client event handler that receives the result of the successful server event.
     * @param {function} option.context - Client script that is evaluated on the client prior to initiating the callback. The result of the script is passed back to the client event handler.
     * @param {function} option.errorCallback - The name of the client event handler that receives the result when an error occurs in the server event handler.
     * @param {bool} option.useAsync - true to perform the callback asynchronously; false to perform the callback synchronously. Default is true.
     */
    var doCallback = function(options) {
        WebForm_DoCallback(options.targetControlUniqueId, options.args, options.successCallback, options.context, options.errorCallback, options.useAsync);
    };

    return {
        doCallback: doCallback
    };
});