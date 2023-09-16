using System.Collections.Generic;

namespace SetBrandMeisterSettingsToDevice.DataElements
{
    internal class Setting
    {
        public string name { get; set; }
        public List<Talkgroup> talkgroups { get; set; }
    }
}
