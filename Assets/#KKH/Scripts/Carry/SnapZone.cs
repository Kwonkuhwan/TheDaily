using UnityEngine;

namespace KKH
{
    public class SnapZone : MonoBehaviour
    {
        public string acceptsItemId = "cup";
        public Transform snapPoint; // 비우면 자기 자신
        public float snapDelay = 0.05f;

        public bool occupied;

        void Awake()
        {
            if (!snapPoint) snapPoint = transform;
        }

        void OnTriggerEnter(Collider other)
        {
            if (occupied) return;

            var item = other.GetComponentInParent<CarryItem>();
            if (!item) return;
            if (item.itemId != acceptsItemId) return;

            // 플레이어가 아직 들고 있는 상태면 스냅하지 않음
            if (item.GetComponent<Rigidbody>()?.useGravity == false) return;

            StartCoroutine(SnapRoutine(item));
        }

        System.Collections.IEnumerator SnapRoutine(CarryItem item)
        {
            yield return new WaitForSeconds(snapDelay);
            if (occupied) yield break;

            // 안전 체크
            if (!item) yield break;

            // 위치 고정 스냅
            var rb = item.GetComponent<Rigidbody>();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.MovePosition(snapPoint.position);
            rb.MoveRotation(snapPoint.rotation);

            occupied = true;

            // 정리도 반영
            CleaningManager.Instance?.OnItemSorted(item);
            RoutineManager.Instance?.OnSorted(item.itemId);
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

            // SnapPoint 표시
            if (snapPoint)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(snapPoint.position, 0.05f);

                Gizmos.color = Color.white;
                Gizmos.DrawLine(snapPoint.position + Vector3.left * 0.1f, snapPoint.position + Vector3.right * 0.1f);
                Gizmos.DrawLine(snapPoint.position + Vector3.forward * 0.1f, snapPoint.position + Vector3.back * 0.1f);
                Gizmos.DrawLine(snapPoint.position + Vector3.up * 0.1f, snapPoint.position + Vector3.down * 0.1f);
            }
        }
#endif

    }
}