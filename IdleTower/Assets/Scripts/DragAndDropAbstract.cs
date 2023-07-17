using UnityEngine;


public abstract class DragAndDropAbstract : MonoBehaviour
{
    [SerializeField] protected float zShift = 0.1f;
    protected float _zDistance;


    private Camera _mainCamera;
    [HideInInspector] public Vector3 _startPos, _newPos, _screenPos;
    public bool isActive = true;

    public virtual void Start()
    {
        _startPos = transform.position;
        _mainCamera = Camera.main;
        _zDistance = _mainCamera.WorldToScreenPoint(transform.position).z;
    }

    protected abstract void CheckIsMatch();

    private void OnMouseDrag()
    {
        if (isActive)
        {
            Drag();
        }
    }

    private void OnMouseUp()
    {
        if (!isActive)
            return;

        CheckIsMatch();
    }

    public virtual void Drag()
    {
        _screenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _zDistance - zShift);
        _newPos = _mainCamera.ScreenToWorldPoint(_screenPos);
        transform.position = _newPos;
    }

    protected virtual void CanBeActivate()
    {
        isActive = true;
    }
}
