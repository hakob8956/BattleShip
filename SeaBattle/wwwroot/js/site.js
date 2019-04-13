function ReadyStart(Yes) {
    if (Yes) {
        $(".welcome").addClass("none");
        $("#maincontainer").removeClass("none");
        $(".battlefield-stat").removeClass("none");
        $(".battlefield__rival").removeClass("battlefield__wait").addClass(".battlefield__start");
        $(".notification").removeClass("none");
    } else {
        $(".welcome").removeClass("none");
        $("#maincontainer").addClass("none");
        $(".battlefield-stat").addClass("none");
        $(".battlefield__rival").addClass("battlefield__wait").removeClass(".battlefield__start");
        $(".notification").addClass("none");
    }
}
function YourTurn(Yes) {
    if (Yes) {
        $(".battlefield__self").addClass("battlefield__wait");
        $(".battlefield__rival").removeClass("battlefield__wait");
    } else {
        $(".battlefield__self").removeClass("battlefield__wait");
        $(".battlefield__rival").addClass("battlefield__wait");
    }
}

$(".battlefield__rival tr td.battlefield-cell__empty").click(function (e) {
    newValue = 0;
    let currentElement = $(this);
    let x = currentElement.children().data("x");
    let y = currentElement.children().data("y");
    console.log(x, y);
    hubConnection.invoke("SendCordinateEnemy", x, y, _connectionId);//GetValue

    //setTimeout(SetStatusColor, 100, currentElement);
});
function SetStatusColor(currentElement, value) {
    switch (value) {
        case 2:
            currentElement.removeClass("battlefield-cell__empty").removeClass("battlefield-cell__busy").addClass("battlefield-cell__shoot")
                .children().append("<div class='battlefield-cell__shoot__mark-leftLine'></div><div class='battlefield-cell__shoot__mark-rigthLine'></div>");
            break;
        case -1:
            currentElement.removeClass("battlefield-cell__empty").removeClass("battlefield-cell__busy").addClass("battlefield-cell__missed").children().children().addClass("battlefield-cell__missed__mark");
            break;
        case 0:
            currentElement.removeClass("battlefield-cell__empty").removeClass("battlefield-cell__busy").addClass("battlefield-cell__empty");
            break;
        case 1:
            currentElement.removeClass("battlefield-cell__empty").removeClass("battlefield-cell__busy").addClass("battlefield-cell__busy");
            break;
        default:
    }
    currentElement.attr("disabled", "disabled").off('click');
}
function changeurl(connectionID) {
    var new_url = "?id=" + connectionID;
    window.history.pushState("data", "Title", new_url);
    return window.location.href;
}

function getRandomConnectionID() {
    return Math.floor(Math.random() * 88888888) + 10000000;//get random connectionID 10000000-99999999
}