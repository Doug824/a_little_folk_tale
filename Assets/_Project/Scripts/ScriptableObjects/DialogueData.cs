using UnityEngine;

namespace ALittleFolkTale.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "A Little Folk Tale/Dialogue/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        [Header("Dialogue Info")]
        public string dialogueID;
        public string dialogueTitle;

        [Header("Dialogue Lines")]
        public DialogueLine[] dialogueLines;

        [Header("Conditions")]
        public DialogueCondition[] requiredConditions;

        [Header("Events")]
        public UnityEngine.Events.UnityEvent onDialogueStart;
        public UnityEngine.Events.UnityEvent onDialogueEnd;

        public bool CanStart()
        {
            if (requiredConditions == null || requiredConditions.Length == 0)
                return true;

            foreach (var condition in requiredConditions)
            {
                if (!condition.IsMet())
                    return false;
            }

            return true;
        }
    }

    [System.Serializable]
    public abstract class DialogueCondition : ScriptableObject
    {
        public abstract bool IsMet();
    }
}