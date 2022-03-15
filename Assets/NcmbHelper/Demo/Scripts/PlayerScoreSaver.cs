using UnityEngine;
using UnityEngine.UI;

namespace Yusuke57.CommonPackage
{
    public class PlayerScoreSaver : MonoBehaviour
    {
        [SerializeField] private InputField playerNameInputField;
        [SerializeField] private InputField scoreInputField;
        [SerializeField] private Button saveButton;

        private void Awake()
        {
            saveButton.onClick.AddListener(OnClickSaveButton);
        }

        /// <summary>
        /// セーブボタン押下時に呼び出される
        /// </summary>
        private void OnClickSaveButton()
        {
            // InputFieldの値を取得する
            var playerName = playerNameInputField.text;
            if (!int.TryParse(scoreInputField.text, out var score))
            {
                Debug.LogError("Scoreをint型に変換できません");
                return;
            }
            
            // セーブするデータを作成
            var data = new PlayerScoreData
            {
                playerName = playerName,
                score = score
            };
            
            // NCMBにセーブ
            NcmbHelper.Save(data, isSuccess =>
            {
                if (isSuccess)
                {
                    Debug.Log($"NCMBにセーブしました: playerName={playerName}, score={score}");
                }
                else
                {
                    Debug.LogError($"NCMBにセーブできませんでした: playerName={playerName}, score={score}");
                }
            });
            
            // InputFieldのテキストを消す
            playerNameInputField.text = string.Empty;
            scoreInputField.text = string.Empty;
        }
    }
}