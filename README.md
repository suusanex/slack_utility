# slack_utility

SlackのWebAPIを使って処理をするツール。

現状のメイン機能は、特定サイズ以上or作成日時が特定日以前のファイルを削除する処理。

無料プランで容量を使い切りそうになった場合などに便利。（その用途で実用中）

Web.configの次の値を環境に合わせて書き換えて使用する。

    <add key="ClientId" value="Your ClientId" />
    <add key="ClientSecret" value="Your ClientSecret" />
    <add key="TeamName" value="Your Team" />