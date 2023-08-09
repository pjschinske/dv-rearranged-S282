# TODO

- add reload functionality [https://wiki.nexusmods.com/index.php/How_to_reload_mod_at_runtime_(UMM)](https://wiki.nexusmods.com/index.php/How_to_reload_mod_at_runtime_(UMM))

I thought about adding a new \[axle\] to the PoweredWheelsManager in \[poweredWheels\] but it doesn't seem to be used on the steam locos so I don't think I need to bother.

## Bugs
- Exploded body is not rotated
- [WheelRearranger] might add leading and trailing axles to all exploded models, regardless of previous wheel arrangement
    - IDK how but I don't think this happens
- Can spawn with big trailing axle, even though it's not selected in the settings
    - Sometimes happens when I spawn locomotives with comms radio
- Setting save is broken somehow
    - A reboot fixed this :thinking_emoji:

## Features
- save wheel arrangement in save file
- modify hunting
- alter mesh around firebox for 10-coupleds (sigh)
- move colliders in `[bogies]`

## Config options
- x-8-2 trailing truck:
    - vanilla
    - small wheels
    - small wheels, alternate position

- x-10-2 trailing truck:
    - vanilla
    - small wheels

- Spawn settings:
    - Spawn with vanilla wheel arrangement
    - Spawn with random wheel arrangement