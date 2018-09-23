"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/wikiGraphHub").build();

connection.on("ReceiveDebugInfo", addDebugMessage);
connection.on("ReceiveGraph", receiveGraph);

connection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("submitButton").addEventListener("click", function (event) {
    Graph = null;
    document.getElementById("messagesList").innerHTML = "";

    var address = document.getElementById("addressInput").value;
    var depth = parseInt(document.getElementById("depthInput").value);

    initiateCrawl(address, depth);

    event.preventDefault();
});

function initiateCrawl(address, depth) {
    connection.invoke("SubmitAddress", address, depth).catch(function (err) {
        return console.error(err.toString());
    });
}

function addDebugMessage(message) {
    var now = new Date();
    var timestamp = `${now.getHours()}:${now.getMinutes()}:${now.getSeconds()}.${now.getMilliseconds().toFixed(3)}`

    var li = document.createElement("li");
    if (message.type !== 0) {
        li.style.color = "red";
    }

    li.textContent = `[${timestamp}] ${message.message}`;

    document.getElementById("messagesList").appendChild(li);
}

function showGraph() {
    return document.getElementById("showGraphInput").checked;
}

function receiveGraph(graph) {
    if (showGraph()) {
        const gData = buildGraphData(graph);
        createNewGraph(gData);
    }
}

function buildGraphData(graph) {
    const nodes = Object.keys(graph).map(n => ({ id: n.toLowerCase() }));
    const linkCollections = Object.keys(graph).map(n => graph[n].map(l => ({ source: n.toLowerCase(), target: l.toLowerCase() })));
    const links = [].concat.apply([], linkCollections);
    const graphData = {
      nodes: nodes,
      links: links
    };

    console.log(graphData);

    return graphData;
}

function createNewGraph(graphData) {
    Graph = ForceGraph()
      (document.getElementById('graph'))
        .nodeLabel('id')
        .onNodeClick(node => {
            Graph.centerAt(node.x, node.y, 1000);
        })
        .graphData(graphData);
}

function crawlNode(node) {
    var address = encodeURI("http://wikipedia.org/wiki/" + node.id);
    initiateCrawl(address, 1);
}

let Graph;