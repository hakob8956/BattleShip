
function changeurl(connectionID) {
    var new_url = "https://localhost:44333/id" + connectionID;
    window.history.pushState("data", "Title", new_url);


}
const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/seabattle")
    .build();
let userName = "";
// получение сообщения от сервера
hubConnection.on("Receive", function (message, userName, connectionID) {

    // создаем элемент <b> для имени пользователя
    let userNameElem = document.createElement("b");
    userNameElem.appendChild(document.createTextNode(userName + ": "));

    // создает элемент <p> для сообщения пользователя
    let elem = document.createElement("p");
    elem.appendChild(userNameElem);
    elem.appendChild(document.createTextNode(message));
    a
    var firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(elem, firstElem);

});

// диагностическое сообщение
hubConnection.on("Notify", function (message) {

    let elem = document.createElement("p");
    elem.appendChild(document.createTextNode(message));

    var firstElem = document.getElementById("chatroom").firstChild;
    document.getElementById("chatroom").insertBefore(elem, firstElem);
});

// установка имени пользователя
document.getElementById("loginBtn").addEventListener("click", function (e) {
    hubConnection.invoke("Enter", "");

});
hubConnection.on("GetConnectionID", function (connectionID) {
    console.log("ID: " + connectionID);
    changeurl(connectionID);
});
// отправка сообщения на сервер
document.getElementById("sendBtn").addEventListener("click", function (e) {
    let message = document.getElementById("message").value;

    hubConnection.invoke("Send", message, userName);
});

hubConnection.start();
