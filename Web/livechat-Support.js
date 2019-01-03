$(document).ready(function() {

    $("#startform").submit(function() {
        name = $("#name").val();
        if(name == "") {
            $("#response").html("Name Eingeben LOS!");
        } else {
            con = new WebSocket('ws://localhost:4649/LiveSocket');
            $("#submit").attr("disable", "disable");
            $("#response").html("Loading...");
            con.onopen = function () {
                con.send('name='+name+ ';' + 'key=TestKey'); // Send the message 'Ping' to the server
            };
            $("#main").html('<form id="test" method="post"><div id="ChatBox"></div><input type="text" id="msg">' +
                        '<input type="hidden" id="name" value="' + name +'">' +
                        '<input type="submit" id="msgsend" value="Send">');
                        con.onerror = function (error) {
                console.log('WebSocket Error ' + error);
            };

            // Log messages from the server
            con.onmessage = function (e) {
                $("#ChatBox").append(e.data+"<br / >");
                console.log(e.data);
            };
            $("#response").html("Mhm OK - " + name);
            $("#test").submit(function() {
                    console.log("name="+$("#name").val() + ";msg="+$("#msg").val());
                    con.send("name="+$("#name").val() + ";msg="+$("#msg").val());
                return false;
            });
            return false;
        }
        return false;
    });

});