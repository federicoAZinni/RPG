using UnityEngine;
using UnityEditor;

[System.Serializable]
public class UniqueID
{
    const string SHORTCUT = "&u";

    public string uniqueID;

    public UniqueID() => uniqueID = System.Guid.NewGuid().ToString();
    public UniqueID(byte[] byteArray) => uniqueID = new System.Guid(byteArray).ToString();
    public UniqueID(string id) => uniqueID = new System.Guid(id).ToString();
    public UniqueID(UniqueID id) => uniqueID = id.uniqueID;
    public UniqueID(System.Guid id) => uniqueID = id.ToString();

    public int GetHash() => new System.Guid(uniqueID).GetHashCode();
    public byte[] GetByteArray() => new System.Guid(uniqueID).ToByteArray();

    public override string ToString() => uniqueID;
    public override int GetHashCode() => GetHash();

    public static implicit operator string(UniqueID id) => id.uniqueID;
    public static implicit operator byte[](UniqueID id) => id.GetByteArray();

    public override bool Equals(object obj)
    {
        UniqueID other = obj as UniqueID;
        if (other == null) return false;
        return uniqueID == other.uniqueID;
    }

    public static bool operator ==(UniqueID my, UniqueID other) => my.uniqueID == other.uniqueID;
    public static bool operator !=(UniqueID my, UniqueID other) => my.uniqueID != other.uniqueID;

#if UNITY_EDITOR
    [MenuItem("Pizia/Generate Random GUID " + SHORTCUT)]
    static void LogRandomGUID() => Debug.Log(System.Guid.NewGuid().ToString());
#endif
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(UniqueID))]
public class GRandomGUIDEditor : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);
        SerializedProperty prop = property.FindPropertyRelative("uniqueID");
        EditorGUI.BeginDisabledGroup(true);

        Vector2 textSize = new Vector2(rect.width - 90, rect.height);
        EditorGUI.TextField(new Rect(rect.position, textSize), label, prop.stringValue);
        EditorGUI.EndDisabledGroup();

        if (GUI.Button(new Rect(rect.position + Vector2.right * textSize + Vector2.right, new Vector2(rect.width - textSize.x, rect.height)), "Generate UID"))
            prop.stringValue = System.Guid.NewGuid().ToString();

        EditorGUI.EndProperty();
    }
}
#endif