using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackGenerator : MonoBehaviour
{
    public GameObject shooter; //투사체 GameObject
    private GameObject shooterobj;
    private GameObject minion; //미니언 Object
    private NavMeshAgent agent;

    private float smooth = 10f;

    public void makeObject() //원거리 투사체 생성
    {
        minion = this.gameObject.transform.parent.gameObject;
        agent = minion.GetComponent<NavMeshAgent>();
        if (minion.name.Contains("Team1"))
        {
            shooterobj = Instantiate(shooter, minion.transform.position, Quaternion.Euler(0, 180f, 0));
            shooterobj.name = "Team1Projectile" + minion.name.Substring(22);
        }
        else if (minion.name.Contains("Team2"))
        {
            shooterobj = Instantiate(shooter, minion.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            shooterobj.name = "Team2Projectile" + minion.name.Substring(22);
        }
        StartCoroutine(moveShooter());
    }

    IEnumerator moveShooter()
    {
        while (true)
        {
            if (shooterobj == null || agent == null)
                break;

            if (shooterobj.transform.position == agent.destination)
            {
                //Debug.Log("목표 도착");
                break;
            }

            shooterobj.gameObject.transform.position = Vector3.Lerp(shooterobj.transform.position, agent.destination, Time.deltaTime * smooth);
            yield return null;
        }

        GameObject obj = GameObject.Find(shooterobj.name);
        if(obj)
            Destroy(shooterobj.gameObject);
        yield return null;
    }
}
