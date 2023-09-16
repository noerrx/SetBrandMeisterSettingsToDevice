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

        public static void GetUserInputs(LoadedContents loadedContents, Dictionary<int, BmDevice> bmDevices, out BmDevice bmDeviceSelected, out Setting deviceSettingSelected)
        {
            ConsoleExt.WriteLine(string.Empty);
            ConsoleExt.WriteLine("Which device?", Severity.Question);

            int counter = 1;
            foreach (var bmDevice in bmDevices.Values)
            {
                ConsoleExt.WriteLine($"\t[{counter}] {bmDevice.id} ({bmDevice.callsign}) - {ProgramHelpers.BMOnlineStatusToText(bmDevice.last_seen)}", Severity.None);
                counter++;
            }
            ConsoleExt.Write("Enter the Index of the Device ([1]): ", Severity.Question);
            string consoleInput = Console.ReadLine();

            if (string.IsNullOrEmpty(consoleInput))
            {
                consoleInput = 1.ToString();
            }

            int deviceNumber = int.Parse(consoleInput);

            var localBrandMeisterDeviceSelected = bmDevices.Values.ToArray()[deviceNumber - 1];
            Device deviceSelected = loadedContents.DeviceSettings.devices.Where(dev => dev.deviceId == localBrandMeisterDeviceSelected.id).First();

            ConsoleExt.WriteLine("Which setting?", Severity.Question);

            counter = 1;
            foreach (var setting in deviceSelected.settings)
            {
                ConsoleExt.WriteLine($"\t[{counter}] {setting.name}", Severity.None);
                counter++;
            }
            ConsoleExt.Write("Enter the Index of the Device ([1]): ", Severity.Question);
            consoleInput = Console.ReadLine();

            if (string.IsNullOrEmpty(consoleInput))
            {
                consoleInput = 1.ToString();
            }

            int settingNumber = int.Parse(consoleInput);
            deviceSettingSelected = deviceSelected.settings[settingNumber - 1];

            bmDeviceSelected = localBrandMeisterDeviceSelected;
        }

        public static async Task<Dictionary<int, BmDevice>> LoadAllBrandMeisterDeviceInfos(LoadedContents loadedContents)
        {
            Dictionary<int, BmDevice> bmDevices = new Dictionary<int, BmDevice>();
            ConsoleExt.WriteLine("Loading Brandmeister-Device-Infos");
            foreach (var device in loadedContents.DeviceSettings.devices)
            {
                if (!bmDevices.ContainsKey(device.deviceId))
                {
                    bmDevices.Add(device.deviceId, await BrandMeisterWeb.GetDeviceToDeviceID(device.deviceId));
                }
            }
            return bmDevices;
        }

    }
}