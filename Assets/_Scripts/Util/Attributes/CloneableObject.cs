using UnityEngine;

public abstract class CloneableObject<T>
{
    public T GetMemberwiseClone()
    {
        return (T)MemberwiseClone();
    }
}
