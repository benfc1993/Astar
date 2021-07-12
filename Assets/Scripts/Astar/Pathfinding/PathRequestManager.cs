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
        static PathRequestManager instance;
        public static event Action OnReady;

        void Awake()
        {
            instance = this;
            _pathFinding = GetComponent<Pathfinding>();
        }


        public void Init()
        {
            OnReady?.Invoke();
        }

        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
            instance._requestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }

        private void TryProcessNext()
        {
            if (!_isProcessingPath && _requestQueue.Count > 0)
            {
                _currentRequest = _requestQueue.Dequeue();
                _isProcessingPath = true;
                _pathFinding.StartFindPath(_currentRequest.pathStart, _currentRequest.pathEnd);
            }
        }

        public void FinishedProcessingPath(Vector3[] path, bool sucess)
        {
            _currentRequest.callback(path, sucess);
            _isProcessingPath = false;
            TryProcessNext();
        }

        readonly struct PathRequest
        {
            public readonly Vector3 pathStart;
            public readonly Vector3 pathEnd;
            public readonly Action<Vector3[], bool> callback;

            public PathRequest(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
            {
                this.pathStart = pathStart;
                this.pathEnd = pathEnd;
                this.callback = callback;
            }
        }

    }
}
