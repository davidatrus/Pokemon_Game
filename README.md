# Welcome to my pokemon recreation in Unity! For this game so far I've realy only been working on features, theres no story for this game yet but there are alot of features i've gone on to implment.  First here are the basic controls:
## Controls
Pause state= Enter
Confirm action/interact with objects/go through dialog = z 
Go back an action = x
Movement= arrow keys

## Features Implmented:
1. *Battle system*

	Fundemtal part of the game, be able to battle wild pokemon and other trainers, added fading in and out of battle scenes, different moves ( see Pokemon section for more detail) , animations for dealing and recieving damage, experience gain after pokemon has fainted. Just like in real pokemon games some npc's are trainers that will trigger battles if you walk in their FOV, this is done using ontrigger effects in unity. Defeating trainers will give player money and dialog after you defeat them. Different backgrounds for whether you are fighting on land or water. In battle system you can chose between attacking, using items from the bag, switching out pokemon or running. In trainer battles you can't run from an opponent nor can you catch their pokemon. In wild battles you can catch them or run away.  

2. *Catching mechanics*

	Ability to catch wild pokemon with a varity of different pokeballs. catch rates are implemented using pokemon's formula for catching.(See pokemon section for more detail) Each ball has different weighting of catch sucess just like in the real game. Animations for throwing a ball, attempting to catch, sucessful/unsuccessful  catches. All animations/effects done by using DOTween tool from demigiant.  To catch pokemon in wild trainer battle, go to bag, use right/left arrow keys to switch to pokeballs then select which pokeball you wish to use to catch said pokemon. 

3. *Pokemon Inventory / party screen*

	When in overworld to acess pokemon inventory press enter and click on pokemon, there you will see all your pokemon, their level and health. You can also click on a pokemon to check their summary, moves,  or change position in party Can also acess this screen in battle, where you can switch out your pokemon. On To acess summary page you can do it from freeroam state by pressing enter, clicking pokemon pressing z and going to summary. While on summary you can use the up &down arrows to switch between pokemon and the left and right arrow keys to switch between pokemon moves.
   > as of final submission   summary page is completed, however selecting on individial moves to update the description is not done or changing party position.

4. *Shop*

	Created a buy and sell mechanic, where player can buy items from shopkeepers and also sell items on their person back to the npc. The shop keeper is loacated on the house to the left of the pokemon spawn and is the person on the right. (left person is healer). Player has money to spend and will also gain money from defeating trainers.

5. *Quests*

	Implemented a Quest mechanic where players can interact with npcs to gain quests to get items/rewards. Quests can be made to make the player have to complete them before being able to progress, also can make certain items appear only once a quest has been started, in the example below the revive only pops on screen once the player has gone to talk to the npc . 
	 One quest that is in the game is talk to the npc on the house on the right, he will tell you his pokemon fainted and will ask for you to give him a revive, said revive pops up once quest has been started, return the revive to him and he will give you HM01 cut.
	 Another quest i implemented was the one that works on the feature for not allowing you to leave town, Once attempting to leave the town a trigger is activated and an npc will walk on over to you telling you that the prof wants to see you and he will take you to the house where the prof is at to finish the quest. Once you talk to the prof (npc walking around in bottom left corner) he will give you a mudkip, allowing you to progress out of the town. 

6. *Interactble overworld*

	In Pokemon, there are areas where you would need to interact with them, like surfing or cutting down trees. In my project have implemented a surfing mechanic where you go to a body of water, press z and it will prompt you if you want to surf on it or not, you can also encounter pokemon on it. Same with cutting down trees.  Also created ledges just like in original pokemon game where you can only go through it in a certain direction. In the world the short grass and water are where you can catch pokemon, to populate these areas its done in the unity editor. Attached to each scene there is a Map Area script, where you can populate which pokemon will appear, what % they appear at, what levels they can be when you encounter them. There are differnet pokemon populations for different enivormnets.
	Note by default none of your pokemon know cut, so to use cut on the tree first teach one of your pokemon cut. 

