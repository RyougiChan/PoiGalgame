# PoiGalgame

## 描述

PoiGalgame 是一个基于 Unity 引擎的 Galgame/美少女游戏/文字冒险游戏脚本化开发框架。

当今主流使用的 Galgame 制作引擎有很多，比如 `KIRIKIRI`、`NScriptor`、`YU-RIS`、`Siglus&RealLive` 等。Unity 作为一个优秀的跨平台游戏开发引擎，用于开发跨平台 2D/3D 游戏可谓十分强大，使用 Unity 来构建 Galgame/文字冒险游戏绰绰有余，乍看之下还似乎有点大材小用。但是麻雀虽小五脏俱全，要使用 Unity 来构建一个和主流制作引擎相比不分上下的 Galgame，这其中有很多方面需要仔细琢磨。

**警告:** 本项目仍处于开发阶段！

## 支持

- 开发环境
  - Windows 10
- 开发工具
  - Unity 2020.1.0f1
  - Microsoft Visual Studio 2017
  - Microsoft Visual Studio Code

## 开发目标

- [x] 基本的 Galgame 要素
- [x] 可视化编辑的剧本框架
- [x] 自定义一种可读性更高的脚本化剧本(类似 kirikiri)
- [x] 自定义剧本可视化编辑器: KsEditor
- [ ] 脚本化剧本更新行为监控(待评估)
- [ ] 实时解析、预览和展现剧本内容
- [ ] 实现页面动画、转换、特效等
- [ ] 文档化

## 如何使用

**警告:** 本项目仍处于开发阶段！

1. 写 KS 脚本并转换为 Unity Asset
    1. 使用可视化编辑器 [KsEditor](KsEditor/README.md) 编写脚本.
    2. 在 [KsEditor](KsEditor/README.md) 中通过点击导出按钮导出 KS 脚本(`.ks` 后缀).
    3. 将步骤2导出的 KS 脚本放置在 Unity 的 `Assets/Resources/Chapter.ks`(推荐) 目录或其他目录中.
    4. 在 Unity 编辑器模式下打开视图 `Translator`，输入步骤3放置的文件的完整路径，点击按钮 **`スクリプト翻訳`**.
2. 在 [ChapterController](Assets/Script/Chapter/ChapterController.cs) 文件中加载剧本.
  查找下面这一行并替换路径 `Chapter/Chapter-01`

  ```cs
  currentScript = Resources.Load<GalgameScript>("Chapter/Chapter-01");
  ```

## 发布版本

**警告:** 本项目仍处于开发阶段！

- [Pre-release](https://github.com/RyougiChan/PoiGalgame/releases) 使用视图 `ChapterDisplay` 构建.

## 文档

- [ ] [剧本标注文档](#)
- [ ] [框架文档](#)
- [ ] [关键问题记录](#)

## 声明

本项目演示使用了来自游戏 CLANNAD 的少许资源，其版权归 [Key](http://key.visualarts.gr.jp) 公司所有。本项目所使用的其他资源基于[CC BY-NC-SA 4.0](https://creativecommons.org/licenses/by-nc-sa/4.0/)，这意味着你可以拷贝、并再发行本项目的内容，但是你将必须同样提供 **原作者信息**以及 **协议声明**。同时你也 **不能将本项目用于商业用途**，按照我们狭义的理解（增加附属条款），**凡是任何盈利的活动皆属于商业用途**。

本项目基于[Apache License 2.0](LICENSE)发布

版权所有 © [RyougiChan](https://github.com/RyougiChan)
