

const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/seabattle")
    .build();


hubConnection.on("Notify", function (message) {
    console.log(message);
    $("#notif_content").text(message);
});
hubConnection.on("InvaildField", function () {
    swal("Invaild Field", "", "error");
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
        swal("You Win", "", "info").then(() => window.location.reload());
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
                    a = true;
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
        swal("You loss", "", "info").then(() => window.location.reload());

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

hubConnection.on("CanSetShip", function (_canSet, elementId, x, y, changeDir) {
    canSet = _canSet;
    var _droppable = $(".battlefield__self tr td.battlefield-cell .battlefield-cell-content[data-x='" + x + "'][data-y='" + y + "']");
    var _draggable = $(".ship-box[data-id='" + elementId + "']");
    if (canSet && !changeDir) {
        Ox[elementId] = x;
        Oy[elementId] = y;

        _droppable.children().append(_draggable.css({ "left": "0", "rigth": "0", "top": "0", "bottom": "0" }));
    }

    if (changeDir && canSet) {
        console.log(width);

        var width = _draggable.css("width");
        var height = _draggable.css("height")
        var dir = _draggable.data("position");
        switch (dir) {
            case "v":
                dir = "h";
                break;
            case "h":
                dir = "v";
                break;
            default:
        }

        _draggable.data("position", dir);

        _draggable.css({ "width": height, "height": width });
    }
    console.log("CanSetShip " + canSet);
});
hubConnection.on("DeleteShip", function () {
    console.log("DeleteShip");
});

hubConnection.on("GetConnectionID", function (connectionID) {
    _connectionId = connectionID;
    console.log("GetConnectionID = " + connectionID);
    let textBox = $("#connectionIDtxt");
    textBox.attr("value", changeurl(connectionID));
    textBox.attr("data-value", connectionID);
});
hubConnection.on("Disconnect", function (connectionID) {
    swal("Your opponent has left the game.", "", "info").then(() => window.location.reload());

});



hubConnection.onclose(function (e) {
    swal("Connection was interrupted.", "", "info").then(() => window.location.reload());

});
$("body").on("dblclick", "td .ship-box", function () {
    var dir = $(this).data("position");
    var x = $(this).parent().parent().data("x");
    var y = $(this).parent().parent().data("y");
    var length = $(this).data("length");
    var currentId = $(this).data("id");
    console.log("DeleteShip - > x = " + x + " y = " + y + " id  = " + currentId);
    hubConnection.invoke("DeleteShip", x, y);
    switch (dir) {
        case "v":
            dir = 1;
            break;
        case "h":
            dir = 0;
            break;
        default:
    }
    hubConnection.invoke("CanSetShip", length, dir, x, y, x, y, currentId, true);

    console.log("length = " + length + " dir = " + dir + " x = " + x + " y = " + y);
});
$(function () {
    $(".ship-box").draggable({
        revert: true,
        revertDuration: 0,
        cursor: "move",
        cursorAt: { top: 0, left: 0, right: 0, bottom: 0 }

    });
    $(".battlefield-cell__empty").droppable({
        drop: function (event, ul) {
            console.log('------------------------------------------------------------');

            var length = ul.draggable.data("length");
            var dir = ul.draggable.data("position");
            x = $(this).children().data("x");
            y = $(this).children().data("y");
            let curentId = ul.draggable.data("id");
            let oldX = Ox[curentId];
            let oldY = Oy[curentId];
            if (oldX == null || oldY == null) {
                oldX = -1;
                oldY = -1;
            }
            console.log("DeleteShip - > x = " + oldX + " y = " + oldY + " id  = " + curentId + " dir = " + dir);
            hubConnection.invoke("DeleteShip", oldX, oldY);
            switch (dir) {
                case "v":
                    dir = 0;
                    break;
                case "h":
                    dir = 1;
                    break;
                default:
            }
            hubConnection.invoke("CanSetShip", length, dir, x, y, oldX, oldY, curentId, false);
            ul.draggable.disableSelection();
            console.log("length = " + length + " dir = " + dir + " x = " + x + " y = " + y);

        },
        over: function (e, ul) {

        }

    });

});
$('input[type=radio][name=setting]').change(function () {
    if (this.value == 'random') {
        $(".battlefield__self .battlefield-gap").removeClass("battlefield__preparing").addClass("none");
        $(".port").addClass("none");
        hubConnection.invoke("DeleteField");

    }
    else if (this.value == 'setup') {
        $(".battlefield__self .battlefield-gap").addClass("battlefield__preparing").removeClass("none");
        $(".port").removeClass("none");
    }
});

hubConnection.start();

hubConnection.serverTimeoutInMilliseconds = 100000;//100 second