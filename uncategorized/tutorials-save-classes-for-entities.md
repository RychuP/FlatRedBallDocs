## Introduction

The "Save" pattern provides a very powerful solution to saving information from your game. This article will expand on the Save pattern by discussing how you might create a Save class for an Entity in your game.

## What do you want to save?

A Save class can save a variety of information - and they're often used in a lot of different situations. Let's consider a few different cases:

-   Save class for an RPG character - If you are making an RPG, you may have a Character Entity. This Entity would include the actual visible representation of the character, references to AnimationChains, and perhaps some Emitters for spells and special attacks. The information you probably want to save in the Save class might be experience, equipment, and health. You will also want to keep track of any kind of information that is necessary to reconstruct the runtime Entity when needed. For example, in a game like Final Fantasy, all you may need to keep track of is the character's name. In other words, in Final Fantasy 2 US (4 Japan), you may want to keep track of your character's name (or type) as "Cecil Dark Knight" or "Kain". Then when you create the Entity you can simply use this property to select the right visible representation, or even the type you instantiate (as shown below).
-   Save class for a tool - If you are making a tool for placing objects, your Save class will need different information than the RPG example. In this case, you need information such as the position of an object. For example, if you are making a game like Super Mario Bros and you decide to create a level editor for this game, you may create CoinSave and CoinEntity classes. The CoinSave class only needs to have X and Y position. Since it is a CoinSave, your code will know to create a CoinEntity, and the CoinEntity will know how big it should be and what texture to use, and so on.
-   Save game for debugging - If you are making an advanced debugging tool, you may want to create EntitySaves that can reconstruct a scene in your game at any time. For example, you may be creating fighting game, and you are using Save classes to keep track of snapshots of your game. In this case, you may need to know information that wouldn't be important in the other scenarios listed above. Specifically, you may need to keep track of Entity velocity, current state (such as punching or kicking or blocking), current health, and any other information necessary to reconstruct the scene.

## What information do you not need?

Clearly there's a lot of information that you may want to put in a Save class for your Entities, but there is some information you almost should never save. Let's consider the Super Mario example from above. In this case, you decide to create a CoinSave class. As already mentioned, you may want to include an X and Y property in your Save class. In other words, your Save class may look like this:

    public class CoinSave
    {
        public float X;
        public float Y;

        public Coin ToEntity(string contentManagerName)
        {
            Coin coinToReturn = new Coin(contentManagerName);
            
            coin.X = this.X;
            coin.Y = this.Y;

            return coin;
        }

        public static CoinSave FromEntity(Coin coin)
        {
            CoinSave coinSaveToReturn = new CoinSave();
          
            coinSaveToReturn.X = coin.X;
            coinSaveToReturn.Y = coin.Y;

            return coinSaveToReturn;
        }
    }

Notice that we didn't store any information like Sprite information (scale, rotation), or which texture to use (such as coinImage.png), or collision information. The reason for this is because this information should be stored in the Entity itself. In other words, the size of a coin and which texture it uses should all be set in the Coin's initialization which may be hand-written or generated by Glue.

Another way to look at it is - even though a Coin does contain a Sprite, there is no need to have a SpriteSave in the CoinSave class. The settings of the Sprite will all be controlled by the Coin when it is created through its constructor.

## But what about customization?

The above rules about not including information like a SpriteSave in an EntitySave class may seem like it prevents customization. Actually, you can have as much customization as you want in your Save class - as long as your Entity supports it. It is very important that any customization you want to have should be first allowed in you Entity. In fact, the Coin example above shows this. The Coin class automatically allows its X and Y positions to be set, since it is a PositionedObject. This means that you can have any property that may be set on a PositionedObject stored in your Save class because the PositionedObject automatically exposes it.

Of course, you want to still be sure to only include properties which actually should be set on your Entity. For example, all Coins in Super Mario are oriented the same. Therefore, it's not practical to have a rotation value in your CoinSave class.

## Conditional Entity Types

Save classes can also be used to represent more general types of information. For example, if you are using inheritance in your Entities, you can have the Save class represent the base type. For example, let's say you had the following setup:

    Enemy (base Entity)
    > Goomba (inherits from Enemy)
    > Koopa (inherits from Enemy)
    > CheepCheep (inherits from Enemy)

You may want to define an enumeration for the different types of Entities you can have:

    public enum EnemyType
    {
        Goomba,
        Koopa,
        CheepCheep
    }

Your EnemySave class may have a ToRuntime as follows:

    // The EnemySave would need this defined:
    public EnemyType EnemyType;

    // Then you could have the following ToRuntime:
    public Enemy ToRuntime(string contentManager)
    {
       Enemy enemyToReturn = null;
       switch(EnemyType)
       {
           case EnemyType.Goomba:
               enemyToReturn = new Goomba(contentManager);
               break;
           case EnemyType.Koopa:
               enemyToReturn = new Koopa(contentManager);
               break;
           case EnemyType.CheepCheep:
               enemyToReturn = new CheepCheep(contentManager);
               break;
       }
       return enemyToReturn;
    }