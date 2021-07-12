using System.Collections;
using Astar.Pathfinding;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    float speed = 1;
    Vector3[] _path;
    int _targetIndex;

    void Awake()
    {
        PathRequestManager.OnReady += Init;
    }

    void Init()
    {
        print("fire");
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            _path = newPath;
            StopCoroutine(nameof(FollowPath));
            StartCoroutine(nameof(FollowPath));
        }

    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = _path[0];
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                _targetIndex++;
                if(_targetIndex >= _path.Length)
                    yield break;
                currentWaypoint = _path[_targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        if (_path != null)
        {
            for (int i = _targetIndex; i < _path.Length; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(_path[i], Vector3.one);

                if (i == _targetIndex)
                {
                    Gizmos.DrawLine(transform.position, _path[i]);
                }
                else
                {
                    Gizmos.DrawLine(_path[i-1], _path[i]);
                }
            }
        }
    }
}
