# IAttachable

### Introduction

IAttachables provide properties and methods to control attachments.

### What are attachments?

An attachment is a relationship between two [PositionedObjects](../../../../frb/docs/index.php) in which one is identified as the parent object and one as the child object. The child object's absolute rotation and position values become read-only and its position and rotation are controlled by its parent's absolute position and rotation values as well as its own relative position and rotation values. The following examples highlight some of the uses of attachments:

* Relatively positioned objects: There are certain objects which should always be positioned relative to another object, like wings on an airplane. It doesn't matter if the airplane is positioned at x = 0 or 50, nor does it matter if the plane is rotated. The wings should always appear in the same relative position to the airplane. Furthermore, we would like the wings to move with the airplane, so that we don't have to readjust their position every frame.
* Orbits: Attached [PositionedObjects](../../../../frb/docs/index.php) do not have to be touching for the attachment to be valid. We could easily have one object orbit around another by attaching the orbiting [PositionedObject](../../../../frb/docs/index.php) to an invisible, spinning [PositionedObject](../../../../frb/docs/index.php) positioned at the center of the orbit.

### Relative Values

The relative values in an IAttachable are effective **only if an IAttachable has a parent**. Otherwise they sit idle and do nothing to the IAttachable.

### Creating Attachments

The following code creates two [Sprites](../../../../frb/docs/index.php). The larger [Sprite](../../../../frb/docs/index.php) is the parent [Sprite](../../../../frb/docs/index.php). The smaller [Sprite](../../../../frb/docs/index.php) is the child [Sprite](../../../../frb/docs/index.php) which is attached to the parent [Sprite](../../../../frb/docs/index.php). Notice that attachments are created through the child and not the parent.

```
// Replace your Initialize method with the following:
protected override void Initialize()
{
    FlatRedBallServices.InitializeFlatRedBall(this, this.graphics);

    Sprite parentSprite = SpriteManager.AddSprite("redball.bmp");
    parentSprite.ScaleX = 3;
    parentSprite.ScaleY = 3;

    parentSprite.CustomBehavior += PositionSpriteAtCursor;

    Sprite childSprite = SpriteManager.AddSprite("redball.bmp");
    // Set the absolute position before attaching childSprite to 
    // parentSprite.
    childSprite.X = 4;

    childSprite.AttachTo(parentSprite, true);

    base.Initialize();
}

// The following method is used to reposition the parent Sprite.
void PositionSpriteAtCursor(Sprite sprite)
{
   // Since our Sprites exist in a 3D world, the WorldXAt and 
   // WorldYAt methods require a Z value.
   sprite.X = InputManager.Mouse.WorldXAt(0);
   sprite.Y = InputManager.Mouse.WorldYAt(0);
}
```

![ChildAndParentSprite.png](../../../../.gitbook/assets/migrated\_media-ChildAndParentSprite.png)

### AttachTo Method

The AttachTo method creates a child-parent relationship between to IAttachables. The child IAttachable creates the relationship by calling AttachTo and passing the parent as the first argument. A parent can have any number of children, but a child can only have one parent. Calling AttachTo a second time breaks the first child-parent relationship and creates a new attachment between the calling instance and the instance passed as the first argument to the AttachTo method. The second argument determines whether relative values change, which also determines whether absolute values remain the same before and after the AttachTo call. For more information see the [AttachTo page](../../../../frb/docs/index.php).

#### Relative Positioning

All IAttachables have relative versions of position, velocity, acceleration, rotation, and rotational velocity. Relative properties are named the same as their absolute counterparts prefixed with the word "Relative". If the IAttachable is a [PositionedObject](../../../../frb/docs/index.php) as is the case with [Sprites](../../../../frb/docs/index.php), [Cameras](../../../../frb/docs/index.php), [Text objects](../../../../frb/docs/index.php), and [Emitters](../../../../frb/docs/index.php), then the following relationships hold:

|                     |                           |
| ------------------- | ------------------------- |
| "Absolute" Property | "Relative" Property       |
| X                   | RelativeX                 |
| Y                   | RelativeY                 |
| Z                   | RelativeZ                 |
| XVelocity           | RelativeXVelocity         |
| YVelocity           | RelativeYVelocity         |
| ZVelocity           | RelativeZVelocity         |
| XAcceleration       | RelativeXAcceleration     |
| YAcceleration       | RelativeYAcceleration     |
| ZAcceleration       | RelativeZAcceleration     |
| RotationX           | RelativeRotationX         |
| RotationY           | RelativeRotationY         |
| RotationZ           | RelativeRotationZ         |
| RotationXVelocity   | RelativeRotationXVelocity |
| RotationYVelocity   | RelativeRotationYVelocity |
| RotationZVelocity   | RelativeRotationZVelocity |

#### Remember, child absolute values are read-only

