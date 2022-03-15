namespace Yusuke57.CommonPackage
{
    /// <summary>
    /// サンプルデータクラス
    /// NCMB上で同じ名前のクラス & フィールドを作成する必要がある
    /// </summary>
    public class PlayerScoreData : NcmbDataClassBase<PlayerScoreData>
    {
        public string playerName;
        public int score;
    }
}