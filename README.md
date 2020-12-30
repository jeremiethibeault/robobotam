# Tobobotam
Set of Raspberry Pi applications.

## Hogwarts

### Running the program

- Publish by executing the following command `dotnet publish -c release -r linux-arm --no-self-contained`.

- Create a new folder on the Raspberry Pi `/home/pi/apps/hogwarts`.

- Deploy on the Raspberry Pi and delete the `appsettings.Local.json` file.

- The folder structure shoud look like this
  - `/home/pi/apps/hogwarts/Hogwarts`: Executable application
  - `/home/pi/apps/hogwarts/songs`: Songs to play

- Change the execution mode of startup script using `chmod +x startup.sh`.

- Run the program using `./startup.sh`.

### Configuring the autostart

- Add the file `hogwarts-autostart.desktop` to `~/.config/autostart`.

- Restart the Raspberry Pi.

- You should see a terminal running the program.