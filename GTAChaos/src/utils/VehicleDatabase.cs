// Copyright (c) 2019 Lordmau5
using System;
using System.Collections.Generic;

namespace GTAChaos.Utils
{
    internal class VehicleDatabase
    {
        private static readonly List<int> potentialVehicles_SA = new()
        {
            // Bikes
            581, // BF-400
            481, // BMX
            462, // Faggio
            521, // FCR-900
            463, // Freeway
            522, // NRG-500
            461, // PCJ-600
            448, // Pizzaboy
            468, // Sanchez

            // 2-Door & Compact Cars
            602, // Alpha
            496, // Blista Compact
            401, // Bravura
            518, // Buccaneer
            527, // Cadrona
            589, // Club
            419, // Esperanto
            587, // Euros
            533, // Feltzer
            526, // Fortune
            474, // Hermes
            545, // Hustler
            517, // Majestic
            410, // Manana
            600, // Picador
            436, // Previon
            439, // Stallion
            549, // Tampa
            491, // Virgo

            // 4-Door & Luxury Cars
            445, // Admiral
            604, // Damaged Glendale
            507, // Elegant
            585, // Emperor
            466, // Glendale
            492, // Greenwood
            546, // Intruder
            551, // Merit
            516, // Nebula
            467, // Oceanic
            426, // Premier
            547, // Primo
            405, // Sentinel
            580, // Stafford
            409, // Stretch
            550, // Sunrise
            566, // Tahoma
            540, // Vincent
            421, // Washington
            529, // Willard

            // Civil Service
            485, // Baggage
            431, // Bus
            438, // Cabbie
            437, // Coach
            574, // Sweeper
            420, // Taxi
            525, // Towtruck
            408, // Trashmaster
            552, // Utility Van

            // Government Vehicles
            416, // Ambulance
            433, // Barracks
            427, // Enforcer
            490, // FBI Rancher
            528, // FBI Truck
            407, // Fire Truck
            523, // HPV1000
            470, // Patriot
            596, // Police LS
            598, // Police LV
            599, // Police Ranger
            597, // Police SF
            432, // Rhino
            601, // S.W.A.T.
            428, // Securicar

            // Heavy & Utility Trucks
            499, // Benson
            498, // Boxville
            524, // Cement Truck
            532, // Combine Harvester
            578, // DFT-30
            486, // Dozer
            406, // Dumper
            573, // Dune
            455, // Flatbed
            588, // Hotdog
            403, // Linerunner
            423, // Mr. Whoopee
            414, // Mule
            443, // Packer
            515, // Roadtrain
            514, // Tanker
            531, // Tractor
            456, // Yankee

            // Light Trucks & Vans
            459, // Berkley's RC Van
            422, // Bobcat
            482, // Burrito
            605, // Damaged Sadler
            530, // Forklift
            418, // Moonbeam
            572, // Mower
            582, // News Van
            413, // Pony
            440, // Rumpo
            543, // Sadler
            583, // Tug
            478, // Walton
            554, // Yosemite

            // SUVs & Wagons
            579, // Huntley
            400, // Landstalker
            404, // Perennial
            489, // Rancher
            479, // Regina
            442, // Romero
            458, // Solair

            // Lowriders
            536, // Blade
            575, // Broadway
            534, // Remington
            567, // Savanna
            535, // Slamvan
            576, // Tornado
            412, // Voodoo

            // Muscle Cars
            402, // Buffalo
            542, // Clover
            603, // Phoenix
            475, // Sabre

            // Street Racers
            429, // Banshee
            541, // Bullet
            415, // Cheetah
            480, // Comet
            562, // Elegy
            565, // Flash
            434, // Hotknife
            494, // Hotring Racer
            411, // Infernus
            559, // Jester
            561, // Stratum
            560, // Sultan
            506, // Super GT
            451, // Turismo
            558, // Uranus
            555, // Windsor
            477, // ZR-350

            // Recreational
            568, // Bandito
            424, // BF Injection
            504, // Bloodring Banger
            457, // Caddy
            483, // Camper
            508, // Journey
            571, // Kart
            500, // Mesa
            444, // Monster
            471, // Quadbike
            495, // Sandking
            539, // Vortex
        };

