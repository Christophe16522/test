define(['CMS/EventHub', 'jQuery'], function (EventHub) {
    var $ = window.jQuery,
        wdt_closeTile = window.WDT_CloseTile, // Global function for async postback to post tile closed

    	WelcomeTile = function () {
    	    var that = this;

    	    this.$closeButton = $('.js-close-button');
    	    this.$appListLink = $('.js-app-list-link');
    	    this.$contextHelpLink = $('.js-context-help-link');
	        this.$welcomeTileParent = $('.welcome-tile').parent();

    		this.$closeButton.bind('click', function () {
		        wdt_closeTile && wdt_closeTile();
    		    that.$welcomeTileParent.remove();
    		});
    		
    		this.$appListLink.bind('click', function () {
    			EventHub.publish('ShowApplicationList');
    		});

    		this.$contextHelpLink.bind('click', function () {
                EventHub.publish('ShowContextHelp');
    		});
    	};

    return WelcomeTile;
});