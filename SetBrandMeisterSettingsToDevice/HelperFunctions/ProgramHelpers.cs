using Newtonsoft.Json;
using SetBrandMeisterSettingsToDevice.DataElements;
using SetBrandMeisterSettingsToDevice.WebFunctions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SetBrandMeisterSettingsToDevice.HelperFunctions
{
    internal static class ProgramHelpers
    {
        private static string deviceSettingsFileName = "DeviceSettings.json";
        private static string brandmeisterAPIKeyFileName = "BrandMeisterAPIKey.json";

        public static async Task<LoadedContents> LoadFileContentAsync()
        {
            DeviceSettings deviceSettings = null;
            BrandMeisterApiKeyInfo brandMeisterApiKeyInfo = null;
            ConsoleExt.Write("Looking for the Settings-File...");
            if (File.Exists(deviceSettingsFileName))
            {
                string deviceSettingsFileContent = File.ReadAllText(deviceSettingsFileName);
                deviceSettings = JsonConvert.DeserializeObject<DeviceSettings>(deviceSettingsFileContent);
                ConsoleExt.WriteLine($"Loaded. {deviceSettings.devices.Count} Device(s).", Severity.Info, System.ConsoleColor.Green);
            }
            else
            {
                ConsoleExt.WriteLine("File Not Found.", Severity.Error, System.ConsoleColor.Red);
                deviceSettings = null;
            }
            ConsoleExt.Write("Looking for Brandmeister-API-Key-File...");
            if (File.Exists(brandmeisterAPIKeyFileName))
            {
                string brandmeisterAPIKeyFileContent = File.ReadAllText(brandmeisterAPIKeyFileName);
                brandMeisterApiKeyInfo = JsonConvert.DeserializeObject<BrandMeisterApiKeyInfo>(brandmeisterAPIKeyFileContent);
                ConsoleExt.WriteLine($"Loaded.", Severity.Info, System.ConsoleColor.Green);
                if (!await BrandMeisterWeb.IsAPIKeyIsValid(brandMeisterApiKeyInfo))
                {
                    brandMeisterApiKeyInfo = null;
                }
            }
            else
            {
                ConsoleExt.WriteLine("File Not Found.", Severity.Error, System.ConsoleColor.Red);
                ConsoleExt.WriteLine($"No File named {brandmeisterAPIKeyFileName} found. Please create one using https://brandmeister.network/?page=profile-api", Severity.Error);
                brandMeisterApiKeyInfo = null;
            }
            return new LoadedContents(deviceSettings, brandMeisterApiKeyInfo);
        }


        public static List<TalkGroupOfDevice> GetDeltaOfTalkGroupsToAdd(List<TalkGroupOfDevice> talkGroupsOfDevice, List<Talkgroup> talkgroups, int id)
        {
            List<TalkGroupOfDevice> retval = new List<TalkGroupOfDevice>();

            foreach (var talkGroup in talkgroups)
            {
                if (!talkGroupsOfDevice.Any(tg => tg.talkgroup == talkGroup.talkgroup && tg.slot == talkGroup.slot))
                {
                    retval.Add(new TalkGroupOfDevice() { repeaterid = id, slot = talkGroup.slot, talkgroup = talkGroup.talkgroup });
                }
            }

            return retval;
        }

        public static List<TalkGroupOfDevice> GetDeltaOfTalkGroupsToRemove(List<TalkGroupOfDevice> talkGroupsOfDevice, List<Talkgroup> talkgroups)
        {
            List<TalkGroupOfDevice> retval = new List<TalkGroupOfDevice>();

            foreach (var talkGroup in talkGroupsOfDevice)
            {
                if (!talkgroups.Any(tg => tg.talkgroup == talkGroup.talkgroup && tg.slot == talkGroup.slot))
                {
                    retval.Add(talkGroup);
                }
            }

            return retval;
        }

        public static string BMOnlineStatusToText(DateTime last_seen)
        {
            if (DateTime.Now - last_seen.ToLocalTime() < TimeSpan.FromMinutes(2))
            {
                return "ONLINE";
            }
            return "OFFLINE";
        }
    }
}