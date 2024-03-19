# Boosters
Here's how steam flows:
```mermaid
graph TD;
SteamEngine[Main cylinders]
ThrottleCalculator[Throttle calculator]
BoosterThrottleDummy[Booster throttle dummy]
BoosterCylinderCockOperator[Booster cylinder<br>cock operator]
BoosterIntake[Booster intake]
BoosterExhaust[Booster exhaust]
CylinderCocks[Cylinder cocks]
BoosterThrottle[Booster throttle]
    subgraph Locomotive
        Boiler-->ThrottleCalculator
        ThrottleCalculator-->SteamEngine
        SteamEngine-->Exhaust
	ThrottleCalculator--->BoosterThrottle
    end
    subgraph Tender
        BoosterThrottle-->BoosterThrottleDummy
	BoosterThrottleDummy-->BoosterIntake
        BoosterThrottleDummy-->BoosterCylinderCockOperator
        BoosterIntake-->Booster
        Booster-->BoosterExhaust
        Booster-->CylinderCocks
    end
```
Here's how air flows:
```mermaid
graph TD;
AirTank[Main air reservoir]
ReverserPilotValve[Reverser pilot valve]
ReverserPilotValveDummy[Reverser pilot<br>valve dummy]
BoosterCylinderCockOperator[Booster cylinder<br>cock operator]
    subgraph Locomotive
        AirTank-->ReverserPilotValve
	ReverserPilotValve-- Preliminary
			        Throttle -->BoosterThrottle
	ReverserPilotValve-- Clutch -->BoosterThrottle
    end
    subgraph Tender
	ReverserPilotValve-- Clutch --->ReverserPilotValveDummy
        ReverserPilotValveDummy-->BoosterCylinderCockOperator
        BoosterCylinderCockOperator-->Booster
        ReverserPilotValveDummy-->Clutch
    end
```
And here's how torque flows:
```mermaid
graph TD;
BoosterTraction[Traction]
BoosterDrivingForce[DrivingForce]
BoosterWheelslipController[WheelslipController]
    subgraph Locomotive
        SteamEngine-->Traction
        SteamEngine-->DrivingForce
        SteamEngine-->WheelslipController
    end
    subgraph Tender
        Booster-->Clutch
        Clutch-->BoosterTraction
        Clutch-->BoosterDrivingForce
        Clutch-->BoosterWheelslipController
    end
```

This isn't quite how it works in real life; we're not simulating the dome pilot valve or the preliminary throttle valve. Instead, that behavior is integrated into the `BoosterCylinderCockOperator` and the `ReverserPilotValve`.

There are also now connections between the locomotive and the tender for lubrication (taken from the mechanical lubricator), "steam quality" (I forget what this is but it's taken from the boiler) and steam temperature (taken from the firebox, because on the S282 the steam temp is the same as the fire temp).
