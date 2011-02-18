![Pipes.NET logo](/ahzf/pipes.NET/raw/master/doc/pipes.NET-logo.png)

Pipes.NET is a graph-based data flow framework for [property graph models](http://github.com/tinkerpop/gremlin/wiki/Defining-a-Property-Graph)
written in .NET/Mono. It comes with some syntactic sugar to build a powerful "LINQ to graphs" interface. A process graph is composed of a set of process vertices connected to one another by a set of communication edges. Pipes supports the splitting, merging, and transformation of data from input to output. 

#### Usage

Pipes.NET comes with some syntactic sugar to make coexistence with LINQ a bit easier.

    var _Friends = _Graph.VertexId(1).
                   OutEdges("knows").
                   InVertex().
                   GetProperty<String>("name");
    foreach (var _Friend in _Friends)
    {
        Console.WriteLine(_Friend);
    }

...which is in detail equivalent to the following standard syntax:

    var _Pipe1    = new VertexEdgePipe(VertexEdgePipe.Step.OUT_EDGES);
    var _Pipe2    = new LabelFilterPipe("knows", ComparisonFilter.NOT_EQUAL);
    var _Pipe3    = new EdgeVertexPipe(EdgeVertexPipe.Step.IN_VERTEX);
    var _Pipe4    = new VertexPropertyPipe<String>("name");
    var _Pipeline = new Pipeline<IVertex,String>(_Pipe1, _Pipe2, _Pipe3, _Pipe4);
    _Pipeline.SetSource(new SingleEnumerator<IVertex>(_Graph.GetVertex(new VertexId(1)));
    foreach (var _Friend in _Pipeline)
    {
        Console.WriteLine(_Friend);
    }

#### License

Pipes.NET is released under the [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0). For details see the [LICENSE](/ahzf/pipes.NET/blob/master/LICENSE) file.

#### Acknowledgments

Pipes.NET is a reimplementation of the [Pipes](http://github.com/tinkerpop/pipes) library for Java provided by [Tinkerpop](http://tinkerpop.com).

Please read the [NOTICE](/ahzf/pipes.NET/blob/master/NOTICE) file for further credits.
