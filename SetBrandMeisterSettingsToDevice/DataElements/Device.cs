using System.Collections.Generic;

namespace SetBrandMeisterSettingsToDevice.DataElements
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    internal class Device
    {
        public int deviceId { get; set; }
        public List<Setting> settings { get; set; }
    }
}
