namespace GTA_SA_Chaos.util
{
    public sealed class Location
    {
        public int X;
        public int Y;
        public int Z;

        public Location(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"{X},{Y},{Z}";
        }

        public static readonly Location GrooveStreet = new Location(2493, -1670, 15);
        public static readonly Location LSTower = new Location(1544, -1353, 332);
        public static readonly Location LSPier = new Location(836, -2061, 15);
        public static readonly Location LSAirport = new Location(2109, -2544, 16);
        public static readonly Location LSDocks = new Location(2760, -2456, 16);
        public static readonly Location MountChiliad = new Location(-2233, -1737, 483);
        public static readonly Location SFAirport = new Location(-1083, 409, 17);
        public static readonly Location SFBridge = new Location(-2669, 1595, 220);
        public static readonly Location Area52 = new Location(213, 1911, 20);
        public static readonly Location LVQuarry = new Location(614, 856, -40);
        public static readonly Location LVAirport = new Location(1612, 1166, 17);
        public static readonly Location LVSatellite = new Location(-310, 1524, 78);
    }
}
