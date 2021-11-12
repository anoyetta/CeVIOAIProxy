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

![image](https://user-images.githubusercontent.com/8295826/131949148-e01802aa-c2c9-4c69-a818-8a3298a9da18.png)

3. CastCraft などの棒読みちゃんを呼び出すアプリケーションを起動する
4. CastCraft などで文章の読上げを発生させる
5. CeVIO AI が文章を読上げる

CeVIOAIProxy は棒読みちゃんの代わりとなります。したがって、棒読みちゃんを起動する必要はありません。棒読みちゃんを同時に起動すると、ポートの競合などでエラーとなりますのでご注意ください。

## Tubeyomi との連携方法
### 大まかな流れ
1. Tubeyomi にコメントファイルを出力させる
2. Tubeyomi に出力させたコメントファイルを CeVIOAIProxy に監視させる

このような仕組みで Tubeyomi（Twitch）からのコメントを読み上げさせます。

### やり方
1. [オプション] -> [読み上げ] -> [テキストファイルに出力] のチェックをONにする

![image](https://user-images.githubusercontent.com/8295826/141482130-7f547ae3-bad6-43fe-b46a-b96981cee573.png)

2. [オプション] -> [Twitch] -> [チャットを読上げる] の**チェックをOFF**にする  
※ このチェックが入っている場合、Tubeyomi はなぜかコメントファイルを出力しないため。

![image](https://user-images.githubusercontent.com/8295826/141482480-b1753a90-f1c3-4a6c-995a-9db86c1bb1d9.png)

3. CeVIOAIProxy の [テキストファイルを監視して読上げる] のチェックをONにして1.で設定したコメントファイルを指定する

![image](https://user-images.githubusercontent.com/8295826/141483576-1e4bb77e-6a89-4cb2-b2f2-71b15a724a93.png)

これで Twitch にコメントが投稿されると Tubeyomi -> CeVIOAIProxy -> CeVIO AI と連携されてコメントが読み上げられます。
