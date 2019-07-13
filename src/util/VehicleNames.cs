using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTA_SA_Chaos.src.util
{
    class VehicleNames
    {
        private static readonly string[] vehicleNames = {
            "Landstalker", "Bravura", "Buffalo", "Linerunner",
            "Perenail", "Sentinel", "Dumper", "Firetruck",
            "Trashmaster", "Stretch", "Manana", "Infernus",
            "Voodoo", "Pony", "Mule", "Cheetah", "Ambulance",
            "Levetian", "Moonbeam", "Esperanto", "Taxi", "Washington",
            "Bobcat", "Mr Whoopee", "BF Injection", "Hunter", "Premier",
            "Enforcer", "Securicar", "Banshee", "Predator", "Bus", "Rhino",
            "Barracks", "Hotknife", "Artic Trailer 1", "Previon", "Coach",
            "Cabbie", "Stallion", "Rumpo", "RC Bandit", "Romero", "Packer",
            "Monster", "Admiral", "Squalo", "Seasparrow", "Pizza Boy", "Tram",
            "Artic Trailer 2", "Turismo", "Speeder", "Reefer", "Tropic",
            "Flatbed", "Yankee", "Caddy", "Solair", "Top Fun", "Skimmer",
            "PCJ 600", "Faggio", "Freeway", "RC Baron", "RC Raider", "Glendale",
            "Oceanic", "Sanchez", "Sparrow", "Patriot", "Quad", "Coastgaurd",
            "Dinghy", "Hermes", "Sabre", "Rustler", "ZR 350", "Walton",
            "Regina", "Comet", "BMX", "Burriro", "Camper", "Marquis",
            "Baggage", "Dozer", "Maverick", "VCN Maverick", "Rancher",
            "FBI Rancher", "Virgo", "Greenwood", "Jetmax", "Hotring",
            "Sandking", "Blistac", "Police maverick", "Boxville",
            "Benson", "Mesa", "RC Goblin", "Hotring A", "Hotring B",
            "Bloodring Banger", "Rancher", "Super GT",
            "Elegant", "Journey", "Bike", "Mountain Bike", "Beagle",
            "Cropduster", "Stuntplane", "Petrol", "Roadtrain", "Nebula",
            "Majestic", "Buccaneer", "Shamal", "Hydra", "FCR 900", "NRG 500",
            "HPV 1000", "Cement", "Towtruck", "Fortune", "Cadrona", "FBI Truck",
            "Williard", "Forklift", "Tractor", "Combine", "Feltzer",
            "Remington", "Slamvan", "Blade", "Freight", "Streak", "Vortex",
            "Vincent", "Bullet", "Clover", "Sadler", "Firetruck LA",
            "Hustler", "Intruder", "Primo", "Cargobob", "Tampa", "Sunrise",
            "Merit", "Utility Van", "Nevada", "Yosemite", "Windsor",
            "Monster A", "Monster B", "Uranus", "Jester", "Sultan",
            "Stratum", "Elegy", "Raindance", "RC Tiger", "Flash", "Tahoma",
            "Savanna", "Bandito", "Freight", "Streak", "Kart",
            "Mower", "Duneride", "Sweeper", "Broadway", "Tornado",
            "AT 400", "DFT 30", "Huntley", "Stafford", "BF 400", "Newsvan",
            "Tug", "Petrol Tanker", "Emperor", "Wayfarer", "Euros", "Hotdog",
            "Club", "Freight Box", "Artic Trailer 3", "Andromada", "Dodo",
            "RC Cam", "Launch", "Cop Car LS", "Cop Car SF", "Cop Car LV",
            "Ranger", "Picador", "S.W.A.T. Tank", "Alpha", "Phoenix",
            "Glendale", "Sadler", "Bag Box A", "Bag Box B",
            "Stairs", "Boxville", "Farm Trailer", "Utility Van Trailer"
        };

        public static string GetVehicleName(int modelID)
        {
            return vehicleNames[Math.Max(400, Math.Min(modelID, 611)) - 400];
        }
    }
}
