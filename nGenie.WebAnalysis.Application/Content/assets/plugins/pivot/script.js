$(function() {
	var _data;
	var json;
	var myJSONConfigurator = [];
	var myJSONValue = [];
	var xmlAsString;
	var jsonAsString;
  
  var result = document.getElementById('result');
  $.get('http://ssas.ngenie.local:8085/kendoui/data/hello.xml', function(xml) {
    //console.log(xml);
    // This part is for displaying the xml that should be converted in this example
    xmlAsString = xml.documentElement.innerHTML;
    //result.textContent = xmlAsString;
  });
  $('#convert').click(function() {
    $.get('http://ssas.ngenie.local:8085/kendoui/data/hello.xml', function(xml) {
      json = $.xml2json(xml);
      //console.log(json);
      // This part is for displaying the json result  in this example
      jsonAsString = JSON.stringify(json, null, 2);
      result.innerHTML = jsonAsString;
    });
  });
});