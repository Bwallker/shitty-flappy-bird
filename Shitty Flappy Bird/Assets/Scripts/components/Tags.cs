using System.Collections.Generic;

using UnityEngine;


namespace components
{
  public sealed class Tags : MonoBehaviour
  {
    [SerializeField]
    private List<string> tagsList = new();

    public HashSet<string> tagsSet = new(0);

    public void Awake()
    {
      this.tagsSet = new(this.tagsList);
    }

    internal static HashSet<string> GetTags(GameObject obj)
    {
      var tags = obj.GetComponent<Tags>();

      return tags == null ? new(0) : tags.tagsSet;
    }

    internal static bool HasTag(GameObject obj, string tag)
    {
      var tags = Tags.GetTags(obj);

      return tags.Contains(tag);
    }

    internal static bool RemoveTag(GameObject obj, string tagToRemove)
    {
      var tags = Tags.GetTags(obj);

      return tags.Remove(tagToRemove);
    }

    internal static bool AddTag(GameObject obj, string tagToAdd)
    {
      var tags = Tags.GetTags(obj);

      return tags.Add(tagToAdd);
    }
  }
}
