using UnityEngine;

namespace KKH
{
    public class CarrySystem : MonoBehaviour
    {
        public Transform holdPoint;
        public float holdLerp = 20f;

        Rigidbody carriedRb;
        CarryItem carriedItem;

        void Update()
        {
            if (carriedRb)
            {
                Vector3 targetPos = holdPoint.position;
                Quaternion targetRot = holdPoint.rotation;

                carriedRb.MovePosition(Vector3.Lerp(carriedRb.position, targetPos, Time.deltaTime * holdLerp));
                carriedRb.MoveRotation(Quaternion.Slerp(carriedRb.rotation, targetRot, Time.deltaTime * holdLerp));
            }
        }

        public bool IsCarrying => carriedRb != null;

        public bool TryPick(CarryItem item)
        {
            if (IsCarrying) return false;

            carriedItem = item;
            carriedRb = item.GetComponent<Rigidbody>();

            carriedRb.interpolation = RigidbodyInterpolation.Interpolate;
            carriedRb.constraints = RigidbodyConstraints.FreezeRotation; // 들고 있을 때 안정감
            carriedRb.useGravity = false;

            item.OnPickedUp();
            return true;
        }

        public bool TryDrop()
        {
            if (!IsCarrying) return false;

            carriedItem.OnDropped();

            // 🔹 수평 기준 놓기 위치
            Vector3 flatForward = transform.forward;
            flatForward.y = 0f;
            flatForward.Normalize();

            Vector3 dropPos = transform.position;
            dropPos.y += 1.2f;              // 가슴~눈 높이
            dropPos += flatForward * 0.4f;  // 앞쪽

            carriedRb.linearVelocity = Vector3.zero;
            carriedRb.angularVelocity = Vector3.zero;
            carriedRb.position = dropPos;

            carriedRb.useGravity = true;
            carriedRb.constraints = RigidbodyConstraints.None;

            carriedRb = null;
            carriedItem = null;
            return true;
        }

        public bool TryThrow(float force)
        {
            if (!IsCarrying) return false;

            carriedItem.OnDropped();

            Rigidbody rb = carriedRb;

            // 🔹 카메라 방향으로 던지기
            Vector3 dir = Camera.main.transform.forward.normalized;

            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(dir * force, ForceMode.Impulse);

            carriedRb = null;
            carriedItem = null;
            return true;
        }

        public CarryItem CurrentItem => carriedItem;
    }
}