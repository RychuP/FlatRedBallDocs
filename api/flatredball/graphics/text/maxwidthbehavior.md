# MaxWidthBehavior

### Introduction

The MaxWidthBehavior controls how the Text object displays its [DisplayText](../../../../frb/docs/index.php) when it is longer than the [MaxWidth](../../../../frb/docs/index.php). MaxWidthBehavior is an enumeration. Available values are:

* Chop
* Wrap

The default behavior is Chop.

### Code Example

The following code creates 2 Text objects. It assumes a 3D camera.

```
Text clampText = TextManager.AddText(
    "Hello I am some long text.  I need to be long so that I extend past the max width");
clampText.MaxWidth = 10;

Text wrapText = TextManager.AddText(
    "Hello I am some long text.  I need to be long so that I extend past the max width");
wrapText.MaxWidthBehavior = MaxWidthBehavior.Wrap;
wrapText.MaxWidth = 10;
wrapText.Y = -5;
```

![TextMaxWidthBehavior.PNG](../../../../.gitbook/assets/migrated\_media-TextMaxWidthBehavior.PNG)
