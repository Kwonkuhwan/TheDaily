using UnityEngine;

namespace KKH
{
    public enum DishState
    {
        Clean,
        Dirty,
        Garbage
    }

    [RequireComponent(typeof(Rigidbody))]
    public class CarryItem : MonoBehaviour, IInteractable
    {
        public string itemId = "cup";
        public string displayName = "컵";
        public int cleanValue = 1; // 제자리에 두면 올라가는 정리도

        public DishState dishState = DishState.Clean;

        public string Prompt => "E: 집기/놓기";

        private SnapSlot currentSlot;
        public bool IsSnapped { get; private set; }

        public void Interact(Interactor interactor)
        {
            var carry = interactor.carry;
            if (!carry) return;

            if (!carry.IsCarrying)
            {
                carry.TryPick(this);
            }
            else
            {
                // 같은 아이템을 들고 있을 때만 드랍
                if (carry.CurrentItem == this)
                    carry.TryDrop();
            }
        }

        public void OnPickedUp()
        {
            UIManager.Inst.SetKeyUI("E : 놓기", "F : 던지기");
            // 필요하면 SFX, 하이라이트 등
            // 슬롯 점유 해제
            if (IsSnapped && currentSlot != null)
            {
                currentSlot.Clear();
                ClearSnapped();
            }

            var rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = false; // 들고 있을 땐 중력 끄는 기존 로직 유지
        }

        public void OnDropped()
        {
            UIManager.Inst.SetKeyUI("E : 줍기");
            // 필요하면 SFX 등
        }

        public void SetSnapped(KKH.SnapSlot slot)
        {
            currentSlot = slot;
            IsSnapped = true;
        }

        public void ClearSnapped()
        {
            currentSlot = null;
            IsSnapped = false;
        }

        // 설거지 완료 처리
        public void SetClean()
        {
            dishState = DishState.Clean;
            // 비주얼 변경은 여기서 (머티리얼, 이펙트)
            gameObject.SetActive(false);
        }
    }
}