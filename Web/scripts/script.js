$(document).ready(function() {
    $('input[type="radio"]').click(function() {
       if($(this).attr('id') == 'y-option') {
            $('#officeItemY').fadeIn();         
       }

       else {
            $('#officeItemY').fadeOut();   
       }
   });
	$('input[type="radio"]').click(function() {
       if($(this).attr('id') == 'n-option') {
            $('#officeItemN').fadeIn();           
       }

       else {
            $('#officeItemN').fadeOut();   
       }
   });
	$('#y-option').click(function() {     
    $('html,body').animate({
        scrollTop :720          //$("#officeItemN").offset().top();                         
    }, 1000);
});
	$('#n-option').click(function() {     
    $('html,body').animate({
        scrollTop :400                
    }, 1000);
});
	$("i").click(function () {
    $('.search > input[type="text"],.search > .out > .fa').toggle();
	$('.nav-left,.nav-right').toggle()("display","none");
    });
});
