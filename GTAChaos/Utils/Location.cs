// Copyright (c) 2019 Lordmau5
namespace GTAChaos.Utils
{
    public sealed class Location
    {
        public readonly string Id;
        public int X;
        public int Y;
        public int Z;

        public Location(string id, int x, int y, int z)
        {
            Id = id;
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return Id;
        }

        public static readonly Location GrooveStreet = new Location("grove_street", 2493, -1670, 15);
        public static readonly Location LSTower = new Location("ls_tower", 1544, -1353, 332);
        public static readonly Location LSPier = new Location("ls_pier", 836, -2061, 15);
        public static readonly Location LSAirport = new Location("ls_airport", 2109, -2544, 16);
        public static readonly Location LSDocks = new Location("ls_docks", 2760, -2456, 16);
        public static readonly Location MountChiliad = new Location("mount_chiliad", -2233, -1737, 483);
        public static readonly Location SFAirport = new Location("sf_airport", -1083, 409, 17);
        public static readonly Location SFBridge = new Location("sf_bridge", -2669, 1595, 220);
        public static readonly Location Area52 = new Location("area_52", 213, 1911, 20);
        public static readonly Location LVQuarry = new Location("lv_quarry", 614, 856, -40);
        public static readonly Location LVAirport = new Location("lv_airport", 1612, 1166, 17);
        public static readonly Location LVSatellite = new Location("lv_satellite", -310, 1524, 78);
    }
}
