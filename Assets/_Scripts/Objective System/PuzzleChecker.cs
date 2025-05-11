using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWork
{
    public class PuzzleChecker : MonoBehaviour
    {
        [Serializable]
        private struct ChecklistItem_UI
        {
            public Puzzle puzzle;
            public Image image;
        }

        public static event Action OnAllPuzzlesCompleted = delegate { };
        [SerializeField] private string checkmarkTagPrefix = "Checkmark_";
        [SerializeField] private CheckListManager<ChecklistItem_UI> puzzleChecklistManager;
        
        private void Awake()
        {
            InitializePuzzleChecklist();            
        } 

        private void Start()
        {
            Puzzle.OnSpecificPuzzleCompleted += UpdateChecklist;
        }

        private void OnDestroy()
        {
            Puzzle.OnSpecificPuzzleCompleted -= UpdateChecklist;
        }

        /// <summary>
        /// Initializes the puzzle checklist by finding all puzzles in the scene and creating checklist items for them.
        /// <para>IMPORTANT:</para>
        /// It needs TAGS to be assigned:
        /// <list type="bullet">
        /// <item>"PuzzleName" TAG on GameObjects having Puzzle component</item>
        /// <item>"Checkmark_PuzzleName" TAG on the corresponding Icon GameObject on MissionScreen</item>
        /// </list>
        /// </summary>
        private void InitializePuzzleChecklist()
        {
            List<Puzzle> puzzles = FindObjectsByType<Puzzle>(FindObjectsSortMode.None).ToList();
            List<ChecklistItem_UI> checklistItems = new List<ChecklistItem_UI>();
            foreach (var puzzle in puzzles)
            {
                ChecklistItem_UI checklistItem = new ChecklistItem_UI
                {
                    puzzle = puzzle,
                    image = GameObject.FindGameObjectWithTag(tag: checkmarkTagPrefix + puzzle.tag).GetComponent<Image>()
                };
                checklistItems.Add(checklistItem);
            }
            puzzleChecklistManager.InitializeCheckList(checklistItems, isPermanent: true);
        }

        private void UpdateChecklist(Puzzle completedPuzzle)
        {
            ChecklistItem_UI checklistItem = new();
            foreach (var item in puzzleChecklistManager.Items.Where(item => item.element.puzzle == completedPuzzle))
            {
                checklistItem = item.element;
            }

            if(checklistItem.puzzle == null)
            {
                Debug.LogError($"Puzzle {completedPuzzle.name} not found in the checklist.");
                return;
            }

            checklistItem.image.enabled = true;
            puzzleChecklistManager.SetItemCompleted(checklistItem);

            if (puzzleChecklistManager.IsListFullyCompleted())
            {
                InvokePuzzlesCompletedEvent();
            }
        }

        private void InvokePuzzlesCompletedEvent()
        {
            OnAllPuzzlesCompleted?.Invoke();
            Debug.Log("All puzzles completed!");
        }
    }
}