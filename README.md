# NetDot
![NetStandard](https://img.shields.io/badge/.NET%20Standard-2.0-lightgrey.svg) ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/NetDot)
  ![Nuget](https://img.shields.io/nuget/dt/NetDot)
  ![NET workflow](https://github.com/loudenvier/NetDot/actions/workflows/dotnet.yml/badge.svg?event=push)


A (very) simple parser/(des)erializer for Dot Notation text strings. It can deserialize text into strongly typed classes and/or dictionaries+lists. 

In the remote possibility that you ever wanted to parse strings like these...

```csharp
Network.DefaultInterface=eth2
Network.Domain=dauha
Network.Hostname=BSC
WLan.eth2.Network.DefaultGateway=192.168.0.1
WLan.eth2.Network.DhcpEnable=true
WLan.eth2.Network.DnsServers[0]=192.168.0.1
WLan.eth2.Network.DnsServers[1]=0.0.0.0
WLan.eth2.Network.SubnetMask=255.255.255.0
WLan.eth2.SSID=loudenvier
PictureHttpUpload.Enable=true
PictureHttpUpload.UploadServerList[0].Address=192.168.1.225
PictureHttpUpload.UploadServerList[0].Port=7000
```

...into strongly typed objects as follows...

```csharp
public record NetworkConfig(NetworkInfo Network, WLan WLan);
public record NetworkInfo(string DefaultInterface, string Domain, string Hostname);
public record WLan(NetworkInterface eth2);
public record NetworkInterface(Network Network, string SSID);
public record Network(string DefaultGateway, bool DhcpEnable, string[] DnsServers, string SubnetMask);
```

**...then this library is for you!** 

With it all you need to do to deserialize the above text into these objects is:
```csharp
var netConfig = DotNotation.Deserialize<NetworkConfig>(textInDotNotation);
Assert.NotNull(netConfig);
Assert.Equal("0.0.0.0", netConfig.WLan.eth2.Network.DnsServers[1]);
...
```
## Motivation
[Dahua's](https://www.dahuasecurity.com) camera API/SDK uses this flavor of *dot notation* profusely in their responses (notice the entry `Network.Domain=dauha` in the example above). When I started writing code to inferface with one of their facial recognition products branded by a Brazilian company ([Intelbras](https://www.intelbras.com)) this library was born. 

I adamantly refused to work with plain strings or untyped dictionaries, so I wrote a very simple dot notation parser and hacked my way with [Newtonsoft' Json.Net](https://www.newtonsoft.com/json) to build strongly typed objects out of parsed dictionaries and lists.

## How it works

Since Json objects can be represented as a hierarchy of dictionaries (`IDictionary<string, object>`) and lists (`IList<object?>`), the library simply parses the dot notation text into such hierarchy, then uses Json.Net to serialize it to Json and finally deserializes it back into strongly typed objects. 

Deserialization is totally optional, and you can work directly with the dictionary/list hierarchy, which can be useful for dynamic scenarios. You can also leverage the fact that the `ExpandoObject` implements the `IDictionary<string, object>` interface and pass it as the root of the `Parse(string text, IDictionary<string, object> root)` method and have proper dynamic access to parsed properties and lists.

## Usage

Parsing is at the core of NetDot's functionality. You can use `DotNotation.Parse()` static method directly to parse dot notation text into a hierarchy of dictionaries and lists which can be traversed with some typecasting. Working with it can be a little awkward, so the best thing to do is to *deserialize* the text into strongly typed objects with `DotNotation.Deserialize<T>()`static method. You can also *serialize* objects to dot notation format (with many custom settings available to control de final output!) by calling `DotNotation.Serialize()` static method.

### Parsing

`DotNotation.Parse` will build a hierarchy of dictionaries and lists which can be traversed with some typecasting::
```csharp
var dict = DotNotation.Parse("""
    person.name=felipe
    person.age=47
    """);
var person = (Dictionary<string, object>)dict["person"];
Assert.Equal("felipe", person["name"]);
Assert.Equal("47", person["age"]);
```
Simple members at root level are directly accessible (making it easy to parse lines of `{name}={value}`  pairs):
```csharp
var dict = DotNotation.Parse("""
    person=felipe
    age=47
    """);
Assert.Equal("felipe", dict["person"]);
Assert.Equal("47", dict["age"]);
```

#### Arrays/lists
Arrays will become lists holding either direct values or nested dictionaries for complex objects:
```csharp
var dict = DotNotation.Parse("person[0]=felipe");
var people = (List<object?>)dict["person"];
Assert.Single(people);
Assert.Equal("felipe", people[0]); // holds a simple value
```
```csharp
var dict = DotNotation.Parse("person[0].name=felipe");
var people = dict["pessoa"] as List<object?>;
Assert.Single(pessoas);
var person = people[0] as Dictionary<string, object>; // holds a nested Dictionary<string, object>
Assert.Single(person);
Assert.Equal("felipe", person["name"]); 
```

Arrays can hold other arrays (which could also hold arrays *ad aeternum*):
```csharp
var dict = DotNotation.Parse("""
    person[0].course[0]=judo
    person[0].name=felipe
    """);
var people = dict["person"] as List<object?>;
var felipe = people[0] as Dictionary<string, object>;
var courses = felipe["course"] as List<object?>;
Assert.Single(courses);
Assert.Equal("judo", courses[0]);
```

### Deserialization

Deserialization works by first *parsing* the dot notation text into NetDot's customary hierarchy, then serializing it to JSON via Json.NET and, finally, deserializing the resulting JSON back into the proper type (again with Json.NET). This way the library leverages all amenities provided by Newtonsoft's amazingly robust, flexible and mature codebase. It works because Json itself is built as a collection of name/value pairs (dictionaries) and ordered lists of values (lists), which are incidentally equivalent to NetDot's parsed results.

#### Simple class
To deserialize a simple class `Person`:
```csharp
public class Person
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}
```
Just call `DotNotation.Deserialize<Person>(text)`:

```csharp
var person = DotNotation.Deserialize<Person>("""
    name=felipe
    age=47
    """);
Assert.Equal("felipe", person.Name);
Assert.Equal(47, person.Age);
```

Notice that Json.NET took care of matching our pascal case properties to the lowercase keys used in this dot notation text, and that it also converted `"47"` to `int`. It will always try to convert "*string*" values in dot notation into the type of the matching property.
