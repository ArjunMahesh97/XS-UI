using UnityEngine;

public class WordSearchAppearer : MonoBehaviour
{
    public float Duration = 1;

    private float time;

    private void Update()
    {
        time += Time.deltaTime;
        
        float t = time / Duration;
        t = 1 - (1 - t) * (1 - t);
        transform.localScale = Vector3.one * Mathf.Clamp01(t);

        if (time > Duration)
        {
            transform.localScale = Vector3.one;
            Destroy(this);
        }
    }
}
