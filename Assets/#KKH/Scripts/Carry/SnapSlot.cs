using UnityEngine;

namespace KKH
{
    public class SnapSlot : MonoBehaviour
    {
        [Header("Accept Filter (optional)")]
        [Tooltip("비우면 어떤 itemId든 허용")]
        public string[] acceptsItemIds;

        [Tooltip("비우면 어떤 Tag든 허용")]
        public string acceptsTag = "";

        [Header("State (read only)")]
        public bool occupied;
        public CarryItem current;

        public bool CanAccept(CarryItem item)
        {
            if (occupied) return false;
            if (!item) return false;

            // Tag 필터
            if (!string.IsNullOrEmpty(acceptsTag) && !item.CompareTag(acceptsTag))
                return false;

            // itemId 필터
            if (acceptsItemIds != null && acceptsItemIds.Length > 0)
            {
                for (int i = 0; i < acceptsItemIds.Length; i++)
                {
                    if (item.itemId == acceptsItemIds[i])
                        return true;
                }
                return false;
            }

            return true;
        }

        public void Snap(CarryItem item)
        {
            var rb = item.GetComponent<Rigidbody>();
            if (!rb) return;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.position = transform.position;           // 즉시 이동 (안정적)
            rb.rotation = transform.rotation;           // 즉시 회전

            // 스냅 후 고정 (반복 낙하 방지)
            //rb.isKinematic = true;
            //rb.useGravity = false;

            occupied = true;
            current = item;

            item.SetSnapped(this); // CarryItem에 구현되어 있어야 함
        }

        public void ClearIfSame(CarryItem item)
        {
            if (current == item)
            {
                occupied = false;
                current = null;
            }
        }

        public void Clear()
        {
            occupied = false;
            current = null;
        }
    }
}
