# NetDot
![NetStandard](https://img.shields.io/badge/.NET%20Standard-2.0-lightgrey.svg) ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/NetDot)
  ![Nuget](https://img.shields.io/nuget/dt/NetDot)
  ![NET workflow](https://github.com/loudenvier/NetDot/actions/workflows/dotnet.yml/badge.svg)


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

