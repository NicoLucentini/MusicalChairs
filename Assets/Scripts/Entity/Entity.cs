using UnityEngine;

public abstract class Entity : MonoBehaviour{
    protected Animation anim;

    [Header("Custom Properties")]
    public EntitySettings settings;

    public bool isHuman;

    public void ApplySettings(EntitySettings settings)
    {
        this.settings = settings;

        GameObject go = Instantiate(settings.prefab, transform);
        anim = go.GetComponent<Animation>();
    }
    public abstract void Jump();
    public abstract void Sit();
}
