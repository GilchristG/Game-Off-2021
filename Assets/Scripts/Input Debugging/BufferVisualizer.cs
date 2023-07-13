using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferVisualizer : MonoBehaviour
{
    [SerializeField] GameObject entryPF;
    [SerializeField] GameObject parentToSpawnOn;
    Queue<GameObject> moveEntries = new Queue<GameObject>();

    public int maxEntiresShown;

    public void SetupBV(GameObject UIParent)
    {
        parentToSpawnOn = UIParent;
    }

    public void DisplayBuffer(InputFrame currentFrame)
    {
        if (parentToSpawnOn != null)
        {
            var element = Instantiate(entryPF, parentToSpawnOn.transform);
            element.GetComponent<FrameUIElement>().Setup(currentFrame);
            moveEntries.Enqueue(element);
            if (moveEntries.Count > maxEntiresShown)
            {
                var lastElement = moveEntries.Dequeue();
                Destroy(lastElement);
            }
        }
    }
}
