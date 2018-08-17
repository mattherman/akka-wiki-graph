"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/wikiGraphHub").build();

connection.on("ReceiveDebugInfo", addDebugMessage);
connection.on("ReceiveGraph", receiveGraph);

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

function receiveGraph(graph) {
    var root = document.getElementById("messagesList");
    console.log(graph);
    for(var node in graph) {
        outputNode(root, node, graph[node]);
     }
}

function outputNode(listRoot, node, children) {
    var link = createArticleLink(node);
    var text = document.createTextNode(' - ' + children.join(','));
    var li = document.createElement("li");
    li.appendChild(link);
    li.appendChild(text);
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