# Troubleshoot

## Mouse pointer lags
- Execute `sudo nano /boot/cmdline.txt`.
- Add at the end of the line `usbhid.mousepoll=0`.
- Save and restart.
