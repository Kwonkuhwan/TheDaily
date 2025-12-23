using UnityEngine;


namespace KKH
{
    [RequireComponent(typeof(BoxCollider))]
    public class GarbageArea : MonoBehaviour
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
            if (item.dishState != DishState.Garbage) return;

            // 들고 처리 하지 않음
            var rb = item.GetComponent<Rigidbody>();
            if (rb && rb.useGravity == false) return;

            StartCoroutine(GarbageRoutine(item));
        }

        System.Collections.IEnumerator GarbageRoutine(CarryItem item)
        {
            // 중복 방지
            if (item.dishState != DishState.Garbage) yield break;

            yield return new WaitForSeconds(washTime);

            item.SetClean();
            Debug.Log("쓰레기 처리 완료!");
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // SnapZone 영역 표시
            Gizmos.color = new Color(1f, 0f, 0f, 0.15f); // 파랑 반투명

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