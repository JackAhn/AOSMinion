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
    private float attackNearRadius = 1.5f; //근거리 공격 거리
    private float attackLongRadius = 4f; //원거리 공격 거리
    private float selectattackRadius; //미니언 종류에 따른 공격 거리 설정
    private int minionHP = 100;
    private float generateMinionZ;
    private bool isDie = false;

    private Animator animator;
    private Transform target; //챔피언 거리
    private GameObject nearMinion; //근거리 미니언 GameObject
    private GameObject longDistanceMinion; //원거리 미니언 GameObject
    //private NavMeshAgent agent;
    //private Vector3 objectPosition;

    // Start is called before the first frame update
    void Start()
    {
        animator = this.gameObject.GetComponentInChildren<Animator>();
        target = PlayerManager.instance.player.transform;
        //agent = this.gameObject.GetComponent<NavMeshAgent>();
        nearMinion = MinionManager.instance.NearMinion;
        longDistanceMinion = MinionManager.instance.LongDistanceMinion;
        StartCoroutine(checkState());
        StartCoroutine(minionAction());
        if (transform.name.Contains("NearMinion")) { 
            selectattackRadius = attackNearRadius;
        }
        else if (transform.name.Contains("LongDistanceMinion"))
        {
            selectattackRadius = attackLongRadius;
        }
    }
 

    // Update is called once per frame
    void Update()
    {
        /*float distance = Vector3.Distance(target.position, transform.position); //챔피언과 미니언 간의 거리 구하기
        
        float distance2 = Vector3.Distance() //미니언과 미니언 간의 거리 구하기
        

        if(distance <= lookRadius)
        {
            animator.SetBool("IsWalk", true);
            agent.SetDestination(target.position);
        }
        else
        {
            animator.SetBool("IsWalk", false);
        }*/
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
                    //string[] splitString = allobjects[i].name.Split(new string[]{ "Minion" }, StringSplitOptions.None);
                    //string[] splitString2 = transform.name.Split(new string[] { "Minion" }, StringSplitOptions.None);
                    //int num1 = 0, num2 = 0;
                    if (allobjects[i].name.Substring(0, 5).Equals(transform.name.Substring(0, 5)))
                    {
                        //Debug.Log(allobjects[i].name.Substring(0, 4) + " and " + transform.name.Substring(0, 4));
                        Debug.Log(allobjects[i].name + " and " + transform.name);
                        continue;
                    }
                    else
                    {
                        //objectPosition = allobjects[i].transform.position;
                        minDistance = distance;
                        Debug.Log(allobjects[i].name + " and " + transform.name + " distance = " + minDistance);
                    }
                }
            }

            if (champdistance <= lookRadius) //챔피언 거리가 가깝다면
            {
                minDistance = target.position.z;
                if(champdistance <= selectattackRadius)
                {
                    if (selectattackRadius == attackLongRadius)
                        minionState = MinionState.longattack;
                    else
                        minionState = MinionState.nearattack;
                }
                else
                {
                    minionState = MinionState.move;
                }
            }
            else if (minDistance <= selectattackRadius)
            {
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
                minionState = MinionState.move;
            }
            yield return new WaitForSeconds(0.2f);
        }
 
    }

    IEnumerator minionAction() //미니언 애니메이션 설정
    {
        while (!isDie)
        {
            switch (minionState)
            {
                case MinionState.move:
                    transform.Translate(new Vector3(0, 0, 2f) * Time.deltaTime);
                    if (animator)
                    {
                        animator.SetBool("IsWalk", true);
                    }
                    break;
                case MinionState.nearattack:
                    if (animator)
                    {
                        animator.SetBool("IsWalk", false);
                        animator.SetTrigger("NearAttack");
                    }
                    break;
                case MinionState.longattack:
                    if (animator)
                    {
                        animator.SetBool("IsWalk", false);
                        animator.SetTrigger("NearAttack");
                    }
                    break;
            }
            yield return null;
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
