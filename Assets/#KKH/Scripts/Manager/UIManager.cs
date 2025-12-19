using TMPro;
using UnityEngine;

namespace KKH
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Inst => instance;
        private static UIManager instance;

        public TMP_Text key_E_Text;
        public TMP_Text key_F_Text;
        public TMP_Text item_Text;

        private void Awake()
        {
            instance = this;
        }

        public void SetKeyUI(string str_e = "", string str_f = "")
        {
            key_E_Text.text = str_e;
            key_F_Text.text = str_f;
        }

        public void SetItemUI(string str_item = "")
        {
            item_Text.text = str_item;
        }
    }
}
