using SetBrandMeisterSettingsToDevice.DataElements;
using SetBrandMeisterSettingsToDevice.HelperFunctions;
using SetBrandMeisterSettingsToDevice.WebFunctions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SetBrandMeisterSettingsToDevice
{
    internal class Program
    {
        static async Task Main()
        {


            LoadedContents loadedContents = await ProgramHelpers.LoadFileContentAsync();

            if (!loadedContents.Success) { return; }

            Dictionary<int, BmDevice> bmDevices = await ProgramHelpers.LoadAllBrandMeisterDeviceInfos(loadedContents);

            BmDevice bmDeviceSelected;
            Setting deviceSettingSelected;

            ProgramHelpers.GetUserInputs(loadedContents, bmDevices, out bmDeviceSelected, out deviceSettingSelected);

            ConsoleExt.WriteLine($"Applying Setting \"{deviceSettingSelected.name}\" to Device \"{bmDeviceSelected.id} ({bmDeviceSelected.callsign}) - {ProgramHelpers.BMOnlineStatusToText(bmDeviceSelected.last_seen)}\"");

            List<TalkGroupOfDevice> talkGroupsOfDevice = await BrandMeisterWeb.GetTalkGroupsToDeviceID(bmDeviceSelected.id);
            List<TalkGroupOfDevice> deltaToRemove = ProgramHelpers.GetDeltaOfTalkGroupsToRemove(talkGroupsOfDevice, deviceSettingSelected.talkgroups);
            List<TalkGroupOfDevice> deltaToAdd = ProgramHelpers.GetDeltaOfTalkGroupsToAdd(talkGroupsOfDevice, deviceSettingSelected.talkgroups, bmDeviceSelected.id);
            List<int> slotsToDrop = deviceSettingSelected.talkgroups.Select(tg => tg.slot).Distinct().ToList();

            ConsoleExt.WriteLine($"Removing {deltaToRemove.Count} Talkgroups");
            foreach (var talkGroup in deltaToRemove)
            {
                if (!await BrandMeisterWeb.RemoveTalkGroupFromDevice(talkGroup, loadedContents.BrandMeisterApiKeyInfo))
                {
                    ConsoleExt.WriteLine($"Error while removing talkgroup {talkGroup.talkgroup} at {talkGroup.repeaterid}, slot {talkGroup.slot}. Stoping.", Severity.Error);
                    return;
                }
            }

            ConsoleExt.WriteLine($"Adding {deltaToAdd.Count} Talkgroups");
            foreach (var talkGroup in deltaToAdd)
            {
                if (!await BrandMeisterWeb.AddTalkGroupToDevice(talkGroup, loadedContents.BrandMeisterApiKeyInfo))
                {
                    ConsoleExt.WriteLine($"Error while adding talkgroup {talkGroup.talkgroup} at {talkGroup.repeaterid}, slot {talkGroup.slot}. Stoping.", Severity.Error);
                    return;
                }
            }
            ConsoleExt.WriteLine($"Dropping all calls on all slots.");
            foreach (var slot in slotsToDrop)
            {
                if (!await BrandMeisterWeb.DropCallRoute(bmDeviceSelected.id, slot, loadedContents.BrandMeisterApiKeyInfo))
                {
                    ConsoleExt.WriteLine($"Error while dropping call on slot {slot} at {bmDeviceSelected.id}. Stopping.", Severity.Error);
                    return;
                }
                if (!await BrandMeisterWeb.DropDynamicGroups(bmDeviceSelected.id, slot, loadedContents.BrandMeisterApiKeyInfo))
                {
                    ConsoleExt.WriteLine($"Error while dropping dynamic groups on slot {slot} at {bmDeviceSelected.id}. Stopping.", Severity.Error);
                    return;
                }
            }
            ConsoleExt.WriteLine(string.Empty);
            ConsoleExt.WriteLine("You may have to drop static links calling TG 4000 on your DMR-Radio.");
            ConsoleExt.WriteLine(string.Empty);
            ConsoleExt.WriteLine("All done. Have a nice day, 73");
        }
    }
}
