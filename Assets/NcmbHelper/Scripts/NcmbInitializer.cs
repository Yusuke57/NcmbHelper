using NCMB;
using UnityEngine;

namespace Yusuke57.CommonPackage
{
    public class NcmbInitializer : MonoBehaviour
    {
        [SerializeField] private string applicationKey;
        [SerializeField] private string clientKey;
        
        private void Awake()
        {
            CreateNcmbObjects();
        }

        private void CreateNcmbObjects()
        {
            if (NCMBSettings._isInitialized)
            {
                return;
            }
            
            // NCMB側ではオブジェクト名で管理している処理があるので、オブジェクト名は固定
            var managerObj = new GameObject("NCMBManager");
            managerObj.AddComponent<NCMBManager>();
            var settingsObj = new GameObject("NCMBSettings");
            settingsObj.AddComponent<NCMBSettings>();
            
            NCMBSettings.Initialize(applicationKey, clientKey, string.Empty, string.Empty);
        }
    }
}