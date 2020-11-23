# Reactive Node system

UniGame Reactive Node System for Unity

## How To Install

### Unity Package Installation

Add to your project manifiest by path [%UnityProject%]/Packages/manifiest.json these lines:

```json
{
  "scopedRegistries": [
    {
      "name": "Unity",
      "url": "https://packages.unity.com",
      "scopes": [
        "com.unity"
      ]
    },
    {
      "name": "UniGame",
      "url": "http://packages.unigame.pro:4873/",
      "scopes": [
        "com.unigame"
      ]
    }
  ],
}
```
Open window Package Manager in Unity and install UniGame Nodes System Package

![](https://i.gyazo.com/d7f2f8e2125814bb1505cffb096642de.png)

## Graph UI

### How to Create Graph

![](https://i.gyazo.com/fbe45b82715d49e1a061fb4038c027da.png)

After that you already can view Graph Editor by clicking "Show Graph" button on prefab

![](https://github.com/UniGameTeam/UniGame.GameFlow/blob/master/GitAssets/open_graph.png)

## Node API

### Add Custom Node

Each graph - regular Unity Prefab Object.

Yout can create new Node woth two ways:

1. As Unity Component Asset. Create new class file with inheritance from **UniNode** base class.
When you create you node, there is several suitable methods for override

```csharp
public class DemoComponentNode : UniNode
{
    
}
```

2. Serializable Class Object. As before create you custom class, but based on **SNode** class (Serializable Node). Don't forget mark your class with **[Serializable]** attribute

```csharp
[Serializable]
public class DemoComponentNode : SNode
{
    
}
```

For any custom node your can define menu name:

```csharp
[CreateNodeMenu("Examples/DemoComponent","DemoComponent")]
public class DemoComponentNode : UniNode
{
    
}
```
    
### Base Node Methods

- Initialization. Allow you to initialize your node data, one time per whole graph lifetime
  
```csharp
protected virtual void OnInitialize(){}
```

- Execution. Custom logic for handle each 'Graph.Execute()' execution call

```csharp
protected virtual void OnExecute(){}
```

- Commands. Allow you add your custom node commands for nodes and 
separate your execution into small reusable pieces

```csharp
protected virtual void UpdateCommands(List<ILifeTimeCommand> nodeCommands){}
```

### Node Ports

#### Define Ports

After that your can define your own node ports

```csharp
public class DemoComponentNode : UniNode
{
    [Port(PortIO.Input)]
    public object inPort;

    [Port(PortIO.Output)]
    public object outPort;
}
```

Any defined port can be requested by it name

```csharp
[Serializable]
    public class DemoComponentNode : SNode
    {
        [Port(PortIO.Input)]
        public object inPort;


        protected override void OnExecute()
        {
            var port = GetPort(nameof(inPort));
        }
    }
```

Type of port field that's defined with attributes uses as type filter. Except of  **System.Object** type. Port with **System.Object** type interpret as port of any type.

```csharp
[Port(PortIO.Input)]
public object anyTypePort;

[Port(PortIO.Input)]
public int intTypePort;

[Port(PortIO.Input)]
public ISomeApi someApiTypePort;
```

Besides of attribute usage you can define new port with declaration from the code

```csharp
[Serializable]
public class DemoComponentNode : SNode
{
    protected override void UpdateCommands(List<ILifeTimeCommand> nodeCommands)
    {
        var newPort  = this.UpdatePortValue("newPost1",PortIO.Output);
        var newPort2 = AddPort("newPort2", Enumerable.Empty<Type>(), PortIO.Output);
    }
}
```

#### Receive/Publish port data

When you retrive runtime port handle, you can subscribe for it input data stream

```csharp
[Serializable]
public class DemoComponentNode : SNode
{
    [Port(PortIO.Input)]
    public int intPort;
    
    protected override void OnExecute()
    {
        var intPortHandle = GetPort(nameof(intPort));
        var portValue     = intPortHandle.Value;
        portValue.Receive<int>().
            Subscribe(x => Debug.Log($"RECEIVE INT VALUE {x}")).
            AddTo(LifeTime);
    }
    
}
```

### Nodes Info Window

### Async States

### View Port Values

At Any moment of Graph execution you can check - What kind of data that port contains?

![](https://github.com/UniGameTeam/UniGame.GameFlow/blob/master/GitAssets/port_info.png)

