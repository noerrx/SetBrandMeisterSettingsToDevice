using Newtonsoft.Json;
using SetBrandMeisterSettingsToDevice.DataElements;
using SetBrandMeisterSettingsToDevice.HelperFunctions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SetBrandMeisterSettingsToDevice.WebFunctions
{
    internal class BrandMeisterWeb
    {

        internal static string brandMeisterUrl = "https://api.brandmeister.network/v2";

        public static async Task<bool> IsAPIKeyIsValid(BrandMeisterApiKeyInfo brandMeisterApiKeyInfo)
        {
            ConsoleExt.Write($"Validating API-Key... ");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", brandMeisterApiKeyInfo.apiKey);
                var response = await client.PostAsync($"{brandMeisterUrl}/user/whoAmI", null);
                if (response.IsSuccessStatusCode)
                {
                    ConsoleExt.WriteLine("Done", Severity.Info, ConsoleColor.Green);
                    return true;
                }
                ConsoleExt.WriteLine($"Failed! {response.StatusCode}", Severity.Error, ConsoleColor.Red);
                ConsoleExt.WriteLine(response.ReasonPhrase);
            }
            return false;
        }

        public static async Task<List<TalkGroupOfDevice>> GetTalkGroupsToDeviceID(int id)
        {
            ConsoleExt.Write($"Getting TalkGroups Of DeviceID {id}... ");
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{brandMeisterUrl}/device/{id}/talkgroup");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    ConsoleExt.WriteLine("Done", Severity.Info, ConsoleColor.Green);
                    return JsonConvert.DeserializeObject<List<TalkGroupOfDevice>>(content);
                }
                ConsoleExt.WriteLine($"Failed! {response.StatusCode}", Severity.Error, ConsoleColor.Red);
                ConsoleExt.WriteLine(response.ReasonPhrase);
            }
            return null;
        }
        public static async Task<BmDevice> GetDeviceToDeviceID(int id)
        {
            ConsoleExt.Write($"Getting Device-Infos Of DeviceID {id}... ");
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"{brandMeisterUrl}/device/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    ConsoleExt.WriteLine("Done", Severity.Info, ConsoleColor.Green);
                    return JsonConvert.DeserializeObject<BmDevice>(content);
                }
                ConsoleExt.WriteLine($"Failed! {response.StatusCode}", Severity.Error, ConsoleColor.Red);
                ConsoleExt.WriteLine(response.ReasonPhrase);
            }

            return null;
        }

        public static async Task<bool> AddTalkGroupToDevice(TalkGroupOfDevice talkGroup, BrandMeisterApiKeyInfo brandMeisterApiKeyInfo)
        {
            ConsoleExt.Write($"Removing Talkgroup {talkGroup.talkgroup} at Slot {talkGroup.slot} from DeviceID {talkGroup.repeaterid}... ");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", brandMeisterApiKeyInfo.apiKey);
                BmSlotGroupPair pair = new BmSlotGroupPair() { slot = talkGroup.slot, group = talkGroup.talkgroup };

                var myContent = JsonConvert.SerializeObject(pair);

                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync($"{brandMeisterUrl}/device/{talkGroup.repeaterid}/talkgroup", byteContent);
                if (response.IsSuccessStatusCode)
                {
                    ConsoleExt.WriteLine("Done", Severity.Info, ConsoleColor.Green);
                    return true;
                }
                ConsoleExt.WriteLine($"Failed! {response.StatusCode}", Severity.Error, ConsoleColor.Red);
                ConsoleExt.WriteLine(response.ReasonPhrase);
            }
            return false;
        }

        public static async Task<bool> RemoveTalkGroupFromDevice(TalkGroupOfDevice talkGroup, BrandMeisterApiKeyInfo brandMeisterApiKeyInfo)
        {
            ConsoleExt.Write($"Adding Talkgroup {talkGroup.talkgroup} at Slot {talkGroup.slot} to DeviceID {talkGroup.repeaterid}... ");
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", brandMeisterApiKeyInfo.apiKey);
                var response = await client.DeleteAsync($"{brandMeisterUrl}/device/{talkGroup.repeaterid}/talkgroup/{talkGroup.slot}/{talkGroup.talkgroup}");
                if (response.IsSuccessStatusCode)
                {
                    ConsoleExt.WriteLine("Done", Severity.Info, ConsoleColor.Green);
                    return true;
                }
                ConsoleExt.WriteLine($"Failed! {response.StatusCode}", Severity.Error, ConsoleColor.Red);
                ConsoleExt.WriteLine(response.ReasonPhrase);
            }
            return false;
        }
    }
}