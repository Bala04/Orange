$(function (mintimeout) {

   function getData(c, b) {

        var a = JSON.parse(c);

        for (var i = 0; i < a.length; i++) {
            if (a[i].actor == b) {
                return a[i];
            }
        }

        return "-1";
    }
    
    function getServer(hub) {
        return $.connection[hub].server;
    }
    function callBack(hub, name, func) {
        $.connection[hub].client[name] = func;
    }
    function startServer(func) {
        $.connection.hub.start().done(func);
    }
    function stopServer() {
        $.connection.hub.stop();

    }
    function setOnStart(func) {
        $.connection.hub.starting(func);
    }
    function setOnError(func) {
        $.connection.hub.error(func);
    }
    var disconnect;
    function setOnDisconnected(func) {
        disconnect = func;
    }
    function setOnReconnected(func) {
        $.connection.hub.reconnected(func);
    }
    function setOnConnectionSlow(func) {
        $.connection.hub.connectionSlow(func);
    }

    var notify;
    var d = false;
   

    $.connection.hub.logging = true;
    
    $.connection.hub.disconnected(function () {
        console.log("disconnected");
        d = true;
        changeCount();
        disconnect();
    });

    function reStartServer() {
       
        if (d) {
            console.log("startServer");

            $.connection.hub.start().done(
                function () {
                    d = false;
                    count = mintimeout;
                    setNewTimeout();
                });

        }
    }
    var count = mintimeout;
    var interval = setTimeout(reStartServer, count);
    function changeCount() {
        console.log(count);
        count = count * 2;
        setNewTimeout();
    }
    function setNewTimeout() {
        clearInterval(interval);
        interval = setTimeout(reStartServer, count);
    }

});