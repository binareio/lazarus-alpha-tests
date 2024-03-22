// Create a request variable and assign a new XMLHttpRequest object to it.

let  cityname = 'Chennai',cityId = '2172797'
let API_key = '51ea188a97625ddf670379a1aa8e42ca'
// Open a new connection, using the GET request on the URL endpoint
let API_Arr = ['http://api.openweathermap.org/data/2.5/weather?q='+cityname+'&appid='+API_key,'api.openweathermap.org/data/2.5/weather?id='+cityId+'&appid='+API_key]

for(var i =0;i<API_Arr.length;i++){
    var request = new XMLHttpRequest()
    console.log(API_Arr[i])
    request.open('GET',"https://cors-anywhere.herokuapp.com/"+API_Arr[i] , true)//,"rohinimurugesan.23@gmail.com","Rohi@238")
//    request.withCredentials = true
    request.onload = function() {
        // Begin accessing JSON data here
        console.log(request)
        // if (request.status === 200 or request.status === 202)
        // {
            var data = JSON.parse(this.response)
            console.log(data)
        // }
        // else{
        //     console.log("Request Failed with status"+String(request.status))
        // }
    }

    // Send request
    request.send()

}