        private static readonly string[] vehicleNames_SA =
        {
            "Landstalker", "Bravura", "Buffalo", "Linerunner",
            "Perennial", "Sentinel", "Dumper", "Firetruck",
            "Trashmaster", "Stretch", "Manana", "Infernus",
            "Voodoo", "Pony", "Mule", "Cheetah", "Ambulance",
            "Leviathan", "Moonbeam", "Esperanto", "Taxi", "Washington",
            "Bobcat", "Mr Whoopee", "BF Injection", "Hunter", "Premier",
            "Enforcer", "Securicar", "Banshee", "Predator", "Bus", "Rhino",
            "Barracks", "Hotknife", "Artic Trailer 1", "Previon", "Coach",
            "Cabbie", "Stallion", "Rumpo", "RC Bandit", "Romero", "Packer",
            "Monster", "Admiral", "Squalo", "Seasparrow", "Pizza Boy", "Tram",
            "Artic Trailer 2", "Turismo", "Speeder", "Reefer", "Tropic",
            "Flatbed", "Yankee", "Caddy", "Solair", "Top Fun", "Skimmer",
            "PCJ-600", "Faggio", "Freeway", "RC Baron", "RC Raider", "Glendale",
            "Oceanic", "Sanchez", "Sparrow", "Patriot", "Quad", "Coastguard",
            "Dinghy", "Hermes", "Sabre", "Rustler", "ZR-350", "Walton",
            "Regina", "Comet", "BMX", "Burrito", "Camper", "Marquis",
            "Baggage", "Dozer", "Maverick", "VCN Maverick", "Rancher",
            "FBI Rancher", "Virgo", "Greenwood", "Jetmax", "Hotring",
            "Sandking", "Blista Compact", "Police maverick", "Boxville",
            "Benson", "Mesa", "RC Goblin", "Hotring A", "Hotring B",
            "Bloodring Banger", "Rancher", "Super GT",
            "Elegant", "Journey", "Bike", "Mountain Bike", "Beagle",
            "Cropduster", "Stuntplane", "Petrol", "Roadtrain", "Nebula",
            "Majestic", "Buccaneer", "Shamal", "Hydra", "FCR-900", "NRG-500",
            "HPV1000", "Cement", "Towtruck", "Fortune", "Cadrona", "FBI Truck",
            "Willard", "Forklift", "Tractor", "Combine", "Feltzer",
            "Remington", "Slamvan", "Blade", "Freight", "Streak", "Vortex",
            "Vincent", "Bullet", "Clover", "Sadler", "Firetruck LA",
            "Hustler", "Intruder", "Primo", "Cargobob", "Tampa", "Sunrise",
            "Merit", "Utility Van", "Nevada", "Yosemite", "Windsor",
            "Monster A", "Monster B", "Uranus", "Jester", "Sultan",
            "Stratum", "Elegy", "Raindance", "RC Tiger", "Flash", "Tahoma",
            "Savanna", "Bandito", "Freight", "Streak", "Kart",
            "Mower", "Dune", "Sweeper", "Broadway", "Tornado",
            "AT-400", "DFT-30", "Huntley", "Stafford", "BF-400", "Newsvan",
            "Tug", "Petrol Tanker", "Emperor", "Wayfarer", "Euros", "Hotdog",
            "Club", "Freight Box", "Artic Trailer 3", "Andromada", "Dodo",
            "RC Cam", "Launch", "Cop Car LS", "Cop Car SF", "Cop Car LV",
            "Ranger", "Picador", "S.W.A.T. Tank", "Alpha", "Phoenix",
            "Glendale", "Sadler", "Bag Box A", "Bag Box B",
            "Stairs", "Boxville", "Farm Trailer", "Utility Van Trailer"
        };

        private static readonly string[] vehicleNames_VC =
        {
            "Landstalker", "Idaho", "Stinger", "Linerunner", "Perennial", "Sentinel",
            "Rio", "Firetruck", "Trashmaster", "Stretch", "Manana", "Infernus", "Voodoo",
            "Pony", "Mule", "Cheetah", "Ambulance", "FBI Washington", "Moonbeam", "Esperanto",
            "Taxi", "Washington", "Bobcat", "Mr. Whoopee", "BF Injection", "Hunter",
            "Police Car", "Enforcer", "Securicar", "Banshee", "Predator", "Bus",
            "Rhino", "Barracks", "Cuban Hermes", "Helicopter", "Angel", "Coach",
            "Cabbie", "Stallion", "Rumpo", "RC Bandit", "Romero", "Packer",
            "Sentinel XS", "Admiral", "Squalo", "Seasparrow", "Pizzaboy", "Gang Burrito",
            "Airplane", "Dodo", "Speeder", "Reefer", "Tropic", "Flatbed", "Yankee", "Caddy",
            "Zebra Cab", "Top Fun", "Skimmer", "PCJ 600", "Faggio", "Freeway", "RC Baron",
            "RC Raider", "Glendale", "Oceanic", "Sanchez", "Sparrow", "Patriot",
            "Love Fist", "Coastguard", "Dinghy", "Hermes", "Sabre", "Sabre Turbo",
            "Phoenix", "Walton", "Regina", "Comet", "Deluxo", "Burrito", "Spand Express",
            "Marquis", "Baggage", "Kaufman Cab", "Maverick", "VCN Maverick", "Rancher",
            "FBI Rancher", "Virgo", "Greenwood", "Cuban Jetmax", "Hotring Racer", "Sandking",
            "Blista Compact", "Police Maverick", "Boxville", "Benson", "Mesa Grande", "RC Goblin",
            "Hotring Racer Alt.", "Hotring Racer Alt. 2", "Bloodring Banger", "Bloodring Banger Alt.",
            "Cheetah"
        };

        public static List<int> GetPotentialVehicles()
        {
            if (Shared.SelectedGame == "san_andreas")
            {
                return potentialVehicles_SA;
            }

            return new();
        }

        public static string GetVehicleName(int modelID)
        {
            if (Shared.SelectedGame == "san_andreas")
            {
                return vehicleNames_SA[Math.Max(400, Math.Min(modelID, 611)) - 400];
            }
            else if (Shared.SelectedGame == "vice_city")
            {
                return vehicleNames_VC[Math.Max(130, Math.Min(modelID, 236)) - 130];
            }

            return "";
        }
    }
}
