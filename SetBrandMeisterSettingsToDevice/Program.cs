using SetBrandMeisterSettingsToDevice.DataElements;
using SetBrandMeisterSettingsToDevice.HelperFunctions;
using SetBrandMeisterSettingsToDevice.WebFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetBrandMeisterSettingsToDevice
{
    internal class Program
    {
        private static readonly Dictionary<int, BmDevice> bmDevices = new Dictionary<int, BmDevice>();
        static async Task Main()
        {
            LoadedContents loadedContents = await ProgramHelpers.LoadFileContentAsync();
            if (!loadedContents.Success) { return; }

            ConsoleExt.WriteLine("Loading Brandmeister-Device-Infos");
            foreach (var device in loadedContents.DeviceSettings.devices)
            {
                if (!bmDevices.ContainsKey(device.deviceId))
                {
                    bmDevices.Add(device.deviceId, await BrandMeisterWeb.GetDeviceToDeviceID(device.deviceId));
                }
            }

            ConsoleExt.WriteLine("Which device?", Severity.Question);

            int counter = 1;
            foreach (var bmDevice in bmDevices.Values)
            {
                ConsoleExt.WriteLine($"\t[{counter}] {bmDevice.id} ({bmDevice.callsign}) - {ProgramHelpers.BMOnlineStatusToText(bmDevice.last_seen)}", Severity.None);
                counter++;
            }
            ConsoleExt.Write("Eingabe ([1]): ");
            string consoleInput = Console.ReadLine();

            if (string.IsNullOrEmpty(consoleInput))
            {
                consoleInput = 1.ToString();
            }

            int deviceNumber = int.Parse(consoleInput);

            BmDevice bmDeviceSelected = bmDevices.Values.ToArray()[deviceNumber - 1];
            Device deviceSelected = loadedContents.DeviceSettings.devices.Where(dev => dev.deviceId == bmDeviceSelected.id).First();

            ConsoleExt.WriteLine("Which setting?", Severity.Question);

            counter = 1;
            foreach (var setting in deviceSelected.settings)
            {
                ConsoleExt.WriteLine($"\t[{counter}] {setting.name}", Severity.None);
                counter++;
            }
            ConsoleExt.Write("Eingabe ([1]): ");
            consoleInput = Console.ReadLine();

            if (string.IsNullOrEmpty(consoleInput))
            {
                consoleInput = 1.ToString();
            }

            int settingNumber = int.Parse(consoleInput);
            Setting deviceSettingSelected = deviceSelected.settings[settingNumber - 1];

            ConsoleExt.WriteLine($"Applying Setting \"{deviceSettingSelected.name}\" to Device \"{bmDeviceSelected.id} ({bmDeviceSelected.callsign}) - {ProgramHelpers.BMOnlineStatusToText(bmDeviceSelected.last_seen)}\"");

            List<TalkGroupOfDevice> talkGroupsOfDevice = await BrandMeisterWeb.GetTalkGroupsToDeviceID(bmDeviceSelected.id);

            List<TalkGroupOfDevice> deltaToRemove = ProgramHelpers.GetDeltaOfTalkGroupsToRemove(talkGroupsOfDevice, deviceSettingSelected.talkgroups);
            List<TalkGroupOfDevice> deltaToAdd = ProgramHelpers.GetDeltaOfTalkGroupsToAdd(talkGroupsOfDevice, deviceSettingSelected.talkgroups, bmDeviceSelected.id);

            ConsoleExt.WriteLine($"Removing {deltaToRemove.Count} Talkgroups");
            foreach (var talkGroup in deltaToRemove)
            {
                if (!await BrandMeisterWeb.RemoveTalkGroupFromDevice(talkGroup, loadedContents.BrandMeisterApiKeyInfo))
                {
                    ConsoleExt.WriteLine($"Fehler beim Löschen von {talkGroup.talkgroup} auf {talkGroup.repeaterid}, Slot {talkGroup.slot}. Breche ab.");
                    return;
                }
            }

            ConsoleExt.WriteLine($"Adding {deltaToAdd.Count} Talkgroups");
            foreach (var talkGroup in deltaToAdd)
            {
                if (!await BrandMeisterWeb.AddTalkGroupToDevice(talkGroup, loadedContents.BrandMeisterApiKeyInfo))
                {
                    ConsoleExt.WriteLine($"Fehler beim Hinzufügen von {talkGroup.talkgroup} auf {talkGroup.repeaterid}, Slot {talkGroup.slot}. Breche ab.");
                    return;
                }
            }
        }
    }
}
