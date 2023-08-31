# TODO
- double check all exploded locomotives

## Bugs
- Error in log: "Mesh.vertices is too small. The supplied vertex array has less vertices than are referenced by the triangles array."

- Hidden axles don't get reskinned (bug in Skin Manager, fix incoming on their end)
- Fourth axle spring perch clips with 6th drive wheels (won't fix)
- Drivetrain animations are synced after explosion and deexplosion (might fix later)
- Scale front axle mounts don't get scaled properly (might fix later)

## Set aside for future updates
- Moving valve gear bracket
- make blind drivers wider. PRR I-1 had 5.25" wide tires on #1 and #5, 7.25" on blind #2 and #4, 8.25" on blind #3
- add LODs and make them functional
- raise most of S282 by 0.055m?
- make `Settings` a singleton?
- give 0-8-4 bigger rear wheels?
- make exploded drivetrain rotate
- add reload functionality? [https://wiki.nexusmods.com/index.php/How_to_reload_mod_at_runtime_(UMM)](https://wiki.nexusmods.com/index.php/How_to_reload_mod_at_runtime_(UMM))

I thought about adding a new \[axle\] to the PoweredWheelsManager in \[poweredWheels\] but it doesn't seem to be used on the steam locos so I don't think I need to bother.

## Changes since last update
- Added wheel arrangements with different sized drivers:
    - 4-4-0 American
    - 4-4-2 Atlantic
    - 4-4-4 Reading (pronounced RED - ing)
    - 4-6-0 Ten-wheeler
    - 4-6-2 Pacific
    - 2-8-2 High-speed Mikado
    - 0-12-0 Twelve-wheel switcher
    - 2-12-0 Centipede
    - 2-12-2
    - 4-12-2 Union Pacific
- On the 4-8-0, 4-8-2, and 4-8-4, the drivers have been moved back and the leading wheels altered to look better
- Some other minor leading and trailing axle spacing improvements
- Ten-coupled engines now have a blind fourth driver as well (to match the PRR I-1 class)
- Vanilla trailing axle is shrunk by 10% because I like it better that way
- Brakes now appear on all drive wheels
- You can now configure which wheel arrangements automatically spawn
- Created option to allow the S282's wheels to spin when exploded
- Patched a bug in vanilla DV where the wheel sparks spawn slightly too low on the S282
### Bug fixes
- Vanilla trailing axles now turn at the correct speed
- Decreased clipping through the rails
- Got rid of strange movement when changing wheel arrangements
- Exploded locomotives now look like they should