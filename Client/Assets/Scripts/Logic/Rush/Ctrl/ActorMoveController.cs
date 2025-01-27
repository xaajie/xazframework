using NodeCanvas.Tasks.Conditions;
using UnityEngine;
using UnityEngine.AI;

public class ActorMoveController
{
    private NavMeshAgent agent;
    private bool isMoving = false;
    //private int detectNum;
    //private static int maxdetectnum = 5;
    private bool hasAudio;
    private float checkTimer = 0;
    private float AudioInv = 0.3f;
    // 初始化NavMeshAgent
    public void SetTarget(GameObject tar)
    {
        bool hasagent = tar.TryGetComponent<NavMeshAgent>(out agent);
        if (!hasagent)
        {
            agent = tar.AddComponent<NavMeshAgent>();
            agent.speed = 10f;
            agent.radius = 0.3f;
            agent.stoppingDistance = 0.1f;
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.acceleration = 100f;
            agent.angularSpeed = 1000f;
            agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        }
    }

    public void SetAudio(bool hasAudiot)
    {
        hasAudio = hasAudiot;
    }
    public void SetAgentSpeed(float speed)
    {
        agent.speed = speed;
    }

    public void MoveByDir(Vector3 moveDirection)
    {

        moveDirection = (Quaternion.Euler(0, -45, 0) * moveDirection).normalized;
        Vector3 targetPos = agent.transform.position + moveDirection * Time.deltaTime * agent.speed*6;
        if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            isMoving = true;
        }
    }

    private Vector3 prePos;
    /// <summary>
    /// 移动到目标点，如果目标点不可达，尝试寻找最近可达点。
    /// </summary>
    /// <param name="targetPos">目标位置</param>
    public void MoveTo(Vector3 targetPos)
    {
        if(prePos == targetPos)
        {
            return;
        }
        prePos = targetPos;
        if (!NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            Debug.Log("目标点无效或被阻挡，尝试寻找附近的可达点...");
            Logger.Print(this.agent.transform.position,targetPos);
            targetPos = FindNearestReachablePoint(targetPos);
        }
        agent.SetDestination(targetPos);
        isMoving = true;
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    public void StopMove()
    {
        if (agent.isActiveAndEnabled)
        {
            agent.ResetPath();
        }
        prePos = Vector3.zero;
        isMoving = false;
    }

    public bool IsMoving()
    {
        return !agent.isStopped && isMoving;
    }
    const int maxAttempts = 30;
    const float searchRadius = 2f;
    const int perRadiusnum = 5;
    /// <summary>
    /// 检查目标点是否完全被阻挡，如果是则返回一个随机的附近点。
    /// </summary>
    /// <param name="originPos">起始点</param>
    /// <returns>可达的随机点</returns>
    private Vector3 FindNearestReachablePoint(Vector3 originPos)
    {
        int num = 0;
        for (int i = 0; i < searchRadius; i++)
        {
            int init = i == 0 ? (perRadiusnum-1) : 0;
            for (int m = init; m < perRadiusnum; m++)
            {
                num++;
                Vector3 randomDirection = originPos + Random.insideUnitSphere * (float)i;
                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            if (num > maxAttempts)
            {
                break;
            }
        }

        Debug.LogError("未能找到可达的替代点，返回原始点！");
        return originPos;
    }

    // 检测是否到达目的地
    public void UpdateMove()
    {
        if (isMoving && hasAudio)
        {
            checkTimer += Time.deltaTime;
            if (checkTimer >= AudioInv)
            {
                checkTimer = 0f;
                AudioMgr.Instance.Play(AudioEnum.Footstep);
            }
        }
        if (isMoving && agent.remainingDistance <= agent.stoppingDistance)
        {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                isMoving = false;
                OnReachedDestination();
            }
        }
    }

    /// <summary>
    /// 当到达目的地时触发的事件
    /// </summary>
    private void OnReachedDestination()
    {
       //Debug.Log("到达目标位置！");
    }

    public bool HasArrived(float offt)
    {
        if (!agent.pathPending)
        {
            if (offt > 0)
            {
                return agent.remainingDistance <= (agent.stoppingDistance + offt);
            }
            else
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
