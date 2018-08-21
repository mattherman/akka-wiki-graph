"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/wikiGraphHub").build();

connection.on("ReceiveDebugInfo", addDebugMessage);
connection.on("ReceiveGraph", loadGraph);

connection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("submitButton").addEventListener("click", function (event) {
    document.getElementById("messagesList").innerHTML = "";
    var address = document.getElementById("addressInput").value;
    var depth = parseInt(document.getElementById("depthInput").value);
    connection.invoke("SubmitAddress", address, depth).catch(function (err) {
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

function loadGraph(graph) {
    const nodes = Object.keys(graph).map(n => ({ id: n.toLowerCase() }));
    const linkCollections = Object.keys(graph).map(n => graph[n].map(l => ({ source: n.toLowerCase(), target: l.toLowerCase() })));
    const links = [].concat.apply([], linkCollections);
    const gData = {
      nodes: nodes,
      links: links
    };

    const Graph = ForceGraph()
      (document.getElementById('graph'))
        .nodeLabel('id')
        .onNodeClick(node => {
            // Center/zoom on node
            Graph.centerAt(node.x, node.y, 1000);
            Graph.zoom(2, 2000);
        })
        .graphData(gData);
}