using System.Collections.Generic;
using UnityEngine;

namespace KKH
{
    public class RoutineManager : MonoBehaviour
    {
        public static RoutineManager Instance { get; private set; }

        [System.Serializable]
        public class Task
        {
            public string id;      // "wash", "eat", "pc", "sort_cup"
            public string title;   // 표시용
            public bool done;
        }

        public List<Task> tasks = new List<Task>()
    {
        new Task{ id="wash", title="세면대에서 세수하기", done=false },
        new Task{ id="eat",  title="식탁에서 아침 먹기", done=false },
        new Task{ id="pc",   title="컴퓨터 켜기", done=false },
        new Task{ id="sort_cup", title="컵 제자리에 두기", done=false },
        new Task{ id="sort_book", title="책 제자리에 두기", done=false },
    };

        void Awake()
        {
            Instance = this;
        }

        public void Complete(string taskId)
        {
            var t = tasks.Find(x => x.id == taskId);
            if (t != null && !t.done)
            {
                t.done = true;
                Debug.Log($"[Routine] 완료: {t.title}");
            }
        }

        public void OnSorted(string itemId)
        {
            // itemId -> taskId 매핑
            if (itemId == "cup") Complete("sort_cup");
            if (itemId == "book") Complete("sort_book");
        }

        public bool IsAllDone()
        {
            foreach (var t in tasks)
                if (!t.done) return false;
            return true;
        }
    }
}