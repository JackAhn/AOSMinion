using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team2Manager : MonoBehaviour
{

    private MinionManager instance;
    private int team2Flag = 0; //Team2 미니언 -> 짝수 숫자로 보기 쉽게 구분, 굳이 필요 없어서 수정 가능
    private GameObject team2Obj;

    // Start is called before the first frame update
    void Start()
    {
        instance = MinionManager.instance;
        team2Obj = GameObject.Find("Team2");
        StartCoroutine(generateMinion());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator generateMinion() //미니언 동적 생성
    {
        while (!instance.isGameEnd)
        {
            float objX = -2f;

            for (int i = 0; i < 2; i++)
            {
                GameObject minion = instance.NearMinion;
                minion.name = "Team2NearMinion" + team2Flag;
                Instantiate(minion, new Vector3(objX, team2Obj.transform.position.y, team2Obj.transform.position.z), Quaternion.Euler(0, 0, 0));
                //minion.transform.Find("Sword01Free").name = "Team2Sword" + team2Flag;
                team2Flag += 2;
                objX += 3f;
            }
            yield return new WaitForSeconds(1f);

            objX = -2f;

            for (int i = 0; i < 2; i++)
            {
                GameObject minion = instance.LongDistanceMinion;
                minion.name = "Team2LongDistanceMinion" + team2Flag;
                Instantiate(minion, new Vector3(objX, team2Obj.transform.position.y, team2Obj.transform.position.z), Quaternion.Euler(0, 0, 0));
                team2Flag += 2;
                objX += 3f;
            }

            yield return new WaitForSeconds(1f);

            yield return new WaitForSeconds(14f);
        }
    }
}
