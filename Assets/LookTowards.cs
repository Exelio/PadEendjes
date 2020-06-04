using UnityEngine;

public class LookTowards : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    private Quaternion _targetRotation;

    void Update()
    {
        _targetRotation.y = _target.rotation.y;

        transform.rotation = _targetRotation;
    }
}
