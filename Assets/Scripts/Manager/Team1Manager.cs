using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team1Manager : MonoBehaviour
{

    private MinionManager instance;
    private int team1Flag = 1; //Team1 미니언 -> 홀수 숫자로 구분

    // Start is called before the first frame update
    void Start()
    {
        instance = MinionManager.instance;
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
                minion.name = "Team1NearMinion" + team1Flag;
                Instantiate(minion, new Vector3(objX, 0, 15f), Quaternion.Euler(0, 180f, 0));
                team1Flag += 2;
                objX += 3f;
            }
            yield return new WaitForSeconds(1f);

            objX = -2f;

            for (int i = 0; i < 2; i++)
            {
                GameObject minion = instance.LongDistanceMinion;
                minion.name = "Team1LongDistanceMinion" + team1Flag;
                Instantiate(minion, new Vector3(objX, 0, 15f), Quaternion.Euler(0, 180f, 0));
                team1Flag += 2;
                objX += 3f;
            }
            yield return new WaitForSeconds(1f);

            yield return new WaitForSeconds(14f);
        }
    }
}
