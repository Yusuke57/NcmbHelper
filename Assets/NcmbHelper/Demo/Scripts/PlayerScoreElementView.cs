using UnityEngine;
using UnityEngine.UI;

namespace Yusuke57.CommonPackage
{
    public class PlayerScoreElementView : MonoBehaviour
    {
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text scoreText;

        public void Show(string playerName, int score)
        {
            playerNameText.text = playerName;
            scoreText.text = score.ToString();
        }
    }
}