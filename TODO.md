# Todo List

* Implement basic console
* Implement toggle menu for overlays / logic
* Run HatMouse and let's see?
*

# Parking Lot



# Fixed List

* PathFinding cache to speedup reload

* FIXED: Entering the wizard house is calling 4 more randoms than in base TAS
[01:10:23 ERROR screen_3 TASMod] 20504: Game1.random: [{seed: 19149284, index:29930}]   Frame: {seed: 19149284, index:29926}
    This was due to InternalSample being called 2x because of the event lightID checking Next(Int32.MinValue, Int32.MaxValue)
    Basically run a decrement so that it's consistent with legacy TAS/easier to trace number of RNG calls in code

* FIXED: AudioEngine nonsense
    Seems to work? tested on a mines day where I waited for music to stop playing, 
    then continued to advance for another floor, then reset (and fast reset) and no rng diff

* 
* FIXED: Blocking Saves from occuring for speed
    I'm a frame behind for the first SparklingText frame (254 on TAS, 255 on mod)
    for some reason I couldn't override the IEnumerator<int> return from SaveGame.Save()
    instead on the SaveGameMenu, wait for the frame where it was meant to 
    alternative would be a prefix False on update with a postfix rewrite of the method
