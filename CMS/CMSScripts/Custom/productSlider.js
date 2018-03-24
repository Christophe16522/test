function productSlider(stepLength,width,height,container,buttonNext,buttonPrev)
{
  if ($j(container+" ul li").length>stepLength)
  {
    overStep=$j(container+" ul li").length%stepLength;
    blanks=0;
    if (overStep>0) blanks=stepLength-overStep;
    for (i=0;i<blanks;i++) 
      $j(container+" ul").append("<li></li>");
    $j(container).jCarouselLite({
      'btnNext': buttonNext,
      'btnPrev': buttonPrev,
      'width': width,
      'height': height,
      'circular': false,
      'visible': stepLength,
      'scroll': stepLength
    }); 
    $j(container).show();
    $j(container).mouseover(function(){
      $j(buttonNext).show();
      $j(buttonPrev).show();
    });
    $j(container).mouseout(function(){
      $j(buttonNext).hide();
      $j(buttonPrev).hide();
    });
  }
}