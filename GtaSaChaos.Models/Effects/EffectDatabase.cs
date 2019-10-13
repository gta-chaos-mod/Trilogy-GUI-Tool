// Copyright (c) 2019 Lordmau5
using System;
using System.Collections.Generic;
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Effects.extra;
using GtaChaos.Models.Effects.impl;
using GtaChaos.Models.Utils;

namespace GtaChaos.Models.Effects
{
    public static class EffectDatabase
    {
        public static List<AbstractEffect> Effects { get; } = new List<AbstractEffect>
        {
            new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 1", "ThugsArmoury", "weapon_set_1"), // Weapon Set 1
            new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 2", "ProfessionalsKit", "weapon_set_2"), // Weapon Set 2
            new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 3", "NuttersToys", "weapon_set_3"), // Weapon Set 3
            new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 4", "MinigunMadness", "weapon_set_4"), // Weapon Set 4
            new FunctionEffect(Category.WeaponsAndHealth, "Health, Armor, $250k", "INeedSomeHelp", "health_armor_money"), // Health, Armor, $250k
            new FunctionEffect(Category.WeaponsAndHealth, "Suicide", "GoodbyeCruelWorld", "suicide").DisableRapidFire(), // Suicide
            new FunctionEffect(Category.WeaponsAndHealth, "Infinite Ammo", "FullClip", "infinite_ammo"), // Infinite ammo
            new FunctionEffect(Category.WeaponsAndHealth, "Infinite Health (Player)", "NoOneCanHurtMe", "infinite_health"), // Infinite Health (Player)

            new FunctionEffect(Category.WantedLevel, "Wanted Level +2 Stars", "TurnUpTheHeat", "wanted_plus_two"), // Wanted level +2 stars
            new FunctionEffect(Category.WantedLevel, "Clear Wanted Level", "TurnDownTheHeat", "clear_wanted"), // Clear wanted level
            new FunctionEffect(Category.WantedLevel, "Never Wanted", "IDoAsIPlease", "never_wanted"), // Never wanted
            new FunctionEffect(Category.WantedLevel, "Six Wanted Stars", "BringItOn", "wanted_six_stars").DisableRapidFire(), // Six wanted stars

            new WeatherEffect("Sunny Weather", "PleasantlyWarm", 1), // Sunny weather
            new WeatherEffect("Very Sunny Weather", "TooDamnHot", 0), // Very sunny weather
            new WeatherEffect("Overcast Weather", "DullDullDay", 4), // Overcast weather
            new WeatherEffect("Rainy Weather", "StayInAndWatchTV", 16), // Rainy weather
            new WeatherEffect("Foggy Weather", "CantSeeWhereImGoing", 9), // Foggy weather
            new WeatherEffect("Thunderstorm", "ScottishSummer", 16), // Thunder storm
            new WeatherEffect("Sandstorm", "SandInMyEars", 19), // Sand storm

            new FunctionEffect(Category.Spawning, "Get Parachute", "LetsGoBaseJumping", "get_parachute"), // Get Parachute
            new FunctionEffect(Category.Spawning, "Get Jetpack", "Rocketman", "get_jetpack"), // Get Jetpack
            new SpawnVehicleEffect("TimeToKickAss", 432), // Spawn Rhino
            new SpawnVehicleEffect("OldSpeedDemon", 504), // Spawn Bloodring Banger
            new SpawnVehicleEffect("DoughnutHandicap", 489), // Spawn Rancher
            new SpawnVehicleEffect("NotForPublicRoads", 502), // Spawn Hotring A
            new SpawnVehicleEffect("JustTryAndStopMe", 503), // Spawn Hotring B
            new SpawnVehicleEffect("WheresTheFuneral", 442), // Spawn Romero
            new SpawnVehicleEffect("CelebrityStatus", 409), // Spawn Stretch
            new SpawnVehicleEffect("TrueGrime", 408), // Spawn Trashmaster
            new SpawnVehicleEffect("18Holes", 457), // Spawn Caddy
            new SpawnVehicleEffect("JumpJet", 520), // Spawn Hydra
            new SpawnVehicleEffect("IWantToHover", 539), // Spawn Vortex
            new SpawnVehicleEffect("OhDude", 425), // Spawn Hunter
            new SpawnVehicleEffect("FourWheelFun", 471), // Spawn Quad
            new SpawnVehicleEffect("ItsAllBull", 486), // Spawn Dozer
            new SpawnVehicleEffect("FlyingToStunt", 513), // Spawn Stunt Plane
            new SpawnVehicleEffect("MonsterMash", 556), // Spawn Monster
            new SpawnVehicleEffect("SurpriseDriver", -1), // Spawn Random Vehicle

            // They reset if a mission is passed / failed
            new FunctionEffect(Category.Time, "0.25x Game Speed", "MatrixMode", "quarter_gamespeed"), // Quarter Gamespeed
            new FunctionEffect(Category.Time, "0.5x Game Speed", "SlowItDown", "half_gamespeed"), // Half Gamespeed
            new FunctionEffect(Category.Time, "2x Game Speed", "SpeedItUp", "double_gamespeed"), // Double Gamespeed
            new FunctionEffect(Category.Time, "4x Game Speed", "YoureTooSlow", "quadruple_gamespeed"), // Quadruple Gamespeed
            new FunctionEffect(Category.Time, "Always Midnight", "NightProwler", "always_midnight"), // Always midnight
            new FunctionEffect(Category.Time, "Stop Game Clock", "DontBringOnTheNight", "stop_game_clock"), // Stop game clock, orange sky
            new FunctionEffect(Category.Time, "Faster Clock", "TimeJustFliesBy", "faster_clock"), // Faster clock

            new FunctionEffect(Category.VehiclesTraffic, "Blow Up All Cars", "AllCarsGoBoom", "blow_up_all_cars").DisableRapidFire(), // Blow up all cars
            new FunctionEffect(Category.VehiclesTraffic, "Pink Traffic", "PinkIsTheNewCool", "pink_traffic"), // Pink traffic
            new FunctionEffect(Category.VehiclesTraffic, "Black Traffic", "SoLongAsItsBlack", "black_traffic"), // Black traffic
            new FunctionEffect(Category.VehiclesTraffic, "Cheap Cars", "EveryoneIsPoor", "cheap_cars"), // Cheap cars
            new FunctionEffect(Category.VehiclesTraffic, "Expensive Cars", "EveryoneIsRich", "expensive_cars"), // Expensive cars
            new FunctionEffect(Category.VehiclesTraffic, "Insane Handling", "StickLikeGlue", "insane_handling"), // Insane handling
            new FunctionEffect(Category.VehiclesTraffic, "All Green Lights", "DontTryAndStopMe", "all_green_lights"), // All green lights
            new FunctionEffect(Category.VehiclesTraffic, "Cars On Water", "JesusTakeTheWheel", "cars_on_water"), // Cars on water
            new FunctionEffect(Category.VehiclesTraffic, "Boats Fly", "FlyingFish", "boats_fly"), // Boats fly
            new FunctionEffect(Category.VehiclesTraffic, "Cars Fly", "ChittyChittyBangBang", "cars_fly"), // Cars fly
            new FunctionEffect(Category.VehiclesTraffic, "Smash N' Boom", "TouchMyCarYouDie", "smash_n_boom"), // Smash n' boom
            new FunctionEffect(Category.VehiclesTraffic, "All Cars Have Nitro", "SpeedFreak", "all_cars_have_nitro"), // All cars have nitro
            new FunctionEffect(Category.VehiclesTraffic, "Cars Float Away When Hit", "BubbleCars", "cars_float_away_when_hit"), // Cars float away when hit
            new FunctionEffect(Category.VehiclesTraffic, "All Taxis Have Nitrous", "SpeedyTaxis", "all_taxis_have_nitro"), // All taxis have nitrous
            new FunctionEffect(Category.VehiclesTraffic, "Invisible Vehicles (Only Wheels)", "WheelsOnlyPlease", "wheels_only_please"), // Invisible Vehicles (Only Wheels)

            new FunctionEffect(Category.PedsAndCo, "Peds Attack Each Other", "RoughNeighbourhood", "peds_attack_each_other"), // Peds attack other (+ get golf club)
            new FunctionEffect(Category.PedsAndCo, "Have A Bounty On Your Head", "StopPickingOnMe", "have_a_bounty_on_your_head"), // Have a bounty on your head
            new FunctionEffect(Category.PedsAndCo, "Elvis Is Everywhere", "BlueSuedeShoes", "elvis_is_everywhere"), // Elvis is everywhere
            new FunctionEffect(Category.PedsAndCo, "Peds Attack You", "AttackOfTheVillagePeople", "peds_attack_you"), // Peds attack you
            new FunctionEffect(Category.PedsAndCo, "Gang Members Everywhere", "OnlyHomiesAllowed", "gang_members_everywhere"), // Gang members everywhere
            new FunctionEffect(Category.PedsAndCo, "Gangs Control The Streets", "BetterStayIndoors", "gangs_control_the_streets"), // Gangs control the streets
            new FunctionEffect(Category.PedsAndCo, "Riot Mode", "StateOfEmergency", "riot_mode"), // Riot mode
            new FunctionEffect(Category.PedsAndCo, "Everyone Armed", "SurroundedByNutters", "everyone_armed"), // Everyone armed
            new FunctionEffect(Category.PedsAndCo, "Aggressive Drivers", "AllDriversAreCriminals", "aggressive_drivers"), // Aggressive drivers
            new FunctionEffect(Category.PedsAndCo, "Recruit Anyone (9mm)", "WannaBeInMyGang", "recruit_anyone_9mm"), // Recruit anyone (9mm)
            new FunctionEffect(Category.PedsAndCo, "Recruit Anyone (AK-47)", "NoOneCanStopUs", "recruit_anyone_ak47"), // Recruit anyone (AK-47)
            new FunctionEffect(Category.PedsAndCo, "Recruit Anyone (Rockets)", "RocketMayhem", "recruit_anyone_rockets"), // Recruit anyone (Rockets)
            new FunctionEffect(Category.PedsAndCo, "Ghost Town", "GhostTown", "ghost_town"), // Reduced traffic
            new FunctionEffect(Category.PedsAndCo, "Beach Party", "LifesABeach", "beach_theme"), // Beach party
            new FunctionEffect(Category.PedsAndCo, "Ninja Theme", "NinjaTown", "ninja_theme"), // Ninja theme
            new FunctionEffect(Category.PedsAndCo, "Kinky Theme", "LoveConquersAll", "kinky_theme"), // Kinky theme
            new FunctionEffect(Category.PedsAndCo, "Funhouse Theme", "CrazyTown", "funhouse_theme"), // Funhouse theme
            new FunctionEffect(Category.PedsAndCo, "Country Traffic", "HicksVille", "country_traffic"), // Country traffic

            new FunctionEffect(Category.PlayerModifications, "Weapon Aiming While Driving", "IWannaDriveBy", "weapon_aiming_while_driving"), // Weapon aiming while driving
            new FunctionEffect(Category.PlayerModifications, "Huge Bunny Hop", "CJPhoneHome", "huge_bunny_hop"), // Huge bunny hop
            new FunctionEffect(Category.PlayerModifications, "Mega Jump", "Kangaroo", "mega_jump"), // Mega jump
            new FunctionEffect(Category.PlayerModifications, "Infinite Oxygen", "ManFromAtlantis", "infinite_oxygen"), // Infinite oxygen
            new FunctionEffect(Category.PlayerModifications, "Mega Punch", "StingLikeABee", "mega_punch"), // Mega punch

            new FunctionEffect(Category.Stats, "Fat Player", "WhoAteAllThePies", "fat_player"), // Fat player
            new FunctionEffect(Category.Stats, "Max Muscle", "BuffMeUp", "muscle_player"), // Max muscle
            new FunctionEffect(Category.Stats, "Skinny Player", "LeanAndMean", "skinny_player"), // Skinny player
            new FunctionEffect(Category.Stats, "Max Stamina", "ICanGoAllNight", "max_stamina"), // Max stamina
            new FunctionEffect(Category.Stats, "No Stamina", "ImAllOutOfBreath", "no_stamina"), // No stamina
            new FunctionEffect(Category.Stats, "Hitman Level For All Weapons", "ProfessionalKiller", "hitman_level_for_all_weapons"), // Hitman level for all weapons
            new FunctionEffect(Category.Stats, "Beginner Level For All Weapons", "BabysFirstGun", "beginner_level_for_all_weapons"), // Beginner level for all weapons
            new FunctionEffect(Category.Stats, "Max Driving Skills", "NaturalTalent", "max_driving_skills"), // Max driving skills
            new FunctionEffect(Category.Stats, "No Driving Skills", "BackToDrivingSchool", "no_driving_skills"), // No driving skills
            new FunctionEffect(Category.Stats, "Never Get Hungry", "IAmNeverHungry", "never_get_hungry"), // Never get hungry
            new FunctionEffect(Category.Stats, "Lock Respect At Max", "WorshipMe", "lock_respect_at_max"), // Lock respect at max
            new FunctionEffect(Category.Stats, "Lock Sex Appeal At Max", "HelloLadies", "lock_sex_appeal_at_max"), // Lock sex appeal at max

            new FunctionEffect(Category.CustomEffects, "Clear Active Effects", "ClearActiveEffects", "clear_active_effects").SetType("other").DisableTwitch(), // Clear Active Effects
            new FunctionEffect(Category.CustomEffects, "Remove All Weapons", "NoWeaponsAllowed", "remove_all_weapons"), // Remove all weapons
            new FunctionEffect(Category.CustomEffects, "Get Busted", "GoToJail", "get_busted").DisableRapidFire(), // Get's you busted on the spot
            new FunctionEffect(Category.CustomEffects, "Set All Peds On Fire", "HotPotato", "set_all_peds_on_fire").DisableRapidFire(), // Set all peds on fire
            new FunctionEffect(Category.CustomEffects, "Kick Player Out Of Vehicle", "ThisAintYourCar", "kick_player_out_of_vehicle"), // Kick player out of vehicle
            new FunctionEffect(Category.CustomEffects, "Lock Player Inside Vehicle", "ThereIsNoEscape", "lock_player_inside_vehicle"), // Lock player inside vehicle
            new FunctionEffect(Category.CustomEffects, "Set Current Vehicle On Fire", "WayTooHot", "set_current_vehicle_on_fire").DisableRapidFire(), // Set current vehicle on fire
            new FunctionEffect(Category.CustomEffects, "Pop Tires Of All Vehicles", "TiresBeGone", "pop_tires_of_all_vehicles"), // Pop tires of all vehicles
            new FunctionEffect(Category.CustomEffects, "Send Vehicles To Space", "StairwayToHeaven", "send_vehicles_to_space"), // Gives an immense upwards boost to all vehicles
            new FunctionEffect(Category.CustomEffects, "Turn Vehicles Around", "TurnAround", "turn_vehicles_around"), // Turn vehicles around
            new FunctionEffect(Category.CustomEffects, "To The Left, To The Right", "ToTheLeftToTheRight", "to_the_left_to_the_right"), // Gives cars a random velocity
            new FunctionEffect(Category.CustomEffects, "Timelapse Mode", "DiscoInTheSky", "timelapse"), // Timelapse mode
            new FunctionEffect(Category.CustomEffects, "Where Is Everybody?", "ImHearingVoices", "where_is_everybody"), // Where is everybody?
            new FunctionEffect(Category.CustomEffects, "Everybody Bleed Now!", "EverybodyBleedNow", "everybody_bleed_now"), // Everybody bleed now!
            new FunctionEffect(Category.CustomEffects, "To Drive Or Not To Drive", "ToDriveOrNotToDrive", "to_drive_or_not_to_drive"), // To drive or not to drive
            new FunctionEffect(Category.CustomEffects, "One Hit K.O.", "ILikeToLiveDangerously", "one_hit_ko").DisableRapidFire(), // One Hit K.O.
            new FunctionEffect(Category.CustomEffects, "Experience The Lag", "PacketLoss", "experience_the_lag"), // Experience the lag
            new FunctionEffect(Category.CustomEffects, "Ghost Rider", "GhostRider", "ghost_rider"), // Set current vehicle constantly on fire
            new FunctionEffect(Category.CustomEffects, "Disable HUD", "FullyImmersed", "disable_hud"), // Disable HUD
            new FunctionEffect(Category.CustomEffects, "Disable Radar Blips", "BlipsBeGone", "disable_radar_blips"), // Disable Radar Blips
            new FunctionEffect(Category.CustomEffects, "Disable All Weapon Damage", "TruePacifist", "disable_all_weapon_damage"), // Disable all Weapon Damage
            new FunctionEffect(Category.CustomEffects, "Let's Take A Break", "LetsTakeABreak", "lets_take_a_break").DisableRapidFire(), // Let's take a break
            new FunctionEffect(Category.CustomEffects, "Rainbow Cars", "AllColorsAreBeautiful", "rainbow_cars"), // Rainbow Cars
            new FunctionEffect(Category.CustomEffects, "High Suspension Damping", "VeryDampNoBounce", "high_suspension_damping"), // Cars have high suspension damping
            new FunctionEffect(Category.CustomEffects, "Little Suspension Damping", "BouncinUpAndDown", "little_suspension_damping"), // Cars have very little suspension damping
            new FunctionEffect(Category.CustomEffects, "Zero Suspension Damping", "LowrideAllNight", "zero_suspension_damping"),  // Cars have almost zero suspension damping
            new FunctionEffect(Category.CustomEffects, "Long Live The Rich!", "LongLiveTheRich", "long_live_the_rich"),  // Money = Health, shoot people to gain money, take damage to lose it
            new FunctionEffect(Category.CustomEffects, "Inverted Controls", "InvertedControls", "inverted_controls"),  // Inverts some controls
            new FunctionEffect(Category.CustomEffects, "Disable One Movement Key", "DisableOneMovementKey", "disable_one_movement_key"),  // Disable one movement key
            new FunctionEffect(Category.CustomEffects, "Fail Current Mission", "MissionFailed", "fail_current_mission").DisableRapidFire(), // Fail Current Mission
            new FunctionEffect(Category.CustomEffects, "Night Vision", "NightVision", "night_vision"), // Night Vision
            new FunctionEffect(Category.CustomEffects, "Thermal Vision", "ThermalVision", "thermal_vision"), // Thermal Vision
            new FunctionEffect(Category.CustomEffects, "Pass Current Mission", "IllTakeAFreePass", "pass_current_mission").DisableRapidFire(), // Pass Current Mission
            new FunctionEffect(Category.CustomEffects, "Infinite Health (Everyone)", "NoOneCanHurtAnyone", "infinite_health_everyone"), // Infinite Health (Everyone)
            new FunctionEffect(Category.CustomEffects, "Invisible Vehicles", "InvisibleVehicles", "invisible_vehicles"), // Invisible Vehicles
            new FunctionEffect(Category.CustomEffects, "Powerpoint Presentation", "PowerpointPresentation", "framerate_15"), // Powerpoint Presentation (15 FPS)
            new FunctionEffect(Category.CustomEffects, "Smooth Criminal", "SmoothCriminal", "framerate_60"), // Smooth Criminal (60 FPS)
            new FunctionEffect(Category.CustomEffects, "Reload Autosave", "HereWeGoAgain", "reload_autosave").DisableRapidFire(), // Reload Autosave
            new FunctionEffect(Category.CustomEffects, "Quarter Gravity", "GroundControlToMajorTom", "quarter_gravity"), // Sets the gravity to 0.002f
            new FunctionEffect(Category.CustomEffects, "Half Gravity", "ImFeelingLightheaded", "half_gravity"), // Sets the gravity to 0.004f
            new FunctionEffect(Category.CustomEffects, "Double Gravity", "KilogramOfFeathers", "double_gravity"), // Sets the gravity to 0.016f
            new FunctionEffect(Category.CustomEffects, "Quadruple Gravity", "KilogramOfSteel", "quadruple_gravity"), // Sets the gravity to 0.032f
            new FunctionEffect(Category.CustomEffects, "Inverted Gravity", "BeamMeUpScotty", "inverted_gravity").DisableRapidFire(), // Sets the gravity to -0.002f
            new FunctionEffect(Category.CustomEffects, "Zero Gravity", "ImInSpaaaaace", "zero_gravity").DisableRapidFire(), // Sets the gravity to 0f
            new FunctionEffect(Category.CustomEffects, "Insane Gravity", "StraightToHell", "insane_gravity").DisableRapidFire(), // Sets the gravity to 0.64f
            new FunctionEffect(Category.CustomEffects, "Tunnel Vision", "TunnelVision", "tunnel_vision").DisableRapidFire(), // Tunnel Vision
            new FunctionEffect(Category.CustomEffects, "High Pitched Audio", "CJAndTheChipmunks", "high_pitched_audio"), // High Pitched Audio
            new FunctionEffect(Category.CustomEffects, "Pitch Shifter", "VocalRange", "pitch_shifter"), // Pitch Shifter
            new FunctionEffect(Category.CustomEffects, "Pass Current Mission", "IWontTakeAFreePass", "fake_pass_current_mission").DisableRapidFire(), // Fake Pass Current Mission
            new FunctionEffect(Category.CustomEffects, "DVD Screensaver", "ItsGonnaHitTheCorner", "dvd_screensaver").DisableRapidFire(), // DVD Screensaver
            new FunctionEffect(Category.CustomEffects, "Lightspeed Braking", "WinnersDoBrake", "lightspeed_braking").DisableRapidFire(), // Lightspeed Braking

            // All teleports are disabled during Rapid-Fire mode
            new TeleportationEffect("Teleport Home", "BringMeHome", Location.GrooveStreet),
            new TeleportationEffect("Teleport To A Tower", "BringMeToATower", Location.LSTower),
            new TeleportationEffect("Teleport To A Pier", "BringMeToAPier", Location.LSPier),
            new TeleportationEffect("Teleport To The LS Airport", "BringMeToTheLSAirport", Location.LSAirport),
            new TeleportationEffect("Teleport To The Docks", "BringMeToTheDocks", Location.LSDocks),
            new TeleportationEffect("Teleport To A Mountain", "BringMeToAMountain", Location.MountChiliad),
            new TeleportationEffect("Teleport To The SF Airport", "BringMeToTheSFAirport", Location.SFAirport),
            new TeleportationEffect("Teleport To A Bridge", "BringMeToABridge", Location.SFBridge),
            new TeleportationEffect("Teleport To A Secret Place", "BringMeToASecretPlace", Location.Area52),
            new TeleportationEffect("Teleport To A Quarry", "BringMeToAQuarry", Location.LVQuarry),
            new TeleportationEffect("Teleport To The LV Airport", "BringMeToTheLVAirport", Location.LVAirport),
            new TeleportationEffect("Teleport To Big Ear", "BringMeToBigEar", Location.LVSatellite),
        };

