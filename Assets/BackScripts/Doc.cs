using UnityEngine;
/// <summary>
///  A class used for maintaining per-object documentation
/// </summary>
public class Doc : MonoBehaviour
{
    [TextArea(3, 8)]
    public string Notes;
}