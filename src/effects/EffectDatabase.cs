using GTA_SA_Chaos.src.effects.extra;
using GTA_SA_Chaos.util;
using System;
using System.Collections.Generic;

namespace GTA_SA_Chaos.effects
{
    static class EffectDatabase
    {
        public static List<AbstractEffect> Effects { get; } = new List<AbstractEffect>
        {
            new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 1", "ThugsArmoury", "weapons", "0"), // Weapon Set 1
            new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 2", "ProfessionalsKit", "weapons", "1"), // Weapon Set 2
            new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 3", "NuttersToys", "weapons", "2"), // Weapon Set 3
            new FunctionEffect(Category.WeaponsAndHealth, "Health, Armor, $250k", "INeedSomeHelp", "cheat", "give_health_armor_money"), // Health, Armor, $250k
            new FunctionEffect(Category.WeaponsAndHealth, "Suicide", "GoodbyeCruelWorld", "cheat", "suicide"), // Suicide
            new TimedFunctionEffect(Category.WeaponsAndHealth, "Infinite ammo", "FullClip", "timed_cheat", "infinite_ammo"), // Infinite ammo

            new FunctionEffect(Category.WantedLevel, "Wanted level 2 stars", "TurnUpTheHeat", "wanted", "plus_two"), // Wanted level 2 stars
            new FunctionEffect(Category.WantedLevel, "Clear wanted level", "TurnDownTheHeat", "wanted", "clear"), // Clear wanted level
            new TimedFunctionEffect(Category.WantedLevel, "Never wanted", "IDoAsIPlease", "timed_cheat", "never_wanted"), // Never wanted
            new FunctionEffect(Category.WantedLevel, "Six wanted stars", "BringItOn", "wanted", "six_stars"), // Six wanted stars

            new WeatherEffect("Sunny weather", "PleasantlyWarm", 1), // Sunny weather
            new WeatherEffect("Very sunny weather", "TooDamnHot", 0), // Very sunny weather
            new WeatherEffect("Overcast weather", "DullDullDay", 4), // Overcast weather
            new WeatherEffect("Rainy weather", "StayInAndWatchTV", 16), // Rainy weather
            new WeatherEffect("Foggy weather", "CantSeeWhereImGoing", 9), // Foggy weather
            new WeatherEffect("Thunderstorm", "ScottishSummer", 16), // Thunder storm
            new WeatherEffect("Sandstorm", "SandInMyEars", 19), // Sand storm

            new FunctionEffect(Category.Spawning, "Get Parachute", "LetsGoBaseJumping", "weapons", "3"), // Get Parachute
            new FunctionEffect(Category.Spawning, "Get Jetpack", "Rocketman", "cheat", "jetpack"), // Get Jetpack
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
            new GameSpeedEffect("0.25x Game Speed", "MatrixMode", 0.25f),
            new GameSpeedEffect("0.5x Game Speed", "SlowItDown", 0.5f),
            new GameSpeedEffect("2x Game Speed", "SpeedItUp", 2f),
            new GameSpeedEffect("4x Game Speed", "YoureTooSlow", 4f),
            new TimedFunctionEffect(Category.Time, "Always midnight", "NightProwler", "timed_cheat", "always_midnight"), // Always midnight
            new TimedFunctionEffect(Category.Time, "Stop game clock, orange sky", "DontBringOnTheNight", "timed_cheat", "orange_sky"), // Stop game clock, orange sky
            new TimedFunctionEffect(Category.Time, "Faster clock", "TimeJustFliesBy", "timed_cheat", "faster_clock"), // Faster clock

            new FunctionEffect(Category.VehiclesTraffic, "Blow up all cars", "AllCarsGoBoom", "other", "explode_cars"), // Blow up all cars
            new TimedFunctionEffect(Category.VehiclesTraffic, "Pink traffic", "PinkIsTheNewCool", "timed_cheat", "pink_traffic"), // Pink traffic
            new TimedFunctionEffect(Category.VehiclesTraffic, "Black traffic", "SoLongAsItsBlack", "timed_cheat", "black_traffic"), // Black traffic
            new TimedFunctionEffect(Category.VehiclesTraffic, "Cheap cars", "EveryoneIsPoor", "timed_cheat", "cheap_cars"), // Cheap cars
            new TimedFunctionEffect(Category.VehiclesTraffic, "Expensive cars", "EveryoneIsRich", "timed_cheat", "expensive_cars"), // Expensive cars
            new TimedFunctionEffect(Category.VehiclesTraffic, "Invisible cars", "WheelsOnlyPlease", "timed_cheat", "invisible_cars"), // Invisible cars
            new TimedFunctionEffect(Category.VehiclesTraffic, "Insane handling", "StickLikeGlue", "timed_cheat", "insane_handling"), // Insane handling
            new TimedFunctionEffect(Category.VehiclesTraffic, "All green lights", "DontTryAndStopMe", "timed_cheat", "all_green_lights"), // All green lights
            new TimedFunctionEffect(Category.VehiclesTraffic, "Cars on water", "JesusCars", "timed_cheat", "cars_on_water"), // Cars on water
            new TimedFunctionEffect(Category.VehiclesTraffic, "Boats fly", "FlyingFish", "timed_cheat", "boats_fly"), // Boats fly
            new TimedFunctionEffect(Category.VehiclesTraffic, "Cars fly", "ChittyChittyBangBang", "timed_cheat", "cars_fly"), // Cars fly
            new TimedFunctionEffect(Category.VehiclesTraffic, "Smash n' boom", "TouchMyCarYouDie", "timed_cheat", "smash_n_boom"), // Smash n' boom
            new TimedFunctionEffect(Category.VehiclesTraffic, "All cars have nitro", "SpeedFreak", "timed_cheat", "all_cars_nitro"), // All cars have nitro
            new TimedFunctionEffect(Category.VehiclesTraffic, "Cars float away when hit", "BubbleCars", "timed_cheat", "bubble_cars"), // Cars float away when hit
            new TimedFunctionEffect(Category.VehiclesTraffic, "Reduced traffic", "GhostTown", "timed_cheat", "reduced_traffic"), // Reduced traffic
            new TimedFunctionEffect(Category.VehiclesTraffic, "All taxis have nitrous", "SpeedyTaxis", "timed_cheat", "all_taxis_nitro"), // All taxis have nitrous

            new TimedFunctionEffect(Category.PedsAndCo, "Peds attack other (+ get golf club)", "RoughNeighbourhood", "timed_cheat", "rough_neighbourhood"), // Peds attack other (+ get golf club)
            new TimedFunctionEffect(Category.PedsAndCo, "Have a bounty on your head", "StopPickingOnMe", "timed_cheat", "bounty_on_your_head"), // Have a bounty on your head
            new TimedFunctionEffect(Category.PedsAndCo, "Elvis is everywhere", "BlueSuedeShoes", "timed_cheat", "elvis_lives"), // Elvis is everywhere
            new TimedFunctionEffect(Category.PedsAndCo, "Peds attack you with rockets", "AttackOfTheVillagePeople", "timed_cheat", "village_people"), // Peds attack you with rockets
            new TimedFunctionEffect(Category.PedsAndCo, "Gang members everywhere", "OnlyHomiesAllowed", "timed_cheat", "only_homies"), // Gang members everywhere
            new TimedFunctionEffect(Category.PedsAndCo, "Gangs control the streets", "BetterStayIndoors", "timed_cheat", "stay_indoors"), // Gangs control the streets
            new TimedFunctionEffect(Category.PedsAndCo, "Riot mode", "StateOfEmergency", "timed_cheat", "riot_mode"), // Riot mode
            new TimedFunctionEffect(Category.PedsAndCo, "Everyone armed", "SurroundedByNutters", "timed_cheat", "everyone_armed"), // Everyone armed
            new TimedFunctionEffect(Category.PedsAndCo, "Aggressive drivers", "AllDriversAreCriminals", "timed_cheat", "aggressive_drivers"), // Aggressive drivers
            new TimedFunctionEffect(Category.PedsAndCo, "Recruit anyone (9mm)", "WannaBeInMyGang", "timed_cheat", "recruit_9mm"), // Recruit anyone (9mm)
            new TimedFunctionEffect(Category.PedsAndCo, "Recruit anyone (AK-47)", "NoOneCanStopUs", "timed_cheat", "recruit_ak47"), // Recruit anyone (AK-47)
            new TimedFunctionEffect(Category.PedsAndCo, "Recruit anyone (Rockets)", "RocketMayhem", "timed_cheat", "recruit_rockets"), // Recruit anyone (Rockets)
            new TimedFunctionEffect(Category.PedsAndCo, "Prostitutes pay you", "ReverseHooker", "timed_cheat", "reverse_hooker"), // Prostitutes pay you
            new TimedFunctionEffect(Category.PedsAndCo, "Beach party", "LifesABeach", "timed_cheat", "beach_party"), // Beach party
            new TimedFunctionEffect(Category.PedsAndCo, "Ninja theme", "NinjaTown", "timed_cheat", "ninja_theme"), // Ninja theme
            new TimedFunctionEffect(Category.PedsAndCo, "Slut magnet", "LoveConquersAll", "timed_cheat", "slut_magnet"), // Slut magnet
            new TimedFunctionEffect(Category.PedsAndCo, "Funhouse theme", "CrazyTown", "timed_cheat", "funhouse_theme"), // Funhouse theme
            new TimedFunctionEffect(Category.PedsAndCo, "Country traffic", "HicksVille", "timed_cheat", "country_traffic"), // Country traffic

            new TimedFunctionEffect(Category.PlayerModifications, "Weapon aiming while driving", "IWannaDriveBy", "timed_cheat", "drive_by"), // Weapon aiming while driving
            new TimedFunctionEffect(Category.PlayerModifications, "Huge bunny hop", "CJPhoneHome", "timed_cheat", "huge_bunny_hop"), // Huge bunny hop
            new TimedFunctionEffect(Category.PlayerModifications, "Mega jump", "Kangaroo", "timed_cheat", "mega_jump"), // Mega jump
            new TimedFunctionEffect(Category.PlayerModifications, "Infinite health", "NoOneCanHurtMe", "timed_cheat", "infinite_health"), // Infinite health
            new TimedFunctionEffect(Category.PlayerModifications, "Infinite oxygen", "ManFromAtlantis", "timed_cheat", "infinite_oxygen"), // Infinite oxygen
            new TimedFunctionEffect(Category.PlayerModifications, "Mega punch", "StingLikeABee", "timed_cheat", "mega_punch"), // Mega punch

            new FunctionEffect(Category.Stats, "Fat player", "WhoAteAllThePies", "cheat", "fat_player"), // Fat player
            new FunctionEffect(Category.Stats, "Max muscle", "BuffMeUp", "cheat", "max_muscle"), // Max muscle
            new FunctionEffect(Category.Stats, "Skinny player", "LeanAndMean", "cheat", "skinny_player"), // Skinny player
            new FunctionEffect(Category.Stats, "Max stamina", "ICanGoAllNight", "effect", "max_stamina"), // Max stamina
            new FunctionEffect(Category.Stats, "Hitman level for all weapons", "ProfessionalKiller", "effect", "max_weapon_skill"), // Hitman level for all weapons
            new FunctionEffect(Category.Stats, "Max driving skills", "NaturalTalent", "effect", "max_driving_skill"), // Max driving skills
            new TimedFunctionEffect(Category.Stats, "Never get hungry", "IAmNeverHungry", "timed_cheat", "never_hungry"), // Never get hungry
            new TimedFunctionEffect(Category.Stats, "Lock respect at max", "WorshipMe", "timed_cheat", "lock_respect"), // Lock respect at max
            new TimedFunctionEffect(Category.Stats, "Lock sex appeal at max", "HelloLadies", "timed_cheat", "lock_sex_appeal"), // Lock sex appeal at max

            new FunctionEffect(Category.CustomEffects, "Pop tires of all vehicles", "TiresBeGone", "effect", "pop_vehicle_tires"), // Pop tires of all vehicles
            new FunctionEffect(Category.CustomEffects, "Set current vehicle on fire", "WayTooHot", "effect", "set_vehicle_on_fire"), // Set current vehicle on fire
            new FunctionEffect(Category.CustomEffects, "Turn vehicles around", "TurnAround", "effect", "turn_vehicles_around"), // Turn vehicles around
            new FunctionEffect(Category.CustomEffects, "Stairway To Heaven", "StairwayToHeaven", "effect", "stairway_to_heaven"), // Gives an immense upwards boost to the current vehicle
            new FunctionEffect(Category.CustomEffects, "Get Busted", "GoToJail", "effect", "player_busted"), // Get's you busted on the spot
            new FunctionEffect(Category.CustomEffects, "Get Wasted", "Hospitality", "effect", "player_wasted"), // Get's you wasted on the spot
            new TimedFunctionEffect(Category.CustomEffects, "One Hit K.O.", "ILikeToLiveDangerously", "timed_effect", "one_hit_ko"), // One Hit K.O.
            new ModifyGravityEffect("Inverted gravity", "BeamMeUpScotty", -0.002f, 1000 * 5), // Sets the gravity to -0.002f
            new ModifyGravityEffect("Zero gravity", "ImInSpaaaaace", 0f, 1000 * 10), // Sets the gravity to 0f
            new ModifyGravityEffect("Quarter gravity", "GroundControlToMajorTom", 0.002f), // Sets the gravity to 0.002f
            new ModifyGravityEffect("Half gravity", "ImFeelingLightheaded", 0.004f), // Sets the gravity to 0.004f
            new ModifyGravityEffect("Double gravity", "KilogramOfFeathers", 0.016f), // Sets the gravity to 0.016f
            new ModifyGravityEffect("Quadruple gravity", "KilogramOfSteel", 0.032f), // Sets the gravity to 0.032f
            new ModifyGravityEffect("Insane gravity", "StraightToHell", 0.64f, 1000 * 10), // Sets the gravity to 0.64f
            new TimedFunctionEffect(Category.CustomEffects, "Experience the lag", "PacketLoss", "timed_effect", "experience_the_lag"), // Experience the lag
            new TimedFunctionEffect(Category.CustomEffects, "To drive or not to drive", "ToDriveOrNotToDrive", "timed_effect", "to_drive_or_not_to_drive"), // To drive or not to drive
            new FunctionEffect(Category.CustomEffects, "Timelapse mode", "DiscoInTheSky", "timed_effect", "timelapse"), // Timelapse mode
            new TimedFunctionEffect(Category.CustomEffects, "Ghost Rider", "GhostRider", "timed_effect", "ghost_rider"), // Set current vehicle constantly on fire
            new TimedFunctionEffect(Category.CustomEffects, "To the left, to the right", "ToTheLeftToTheRight", "timed_effect", "totheleft_totheright"), // Gives cars a random velocity
            new TimedFunctionEffect(Category.CustomEffects, "Disable HUD", "FullyImmersed", "timed_effect", "disable_hud"), // Disable HUD
            new FunctionEffect(Category.CustomEffects, "Remove all weapons", "TruePacifist", "other", "clear_weapons"), // Remove all weapons
            new TimedFunctionEffect(Category.CustomEffects, "Where is everybody?", "WhatAreThoseVoices", "timed_effect", "where_is_everybody"), // Where is everybody?
            new TimedFunctionEffect(Category.CustomEffects, "Everybody bleed now!", "EverybodyBleedNow", "timed_effect", "everybody_bleed_now"), // Everybody bleed now!
            new FunctionEffect(Category.CustomEffects, "Set all peds on fire", "HotPotato", "effect", "hot_potato"), // Set all peds on fire
            new FunctionEffect(Category.CustomEffects, "Kick player out of vehicle and lock doors", "ThisAintYourCar", "effect", "kick_out_of_car"), // Kick player out of vehicle
            new FunctionEffect(Category.CustomEffects, "Lock player inside vehicle", "ThereIsNoEscape", "effect", "there_is_no_escape"), // Lock player inside vehicle
            new TimedFunctionEffect(Category.CustomEffects, "Disable Radar Blips", "BlipsBeGone", "timed_effect", "disable_radar_blips"), // Disable Radar Blips

            new TeleportationEffect("Teleport home", "BringMeHome", Location.GrooveStreet),
            new TeleportationEffect("Teleport to a tower", "BringMeToATower", Location.LSTower),
            new TeleportationEffect("Teleport to a pier", "BringMeToAPier", Location.LSPier),
            new TeleportationEffect("Teleport to the LS airport", "BringMeToTheLSAirport", Location.LSAirport),
            new TeleportationEffect("Teleport to the docks", "BringMeToTheDocks", Location.LSDocks),
            new TeleportationEffect("Teleport to a mountain", "BringMeToAMountain", Location.MountChiliad),
            new TeleportationEffect("Teleport to the SF airport", "BringMeToTheSFAirport", Location.SFAirport),
            new TeleportationEffect("Teleport to a bridge", "BringMeToABridge", Location.SFBridge),
            new TeleportationEffect("Teleport to a secret place", "BringMeToASecret", Location.Area52),
            new TeleportationEffect("Teleport to a quarry", "BringMeToAQuarry", Location.LVQuarry),
            new TeleportationEffect("Teleport to the LV airport", "BringMeToTheLVAirport", Location.LVAirport),
            new TeleportationEffect("Teleport to the dish", "BringMeToTheDish", Location.LVSatellite),
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
