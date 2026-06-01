## v1.1.1
### General
- Fixed settings not saving if scripts reloaded when settings window was open.

### Auto Accept Copyright Dialog
- Updated copyright agreement popup.
---

## v1.1.0
### Auto Accept Copyright Dialog
- Added a new patch to automatically agree to the VRC SDK copyright agreement when uploading an avatar (or world?, didn't try).
---

## v1.0.4
### Anonymize Avatar Name
- Fixed closing Unity not saving settings if the settings window was left open.
---

## v1.0.3
### Anonymize Avatar Name
- Made list of possible names reoderable.
---

## v1.0.2
### Anonymize Avatar Name
- Sanitize avatar names before saving them. This replaces all illegal path characters (from `Path.GetIllegalPathCharacters()`) with underscores.
---

## v1.0.1
### General
- Fixed VRC SDK dependency being outdated.
---

## v1.0.0
### Anonymize Avatar Name
- Added Harmony patch to anonymize avatar names by renaming avatar thumbnails, which is where VRCX grabs the avatar name from.