# Bumble - Dev Log

Given that I am using this GitHub as a repository for each of my updates, the readme seems a good place to document and store my thought process with regards to future updates. These may be anything from links to sketches of how i want the UI or certain assets to look, my decision process behind why certain mechanics are the way they are, or even just a runoff of ideas I had at 4AM when I couldn't sleep. 

## 17/09/16

NOTE: I try to stray from explicitly giving exact values to variables, as the majority of the game hase yet to be balanced and they are almost guaranteed to change once the features are implemented. As such, any explicit values are placeholders.

### Enemy Spawning
Enemies need to spawn offscreen, so the player doesn't see them pop into existence. Originally it seemed like the easiest way to guarantee this is to make them appear off the edge of the camera at maximum zoom, regardless of zoom. While a hive made of concentric rings would allow for a hive-size based calculation, I don't want to restrict the player's expansion of their colony unless I need to. It may turn out that making an oddly shaped hive is effective, that's half the fun of giving people the freedom to be creative.

The only issue is that being able to move the camera would make it possible to push enemy spawn locations further and further away from the hive, so an alternative method would be to pick an xy co-ordinate and raycast from that point towards 0,0 until it hits something. If it hits something within a minimum distance, move it x amount further away and try again. If it would be spawning within the bounds of the camera, move it just past the edge of view. This seems inefficient however, and may need improving / replacing in the long run.

### Spawn Alerts

Since the enemies would be appearing off the edge of what the player can see, the game needs to alert players that they have spawned. The simplest implementation would be an icon along the edge from which it is approaching. This can easily be achieved using a UI object that derives it's position from the enemy's location. A line between the enemy and the camera position would pass through the end of the screen, and that's the point at which to place the icon.

### Guards, Combat, Enemy Health

Guards need to be able to attack or they aren't very good at their job. They attack much like you'd expect a bee to: by stinging. There is a time delay between their attacks (1 second?) so they don't machine-gun sting things.

What happens when the guard stings an enemy depends on the enemy type. There are two main categories of enemy, large and small. Small enemies have a 1 in 3 chance of killing the guard when stung, to represent the enemy retaliating. Large enemies are guaranteed to kill the guard, for the reason bees die when they sting humans.

Enemy health is tracked in the form of how many stings it takes to bring the creature down. The weakest enemy, most likely going to be a wasp, would only require two stings to kill, so a single guard nest is sufficient to deal with it.

Enemies do not attack the bees directly, instead going for the hive itself. They will either go for the queen, who sits in the center of the hive, or honey stores, depending on which is closest. This does mean I will have to keep a list of honey stores so i can check when an enemy spawns what's closest to them. If an enemy's path to their target is blocked, they will simply attack whatever cell is in their way until it is destroyed. This will either become an empty or typeless cell, I haven't yet decided. The only real difference is 200 wax, which in the long run becomes a minor amount but in the short term may be the difference between a mild inconvience and a nuisance that slows the game down.

  **Note to self** - *On the subject of the queen, she needs to be implemented as a separate gameObject rather than another cell type, as her cell is unique and functions differently to any of the buildable cells.*
  
### Cell Damage & Repair
Cells will need to be given health, which is simple enough to implement, although it means more to balance. Yay. If a cell is partially damaged, it would need to be repaired, as dropping below 50% health disables a cell until *fully* repaired. (Guards are an exception to this rule, and won't be disabled unless destroyed as the cell represents capacity not count) The cost of repair would be in the form of wax, as a % of the original cell cost based on what % damaged the cell was, rounded to the nearest int. It's worth nothing that most cell types cost food to build, as wax is only used to build the typeless cell. However, wax has (currently) triple the value of food, so the conversion is simple enough. *e.g. a Forager nest is 8% damaged. 8% of the 250 food cost is 20 food, or 6.66 wax. So it would cost 7 wax to repair.*
  
Since cost of repair scales off cost, cell *health* should also scale off the cost of the cell, and enemy damage should be in absolute numbers rather than percentage based to not make said scaling redundant.

Cell damage allows for the introduction of yet another type of bee: *the Nurse*. The Nurse bees act as dedicated repair teams, patching up damaged cells and ensuring new bees are hatched to replace the ones lost during an attack. If no nurses are available, the role falls to the builders, meaning wax production slows, exacerbating the repair cost. Repairing is prioritised over building, just as nurses are prioritised for said repairs over builders.
  
Nurses are as a result technically the first optional bee type, as their role can be filled by an existing one. However, while they do not produce wax, nurse nests provide two nurses for the cost of a single food/sec, meaning there is a balance to be struck

**example** - *If you have 5 cells, 3 builders and 2 damaged, until the repairs are complete, you are only producing 1 wax/sec. However, if you have 2 builder nests and a single nurse nest, that single nest can be repairing both damaged cells, meaning you're producing 2 wax/sec. Of course, if 0 or 1 cell was damamged, the builder setup would be either equally effective, or moreso. And this is where the balance comes from. Do you slow your production slightly to avoid it being slowed to a greater extent later on, or do you take the risk and go full production?*

### Tooltips

The current bee type tooltips are placeholders, and will most likely be moved from the labels to the buttons used to create the nests for said types. The label will instead be updated to show x/y, where y is the total number of that bee type and x is the number currently fulfilling their role.

On hover, the tooltip would display the reasons for unavailabilities, be it a damaged nest, new bees being spawned, etc. For builders this would also list any that are currently allocated to repairs as unavailable, since their primary role is wax production.

##21/09/16

###Needs More Testing

It's 4AM, and 90% of this last commit has been bugfixing, because it turned out there was an oversight in some of the very first code I wrote. But this was only caused by a set of inputs that came from cell destruction, so it seemed like the new code was working and I had to work my way from the top down to it. Turns out a single value had been changed on a raycast, and it should have been easy to catch. Clearly I didn't test it as thoroughly as I thought.

Either way, it has been fixed, and I can move on to sorting out enemy spawning so the player actually has time to build guards.
