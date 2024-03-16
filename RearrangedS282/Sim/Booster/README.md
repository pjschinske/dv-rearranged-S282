# Boosters
Here's how steam flows:
```mermaid
graph TD;
SteamEngine[Main cylinders]
ReverserPilotValve[Reverser pilot valve]
ThrottleCalculator[Steam chest]
BoosterCylinderCockOperator[Booster cylinder cock operator]
BoosterIntake[Booster intake]
BoosterExhaust[Booster exhaust]
CylinderCocks[Cylinder cocks]
    subgraph Locomotive
        Boiler-->Throttle-->ThrottleCalculator
        ThrottleCalculator-->SteamEngine
        SteamEngine-->Exhaust
        ThrottleCalculator-->ReverserPilotValve
    end
    subgraph Tender
        ThrottleCalculator---->BoosterIntake
        ThrottleCalculator---->BoosterCylinderCockOperator
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
BoosterCylinderCockOperator[Booster cylinder cock operator]
    subgraph Locomotive
        AirTank-->ReverserPilotValve
    end
    subgraph Tender
        ReverserPilotValve-->BoosterCylinderCockOperator
        BoosterCylinderCockOperator-->Booster
        ReverserPilotValve-->Clutch
    end
```
And here's how torque flows:
```mermaid
graph TD;
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

This isn't quite how it works in real life; I'm not simulating the dome pilot valve or the preliminary throttle valve. Instead, that behavior is integrated into the `BoosterCylinderCockOperator` and the `ReverserPilotValve`.
