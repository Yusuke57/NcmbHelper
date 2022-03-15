# NcmbHelper
## 概要
NCMBをUnityで簡単に使えるようにした

1. NCMB上のデータクラスと同じものをUnity(C#)で作成
2. 適当なオブジェクトにNcmbInitializer.csをアタッチし、NCMBのapplicationKeyとclientKeyを入力

するだけで、以下のように簡単にNCMBでのセーブ/ロードが書ける

```cs
// NCMBからロード
NcmbHelper.Load<MyData>(list =>
{
    foreach (var data in list)
    {
        Debug.Log($"ロードした内容: {data.score}");
    }
});

// NCMBにセーブ
var data = new MyData{ score = 100 };
NcmbHelper.Save(data);
```

## 環境
- Unity 2021.2.7f1
- NCMB 4.4.1

## 導入方法
1. [NCMBの公式ドキュメント](https://mbaas.nifcloud.com/doc/current/introduction/quickstart_unity.html)を参考にNCMBをセットアップする
2. [NcmbHelperのreleases](https://github.com/Yusuke57/NcmbHelper/releases)からNcmbHelperのunitypackageをDL&インポートする
3. NCMB上のデータクラスと同じクラス名&同じフィールド名のクラスをUnity(C#)で作成する
4. NCMBを使うシーンの適当なオブジェクトにNcmbInitializer.csをアタッチし、NCMBのapplicationKeyとclientKeyを入力する


## データクラスの作成

### 簡単な方法
`NcmbDataClassBase` を継承したデータクラスを作成する

```cs
public class MyData : NcmbDataClassBase<MyData>
{
    public int score;
}
```

リフレクションを使ってフィールド名を取得しているため、  
処理速度や設計が気になる方は次の「リフレクションを利用しない方法」がおすすめ

### リフレクションを利用しない方法
`INcmbObjectConverter` インターフェースを実装したデータクラスを作成する

```cs
public class MyData : INcmbObjectConverter
{
    public int score;
    
    // データクラスからNCMBObjectを生成して返す
    public NCMBObject ExportToNcmbObject()
    {
        var obj = new NCMBObject(typeof(MyData).Name);
        obj["score"] = score.ToString();
    }
    
    // NCMBObjectからデータクラスに値を入れる
    public void ImportToDataClass(NCMBObject obj)
    {
        obj.score = int.Parse(obj["score"].ToString());
    }
}
```


## セーブ/ロード処理

### 全てのデータをロードしたい
```cs
NcmbHelper.Load<MyData>(list =>
{
    if(list == null)
    {
        Debug.LogError("取得失敗");
        return;
    }
    
    foreach (var data in list)
    {
        Debug.Log($"ロードした内容: {data.score}");
    }
});
```

### 条件を絞ってデータをロードしたい
```cs
// クエリを作成
var query = NcmbHelper.CreateQuery<MyData>();
query.OrderByDescending("score"); // 降順
query.Limit = 5; // 最大5件まで

NcmbHelper.Load<MyData>(query, list =>
{
    if(list == null)
    {
        Debug.LogError("取得失敗");
        return;
    }
    
    foreach (var data in list)
    {
        Debug.Log($"ロードした内容: {data.score}");
    }
});
```

### データをセーブしたい
```cs
var data = new MyData{ score = 100 };

// callbackが不要なとき
NcmbHelper.Save(data);

// callbackが必要なとき
NcmbHelper.Save(data, isSuccess => 
{
    if(isSuccess)
    {
        Debug.Log("セーブ完了");
    }
    else
    {
        Debug.Log("セーブ失敗");
    }
});
```

### 全てのデータの総数を取得したい
```cs
NcmbHelper.Count<MyData>(count =>
{
    if(count == -1)
    {
        Debug.LogError("取得失敗");
        return;
    }
    
    Debug.Log($"件数: {count}");
});
```

### 条件を絞ったデータの総数を取得したい
```cs
// クエリを作成
var query = NcmbHelper.CreateQuery<MyData>();
query.WhereEqualTo("score", 100); // スコアが100

NcmbHelper.Count<MyData>(query, count =>
{
    if(count == -1)
    {
        Debug.LogError("取得失敗");
        return;
    }
    
    Debug.Log($"件数: {count}");
});
```

### データを削除したい
```cs
MyData data;  // LoadしたMyDataインスタンスが入っている想定

// callbackが不要なとき
NcmbHelper.Delete(data);

// callbackが必要なとき
NcmbHelper.Delete(data, isSuccess =>
{
    if(isSuccess)
    {
        Debug.Log("削除完了");
    }
    else
    {
        Debug.Log("削除失敗");
    }
});
```

### ObjectIdをもとにデータを削除したい
```cs
var objectId = "xxxxxxxxx";

// callbackが不要なとき
NcmbHelper.Delete(objectId);

// callbackが必要なとき
NcmbHelper.Delete(objectId, isSuccess =>
{
    if(isSuccess)
    {
        Debug.Log("削除完了");
    }
    else
    {
        Debug.Log("削除失敗");
    }
});
```
