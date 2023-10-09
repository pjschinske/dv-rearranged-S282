# TODO

- remove gap at end of stroke for duplex valve gear
- rename dry pipes to branch pipes
- model rest of Franklin type B valve gear
- fix cylinder to crosshead bracket interface
- add rear cylinder colliders

- move cylinder water drain thing with cylinders
- copy cylinder water drip animations
- show dry pipes in exploded model
- hide front dry pipe?
- remodel front dry pipe?
- texture dry pipe mesh
    - model should be 100% done for this
- make sure duplex wheels are clocked correctly
    - should be the same front-to-back
- make WA persist across saves
    - custom save file that matches locomotive number to wheel arrangement
- fix crosshead brackets on duplexes
- Either hide all LODs or make them actually LOD
- decrease the derail threshold with # of leading/trailing axles

## Bugs
- Error in log: "Mesh.vertices is too small. The supplied vertex array has less vertices than are referenced by the triangles array."

- Hidden axles don't get reskinned (bug in Skin Manager, fix incoming on their end)
- Fourth axle spring perch clips with 6th drive wheels (won't fix)
- Drivetrain animations are synced after explosion and deexplosion (might fix later)
- Scale front axle mounts don't get scaled properly (might fix later)
- Doesn't work on Linux due to extractor not running right
    - This sometimes happens on Windows too for some reason
    - can fix if I can release assets as prefabs

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
    - 2-4-4-2
    - 4-4-4-4
    - 2-6-6-2?
### Bug fixes
