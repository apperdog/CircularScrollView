using UnityEngine;

public class DragObject : MonoBehaviour
{
  public Transform transform { get; private set; }
  public CircularScrollView circularScrollView;

  protected virtual void Awake()
  {
    transform = GetComponent<Transform>();
  }
}
