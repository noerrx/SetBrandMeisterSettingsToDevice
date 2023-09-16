namespace SetBrandMeisterSettingsToDevice.DataElements
{
    internal class LoadedContents
    {
        public LoadedContents(DeviceSettings deviceSettings, BrandMeisterApiKeyInfo brandMeisterApiKeyInfo)
        {
            DeviceSettings = deviceSettings;
            BrandMeisterApiKeyInfo = brandMeisterApiKeyInfo;
        }

        public DeviceSettings DeviceSettings { get; }
        public BrandMeisterApiKeyInfo BrandMeisterApiKeyInfo { get; }
        public bool Success
        {
            get
            {
                return DeviceSettings != null && BrandMeisterApiKeyInfo != null;
            }

        }
    }
}