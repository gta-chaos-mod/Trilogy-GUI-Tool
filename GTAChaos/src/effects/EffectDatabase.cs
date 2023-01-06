// Copyright (c) 2019 Lordmau5
using GTAChaos.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly Random rand = RandomHandler.Random;
        private double AccumulatedWeight = 0;

        public Entry Add(T item, double weight = 1.0)
        {
            Entry entry = new()
            {
                item = item,
                weight = weight
            };

            this.entries.Add(entry);

            this.CalculateAccumulatedWeight();

            return entry;
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

        public (bool success, T entry) GetRandom(Random rand, Func<Entry, bool> predicate)
        {
            rand ??= this.rand;

            IEnumerable<Entry> list = this.entries.Where(predicate);
            Debug.WriteLine($"{this.Count} : {list.Count()}");

            if (list.Count() <= 0)
            {
                return (false, default); // this.Count > 0 ? this.entries[0].item : default;
            }

            // Calculate accumulated weight based on predicate
            double accumulatedWeight = 0.0f;
            foreach (Entry entry in list)
            {
                accumulatedWeight += entry.weight;
            }

            double randomNumber = rand.NextDouble() * accumulatedWeight;

            foreach (Entry entry in list)
            {
                if (entry.weight >= randomNumber)
                {
                    return (true, entry.item);
                }

                randomNumber -= entry.weight;
            }

            return (false, default);
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

        public Entry Add(Entry entry) => this.Add(entry.item, entry.weight);

        public void Remove(Entry item) => this.entries.Remove(item);

        public bool Contains(Entry item) => this.entries.Contains(item);

        public void Sort(Comparison<Entry> comparison) => this.entries.Sort(comparison);

        public Entry Find(Predicate<Entry> match) => this.entries.Find(match);

        public void Clear() => this.entries.Clear();
    }

    public static class EffectDatabase
    {
        public static WeightedRandomBag<AbstractEffect> Effects { get; } = new();

        private static readonly Dictionary<AbstractEffect, int> EffectCooldowns = new();

        private static void AddEffect(AbstractEffect effect, double weight = 1.0)
        {
            if (effect is null)
            {
                return;
            }

            WeightedRandomBag<AbstractEffect>.Entry entry = Effects.Add(effect, weight);
            EnabledEffects.Add(entry);
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
                AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Death", "GoodbyeCruelWorld", "death").DisableRapidFire()); // Death
                AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Health, Armor, $250k", "INeedSomeHelp", "health_armor_money")); // Health, Armor, $250k
                AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Infinite Ammo", "FullClip", "infinite_ammo")); // Infinite ammo
                AddEffect(new FunctionEffect(Category.WeaponsAndHealth, "Invincible (Player)", "NoOneCanHurtMe", "infinite_health_player")); // Infinite Health (Player)
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
                AddEffect(new FunctionEffect(Category.Spawning, "Get Jetpack", "Rocketman", "get_jetpack")); // Get Jetpack
                AddEffect(new FunctionEffect(Category.Spawning, "Get Parachute", "LetsGoBaseJumping", "get_parachute")); // Get Parachute

                AddEffect(new SpawnVehicleEffect("OldSpeedDemon", 504)); // Spawn Bloodring Banger
                AddEffect(new SpawnVehicleEffect("18Holes", 457)); // Spawn Caddy
                AddEffect(new SpawnVehicleEffect("ItsAllBull", 486)); // Spawn Dozer
                AddEffect(new SpawnVehicleEffect("NotForPublicRoads", 502)); // Spawn Hotring A
                AddEffect(new SpawnVehicleEffect("JustTryAndStopMe", 503)); // Spawn Hotring B
                AddEffect(new SpawnVehicleEffect("OhDude", 425)); // Spawn Hunter
                AddEffect(new SpawnVehicleEffect("JumpJet", 520)); // Spawn Hydra
                AddEffect(new SpawnVehicleEffect("MonsterMash", 556)); // Spawn Monster
                AddEffect(new SpawnVehicleEffect("EnergyFiveHundred", 522)); // Spawn NRG-500
                AddEffect(new SpawnVehicleEffect("FourWheelFun", 471)); // Spawn Quad
                AddEffect(new SpawnVehicleEffect("DoughnutHandicap", 489)); // Spawn Rancher
                AddEffect(new SpawnVehicleEffect("SurpriseDriver", -1)); // Spawn Random Vehicle
                AddEffect(new SpawnVehicleEffect("TimeToKickAss", 432)); // Spawn Rhino
                AddEffect(new SpawnVehicleEffect("WheresTheFuneral", 442)); // Spawn Romero
                AddEffect(new SpawnVehicleEffect("CelebrityStatus", 409)); // Spawn Stretch
                AddEffect(new SpawnVehicleEffect("FlyingToStunt", 513)); // Spawn Stunt Plane
                AddEffect(new SpawnVehicleEffect("TrueGrime", 408)); // Spawn Trashmaster
                AddEffect(new SpawnVehicleEffect("IWantToHover", 539)); // Spawn Vortex
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
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Black Vehicles", "SoLongAsItsBlack", "black_traffic")); // Black traffic
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Boats Fly", "FlyingFish", "boats_fly")); // Boats fly
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Cars Float Away When Hit", "BubbleCars", "cars_float_away_when_hit")); // Cars float away when hit
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Cars Fly", "ChittyChittyBangBang", "cars_fly")); // Cars fly
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Cars On Water", "JesusTakeTheWheel", "cars_on_water")); // Cars on water
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Cheap Cars", "EveryoneIsPoor", "cheap_cars")); // Cheap cars
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Expensive Cars", "EveryoneIsRich", "expensive_cars")); // Expensive cars
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Explode All Vehicles", "AllCarsGoBoom", "explode_all_cars").DisableRapidFire()); // Explode all cars
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Insane Handling", "StickLikeGlue", "insane_handling")); // Insane handling
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Pink Vehicles", "PinkIsTheNewCool", "pink_traffic")); // Pink traffic
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Smash N' Boom", "TouchMyCarYouDie", "smash_n_boom")); // Smash n' boom
                AddEffect(new FunctionEffect(Category.VehiclesTraffic, "Wheels Only, Please", "WheelsOnlyPlease", "wheels_only_please")); // Invisible Vehicles (Only Wheels)
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
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "0.5x Effect Speed", "LetsDragThisOutABit", "half_timer_speed", -1, 2.5f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "2x Effect Speed", "LetsDoThisABitFaster", "double_timer_speed", -1, 10.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "5x Effect Speed", "LetsDoThisReallyFast", "quintuple_timer_speed", -1, 25.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Clear Active Effects", "ClearActiveEffects", "clear_active_effects"), 3.0); // Clear Active Effects
                AddEffect(new DiscountRapidFireEffect("LIDL Rapid-Fire", "SystemError", "discount_rapid_fire"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Hide Chaos UI", "AsIfNothingEverHappened", "hide_chaos_ui", -1, 5.0f).DisableRapidFire());
                AddEffect(new HyperRapidFireEffect("Hyper Rapid-Fire", "SystemCrash", "hyper_rapid_fire"));
                AddEffect(new RapidFireEffect("Rapid-Fire", "SystemOverload", "rapid_fire"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Reset Effect Timers", "HistoryRepeatsItself", "reset_effect_timers"));

                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Backwards Clock", "TimeJustGoesBackwards", "backwards_clock"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Buttsbot", "ButtsbotYes", "buttsbot", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Custom Textures", "CustomTextures", "textures_custom"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Deforestation", "Deforestation", "deforestation"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Delayed Controls", "WhatsWrongWithThisKeyboard", "delayed_controls", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "EASY TO READ", "ItsEasierToRead", "very_big_font_scale"));
                AddEffect(new FakeCrashEffect("Game Crash", "TooManyModsInstalled"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Get To The Marker!", "GetToTheMarker", "get_to_marker", 1000 * 60 * 1));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "haha funni font", "ComicSansMasterRace", "font_comic_sans"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "It's Morbin Time", "ItsMorbinTime", "textures_its_morbin_time"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "KIRYU-CHAN!!!", "KiryuChan", "font_yakuza"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Low LOD Vehicles", "AtariVehicles", "low_lod_vehicles"));
                //AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Greyscale Screen", "GreyscaleScreen", "greyscale_screen", -1, 1.0f)); // Greyscale Screen
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Mirrored Screen", "WhatsLeftIsRight", "mirrored_screen", -1, 1.0f)); // Mirrored Screen
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Mirrored World", "LetsTalkAboutParallelUniverses", "mirrored_world")); // Mirrored World
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Night Vision", "NightVision", "night_vision", -1, 1.0f)); // Night Vision
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Nothing", "ThereIsNoEffect", "nothing"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "No Visible Water", "OceanManGoneAgain", "no_visible_water"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "No Water Physics", "FastTrackToAtlantis", "no_water_physics"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "One Hit K.O. (Player)", "ILikeToLiveDangerously", "one_hit_ko").DisableRapidFire()); // One Hit K.O. (Player)
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Pausing", "LetsPause", "pausing", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Quick Sprunk Stop", "ARefreshingDrink", "quick_sprunk_stop"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Random Inputs", "PossessedKeyboard", "random_inputs"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Radar Zoom (Small)", "SmallRadarZoom", "radar_zoom_small"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Radar Zoom (Large)", "LargeRadarZoom", "radar_zoom_large"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Random Stunt Jump", "RandomStuntJump", "random_stunt_jump").DisableRapidFire());
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Reload Autosave", "HereWeGoAgain", "reload_autosave").DisableRapidFire()); // Reload Autosave
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Roll Credits", "WaitItsOver", "roll_credits")); // Roll Credits - Rolls the credits but only visually!
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Screensaver HUD", "ScreensaverHUD", "screensaver_hud"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Replace All Text", "ReplaceAllText", "replace_all_text", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Spawn Ramp", "FreeStuntJump", "spawn_ramp"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Super Smokio 64", "SuperSmokio64", "font_mario_64"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "TABLE!", "TABLE", "spawn_table"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Team Trees", "TeamTrees", "spawn_tree"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "The Mirror Dimension", "ThisSeemsStrange", "the_mirror_dimension", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Thermal Vision", "ThermalVision", "thermal_vision", -1, 1.0f)); // Thermal Vision
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Too Much Information", "TooMuchInformation", "too_much_information", -1, 1.5f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Underwater View", "HelloHowAreYouIAmUnderTheWater", "underwater_view"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Uninstall CS: Source", "UninstallCSS", "textures_counter_strike_source"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Upside-Down Screen", "WhatsUpIsDown", "upside_down_screen", -1, 1.0f)); // Upside-Down Screen
                AddEffect(new FunctionEffect(Category.CustomEffects_Generic, "Vehicle Bullets", "ImShootingCars", "vehicle_bullets"));
                // --------------- //

                // --- Audio --- //
                AddEffect(new FunctionEffect(Category.CustomEffects_Audio, "High Pitched Audio", "CJAndTheChipmunks", "high_pitched_audio")); // High Pitched Audio
                AddEffect(new FunctionEffect(Category.CustomEffects_Audio, "Pitch Shifter", "VocalRange", "pitch_shifter")); // Pitch Shifter
                // ------------- //

                // --- Gravity --- //
                AddEffect(new FunctionEffect(Category.CustomEffects_Gravity, "Double Gravity", "KilogramOfFeathers", "double_gravity", -1, 2.0f)); // Sets the gravity to 0.016f
                AddEffect(new FunctionEffect(Category.CustomEffects_Gravity, "Half Gravity", "ImFeelingLightheaded", "half_gravity", -1, 2.0f)); // Sets the gravity to 0.004f
                AddEffect(new FunctionEffect(Category.CustomEffects_Gravity, "Insane Gravity", "StraightToHell", "insane_gravity", 1000 * 10).DisableRapidFire()); // Sets the gravity to 0.64f
                AddEffect(new FunctionEffect(Category.CustomEffects_Gravity, "Inverted Gravity", "BeamMeUpScotty", "inverted_gravity", 1000 * 10).DisableRapidFire()); // Sets the gravity to -0.002f
                AddEffect(new FunctionEffect(Category.CustomEffects_Gravity, "Quadruple Gravity", "KilogramOfSteel", "quadruple_gravity", -1, 1.0f)); // Sets the gravity to 0.032f
                AddEffect(new FunctionEffect(Category.CustomEffects_Gravity, "Quarter Gravity", "GroundControlToMajorTom", "quarter_gravity", -1, 1.0f)); // Sets the gravity to 0.002f
                AddEffect(new FunctionEffect(Category.CustomEffects_Gravity, "Zero Gravity", "ImInSpaaaaace", "zero_gravity", 1000 * 10).DisableRapidFire()); // Sets the gravity to 0f
                // --------------- //

                // --- HUD --- //
                AddEffect(new FunctionEffect(Category.CustomEffects_HUD, "Blind", "WhoTurnedTheLightsOff", "blind", 1000 * 10));
                AddEffect(new FunctionEffect(Category.CustomEffects_HUD, "Disable HUD", "FullyImmersed", "disable_hud")); // Disable HUD
                AddEffect(new FunctionEffect(Category.CustomEffects_HUD, "DVD Screensaver", "ItsGonnaHitTheCorner", "dvd_screensaver", -1, 1.0f).DisableRapidFire()); // DVD Screensaver
                AddEffect(new FunctionEffect(Category.CustomEffects_HUD, "Freeze Radar", "OutdatedMaps", "freeze_radar"));
                AddEffect(new FunctionEffect(Category.CustomEffects_HUD, "No Blips/Markers/Pickups", "INeedSomeInstructions", "disable_blips_markers_pickups")); // Disable Blips / Markers / Pickups
                AddEffect(new FunctionEffect(Category.CustomEffects_HUD, "Portrait Mode", "PortraitMode", "portrait_mode"));
                AddEffect(new FunctionEffect(Category.CustomEffects_HUD, "Tunnel Vision", "TunnelVision", "tunnel_vision", -1, 1.0f).DisableRapidFire()); // Tunnel Vision
                // ----------- //

                // --- Mission --- //
                AddEffect(new FunctionEffect(Category.CustomEffects_Mission, "Fail Current Mission", "MissionFailed", "fail_current_mission").DisableRapidFire()); // Fail Current Mission
                AddEffect(new FunctionEffect(Category.CustomEffects_Mission, "Pass Current Mission", "IllTakeAFreePass", "pass_current_mission")); // Pass Current Mission
                AddEffect(new FunctionEffect(Category.CustomEffects_Mission, "Fake Pass Current Mission", "IWontTakeAFreePass", "fake_pass_current_mission").SetDisplayName(DisplayNameType.GAME, "Pass Current Mission").DisableRapidFire()); // Fake Pass Current Mission
                // --------------- //

                // --- Ped --- //
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Action Figures", "ILikePlayingWithToys", "action_figures"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Ant Peds", "AntsAntsAntMan", "ped_size_super_tiny"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "ASSERT DOMINANCE", "AssertDominance", "t_pose_peds"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Backwards Peds", "BackwardsPeds", "ped_rotation_backwards"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Big Heads", "BigHeadsMode", "big_heads"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Disable Headshots", "BulletproofForeheads", "disable_headshots"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Disable All Weapon Damage", "TruePacifist", "disable_all_weapon_damage")); // Disable all Weapon Damage
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Everybody Bleed Now!", "EverybodyBleedNow", "everybody_bleed_now", -1, 1.5f)); // Everybody bleed now!
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Head Peds", "HeadPeds", "head_peds"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Hold The F Up...", "HoldTheFUp", "hold_the_f_up").SetDisplayName(DisplayNameType.STREAM, "Hold The F*** Up..."));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Inverted Weapon Damage", "HealingBullets", "inverted_weapon_damage"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Invincible (Everyone)", "NoOneCanHurtAnyone", "infinite_health_everyone")); // Infinite Health (Everyone)
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Large Peds", "LargePeepoPeds", "ped_size_large"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Long Live The Rich!", "LongLiveTheRich", "long_live_the_rich"));  // Money = Health, shoot people to gain money, take damage to lose it
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Long Necks", "ICanSeeMyHouseFromUpHere", "long_necks"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "One Bullet Magazines", "OneInTheChamber", "one_bullet_magazines"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "One Hit K.O. (Everyone)", "EveryoneLikesToLiveDangerously", "one_hit_ko_everyone").DisableRapidFire()); // One Hit K.O. (Everyone)
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Ped Wallhack", "ICanSeeYouGuysThroughWalls", "ped_wallhack"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Peds Explode If Run Over", "ExplosivePeds", "peds_explode_when_run_over")); // Peds Explode If Run Over
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Peds Leave All Vehicles", "ImTiredOfDriving", "everyone_leaves_all_vehicles"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Rainbow Peds", "TastyUnicornPoop", "rainbow_peds"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Remove Everyone's Weapons", "NoWeaponsAllowed", "remove_everyones_weapons"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Rotating Peds (X)", "PedsRotatingOnX", "ped_rotation_continuous_x"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Rotating Peds (Y)", "PedsRotatingOnY", "ped_rotation_continuous_y"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Rotating Peds (Z)", "PedsRotatingOnZ", "ped_rotation_continuous_z"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Set Everyone On Fire", "HotPotato", "set_everyone_on_fire").DisableRapidFire()); // Set everyone on fire
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Spawn Dumptrucks", "DamnBoiHeThicc", "big_butts"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Speeeeeen!", "Speeeeeen", "rotating_peds"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Tiny Peds", "TinyPeepoPeds", "ped_size_tiny"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Upside-Down Peds", "InALandDownUnder", "ped_rotation_flipped"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "WAYTOODANK", "WAYTOODANK", "dont_lose_your_head", -1, 1.5f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Ped, "Where Is Everybody?", "ImHearingVoices", "where_is_everybody")); // Where is everybody?
                // ----------- //

                // --- Objects --- //
                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Backwards Objects", "ObjectsAreBackwards", "object_rotation_backwards", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Upside-Down Objects", "ObjectsAreUpsideDown", "object_rotation_flipped", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Rotating Objects (X)", "ObjectsRotatingOnX", "object_rotation_continuous_x", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Rotating Objects (Y)", "ObjectsRotatingOnY", "object_rotation_continuous_y", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Rotating Objects (Z)", "ObjectsRotatingOnZ", "object_rotation_continuous_z", 1000 * 30));

                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Tiny Objects", "ObjectsAreTiny", "object_size_tiny", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Large Objects", "ObjectsAreLarge", "object_size_large", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Wide Objects", "ObjectsAreWide", "object_size_wide", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Super Wide Objects", "ObjectsAreSuperWide", "object_size_super_wide", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Tall Objects", "ObjectsAreTall", "object_size_tall", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Long Objects", "ObjectsAreLong", "object_size_long", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Paper Thin Objects", "ObjectsArePaperThin", "object_size_paper_thin", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Objects, "Flat Objects", "ObjectsAreFlat", "object_size_flat", 1000 * 30));
                // --------------- //

                // --- NPCs --- //
                AddEffect(new FunctionEffect(Category.CustomEffects_NPCs, "Cops Everywhere", "TooMuchLawAndOrder", "cops_everywhere"));
                AddEffect(new FunctionEffect(Category.CustomEffects_NPCs, "Disarm All NPCs", "LeaveTheGunsToMe", "disarm_all_npcs"));
                AddEffect(new FunctionEffect(Category.CustomEffects_NPCs, "Explode All NPCs", "BoomGoesTheDynamite", "explode_all_npcs").DisableRapidFire());
                AddEffect(new FunctionEffect(Category.CustomEffects_NPCs, "Give NPCs An RPG", "RocketParty", "give_npcs_an_rpg"));
                AddEffect(new FunctionEffect(Category.CustomEffects_NPCs, "Launch All NPCs", "UpUpAndAway", "launch_all_npcs"));
                AddEffect(new FunctionEffect(Category.CustomEffects_NPCs, "Teleport All NPCs To Player", "WhoAreYouPeople", "teleport_all_npcs_to_player").SetDisplayName(DisplayNameType.STREAM, "TP All NPCs To Player"));
                // ------------ //

                // --- Traffic --- //
                AddEffect(new FunctionEffect(Category.CustomEffects_Traffic, "Traffic Is Bobcat", "BobcatAllAround", "vehicle_spawns_bobcat"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Traffic, "Traffic Is Caddy", "CaddyAllAround", "vehicle_spawns_caddy"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Traffic, "Traffic Is Combine", "CombineAllAround", "vehicle_spawns_combine"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Traffic, "Traffic Is Infernus", "InfernusAllAround", "vehicle_spawns_infernus"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Traffic, "Traffic Is Kart", "KartAllAround", "vehicle_spawns_kart"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Traffic, "Traffic Is Monster", "MonsterAllAround", "vehicle_spawns_monster"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Traffic, "Traffic Is Mr. Whoopee", "MrWhoopeeAllAround", "vehicle_spawns_whoopee"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Traffic, "Traffic Is Mower", "MowerAllAround", "vehicle_spawns_mower"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Traffic, "Traffic Is Rhino", "RhinoAllAround", "vehicle_spawns_rhino"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Traffic, "Traffic Is Vortex", "VortexAllAround", "vehicle_spawns_vortex"));
                // --------------- //

                // --- Player --- //
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "-Health, -Armor, -$250k", "INeedSomeHindrance", "anti_health_armor_money"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Add Random Blips", "PointsOfUninterest", "add_random_blips"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Arcade Racer Camera", "SegaRallyChampionship", "arcade_racer_camera", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Bankrupt", "CrashTookAllMyMoney", "bankrupt"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Carl! It's Zero!", "ZeroNeedsYourHelp", "teleport_to_zero").DisableRapidFire());
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Cinematic Vehicle Camera", "MachinimaMode", "cinematic_vehicle_camera", -1, 1.0f)); // Cinematic Vehicle Camera
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Death (1% Chance)", "TheChanceOfSuddenDeath", "one_percent_death"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Disable Aiming", "IForgotHowToAim", "disable_aiming"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Disable One Movement Key", "DisableOneMovementKey", "disable_one_movement_key", -1, 1.5f));  // Disable one movement key
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Disable Shooting", "IForgotHowToShoot", "disable_shooting"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Disable Swimming", "IForgotHowToSwim", "disable_swimming", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Drive Wander", "Autopilot", "drive_wander", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Drunk Player", "DrunkPlayer", "drunk_player")); // Drunk Player
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Experience The Lag", "PacketLoss", "experience_the_lag", -1, 1.0f)); // Experience the lag
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Explosive Bullets", "BombasticImpact", "explosive_bullets"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Eye For An Eye", "EyeForAnEye", "pacifist"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Fire Bullets", "OilOnTheStreets", "fire_bullets"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Flower Power", "FlowerPower", "flower_power"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Force Field", "ForceField", "force_field"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Force Mouse Steering", "ForceVehicleMouseSteering", "force_vehicle_mouse_steering")); // Force Mouse Steering
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Forced Aiming", "ICanOnlyAim", "forced_aiming", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Forced Look Behind", "EyesInTheBack", "forced_look_behind", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Forced Shooting", "ICanOnlyShoot", "forced_shooting", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Freefall!", "WhereWeDroppingBoys", "freefall").DisableRapidFire()); // Freefall! - Gives CJ a parachute and teleports him very high
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Galaxy Note 7", "DangerousPhoneCalls", "galaxy_note_7"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Get Busted", "GoToJail", "get_busted").DisableRapidFire()); // Get's you busted on the spot
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Get Wasted", "Hospitality", "get_wasted").DisableRapidFire()); // Get's you wasted on the spot
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Grounded", "ImNotAKangaroo", "disable_jumping"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Gun Game", "ModernWarfare2Lobby", "gun_game"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Instantly Hungry", "IllHave2Number9s", "instantly_hungry")); // Instantly Hungry - Makes CJ instantly hungry
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Inverted Controls", "InvertedControls", "inverted_controls"));  // Inverts some controls
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "It's Rewind Time!", "ItsRewindTime", "its_rewind_time", -1, 1.5f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Kick Out Of Vehicle", "ThisAintYourCar", "kick_player_out_of_vehicle")); // Kick player out of vehicle
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Let's Take A Break", "LetsTakeABreak", "lets_take_a_break", 1000 * 10).DisableRapidFire()); // Let's take a break
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Lock Mouse", "WhoUnpluggedMyMouse", "lock_mouse", -1, 1.0f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Lock Player In Vehicle", "ThereIsNoEscape", "lock_player_inside_vehicle")); // Lock player inside vehicle
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Low FOV", "LowFOV", "low_fov", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Millionaire", "IJustWonTheLottery", "millionaire"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "No Need To Hurry", "NoNeedToHurry", "no_need_to_hurry", -1, 1.5f)); // No Need To Hurry
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "No Shooting Allowed", "GunsAreDangerous", "no_shooting_allowed"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "No Tasks Allowed", "NoTasksAllowed", "no_tasks_allowed", 1000 * 10));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Pedal To The Metal", "PedalToTheMetal", "pedal_to_the_metal")); // Pedal To The Metal
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Portal Guns", "CaveJohnsonWouldBeProud", "portal_guns"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Quake FOV", "QuakeFOV", "quake_fov")); // Quake FOV
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Random Outfit", "ASetOfNewClothes", "random_outfit"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Reset Camera", "NaturalView", "reset_camera"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Remove All Weapons", "NoWeaponsForYou", "remove_all_weapons"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Remove Current Weapon", "IWillTakeThisGunFromYou", "remove_current_weapon"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Remove Random Weapon", "IWillTakeAGunFromYou", "remove_random_weapon"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Ring Ring !!", "RingRing", "ring_ring", 1000 * 30).DisableRapidFire()); // Ring Ring !!
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Shaky Hands", "IJustCantHoldStill", "shaky_hands"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Shuffle Blips", "ThesePlacesOnceMadeSense", "shuffle_blips"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Solid Water", "JesusInTheHouse", "walk_on_water"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Steer Bias (Left)", "LeftSideBias", "steer_bias_left"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Steer Bias (Right)", "RightSideBias", "steer_bias_right"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Super Low FOV", "SuperLowFOV", "super_low_fov", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "SUPER. HOT.", "SUPERHOT", "superhot"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Swim Like Tommy", "SwimLikeTommy", "swim_like_tommy", -1, 1.5f));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Switch To Unarmed", "PleaseUseYourFists", "switch_to_unarmed"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "The Firing Circus", "TheFiringCircus", "the_firing_circus", 1000 * 5).DisableRapidFire());
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "The Flash", "FastestManAlive", "the_flash")); // The Flash - Let's you run and swim at incredibly high speeds while not taking fall damage or drowing!
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Vehicle Bumper Camera", "FrontRowSeat", "vehicle_bumper_camera", -1, 1.0f)); // Vehicle Bumper Camera - Forces the vehicle's bumper camera
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Void Warp", "UnderTheMap", "void_warp"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Walk Off", "LetsGoForAWalk", "walk_off", 1000 * 30));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Warp In Random Vehicle", "ItsYourUber", "warp_player_into_random_vehicle"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Weapon Recoil", "ThoseAreSomeStrongWeapons", "weapon_recoil"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Weapon Roulette", "WeaponRoulette", "weapon_roulette")); // Weapon Roulette
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "You've been struck by...", "YouveBeenStruckBy", "struck_by_truck"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "You know what to do.", "WhyDidYouBlowUpRydersCar", "blow_up_ryders_car").DisableRapidFire());
                AddEffect(new FunctionEffect(Category.CustomEffects_Player, "Zooming FOV", "ZoomingFOV", "zooming_fov", 1000 * 30));
                // -------------- //

                // --- Time --- //
                AddEffect(new FunctionEffect(Category.CustomEffects_Framerate, "Powerpoint Presentation", "PowerpointPresentation", "fps_15", -1, 1.0f)); // Powerpoint Presentation (15 FPS)
                AddEffect(new FunctionEffect(Category.CustomEffects_Framerate, "Smooth Criminal", "SmoothCriminal", "fps_60")); // Smooth Criminal (60 FPS)
                // ------------ //

                // --- Vehicle --- //
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "All Vehicles Alarmy", "SoundTheAlarm", "all_vehicles_alarmy"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Backwards Vehicles", "BackPeepoHappy", "vehicle_rotation_backwards"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Beyblade", "LetItRip", "beyblade")); // Beyblade
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Carmageddon", "Carmageddon", "carmageddon")); // Carmageddon - Makes vehicles rain from the sky!
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Delete All Vehicles", "GoodbyeAllSweetRides", "delete_all_vehicles").DisableRapidFire());
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Delete Vehicle", "GoodbyeSweetRide", "delete_vehicle").DisableRapidFire());
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Do A Kickflip!", "DoAKickflip", "kickflip")); // Do A Kickflip!
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Explode Random Vehicle", "OneCarGoesBoom", "explode_random_vehicle").DisableRapidFire());
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Flat Vehicles", "FlatPeepoHappy", "vehicle_size_flat"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Flipped Vehicles", "FlippedPeepoHappy", "vehicle_rotation_flipped"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Freeze Vehicle", "StuckInTime", "freeze_vehicle", 1000 * 5));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Ghost Vehicles", "InvisibleVehicles", "invisible_vehicles")); // Invisible Vehicles
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Ghost Rider", "GhostRider", "ghost_rider")); // Set current vehicle constantly on fire
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "High Suspension Damping", "VeryDampNoBounce", "high_suspension_damping")); // Cars have high suspension damping
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "HONK!!!", "HONKHONK", "honk_vehicle", 1000 * 30).SetAudioVariations(5)); // Honk Vehicle
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Honk Boost", "GottaHonkFast", "honk_boost")); // Honk Boost
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Hot Wheels", "HotWheelsRacing", "vehicle_size_super_tiny"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Ignite Current Vehicle", "WayTooHot", "set_current_vehicle_on_fire").DisableRapidFire()); // Set current vehicle on fire
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Invert Vehicle Speed", "LetsGoBack", "invert_vehicle_speed"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Large Vehicles", "LargePeepoHappy", "vehicle_size_large"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Long Vehicles", "LongPeepoHappy", "vehicle_size_long"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Oh Hey, Tanks!", "OhHeyTanks", "oh_hey_tanks")); // Spawns tanks around the player
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Paper Thin Vehicles", "PaperPeepoHappy", "vehicle_size_paper_thin"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Pop Tires Of All Vehicles", "TiresBeGone", "pop_tires_of_all_vehicles")); // Pop tires of all vehicles
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Pride Vehicles", "AllColorsAreBeautiful", "pride_traffic")); // Pride Traffic / Rainbow Cars
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Relative Car Gravity", "SpiderCars", "vehicle_driving_on_walls"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Rota(to)ry Engines", "RotatoryEngines", "vehicle_rotation_based_on_speed"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Rotating Vehicles (X)", "RotatePeepoHappyX", "vehicle_rotation_continuous_x"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Rotating Vehicles (Y)", "RotatePeepoHappyY", "vehicle_rotation_continuous_y"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Rotating Vehicles (Z)", "RotatePeepoHappyZ", "vehicle_rotation_continuous_z"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Send Vehicles To Space", "StairwayToHeaven", "send_vehicles_to_space")); // Gives an immense upwards boost to all vehicles
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Speed (1994)", "KeepYourPace", "minimum_speed"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Super Wide Vehicles", "WiderPeepoHappy", "vehicle_size_super_wide"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Swap Vehicles On Impact", "SwapVehiclesOnImpact", "swap_vehicles_on_impact"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Tall Vehicles", "TallPeepoHappy", "vehicle_size_tall"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Tiny Vehicles", "TinyPeepoHappy", "vehicle_size_tiny"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Tipping Point", "TippingPoint", "very_flippable_vehicles")); // Vehicles are very easy to flip
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "To Drive Or Not To Drive", "ToDriveOrNotToDrive", "to_drive_or_not_to_drive")); // To drive or not to drive
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "To The Left, To The Right", "ToTheLeftToTheRight", "to_the_left_to_the_right")); // Gives cars a random velocity
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Turn Vehicles Around", "TurnAround", "turn_vehicles_around")); // Turn vehicles around
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Unflippable Vehicles", "UnflippableVehicles", "unflippable_vehicles")); // Vehicles are very hard to flip
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Vehicle Boost", "FullForceForward", "vehicle_boost"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Vehicle One Hit K.O.", "NoDings", "vehicle_one_hit_ko"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Wide Vehicles", "WidePeepoHappy", "vehicle_size_wide"));
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Your Car Floats When Hit", "ImTheBubbleCar", "your_car_floats_away_when_hit")); // Your Car Floats Away When Hit
                AddEffect(new FunctionEffect(Category.CustomEffects_Vehicle, "Zero Suspension Damping", "LowrideAllNight", "zero_suspension_damping"));  // Cars have almost zero suspension damping
                // --------------- //

                // --- Wanted --- //
                AddEffect(new FunctionEffect(Category.CustomEffects_Wanted, "Always Wanted", "ICanSeeStars", "always_wanted").DisableRapidFire()); // Always Wanted
                // -------------- //

                // --- Teleport --- //
                AddEffect(new FakeTeleportEffect("Fake Teleport", "HahaGotYourNose")); // Fake Teleport
                AddEffect(new FunctionEffect(Category.Teleportation, "Random Teleport", "LetsGoSightseeing", "random_teleport").DisableRapidFire()); // Random Teleport - Teleports CJ to a random location on the map
                AddEffect(new FunctionEffect(Category.Teleportation, "Teleport To Waypoint", "IKnowJustTheRightPlace", "teleport_to_marker", 1000 * 30)); // Teleport To Waypoint
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

            AudioPlayer.INSTANCE.CreateAndPrintAudioFileReadme();
        }

        public static WeightedRandomBag<AbstractEffect> EnabledEffects { get; } = new();

        public static int GetEnabledEffectsCount() => EnabledEffects.Count;

        public static AbstractEffect GetByID(string id, bool onlyEnabled = false) => (onlyEnabled ? EnabledEffects : Effects).Find(e => e.item.GetID().Equals(id)).item;

        public static AbstractEffect GetByWord(string word, bool onlyEnabled = false) => (onlyEnabled ? EnabledEffects : Effects).Find(e => !string.IsNullOrEmpty(e.item.Word) && string.Equals(e.item.Word, word, StringComparison.OrdinalIgnoreCase)).item;

        public static AbstractEffect GetByDescription(string description, bool onlyEnabled = false) => (onlyEnabled ? EnabledEffects : Effects).Find(e => string.Equals(description, e.item.GetDisplayName(DisplayNameType.UI), StringComparison.OrdinalIgnoreCase)).item;

        public static AbstractEffect GetRandomEffect(bool onlyEnabled = false, int attempts = 0, bool addEffectToCooldown = false)
        {
            WeightedRandomBag<AbstractEffect> effects = onlyEnabled ? EnabledEffects : Effects;

            if (effects.Count > 0)
            {
                (bool success, AbstractEffect effect) = effects.GetRandom(RandomHandler.Random, entry => !EffectCooldowns.ContainsKey(entry.item));
                if (!success || effect is null || attempts++ > 10)
                {
                    ResetEffectCooldowns();
                    return GetRandomEffect(onlyEnabled, attempts, addEffectToCooldown);
                }

                if (!onlyEnabled)
                {
                    if (addEffectToCooldown)
                    {
                        SetCooldownForEffect(effect);
                    }
                }

                return effect;

                //if (effect is not null)
                //{
                //    return effect;
                //}
                //else
                //{
                //    return GetRandomEffect(onlyEnabled, attempts, addEffectToCooldown);
                //}
            }

            return null;
        }

        public static bool ShouldCooldown = true;

        private static void CheckForNonCooldownEffects()
        {
            IEnumerable<WeightedRandomBag<AbstractEffect>.Entry> cooldownEffects = Effects.Get().Where(entry => EffectCooldowns.ContainsKey(entry.item));
            if (cooldownEffects.Count() > Config.GetEffectCooldowns())
            {
                ResetEffectCooldowns();
            }
        }

        public static void CooldownEffects()
        {
            if (!ShouldCooldown)
            {
                return;
            }

            lock (EffectCooldowns)
            {
                foreach (KeyValuePair<AbstractEffect, int> item in EffectCooldowns.ToList())
                {
                    if (EffectCooldowns[item.Key]-- <= 0)
                    {
                        EffectCooldowns.Remove(item.Key);
                    }
                }
            }

            CheckForNonCooldownEffects();
        }

        public static void ResetEffectCooldowns()
        {
            lock (EffectCooldowns)
            {
                EffectCooldowns.Clear();
            }
        }

        public static void SetCooldownForEffect(AbstractEffect effect, int cooldown = -1)
        {
            if (!ShouldCooldown)
            {
                return;
            }

            if (effect is not null && effect.IsCooldownable())
            {
                if (cooldown < 0)
                {
                    cooldown = Math.Max(0, Config.GetEffectCooldowns());
                }

                cooldown = Math.Min(cooldown, Config.GetEffectCooldowns());

                lock (EffectCooldowns)
                {
                    EffectCooldowns[effect] = cooldown;
                }
            }
        }

        public static AbstractEffect RunEffect(string id, bool onlyEnabled = true) => RunEffect(GetByID(id, onlyEnabled));

        public static AbstractEffect RunEffect(AbstractEffect effect, int seed = -1, int duration = -1)
        {
            if (effect is not null)
            {
                Task _ = effect.RunEffect(seed, duration);
                SetCooldownForEffect(effect);

                CooldownEffects();
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
