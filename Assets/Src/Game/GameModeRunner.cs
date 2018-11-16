using UnityEngine;

public class GameModeRunner : MonoBehaviour
{
    protected GameState state = GameState.Loading;
    protected Player player = null;

    public virtual void Init()
    {
    }

    public virtual void Destroy()
    {
        Destroy(this);
    }

    public virtual void Update()
    {
    }

    public virtual void LateUpdate()
    {
    }

    public virtual void FixedUpdate()
    {
        Physics.SyncTransforms();
    }

    public GameState GetState()
    {
        return this.state;
    }

    public Player GetPlayer()
    {
        return this.player;
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }
}
