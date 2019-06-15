using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MinionController : MonoBehaviour
{

    public float lookRadius;

    private enum MinionState { idle, move, longattack, nearattack, die };
    private MinionState minionState = MinionState.idle;

    private float attackNearRadius = 2f; //근거리 공격 거리
    private float attackLongRadius = 4f; //원거리 공격 거리
    private float selectattackRadius; //미니언 종류에 따른 공격 거리 설정
    private int minionHP;
    private int currentHP;
    private bool isDie = false;

    private Animator animator;
    private Transform target; //챔피언 거리
    private GameObject minionobj;
    private NavMeshAgent agent;

    public event Action<float> OnHealthPercentChanged = delegate { };


    // Start is called before the first frame update
    void Start()
    {
        animator = this.gameObject.GetComponentInChildren<Animator>();
        target = PlayerManager.instance.player.transform;
        agent = this.gameObject.GetComponent<NavMeshAgent>();

        StartCoroutine(checkState());
        StartCoroutine(minionAction());

        if (transform.name.Contains("NearMinion")) {
            minionHP = 220;
            selectattackRadius = attackNearRadius;
        }
        else if (transform.name.Contains("LongDistanceMinion"))
        {
            minionHP = 130;
            selectattackRadius = attackLongRadius;
        }
        currentHP = minionHP;
    }
   
    IEnumerator checkState() //미니언 상태 설정
    {
        while (!isDie)
        {
            float champdistance = Vector3.Distance(target.position, transform.position); //챔피언과 미니언 간의 거리 구하기
            Collider[] allobjects = Physics.OverlapSphere(transform.position, 99999f);
            float minDistance = 99999f; //최소 거리 값 저장 변수

            for (int i = 0; i < allobjects.Length; i++)
            {
                float distance = Vector3.Distance(transform.position, allobjects[i].transform.position);
                if (minDistance > distance && distance != 0 && allobjects[i].name.Contains("Minion"))
                {
                    if (allobjects[i].name.Substring(0, 5).Equals(transform.name.Substring(0, 5))) //같은 팀 미니언이라면
                    {
                        continue;
                    }
                    else
                    {
                        minDistance = distance;
                        minionobj = allobjects[i].gameObject;
                    }
                }
            }

            if (champdistance <= lookRadius) //챔피언 거리가 가깝다면
            {
                if(champdistance <= selectattackRadius)
                {
                    agent.isStopped = true;
                    if (selectattackRadius == attackLongRadius)
                        minionState = MinionState.longattack;
                    else
                        minionState = MinionState.nearattack;
                }
                else
                {
                    agent.isStopped = false;
                    agent.SetDestination(target.position);
                    minionState = MinionState.move;
                }

            }
            else if (minDistance <= selectattackRadius)
            {
                agent.isStopped = true;
                agent.SetDestination(minionobj.transform.position);
                if (selectattackRadius == attackLongRadius)
                {
                    minionState = MinionState.longattack;
                }
                else
                {
                    minionState = MinionState.nearattack;
                }
            }
            else //아무 것도 아니라면 이동
            {
                agent.isStopped = false;
                agent.SetDestination(minionobj.transform.position);
                minionState = MinionState.move;
            }
            yield return new WaitForSeconds(0.2f);
        }
 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Sword"))
        {
            if (transform.name.Equals(other.transform.parent.name))
            {
                return;
            }

            currentHP -= 10;
            float currentHealthPercent = (float)currentHP / (float)minionHP;
            OnHealthPercentChanged(currentHealthPercent);
        }

        if(currentHP == 0)
        {
            minionState = MinionState.die;
            agent.isStopped = true;
            isDie = true;
        }

    }

    IEnumerator minionAction() //미니언 애니메이션 설정
    {
        while (!isDie)
        {
            switch (minionState)
            {
                case MinionState.move:
                    //transform.Translate(new Vector3(0, 0, 2f) * Time.deltaTime);
                    if (animator)
                    {
                        animator.SetBool("IsWalk", true);
                    }
                    break;
                case MinionState.nearattack:
                    if (animator)
                    {
                        animator.SetBool("IsWalk", false);
                        animator.SetBool("SetAttack", true);
                    }
                    break;
                case MinionState.longattack:
                    if (animator)
                    {
                        animator.SetBool("IsWalk", false);
                        animator.SetBool("SetAttack", true);
                    }
                    break;
                case MinionState.die:
                    if (animator)
                    {
                        animator.SetBool("SetAttack", false);
                        animator.SetBool("IsDie", true);
                    }
                    break;
            }
            yield return null;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("IsDie"))
        {
            Destroy(gameObject);
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
