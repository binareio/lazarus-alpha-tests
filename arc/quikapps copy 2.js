describe('Protractor psc2', function() {
	var obj=  require("./Jsobjectdemo.js");
  var using=  require("jasmine-data-provider");
  var d=  require("./data.js");
  it('psc application', function() {
    obj.getURL();
    
  });

  obj.close.click().then(function(){
    browser.sleep(1000);
});






   //Test1
   //obj.email.sendKeys("akshayabm@chimeratechnologies.com");
    //.then(function(){
    	    //	browser.sleep(1000);
   // });
    //obj.password.sendKeys("akshayC@1")
    //.then(function(){
    	    //	browser.sleep(1000);
  //  });
   // obj.login.click().then(function(){
   // 	    	browser.sleep(1000);
   // });
	using(d.Datadriven2, function (data, description) {
    it('TS-'+description, function() {
      obj.email1.sendKeys(data.email1);
      obj.password1.sendKeys(data.password1);
      obj.login1.click();
      expect(obj.result1.getText()).toBe(data.result1);
      obj.result1.getText().then(function(text){
        console.log(text);
        browser.sleep(1000);
    
    });
    
    });	
    });	
  
  
 //Test2
   using(d.Datadriven3, function (data, description) {
    it('TS-'+description, function() {
      obj.privacy.click().then(function(){
        browser.sleep(1000);
  });
  obj.close.click().then(function(){
    browser.sleep(1000);
});

    obj.student.click().then(function(){
      browser.sleep(1000);
});
    obj.addstudent.click().then(function(){
      browser.sleep(1000);
});
    obj.country.click();
    obj.countryselect.click();
    obj.district.click();
    obj.districtselect.click();
    obj.studentid.sendKeys(data.studentid);
    obj.fname.sendKeys(data.fname);
    obj.lname.sendKeys(data.lname);
    obj.relation.click().then(function(){
      	browser.sleep(1000);
});
obj.relationselect.click().then(function(){
  browser.sleep(1000);
});
    obj.studentbutton.click().then(function(){
      browser.sleep(1000);
});
    
    browser.switchTo().alert().dismiss().then(function()

    		{
    		//10sec
    		browser.sleep(5000);
    		});

    obj.studentclosepopup.click().then(function(){
    browser.sleep(1000);
    });
    
obj.deletestudent.click().then(function(){
  browser.sleep(1000);
});
    expect(obj.removepopup.getText()).toBe(data.removepopup);
    obj.removepopup.getText().then(function(text){
      console.log(text);
      browser.sleep(1000);
  
  });
  obj.yes.click().then(function(){
    browser.sleep(1000);
});

obj.returntodashboard.click().then(function(){
  browser.sleep(1000);
});



  });	
  });	




//test6
using(d.Datadriven4, function (data, description) {
  it('TS-'+description, function() {
    
    obj.paymentmethod.click().then(function(){
  browser.sleep(1000);
});

obj.addpayment.click().then(function(){
  browser.sleep(1000);
});

obj.paymentdropdown.click().then(function(){
  browser.sleep(1000);
});

obj.paymentselection.click().then(function(){
  browser.sleep(1000);
});

obj.nickname.sendKeys(data.nickname).then(function(){
  browser.sleep(1000);
});

obj.cardnumber.sendKeys(data.cardnumber).then(function(){
  browser.sleep(1000);
});

obj.expirydate.sendKeys(data.expirydate).then(function(){
  browser.sleep(1000);
});

obj.cvv.sendKeys(data.cvv).then(function(){
  browser.sleep(1000);
});

obj.checkbox1.click().then(function(){
  browser.sleep(1000);
});

obj.checkbox2.click().then(function(){
  browser.sleep(1000);
});

obj.addpaymentbutton.click().then(function(){
  browser.sleep(1000);
});

obj.cancelpayment.click().then(function(){
  browser.sleep(1000);
});


obj.cancelpaymentyes.click().then(function(){
  browser.sleep(1000);
});



});
});










  // Last bracket 
  });	