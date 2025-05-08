using UnityEngine;

[CreateAssetMenu(fileName = "NewObjective", menuName = "Game/Objective Definition")]
public class ObjectiveDefinition : ScriptableObject
{
    [TextArea(2, 4)]
    public string displayText;
    public bool isMandatory = true;
    public bool hideWhenCompleted = false;
}