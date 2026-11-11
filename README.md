# JReact

JReact is a collection of Unity C# helper scripts for reactive values, observable-style patterns, and common gameplay utilities.

## Features

- Reactive ScriptableObject values for common types
- Observable and observer interfaces
- Event helpers for simple callback flows
- Utility modules for Unity systems such as pooling, input, UI, scene control, localization, and more
- Helper classes for common patterns like activable objects and singletons

## Installation

Clone or copy this repository into your Unity project's `Assets` folder:

```bash
git clone https://github.com/GiacomoMariani/JReact.git Assets/JReact
```

## Dependencies

Mostly all the classes of this library use **Odin Inspector**.\
You may use Odin Inspector or remove all attributes from fields and properties.\
https://sirenix.net/odininspector

Many classes of this library use **MEC - More Effective Coroutine Pro**.\
To use those classes you may replace them with unity coroutine or use MEC Free.\
http://trinary.tech/what-is-more-effective-coroutines/

A very few classes of this library use **PrimeTween**.\
https://github.com/KyryloKuzyk/PrimeTween


You can install these packages or replace/remove the related usages where needed.

## Usage

Import the namespace in your Unity scripts:

```csharp
using JReact;
```

Example using a reactive integer:

```csharp
using JReact;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private J_ReactiveInt score;

    private void OnEnable()
    {
        score.Subscribe(OnScoreChanged);
    }

    private void OnDisable()
    {
        score.UnSubscribe(OnScoreChanged);
    }

    private void OnScoreChanged(int value)
    {
        Debug.Log($"Score: {value}");
    }
}
```

Reactive assets can be created from Unity's asset menu, for example:

```text
Create > Reactive > Basics > Reactive Int
```

## Project Structure

```text
_Basics/          Core helpers, reactive values, services, and base patterns
Events/           Event helpers
JMath/            Math and geometry utilities
Pool/             Pooling utilities
SceneControl/     Scene-related helpers
UiViewMono/       UI view helpers
Utils/            General utilities
```

## License

This project is licensed under the MIT License.
