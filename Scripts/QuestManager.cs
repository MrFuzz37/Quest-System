using UnityEngine;
using UnityEngine.Events;

// helper class to handle the unity events
public class QuestManager : MonoBehaviour
{
    [System.Serializable]
    public class QuestKill : UnityEvent<string> { }

    [System.Serializable]
    public class QuestInteract : UnityEvent { }

    [System.Serializable]
    public class QuestNarrative : UnityEvent<int> { }

    [System.Serializable]
    public class DialogueAccept : UnityEvent<Quest> { }

    [System.Serializable]
    public class QuestGoal : UnityEvent<GameObject> { }

    [System.Serializable]
    public class QuestDialogue : UnityEvent<string> { }
}
 