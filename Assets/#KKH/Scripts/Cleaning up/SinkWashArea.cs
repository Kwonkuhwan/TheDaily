using UnityEngine;

namespace KKH
{
    [RequireComponent(typeof(BoxCollider))]
    public class SinkWashArea : MonoBehaviour
    {
        public float washTime = 2.0f;

        void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;
        }

        void OnTriggerStay(Collider other)
        {
            var item = other.GetComponentInParent<CarryItem>();
            if (!item) return;
            if (item.dishState != DishState.Dirty) return;

            // 들고 있으면 씻지 않음
            var rb = item.GetComponent<Rigidbody>();
            if (rb && rb.useGravity == false) return;

            StartCoroutine(WashRoutine(item));
        }

        System.Collections.IEnumerator WashRoutine(CarryItem item)
        {
            // 중복 방지
            if (item.dishState != DishState.Dirty) yield break;

            yield return new WaitForSeconds(washTime);

            item.SetClean();
            Debug.Log("설거지 완료!");
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // SnapZone 영역 표시
            Gizmos.color = new Color(0f, 0f, 1f, 0.15f); // 파랑 반투명

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
