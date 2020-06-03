using UnityEngine;

public class WordSearchRotator : MonoBehaviour
{
    public float Speed = 50;

    private void Update()
    {
        transform.Rotate(0, 0, Speed * Time.unscaledDeltaTime);
    }
}