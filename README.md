# CeVIOAIProxy
CeVIO AI に棒読みちゃんと同等のTCPソケットインターフェースを生やすアプリケーションです。CastCraft などの棒読みちゃんを呼び出して読上げさせるソフトウェアから CeVIO AI を呼び出せるようになります。

## どういうこと？
CastCraft → 棒読みちゃん → CeVIO AI  
という流れで配信時のチャットを読上げさせようとします。しかし、棒読みちゃんは32bitアプリケーションであるため、64bitアプリケーションである CeVIO AI を呼び出すことが出来ません。

このとき…  
CastCraft → CeVIOAIProxy → CeVIO AI  
という流れで、このアプリケーションが棒読みちゃんのふりをして、CeVIO AI までの間を連携します。

**要するに CastCraft などで CeVIO AI（小春六花など）が使えるようになります。**

# ダウンロード
*[こちら](https://github.com/anoyetta/CeVIOAIProxy/releases)* からダウンロードしてください。

# 使い方
1. CeVIOAIProxy.exe を起動する
同時に CeVIO AI が起動します。
2. CeVIOAIProxy の設定画面でキャストや各種読上げパラメータを設定する

![image](https://user-images.githubusercontent.com/8295826/117169466-15932880-ae04-11eb-85e0-edc1b05196e0.png)

3. CastCraft などの棒読みちゃんを呼び出すアプリケーションを起動する
4. CastCraft などで文章の読上げを発生させる
5. CeVIO AI が文章を読上げる

CeVIOAIProxy は棒読みちゃんの代わりとなります。したがって、棒読みちゃんを起動する必要はありません。
