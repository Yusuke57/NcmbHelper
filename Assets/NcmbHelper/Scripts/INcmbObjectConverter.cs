using NCMB;

namespace Yusuke57.CommonPackage
{
    /// <summary>
    /// NCMBで扱うデータクラスとNCMBObjectを相互変換するインターフェース
    /// NcmbDataClassBaseを使わない場合はこれをデータクラスに実装する
    /// </summary>
    public interface INcmbObjectConverter
    {
        /// <summary>
        /// データクラスからNCMBObjectを生成して返す
        /// </summary>
        public NCMBObject ExportToNcmbObject();
        
        /// <summary>
        /// NCMBObjectからデータクラスに値を入れる
        /// </summary>
        public void ImportToDataClass(NCMBObject obj);
    }
}