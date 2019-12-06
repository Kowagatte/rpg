using UnityEngine;

public class ExampleCamSine : MonoBehaviour
{
    [SerializeField] private float strength = 1f;
    [SerializeField] private float speed = 1f;
    private Vector3 startPosition;
    private float elapsed = 0f;

    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPosition + new Vector3(Mathf.Sin(speed * elapsed) * strength, 0f, 0f);
        elapsed += Time.deltaTime;
        var vec3zero = structexample.NewZero;
    }
}
