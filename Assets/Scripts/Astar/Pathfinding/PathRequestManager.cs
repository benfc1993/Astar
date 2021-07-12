using System;
using System.Collections.Generic;
using UnityEngine;

namespace Astar.Pathfinding
{
    public class PathRequestManager : MonoBehaviour
    {
        Queue<PathRequest> _requestQueue = new Queue<PathRequest>();
        PathRequest _currentRequest;
        bool _isProcessingPath;
        Pathfinding _pathFinding;
        static PathRequestManager _instance;
        public static event Action OnReady;

        void Awake()
        {
            _instance = this;
            _pathFinding = GetComponent<Pathfinding>();
        }


        public void Init()
        {
            OnReady?.Invoke();
        }

        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback, bool simplify = true)
        {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback, simplify);
            _instance._requestQueue.Enqueue(newRequest);
            _instance.TryProcessNext();
        }

        void TryProcessNext()
        {
            if (!_isProcessingPath && _requestQueue.Count > 0)
            {
                _currentRequest = _requestQueue.Dequeue();
                _isProcessingPath = true;
                _pathFinding.StartFindPath(_currentRequest.pathStart, _currentRequest.pathEnd, _currentRequest.simplify);
            }
        }

        public void FinishedProcessingPath(Vector3[] path, bool success)
        {
            _currentRequest.callback(path, success);
            _isProcessingPath = false;
            TryProcessNext();
        }

        readonly struct PathRequest
        {
            public readonly Vector3 pathStart;
            public readonly Vector3 pathEnd;
            public readonly Action<Vector3[], bool> callback;
            public readonly bool simplify;

            public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback, bool simplify)
            {
                this.pathStart = pathStart;
                this.pathEnd = pathEnd;
                this.callback = callback;
                this.simplify = simplify;
            }
        }

    }
}
