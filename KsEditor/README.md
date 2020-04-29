# KsEditor

## Overview

![Preview](./images/preview/preview-main.png)

**KsEditor** is a JavaScript-based visual editor for [KsScript language](#). All you need is drag/create components to let JavaScript generate code autoly.

## Requirements

### Browser Requirement

This editor developed using a quantity of new feature of **ES6+**(ECMAScript 6.0 and higher version).
So we highly recommend using lastest [Google Chrome](https://chrome.google.com) browser.
Otherwise, We are not responsible for anything strange that happens `（＞人＜；）`.

### Priority Requirement

You must allow your browser to download multiple files at the same time when using the **Export** feature.

### Monitor Resolution Requirement

This editor developed base on monitor with resolution `1920*1080` and `2560*1440`. Therefore it's better to use the  two resolutions for best experience.

## Components Introduction

<style>
table th:nth-of-type(2) { width: 160pt }
</style>

| Component | Introduction | Ks Support | Example |
| --------- | ------------ | :--------: | ------- |
| Common Action | A action contains most of widgets, including `Line`, `BGM`, `BG`, `FG`, `Adjuster` | YES | ![common action](./images/preview/comp-action-common.png) |
| Line Action | A action to configure lines of character. Properties `font color`, `font size`, `line spacing` can be configured here | YES | ![Line action](./images/preview/comp-action-line.png) |
| BG/FG Action | A action to configure background image and forground image(Picture of character in most case) | YES | ![Bg/Fg action](./images/preview/comp-action-bg_fg.png) |
| BGM Action | A action to configure background music | YES | ![BGM action](./images/preview/comp-action-bgm.png) |
| Video Action | A action to configure and play video | NO | ![Video action](./images/preview/comp-action-video.png) |
| Selector Action | Configuring a select action to provide several choices for player. In every select item, you can configure `BG`, `BGM`. Also, you can provide special `Line` contents for this select | YES | ![Selector action](./images/preview/comp-action-selector.png) |
| Judge Action | A action to trigger events specified by `Event ID` according to judge conditions set(can be multiple) | YES | ![Judge action](./images/preview/comp-action-judge.png) |
| Adjuster Action | Manually adjust some value via this action | YES | ![Adjuster action](./images/preview/comp-action-adjuster_1.png) ![Adjuster action](./images/preview/comp-action-adjuster_2.png) |

## Generated Code Example

The following is a example of generated code

```r
[chs]
    [action id="1" nextActionId="2"]
        [bg src="test" layer="Background1"]
        [fg src="test" layer="Foreground1"]
        [bgm src="test" volume="90" loop action="play"]
        [adjuster id="adjuster-1"]
            [pair name="sorrow" value="27"]
            [pair name="angry" value="50"]
            [pair name="hate" value="50"]
        [adjuster]
        [line actor="女の子" line="この町　好きですか？" voice="test" fsize="15" linespacing="15" fcolor="0x6079d2ff" fstyle="italic"] 
        [line actor="朋也" line="（？）" fsize="15" linespacing="15" fcolor="0x6079d2ff" fstyle="italic"] 
        [line actor="女の子" line="私は好きです。" voice="test" fsize="15" linespacing="15" fcolor="0x6079d2ff" fstyle="italic"] 
    [action]
    [action id="2" nextActionId="3"]
        [line actor="朋也" line="...." fcolor="0x7f3d65ff"] 
    [action]
    [action id="3" nextActionId="5"]
        [bg src="test" layer="Background1"]
        [fg src="test" layer="Foreground1"]
    [action]
    [action id="5" nextActionId="6"]
        [bgm src="test" volume="58" loop action="play"]
    [action]
    [action id="6" nextActionId="7"]
        [video src="test" volume="90"  action="play"]
    [action]
    [action id="7" nextActionId="8"]
        [select type="horizontal"]
            [option text="糸" bg="test"]
            [option]
            [option text="合" bg="test"]
                [line actor="女の子" line="10月" ] 
            [option]
        [select]
    [action]
    [action id="8" nextActionId="9"]
        [judge id="judge-1" events="10000,10001"]
            [group id="group-1"]
                [pair name="HP" value="12"]
            [group]
            [group id="group-2"]
                [pair name="MP" value="15"]
            [group]
        [judge]
    [action]
    [action id="9"]
        [adjuster id="adjuster-2"]
            [pair name="sorrow" value="27"]
            [pair name="angry" value="70"]
            [pair name="hate" value="75"]
        [adjuster]
    [action]
[che]

```

## Copyright

Copyright © [RyougiChan](https://github.com/RyougiChan). All rights reserved.
