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

    private int NearMinionAttack = 30; //근거리 미니언 공격력
    private int LongDistanceMinionAttack = 45; //원거리 미니언 공격력
    private int selectAttack; //미니언 종류에 따른 공격력 설정

    private int minionHP;
    private int currentHP;
    private bool isDie = false;

    private Animator animator;
    private Transform target; //챔피언 거리
    private GameObject minionobj;
    private NavMeshAgent agent;

    public event Action<float> OnHealthPercentChanged = delegate { };


    void Awake()
    {
        animator = this.gameObject.GetComponentInChildren<Animator>();
        target = PlayerManager.instance.player.transform;
        agent = this.gameObject.GetComponent<NavMeshAgent>();

        if (transform.name.Contains("NearMinion"))
        {
            minionHP = 220;
            //minionHP = 10; //테스트용 HP
            selectattackRadius = attackNearRadius;
            selectAttack = NearMinionAttack;
        }


        else if (transform.name.Contains("LongDistanceMinion"))
        {
            minionHP = 130;
            selectattackRadius = attackLongRadius;
            selectAttack = LongDistanceMinionAttack;
        }

        currentHP = minionHP;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(checkState());
        StartCoroutine(minionAction());
    }

    void Update()
    {
        /*if (transform.name.Contains("Team1"))
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.z + 180f, transform.rotation.z));
        else
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, transform.rotation.z));*/
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
                if (allobjects[i].name.Contains("Minion")) //미니언만 대소 비교를 함
                {

                    if (allobjects[i].name.Substring(0, 5).Equals(transform.name.Substring(0, 5))) //같은 팀 미니언이라면 거리 대소비교 안함
                    {
                        continue;
                    }

                    else
                    {
                        float distance = Vector3.Distance(transform.GetChild(0).GetChild(0).transform.position, allobjects[i].transform.GetChild(0).GetChild(0).position); //챔피언 - 미니언 간 거리 계산

                        if(minDistance > distance && distance != 0)
                        {
                            minDistance = distance;
                            minionobj = allobjects[i].gameObject;
                        }

                    }

                }

            }


            if (champdistance <= minDistance && champdistance <= 5f) //챔피언 거리가 가깝다면
            {
                agent.isStopped = false;
                agent.SetDestination(target.transform.position);
                setMinionRotation(target.gameObject);

                if (champdistance <= selectattackRadius)
                {
                    agent.isStopped = true;
                    if (selectattackRadius == attackLongRadius)
                        minionState = MinionState.longattack;
                    else
                        minionState = MinionState.nearattack;
                }
            }

            else if (minDistance <= selectattackRadius) //미니언 거리가 가깝다면
            {
                agent.isStopped = false;
                agent.SetDestination(minionobj.transform.position);
                setMinionRotation(minionobj);

                if (selectattackRadius == attackLongRadius)
                {
                    agent.isStopped = true;
                    minionState = MinionState.longattack;
                }
                else
                {
                    agent.isStopped = true;
                    minionState = MinionState.nearattack;
                }
            }

            else //아무 것도 아니라면 이동
            {
                agent.isStopped = false;
                agent.SetDestination(minionobj.transform.position);
                setMinionRotation(minionobj);

                minionState = MinionState.move;
            }

            yield return new WaitForSeconds(0.2f);
        }

        yield return null;
 
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.name.Contains("Sword"))
        {
            if (transform.name.Substring(0, 5).Equals(other.transform.root.name.Substring(0, 5)))
            {
                return;
            }
            currentHP -= selectAttack;
            float currentHealthPercent = (float)currentHP / (float)minionHP;
            OnHealthPercentChanged(currentHealthPercent);
        }

        else if (other.name.Contains("Projectile"))
        {
            if (transform.name.Substring(0, 5).Equals(other.transform.root.name.Substring(0, 5)))
            {
                return;
            }

            currentHP -= selectAttack;
            float currentHealthPercent = (float)currentHP / (float)minionHP;
            OnHealthPercentChanged(currentHealthPercent);
        }

        if (currentHP <= 0)
        {
            Debug.Log("minion die");
            agent.isStopped = true;
            minionState = MinionState.die;
        }

    }

    IEnumerator minionAction() //미니언 애니메이션 설정
    {
        while (!isDie)
        {
            switch (minionState)
            {
                case MinionState.move:

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
                        animator.SetTrigger("LongAttack");
                    }
                    break;

                case MinionState.die:

                    if (animator)
                    {
                        animator.SetTrigger("IsDie");
                        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
                        isDie = true;
                    }
                    break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        while (true)
        {
            GameObject obj = GameObject.Find(gameObject.name.Substring(22));
            if (obj)
                Destroy(obj);
            else
                break;
        }
        Destroy(gameObject);
        yield return null;
    }

    IEnumerator WaitForAnimation(Animation animation)
    {
        Debug.Log(1);
        do
        {
            yield return null;
        } while (animation.isPlaying);
        yield return null;
    }

    private void setMinionRotation(GameObject obj) //미니언 방향 설정
    {
        float smooth = 10f;

        Vector3 direction = (obj.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * smooth);

        /*if (transform.name.Contains("Team1")) //Team1인 경우 y축 180도 회전해야 함.
        {
            var newRotation = new Vector3(obj.transform.position.x, 180f, obj.transform.position.y);
            transform.rotation = Quaternion.LookRotation(agent, velocity.normalized);
        }
        else if(transform.name.Contains("Team2"))
        {
            var newRotation = obj.transform.position;
            transform.rotation = Quaternion.LookRotation()
        }*/

    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
