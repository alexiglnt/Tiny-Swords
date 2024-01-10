using UnityEngine.UI;
using UnityEngine;

public class GifPlayer : MonoBehaviour
{
    public Texture2D[] frames;
    public float framesPerSecond = 10.0f;

    private RawImage rawImage;

    void Start()
    {
        rawImage = GetComponent<RawImage>();
        rawImage.texture = frames[0];
    }

    void Update()
    {
        //int index = (int)(Time.time * framesPerSecond) % frames.Length;
        //rawImage.texture = frames[index];
    }
}