7. *Pokemon's*

	Another fundemental part of any pokemon game are the pokemon themselves, just like in the original all pokemon have different stats, tpyes, exp gained for killing them, their exp growth rate, their catch rate, learnable moves (both by level up and items) and their evolutions. So all pokemon created are scriptable objects, where you can chose their sprites type and stats. Just like in the original, some pokemon are strong and weak to certain types and that is the same in this(Check moves) Pokemon can evole by gaining experience or by using items on them to force them to evole. Small Animation played when evolving a pokemon. Pokemon can be healed/cured of status condiutions and missing health using items on them. 
	*Moves*
	Different pokemon can learn different moves, moves like the original have types and a physical/special split that use each pokemons attack/sp Attack stats. Moves also have a power and accurarcy numbers attached to them,  move priority, effects and statuses to them. Note all of these are calculated using the formulas/numbers that the original games use.  You can create moves that cause psn,burn,frz, or have secendary affects (think of a move like ember it does its base damage but there is also a % of chance it can burn you) Also can make moves that target foe/yourself for boosts  and whatnot just like in original game, you can see all of this when you are creating a move. Pokemon can only learn 4 moves and if you are gaining a new move when you level up you will be prompted if you want to keep or forget moves. When teaching moves via TMs/HMs in party screen, will tell you if the pokemon can learn said move. 
	Moves on wild/trainer pokemon are given to them by attaching the pokemon party script on them and choosing a pokemon and level for them. The pokemon will have the moves for their level on them. Move selection is chosen by randomly genarating a number between 0-3 and picking it and pokemon will use the move said number correlates too. (In future wish to implement a smart ish AI that will look to use moves that are super effective against the player, prioritize kill potienal(if pokemon in range of dying to a priority move they have use that)) 


8. *Additive Scene Loading*

	Instead of having all scenes be loaded at once, using additive scene loading, only the scene that is connected to where the player is will be loaded up and once you move areas the scenes that are directly connected will load up too.   To add connected scenes, on the chosen scene there is a scene Details script which allows you to add what scenes should be conncected.
	So How this works is I create each scene indepently, adding whatever I need to them, that consists of  the area design, houses, objects, wild grass area, npcs etc etc, then a create that scene into a prefab and have my main scene called gameplay which will call in all necessary objects & scenes to run the game. 
	
9. *Items*

	Currently implemented different items that player can use, consisting of TMs/Hms, Pokeballs, Healing items and evolution items. To see items press enter, open up the bag and you will be in the inventory screen, where you can use the left and right arrow keys to lopo between  items, pokeballs, and TMs/HMs. You can then press z to use said item and the approtiate screen will pop up. Checks in place for using items that will have no affect, message will pop up along with it. 

10. *Audio*

	To give the game a more authentic feel, added overworld music to the game, along with battle music that will switch once a player enters a battle. Also audio for hits, obtain items, catching pokemon.

11. *Save/Load* 

	You can save the game by pressing enter pressing save, then if you close and start again you can press load to load up the last instance of the game, with your pokemon(health,stats,moves), items, quest completion, battle completion, money and inventory all persisting. using this Isavable on unity forms https://forum.unity.com/threads/complex-save-system-example.897968/ and the GameDev.tv saving system. 
	*Was not able to fix loading state from main menu, page, but in game you can press enter then select load and game will load up from last save. 

12. *Dialog Manager*

	Created dialog manager that shows text to the player, this is in action in various places, like talking to npcs, the tet that comes up when starting wild encouters. 

13. *Unity Editor &  QOL improvements*

	In the unity editor, to make creating pokemons, moves items etc easier, All of these are scriptable objects and to create them in project area you can right click go to create and click what object you want to create. For Moves for exmaple you can create moves, give them a type, how much power accuracy effects etc etc. Also created a cutscene editor, which takes in my already created dialog and movement scripts for npcs and put them all in one place so its easy to create your own cutscens. Used PropertyDrawer to tidy up the inspector UI.  To access cutscene editor, you can create an empty game object and add the Cutscene script, there you will see all the different buttons to add actions that will be exectued in order of the list. 
	The different Actions that can be seleceted are, Dialog, Move, Turn, Teleport Object, Enable Object, Disable Object, NPC interact, Fade In, Fade out. 
	Dialog gives a dialog for npc to say, Turn action can turn the player or npc in direction you want them to be in(just have to click checkbox for is player or not, if not player drag whatever npc is handling the action to the scene), Move action can also move the player or an npc a set distance(used in quest of meeting the professor), Enable obj and disable object just set the gameobject to be active or not (quest for asking for a revive in quest section) Fade in fade out call in the fade in and fade out action that was created for battle screen trasnsitions, Teleport object teleports whatever object you want to certain postion( The prevent from leaving cutscene uses this after npc has taken you to profs house. NPC interact calls the npc controller class, which handles all of npc logic like giving a quest, giving items etc.  This is also used in quest creation. 
	Also using dynamicly created text boxes, these pop up when using the shop system, or acessesing the pokemon party and pressing z on a pokemon, showing the summary page. 
	
	Also used a state machine to handle logic for different game states,  made the statemachine a namespace so I can call in their functions across different files.  Statemachine  consit of an Enter, Execute and Exit functions that will handle logic for entering a state, exiting a state and executing a state. In the State class theres logic for Pushing and Popping. 
	
