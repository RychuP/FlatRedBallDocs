# Texture

### Introduction

The Sprite's Texture property controls which image the Sprite is displaying. This property is of type [Texture2D](../../../frb/docs/index.php). When a Sprite is created through the [SpriteManager's](../../../frb/docs/index.php) AddSprite method, the Sprite's Texture property gets set inside that method.

### Code Example

The following code explains how Textures are assigned. Files used:![FRB logo.png](../../../.gitbook/assets/migrated\_media-FRB\_logo.png) Add the following at class scope:

```
Sprite sprite;
```

Add the following in Initialize after initializing FlatRedBall:

```
sprite = SpriteManager.AddSprite("redball.bmp");
```

Add the following in update:

```
if (InputManager.Keyboard.KeyPushed(Keys.D1))
{
    sprite.Texture = FlatRedBallServices.Load<Texture2D>("redball.bmp");
}
else if(InputManager.Keyboard.KeyPushed(Keys.D2))
{
    sprite.Texture = FlatRedBallServices.Load<Texture2D>("FRB_logo.png");
}
```

![LogoSmall.png](../../../.gitbook/assets/migrated\_media-LogoSmall.png)
