# Set-BrandMeister-Settings-To-Device
Configure several sets of Talkgroups and quickly switch between them.

Only recomended for hotspots.

This is a commandline application. ;-)
![grafik](https://github.com/noerrx/SetBrandMeisterSettingsToDevice/assets/42487986/e5704dcf-681f-41a2-a9b4-cb8374a39a93)

## Usage
Prerequisites:
* A valid Brandmeiser-Account.
* A Hotspot already set up and working as you like on the DMR-Brandmeister-Network.
* An API-Key from the Brandmeister-Network. Get yours from https://brandmeister.network/?page=profile-api.

### Installation

1. Download the latest release-zip from the [release-page](https://github.com/noerrx/SetBrandMeisterSettingsToDevice/releases).
2. Unzip the downloaded file.
3. Add the `BrandMeisterAPIKey.json`-File
3. Add the `DeviceSettings.json`-File

### BrandMeisterAPIKey.json
The file must have the following format:

```js
{
  "apiKey": "[API-Key-From-brandmeister.network]"
}
```
Just replace ```[API-Key-From-brandmeister.network]``` with the API-Key you created at https://brandmeister.network/?page=profile-api.

### DeviceSettings.json
The file must have the following format:

```js
{
  "devices": [
    {
      "deviceId": 0, // id of your device you can thee unter My Devices
      "settings": [
        {
          "name": "", // name of this setting
          "talkgroups": [ // array of talkgroups
            {
              "talkgroup": 0, // talkgroup-id
              "slot": 0 // slot of the talkgroup
            },
            {
              "talkgroup": 0, // talkgroup-id
              "slot": 0 // slot of the talkgroup
            }
          ]
        },
        {
          "name": "", // name of this setting
          "talkgroups": [ // array of talkgroups
            {
              "talkgroup": 0, // talkgroup-id
              "slot": 0 // slot of the talkgroup
            },
            {
              "talkgroup": 0, // talkgroup-id
              "slot": 0 // slot of the talkgroup
            }
          ]
        }
      ]
    }
    ,
    {
      "deviceId": 0, // id of your device you can thee unter My Devices
      "settings": [
        {
          "name": "", // name of this setting
          "talkgroups": [ // array of talkgroups
            {
              "talkgroup": 0, // talkgroup-id
              "slot": 0 // slot of the talkgroup
            },
            {
              "talkgroup": 0, // talkgroup-id
              "slot": 0 // slot of the talkgroup
            }
          ]
        },
        {
          "name": "", // name of this setting
          "talkgroups": [ // array of talkgroups
            {
              "talkgroup": 0, // talkgroup-id
              "slot": 0 // slot of the talkgroup
            },
            {
              "talkgroup": 0, // talkgroup-id
              "slot": 0 // slot of the talkgroup
            }
          ]
        }
      ]
    }
  ]
}
```

#### Config Options

The following properties can be configured:

|Option|Description|
|---|---|
|`deviceId`|The ID of your Hotspot|
|`name`|The name of one setting. You can choose a name as you like. It won't show up anywhere at brandmeister.network. |
|`talkgroup`|One Talkgroup-ID you can find at https://brandmeister.network/?page=talkgroups#|
|`slot`|The slot where the talkgroup should be placed at. Choose `0` when you can only see one slot on the device-selfcare-page. |

## Running the app
Start `SetBrandMeisterSettingsToDevice.exe` within a Windows commandline.

## When the app does not start properly
The following files must be in the same folder as `SetBrandMeisterSettingsToDevice.exe`:
* `BrandMeisterAPIKey.json`
* `DeviceSettings.json`
* `Newtonsoft.Json.dll`
* `Newtonsoft.Json.xml`
* `SetBrandMeisterSettingsToDevice.exe`


