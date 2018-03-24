jQuery(document).ready(function(){
	
	jQuery(".menu_parent").mouseover(function(){
	var h_parent=20;
	var h_sous_menu=jQuery(this).find(".sous_menu").height();
	var h_total=(h_parent+h_sous_menu);
	if((h_sous_menu).val=!'')
	{
		jQuery(this).stop().animate({height: h_total+20+'px'},{queue:false, duration:600, easing: 'easeInQuint'})
		jQuery(this).find("li").click(function(){
			jQuery(this).addClass("active");
			jQuery(this).parent().parent().addClass("active");
			});
	}
	});
	jQuery("li.menu_parent").mouseout(function(){
	if(!jQuery(this).hasClass("active"))
	{jQuery(this).stop().animate({height:'37px'},{queue:false, duration:600, easing: 'easeInQuint'})
		}
	});
	
});