using System.Collections.Generic;
using game;
using Unity.VisualScripting;
using UnityEngine;

public class PathHistory : MonoBehaviour
{
    private class Frame
    {
        public Sprite sprite;
        public Vector3 position;
        public Quaternion rotation;
        public GameObject instance;
    }
    
    [field: SerializeField] public Transform target { get; private set; }
    [field: SerializeField] private float time { get; set; } = 1f;
    
    private SpriteRenderer spriteRenderer { get; set; }
    private PlayerInputAsset inputs { get; set; }
    private List<Frame> frames { get; set; } = new();
    private bool recording { get; set; }

    private const float DefAlpha = .35f;
    
    private void Awake()
    {
        inputs = new PlayerInputAsset();
        inputs.debug.RecordPath.performed += context => RecordPath();
        inputs.debug.ClearPath.performed += context => ClearPath();
        spriteRenderer = target.GetComponent<SpriteRenderer>();
    }

    private void OnEnable() => inputs.Enable();
    private void OnDisable() => inputs.Disable();

    public void RecordPath()
    {
        recording = !recording;

        if (recording)
        {
            AddFrame();
            GameManager.instance.WaitAndDoWhile(time, AddFrame, () => recording);
            return;
        }
        
        if (frames.Count == 0) return;
        foreach (var frame in frames) InstantiateFrame(frame);
    }

    public void ClearPath()
    {
        if (frames.Count == 0) return;
        foreach (var frame in frames) Destroy(frame.instance);
        frames.Clear();
    }

    private static void InstantiateFrame(Frame frame)
    {
        var instance = new GameObject();
        instance.transform.position = frame.position;
        instance.transform.rotation = frame.rotation;
        var r = instance.AddComponent<SpriteRenderer>();
        r.sprite = frame.sprite;
        r.flipX = true;
        r.color = new Color(r.color.r, r.color.g, r.color.b, DefAlpha);
        frame.instance = instance;
    }

    private void AddFrame()
    {
        frames.Add(new Frame()
        {
            sprite = spriteRenderer.sprite,
            position = target.position,
            rotation = target.rotation
        });
    }
}
