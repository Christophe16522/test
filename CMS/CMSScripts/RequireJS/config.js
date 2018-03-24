require.config({
    baseUrl: '{%AppPath%}/CMSPages/GetResource.ashx?scriptmodule=',
    paths: {
        jQuery: '{%AppPath%}/CMSScripts/jquery/jquery-core',
        Underscore: '{%AppPath%}/CMSScripts/underscore/underscore.min',
        jQueryJScrollPane: '{%AppPath%}/CMSScripts/jquery-jscrollpane',
        jQueryFancySelector: '{%AppPath%}/CMSScripts/jquery/jquery-fancyselect'
    },
    shim: {
        jQuery: { exports: 'jQuery' },
        Underscore: { exports: '_' },
        jQueryJScrollPane: { deps: ['jQuery'] },
        jQueryFancySelector: { deps: ['jQuery'] }
    }
});    