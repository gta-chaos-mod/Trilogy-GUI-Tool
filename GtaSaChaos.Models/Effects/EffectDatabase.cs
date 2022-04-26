// Copyright (c) 2019 Lordmau5
using GtaChaos.Models.Effects.@abstract;
using GtaChaos.Models.Effects.extra;
using GtaChaos.Models.Effects.impl;
using GtaChaos.Models.Utils;
using System;
using System.Collections.Generic;

namespace GtaChaos.Models.Effects
{
    public static class EffectDatabase
    {
        public static List<AbstractEffect> Effects { get; } = new List<AbstractEffect> { };

        public static void PopulateEffects(string game)
        {
            foreach (Category cat in Category.Categories)
            {
                cat.ClearEffects();
            }
            Effects.Clear();
            EnabledEffects.Clear();

            if (game == "san_andreas")
            {
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 1", "ThugsArmoury", "weapon_set_1")); // Weapon Set 1
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 2", "ProfessionalsKit", "weapon_set_2")); // Weapon Set 2
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 3", "NuttersToys", "weapon_set_3")); // Weapon Set 3
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 4", "MinigunMadness", "weapon_set_4")); // Weapon Set 4
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Health, Armor, $250k", "INeedSomeHelp", "health_armor_money")); // Health, Armor, $250k
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Suicide", "GoodbyeCruelWorld", "suicide").DisableRapidFire()); // Suicide
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Infinite Ammo", "FullClip", "infinite_ammo")); // Infinite ammo
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Infinite Health (Player)", "NoOneCanHurtMe", "infinite_health_player")); // Infinite Health (Player)

                Effects.Add(new FunctionEffect(Category.WantedLevel, "Wanted Level +2 Stars", "TurnUpTheHeat", "wanted_level_plus_two")); // Wanted level +2 stars
                Effects.Add(new FunctionEffect(Category.WantedLevel, "Clear Wanted Level", "TurnDownTheHeat", "clear_wanted_level")); // Clear wanted level
                Effects.Add(new FunctionEffect(Category.WantedLevel, "Never Wanted", "IDoAsIPlease", "never_wanted")); // Never wanted
                Effects.Add(new FunctionEffect(Category.WantedLevel, "Six Wanted Stars", "BringItOn", "wanted_level_six_stars").DisableRapidFire()); // Six wanted stars

                Effects.Add(new WeatherEffect("Sunny Weather", "PleasantlyWarm", 1)); // Sunny weather
                Effects.Add(new WeatherEffect("Very Sunny Weather", "TooDamnHot", 0)); // Very sunny weather
                Effects.Add(new WeatherEffect("Overcast Weather", "DullDullDay", 4)); // Overcast weather
                Effects.Add(new WeatherEffect("Rainy Weather", "StayInAndWatchTV", 16)); // Rainy weather
                Effects.Add(new WeatherEffect("Foggy Weather", "CantSeeWhereImGoing", 9)); // Foggy weather
                Effects.Add(new WeatherEffect("Sandstorm", "SandInMyEars", 19)); // Sandstorm
                Effects.Add(new WeatherEffect("Weather?!", "WhatsThisWeatherAbout", 131, 1000 * 10)); // Sand storm

                Effects.Add(new FunctionEffect(Category.Spawning, "Get Parachute", "LetsGoBaseJumping", "get_parachute")); // Get Parachute
                Effects.Add(new FunctionEffect(Category.Spawning, "Get Jetpack", "Rocketman", "get_jetpack")); // Get Jetpack
                Effects.Add(new SpawnVehicleEffect("TimeToKickAss", 432)); // Spawn Rhino
                Effects.Add(new SpawnVehicleEffect("OldSpeedDemon", 504)); // Spawn Bloodring Banger
                Effects.Add(new SpawnVehicleEffect("DoughnutHandicap", 489)); // Spawn Rancher
                Effects.Add(new SpawnVehicleEffect("NotForPublicRoads", 502)); // Spawn Hotring A
                Effects.Add(new SpawnVehicleEffect("JustTryAndStopMe", 503)); // Spawn Hotring B
                Effects.Add(new SpawnVehicleEffect("WheresTheFuneral", 442)); // Spawn Romero
                Effects.Add(new SpawnVehicleEffect("CelebrityStatus", 409)); // Spawn Stretch
                Effects.Add(new SpawnVehicleEffect("TrueGrime", 408)); // Spawn Trashmaster
                Effects.Add(new SpawnVehicleEffect("18Holes", 457)); // Spawn Caddy
                Effects.Add(new SpawnVehicleEffect("JumpJet", 520)); // Spawn Hydra
                Effects.Add(new SpawnVehicleEffect("IWantToHover", 539)); // Spawn Vortex
                Effects.Add(new SpawnVehicleEffect("OhDude", 425)); // Spawn Hunter
                Effects.Add(new SpawnVehicleEffect("FourWheelFun", 471)); // Spawn Quad
                Effects.Add(new SpawnVehicleEffect("ItsAllBull", 486)); // Spawn Dozer
                Effects.Add(new SpawnVehicleEffect("FlyingToStunt", 513)); // Spawn Stunt Plane
                Effects.Add(new SpawnVehicleEffect("MonsterMash", 556)); // Spawn Monster
                Effects.Add(new SpawnVehicleEffect("SurpriseDriver", -1)); // Spawn Random Vehicle

                Effects.Add(new FunctionEffect(Category.Time, "0.25x Game Speed", "MatrixMode", "quarter_game_speed", -1, 1.0f)); // Quarter Gamespeed
                Effects.Add(new FunctionEffect(Category.Time, "0.5x Game Speed", "SlowItDown", "half_game_speed", -1, 2.0f)); // Half Gamespeed
                Effects.Add(new FunctionEffect(Category.Time, "2x Game Speed", "SpeedItUp", "double_game_speed")); // Double Gamespeed
                Effects.Add(new FunctionEffect(Category.Time, "4x Game Speed", "YoureTooSlow", "quadruple_game_speed")); // Quadruple Gamespeed
                Effects.Add(new FunctionEffect(Category.Time, "Always Midnight", "NightProwler", "always_midnight")); // Always midnight
                Effects.Add(new FunctionEffect(Category.Time, "Stop Game Clock", "DontBringOnTheNight", "stop_game_clock")); // Stop game clock, orange sky
                Effects.Add(new FunctionEffect(Category.Time, "Faster Clock", "TimeJustFliesBy", "faster_clock")); // Faster clock

                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Blow Up All Cars", "AllCarsGoBoom", "blow_up_all_cars").DisableRapidFire()); // Blow up all cars
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Pink Traffic", "PinkIsTheNewCool", "pink_traffic")); // Pink traffic
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Black Traffic", "SoLongAsItsBlack", "black_traffic")); // Black traffic
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Cheap Cars", "EveryoneIsPoor", "cheap_cars")); // Cheap cars
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Expensive Cars", "EveryoneIsRich", "expensive_cars")); // Expensive cars
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Insane Handling", "StickLikeGlue", "insane_handling")); // Insane handling
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "All Green Lights", "DontTryAndStopMe", "all_green_lights")); // All green lights
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Cars On Water", "JesusTakeTheWheel", "cars_on_water")); // Cars on water
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Boats Fly", "FlyingFish", "boats_fly")); // Boats fly
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Cars Fly", "ChittyChittyBangBang", "cars_fly")); // Cars fly
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Smash N' Boom", "TouchMyCarYouDie", "smash_n_boom")); // Smash n' boom
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "All Cars Have Nitro", "SpeedFreak", "all_cars_have_nitro")); // All cars have nitro
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Cars Float Away When Hit", "BubbleCars", "cars_float_away_when_hit")); // Cars float away when hit
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "All Taxis Have Nitrous", "SpeedyTaxis", "all_taxis_have_nitro")); // All taxis have nitrous
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Invisible Vehicles (Only Wheels)", "WheelsOnlyPlease", "wheels_only_please")); // Invisible Vehicles (Only Wheels)

                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Peds Attack Each Other", "RoughNeighbourhood", "peds_attack_each_other")); // Peds attack other (+ get golf club)
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Have A Bounty On Your Head", "StopPickingOnMe", "have_a_bounty_on_your_head")); // Have a bounty on your head
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Elvis Is Everywhere", "BlueSuedeShoes", "elvis_is_everywhere")); // Elvis is everywhere
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Peds Attack You", "AttackOfTheVillagePeople", "peds_attack_you")); // Peds attack you
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Gang Members Everywhere", "OnlyHomiesAllowed", "gang_members_everywhere")); // Gang members everywhere
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Gangs Control The Streets", "BetterStayIndoors", "gangs_control_the_streets")); // Gangs control the streets
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Riot Mode", "StateOfEmergency", "riot_mode")); // Riot mode
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Everyone Armed", "SurroundedByNutters", "everyone_armed")); // Everyone armed
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Aggressive Drivers", "AllDriversAreCriminals", "aggressive_drivers")); // Aggressive drivers
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Recruit Anyone (9mm)", "WannaBeInMyGang", "recruit_anyone_9mm")); // Recruit anyone (9mm)
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Recruit Anyone (AK-47)", "NoOneCanStopUs", "recruit_anyone_ak47")); // Recruit anyone (AK-47)
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Recruit Anyone (Rockets)", "RocketMayhem", "recruit_anyone_rockets")); // Recruit anyone (Rockets)
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Ghost Town", "GhostTown", "ghost_town")); // Reduced traffic
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Beach Party", "LifesABeach", "beach_theme")); // Beach party
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Ninja Theme", "NinjaTown", "ninja_theme")); // Ninja theme
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Kinky Theme", "LoveConquersAll", "kinky_theme")); // Kinky theme
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Funhouse Theme", "CrazyTown", "funhouse_theme")); // Funhouse theme
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Country Traffic", "HicksVille", "country_traffic")); // Country traffic

                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Weapon Aiming While Driving", "IWannaDriveBy", "weapon_aiming_while_driving")); // Weapon aiming while driving
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Huge Bunny Hop", "CJPhoneHome", "huge_bunny_hop")); // Huge bunny hop
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Mega Jump", "Kangaroo", "mega_jump")); // Mega jump
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Infinite Oxygen", "ManFromAtlantis", "infinite_oxygen")); // Infinite oxygen
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Mega Punch", "StingLikeABee", "mega_punch")); // Mega punch

                Effects.Add(new FunctionEffect(Category.Stats, "Fat Player", "WhoAteAllThePies", "fat_player")); // Fat player
                Effects.Add(new FunctionEffect(Category.Stats, "Max Muscle", "BuffMeUp", "max_muscle")); // Max muscle
                Effects.Add(new FunctionEffect(Category.Stats, "Skinny Player", "LeanAndMean", "skinny_player")); // Skinny player
                Effects.Add(new FunctionEffect(Category.Stats, "Max Stamina", "ICanGoAllNight", "max_stamina")); // Max stamina
                Effects.Add(new FunctionEffect(Category.Stats, "No Stamina", "ImAllOutOfBreath", "no_stamina")); // No stamina
                Effects.Add(new FunctionEffect(Category.Stats, "Hitman Level For All Weapons", "ProfessionalKiller", "hitman_level_for_all_weapons")); // Hitman level for all weapons
                Effects.Add(new FunctionEffect(Category.Stats, "Beginner Level For All Weapons", "BabysFirstGun", "beginner_level_for_all_weapons")); // Beginner level for all weapons
                Effects.Add(new FunctionEffect(Category.Stats, "Max Driving Skills", "NaturalTalent", "max_driving_skills")); // Max driving skills
                Effects.Add(new FunctionEffect(Category.Stats, "No Driving Skills", "BackToDrivingSchool", "no_driving_skills")); // No driving skills
                Effects.Add(new FunctionEffect(Category.Stats, "Never Get Hungry", "IAmNeverHungry", "never_get_hungry")); // Never get hungry
                Effects.Add(new FunctionEffect(Category.Stats, "Lock Respect At Max", "WorshipMe", "lock_respect_at_max")); // Lock respect at max
                Effects.Add(new FunctionEffect(Category.Stats, "Lock Sex Appeal At Max", "HelloLadies", "lock_sex_appeal_at_max")); // Lock sex appeal at max

                Effects.Add(new FunctionEffect(Category.CustomEffects, "Clear Active Effects", "ClearActiveEffects", "clear_active_effects").DisableTwitch()); // Clear Active Effects
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Remove All Weapons", "NoWeaponsAllowed", "remove_all_weapons")); // Remove all weapons
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Get Busted", "GoToJail", "get_busted").DisableRapidFire()); // Get's you busted on the spot
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Get Wasted", "Hospitality", "get_wasted").DisableRapidFire()); // Get's you wasted on the spot
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Set Everyone On Fire", "HotPotato", "set_everyone_on_fire").DisableRapidFire()); // Set everyone on fire
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Kick Player Out Of Vehicle", "ThisAintYourCar", "kick_player_out_of_vehicle")); // Kick player out of vehicle
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Lock Player Inside Vehicle", "ThereIsNoEscape", "lock_player_inside_vehicle")); // Lock player inside vehicle
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Set Current Vehicle On Fire", "WayTooHot", "set_current_vehicle_on_fire").DisableRapidFire()); // Set current vehicle on fire
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Pop Tires Of All Vehicles", "TiresBeGone", "pop_tires_of_all_vehicles")); // Pop tires of all vehicles
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Send Vehicles To Space", "StairwayToHeaven", "send_vehicles_to_space")); // Gives an immense upwards boost to all vehicles
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Turn Vehicles Around", "TurnAround", "turn_vehicles_around")); // Turn vehicles around
                Effects.Add(new FunctionEffect(Category.CustomEffects, "To The Left, To The Right", "ToTheLeftToTheRight", "to_the_left_to_the_right")); // Gives cars a random velocity
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Timelapse Mode", "DiscoInTheSky", "timelapse")); // Timelapse mode
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Where Is Everybody?", "ImHearingVoices", "where_is_everybody")); // Where is everybody?
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Everybody Bleed Now!", "EverybodyBleedNow", "everybody_bleed_now", -1, 1.5f)); // Everybody bleed now!
                Effects.Add(new FunctionEffect(Category.CustomEffects, "To Drive Or Not To Drive", "ToDriveOrNotToDrive", "to_drive_or_not_to_drive")); // To drive or not to drive
                Effects.Add(new FunctionEffect(Category.CustomEffects, "One Hit K.O.", "ILikeToLiveDangerously", "one_hit_ko").DisableRapidFire()); // One Hit K.O.
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Experience The Lag", "PacketLoss", "experience_the_lag")); // Experience the lag
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Ghost Rider", "GhostRider", "ghost_rider")); // Set current vehicle constantly on fire
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Disable HUD", "FullyImmersed", "disable_hud")); // Disable HUD
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Disable Blips / Markers / Pickups", "INeedSomeInstructions", "disable_blips_markers_pickups")); // Disable Blips / Markers / Pickups
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Disable All Weapon Damage", "TruePacifist", "disable_all_weapon_damage")); // Disable all Weapon Damage
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Let's Take A Break", "LetsTakeABreak", "lets_take_a_break").DisableRapidFire()); // Let's take a break
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Pride Traffic", "AllColorsAreBeautiful", "pride_traffic")); // Pride Traffic / Rainbow Cars
                Effects.Add(new FunctionEffect(Category.CustomEffects, "High Suspension Damping", "VeryDampNoBounce", "high_suspension_damping")); // Cars have high suspension damping
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Zero Suspension Damping", "LowrideAllNight", "zero_suspension_damping"));  // Cars have almost zero suspension damping
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Long Live The Rich!", "LongLiveTheRich", "long_live_the_rich"));  // Money = Health, shoot people to gain money, take damage to lose it
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Inverted Controls", "InvertedControls", "inverted_controls"));  // Inverts some controls
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Disable One Movement Key", "DisableOneMovementKey", "disable_one_movement_key"));  // Disable one movement key
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Fail Current Mission", "MissionFailed", "fail_current_mission").DisableRapidFire()); // Fail Current Mission
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Night Vision", "NightVision", "night_vision")); // Night Vision
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Thermal Vision", "ThermalVision", "thermal_vision")); // Thermal Vision
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Pass Current Mission", "IllTakeAFreePass", "pass_current_mission")); // Pass Current Mission
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Infinite Health (Everyone)", "NoOneCanHurtAnyone", "infinite_health_everyone")); // Infinite Health (Everyone)
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Invisible Vehicles", "InvisibleVehicles", "invisible_vehicles")); // Invisible Vehicles
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Powerpoint Presentation", "PowerpointPresentation", "framerate_15")); // Powerpoint Presentation (15 FPS)
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Smooth Criminal", "SmoothCriminal", "framerate_60")); // Smooth Criminal (60 FPS)
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Reload Autosave", "HereWeGoAgain", "reload_autosave").DisableRapidFire()); // Reload Autosave
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Quarter Gravity", "GroundControlToMajorTom", "quarter_gravity")); // Sets the gravity to 0.002f
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Half Gravity", "ImFeelingLightheaded", "half_gravity")); // Sets the gravity to 0.004f
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Double Gravity", "KilogramOfFeathers", "double_gravity")); // Sets the gravity to 0.016f
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Quadruple Gravity", "KilogramOfSteel", "quadruple_gravity")); // Sets the gravity to 0.032f
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Inverted Gravity", "BeamMeUpScotty", "inverted_gravity").DisableRapidFire()); // Sets the gravity to -0.002f
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Zero Gravity", "ImInSpaaaaace", "zero_gravity").DisableRapidFire()); // Sets the gravity to 0f
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Insane Gravity", "StraightToHell", "insane_gravity").DisableRapidFire()); // Sets the gravity to 0.64f
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Tunnel Vision", "TunnelVision", "tunnel_vision", -1, 1.0f).DisableRapidFire()); // Tunnel Vision
                Effects.Add(new FunctionEffect(Category.CustomEffects, "High Pitched Audio", "CJAndTheChipmunks", "high_pitched_audio")); // High Pitched Audio
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Pitch Shifter", "VocalRange", "pitch_shifter")); // Pitch Shifter
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Pass Current Mission", "IWontTakeAFreePass", "fake_pass_current_mission").DisableRapidFire()); // Fake Pass Current Mission
                Effects.Add(new FunctionEffect(Category.CustomEffects, "DVD Screensaver", "ItsGonnaHitTheCorner", "dvd_screensaver", -1, 1.0f).DisableRapidFire()); // DVD Screensaver
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Honk Boost", "GottaHonkFast", "honk_boost")); // Honk Boost
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Oh Hey, Tanks!", "OhHeyTanks", "oh_hey_tanks")); // Spawns tanks around the player
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Always Wanted", "ICanSeeStars", "always_wanted").DisableRapidFire()); // Always Wanted
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Cinematic Vehicle Camera", "MachinimaMode", "cinematic_vehicle_camera")); // Cinematic Vehicle Camera
                //Effects.Add(new FunctionEffect(Category.CustomEffects, "Top Down Camera", "ReturnToTheClassics", "top_down_camera")); // Top Down Camera - BROKEN - CAN FLICKER AND SOFTLOCK
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Your Car Floats Away When Hit", "ImTheBubbleCar", "your_car_floats_away_when_hit")); // Your Car Floats Away When Hit
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Ring Ring !!", "RingRing", "ring_ring")); // Ring Ring !!
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Peds Explode When Run Over", "ExplosivePeds", "peds_explode_when_run_over")); // Peds Explode When Run Over
                Effects.Add(new FunctionEffect(Category.CustomEffects, "HONK!!!", "HONKHONK", "honk_vehicle").SetAudioVariations(5)); // Honk Vehicle
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Quake FOV", "QuakeFOV", "quake_fov")); // Quake FOV
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Beyblade", "LetItRip", "beyblade")); // Beyblade
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Weapon Roulette", "WeaponRoulette", "weapon_roulette")); // Weapon Roulette
                Effects.Add(new FunctionEffect(Category.CustomEffects, "No Need To Hurry", "NoNeedToHurry", "no_need_to_hurry")); // No Need To Hurry
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Drunk Player", "DrunkPlayer", "drunk_player")); // Drunk Player
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Force Vehicle Mouse Steering", "ForceVehicleMouseSteering", "force_vehicle_mouse_steering")); // Force Vehicle Mouse Steering
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Upside-Down Screen", "WhatsUpIsDown", "upside_down_screen", -1, 1.0f)); // Upside-Down Screen
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Mirrored Screen", "WhatsLeftIsRight", "mirrored_screen", -1, 1.0f)); // Mirrored Screen
                //Effects.Add(new FunctionEffect(Category.CustomEffects, "Greyscale Screen", "GreyscaleScreen", "greyscale_screen", -1, 1.0f / 3.0f)); // Greyscale Screen
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Mirrored World", "LetsTalkAboutParallelUniverses", "mirrored_world")); // Mirrored World
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Pedal To The Metal", "PedalToTheMetal", "pedal_to_the_metal")); // Pedal To The Metal
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Unflippable Vehicles", "ThereGoesMyBurrito", "unflippable_vehicles")); // Vehicles are unflippable
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Freefall!", "WhereWeDroppingBoys", "freefall")); // Freefall! - Gives CJ a parachute and teleports him very high
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Carmageddon", "Carmageddon", "carmageddon")); // Carmageddon - Makes vehicles rain from the sky!
                Effects.Add(new FunctionEffect(Category.CustomEffects, "The Flash", "FastestManAlive", "the_flash")); // The Flash - Let's you run and swim at incredibly high speeds while not taking fall damage or drowing!
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Roll Credits", "WaitItsOver", "roll_credits")); // Roll Credits - Rolls the credits but only visually!
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Instantly Hungry", "IllHave2Number9s", "instantly_hungry")); // Instantly Hungry - Makes CJ instantly hungry
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Vehicle Bumper Camera", "FrontRowSeat", "vehicle_bumper_camera")); // Vehicle Bumper Camera - Forces the vehicle's bumper camera

                // New Effects
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Tiny Cars", "TinyPeepoHappy", "vehicle_size_tiny"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Large Cars", "LargePeepoHappy", "vehicle_size_large"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Wide Cars", "WidePeepoHappy", "vehicle_size_wide"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Super Wide Cars", "WiderPeepoHappy", "vehicle_size_super_wide"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Tall Cars", "TallPeepoHappy", "vehicle_size_tall"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Long Cars", "LongPeepoHappy", "vehicle_size_long"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Paper Thin Cars", "PaperPeepoHappy", "vehicle_size_paper_thin"));

                Effects.Add(new FunctionEffect(Category.CustomEffects, "Backwards Cars", "BackPeepoHappy", "vehicle_size_backwards"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Flipped Cars", "FlippedPeepoHappy", "vehicle_size_flipped"));

                Effects.Add(new FunctionEffect(Category.CustomEffects, "Rotating Cars", "RotatePeepoHappy", "vehicle_size_continuous_rotation"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Speed-based Rotating Cars", "SpeedRotatePeepoHappy", "vehicle_rotation_based_on_speed")); 
                
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Relative Car Gravity", "SpiderCars", "vehicle_driving_on_walls"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Vehicle Boost", "FullForceForward", "vehicle_boost"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Invert Vehicle Speed", "LetsGoBack", "invert_vehicle_speed"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Rubberbanding", "ImGettingAllDizzy", "rubberbanding"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "It's Rewind Time!", "ItsRewindTime", "its_rewind_time"));

                Effects.Add(new FunctionEffect(Category.CustomEffects, "Big Heads", "BigHeadsMode", "big_heads"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Tiny Peds", "SmolPeepoPeds", "ped_size_tiny"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Large Peds", "LargPeepoPeds", "ped_size_large"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Long Necks", "ICanSeeMyHouseFromUpHere", "long_necks"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Hold The F Up...", "HoldTheFUp", "hold_the_f_up"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Don't Lose Your Head", "DontLoseYourHead", "dont_lose_your_head"));

                Effects.Add(new FunctionEffect(Category.CustomEffects, "Backwards Peds", "BackwardsPeds", "backwards_peds"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Upside-Down Peds", "InALandDownUnder", "upside_down_peds"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Helicopter Helicopter!", "HelicopterPeds", "helicopter_peds"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "ASSERT DOMINANCE", "AssertDominance", "t_pose_peds"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Action Figures", "ILikePlayingWithToys", "action_figures"));

                Effects.Add(new FunctionEffect(Category.CustomEffects, "One Bullet Magazines", "OneInTheChamber", "one_bullet_magazines"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Everyone Leaves All Vehicles", "ImTiredOfDriving", "everyone_leaves_all_vehicles"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Give Peds An RPG", "RocketParty", "give_peds_an_rpg"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Teleport All Peds To Player", "WhoAreYouPeople", "teleport_all_peds_to_player"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Launch All Peds", "RocketParty", "launch_all_peds"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Explode All Peds", "BoomGoesTheDynamite", "explode_all_peds"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Disarm All Peds", "LeaveTheGunsToMe", "disarm_all_peds"));

                Effects.Add(new FunctionEffect(Category.CustomEffects, "Reset Camera", "NaturalView", "reset_camera"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Freeze Vehicle", "StuckInTime", "freeze_vehicle", 1000 * 5));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Delete Vehicle", "GoodbyeSweetRide", "delete_vehicle"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Delete All Vehicles", "GoodbyeAllSweetRides", "delete_all_vehicles"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "No Tasks Allowed", "NoTasksAllowed", "no_tasks_allowed", 1000 * 10));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Walk Off", "LetsGoForAWalk", "walk_off", 1000 * 5));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Pause", "LetsPause", "pause"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Fake Crash", "TooManyModsInstalled", "fake_crash"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Walk On Water", "JesusInTheHouse", "walk_on_water"));

                Effects.Add(new FunctionEffect(Category.CustomEffects, "Bankrupt", "CrashTookAllMyMoney", "bankrupt"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Millionaire", "IJustWonTheLottery", "millionaire"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Switch To Unarmed", "PleaseUseYourFists", "switch_to_unarmed"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Blind", "WhoTurnedTheLightsOff", "blind", 1000 * 10));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Drive Wander", "Autopilot", "drive_wander", 1000 * 10));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Steer Bias (Left)", "LeftSideBias", "steer_bias_left"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Steer Bias (Right)", "RightSideBias", "steer_bias_right"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "The Firing Circus", "TheFiringCircus", "the_firing_circus", 1000 * 5));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Random Outfit", "ASetOfNewClothes", "random_outfit"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "No Shooting Allowed", "GunsAreDangerous", "no_shooting_allowed"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Pacifist", "TillDeathDoUsPart", "pacifist"));

                Effects.Add(new FunctionEffect(Category.CustomEffects, "No Visible Water", "OceanManGoneAgain", "no_visible_water"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "No Water Physics", "LetsVisitAtlantis", "no_water_physics"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Underwater View", "HelloHowAreYouIAmUnderTheWater", "underwater_view"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Galaxy Note 7", "DangerousPhoneCalls", "galaxy_note_7"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Freeze Radar", "OutdatedMaps", "freeze_radar"));

                // All teleports are disabled during Rapid-Fire mode
                Effects.Add(new TeleportationEffect("Teleport Home", "BringMeHome", Location.GrooveStreet));
                Effects.Add(new TeleportationEffect("Teleport To A Tower", "BringMeToATower", Location.LSTower));
                Effects.Add(new TeleportationEffect("Teleport To A Pier", "BringMeToAPier", Location.LSPier));
                Effects.Add(new TeleportationEffect("Teleport To The LS Airport", "BringMeToTheLSAirport", Location.LSAirport));
                Effects.Add(new TeleportationEffect("Teleport To The Docks", "BringMeToTheDocks", Location.LSDocks));
                Effects.Add(new TeleportationEffect("Teleport To A Mountain", "BringMeToAMountain", Location.MountChiliad));
                Effects.Add(new TeleportationEffect("Teleport To The SF Airport", "BringMeToTheSFAirport", Location.SFAirport));
                Effects.Add(new TeleportationEffect("Teleport To A Bridge", "BringMeToABridge", Location.SFBridge));
                Effects.Add(new TeleportationEffect("Teleport To A Secret Place", "BringMeToASecretPlace", Location.Area52));
                Effects.Add(new TeleportationEffect("Teleport To A Quarry", "BringMeToAQuarry", Location.LVQuarry));
                Effects.Add(new TeleportationEffect("Teleport To The LV Airport", "BringMeToTheLVAirport", Location.LVAirport));
                Effects.Add(new TeleportationEffect("Teleport To Big Ear", "BringMeToBigEar", Location.LVSatellite));
                Effects.Add(new FunctionEffect(Category.Teleportation, "Random Teleport", "LetsGoSightseeing", "random_teleport")); // Random Teleport - Teleports CJ to a random location on the map
                Effects.Add(new FunctionEffect(Category.Teleportation, "Fake Teleport", "HahaGotYourNose", "fake_teleport", 1000 * 3)); // Fake Teleport
                Effects.Add(new FunctionEffect(Category.Teleportation, "Teleport To Marker", "IKnowJustTheRightPlace", "teleport_to_marker", 1000 * 30)); // Fake Teleport
            }
            else if (game == "vice_city")
            {
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 1", "ThugsTools", "weapon_set_1"));
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 2", "ProfessionalTools", "weapon_set_2"));
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 3", "NutterTools", "weapon_set_3"));
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Full Health", "Aspirine", "full_health"));
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Full Armor", "PreciousProtection", "full_armor"));
                Effects.Add(new FunctionEffect(Category.WeaponsAndHealth, "Suicide", "ICantTakeItAnymore", "suicide"));

                Effects.Add(new FunctionEffect(Category.WantedLevel, "Clear Wanted Level", "LeaveMeAlone", "clear_wanted_level"));
                Effects.Add(new FunctionEffect(Category.WantedLevel, "Increase Wanted Level", "YouWontTakeMeAlive", "increase_wanted_level"));

                Effects.Add(new WeatherEffect("Cloudy Weather", "APleasantDay", 0));
                Effects.Add(new WeatherEffect("Very Cloudy Weather", "ABitDrieg", 1));
                Effects.Add(new WeatherEffect("Rainy Weather", "CatsAndDogs", 2));
                Effects.Add(new WeatherEffect("Foggy Weather", "CantSeeAThing", 3));
                Effects.Add(new WeatherEffect("Sunny Weather", "ALovelyDay", 4));

                Effects.Add(new SpawnVehicleEffect("RubbishCar", 138)); // Trashmaster
                Effects.Add(new SpawnVehicleEffect("Panzer", 162)); // Tank
                Effects.Add(new SpawnVehicleEffect("TheLastRide", 172)); // Hearse
                Effects.Add(new SpawnVehicleEffect("BetterThanWalking", 187)); // Caddie
                Effects.Add(new SpawnVehicleEffect("RockAndRollCar", 201)); // Limo
                Effects.Add(new SpawnVehicleEffect("GetThereFast", 206)); // Sabre Turbo
                Effects.Add(new SpawnVehicleEffect("GetThereVeryFastIndeed", 232)); // Hotring Racer
                Effects.Add(new SpawnVehicleEffect("GetThereAmazinglyFast", 233)); // Alt. Hotring Racer
                Effects.Add(new SpawnVehicleEffect("TravelInStyle", 234)); // Bloodring Banger
                Effects.Add(new SpawnVehicleEffect("GetThereQuickly", 235)); // Alt. Bloodring Banger

                Effects.Add(new FunctionEffect(Category.Time, "0.25x Game Speed", "MatrixMode", "quarter_game_speed"));
                Effects.Add(new FunctionEffect(Category.Time, "0.5x Game Speed", "Booooooring", "half_game_speed"));
                Effects.Add(new FunctionEffect(Category.Time, "2x Game Speed", "OnSpeed", "double_game_speed"));
                Effects.Add(new FunctionEffect(Category.Time, "4x Game Speed", "YoureTooSlow", "quadruple_game_speed"));
                Effects.Add(new FunctionEffect(Category.Time, "Quick Clock", "LifeIsPassingMeBy", "quick_clock"));

                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Aggressive Drivers", "MiamiTraffic", "aggressive_drivers"));
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "All Green Lights", "GreenLight", "all_green_lights"));
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Big Wheels", "LoadsOfLittleThings", "big_wheels"));
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Black Cars", "IWantItPaintedBlack", "black_cars"));
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Cars On Water", "Seaways", "cars_on_water"));
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Explode All Vehicles", "BigBang", "explode_all_vehicles"));
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Flying Boats", "Airship", "flying_boats"));
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Flying Cars", "ComeFlyWithMe", "flying_cars"));
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Insane Handling", "GripIsEverything", "insane_handling"));
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Pink Cars", "AHairDressersCars", "pink_cars"));
                Effects.Add(new FunctionEffect(Category.VehiclesTraffic, "Wheels Only", "WheelsAreAllINeed", "wheels_only"));

                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Armed Female Pedestrians", "ChicksWithGuns", "armed_female_peds"));
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Armed Pedestrians", "OurGodGivenRightToBearArms", "armed_peds"));
                //Effects.Add(new FunctionEffect(Category.PedsAndCo, "Hostile Peds", "NobodyLikesMe", "hostile_peds"));
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Ladies' Man", "FannyMagnet", "ladies_man"));
                Effects.Add(new FunctionEffect(Category.PedsAndCo, "Peds Get In Your Car", "HopInGirl", "peds_get_in_your_car"));
                //Effects.Add(new FunctionEffect(Category.PedsAndCo, "Riot Mode", "FightFightFight", "riot"));

                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Candy Suxxx Skin", "IWantBigTits", "candy_suxxx_skin"));
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Dick Skin", "WeLoveOurDick", "dick_skin"));
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Fat Skin", "DeepFriedMarsBars", "fat_skin"));
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Hilary Skin", "ILookLikeHilary", "hilary_skin"));
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Jezz Skin", "RockAndRollMan", "jezz_skin"));
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Ken Skin", "MySonIsALawyer", "ken_skin"));
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Lance Skin", "LookLikeLance", "lance_skin"));
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Mercedes Skin", "FoxyLittleThing", "mercedes_skin"));
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Phil Skin", "OneArmedBandit", "phil_skin"));
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Random Outfit", "StillLikeDressingUp", "random_outfit"));
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Ricardo Skin", "CheatsHaveBeenCracked", "ricardo_skin"));
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Skinny Skin", "Programmer", "skinny_skin"));
                Effects.Add(new FunctionEffect(Category.PlayerModifications, "Sonny Skin", "IDontHaveTheMoneySonny", "sonny_skin"));

                // Custom Effects
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Disable HUD", "DisableHUD", "disable_hud"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Disable Radar Blips", "DisableRadarBlips", "disable_radar_blips"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "DVD Screensaver", "DVDScreensaver", "dvd_screensaver"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Tunnel Vision", "TunnelVision", "tunnel_vision"));

                Effects.Add(new FunctionEffect(Category.CustomEffects, "Disable All Weapon Damage", "DisableAllWeaponDamage", "disable_all_weapon_damage"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Everybody Bleed Now", "EverybodyBleedNow", "everybody_bleed_now"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Infinite Health (Everyone)", "InfiniteHealthEveryone", "infinite_health_everyone"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Long Live The Rich", "LongLiveTheRich", "long_live_the_rich"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "One Hit K.O.", "OneHitKO", "one_hit_ko"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Remove All Weapons", "RemoveAllWeapons", "remove_all_weapons"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Set All Peds On Fire", "SetAllPedsOnFire", "set_all_peds_on_fire"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Where Is Everybody", "WhereIsEverybody", "where_is_everybody"));

                Effects.Add(new FunctionEffect(Category.CustomEffects, "Experience The Lag", "ExperienceTheLag", "experience_the_lag"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "15 FPS", "15FPS", "fps_15"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "60 FPS", "60FPS", "fps_60"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Timelapse", "Timelapse", "timelapse"));

                Effects.Add(new FunctionEffect(Category.CustomEffects, "Reload Autosave", "ReloadAutosave", "reload_autosave"));

                //Effects.Add(new FunctionEffect(Category.CustomEffects, "Ghost Rider", "GhostRider", "ghost_rider"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "High Suspension Damping", "HighSuspensionDamping", "high_suspension_damping"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Invisible Vehicles", "InvisibleVehicles", "invisible_vehicles"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Lightspeed Braking", "LightspeedBraking", "lightspeed_braking"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Little Suspension Damping", "LittleSuspensionDamping", "little_suspension_damping"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Pop Tires Of All Vehicles", "PopTiresOfAllVehicles", "pop_tires_of_all_vehicles"));
                //Effects.Add(new FunctionEffect(Category.CustomEffects, "Rainbow Cars", "RainbowCars", "rainbow_cars"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Send Vehicles To Space", "SendVehiclesToSpace", "send_vehicles_to_space"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Set Current Vehicle On Fire", "SetCurrentVehicleOnFire", "set_current_vehicle_on_fire"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "To Drive Or Not To Drive", "ToDriveOrNotToDrive", "to_drive_or_not_to_drive"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "To The Left, To The Right", "ToTheLeftToTheRight", "to_the_left_to_the_right"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Turn Vehicles Around", "TurnVehiclesAround", "turn_vehicles_around"));
                Effects.Add(new FunctionEffect(Category.CustomEffects, "Zero Suspension Damping", "ZeroSuspensionDamping", "zero_suspension_damping"));
            }
        }

        public static List<AbstractEffect> EnabledEffects { get; } = new List<AbstractEffect>();

        public static AbstractEffect GetByID(string id, bool onlyEnabled = false)
        {
            return (onlyEnabled ? EnabledEffects : Effects).Find(e => e.GetId().Equals(id));
        }

        public static AbstractEffect GetByWord(string word, bool onlyEnabled = false)
        {
            return (onlyEnabled ? EnabledEffects : Effects).Find(e => !string.IsNullOrEmpty(e.Word) && string.Equals(e.Word, word, StringComparison.OrdinalIgnoreCase));
        }

        public static AbstractEffect GetByDescription(string description, bool onlyEnabled = false)
        {
            return (onlyEnabled ? EnabledEffects : Effects).Find(e => string.Equals(description, e.GetDisplayName(), StringComparison.OrdinalIgnoreCase));
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

        public static AbstractEffect RunEffect(AbstractEffect effect, int seed = -1, int duration = -1)
        {
            if (effect != null)
            {
                effect.RunEffect(seed, duration);
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
