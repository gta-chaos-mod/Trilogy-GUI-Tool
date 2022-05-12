// Copyright (c) 2019 Lordmau5
using System;

namespace GTAChaos.Utils
{
    internal class VehicleNames
    {
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
            "PCJ 600", "Faggio", "Freeway", "RC Baron", "RC Raider", "Glendale",
            "Oceanic", "Sanchez", "Sparrow", "Patriot", "Quad", "Coastguard",
            "Dinghy", "Hermes", "Sabre", "Rustler", "ZR 350", "Walton",
            "Regina", "Comet", "BMX", "Burrito", "Camper", "Marquis",
            "Baggage", "Dozer", "Maverick", "VCN Maverick", "Rancher",
            "FBI Rancher", "Virgo", "Greenwood", "Jetmax", "Hotring",
            "Sandking", "Blista Compact", "Police maverick", "Boxville",
            "Benson", "Mesa", "RC Goblin", "Hotring A", "Hotring B",
            "Bloodring Banger", "Rancher", "Super GT",
            "Elegant", "Journey", "Bike", "Mountain Bike", "Beagle",
            "Cropduster", "Stuntplane", "Petrol", "Roadtrain", "Nebula",
            "Majestic", "Buccaneer", "Shamal", "Hydra", "FCR 900", "NRG 500",
            "HPV 1000", "Cement", "Towtruck", "Fortune", "Cadrona", "FBI Truck",
            "Willard", "Forklift", "Tractor", "Combine", "Feltzer",
            "Remington", "Slamvan", "Blade", "Freight", "Streak", "Vortex",
            "Vincent", "Bullet", "Clover", "Sadler", "Firetruck LA",
            "Hustler", "Intruder", "Primo", "Cargobob", "Tampa", "Sunrise",
            "Merit", "Utility Van", "Nevada", "Yosemite", "Windsor",
            "Monster A", "Monster B", "Uranus", "Jester", "Sultan",
            "Stratum", "Elegy", "Raindance", "RC Tiger", "Flash", "Tahoma",
            "Savanna", "Bandito", "Freight", "Streak", "Kart",
            "Mower", "Dune", "Sweeper", "Broadway", "Tornado",
            "AT 400", "DFT 30", "Huntley", "Stafford", "BF 400", "Newsvan",
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
