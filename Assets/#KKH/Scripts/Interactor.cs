using UnityEngine;

namespace KKH
{
    public class Interactor : MonoBehaviour
    {
        [Header("Value")]
        public float distance = 2.2f;
        public float throwForce = 4.5f;

        [Header("KeyCode")]
        public KeyCode dropKey = KeyCode.E;
        public KeyCode throwKey = KeyCode.F;

        [Header("LayerMask")]
        public LayerMask mask = ~0;

        [Header("Scripts")]
        public CarrySystem carry;

        // 이번 프레임에 바라보고 있는 대상 캐시
        private CarryItem lookedCarryItem;

        private void Awake()
        {
            if (!carry) carry = GetComponentInParent<CarrySystem>();
        }

        private void Update()
        {
            // 레이캐스트는 여기서 딱 1번만
            lookedCarryItem = null;

            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, distance, mask))
            {
                // 콜라이더가 자식에 있을 수 있으니 InParent가 안전
                lookedCarryItem = hit.collider.GetComponentInParent<CarryItem>();
            }

            // 바라볼 때 UI 표시 (누르지 않아도 뜸)
            if (lookedCarryItem != null)
                UIManager.Inst.SetItemUI(lookedCarryItem.displayName);
            else
                UIManager.Inst.SetItemUI(""); // 또는 UIManager.Inst.HideItemUI()

            // 던지기
            if (Input.GetKeyDown(throwKey))
            {
                if (carry != null && carry.IsCarrying)
                    carry.TryThrow(throwForce);

                return;
            }

            // E 키: 들고 있으면 드랍, 아니면 바라보는 대상 Interact
            if (Input.GetKeyDown(dropKey))
            {
                if (carry != null && carry.IsCarrying)
                {
                    carry.TryDrop();
                    return;
                }

                if (lookedCarryItem != null)
                    lookedCarryItem.Interact(this);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, transform.forward * distance);
        }
    }
}
