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
    var li = document.createElement("li");
    li.textContent = message;
    document.getElementById("messagesList").appendChild(li);
}

function receiveGraph(graph) {
    if (Graph) {
        updateExistingGraph(graph);
    } else {
        createNewGraph(graph);
    }
}

function buildGraphData(graph) {
    const nodes = Object.keys(graph).map(n => ({ id: n.toLowerCase() }));
    const linkCollections = Object.keys(graph).map(n => graph[n].map(l => ({ source: n.toLowerCase(), target: l.toLowerCase() })));
    const links = [].concat.apply([], linkCollections);
    return {
      nodes: nodes,
      links: links
    };
}

function createNewGraph(graph) {
    const gData = buildGraphData(graph);

    Graph = ForceGraph()
      (document.getElementById('graph'))
        .nodeLabel('id')
        .onNodeClick(node => {
            Graph.centerAt(node.x, node.y, 1000);
            crawlNode(node);
        })
        .graphData(gData);
}

function updateExistingGraph(graph) {
    const newData = buildGraphData(graph);
    const previousData = Graph.graphData();

    Graph.graphData({
        nodes: _.union(previousData.nodes, newData.nodes),
        links: _.union(previousData.links, newData.links)
    });
}

function crawlNode(node) {
    var address = encodeURI("http://wikipedia.org/wiki/" + node.id);
    initiateCrawl(address, 1);
}

let Graph;