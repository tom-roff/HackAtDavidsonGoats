using UnityEngine;
using UnityEditor;

public class TagChildren : EditorWindow
{
    [MenuItem("GameObject/Tag All Children/Set to Player")]
    static void TagAllChildrenAsPlayer()
    {
        GameObject[] selection = Selection.gameObjects;
        
        foreach(GameObject parent in selection)
        {
            // Tag parent
            parent.tag = "Player";
            
            // Tag all children
            foreach(Transform child in parent.GetComponentsInChildren<Transform>(true))
            {
                child.gameObject.tag = "Player";
            }
        }
    }
}
