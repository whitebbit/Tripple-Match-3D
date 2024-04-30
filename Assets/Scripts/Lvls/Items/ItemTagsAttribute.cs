using UnityEngine;

public class ItemTagsAttribute : PropertyAttribute
{
    public int sub;
    public ItemTagsAttribute(int sub)
    {
        this.sub = sub;
    }
}