        public static List<AbstractEffect> EnabledEffects { get; } = new List<AbstractEffect>();

        public static AbstractEffect GetByID(string id, bool onlyEnabled = false)
        {
            return (onlyEnabled ? EnabledEffects : Effects).Find(e => e.Id == id);
        }

        public static AbstractEffect GetByWord(string word, bool onlyEnabled = false)
        {
            return (onlyEnabled ? EnabledEffects : Effects).Find(e => !string.IsNullOrEmpty(e.Word) && string.Equals(e.Word, word, StringComparison.OrdinalIgnoreCase));
        }

        public static AbstractEffect GetByDescription(string description, bool onlyEnabled = false)
        {
            return (onlyEnabled ? EnabledEffects : Effects).Find(e => string.Equals(description, e.GetDescription(), StringComparison.OrdinalIgnoreCase));
        }

        public static AbstractEffect GetRandomEffect(bool onlyEnabled = false)
        {
            List<AbstractEffect> effects = (onlyEnabled ? EnabledEffects : Effects);
            if (effects.Count == 0)
            {
                return null;
            }
            return effects[RandomHandler.Next(effects.Count)];
        }

        public static AbstractEffect RunEffect(string id, bool onlyEnabled = true)
        {
            return RunEffect(GetByID(id, onlyEnabled));
        }

        public static AbstractEffect RunEffect(AbstractEffect effect)
        {
            if (effect != null)
            {
                effect.RunEffect();
            }
            return effect;
        }

        public static void SetEffectEnabled(AbstractEffect effect, bool enabled)
        {
            if (effect != null)
            {
                if (!enabled && EnabledEffects.Contains(effect))
                {
                    EnabledEffects.Remove(effect);
                }
                else if (enabled && !EnabledEffects.Contains(effect))
                {
                    EnabledEffects.Add(effect);
                }
            }
        }
    }
}
