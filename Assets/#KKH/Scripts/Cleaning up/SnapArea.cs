using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    [RequireComponent(typeof(BoxCollider))]
    public class SnapArea : MonoBehaviour
    {
        public enum AreaType { Surface, Container }

        [Header("Type")]
        public AreaType areaType = AreaType.Surface;

        [Header("Placement")]
        public float surfaceYOffset = 0.02f;   // Surface일 때 상판에서 살짝 띄우기
        public float snapDelay = 0.05f;        // 드랍 직후 안정화 대기

        [Header("Slots (children)")]
        public List<SnapSlot> slots = new List<SnapSlot>();

        BoxCollider box;

        void Awake()
        {
            box = GetComponent<BoxCollider>();
            box.isTrigger = true;

            if (slots.Count == 0)
                slots.AddRange(GetComponentsInChildren<SnapSlot>(true));
        }

        void OnTriggerEnter(Collider other)
        {
            var item = other.GetComponentInParent<CarryItem>();
            if (!item || item.IsSnapped) return;

            var rb = item.GetComponent<Rigidbody>();
            if (rb && rb.useGravity == false) return; // 들고 있으면 스냅 X

            StartCoroutine(SnapRoutine(item));
        }

        System.Collections.IEnumerator SnapRoutine(CarryItem item)
        {
            yield return new WaitForSeconds(snapDelay);

            if (!item) yield break;

            var rb = item.GetComponent<Rigidbody>();
            if (!rb) yield break;

            // 슬롯 중에서 "이 아이템을 받을 수 있는" 가장 가까운 슬롯 찾기
            SnapSlot best = null;
            float bestDist = float.MaxValue;

            Vector3 itemPos = item.transform.position;

            for (int i = 0; i < slots.Count; i++)
            {
                var s = slots[i];
                if (!s || !s.CanAccept(item)) continue;

                float d = (s.transform.position - itemPos).sqrMagnitude;
                if (d < bestDist) { bestDist = d; best = s; }
            }

            if (best == null)
                yield break; // 이 영역에 들어왔지만, 받을 슬롯이 없음(=잘못된 장소/자리)

            // Surface면 "상판 높이"로 한번 보정해주고 슬롯 스냅
            // (슬롯 Transform을 상판 위에 정확히 올려뒀다면 사실 필요 없음)
            if (areaType == AreaType.Surface)
            {
                // 슬롯 y가 정확하다면 이 블록은 지워도 됨
                // itemPos.y = best.transform.position.y + surfaceYOffset;
            }

            best.Snap(item);

            CleaningManager.Instance?.OnItemSorted(item);
            RoutineManager.Instance?.OnSorted(item.itemId);
        }

        // (선택) 물건이 영역 밖으로 나가면, 점유 해제
        void OnTriggerExit(Collider other)
        {
            var item = other.GetComponentInParent<CarryItem>();
            if (!item) return;

            for (int i = 0; i < slots.Count; i++)
                slots[i]?.ClearIfSame(item);
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // SnapZone 영역 표시
            Gizmos.color = new Color(0f, 1f, 0f, 0.15f); // 연두 반투명

            var box = GetComponent<BoxCollider>();
            if (box)
            {
                // Collider 기준 월드 좌표 보정
                Matrix4x4 oldMatrix = Gizmos.matrix;
                Gizmos.matrix = transform.localToWorldMatrix;

                Gizmos.DrawCube(box.center, box.size);

                Gizmos.matrix = oldMatrix;
            }
        }
#endif
    }
}
