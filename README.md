# PoiGalgame

## Overview

PoiGalgame is scripted framework to build galgames or adventure games based on Unity.

There are some famous galgame development engine such as `KIRIKIRI`, `NScriptor`, `YU-RIS`, `Siglus&RealLive` and etc. It seems that using Unity engine to galgame developement just like employing a steam engine to crack a nut. But small as the sparrow is, it possesses all its internal organs. To build a excellent galgame with Unity, a great quantity of work should be done.

**WARNING:** This project is still under development!

## Support

- Dev Eviroment
  - Windows 10
- Dev Tools
  - Unity 2020.1.0f1
  - Microsoft Visual Studio 2017
  - Microsoft Visual Studio Code

## Development Goals

- [x] Basic Galgame elements
- [x] Visual editing script framework
- [x] Create a more readable script(like kirikiri)
- [x] Provide a custom script editor tool: [KsEditor](KsEditor/README.md)
- [ ] Script update behavior monitoring (to be evaluated)
- [ ] Real-time parsing, previewing and presentation
- [ ] Tranform, animation, effect, etc.
- [ ] Full documentation

## Getting Start

**WARNING:** This project is still under development!

1. Write your own script and translate to Unity asset.
    1. Write your story using [KsEditor](KsEditor/README.md).
    2. Export KS script(suffix with `.ks`) from [KsEditor](KsEditor/README.md) by clicking EXPORT button.
    3. Place the exported KS script in the unity folder `Assets/Resources/Chapter.ks`(recommended) or other folder.
    4. Open Scene `Translator` in unity editor mode and input full path of KS script file in step3, then click button: **`スクリプト翻訳`**.
2. Load script in [ChapterController](Assets/Script/Chapter/ChapterController.cs).
  find the following line and change the path `Chapter/Chapter-01`

  ```cs
  currentScript = Resources.Load<GalgameScript>("Chapter/Chapter-01");
  ```

## Releases

**WARNING:** This project is still under development!

- [Pre-release](https://github.com/RyougiChan/PoiGalgame/releases) built using Scene `ChapterDisplay`.

## Documentation

- [ ] [Script Mask Documentation](#)
- [ ] [Framework Documentation](#)
- [ ] [Key Issues Record](#)

## Declaration

Some resources using in the demo are from game CLANNAD, which copyright to [Key](http://key.visualarts.gr.jp) company. All other resources in this project are based on [CC BY-NC-SA 4.0](https://creativecommons.org/licenses/by-nc-sa/4.0/), that means you can copy and reissue the contents of this project, but you will also have to provide the **original author information** as well as the **agreement statement**. At the same time, it **cannot be used for commercial purposes**. In accordance with our narrow understanding (Additional subsidiary terms), **all activities that are profitable are of commercial use.**

Released under the [Apache License 2.0](LICENSE)

Copyright © [RyougiChan](https://github.com/RyougiChan)
