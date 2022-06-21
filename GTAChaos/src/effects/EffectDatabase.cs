// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GTAChaos.Effects
{
    public class WeightedRandomBag<T>
    {
        public struct Entry
        {
            public double weight;
            public T item;
        }

        private readonly List<Entry> entries = new();
        private readonly Random rand = new();
        private double AccumulatedWeight = 0;

        public void Add(T item, double weight = 1.0)
        {
            this.entries.Add(new Entry
            {
                item = item,
                weight = weight
            });

            this.CalculateAccumulatedWeight();
        }

        public T GetRandom(Random rand)
        {
            rand ??= this.rand;

            double randomNumber = rand.NextDouble() * this.AccumulatedWeight;

            foreach (Entry entry in this.entries)
            {
                if (entry.weight >= randomNumber)
                {
                    return entry.item;
                }

                randomNumber -= entry.weight;
            }

            return this.Count > 0 ? this.entries[0].item : default;
        }

        private void CalculateAccumulatedWeight()
        {
            this.AccumulatedWeight = 0;
            foreach (Entry entry in this.entries)
            {
                this.AccumulatedWeight += entry.weight;
            }
        }

        public int Count => this.Get().Count;

        public List<Entry> Get() => this.entries;

        public void Add(Entry entry) => this.Add(entry.item, entry.weight);

        public void Remove(Entry item) => this.entries.Remove(item);

        public bool Contains(Entry item) => this.entries.Contains(item);

        public void Sort(Comparison<Entry> comparison) => this.entries.Sort(comparison);

        public Entry Find(Predicate<Entry> match) => this.entries.Find(match);

        public void Clear() => this.entries.Clear();
    }

    public static class EffectDatabase
    {
        public static WeightedRandomBag<AbstractEffect> Effects { get; } = new();

        public static Dictionary<AbstractEffect, int> EffectCooldowns = new();

        private static void AddEffect(AbstractEffect effect, double weight = 1.0)
        {
            if (effect is null)
            {
                return;
            }

            Effects.Add(effect, weight);
        }

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
                // -------------------- Cheats -------------------- //
                // --- NPCs --- //
                AddEffect(new FunctionEffect(Category.NPCs, "Aggressive Drivers", "AllDriversAreCriminals", "aggressive_drivers")); // Aggressive drivers
                AddEffect(new FunctionEffect(Category.NPCs, "Bounty On Your Head", "StopPickingOnMe", "have_a_bounty_on_your_head")); // Have a bounty on your head
                AddEffect(new FunctionEffect(Category.NPCs, "Elvis Is Everywhere", "BlueSuedeShoes", "elvis_is_everywhere")); // Elvis is everywhere
                AddEffect(new FunctionEffect(Category.NPCs, "Everyone Armed", "SurroundedByNutters", "everyone_armed")); // Everyone armed
                AddEffect(new FunctionEffect(Category.NPCs, "Gang Members Everywhere", "OnlyHomiesAllowed", "gang_members_everywhere")); // Gang members everywhere
                AddEffect(new FunctionEffect(Category.NPCs, "Gangs Control The Streets", "BetterStayIndoors", "gangs_control_the_streets")); // Gangs control the streets
                AddEffect(new FunctionEffect(Category.NPCs, "Ghost Town", "GhostTown", "ghost_town")); // Reduced traffic
                AddEffect(new FunctionEffect(Category.NPCs, "NPCs Attack Each Other", "RoughNeighbourhood", "npcs_attack_each_other")); // NPCs attack other (+ get golf club)
                AddEffect(new FunctionEffect(Category.NPCs, "NPCs Attack You", "AttackOfTheVillagePeople", "npcs_attack_you")); // NPCs attack you
                AddEffect(new FunctionEffect(Category.NPCs, "Recruit Anyone (9mm)", "WannaBeInMyGang", "recruit_anyone_9mm")); // Recruit anyone (9mm)
                AddEffect(new FunctionEffect(Category.NPCs, "Recruit Anyone (AK-47)", "NoOneCanStopUs", "recruit_anyone_ak47")); // Recruit anyone (AK-47)
                AddEffect(new FunctionEffect(Category.NPCs, "Recruit Anyone (Rockets)", "RocketMayhem", "recruit_anyone_rockets")); // Recruit anyone (Rockets)
                AddEffect(new FunctionEffect(Category.NPCs, "Riot Mode", "StateOfEmergency", "riot_mode")); // Riot mode

                // --- Themes --- //
                AddEffect(new FunctionEffect(Category.NPCs, "Beach Party", "LifesABeach", "beach_theme")); // Beach party
                AddEffect(new FunctionEffect(Category.NPCs, "Country Traffic", "HicksVille", "country_traffic")); // Country traffic
                AddEffect(new FunctionEffect(Category.NPCs, "Funhouse Theme", "CrazyTown", "funhouse_theme")); // Funhouse theme
                AddEffect(new FunctionEffect(Category.NPCs, "Ninja Theme", "NinjaTown", "ninja_theme")); // Ninja theme
                // -------------- //

                // --- Weapons & Health --- //
                AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Health, Armor, $250k", "INeedSomeHelp", "health_armor_money")); // Health, Armor, $250k
                AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Infinite Ammo", "FullClip", "infinite_ammo")); // Infinite ammo
                AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Invincible (Player)", "NoOneCanHurtMe", "infinite_health_player")); // Infinite Health (Player)
                AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Suicide", "GoodbyeCruelWorld", "suicide").DisableRapidFire()); // Suicide
                AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 1", "ThugsArmoury", "weapon_set_1")); // Weapon Set 1
                AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 2", "ProfessionalsKit", "weapon_set_2")); // Weapon Set 2
                AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 3", "NuttersToys", "weapon_set_3")); // Weapon Set 3
                AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Weapon Set 4", "MinigunMadness", "weapon_set_4")); // Weapon Set 4
                // ------------------------ //

                // --- Stats --- //
                AddEffect(new FunctionEffect(Category.Stats, "Beginner Weapon Skills", "BabysFirstGun", "beginner_level_for_all_weapons")); // Beginner level for all weapons
                AddEffect(new FunctionEffect(Category.Stats, "Fat Player", "WhoAteAllThePies", "fat_player")); // Fat player
                AddEffect(new FunctionEffect(Category.Stats, "Hitman Weapon Skills", "ProfessionalKiller", "hitman_level_for_all_weapons")); // Hitman level for all weapons
                AddEffect(new FunctionEffect(Category.Stats, "Hot CJ In Your Area", "HelloLadies", "lock_sex_appeal_at_max")); // Lock sex appeal at max
                AddEffect(new FunctionEffect(Category.Stats, "Lock Respect At Max", "WorshipMe", "lock_respect_at_max")); // Lock respect at max
                AddEffect(new FunctionEffect(Category.Stats, "Max Driving Skills", "NaturalTalent", "max_driving_skills")); // Max driving skills
                AddEffect(new FunctionEffect(Category.Stats, "Max Lung Capacity", "FilledLungs", "max_lung_capacity")); // Max lung capacity
                AddEffect(new FunctionEffect(Category.Stats, "Max Muscle", "BuffMeUp", "max_muscle")); // Max muscle
                AddEffect(new FunctionEffect(Category.Stats, "Max Stamina", "ICanGoAllNight", "max_stamina")); // Max stamina
                AddEffect(new FunctionEffect(Category.Stats, "Never Get Hungry", "IAmNeverHungry", "never_get_hungry")); // Never get hungry
                AddEffect(new FunctionEffect(Category.Stats, "No Driving Skills", "BackToDrivingSchool", "no_driving_skills")); // No driving skills
                AddEffect(new FunctionEffect(Category.Stats, "No Lung Capacity", "EmptyLungs", "no_lung_capacity")); // No lung capacity
                AddEffect(new FunctionEffect(Category.Stats, "No Stamina", "ImAllOutOfBreath", "no_stamina")); // No stamina
                AddEffect(new FunctionEffect(Category.Stats, "Skinny Player", "LeanAndMean", "skinny_player")); // Skinny player
                // ------------- //

                // --- Wanted --- //
                AddEffect(new FunctionEffect(Category.WantedLevel, "+2 Wanted Stars", "TurnUpTheHeat", "wanted_level_plus_two")); // Wanted level +2 stars
                AddEffect(new FunctionEffect(Category.WantedLevel, "Clear Wanted Level", "TurnDownTheHeat", "clear_wanted_level")); // Clear wanted level
                AddEffect(new FunctionEffect(Category.WantedLevel, "Never Wanted", "IDoAsIPlease", "never_wanted")); // Never wanted
                AddEffect(new FunctionEffect(Category.WantedLevel, "Six Wanted Stars", "BringItOn", "wanted_level_six_stars").DisableRapidFire()); // Six wanted stars
                // -------------- //

                // --- Weather --- //
                AddEffect(new WeatherEffect("Foggy Weather", "CantSeeWhereImGoing", 9)); // Foggy weather
                AddEffect(new WeatherEffect("Overcast Weather", "DullDullDay", 4)); // Overcast weather
                AddEffect(new WeatherEffect("Rainy Weather", "StayInAndWatchTV", 16)); // Rainy weather
                AddEffect(new WeatherEffect("Sandstorm", "SandInMyEars", 19)); // Sandstorm
                AddEffect(new WeatherEffect("Sunny Weather", "PleasantlyWarm", 1)); // Sunny weather
                AddEffect(new WeatherEffect("Very Sunny Weather", "TooDamnHot", 0)); // Very sunny weather
                // --------------- //

                // --- Spawning --- //
                AddEffect(new FunctionEffect(Category.Spawning, "Get Parachute", "LetsGoBaseJumping", "get_parachute")); // Get Parachute
                AddEffect(new FunctionEffect(Category.Spawning, "Get Jetpack", "Rocketman", "get_jetpack")); // Get Jetpack

                AddEffect(new SpawnVehicleEffect("TimeToKickAss", 432)); // Spawn Rhino
                AddEffect(new SpawnVehicleEffect("OldSpeedDemon", 504)); // Spawn Bloodring Banger
                AddEffect(new SpawnVehicleEffect("DoughnutHandicap", 489)); // Spawn Rancher
                AddEffect(new SpawnVehicleEffect("NotForPublicRoads", 502)); // Spawn Hotring A
                AddEffect(new SpawnVehicleEffect("JustTryAndStopMe", 503)); // Spawn Hotring B
                AddEffect(new SpawnVehicleEffect("WheresTheFuneral", 442)); // Spawn Romero
                AddEffect(new SpawnVehicleEffect("CelebrityStatus", 409)); // Spawn Stretch
                AddEffect(new SpawnVehicleEffect("TrueGrime", 408)); // Spawn Trashmaster
                AddEffect(new SpawnVehicleEffect("18Holes", 457)); // Spawn Caddy
                AddEffect(new SpawnVehicleEffect("JumpJet", 520)); // Spawn Hydra
                AddEffect(new SpawnVehicleEffect("IWantToHover", 539)); // Spawn Vortex
                AddEffect(new SpawnVehicleEffect("OhDude", 425)); // Spawn Hunter
                AddEffect(new SpawnVehicleEffect("FourWheelFun", 471)); // Spawn Quad
                AddEffect(new SpawnVehicleEffect("ItsAllBull", 486)); // Spawn Dozer
                AddEffect(new SpawnVehicleEffect("FlyingToStunt", 513)); // Spawn Stunt Plane
                AddEffect(new SpawnVehicleEffect("MonsterMash", 556)); // Spawn Monster
                AddEffect(new SpawnVehicleEffect("SurpriseDriver", -1)); // Spawn Random Vehicle
                // ---------------- //

                // --- Time --- //
                AddEffect(new FunctionEffect(Category.Time, "0.25x Game Speed", "MatrixMode", "quarter_game_speed", -1, 1.0f)); // Quarter Gamespeed
                AddEffect(new FunctionEffect(Category.Time, "0.5x Game Speed", "SlowItDown", "half_game_speed", -1, 2.0f)); // Half Gamespeed
                AddEffect(new FunctionEffect(Category.Time, "2x Game Speed", "SpeedItUp", "double_game_speed", -1, 2.0f)); // Double Gamespeed
                AddEffect(new FunctionEffect(Category.Time, "4x Game Speed", "YoureTooSlow", "quadruple_game_speed", -1, 1.0f)); // Quadruple Gamespeed
                AddEffect(new FunctionEffect(Category.Time, "Always Midnight", "NightProwler", "always_midnight")); // Always midnight
                AddEffect(new FunctionEffect(Category.Time, "Faster Clock", "TimeJustFliesBy", "faster_clock")); // Faster clock
                AddEffect(new FunctionEffect(Category.Time, "Stop Game Clock", "DontBringOnTheNight", "stop_game_clock")); // Stop game clock, orange sky
                AddEffect(new FunctionEffect(Category.Time, "Timelapse Mode", "DiscoInTheSky", "timelapse")); // Timelapse mode
                // ------------ //

                // --- Vehicles & Traffic --- //
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "All Cars Have Nitro", "SpeedFreak", "all_cars_have_nitro")); // All cars have nitro
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "All Green Lights", "DontTryAndStopMe", "all_green_lights")); // All green lights
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "All Taxis Have Nitrous", "SpeedyTaxis", "all_taxis_have_nitro")); // All taxis have nitrous
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Black Cars", "SoLongAsItsBlack", "black_traffic")); // Black traffic
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Blow Up All Cars", "AllCarsGoBoom", "blow_up_all_cars").DisableRapidFire()); // Blow up all cars
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Boats Fly", "FlyingFish", "boats_fly")); // Boats fly
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Cars Float Away When Hit", "BubbleCars", "cars_float_away_when_hit")); // Cars float away when hit
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Cars Fly", "ChittyChittyBangBang", "cars_fly")); // Cars fly
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Cars On Water", "JesusTakeTheWheel", "cars_on_water")); // Cars on water
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Cheap Cars", "EveryoneIsPoor", "cheap_cars")); // Cheap cars
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Expensive Cars", "EveryoneIsRich", "expensive_cars")); // Expensive cars
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Ghost Cars (Only Wheels)", "WheelsOnlyPlease", "wheels_only_please")); // Invisible Vehicles (Only Wheels)
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Insane Handling", "StickLikeGlue", "insane_handling")); // Insane handling
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Pink Cars", "PinkIsTheNewCool", "pink_traffic")); // Pink traffic
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Smash N' Boom", "TouchMyCarYouDie", "smash_n_boom")); // Smash n' boom
                // -------------------------- //

                // --- Player Modifications --- //
                AddEffect(new FunctionEffect(Category.PlayerModifications, "Drive-By Aiming", "IWannaDriveBy", "weapon_aiming_while_driving")); // Weapon aiming while driving
                AddEffect(new FunctionEffect(Category.PlayerModifications, "Infinite Oxygen", "ManFromAtlantis", "infinite_oxygen")); // Infinite oxygen
                AddEffect(new FunctionEffect(Category.PlayerModifications, "Huge Bunny Hop", "CJPhoneHome", "huge_bunny_hop")); // Huge bunny hop
                AddEffect(new FunctionEffect(Category.PlayerModifications, "Mega Jump", "Kangaroo", "mega_jump")); // Mega jump
                AddEffect(new FunctionEffect(Category.PlayerModifications, "Mega Punch", "StingLikeABee", "mega_punch")); // Mega punch
                // ---------------------------- //
                // ----------------------------------------------- //

                //---------------- Custom Effects ---------------- //
                // --- Generic --- //
                AddEffect(new FunctionEffect(Category.CustomEffects, "0.5x Effect Speed", "LetsDragThisOutABit", "half_timer_speed", -1, 2.5f));
                AddEffect(new FunctionEffect(Category.CustomEffects, "2x Effect Speed", "LetsDoThisABitFaster", "double_timer_speed", -1, 10.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects, "5x Effect Speed", "LetsDoThisSuperFast", "quintuple_timer_speed", -1, 25.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Clear Active Effects", "ClearActiveEffects", "clear_active_effects"), 3.0); // Clear Active Effects
                AddEffect(new FunctionEffect(Category.CustomEffects, "Hide Chaos UI", "AsIfNothingEverHappened", "hide_chaos_ui"));
                AddEffect(new HyperRapidFireEffect("Hyper Rapid-Fire", "SystemCrash", "hyper_rapid_fire"));
                AddEffect(new RapidFireEffect("Rapid-Fire", "SystemOverload", "rapid_fire"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Reset Effect Timers", "HistoryRepeatsItself", "reset_effect_timers"));

                AddEffect(new FunctionEffect(Category.CustomEffects, "Delayed Controls", "WhatsWrongWithThisKeyboard", "delayed_controls", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Game Crash", "TooManyModsInstalled", "fake_crash").SetDisplayName(DisplayNameType.UI, "Fake Crash").DisableRapidFire());
                //AddEffect(new FunctionEffect(Category.CustomEffects, "Greyscale Screen", "GreyscaleScreen", "greyscale_screen", -1, 1.0f)); // Greyscale Screen
                AddEffect(new FunctionEffect(Category.CustomEffects, "Mirrored Screen", "WhatsLeftIsRight", "mirrored_screen", -1, 1.0f)); // Mirrored Screen
                AddEffect(new FunctionEffect(Category.CustomEffects, "Mirrored World", "LetsTalkAboutParallelUniverses", "mirrored_world")); // Mirrored World
                AddEffect(new FunctionEffect(Category.CustomEffects, "Night Vision", "NightVision", "night_vision", -1, 1.0f)); // Night Vision
                AddEffect(new FunctionEffect(Category.CustomEffects, "Nothing", "ThereIsNoEffect", "nothing"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "No Visible Water", "OceanManGoneAgain", "no_visible_water"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "No Water Physics", "FastTrackToAtlantis", "no_water_physics"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Pausing", "LetsPause", "pausing"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Quick Sprunk Stop", "ARefreshingDrink", "quick_sprunk_stop", 1000 * 5));
                //AddEffect(new FunctionEffect(Category.CustomEffects, "Rainbow Weapons", "ColorfulFirepower", "rainbow_weapons"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Reload Autosave", "HereWeGoAgain", "reload_autosave").DisableRapidFire()); // Reload Autosave
                AddEffect(new FunctionEffect(Category.CustomEffects, "Roll Credits", "WaitItsOver", "roll_credits")); // Roll Credits - Rolls the credits but only visually!
                AddEffect(new FunctionEffect(Category.CustomEffects, "Screen Flip", "MuscleMemoryMangler", "screen_flip", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Spawn Ramp", "FreeStuntJump", "spawn_ramp", 1000 * 5));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Thermal Vision", "ThermalVision", "thermal_vision", -1, 1.0f)); // Thermal Vision
                AddEffect(new FunctionEffect(Category.CustomEffects, "Underwater View", "HelloHowAreYouIAmUnderTheWater", "underwater_view"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Upside-Down Screen", "WhatsUpIsDown", "upside_down_screen", -1, 1.0f)); // Upside-Down Screen
                // --------------- //

                // --- Audio --- //
                AddEffect(new FunctionEffect(Category.CustomEffects, "High Pitched Audio", "CJAndTheChipmunks", "high_pitched_audio")); // High Pitched Audio
                AddEffect(new FunctionEffect(Category.CustomEffects, "Pitch Shifter", "VocalRange", "pitch_shifter")); // Pitch Shifter
                // ------------- //

                // --- Gravity --- //
                AddEffect(new FunctionEffect(Category.CustomEffects, "Double Gravity", "KilogramOfFeathers", "double_gravity", -1, 2.0f)); // Sets the gravity to 0.016f
                AddEffect(new FunctionEffect(Category.CustomEffects, "Half Gravity", "ImFeelingLightheaded", "half_gravity", -1, 2.0f)); // Sets the gravity to 0.004f
                AddEffect(new FunctionEffect(Category.CustomEffects, "Insane Gravity", "StraightToHell", "insane_gravity", 1000 * 10).DisableRapidFire()); // Sets the gravity to 0.64f
                AddEffect(new FunctionEffect(Category.CustomEffects, "Inverted Gravity", "BeamMeUpScotty", "inverted_gravity", 1000 * 10).DisableRapidFire()); // Sets the gravity to -0.002f
                AddEffect(new FunctionEffect(Category.CustomEffects, "Quadruple Gravity", "KilogramOfSteel", "quadruple_gravity", -1, 1.0f)); // Sets the gravity to 0.032f
                AddEffect(new FunctionEffect(Category.CustomEffects, "Quarter Gravity", "GroundControlToMajorTom", "quarter_gravity", -1, 1.0f)); // Sets the gravity to 0.002f
                AddEffect(new FunctionEffect(Category.CustomEffects, "Zero Gravity", "ImInSpaaaaace", "zero_gravity", 1000 * 10).DisableRapidFire()); // Sets the gravity to 0f
                // --------------- //

                // --- HUD --- //
                AddEffect(new FunctionEffect(Category.CustomEffects, "Blind", "WhoTurnedTheLightsOff", "blind", 1000 * 10));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Disable HUD", "FullyImmersed", "disable_hud")); // Disable HUD
                AddEffect(new FunctionEffect(Category.CustomEffects, "DVD Screensaver", "ItsGonnaHitTheCorner", "dvd_screensaver", -1, 1.0f).DisableRapidFire()); // DVD Screensaver
                AddEffect(new FunctionEffect(Category.CustomEffects, "Freeze Radar", "OutdatedMaps", "freeze_radar"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "No Blips/Markers/Pickups", "INeedSomeInstructions", "disable_blips_markers_pickups")); // Disable Blips / Markers / Pickups
                AddEffect(new FunctionEffect(Category.CustomEffects, "Portrait Mode", "PortraitMode", "portrait_mode"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Tunnel Vision", "TunnelVision", "tunnel_vision", -1, 1.0f).DisableRapidFire()); // Tunnel Vision
                // ----------- //

                // --- Mission --- //
                AddEffect(new FunctionEffect(Category.CustomEffects, "Fail Current Mission", "MissionFailed", "fail_current_mission").DisableRapidFire()); // Fail Current Mission
                AddEffect(new FunctionEffect(Category.CustomEffects, "Pass Current Mission", "IllTakeAFreePass", "pass_current_mission")); // Pass Current Mission
                AddEffect(new FunctionEffect(Category.CustomEffects, "Fake Pass Current Mission", "IWontTakeAFreePass", "fake_pass_current_mission").SetDisplayName(DisplayNameType.GAME, "Pass Current Mission").DisableRapidFire()); // Fake Pass Current Mission
                // --------------- //

                // --- Ped --- //
                AddEffect(new FunctionEffect(Category.CustomEffects, "Action Figures", "ILikePlayingWithToys", "action_figures"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "ASSERT DOMINANCE", "AssertDominance", "t_pose_peds"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Backwards Peds", "BackwardsPeds", "backwards_peds"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Big Heads", "BigHeadsMode", "big_heads"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Disable All Weapon Damage", "TruePacifist", "disable_all_weapon_damage")); // Disable all Weapon Damage
                AddEffect(new FunctionEffect(Category.CustomEffects, "Don't Lose Your Head", "DontLoseYourHead", "dont_lose_your_head", -1, 0.5f));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Everybody Bleed Now!", "EverybodyBleedNow", "everybody_bleed_now", -1, 1.5f)); // Everybody bleed now!
                AddEffect(new FunctionEffect(Category.CustomEffects, "Everyone Leaves All Cars", "ImTiredOfDriving", "everyone_leaves_all_vehicles"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Hold The F Up...", "HoldTheFUp", "hold_the_f_up").SetDisplayName(DisplayNameType.STREAM, "Hold The F*** Up..."));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Inverted Weapon Damage", "HealingBullets", "inverted_weapon_damage"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Invincible (Everyone)", "NoOneCanHurtAnyone", "infinite_health_everyone")); // Infinite Health (Everyone)
                AddEffect(new FunctionEffect(Category.CustomEffects, "Large Peds", "LargPeepoPeds", "ped_size_large"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Long Live The Rich!", "LongLiveTheRich", "long_live_the_rich"));  // Money = Health, shoot people to gain money, take damage to lose it
                AddEffect(new FunctionEffect(Category.CustomEffects, "Long Necks", "ICanSeeMyHouseFromUpHere", "long_necks"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "One Bullet Magazines", "OneInTheChamber", "one_bullet_magazines"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Ped One Hit K.O.", "ILikeToLiveDangerously", "one_hit_ko").DisableRapidFire()); // One Hit K.O.
                AddEffect(new FunctionEffect(Category.CustomEffects, "Peds Explode If Run Over", "ExplosivePeds", "peds_explode_when_run_over")); // Peds Explode If Run Over
                AddEffect(new FunctionEffect(Category.CustomEffects, "Rainbow Peds", "TastyUnicornPoop", "rainbow_peds"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Remove All Weapons", "NoWeaponsAllowed", "remove_all_weapons")); // Remove all weapons
                AddEffect(new FunctionEffect(Category.CustomEffects, "Set Everyone On Fire", "HotPotato", "set_everyone_on_fire").DisableRapidFire()); // Set everyone on fire
                AddEffect(new FunctionEffect(Category.CustomEffects, "Speeeeeen!", "Speeeeeen", "rotating_peds"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Tiny Peds", "SmolPeepoPeds", "ped_size_tiny"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Upside-Down Peds", "InALandDownUnder", "upside_down_peds"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Where Is Everybody?", "ImHearingVoices", "where_is_everybody")); // Where is everybody?
                // ----------- //

                // --- NPCs --- //
                AddEffect(new FunctionEffect(Category.CustomEffects, "Cops Everywhere", "TooMuchLawAndOrder", "cops_everywhere"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Disarm All NPCs", "LeaveTheGunsToMe", "disarm_all_npcs"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Explode All NPCs", "BoomGoesTheDynamite", "explode_all_npcs").DisableRapidFire());
                AddEffect(new FunctionEffect(Category.CustomEffects, "Give NPCs An RPG", "RocketParty", "give_npcs_an_rpg"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Launch All NPCs", "UpUpAndAway", "launch_all_npcs"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Teleport All NPCs To Player", "WhoAreYouPeople", "teleport_all_npcs_to_player").SetDisplayName(DisplayNameType.STREAM, "TP All NPCs To Player"));
                // ------------ //

                // --- Traffic --- //
                AddEffect(new FunctionEffect(Category.CustomEffects, "Traffic Is Bobcat", "BobcatAllAround", "vehicle_spawns_bobcat"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Traffic Is Caddy", "CaddyAllAround", "vehicle_spawns_caddy"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Traffic Is Combine", "CombineAllAround", "vehicle_spawns_combine"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Traffic Is Infernus", "InfernusAllAround", "vehicle_spawns_infernus"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Traffic Is Kart", "KartAllAround", "vehicle_spawns_kart"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Traffic Is Monster", "MonsterAllAround", "vehicle_spawns_monster"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Traffic Is Mr. Whoopee", "MrWhoopeeAllAround", "vehicle_spawns_whoopee"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Traffic Is Mower", "MowerAllAround", "vehicle_spawns_mower"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Traffic Is Rhino", "RhinoAllAround", "vehicle_spawns_rhino"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Traffic Is Vortex", "VortexAllAround", "vehicle_spawns_vortex"));
                // --------------- //

                // --- Player --- //
                AddEffect(new FunctionEffect(Category.CustomEffects, "-Health, -Armor, -$250k", "INeedSomeHindrance", "anti_health_armor_money"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Arcade Racer Camera", "SegaRallyChampionship", "arcade_racer_camera", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Bankrupt", "CrashTookAllMyMoney", "bankrupt"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Cinematic Vehicle Camera", "MachinimaMode", "cinematic_vehicle_camera", -1, 1.0f)); // Cinematic Vehicle Camera
                AddEffect(new FunctionEffect(Category.CustomEffects, "Disable Aiming", "IForgotHowToAim", "disable_aiming"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Disable Jumping", "ImNotAKangaroo", "disable_jumping"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Disable One Movement Key", "DisableOneMovementKey", "disable_one_movement_key"));  // Disable one movement key
                AddEffect(new FunctionEffect(Category.CustomEffects, "Disable Shooting", "IForgotHowToShoot", "disable_shooting"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Drive Wander", "Autopilot", "drive_wander", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Drunk Player", "DrunkPlayer", "drunk_player")); // Drunk Player
                AddEffect(new FunctionEffect(Category.CustomEffects, "Experience The Lag", "PacketLoss", "experience_the_lag", -1, 1.0f)); // Experience the lag
                AddEffect(new FunctionEffect(Category.CustomEffects, "Explosive Bullets", "BombasticImpact", "explosive_bullets"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Eye For An Eye", "EyeForAnEye", "pacifist"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Fire Bullets", "OilOnTheStreets", "fire_bullets"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Flower Power", "FlowerPower", "flower_power"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Force Mouse Steering", "ForceVehicleMouseSteering", "force_vehicle_mouse_steering")); // Force Mouse Steering
                AddEffect(new FunctionEffect(Category.CustomEffects, "Forced Aiming", "ICanOnlyAim", "forced_aiming", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Forced Look Behind", "EyesInTheBack", "forced_look_behind", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Forced Shooting", "ICanOnlyShoot", "forced_shooting", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Freefall!", "WhereWeDroppingBoys", "freefall").DisableRapidFire()); // Freefall! - Gives CJ a parachute and teleports him very high
                AddEffect(new FunctionEffect(Category.CustomEffects, "Galaxy Note 7", "DangerousPhoneCalls", "galaxy_note_7"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Get Busted", "GoToJail", "get_busted").DisableRapidFire()); // Get's you busted on the spot
                AddEffect(new FunctionEffect(Category.CustomEffects, "Get Wasted", "Hospitality", "get_wasted").DisableRapidFire()); // Get's you wasted on the spot
                AddEffect(new FunctionEffect(Category.CustomEffects, "Instantly Hungry", "IllHave2Number9s", "instantly_hungry")); // Instantly Hungry - Makes CJ instantly hungry
                AddEffect(new FunctionEffect(Category.CustomEffects, "Inverted Controls", "InvertedControls", "inverted_controls"));  // Inverts some controls
                AddEffect(new FunctionEffect(Category.CustomEffects, "It's Rewind Time!", "ItsRewindTime", "its_rewind_time", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Kick Out Of Vehicle", "ThisAintYourCar", "kick_player_out_of_vehicle")); // Kick player out of vehicle
                AddEffect(new FunctionEffect(Category.CustomEffects, "Let's Take A Break", "LetsTakeABreak", "lets_take_a_break", 1000 * 10).DisableRapidFire()); // Let's take a break
                AddEffect(new FunctionEffect(Category.CustomEffects, "Lock Mouse", "WhoUnpluggedMyMouse", "lock_mouse", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Lock Player In Vehicle", "ThereIsNoEscape", "lock_player_inside_vehicle")); // Lock player inside vehicle
                AddEffect(new FunctionEffect(Category.CustomEffects, "Millionaire", "IJustWonTheLottery", "millionaire"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "No Need To Hurry", "NoNeedToHurry", "no_need_to_hurry", -1, 1.5f)); // No Need To Hurry
                AddEffect(new FunctionEffect(Category.CustomEffects, "No Shooting Allowed", "GunsAreDangerous", "no_shooting_allowed"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "No Tasks Allowed", "NoTasksAllowed", "no_tasks_allowed", 1000 * 10));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Pedal To The Metal", "PedalToTheMetal", "pedal_to_the_metal")); // Pedal To The Metal
                AddEffect(new FunctionEffect(Category.CustomEffects, "Portal Guns", "CaveJohnsonWouldBeProud", "portal_guns"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Quake FOV", "QuakeFOV", "quake_fov")); // Quake FOV
                AddEffect(new FunctionEffect(Category.CustomEffects, "Random Outfit", "ASetOfNewClothes", "random_outfit"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Reset Camera", "NaturalView", "reset_camera"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Ring Ring !!", "RingRing", "ring_ring", 1000 * 30)); // Ring Ring !!
                AddEffect(new FunctionEffect(Category.CustomEffects, "Shaky Hands", "IJustCantHoldStill", "shaky_hands"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Steer Bias (Left)", "LeftSideBias", "steer_bias_left"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Steer Bias (Right)", "RightSideBias", "steer_bias_right"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Suicide (1% Chance)", "TheChanceOfSuddenDeath", "one_percent_suicide"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Switch To Unarmed", "PleaseUseYourFists", "switch_to_unarmed"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "The Firing Circus", "TheFiringCircus", "the_firing_circus", 1000 * 5));
                AddEffect(new FunctionEffect(Category.CustomEffects, "The Flash", "FastestManAlive", "the_flash")); // The Flash - Let's you run and swim at incredibly high speeds while not taking fall damage or drowing!
                AddEffect(new FunctionEffect(Category.CustomEffects, "Vehicle Bumper Camera", "FrontRowSeat", "vehicle_bumper_camera", -1, 1.0f)); // Vehicle Bumper Camera - Forces the vehicle's bumper camera
                AddEffect(new FunctionEffect(Category.CustomEffects, "Void Warp", "UnderTheMap", "void_warp"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Walk Off", "LetsGoForAWalk", "walk_off", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Walk On Water", "JesusInTheHouse", "walk_on_water"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Warp In Random Vehicle", "ItsYourUber", "warp_player_into_random_vehicle"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Weapon Recoil", "ThoseAreSomeStrongWeapons", "weapon_recoil"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Weapon Roulette", "WeaponRoulette", "weapon_roulette")); // Weapon Roulette
                AddEffect(new FunctionEffect(Category.CustomEffects, "Zooming FOV", "ZoomingFOV", "zooming_fov", -1, 1.0f));
                // -------------- //

                // --- Time --- //
                AddEffect(new FunctionEffect(Category.CustomEffects, "Powerpoint Presentation", "PowerpointPresentation", "fps_15", -1, 1.0f)); // Powerpoint Presentation (15 FPS)
                AddEffect(new FunctionEffect(Category.CustomEffects, "Smooth Criminal", "SmoothCriminal", "fps_60")); // Smooth Criminal (60 FPS)
                // ------------ //

                // --- Vehicle --- //
                AddEffect(new FunctionEffect(Category.CustomEffects, "All Vehicles Alarmy", "SoundTheAlarm", "all_vehicles_alarmy"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Backwards Cars", "BackPeepoHappy", "vehicle_size_backwards"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Beyblade", "LetItRip", "beyblade")); // Beyblade
                AddEffect(new FunctionEffect(Category.CustomEffects, "Carmageddon", "Carmageddon", "carmageddon")); // Carmageddon - Makes vehicles rain from the sky!
                AddEffect(new FunctionEffect(Category.CustomEffects, "Delete All Vehicles", "GoodbyeAllSweetRides", "delete_all_vehicles").DisableRapidFire());
                AddEffect(new FunctionEffect(Category.CustomEffects, "Delete Vehicle", "GoodbyeSweetRide", "delete_vehicle").DisableRapidFire());
                AddEffect(new FunctionEffect(Category.CustomEffects, "Flipped Cars", "FlippedPeepoHappy", "vehicle_size_flipped"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Freeze Vehicle", "StuckInTime", "freeze_vehicle", 1000 * 5));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Ghost Cars", "InvisibleVehicles", "invisible_vehicles")); // Invisible Vehicles
                AddEffect(new FunctionEffect(Category.CustomEffects, "Ghost Rider", "GhostRider", "ghost_rider")); // Set current vehicle constantly on fire
                AddEffect(new FunctionEffect(Category.CustomEffects, "High Suspension Damping", "VeryDampNoBounce", "high_suspension_damping")); // Cars have high suspension damping
                AddEffect(new FunctionEffect(Category.CustomEffects, "HONK!!!", "HONKHONK", "honk_vehicle", 1000 * 30).SetAudioVariations(5)); // Honk Vehicle
                AddEffect(new FunctionEffect(Category.CustomEffects, "Honk Boost", "GottaHonkFast", "honk_boost")); // Honk Boost
                AddEffect(new FunctionEffect(Category.CustomEffects, "Ignite Current Vehicle", "WayTooHot", "set_current_vehicle_on_fire").DisableRapidFire()); // Set current vehicle on fire
                AddEffect(new FunctionEffect(Category.CustomEffects, "Invert Vehicle Speed", "LetsGoBack", "invert_vehicle_speed"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Large Cars", "LargePeepoHappy", "vehicle_size_large"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Long Cars", "LongPeepoHappy", "vehicle_size_long"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Oh Hey, Tanks!", "OhHeyTanks", "oh_hey_tanks")); // Spawns tanks around the player
                AddEffect(new FunctionEffect(Category.CustomEffects, "Paper Thin Cars", "PaperPeepoHappy", "vehicle_size_paper_thin"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Pop Tires Of All Vehicles", "TiresBeGone", "pop_tires_of_all_vehicles")); // Pop tires of all vehicles
                AddEffect(new FunctionEffect(Category.CustomEffects, "Pride Cars", "AllColorsAreBeautiful", "pride_traffic")); // Pride Traffic / Rainbow Cars
                AddEffect(new FunctionEffect(Category.CustomEffects, "Relative Car Gravity", "SpiderCars", "vehicle_driving_on_walls"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Rotating Cars", "RotatePeepoHappy", "vehicle_size_continuous_rotation"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Send Vehicles To Space", "StairwayToHeaven", "send_vehicles_to_space")); // Gives an immense upwards boost to all vehicles
                AddEffect(new FunctionEffect(Category.CustomEffects, "Speed (1994)", "KeepYourPace", "minimum_speed"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Speed-based Rotating Cars", "SpeedRotatePeepoHappy", "vehicle_rotation_based_on_speed"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Super Wide Cars", "WiderPeepoHappy", "vehicle_size_super_wide"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Swap Vehicles On Impact", "SwapVehiclesOnImpact", "swap_vehicles_on_impact"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Tall Cars", "TallPeepoHappy", "vehicle_size_tall"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Tiny Cars", "TinyPeepoHappy", "vehicle_size_tiny"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "To Drive Or Not To Drive", "ToDriveOrNotToDrive", "to_drive_or_not_to_drive")); // To drive or not to drive
                AddEffect(new FunctionEffect(Category.CustomEffects, "To The Left, To The Right", "ToTheLeftToTheRight", "to_the_left_to_the_right")); // Gives cars a random velocity
                AddEffect(new FunctionEffect(Category.CustomEffects, "Turn Vehicles Around", "TurnAround", "turn_vehicles_around")); // Turn vehicles around
                AddEffect(new FunctionEffect(Category.CustomEffects, "Unflippable Vehicles", "ThereGoesMyBurrito", "unflippable_vehicles")); // Vehicles are unflippable
                AddEffect(new FunctionEffect(Category.CustomEffects, "Vehicle Boost", "FullForceForward", "vehicle_boost"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Vehicle One Hit K.O.", "NoDings", "vehicle_one_hit_ko"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Wide Cars", "WidePeepoHappy", "vehicle_size_wide"));
                AddEffect(new FunctionEffect(Category.CustomEffects, "Your Car Floats When Hit", "ImTheBubbleCar", "your_car_floats_away_when_hit")); // Your Car Floats Away When Hit
                AddEffect(new FunctionEffect(Category.CustomEffects, "Zero Suspension Damping", "LowrideAllNight", "zero_suspension_damping"));  // Cars have almost zero suspension damping
                // --------------- //

                // --- Wanted --- //
                AddEffect(new FunctionEffect(Category.CustomEffects, "Always Wanted", "ICanSeeStars", "always_wanted").DisableRapidFire()); // Always Wanted
                // -------------- //

                // --- Teleport --- //
                AddEffect(new FakeTeleportEffect("Fake Teleport", "HahaGotYourNose")); // Fake Teleport
                AddEffect(new FunctionEffect(Category.Teleportation, "Random Teleport", "LetsGoSightseeing", "random_teleport").DisableRapidFire()); // Random Teleport - Teleports CJ to a random location on the map
                AddEffect(new FunctionEffect(Category.Teleportation, "Teleport To Marker", "IKnowJustTheRightPlace", "teleport_to_marker", 1000 * 30)); // Teleport To Marker
                AddEffect(new FunctionEffect(Category.Teleportation, "Teleport To Liberty City", "LetsTalkAboutTheMultiverse", "teleport_to_liberty_city").DisableRapidFire()); // Teleport To Liberty City

                foreach (Location location in Location.Locations)
                {
                    AddEffect(new TeleportationEffect(location));
                }
                // ---------------- //
            }
            else if (game == "vice_city")
            {
                // Removed for now
            }

            Effects.Sort((first, second) => string.Compare(first.item.GetDisplayName(DisplayNameType.UI), second.item.GetDisplayName(DisplayNameType.UI), StringComparison.CurrentCultureIgnoreCase));

            foreach (WeightedRandomBag<AbstractEffect>.Entry e in Effects.Get())
            {
                string displayName = e.item.GetDisplayName(DisplayNameType.STREAM);
                if (displayName.Length > 25)
                {
                    Debug.WriteLine($"WARNING: The following effect is {displayName.Length - 25} characters too long for Twitch: '{displayName}'");
                    Debug.WriteLine($"{displayName.Substring(0, 25)}");
                }
            }
        }

        public static WeightedRandomBag<AbstractEffect> EnabledEffects { get; } = new();

        public static AbstractEffect GetByID(string id, bool onlyEnabled = false) => (onlyEnabled ? EnabledEffects : Effects).Find(e => e.item.GetID().Equals(id)).item;

        public static AbstractEffect GetByWord(string word, bool onlyEnabled = false) => (onlyEnabled ? EnabledEffects : Effects).Find(e => !string.IsNullOrEmpty(e.item.Word) && string.Equals(e.item.Word, word, StringComparison.OrdinalIgnoreCase)).item;

        public static AbstractEffect GetByDescription(string description, bool onlyEnabled = false) => (onlyEnabled ? EnabledEffects : Effects).Find(e => string.Equals(description, e.item.GetDisplayName(DisplayNameType.UI), StringComparison.OrdinalIgnoreCase)).item;

        public static AbstractEffect GetRandomEffect(bool onlyEnabled = false, int attempts = 0)
        {
            WeightedRandomBag<AbstractEffect> effects = onlyEnabled ? EnabledEffects : Effects;

            if (effects.Count > 0)
            {
                attempts++;

                AbstractEffect effect = effects.GetRandom(RandomHandler.Random);
                if (!onlyEnabled || attempts > 10)
                {
                    return effect;
                }

                if (effect is null)
                {
                    return GetRandomEffect(onlyEnabled, attempts);
                }

                return EffectCooldowns.ContainsKey(effect) ? GetRandomEffect(onlyEnabled, attempts) : effect;
            }

            return null;
        }

        public static void CooldownEffects()
        {
            foreach (KeyValuePair<AbstractEffect, int> item in EffectCooldowns.ToList())
            {
                EffectCooldowns[item.Key]--;
            }

            IEnumerable<KeyValuePair<AbstractEffect, int>> toRemove = EffectCooldowns.Where(e => e.Value <= 0).ToList();
            foreach (KeyValuePair<AbstractEffect, int> item in toRemove)
            {
                EffectCooldowns.Remove(item.Key);
            }
        }

        public static void ResetEffectCooldowns() => EffectCooldowns.Clear();

        public static AbstractEffect RunEffect(string id, bool onlyEnabled = true) => RunEffect(GetByID(id, onlyEnabled));

        public static AbstractEffect RunEffect(AbstractEffect effect, int seed = -1, int duration = -1)
        {
            CooldownEffects();

            if (effect is not null)
            {
                effect.RunEffect(seed, duration);
                EffectCooldowns[effect] = Config.Instance().Experimental_EffectsCooldownNotActivating;
            }

            return effect;
        }

        public static void SetEffectEnabled(AbstractEffect effect, bool enabled)
        {
            if (effect is null)
            {
                return;
            }

            WeightedRandomBag<AbstractEffect>.Entry entry = Effects.Find(e => e.item.Equals(effect));
            if (entry.item is null)
            {
                return;
            }

            if (!enabled && EnabledEffects.Contains(entry))
            {
                EnabledEffects.Remove(entry);
            }
            else if (enabled && !EnabledEffects.Contains(entry))
            {
                EnabledEffects.Add(entry);
            }
        }
    }
}
