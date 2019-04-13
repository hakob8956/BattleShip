const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/seabattle")
    .build();


hubConnection.on("Notify", function (message) {
    console.log(message);
    $("#notif_content").text(message);
});

hubConnection.on("TakeStatus", function (_newField, currentTurn, message, countKill, win) {
    console.log("CountKill = " + countKill);
    let field = JSON.parse(_newField);//Get array[,]
    for (var i = 0; i < 10; i++) {
        for (var j = 0; j < 10; j++) {
            if (field[i][j] === -1 || field[i][j] === 2) {
                let currentElement = $(".battlefield__rival tr td.battlefield-cell .battlefield-cell-content[data-x='" + j + "'][data-y='" + i + "']");
                SetStatusColor(currentElement.parent(), field[i][j]);
            }
        }
    }
    $("#notif_content").text(message);
    YourTurn(currentTurn);
    if (countKill !== 0) {
        var mess = ".battlefield__rival .ship-types .ship-type__len_" + countKill + " .ship";
        $(mess).each(function () {
            var a;
            $(this).addClass(function (index, currentClass) {
                var addedClass = "";
                console.log("CurrentClass" + currentClass);
                if (currentClass !== "ship ship__killed") {                  
                    addedClass = "ship__killed";
                    a = true;
                }
                return addedClass;
            });
         
            if (a) {

                return false;
            }

        });
    }
    if (win) {
        alert("You  Win");
        location.reload();
    }

});

hubConnection.on("SetStatus", function (_newField, currentTurn, message, countKill, win) {
    console.log("CountKill = " + countKill);
    let field = JSON.parse(_newField);//Get array[,]
    for (var i = 0; i < 10; i++) {
        for (var j = 0; j < 10; j++) {
            if (field[i][j] === -1 || field[i][j] === 2) {
                let currentElement = $(".battlefield__self tr td.battlefield-cell .battlefield-cell-content[data-x='" + j + "'][data-y='" + i + "']");
                SetStatusColor(currentElement.parent(), field[i][j]);
            }
        }
    }
    YourTurn(currentTurn);
    $("#notif_content").text(message);
    if (countKill !== 0) {
        var mess = ".battlefield__self .ship-types .ship-type__len_" + countKill + " .ship";
        $(mess).each(function () {
            var a;
            $(this).addClass(function (index, currentClass) {
                var addedClass = "";
                console.log("CurrentClass" + currentClass);
                if (currentClass !== "ship ship__killed") {
                    addedClass = "ship__killed";
                    a=true;
                }
                return addedClass;
            });
            console.log(a);
            if (a) {

                return false;
            }

        });
    }
    if (win) {
        alert("You  Loss");
        location.reload();
    }


});
hubConnection.on("StartGame", function (_newField, connectionId) {
    _connectionId = connectionId;
    let field = JSON.parse(_newField);
    for (var i = 0; i < 10; i++) {
        for (var j = 0; j < 10; j++) {
            let currentElement = $(".battlefield__self tr td.battlefield-cell .battlefield-cell-content[data-x='" + j + "'][data-y='" + i + "']");
            SetStatusColor(currentElement.parent(), field[i][j]);
        }
    }
    ReadyStart(true);
    YourTurn(false);


});
hubConnection.on("ChangeTurn", function (YesOrNo) {
    YourTurn(YesOrNo);
});

document.getElementById("connectBtn").addEventListener("click", function (e) {

    $(".notification").removeClass("none");
    hubConnection.invoke("Enter", _connectionId);

});
hubConnection.on("GetConnectionID", function (connectionID) {
    _connectionId = connectionID;
    console.log("GetConnectionID = " + connectionID);
    let textBox = $("#connectionIDtxt");
    textBox.attr("value", changeurl(connectionID));
    textBox.attr("data-value", connectionID);
});
hubConnection.on("Disconnect", function (connectionID) {
    alert("Your enemy left");
    window.location.reload();
});
hubConnection.onclose(function (e) {
    alert('Connection Close');
    window.location.reload();
});


hubConnection.start();

hubConnection.serverTimeoutInMilliseconds = 100000;//100 second


