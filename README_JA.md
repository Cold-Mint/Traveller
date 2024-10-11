[![Star History Chart](https://api.star-history.com/svg?repos=Cold-Mint/Traveller&type=Date)](https://star-history.com/#Cold-Mint/Traveller&Date)

[English](README.md) [简体中文](README_ZH.md) にほんご

## こくじ

ミントの新作ゲームです。

マルチピクセルのRogueliteゲームです

## 最近の研究開発の進捗状況です

| ミッション                                              | じょうたい   |
| ----------------------------------------------------------- | ------------------ |
| マップをランダムに生成します           | 成し遂げる |
| 戦利品                                              | 成し遂げる |
| バックパックのシステムをサポートしています | 成し遂げる      |
| 生物にAIエージェントを追加します | 進行中です |
## スクリーンショットです

ゲームのシーンです

![](screenshot/0.0.1/game_page.png)

ステージエディター

![](screenshot/0.0.1/level_Graph_Editor.png)
## 地元でプロジェクトを進めています

#### ダウンロードエンジンです
1. ダウンロード[Godot Engine .Net](https://godotengine.org/)。

   エンジンをダウンロードした後、エクスポートテンプレートを追加でダウンロードして実行可能なプログラムにする必要があります。

2. ダウンロード [.NetSDK](https://dotnet.microsoft.com/download).

   UbuntuやLinux Mintに。net8.0Sdkをインストールします。

```
apt install dotnet-sdk-8.0
```


#### クローンプロジェクトです

作業リストに次の指示を入力します。

```
git clone https://github.com/Cold-Mint/Traveller.git
```

#### エクスポート

エクスポートプリセット>リソース>エクスポート非リソースファイルまたはフォルダ編集ボックスに記入する必要があります:

```
data/*
```

#### 風習特徴

- **disableVersionIsolation** 版孤立を無効化する。
- **enableMod**実験的な機能、modが有効になっている場合、ゲームはmodディレクトリにdllファイルとpckファイルをロードします。assemblyloadcontextが分離されているため、当面の間、mod内からメインゲームのコンテンツにアクセスすることはできません。

#### Linux上でコンソールを動かします

ゲームの存在するディレクトリに以下のコマンドを入力します:

```
./Traveler.sh
```
## 翻訳に携わります

このプロジェクトは、当初からローカライズの準備ができていました。localsディレクトリのcsvファイルを編集することができます。新しい翻訳を加えたり修正したりしています

## 許可証です

[GPL-3.0 license](LICENSE)

プロトコルの日本語訳を見ます：[GPL-3.0 license にほんご](LICENSE_JA)

商用に対応しており、誰でも修正、構築、販売、無料配布が可能です。このプロジェクトのすべての派生バージョンについて、GPLプロトコルに基づいて、あなたは**作者の著作権**を保持し、**ソースコードの修正を公開します**。

> 注意:修正版を販売する権利はありますが、オリジナル版は販売できません。

## 貢献者です

<a href="https://github.com/Cold-Mint/Traveller/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=Cold-Mint/Traveller" />
</a>