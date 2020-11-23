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

![](https://i.gyazo.com/a3bc4692e2efdd61b73933d21524aa98.png)

## Node API

## Ports

### View Port Values

At Any moment of Graph execution you can check - What kind of data that port contains?

![](https://github.com/UniGameTeam/UniGame.GameFlow/blob/master/GitAssets/port_info.png)

