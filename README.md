# Pumkin's VRChat SDK Patches
This repository includes a bunch of my VRChat SDK Harmony patches.

> [!NOTE]
> To install, if using VCC, find my listing [here](https://rurre.github.io/vpm), and info how to use it [here](https://github.com/rurre/vpm#readme).  
> Alternatively, download the latest unity package [here](https://github.com/rurre/vrc-sdk-patches/releases/latest) and import it into your project as normal.

<img alt="Patch settings window" src="https://github.com/user-attachments/assets/e3edb07d-8db4-49bc-ae0d-49ec05e7d050" />  

> [!NOTE]  
> Patches need to be enabled manually before they will do anything. You can access the settings in the menu bar at the top of Unity at `Tools` > `Pumkin` > `VRC SDK Patches`.

### Anonymize Avatar Name
This patch automatically anonymizes the avatar thumbnail name, which is where apps such as VRCX grab the avatar name from. This happens when you **update the thumbnail through the SDK** (which includes uploading a new avatar).

Settings for this include a list of names you can add for the patch to pick from at random. If no names are added, the avatar will be named `Avatar`.

> [!WARNING]  
> Please note that if someone is in the same instance as you, they will be able to see your real avatar name, both in VRCX and VRChat!

### Auto Accept VRC SDK Copyright Agreement
This patch automatically accepts the SDK agreement dialog when uploading an avatar (or world? haven't tested) after you agree to let it do so on your behalf.  
<img width="389" height="236" alt="Unity_2026-06-10_17-46-00" src="https://github.com/user-attachments/assets/528c8ba8-e073-4295-9c63-2c0207bb8034" />

<img alt="VRChat SDK's content upload copyright agreement dialog" src="https://github.com/user-attachments/assets/9f93ea1e-7bc9-4a8a-870a-47441fbc3fc2" />

> [!WARNING]
> Care has been taken to not modify how anything works on VRC's end but still, use at your own discretion and risk.  
