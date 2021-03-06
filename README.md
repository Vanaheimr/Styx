![Styx logo](/artwork/styx_small.png)

[Styx](http://github.com/Vanaheimr/Styx) is the foundation of a graph-based data flow framework for any kind of data processing.
It is used heavily within [Balder](http://github.com/Vanaheimr/Balder) a data flow framework for
[property graph models](http://github.com/tinkerpop/gremlin/wiki/Defining-a-Property-Graph) on .NET/Mono.
A process graph is composed of a set of process vertices connected to one another by a set of communication edges.
Styx supports the splitting, merging, transformation and concurrent processing of data from input to output. It comes in three flavors:

#### Usage


##### Pipes
The **Pipes** subproject is a lazy data flow framework.    
![Pipes visualization](/artwork/pipes_small.png)

    var List = new List<Int32>() { 0, 1, 2, 2, 3, 4, 4, 5, 6, 2, 7, 8, 9, 1 }.
        DuplicateFilter().
        RangeFilter(2, 7).
        Skip(2).
        RandomFilter(0.25).
        ToList();

##### Arrows

The **Arrows** subproject is an event-based data flow framework.    
![Arrows visualization](/artwork/arrows_small.png)


##### Sensors

The **Sensors** subproject is an data source framework for all kinds of data emitting objects.    
  ![Sensors visualization](/artwork/sensors_small.png)

The following code will create an instance of a sensor producing a sinus wave. This could be used as a mockup for a voltage or current sensoring application - even when the frequency is very low here ;)    
The **WithTimestamp** extension method will modify the output of the sensor to include the timestamp when the 'measurement' took place.    
The **ToActiveSensor** extension method will transform the sensor from an lazy sensor to an event-sending sensor.    
The **SkipArrow** extension method will skip the first event.    
The **ActionArrow** extention method will call the given delegate for every received arrow/event.    

    new SinusSensor("/dev/sinus") {
        Frequency            = 0.05,
        Amplitude            = 240,
        MeasurementIntervall = TimeSpan.FromSeconds(1)
    }.
    WithTimestamp().
    ToActiveSensor(Autostart: true).
    SkipArrow(1).
    ActionArrow(measurement => {
        Console.WriteLine(measurement.Timestamp + "\t" + measurement.Value);
    });

You will see the current timestamp and value of a slow sinus wave on the console output.


#### Help and Documentation

Additional help and much more examples can be found in the [Wiki](http://github.com/Vanaheimr/Styx/wiki).   
News and updates can also be found on twitter by following: [@ahzf](http://www.twitter.com/ahzf).

#### Installation

The installation of Styx is very straightforward.    
Just check out or download its sources and all its dependencies:

- [Illias Commons](http://www.github.com/Vanaheimr/Illias) for common .NET tools.
- [NUnit](http://www.nunit.org/) for unit tests.

#### License and your contribution

[Styx](http://github.com/Vanaheimr/Styx) is released under the [Apache License 2.0](http://www.apache.org/licenses/LICENSE-2.0). For details see the [LICENSE](/Vanaheimr/Styx/blob/master/LICENSE) file.    
To suggest a feature, report a bug or general discussion: [http://github.com/Vanaheimr/Styx/issues](http://github.com/Vanaheimr/Styx/issues)    
If you want to help or contribute source code to this project, please use the same license.   
The coding standards can be found by reading the code ;)

#### Acknowledgments

Styx is a reimplementation of the [Pipes](http://github.com/tinkerpop/pipes) library for Java provided by [Tinkerpop](http://tinkerpop.com).    
Please read the [NOTICE](/Vanaheimr/Styx/blob/master/NOTICE) file for further credits.

#### 
