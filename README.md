![Styx logo](/ahzf/Styx/raw/master/artwork/styx_small.png)

[Styx](http://github.com/ahzf/Styx) is the foundation of a graph-based data flow framework for any kind of data processing.
It is used heavily within [Balder](http://github.com/ahzf/Balder) a data flow framework for
[property graph models](http://github.com/tinkerpop/gremlin/wiki/Defining-a-Property-Graph) for .NET/Mono.
A process graph is composed of a set of process vertices connected to one another by a set of communication edges.
Pipes supports the splitting, merging, and transformation of data from input to output. 

#### Usage

Styx comes with some syntactic sugar to make coexistence with LINQ a bit easier.

    var _Friends = _Graph.VertexId(1).
                   OutEdges("knows").
                   InVertex().
                   GetProperty<String>("name");

    _Friends.ForEach(_Friend => Console.WriteLine(_Friend));

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

#### Help and Documentation

Additional help and much more examples can be found in the [Wiki](http://github.com/ahzf/Styx/wiki).   
News and updates can also be found on twitter by following: [@ahzf](http://www.twitter.com/ahzf) or [@graphdbs](http://www.twitter.com/graphdbs).

#### Installation

The installation of Styx is very straightforward.    
Just check out or download its sources and all its dependencies:

- [NUnit](http://www.nunit.org/) for unit tests

#### License and your contribution

[Styx](http://github.com/ahzf/Styx) is released under the [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0). For details see the [LICENSE](/ahzf/Styx/blob/master/LICENSE) file.    
To suggest a feature, report a bug or general discussion: [http://github.com/ahzf/Styx/issues](http://github.com/ahzf/Styx/issues)    
If you want to help or contribute source code to this project, please use the same license.   
The coding standards can be found by reading the code ;)

#### Acknowledgments

Styx is a reimplementation of the [Pipes](http://github.com/tinkerpop/pipes) library for Java provided by [Tinkerpop](http://tinkerpop.com).    
Please read the [NOTICE](/ahzf/Styx/blob/master/NOTICE) file for further credits.

#### 
