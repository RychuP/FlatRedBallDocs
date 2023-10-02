# spriteeditor-camera

This article is written for the September 2007 (and later) release of the SpriteEditor.

### Creating a 2D Scene

Although the SpriteEditor is made for editing [Sprites](../../../frb/docs/index.php) it displays a 3D view by default. Many users are surprised to find that two [Sprites](../../../frb/docs/index.php) which reference [Textures](../../../frb/docs/index.php) of different sizes will by default appear to be the same size in the SpriteEditor. The reason for this is because the [Scale](../../../frb/docs/index.php) properties are dominant for determining the size of [Sprites](../../../frb/docs/index.php), not the [Sprite's](../../../frb/docs/index.php) source [Texture](../../../frb/docs/index.php). However, it is still possible to tie a [Sprite's](../../../frb/docs/index.php) size to its source [Texture](../../../frb/docs/index.php). This is the first step in creating a traditional 2D scene.

#### Pixel Size

To set the [Sprite's](../../../frb/docs/index.php) [Scale](../../../frb/docs/index.php) based on its source [Texture](../../../frb/docs/index.php), it can be manually set in the SpriteEditor through the properties window; however this can become tedious for large numbers of Sprites. Instead, the scale relative to texture size - also known as pixel size - can be set across the scene.

To access this value, click on the Window menu item -> Editor Properties. The default pixel size is 0 which tells the SpriteEditor to ignore source texture when setting [Scale](../../../frb/docs/index.php).

Since

```
ScaleX * 2 = Width 
```

and

```
ScaleY * 2 = Height
```

Then we know that if a [Sprite](../../../frb/docs/index.php) should match its [Texture's](../../../frb/docs/index.php) width and height, then its ScaleX should be half of its source [Texture's](../../../frb/docs/index.php) width and ScaleY should be half of its source [Texture's](../../../frb/docs/index.php) height. That is:

```
ScaleX = TextureWidth * .5
ScaleY = TextureHeight * .5
```

Therefore, the first step is to set the pixel size to .5.

#### Orthogonal View

When the Camera is in an orthogonal view objects are not distorted by depth. Also, in this view the bounds of the camera are constant at any distance from the camera. These to properties greatly simplify making a 2D scene.

To change the camera to an ortho view, click on the Window menu item -> Camera Properties. Find the combo box for camera view which by default displays "3D View". Change this value to "Ortho View". The up-down window controlling orthogonal width should appear. This controls the unit width of the view.

#### Displaying Camera Bounds

The last step in creating a Scene is identifying the camera bounds. To do this, click the toggle button that controls the scene camera bounds. This is in the Camera properties window accessed through the Window menu item -> Camera Properties. Pushing this button reveals the Scene Camera Properties window.

This will draw a blue outline around the visible area of the screen which can be useful when placing objects. Change the ortho property to true and observe the orthoWidth and orthoHeight properties. These can be changed to match the desired resolution.

If the camera is zoomed in the blue bounds may not be visible. Change the camera's width (in the Camera window not Scene Camera Properties) to a larger value or hold the - button down to zoom out until the bounds are visible.

![SceneCameraBounds.png](../../../media/migrated\_media-SceneCameraBounds.png)

#### Filtering

Filtering is used to blur textures and reduce the pixellated look when a [Sprite](../../../frb/docs/index.php) is viewed closely. While this can improve the appearance of Sprites in a 3D scene it can reduce the crisp look of Sprites when drawn in 2D, and also has a slight impact on performance. Filtering can be turned off for 2D scenes through the Editor Properties window which is accessed through the Window menu item -> Editor Properties.