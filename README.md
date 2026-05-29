This repository includes a bunch of VRC SDK Harmony patches. 

<img alt="Patch settings window" src="https://github.com/user-attachments/assets/e3edb07d-8db4-49bc-ae0d-49ec05e7d050" />  

> [!NOTE]  
> Patches need to be enabled manually before they will do anything. You can access the settings in the menu bar at the top of Unity at `Tools` > `Pumkin` > `VRC SDK Patches`.

### Anonymize Avatar Name
This patch automatically anonymizes the avatar thumbnail name, which is where apps such as VRCX grab the avatar name from. This happens when you **update the thumbnail through the SDK** (which includes uploading a new avatar).

Settings for this include a list of names you can add for the patch to pick from at random. If no names are added, the avatar will be named `Avatar`.

> [!WARNING]  
> Please note that if someone is in the same instance as you, they will be able to see your real avatar name, both in VRCX and VRChat!

### Auto Accept VRC SDK Copyright Agreement
This patch automatically accepts the SDK agreement dialog when uploading an avatar (or world? haven't tested).  

<img alt="VRChat SDK's content upload copyright agreement dialog" src="https://github.com/user-attachments/assets/9f93ea1e-7bc9-4a8a-870a-47441fbc3fc2" />