As mentioned before, if a [PositionedObject](../../../../frb/docs/index.php) has a parent, then its absolute values are read only. For example, consider the X (or Position.X) value. If an object has a parent, then its X position (ignoring rotation) is:

```
child.X = parent.X + object.RelativeX;
```

Therefore, if your code sets the X value, it will just get overwritten before rendering by the above code. This is the same for:

* position
* velocity
* acceleration
* rotation
* rotational velocity.

#### Child absolute velocity and acceleration

While absolute position and rotation values are read-only, velocity and acceleration values should not be used at all. Let's examine why velocity is not usable. As mentioned above, attachments result in the absolute position being set every frame according to the Parent's absolute position and the child's relative position. That means that the Position variable is reset every frame. But the Velocity variable also changes the variable every frame. That means that both attachments and Velocity will modify the absolute position of your object. So what happens if you have a non-zero Velocity? The Velocity will add itself (considering time) every frame, but just before drawing, attachments will set the Position property - overriding the changes that occurred earlier in the frame when Velocity was applied. Reading Velocity can also be misleading because velocity is not by default a "reactive" property. That means that if an object is moved by its Parent, the Velocity property will not automatically update itself according to the movement performed due to the attachment. However, making velocity reactive is possible using ["Real" values](../../../../frb/docs/index.php#Real\_Velocity\_and\_Acceleration).

### "Changing the center" of a [PositionedObject](../../../../frb/docs/index.php)

By default all [PositionedObjects](../../../../frb/docs/index.php) are positioned at their center (the exception to this is the [Text object](../../../../frb/docs/index.php)). This can be changed using attachments. First, let's look at the default center and points of rotation. We'll use a [Sprite](../../../../frb/docs/index.php) to show how this works ([Sprite](../../../../frb/docs/index.php) inherits from [PositionedObject](../../../../frb/docs/index.php) which implements IAttachable.

```
Sprite sprite = SpriteManager.AddSprite("redball.bmp");
sprite.ScaleX = 5;
sprite.ScaleY = 5;
sprite.RotationZVelocity = 1;
```

![RotatingSprite.png](../../../../.gitbook/assets/migrated\_media-RotatingSprite.png) As expected the [Sprite](../../../../frb/docs/index.php) rotates about its center. To change the point that the [Sprite](../../../../frb/docs/index.php) rotates about, we need to use an additional [PositionedObject](../../../../frb/docs/index.php). We'll add a simple [PositionedObject](../../../../frb/docs/index.php), attach the [Sprite](../../../../frb/docs/index.php) to it, then make the [PositionedObject](../../../../frb/docs/index.php) rotate. Notice that we must add the [PositionedObject](../../../../frb/docs/index.php) to the [SpriteManager](../../../../frb/docs/index.php) or else the RotationZVelocity member will not apply automatically.

```
PositionedObject anchor = new PositionedObject();
SpriteManager.AddPositionedObject(anchor);
// Position the anchor where we want the rotation point
// This will put it at the top-left of the Sprite
anchor.X = -5;
anchor.Y = 5;
anchor.RotationZVelocity = 1;

Sprite sprite = SpriteManager.AddSprite("redball.bmp");
sprite.ScaleX = 5;
sprite.ScaleY = 5;
// We no longer make the Sprite rotate - the parent rotates
// We want to "change relative" so that the Sprite's absolute
// values remain the same:
bool changeRelative = true;
sprite.AttachTo(anchor, changeRelative);
```

![NewRotationPoint.png](../../../../.gitbook/assets/migrated\_media-NewRotationPoint.png)

**Stop right there!** The code shown above for attaching is as brief as possible and simply shows the raw syntax behind attachments. However, you may be using Glue, and if so you shouldn't be writing the code above. Instead, you will likely have the above situation mostly set up - the PositionedObject will be the Entity itself and the Sprite will be a Sprite (or Entire Scene) Object in Glue. Instead of doing this in code, you should simply offset the Sprite in the SpriteEditor. Then when the Entity rotates, the Sprite will rotate about an offset naturally.

### IAttachable Members

* [FlatRedBall.Math.IAttachable.Detach](../../../../frb/docs/index.php)
* [FlatRedBall.Math.IAttachable.ParentRotationChangesPosition](../../../../frb/docs/index.php)
* [FlatRedBall.Math.IAttachable.ParentRotationChangesRotation](../../../../frb/docs/index.php)
* [FlatRedBall.Math.IAttachable.TopParent](../../../../frb/docs/index.php)

### Additional Information

* [FlatRedBall.Math.IAttachable:Attachment Updates in the Engine](../../../../frb/docs/index.php)
* [Two-way relationships](../../../../frb/docs/index.php#Two\_Way\_Relationships) - Explains how two-way relationships work between IAttachables and the lists they belong to.

Did this article leave any questions unanswered? Post any question in our [forums](../../../../frb/forum.md) for a rapid response.
