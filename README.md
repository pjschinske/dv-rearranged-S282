# Rearranged SH282

This is a Derail Valley mod that changes the [wheel arrangement](https://en.wikipedia.org/wiki/Wheel_arrangement) of the S282 locomotive.

It can convert the S282 into any of these:
- 0-8-0 Eight-wheel switcher
- 0-8-2
- 0-8-4
- 2-8-0 Consolidation
- 2-8-2 Mikado (which I've altered the look of, although you can switch it back to the vanilla look in the mod's settings)
- 2-8-4 Berkshire
- 4-8-0 Twelve Wheeler
- 4-8-2 Mountain
- 4-8-4 Northern
- 0-10-0 Ten-wheel switcher
- 0-10-2 Union
- 0-10-4
- 2-10-0 Decapod
- 2-10-2 Santa Fe
- 2-10-4 Texas
- 4-10-0 Mastodon
- 4-10-2 Southern Pacific
- 4-10-4

## How does it work?

This mod doesn't rely on Custom Car Loader. Instead, it copies and pastes parts of the existing S282 at runtime and moves them around.

I've also attempted to alter the physics of the locomotive for the different wheel arrangements, but the differences are fairly subtle.

I've also had to alter the mesh of the S282 around the firebox to get rid of some clipping with the fifth drive wheels. This mod automatically extracts the mesh from the game files using [a fork of AssetStudio](https://github.com/aelurum/AssetStudio). It then loads the mesh back into the game, alters the mesh. It then swaps out the existing S282 mesh for the altered mesh every time an S282 spawns in.

## Will you add other wheel arrangements?

The wheel arrangements listed above are the only ones I could find that looked vaguely plausible on the S282.

All others either:
- would look too strange
	- 2-6-0, 4-4-0, etc.
- would require significant clipping 
	- Anything with 12 drivers would have the rearmost drivers sticking through the cab. Shrinking the wheels would work but would require significant changes to the valve gear
- or would take too much effort to add to the game
	- e.g. 4-4-4-4, sorry Pennsy fans

## Nerdy details about the wheel arrangement names

You may not agree with the names of the wheel arrangements I've given. I've tried to give the names that were historically most commonly used in the US. **There was no official standard for the names**, so there are multiple correct names for many classes. Many railroads didn't use any of these names, instead making up their own class designations.

Some people call a 4-8-0 a Mastodon, or a 4-4-0 an Eight-wheeler, or a 4-10-2 an Overland. These names were used, but were not as common in the US as the names I've listed here. For instance, 4-8-0's were usually only called Mastodon's by the Central Pacific Railroad, and almost everyone else called them Twelve Wheelers, so I've picked the name "Twelve Wheeler".

Southern Pacific and Union Pacific both made 4-10-2's, and had different names for the class. Southern Pacific made more of them, so I'm using their designation of "Southern Pacific" rather than Union Pacific's name of "Overland".

Then we get to the names of the 0-*x*-0 classes. Some historical sources call these *x*-coupled, as in six-coupled or eight-coupled. But often they also use these names for other classes as well. So "eight-coupled" might also be used for 2-8-0's and 2-8-2's. As far as I can tell, *x*-coupled really just means "it's got *x* drive wheels, all coupled together." So a 4-8-0 is an eight-coupled, but a 4-4-4-4 isn't.

I thought about calling the 0-8-4 a "Forney eight-coupled", but this mod's 0-8-4 looks nothing like a real Forney, despite sharing the same wheel arrangement.

## License

Source code is distributed under the MIT license.
See [LICENSE](LICENSE) for more information.
