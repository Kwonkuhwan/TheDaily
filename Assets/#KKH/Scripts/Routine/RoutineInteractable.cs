using UnityEngine;

namespace KKH
{
    public class RoutineInteractable : MonoBehaviour, IInteractable
    {
        public string taskId = "wash";
        public string prompt = "E: 사용하기";

        public string Prompt => prompt;

        public void Interact(Interactor interactor)
        {
            RoutineManager.Instance?.Complete(taskId);
        }
    }
}