using System;

namespace SetBrandMeisterSettingsToDevice.DataElements
{
    public class BmDevice
    {
        public int id { get; set; }
        public string callsign { get; set; }
        public string hardware { get; set; }
        public string firmware { get; set; }
        public string tx { get; set; }
        public string rx { get; set; }
        public int colorcode { get; set; }
        public int status { get; set; }
        public int lastKnownMaster { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string city { get; set; }
        public string website { get; set; }
        public int pep { get; set; }
        public int agl { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime last_seen { get; set; }
        public string statusText { get; set; }
    }
}
