[![Star History Chart](https://api.star-history.com/svg?repos=Cold-Mint/Traveller&type=Date)](https://star-history.com/#Cold-Mint/Traveller&Date)

[English](README.md) 简体中文 [にほんご](README_JA.md)

## 简介

薄荷的新作游戏。

一款像素的跨平台的Roguelite游戏。

## 近期研发进度

| 任务        | 状态  |
|-----------|-----|
| 随机生成地图    | 完成  |
| 战利品       | 完成  |
| 支持仍出的背包系统 | 完成  |
| 为生物添加AI代理 | 进行中 |

## 屏幕截图

游戏场景

![](screenshot/0.0.1/game_page.png)

关卡编辑器

![](screenshot/0.0.1/level_Graph_Editor.png)

## 在本地运行项目

#### 下载引擎

1. 下载[Godot Engine .Net](https://godotengine.org/)。

   下载引擎后，您需要额外下载导出模板才能导出为可执行程序。

2. 下载 [.NetSDK](https://dotnet.microsoft.com/download).

   Ubuntu或Linux Mint安装.net8.0Sdk。

```
apt install dotnet-sdk-8.0
```

#### 克隆项目

在您的工作目录输入以下指令：

```
git clone https://github.com/Cold-Mint/Traveller.git
```

#### 导出

需要在导出预设>资源>筛选导出非资源文件或文件夹编辑框内填入：

```
data/*
```

#### 自定义特性

- **disableVersionIsolation** 禁用版本隔离。
- **enableMod** 实验性功能，当启用模组时游戏会在mod目录加载dll文件和pck文件。由于AssemblyLoadContext的隔离性，暂时不能从Mod内访问主游戏内容。

#### 在Linux上运行控制台

在游戏所在目录输入以下命令：

```
./Traveler.sh
```

## 配置Openobserve

> 这是可选的操作，即使您不配置Openobserve，游戏也能正常运行。

openobserve用于在游戏发布后，持续收集日志和报警信息。

#### 搭建openobserve服务器

请见：[openobserve](https://github.com/openobserve/openobserve)

#### 编写配置

在您搭建完毕openobserve的服务器后，按如下步骤配置文件：

1. 在项目的根目录创建名为**AppConfig.yaml**的配置文件。

2. 填入远程服务器的信息。

   ```yaml
   open_observe:
     address: [address]
     access_token: [token]
     org_id: [org_id]
     stream_name: [stream_name]
   ```

   address 服务器的地址，格式为 http(s)://www.example.com。（支持http和https）

## 参与翻译

此项目在编写之初就为本地化做好了准备。您可以编辑locals目录下的csv文件。来修改和添加新的翻译。

## 许可证

[GPL-3.0 license](LICENSE)

查看协议的中文翻译版本：[GPL-3.0 license 简体中文](LICENSE_ZH)

支持商用，任何人可修改，构建，并用于售卖或免费发布。对于此项目的所有衍生版本，根据GPL协议，您应当**保留作者版权**，以及*
*公开修改后的源代码**。

> 注意：您有权售卖修改后的版本，但不能售卖原版。

## 贡献者

<a href="https://github.com/Cold-Mint/Traveller/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=Cold-Mint/Traveller" />
</a>