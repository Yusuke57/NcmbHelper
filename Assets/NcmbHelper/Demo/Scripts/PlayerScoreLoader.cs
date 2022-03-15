using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Yusuke57.CommonPackage
{
    public class PlayerScoreLoader : MonoBehaviour
    {
        [SerializeField] private PlayerScoreElementView elementOriginObj;
        [SerializeField] private Button loadButton;

        private readonly List<PlayerScoreElementView> elementViews = new ();

        private void Awake()
        {
            loadButton.onClick.AddListener(OnClickLoadButton);
        }
        
        /// <summary>
        /// ロードボタン押下時に呼び出される
        /// </summary>
        private void OnClickLoadButton()
        {
            // スコア順にするクエリ作成
            var query = NcmbHelper.CreateQuery<PlayerScoreData>();
            query.OrderByDescending("score");
            
            // NCMBからロード
            NcmbHelper.Load<PlayerScoreData>(query, list =>
            {
                if (list == null)
                {
                    Debug.LogError("NCMBからロードできませんでした");
                    return;
                }
                
                ShowList(list);
                
                Debug.Log($"NCMBからロードしました: {list.Count}件");
                foreach (var data in list)
                {
                    Debug.Log($"playerName={data.playerName}, score={data.score}, ObjectId={data.ObjectId}");
                }
            });
        }

        /// <summary>
        /// データリストを表示
        /// </summary>
        private void ShowList(List<PlayerScoreData> list)
        {
            // 全て非表示
            foreach (var elementView in elementViews)
            {
                elementView.gameObject.SetActive(false);
            }

            // 生成、表示
            foreach (var elementData in list)
            {
                var targetView = elementViews.FirstOrDefault(view => !view.gameObject.activeSelf)
                                 ?? Instantiate(elementOriginObj, elementOriginObj.transform.parent);
                targetView.Show(elementData.playerName, elementData.score);
                targetView.gameObject.SetActive(true);
                
                elementViews.Add(targetView);
            }
        }
    }
}