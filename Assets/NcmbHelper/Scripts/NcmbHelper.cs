using System;
using System.Collections.Generic;
using NCMB;
using UnityEngine;

namespace Yusuke57.CommonPackage
{
    public static class NcmbHelper
    {
        // ====================================================================
        // Load
        // ====================================================================

        /// <summary>
        /// 空クエリを取得
        /// </summary>
        public static NCMBQuery<NCMBObject> GetQuery<T>()
        {
            var dataClassName = typeof(T).Name;
            return new NCMBQuery<NCMBObject>(dataClassName);
        }

        /// <summary>
        /// 条件なしでロード
        /// </summary>
        public static void Load<T>(Action<List<T>> onComplete) where T : INcmbObjectConverter, new()
        {
            var query = GetQuery<T>();
            Load(query, onComplete);
        }

        /// <summary>
        /// クエリを指定してロード
        /// </summary>
        public static void Load<T>(NCMBQuery<NCMBObject> query, Action<List<T>> onComplete) where T : INcmbObjectConverter, new()
        {
            query.FindAsync((objList, e) =>
            {
                if (e != null)
                {
                    //検索失敗時の処理
                    onComplete?.Invoke(null);
                    return;
                }

                // 成功時の処理
                var dataList = new List<T>(objList.Count);
                foreach (var obj in objList)
                {
                    var data = new T();
                    data.ImportToDataClass(obj);
                    dataList.Add(data);
                }
                
                onComplete?.Invoke(dataList);
            });
        }
        

        // ====================================================================
        // Save
        // ====================================================================
        
        /// <summary>
        /// セーブ
        /// </summary>
        public static void Save<T>(T data, Action<bool> onComplete = null) where T : INcmbObjectConverter, new()
        {
            var obj = data.ExportToNcmbObject();

            obj.SaveAsync(e =>
            {
                if (e != null)
                {
                    //エラー処理
                    onComplete?.Invoke(false);
                    return;
                }
                
                //成功時の処理
                onComplete?.Invoke(true);
            });
        }
        
        
        // ====================================================================
        // Count
        // ====================================================================
        
        /// <summary>
        /// 条件なしでカウント
        /// </summary>
        public static void Count<T>(Action<int> onComplete) where T : INcmbObjectConverter, new()
        {
            var query = GetQuery<T>();
            Count<T>(query, onComplete);
        }
        
        /// <summary>
        /// クエリを指定してカウント
        /// </summary>
        public static void Count<T>(NCMBQuery<NCMBObject> query, Action<int> onComplete) where T : INcmbObjectConverter, new()
        {
            query.CountAsync((count, e) =>
            {
                if (e != null)
                {
                    //検索失敗時の処理
                    onComplete?.Invoke(-1);
                    return;
                }

                // 成功時の処理
                onComplete?.Invoke(count);
            });
        }
        
        
        // ====================================================================
        // Delete
        // ====================================================================

        /// <summary>
        /// データを削除（ObjectIdに一致するデータを削除）
        /// </summary>
        public static void Delete<T>(T data, Action<bool> onComplete = null) where T : INcmbObjectConverter, new()
        {
            var obj = data.ExportToNcmbObject();
            Delete<T>(obj.ObjectId, onComplete);
        }
        
        /// <summary>
        /// 指定のObjectIdのデータを削除
        /// </summary>
        public static void Delete<T>(string objectId, Action<bool> onComplete = null) where T : INcmbObjectConverter, new()
        {
            var dataClassName = typeof(T).Name;
            var obj = new NCMBObject(dataClassName)
            {
                ObjectId = objectId
            };

            obj.DeleteAsync(e =>
            {
                Debug.Log(e);
                if (e != null)
                {
                    //エラー処理
                    onComplete?.Invoke(false);
                    return;
                }
                
                //成功時の処理
                onComplete?.Invoke(true);
            });
        }
    }
}