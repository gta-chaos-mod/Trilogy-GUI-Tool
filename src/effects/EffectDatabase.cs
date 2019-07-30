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
            new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 1", "ThugsArmoury", "cheat", "weapon_set_1"), // Weapon Set 1
            new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 2", "ProfessionalsKit", "cheat", "weapon_set_2"), // Weapon Set 2
            new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 3", "NuttersToys", "cheat", "weapon_set_3"), // Weapon Set 3
            new FunctionEffect(Category.WeaponsAndHealth, "Health, Armor, $250k", "INeedSomeHelp", "cheat", "give_health_armor_money"), // Health, Armor, $250k
            new FunctionEffect(Category.WeaponsAndHealth, "Suicide", "GoodbyeCruelWorld", "cheat", "suicide"), // Suicide
            new FunctionEffect(Category.WeaponsAndHealth, "Infinite Ammo", "FullClip", "timed_cheat", "infinite_ammo"), // Infinite ammo

            new FunctionEffect(Category.WantedLevel, "Wanted Level 2 Stars", "TurnUpTheHeat", "wanted", "plus_two"), // Wanted level 2 stars
            new FunctionEffect(Category.WantedLevel, "Clear Wanted Level", "TurnDownTheHeat", "wanted", "clear"), // Clear wanted level
            new FunctionEffect(Category.WantedLevel, "Never Wanted", "IDoAsIPlease", "timed_cheat", "never_wanted"), // Never wanted
            new FunctionEffect(Category.WantedLevel, "Six Wanted Stars", "BringItOn", "wanted", "six_stars"), // Six wanted stars

            new WeatherEffect("Sunny Weather", "PleasantlyWarm", 1), // Sunny weather
            new WeatherEffect("Very Sunny Weather", "TooDamnHot", 0), // Very sunny weather
            new WeatherEffect("Overcast Weather", "DullDullDay", 4), // Overcast weather
            new WeatherEffect("Rainy Weather", "StayInAndWatchTV", 16), // Rainy weather
            new WeatherEffect("Foggy Weather", "CantSeeWhereImGoing", 9), // Foggy weather
            new WeatherEffect("Thunderstorm", "ScottishSummer", 16), // Thunder storm
            new WeatherEffect("Sandstorm", "SandInMyEars", 19), // Sand storm

            new FunctionEffect(Category.Spawning, "Get Parachute", "LetsGoBaseJumping", "cheat", "parachute"), // Get Parachute
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
            new FunctionEffect(Category.Time, "Always Midnight", "NightProwler", "timed_cheat", "always_midnight"), // Always midnight
            new FunctionEffect(Category.Time, "Stop Game Clock, Orange Sky", "DontBringOnTheNight", "timed_cheat", "orange_sky"), // Stop game clock, orange sky
            new FunctionEffect(Category.Time, "Faster Clock", "TimeJustFliesBy", "timed_cheat", "faster_clock"), // Faster clock

            new FunctionEffect(Category.VehiclesTraffic, "Blow Up All Cars", "AllCarsGoBoom", "other", "explode_cars"), // Blow up all cars
            new FunctionEffect(Category.VehiclesTraffic, "Pink Traffic", "PinkIsTheNewCool", "timed_cheat", "pink_traffic"), // Pink traffic
            new FunctionEffect(Category.VehiclesTraffic, "Black Traffic", "SoLongAsItsBlack", "timed_cheat", "black_traffic"), // Black traffic
            new FunctionEffect(Category.VehiclesTraffic, "Cheap Cars", "EveryoneIsPoor", "timed_cheat", "cheap_cars"), // Cheap cars
            new FunctionEffect(Category.VehiclesTraffic, "Expensive Cars", "EveryoneIsRich", "timed_cheat", "expensive_cars"), // Expensive cars
            new FunctionEffect(Category.VehiclesTraffic, "Invisible Cars", "WheelsOnlyPlease", "timed_cheat", "invisible_cars"), // Invisible cars
            new FunctionEffect(Category.VehiclesTraffic, "Insane Handling", "StickLikeGlue", "timed_cheat", "insane_handling"), // Insane handling
            new FunctionEffect(Category.VehiclesTraffic, "All Green Lights", "DontTryAndStopMe", "timed_cheat", "all_green_lights"), // All green lights
            new FunctionEffect(Category.VehiclesTraffic, "Cars On Water", "JesusTakeTheWheel", "timed_cheat", "cars_on_water"), // Cars on water
            new FunctionEffect(Category.VehiclesTraffic, "Boats Fly", "FlyingFish", "timed_cheat", "boats_fly"), // Boats fly
            new FunctionEffect(Category.VehiclesTraffic, "Cars Fly", "ChittyChittyBangBang", "timed_cheat", "cars_fly"), // Cars fly
            new FunctionEffect(Category.VehiclesTraffic, "Smash N' Boom", "TouchMyCarYouDie", "timed_cheat", "smash_n_boom"), // Smash n' boom
            new FunctionEffect(Category.VehiclesTraffic, "All Cars Have Nitro", "SpeedFreak", "timed_cheat", "all_cars_nitro"), // All cars have nitro
            new FunctionEffect(Category.VehiclesTraffic, "Cars Float Away When Hit", "BubbleCars", "timed_cheat", "bubble_cars"), // Cars float away when hit
            new FunctionEffect(Category.VehiclesTraffic, "Reduced Traffic", "GhostTown", "timed_cheat", "reduced_traffic"), // Reduced traffic
            new FunctionEffect(Category.VehiclesTraffic, "All Taxis Have Nitrous", "SpeedyTaxis", "timed_cheat", "all_taxis_nitro"), // All taxis have nitrous

            new FunctionEffect(Category.PedsAndCo, "Peds Attack Other", "RoughNeighbourhood", "timed_cheat", "rough_neighbourhood"), // Peds attack other (+ get golf club)
            new FunctionEffect(Category.PedsAndCo, "Have A Bounty On Your Head", "StopPickingOnMe", "timed_cheat", "bounty_on_your_head"), // Have a bounty on your head
            new FunctionEffect(Category.PedsAndCo, "Elvis Is Everywhere", "BlueSuedeShoes", "timed_cheat", "elvis_lives"), // Elvis is everywhere
            new FunctionEffect(Category.PedsAndCo, "Peds Attack You With Rockets", "AttackOfTheVillagePeople", "timed_cheat", "village_people"), // Peds attack you with rockets
            new FunctionEffect(Category.PedsAndCo, "Gang Members Everywhere", "OnlyHomiesAllowed", "timed_cheat", "only_homies"), // Gang members everywhere
            new FunctionEffect(Category.PedsAndCo, "Gangs Control The Streets", "BetterStayIndoors", "timed_cheat", "stay_indoors"), // Gangs control the streets
            new FunctionEffect(Category.PedsAndCo, "Riot Mode", "StateOfEmergency", "timed_cheat", "riot_mode"), // Riot mode
            new FunctionEffect(Category.PedsAndCo, "Everyone Armed", "SurroundedByNutters", "timed_cheat", "everyone_armed"), // Everyone armed
            new FunctionEffect(Category.PedsAndCo, "Aggressive Drivers", "AllDriversAreCriminals", "timed_cheat", "aggressive_drivers"), // Aggressive drivers
            new FunctionEffect(Category.PedsAndCo, "Recruit Anyone (9mm)", "WannaBeInMyGang", "timed_cheat", "recruit_9mm"), // Recruit anyone (9mm)
            new FunctionEffect(Category.PedsAndCo, "Recruit Anyone (AK-47)", "NoOneCanStopUs", "timed_cheat", "recruit_ak47"), // Recruit anyone (AK-47)
            new FunctionEffect(Category.PedsAndCo, "Recruit Anyone (Rockets)", "RocketMayhem", "timed_cheat", "recruit_rockets"), // Recruit anyone (Rockets)
            new FunctionEffect(Category.PedsAndCo, "Prostitutes Pay You", "ReverseHooker", "timed_cheat", "reverse_hooker"), // Prostitutes pay you
            new FunctionEffect(Category.PedsAndCo, "Beach Party", "LifesABeach", "timed_cheat", "beach_party"), // Beach party
            new FunctionEffect(Category.PedsAndCo, "Ninja Theme", "NinjaTown", "timed_cheat", "ninja_theme"), // Ninja theme
            new FunctionEffect(Category.PedsAndCo, "Kinky Theme", "LoveConquersAll", "timed_cheat", "kinky_theme"), // Kinky theme
            new FunctionEffect(Category.PedsAndCo, "Funhouse Theme", "CrazyTown", "timed_cheat", "funhouse_theme"), // Funhouse theme
            new FunctionEffect(Category.PedsAndCo, "Country Traffic", "HicksVille", "timed_cheat", "country_traffic"), // Country traffic

            new FunctionEffect(Category.PlayerModifications, "Weapon Aiming While Driving", "IWannaDriveBy", "timed_cheat", "drive_by"), // Weapon aiming while driving
            new FunctionEffect(Category.PlayerModifications, "Huge Bunny Hop", "CJPhoneHome", "timed_cheat", "huge_bunny_hop"), // Huge bunny hop
            new FunctionEffect(Category.PlayerModifications, "Mega Jump", "Kangaroo", "timed_cheat", "mega_jump"), // Mega jump
            new FunctionEffect(Category.PlayerModifications, "Infinite Health", "NoOneCanHurtMe", "timed_cheat", "infinite_health"), // Infinite health
            new FunctionEffect(Category.PlayerModifications, "Infinite Oxygen", "ManFromAtlantis", "timed_cheat", "infinite_oxygen"), // Infinite oxygen
            new FunctionEffect(Category.PlayerModifications, "Mega Punch", "StingLikeABee", "timed_cheat", "mega_punch"), // Mega punch

            new FunctionEffect(Category.Stats, "Fat Player", "WhoAteAllThePies", "cheat", "fat_player"), // Fat player
            new FunctionEffect(Category.Stats, "Max Muscle", "BuffMeUp", "cheat", "max_muscle"), // Max muscle
            new FunctionEffect(Category.Stats, "Skinny Player", "LeanAndMean", "cheat", "skinny_player"), // Skinny player
            new FunctionEffect(Category.Stats, "Max Stamina", "ICanGoAllNight", "effect", "max_stamina"), // Max stamina
            new FunctionEffect(Category.Stats, "Hitman Level For All Weapons", "ProfessionalKiller", "effect", "max_weapon_skill"), // Hitman level for all weapons
            new FunctionEffect(Category.Stats, "Max Driving Skills", "NaturalTalent", "effect", "max_driving_skill"), // Max driving skills
            new FunctionEffect(Category.Stats, "Never Get Hungry", "IAmNeverHungry", "timed_cheat", "never_hungry"), // Never get hungry
            new FunctionEffect(Category.Stats, "Lock Respect At Max", "WorshipMe", "timed_cheat", "lock_respect"), // Lock respect at max
            new FunctionEffect(Category.Stats, "Lock Sex Appeal At Max", "HelloLadies", "timed_cheat", "lock_sex_appeal"), // Lock sex appeal at max

            new FunctionEffect(Category.CustomEffects, "Pop Tires Of All Vehicles", "TiresBeGone", "effect", "pop_vehicle_tires"), // Pop tires of all vehicles
            new FunctionEffect(Category.CustomEffects, "Set Current Vehicle On Fire", "WayTooHot", "effect", "set_vehicle_on_fire"), // Set current vehicle on fire
            new FunctionEffect(Category.CustomEffects, "Turn Vehicles Around", "TurnAround", "effect", "turn_vehicles_around"), // Turn vehicles around
            new FunctionEffect(Category.CustomEffects, "Stairway To Heaven", "StairwayToHeaven", "effect", "stairway_to_heaven"), // Gives an immense upwards boost to the current vehicle
            new FunctionEffect(Category.CustomEffects, "Get Busted", "GoToJail", "effect", "player_busted"), // Get's you busted on the spot
            new FunctionEffect(Category.CustomEffects, "Get Wasted", "Hospitality", "effect", "player_wasted"), // Get's you wasted on the spot
            new FunctionEffect(Category.CustomEffects, "One Hit K.O.", "ILikeToLiveDangerously", "timed_effect", "one_hit_ko"), // One Hit K.O.
            new ModifyGravityEffect("Inverted Gravity", "BeamMeUpScotty", -0.002f, 1000 * 5), // Sets the gravity to -0.002f
            new ModifyGravityEffect("Zero Gravity", "ImInSpaaaaace", 0f, 1000 * 10), // Sets the gravity to 0f
            new ModifyGravityEffect("Quarter Gravity", "GroundControlToMajorTom", 0.002f), // Sets the gravity to 0.002f
            new ModifyGravityEffect("Half Gravity", "ImFeelingLightheaded", 0.004f), // Sets the gravity to 0.004f
            new ModifyGravityEffect("Double Gravity", "KilogramOfFeathers", 0.016f), // Sets the gravity to 0.016f
            new ModifyGravityEffect("Quadruple Gravity", "KilogramOfSteel", 0.032f), // Sets the gravity to 0.032f
            new ModifyGravityEffect("Insane Gravity", "StraightToHell", 0.64f, 1000 * 10), // Sets the gravity to 0.64f
            new FunctionEffect(Category.CustomEffects, "Experience The Lag", "PacketLoss", "timed_effect", "experience_the_lag"), // Experience the lag
            new FunctionEffect(Category.CustomEffects, "To Drive Or Not To Drive", "ToDriveOrNotToDrive", "timed_effect", "to_drive_or_not_to_drive"), // To drive or not to drive
            new FunctionEffect(Category.CustomEffects, "Timelapse Mode", "DiscoInTheSky", "timed_effect", "timelapse"), // Timelapse mode
            new FunctionEffect(Category.CustomEffects, "Ghost Rider", "GhostRider", "timed_effect", "ghost_rider"), // Set current vehicle constantly on fire
            new FunctionEffect(Category.CustomEffects, "To The Left, To The Right", "ToTheLeftToTheRight", "timed_effect", "totheleft_totheright"), // Gives cars a random velocity
            new FunctionEffect(Category.CustomEffects, "Disable HUD", "FullyImmersed", "timed_effect", "disable_hud"), // Disable HUD
            new FunctionEffect(Category.CustomEffects, "Remove All Weapons", "NoWeaponsAllowed", "other", "clear_weapons"), // Remove all weapons
            new FunctionEffect(Category.CustomEffects, "Where Is Everybody?", "ImHearingVoices", "timed_effect", "where_is_everybody"), // Where is everybody?
            new FunctionEffect(Category.CustomEffects, "Everybody Bleed Now!", "EverybodyBleedNow", "timed_effect", "everybody_bleed_now"), // Everybody bleed now!
            new FunctionEffect(Category.CustomEffects, "Set All Peds On Fire", "HotPotato", "effect", "hot_potato"), // Set all peds on fire
            new FunctionEffect(Category.CustomEffects, "Kick Player Out Of Vehicle", "ThisAintYourCar", "effect", "kick_out_of_car"), // Kick player out of vehicle
            new FunctionEffect(Category.CustomEffects, "Lock Player Inside Vehicle", "ThereIsNoEscape", "effect", "there_is_no_escape"), // Lock player inside vehicle
            new FunctionEffect(Category.CustomEffects, "Disable Radar Blips", "BlipsBeGone", "timed_effect", "disable_radar_blips"), // Disable Radar Blips
            new FunctionEffect(Category.CustomEffects, "Disable All Weapon Damage", "TruePacifist", "timed_effect", "true_pacifist"), // Disable all Weapon Damage
            new FunctionEffect(Category.CustomEffects, "Carl Is Uncontrollable", "LetsTakeABreak", "timed_effect", "lets_take_a_break", 1000 * 10), // Carl is uncontrollable
            new FunctionEffect(Category.CustomEffects, "Rainbow Cars", "AllColorsAreBeautiful", "timed_effect", "rainbow_cars"), // Rainbow Cars
            new FunctionEffect(Category.CustomEffects, "High Suspension Damping", "VeryDampNoBounce", "timed_effect", "no_bouncy_vehicles"), // Cars have high suspension damping
            new FunctionEffect(Category.CustomEffects, "Little Suspension Damping", "BouncinUpAndDown", "timed_effect", "bouncy_vehicles"), // Cars have very little suspension damping
            new FunctionEffect(Category.CustomEffects, "Zero Suspension Damping", "LowrideAllNight", "timed_effect", "very_bouncy_vehicles"),  // Cars have almost zero suspension damping
            new FunctionEffect(Category.CustomEffects, "Long Live The Rich!", "LongLiveTheRich", "timed_effect", "long_live_the_rich"),  // Money = Health, shoot people to gain money, take damage to lose it
            new FunctionEffect(Category.CustomEffects, "Inverted Controls", "InvertedControls", "timed_effect", "inverted_controls"),  // Inverts some controls

            new TeleportationEffect("Teleport Home", "BringMeHome", Location.GrooveStreet),
            new TeleportationEffect("Teleport To A Tower", "BringMeToATower", Location.LSTower),
            new TeleportationEffect("Teleport To A Pier", "BringMeToAPier", Location.LSPier),
            new TeleportationEffect("Teleport To The LS Airport", "BringMeToTheLSAirport", Location.LSAirport),
            new TeleportationEffect("Teleport To The Docks", "BringMeToTheDocks", Location.LSDocks),
            new TeleportationEffect("Teleport To A Mountain", "BringMeToAMountain", Location.MountChiliad),
            new TeleportationEffect("Teleport To The SF Airport", "BringMeToTheSFAirport", Location.SFAirport),
            new TeleportationEffect("Teleport To A Bridge", "BringMeToABridge", Location.SFBridge),
            new TeleportationEffect("Teleport To A Secret Place", "BringMeToASecret", Location.Area52),
            new TeleportationEffect("Teleport To A Quarry", "BringMeToAQuarry", Location.LVQuarry),
            new TeleportationEffect("Teleport To The LV Airport", "BringMeToTheLVAirport", Location.LVAirport),
            new TeleportationEffect("Teleport To The Dish", "BringMeToTheDish", Location.LVSatellite),
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
