using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;

public class Invocation : MonoBehaviour
{
    [HideInInspector]
    public PjBase user;
    public int duration;
    [HideInInspector]
    public bool untimed;

    public void NormalSetUp(PjBase user, int duration)
    {
        this.user = user;
        this.duration = duration;
    }
    public void Tick()
    {
        if (!untimed)
        {
            duration--;
            if (duration <= 0)
            {
                StartCoroutine(Die());
            }
        }
    }

    public virtual IEnumerator Die()
    {
        yield return null;

        user.invocationList.Remove(this);
        Destroy(gameObject);
    }
}
