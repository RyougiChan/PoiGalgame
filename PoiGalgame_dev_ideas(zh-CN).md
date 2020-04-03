# PoiGalgame 框架开发思路

## 涉及到的技术

- 主要开发语言 `C#`
- 开发环境 `Unity`

## 使用的工具

- Unity 2018.2
- Microsoft Visual Studio 2017
- Microsoft Visual Studio Code

## 需实现的基本功能

- 可视化编辑的剧本框架
- 自定义一种可读性更高的脚本化剧本(类似 kirikiri)
- 脚本化剧本解析成剧本框架可读取的资源类型
- 剧本实时解析、预览和展现
- 脚本化剧本更新行为监控(待评估)

## 思路总结

### 可视化编辑的剧本框架

- 资源类型 `Unity Asset(*.asset)`
- 继承 `UnityEditor.Editor` 类，重写 `void OnInspectorGUI()` 方法
- 创建继承自 `ScriptableObject` 类的剧本模型类和表演操作类

```csharp
    /// <summary>
    /// Script model of a story | 剧本实体模型
    /// </summary>
    public class GalgameScript : ScriptableObject
    {
        public string ScriptName;                           // 剧本名称
        public string ChapterName;                          // 章节名称
        public string ChapterAbstract;                      // 章节简介
        public Sprite Bg;                                   // 初始背景图片
        public AudioClip Bgm;                               // 初始背景音乐
                                                            // 剧本幕集合
        public List<GalgameAction> GalgameScenes = new List<GalgameAction>();
    }


    /// <summary>
    /// A scene in a script | 剧本中的某一幕
    /// </summary>
    [Serializable]
    public class GalgameAction
    {
        public string Line;                     // Actor's lines | 台词
        public AudioClip Bgm;                   // Background music in current scene | 当前幕的背景音乐
        public AudioClip Voice;                 // Actor's voice | 台词语音
        public Sprite Background;               // Background in current scene | 当前幕的背景/环境
        public Actor Actor;                     // Actor's name | 演员名称
        public Animator ActorAnimator;          // Actor's animator: blink, speck, smile and etc. | 演员动作: 眨眼，说话，笑等等
        public List Selector;                   // Select scene | 玩家选择场景
        public string Input;                    // Input scene | 玩家输入场景
    }

    /// <summary>
    /// Actor list | 演员列表
    /// </summary>
    [Serializable]
    public enum Actor
    {
        MARIE,
        FRITZ,
        MR_STAHLBAUM,
        MRS_STAHLBAUM,
        HERR_DROSSELMEYER,
        DANCING_DOLL_HARLEQUIN_1,
        DANCING_DOLL_HARLEQUIN_2,
        NUTCRACKER,
        PRINCE_MOUSE,
        MICE,
        TOY_SOLDIERS,
        CANDIES,
        CHOCOLATES,
        CANDY_CANES,
        SUGAR_PLUM_FAIRY,
        DANIEL
    }

```

- 使用 `MenuItem` 属性创建快速菜单快捷键

### 脚本化剧本

- 标记
  - 基本标识
    - `[chs]`           章节开始
    - `[che]`           章节结束
    - `[br]`            换行
    - `[p]`             换段
    - `[cl]`            点击提示符
  - 元素标签
    - `[text]`          文本（无样式）
    - `[line]`          台词
    - `[bg]`            背景图片
    - `[fg]`            前景图片
    - `[bgm]`           背景音乐
    - `[video]`         视频
    - `[select]`        选择器
      - `[option]`      选择器子选项(仅能与`[select]`绑定使用)
    - `[input]`         输入框
    - `[adjuster]`      数值调整器
    - `[judge]`         裁决器(数值判断)
    - `[battle]`        战斗登记器
    - `[group]`         父容器
    - `[pair]`          键值对，主要用于调整游戏数值
  - 公共属性标识
    - ~~`[pos]`           位置~~
    - `[style]`         样式定义
    - `[tran]`          转换效果
    - ~~`[effect]`        特效（下雨，下雪等）~~
- 属性
  - 公共元素属性
    - `id`              元素唯一标识
    - `tag`             元素类标识
    - `name`            元素标识
    - `value`           元素值
    - `width`           元素宽度
    - `height`          元素高度
    - `top`             元素距离顶部距离
    - `left`            元素距离左部距离
    - `visible`         是否在视图中显示
    - `layer`           元素所在层 `f3 | f2 | f1 | g | b1 | b2 | b3`
    - `method`          元素调用方法名
    - `canskip`         是否可跳过该元素
    - `time`            持续时间
  - 文本类标签公共属性
    - `linespacing`     文本行距
    - `align`           对齐方式 `lt | lm | lb | mt | mm | mb | rt | rm | rb`
    - `fcolor`          文本颜色 `0x00000000-0x00ffffff`
    - `fsize`           文本字体大小
    - `fstyle`          文本风格 `normal | bold | italic`
    - `ffamily`         字体类型 `Arial` 等
  - 图片类标签公共属性
    - `src`             图片路径
  - 音频/视频类标签公共属性
    - `src`             音频/视频路径
    - `volume`          音量
    - `loop`            循环
    - `action`          操作 `pause | resume | stop | play`
  - 台词专用属性
    - `actor`           演员名称
    - `voice`           台词语音
    - `line`            台词文本
    - `anim`            演员动作
  - 选择器 `[select]` 属性
    - `type`            选择器类型 `horizontal | vertical`
    - ~~`text`            子选择项文本 `[A,B,C,...]`(可选)~~
    - ~~`bg`              子选择项背景图片 `[A,B,C,...]`(可选)~~
    - ~~`bgm`             子选择项音频 `[A,B,C,...]`(可选)~~
  - 选择器子选项 `[option]` 属性
    - `text`            子选择项文本
    - `bg`              子选择项背景图片
    - `bgm`             子选择项音频
