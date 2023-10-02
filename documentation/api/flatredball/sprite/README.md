# sprite

### Introduction

The Sprite is a flat graphical object which is commonly used in 2D games as well as for particles, HUDs, and UIs in 3D games. The Sprite class inherits from the [PositionedObject](../../../../frb/docs/index.php) class. For information about how to use Sprites in the FlatRedBall Editor, see the [Sprite FlatRedBall page](../../../tools/glue-reference/objects/glue-reference-sprite.md).

### Sprite Scale

For information on Scale, see the [IScalable wiki entry](../../../../frb/docs/index.php). To match the on-screen pixel dimensions of a Sprite to its referenced [Texture's](../../../../frb/docs/index.php) pixel dimensions, see the [2D in FlatRedBall tutorial](../../../../frb/docs/index.php).

### Texture

Sprites can be thought of as picture frames or canvases - they define how big a displayed image will be, its position, its rotation, and so on. However, the image or picture that they display is separate from the Sprite itself. This is an important realization because this often differs from other game engines where the image and the Sprite are one and the same at runtime. For more information, see the [Sprite.Texture](../../../../frb/docs/index.php) page.

### Color and Alpha

For information on color and alpha operations (blending), see the [IColorable wiki entry](../../../../frb/docs/index.php).

### Z Buffered Sprites

Using a Z Buffer allows Sprites to properly overlap. For more information, see the [Z Buffered Sprites wiki entry](../../../../frb/docs/index.php).

### Particle Sprites

Particle Sprites are created through the [SpriteManager](../../../../frb/docs/index.php) just like other Sprites. Particle Sprites are created from a pool of Sprites that is created when the engine is first initiated. The following code creates a particle Sprite:

```
Sprite newSprite = SpriteManager.AddParticleSprite();
```

Particle Sprites have all of the same functionality as regular Sprites - in fact, they are just Sprites. The only difference is that there is minimal memory allocation and garbage collection so they can be useful when creating particle effects. Particle Sprites are used by [Emitters](../../../../frb/docs/index.php).

### Interfaces

* [FlatRedBall.Graphics.Animation.IAnimationChainAnimatable](../../../../frb/docs/index.php) - Interface defining animation properties and methods. See this entry for information on animating Sprites.
* [FlatRedBall.Math.IAttachable](../../../../frb/docs/index.php)
* [FlatRedBall.Graphics.IColorable](../../../../frb/docs/index.php) - Information on setting individual color values and the ColorOperation property.
* [FlatRedBall.Graphics.IVisible](../../../../frb/docs/index.php)

### Extra Information

* [Custom Sprite Effects](../../../../frb/docs/index.php) - Can be used for special rendering needs.
* [FlatRedBall.SpriteManager.OrderedSortType](../../../../frb/docs/index.php) - Property that controls how Sprites are sorted.
* [Inheriting from Sprite](../../../../frb/docs/index.php)
* [Sorting and Overlapping](../../../../frb/docs/index.php)
* [Changing a Sprite's Origin](../../../../frb/docs/index.php)

&#x20;