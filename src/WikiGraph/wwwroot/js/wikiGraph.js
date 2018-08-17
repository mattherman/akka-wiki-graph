"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/wikiGraphHub").build();

connection.on("ReceiveDebugInfo", addDebugMessage);
connection.on("ReceiveArticle", receiveArticle);

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

function receiveArticle(article) {
    var root = document.getElementById("messagesList");

    var linkedArticles = article.linkedArticles;
    for (var i = 0; i < linkedArticles.length; i++) {
        outputArticle(root, linkedArticles[i]);
    }
}

function outputArticle(listRoot, article) {
    var link = createArticleLink(article.title);
    var li = document.createElement("li");
    li.textContent = "Linked to ";
    li.appendChild(link);
    listRoot.appendChild(li);
}

function createArticleLink(title) {
    var link = document.createElement("a");

    var address = encodeURI("http://wikipedia.org/wiki/" + title);

    link.href = address;
    link.title = title;
    link.textContent = title;

    return link;
}