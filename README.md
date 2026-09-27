# UnityLogFinder

A small portable Windows application for finding Unity game logs.

## Run

Download `UnityLogFinder.exe` from Releases and double-click it. No installer or administrator access is required. Requires Windows 10/11 with .NET Framework 4.5 or later (included in these Windows versions). The executable is not code-signed.

The developer filter is enabled by default and includes both **Alexey Roenko** and **Roenko Games**. Uncheck it to see logs from other developers. Search matches game, developer and log filename.

Select a row to open the log, reveal it in Explorer, copy its path, or save a copy for support. Double-click opens a log. Click Refresh after running a game.

## Where it looks

`%USERPROFILE%\AppData\LocalLow\CompanyName\ProductName\`

Both `Player.log` and `Player-prev.log` appear separately, newest first. Games with only a previous log are included. Developer matching is exact and case-insensitive. Unreadable folders are skipped; directory junctions and symbolic links are not followed.

Only existing logs in the current Windows profile's standard location are shown. Custom `-logFile` locations, other user profiles, Editor logs and crash dumps are outside this utility's scope. A game without a log will not appear. Close a running game before exporting if you need a completed log; an active log may still be changing.

The application does not upload logs, modify the originals, or collect telemetry. Export only logs you intend to share; logs may contain personal information.

## Build and test

On Windows, from this directory:

```powershell
./build.ps1
./test.ps1
```

The build uses the .NET Framework C# compiler included with Windows. No NuGet packages, Unity installation, or .NET SDK are needed. Output: `dist/UnityLogFinder.exe`.

## Українською

Запустіть `UnityLogFinder.exe`. Галочка **Only Alexey Roenko and Roenko Games** показує логи ігор обох розробників. Зніміть її, щоб бачити інших розробників. **Open log** відкриває файл, **Open folder** показує його в Провіднику, **Copy log path** копіює шлях, **Save log as...** зберігає копію. **Refresh** оновлює список. Програма нічого не надсилає автоматично.
