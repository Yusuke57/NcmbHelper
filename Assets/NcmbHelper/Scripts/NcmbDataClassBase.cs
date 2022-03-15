using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NCMB;
using UnityEngine;

namespace Yusuke57.CommonPackage
{
    /// <summary>
    /// NCMBで扱うデータクラスの基底クラス
    /// リフレクションでNCMBObjectとデータクラスを相互に自動変換する
    /// </summary>
    public class NcmbDataClassBase<T> : INcmbObjectConverter
    {
        /// <summary>
        /// NCMB側で設定される値
        /// </summary>
        public string ObjectId { get; private set; }= null;
        public DateTime? CreateDate { get; private set; }
        public DateTime? UpdateDate { get; private set; }
        
        /// <summary>
        /// データクラスからNCMBObjectを生成して返す
        /// </summary>
        public NCMBObject ExportToNcmbObject()
        {
            var type = typeof(T);
            var typeName = type.Name;
            var obj = new NCMBObject(typeName);

            // objectIdを持っていたら指定する（NCMB上で上書きする）
            if (!string.IsNullOrEmpty(ObjectId))
            {
                obj.ObjectId = ObjectId;
            }

            var fields = type.GetFields();
            
            foreach (var field in fields)
            {
                obj[field.Name] = field.GetValue(this);
            }

            return obj;
        }

        /// <summary>
        /// NCMBObjectからデータクラスに値を入れる
        /// </summary>
        public void ImportToDataClass(NCMBObject obj)
        {
            var type = typeof(T);
            var fields = type.GetFields();

            ObjectId = obj.ObjectId;
            CreateDate = obj.CreateDate;
            UpdateDate = obj.UpdateDate;
            
            foreach (var field in fields)
            {
                if (!obj.ContainsKey(field.Name))
                {
                    continue;
                }

                SetFieldValue(field, obj[field.Name]);
            }
        }

        /// <summary>
        /// フィールドに値を入れる
        /// </summary>
        private void SetFieldValue(FieldInfo fieldInfo, object value)
        {
            var fieldType = fieldInfo.FieldType;
            
            // 配列の場合
            if (fieldType.IsArray)
            {
                SetFieldArrayValue(fieldInfo, value);
                return;
            }
            
            // Listの場合
            if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                SetFieldListValue(fieldInfo, value);
                return;
            }
            
            // その他の場合
            SetFieldSimpleValue(fieldInfo, value);
        }

        /// <summary>
        /// 配列でもListでもないフィールドに値を入れる
        /// </summary>
        private void SetFieldSimpleValue(FieldInfo fieldInfo, object value)
        {
            var fieldType = fieldInfo.FieldType;

            switch (fieldType.Name)
            {
                case "Int32":
                    fieldInfo.SetValue(this, int.Parse(value.ToString()));
                    return;
                case "Single":
                    fieldInfo.SetValue(this, float.Parse(value.ToString()));
                    return;
                case "Boolean":
                    fieldInfo.SetValue(this, bool.Parse(value.ToString()));
                    return;
                case "String":
                    fieldInfo.SetValue(this, value.ToString());
                    return;
                case "DateTime":
                    fieldInfo.SetValue(this, DateTime.Parse(value.ToString()));
                    return;
                default:
                    fieldInfo.SetValue(this, value);
                    return;
            }
        }
        
        /// <summary>
        /// 配列のフィールドに値を入れる
        /// </summary>
        private void SetFieldArrayValue(FieldInfo fieldInfo, object value)
        {
            if (value is not IEnumerable enumerable)
            {
                Debug.LogError("[NcmbData]Parseできない配列です");
                return;
            }

            var fieldType = fieldInfo.FieldType;
            var elementType = fieldType.GetElementType();
            var elementTypeName = elementType?.Name ?? string.Empty;

            var array = enumerable as object[] ?? enumerable.Cast<object>().ToArray();
            switch (elementTypeName)
            { 
                case "Int32":
                    fieldInfo.SetValue(this, array.Select(e => int.Parse(e.ToString())).ToArray());
                    break;
                case "Single":
                    fieldInfo.SetValue(this, array.Select(e => float.Parse(e.ToString())).ToArray());
                    break;
                case "Boolean":
                    fieldInfo.SetValue(this, array.Select(e => bool.Parse(e.ToString())).ToArray());
                    break;
                case "String":
                    fieldInfo.SetValue(this, array.Select(e => e.ToString()).ToArray());
                    break;
                case "DateTime":
                    fieldInfo.SetValue(this, array.Select(e => DateTime.Parse(e.ToString())).ToArray());
                    break;
                default:
                    fieldInfo.SetValue(this, array);
                    break;
            }
        }
        
        /// <summary>
        /// Listのフィールドに値を入れる
        /// </summary>
        private void SetFieldListValue(FieldInfo fieldInfo, object value)
        {
            if (value is not IEnumerable enumerable)
            {
                Debug.LogError("[NcmbData]ParseできないListです");
                return;
            }

            var fieldType = fieldInfo.FieldType;
            var elementType = fieldType.GenericTypeArguments[0];
            var elementTypeName = elementType?.Name ?? string.Empty;

            var list = enumerable as List<object> ?? enumerable.Cast<object>().ToList();
            switch (elementTypeName)
            { 
                case "Int32":
                    fieldInfo.SetValue(this, list.Select(e => int.Parse(e.ToString())).ToList());
                    break;
                case "Single":
                    fieldInfo.SetValue(this, list.Select(e => float.Parse(e.ToString())).ToList());
                    break;
                case "Boolean":
                    fieldInfo.SetValue(this, list.Select(e => bool.Parse(e.ToString())).ToList());
                    break;
                case "String":
                    fieldInfo.SetValue(this, list.Select(e => e.ToString()).ToList());
                    break;
                case "DateTime":
                    fieldInfo.SetValue(this, list.Select(e => DateTime.Parse(e.ToString())).ToList());
                    break;
                default:
                    fieldInfo.SetValue(this, list);
                    break;
            }
        }
    }
}