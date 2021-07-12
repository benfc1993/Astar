using Astar.Actors;
using Astar.Grid;
using UnityEngine;

public class Unit : MonoBehaviour
{
       AstarAgent _agent;

       void Awake()
       {
              _agent = GetComponent<AstarAgent>();
              _agent.OnReachDestination += MoveToRandomPoint;
       }

       void Start()
       {
              MoveToRandomPoint();
       }

       void MoveToRandomPoint()
       {
              AStarGrid.SamplePosition(transform.position, 60, out Node point);
              print(point.WorldPosition);
              if(point.walkable) _agent.SetDestination(point.WorldPosition);
              else MoveToRandomPoint();
       }
       
       
}