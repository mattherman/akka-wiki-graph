"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/wikiGraphHub").build();

connection.on("ReceiveDebugInfo", addDebugMessage);
connection.on("ReceiveLink", receiveLink);

connection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("submitButton").addEventListener("click", function (event) {
    document.getElementById("messagesList").innerHTML = "";
    var message = document.getElementById("addressInput").value;
    connection.invoke("SubmitAddress", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function addDebugMessage(message) {
    var li = document.createElement("li");
    li.textContent = message;
    document.getElementById("messagesList").appendChild(li);
}

function receiveLink(address, title) {
    var link = document.createElement("a");
    link.href = address;
    link.title = title;
    link.textContent = title;
    var li = document.createElement("li");
    li.textContent = "Linked to ";
    li.appendChild(link);
    document.getElementById("messagesList").appendChild(li);
}