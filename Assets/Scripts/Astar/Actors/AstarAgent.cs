using System;
using System.Collections;
using Astar.Pathfinding;
using UnityEngine;

namespace Astar.Actors
{
    public class AstarAgent : MonoBehaviour, IAstarActor
    {
        [SerializeField] float speed = 1;

         public Vector3 destination = Vector3.negativeInfinity;
        bool _isAvailable;
        Vector3[] _path;
        int _targetIndex;
        public event Action OnReachDestination;


        void Awake()
        {
            PathRequestManager.OnReady += Init;
        }

        void Init()
        {
            _isAvailable = true;
            if(destination != Vector3.negativeInfinity) RequestPath();
        }

        public void SetDestination(Vector3 point)
        {
            destination = point;
            if(_isAvailable) RequestPath();
        }

        void RequestPath()
        {
            PathRequestManager.RequestPath(transform.position, destination, OnPathFound);
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
            _targetIndex = 0;
            while (true)
            {
                if (transform.position == currentWaypoint)
                {
                    _targetIndex++;
                    if (_targetIndex >= _path.Length)
                    {
                        DestinationReached();
                        yield break;
                    }
                        
                    currentWaypoint = _path[_targetIndex];
                }

                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }
        }

        void DestinationReached()
        {
            StopCoroutine(nameof(FollowPath));
            _path = null;
            OnReachDestination?.Invoke();
        }
    }
}
