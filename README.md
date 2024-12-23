[![Star History Chart](https://api.star-history.com/svg?repos=Cold-Mint/Traveller&type=Date)](https://star-history.com/#Cold-Mint/Traveller&Date)

English [简体中文](README_ZH.md) [にほんご](README_JA.md)

## Intro

Mint's new game.

A pixel cross-platform roguelite game.

## Screenshot

Game scene

![](screenshot/0.0.1/game_page.png)

Level graph editor

![](screenshot/0.0.1/level_Graph_Editor.png)

## Run the project locally

#### Download engine

1. Download [Godot Engine .Net](https://godotengine.org/).

   After downloading the engine, you will need to download an additional export template to export as an executable
   program.

2. Download [.NetSDK](https://dotnet.microsoft.com/download).

   Ubuntu or Linux Mint install the .net 8.0 Sdk.

```
apt install dotnet-sdk-8.0
```

#### Clone project

Enter the following command in your working directory:

```
git clone https://github.com/Cold-Mint/Traveller.git
```

#### Export

You need to fill in the Export Presets > Resources > Filter to export non-resource files or folders edit box:

```
data/*
```

#### Custom feature

- **disableVersionIsolation** Disable version isolation.
- **enableMod** Experimental feature, the game loads dll files and pck files in the mod directory when the mod is
  enabled. Due to the isolation of AssemblyLoadContext, the main game content cannot be accessed from within the Mod for
  the time being.

#### Run the console on Linux

Enter the following command in the directory where the game is located:

```
./Traveler.sh
```

## Participate in translation

The project is prepared for localization at the beginning of writing. You can edit the csv file in the locals directory.
To modify and add new translations.

## License

[GPL-3.0 license](LICENSE)

Support commercial, anyone can modify, build, and sell or distribute for free. For all derivative versions of this
project, under the GPL, you shall  **retain the author copyright** and **publish the modified source code**.

> Note: You have the right to sell the modified version, but not the original.
>

## Contributor

<a href="https://github.com/Cold-Mint/Traveller/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=Cold-Mint/Traveller" />
</a>
