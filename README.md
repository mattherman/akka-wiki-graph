# akka-wiki-graph

A web crawler built with Akka.NET that builds a graph of Wikipedia pages and their linked articles.

## TODO
* Actual graph visualization in the UI
* Separate requests by client
* Refactor download code to handle errors better and use `ContinueWith().PipeTo()` pattern
* Move router to HOCON
* Add debug/metric messages and display
* Make the UI look nicer