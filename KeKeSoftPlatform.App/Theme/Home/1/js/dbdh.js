
$(document).ready(function()
{
	//slides the element with class "menu_body" when paragraph with class "menu_head" is clicked 
	$("#firstpane p.menu_head").click(function()
    {
	    $(this).css({ backgroundImage: "url(/Theme/Home/1/images/left_bg.jpg)" }).next("div.menu_body").slideToggle(300).siblings("div.menu_body").slideUp("slow");
	    $(this).siblings().css({ backgroundImage: "url(/Theme/Home/1/images/left_bg.jpg)" });
	});
	//slides the element with class "menu_body" when mouse is over the paragraph
	$("#secondpane p.menu_head").mouseover(function()
    {
	    $(this).css({ backgroundImage: "url(/Theme/Home/1/images/left_bg.jpg)" }).next("div.menu_body").slideDown(500).siblings("div.menu_body").slideUp("slow");
	    $(this).siblings().css({ backgroundImage: "url(/Theme/Home/1/images/left_bg.jpg)" });
	});
});