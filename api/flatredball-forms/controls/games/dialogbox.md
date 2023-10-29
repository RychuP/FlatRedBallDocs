# DialogBox

The DialogBox control is used to display dialog to the screen. It provides a number of common dialog functionality including:

* Typewriter (letter by letter) display of the text
* Multi-page display using an IEnumerable
* Task (async) support for logic after
* Force display of entire page and page advance input
* Input support using keyboard, mouse, and gamepads

<figure><img src="../../../../../.gitbook/assets/28_17_07_29 (1).gif" alt=""><figcaption></figcaption></figure>

### Implementation Example

Like other FlatRedBall.Forms controls, the easiest way to create a DialogBox is to add a DialogBox instance into your screen. By default dialog boxes are visible, but you may want to mark yours as invisible in your Gum screen so it doesn't display in game until you need it to display. For most games only a single DialogBox instance is needed unless you intend to have multiple dialog boxes displayed at the same time.

<figure><img src="../../../../../.gitbook/assets/image (8).png" alt=""><figcaption></figcaption></figure>

To display a dialog box, use one of the Show methods. The simplest is to call Show with a string, as shown in the following code:

```
void CustomActivity(bool firstTimeCalled)
{
    if(InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Enter))
    {
        var dialogBox = Forms.DialogBoxInstance;
        dialogBox.Show("Hello, I am a dialog box. Let's make same games!");
    }
}
```

<figure><img src="../../../../../.gitbook/assets/28_17_48_03.gif" alt=""><figcaption></figcaption></figure>

Alternatively, multiple pages can be displayed using an IEnumerable such as a string array as shown in the following code snippet:

```
var dialogBox = Forms.DialogBoxInstance;

var pages = new string[]
{

    "Let me tell you why FlatRedBall.Forms is so great:",
    "It has tons of functionality out-of-the-box.",
    "You can do all of your layouts visually in Gum.",
    "It has MVVM support.",
    "Its appearance can be fully customized too!"
};

dialogBox.Show(pages);
```

<figure><img src="../../../../../.gitbook/assets/28_17_53_02.gif" alt=""><figcaption></figcaption></figure>

### ShowAsync for async Programming

The ShowAsync method returns a task which can be used to await for all pages to be shown and for the final page to be dismissed. A common usage of ShowAsync is in a scripted sequence. For example, a scripted sequence may combine dialog and player movement. Since the player can choose when to advance text, the amount of time that a DialogBox is displayed must be awaited. The following shows how code might be used to implement a scripted sequence which combines dialog being displayed and player movement.

```
var dialogBox = Forms.DialogBoxInstance;
await dialogBox.ShowAsync("Hello? Who is there?");
await MovePlayerTo(100, 50);
await dialogBox.ShowAsync("Oh, the room is empty, but I thought I heard a noise.");
await MovePlayerTo(300, 80);
await dialogBox.ShowAsync("No one is here either. Am I hearing things?");
```

### DialogBox Input

DialogBox responds to input and can respond to two types of input: c_onfirm_ input and _cancel_ input.

Confirm input performs the following actions:

* If dialog is printing out character-by-character, the entire page is immediately displayed
* If the entire page is displayed and the DialogBox has more pages to display, the page is cleared and the next page begins displaying
* If the entire page is displayed and the DialogBox has no more pages to display, the dialog box is dismissed

Cancel input performs the following actions:

* If dialog is printing out character-by-character, the entire page is immediately displayed
* If the entire page is displayed and the DialogBox has more pages to display, the page is cleared and the next page is displayed in its entirety
* If the entire page is displayed and the DialogBox has no more pages to display, the dialog box is dismissed

In other words, confirm and cancel input behave the same except that cancel immediately prints out the next page, giving players the choice to skip letter-by-letter display.

Dialog can be advanced with Mouse, Keyboard, Xbox360GamePad, and a custom Func predicate named AdvancePageInputPredicate. Note that if a DialogBox has a non-null AdvancePageInputPredicate, then all other forms of input are ignored. This allows games to fully customize a DialogBox's page advance logic.

#### Mouse Input

The Mouse can only perform confirm input. If a dialog is clicked, then its confirm action is executed.

#### Keyboard Input

The Keyboard's IInputDevice implementation is used for confirm and cancel actions:

* Space (DefaultPrimaryActionInput) is used to confirm
* Escape (DefaultCancelInput) is used to cancel

Note that the keyboard actions will only apply if the DialogBox has focus. For example, the following code shows how to give a DialogBox focus:

```
var dialogBox = Forms.DialogBoxInstance;
dialogBox.IsFocused = true;
dialogBox.Show("Press space or ESC to perform confirm or cancel actions, respectively");
```

**Xbox360GamePad Input**

Xbox360GamePads can be used to advance dialog. A DialogBox must have focus for the Xbox360GamePads to advance dialog, just like the Keyboard. Furthermore, the desired Xbox360GamePads must be added to the GuiManager's GamePadsForUiControl as shown in the following code:

```
GuiManager.GamePadsForUiControl.Add(InputManager.Xbox360GamePads[0]);
```

For more information, see the [GuiManager.GamePadsForUiControl](https://flatredball.com/documentation/api/flatredball/flatredball-gui/flatredball-gui-guimanager/gamepadsforuicontrol/) page.

Once gamepads are added, the dialog box can be shown and focused just like in the example above for Keyboard input.

#### AdvancePageInputPredicate

To customize advance behavior, the AdvancePageInputPredicate delegate can be used to control DialogBox advancement. This method can be used to advance dialog box behavior using custom input or other conditions such as the completion of a tutorial. DialogBoxes must have focus or their AdvancePageInputPredicate will not apply.

The following code shows how to advance the page on a secondary click. Note that this code does not perform any additional logic, such as whether the cursor is over the DialogBox. This means that right-clicking anywhere on the screen advances the dialog.

```
void CustomInitialize()
{
    Forms.DialogBoxInstance.AdvancePageInputPredicate = AdvanceOnSecondaryClick;
}

private bool AdvanceOnSecondaryClick()
{
    return GuiManager.Cursor.SecondaryClick;
}
```

As mentioned above, assigning AdvancePageInputPredicate prevents all other default page advance logic, so the user will not be able to advance the dialog with the keyboard, gamepads, or with a left-click on the DialogBox.