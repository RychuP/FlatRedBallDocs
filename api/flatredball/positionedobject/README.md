# PositionedObject

### Introduction

The PositionedObject is an object which contains properties and functionality for position, rotation, attachment, velocity, acceleration, and instructions. It serves as the base object for common FlatRedBall classes such as the [Camera](../camera/), [Sprite](../sprite/), and [Text](../graphics/text/) objects. The PositionedObject is also the base class of [Entities](../../../glue-reference/entities/).

### FlatRedBall PositionedObject-Inheriting Classes and Associated Managers

|                                                                                          |                                                                          |
| ---------------------------------------------------------------------------------------- | ------------------------------------------------------------------------ |
| PositionedObject-Inheriting Class                                                        | Associated Manager                                                       |
| [FlatRedBall.Camera](../camera/)                                                         | [FlatRedBall.SpriteManager](../spritemanager/)                           |
| [FlatRedBall.Sprite](../sprite/)                                                         | [FlatRedBall.SpriteManager](../spritemanager/)                           |
| [FlatRedBall.Graphics.Particle.Emitter](../graphics/particle/emitter.md)                 | [FlatRedBall.SpriteManager](../spritemanager/)                           |
| [FlatRedBall.Graphics.Text](../graphics/text/)                                           | [FlatRedBall.Graphics.TextManager](../graphics/textmanager/)             |
| [FlatRedBall.Math.Geometry.AxisAlignedRectangle](../math/geometry/axisalignedrectangle/) | [FlatRedBall.Math.Geometry.ShapeManager](../math/geometry/shapemanager/) |
| [FlatRedBall.Math.Geometry.Circle](../math/geometry/circle/)                             | [FlatRedBall.Math.Geometry.ShapeManager](../math/geometry/shapemanager/) |
| [FlatRedBall.Math.Geometry.Line](../math/geometry/line/)                                 | [FlatRedBall.Math.Geometry.ShapeManager](../math/geometry/shapemanager/) |
| [FlatRedBall.Math.Geometry.Polygon](../math/geometry/polygon/)                           | [FlatRedBall.Math.Geometry.ShapeManager](../math/geometry/shapemanager/) |
| FlatRedBall.PositionedObject                                                             | [FlatRedBall.SpriteManager](../spritemanager/)                           |
| [FlatRedBall.Audio.PositionedSound](../audio/positionedsound.md)                         | [FlatRedBall.Audio.AudioManager](../audio/audiomanager/)                 |

### Position

Absolute position can be directly controlled through the X, Y, and Z properties as well as the exposed Position field. The following code creates three [Sprites](../sprite/) with various positions. Note that the default position of a PositionedObject is (0,0,0). Also, note that in the screenshot below, the Z value impacts the size of sprite3 , implying that the game is using a 3D Camera.

```csharp
 Sprite sprite1 = SpriteManager.AddSprite("redball.bmp");
 sprite1.X = 4;

 Sprite sprite2 = SpriteManager.AddSprite("redball.bmp");
 sprite2.Position = new Vector3(-4, -2, 0);

 Sprite sprite3 = SpriteManager.AddSprite("redball.bmp");
 sprite3.Y = 2;
 sprite3.Z = 14;
```

&#x20;

<figure><img src="../../../.gitbook/assets/migrated_media-PositionedSprites.png" alt=""><figcaption></figcaption></figure>

Since the Position and X/Y/Z properties are all part of the PositionedObject class, any class that inherits from the PositionedObject class (list can be viewed above in the table) has these properties. In other words, the code above uses [Sprites](../sprite/), but it could have used any PositionedObject-inheriting object to achieve the same results.

#### Z Position

The default view in FlatRedBall is down the Z axis using a perspective view. Therefore modifying the Z value of PositionedObjects can result in apparent changes in size. **FlatRedBall MDX uses a left-handed coordinate system while FlatRedBall XNA uses a right-handed coordinate system.** This means that the Z axes point the opposite direction on each version, so keep this in mind if switching from one to the other. By default, positive Z moves objects further away from the camera in FlatRedBall MDX. Positive Z moves objects closer to the camera in FlatRedBall XNA.

### Velocity and Acceleration

Velocity represents the speed at which an object is moving. Mathematically, it represents the change in position measured in units per second. While the PositionedObject itself does not apply velocity automatically, any PositionedObject-inheriting instance which is created through or added to FlatRedBall managers will have its velocity automatically applied to its position. This is the case for [Sprites](../sprite/), [Cameras](../camera/), and [Text](../graphics/text/) objects. Similarly, Acceleration modifies velocity on all objects which belong to FlatRedBall managers. It represents the rate of change of velocity in units per second. Acceleration is often used to simulate forces applied to an object such as gravity or a rocket's thrust. The following code creates two Polygons. One moves to the left while the other begins moving up and to the right, but falls downward as if affected by gravity. Although the section covering position used Sprites, this section uses Polygons to show that the same code works on many FlatRedBall types.

```csharp
 // Add the following using to qualify Polygon and ShapeManager:
 using FlatRedBall.Math.Geometry

 Polygon polygon = Polygon.CreateRectangle(3, 3);
 ShapeManager.AddPolygon(polygon); // so it is automatically managed
 polygon.XVelocity = -3;

 Polygon secondPolygon = Polygon.CreateRectangle(5, 2);
 ShapeManager.AddPolygon(secondPolygon);
 secondPolygon.Velocity = new Vector3(3, 5, 0);
 secondPolygon.Acceleration.Y = -3;
```

&#x20;

<figure><img src="../../../.gitbook/assets/migrated_media-TwoPolygons.png" alt=""><figcaption></figcaption></figure>

For more information on Velocity, see the [Velocity page](velocity.md). For more information on Acceleration, see the [Acceleration page](acceleration.md).

### Rotation

See the [IRotatable wiki entry](../math/irotatable/).

### Attachments (AttachTo)

PositionedObjects implement the IAttachable interface. For more information on attachments and the IAttachable interface, see the [IAttachable](../math/iattachable/) entry.

### PositionedObjects as IInstructable

PositionedObjects implement the [IInstructable](../instructions/iinstructable/) interface allowing them to store and execute instructions. Most PositionedObjects such as [Sprites](../sprite/) and [Text](../graphics/text/) objects have their instructions executed automatically by their corresponding managers. For more information, see the [IInstructable](../instructions/iinstructable/) page.

### Managing PositionedObjects

Most PositionedObjects like [Text objects](../graphics/text/) and [Sprites](../../../glue-reference/objects/object-types/glue-reference-sprite.md) are managed by managers as listed above. However, it is common practice to create objects which inherit from PositionedObjects. When one of these objects is instantiated it is not automatically managed, therefore fields like Velocity and Acceleration are not applied. In other words, the following code will **NOT** result in an object that moves across the screen:

```csharp
PositionedObject myObject = new PositionedObject();
myObject.XVelocity = 1; // this won't be applied until added to the SpriteManager
```

The SpriteManager can also perform common management on PositionedObjects. Therefore, adding the following line will result in the object being managed by the SpriteManager and exhibiting all expected behavior:

```csharp
SpriteManager.AddPositionedObject(myObject);
```

To remove an object that has been added this way from management, call:

```csharp
SpriteManager.RemovePositionedObject(myObject);
```
