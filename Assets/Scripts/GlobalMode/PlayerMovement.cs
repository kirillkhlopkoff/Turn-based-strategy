using UnityEngine;
using System.Collections.Generic;

public class General : MonoBehaviour
{
    public string team; // ������� ���������� (��������, "Team1" ��� "Team2")
}

public class PlayerMovement : MonoBehaviour
{
    public Transform parentObject; // ������������ ������, ���������� ��� �����
    public float speed = 5f; // �������� �����������

    private Transform targetPoint; // ������� �����
    private bool isMoving = false; // ����, ����������� �� ��, ��� ����� � ��������
    private Dictionary<string, List<string>> neighbors; // ������� �������� �����
    private Transform[] points; // ������ �����
    private General selectedGeneral; // ��������� ����������
    private List<General> team1Generals; // ���������� ������� 1
    private List<General> team2Generals; // ���������� ������� 2
    private string currentTeam = "Team1"; // ������� �������

    void Start()
    {
        // �������� ��� ����� �� ������������� �������
        points = parentObject.GetComponentsInChildren<Transform>();

        // �������������� ������� �������� �����
        InitializeNeighbors();

        // �������������� ����������� ������
        InitializeGenerals();

        // ������������� ��������� ��������� ����������� �� ��������� �����
        foreach (var general in team1Generals)
        {
            general.transform.position = points[1].position;
        }
        foreach (var general in team2Generals)
        {
            general.transform.position = points[2].position;
        }
    }

    void Update()
    {
        // ���������, ��� �� ������ ���� ����
        if (Input.GetMouseButtonDown(0))
        {
            // �������� ������� �����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ���������, �� ����� ����� ��� ������ ����
                string clickedPointName = hit.transform.name;
                string currentPointName = GetClosestPointName(transform.position);

                if (neighbors.ContainsKey(currentPointName) && neighbors[currentPointName].Contains(clickedPointName))
                {
                    targetPoint = hit.transform;
                    isMoving = true;
                }
            }
        }

        // ���� ����� ������ ������������, ��������� ��������
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        // ���������� ������ � ������� �����
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // ���� ����� ������ ������� �����, ������������� ��������
        if (transform.position == targetPoint.position)
        {
            isMoving = false;
        }
    }

    void InitializeNeighbors()
    {
        neighbors = new Dictionary<string, List<string>>();

        // ��������� ������� �������� �����
        neighbors["01"] = new List<string> { "02", "04" };
        neighbors["02"] = new List<string> { "01", "03" };
        neighbors["03"] = new List<string> { "02", "06" };
        neighbors["04"] = new List<string> { "01", "07", "21", "25" };
        neighbors["05"] = new List<string> { };
        neighbors["06"] = new List<string> { "12", "13", "14" };
        neighbors["07"] = new List<string> { "04", "09", "19", "20" };
        neighbors["08"] = new List<string> { };
        neighbors["09"] = new List<string> { "07", "10", "11", "19" };
        neighbors["10"] = new List<string> { "01", "07", "21", "25" };
    }

    void InitializeGenerals()
    {
        team1Generals = new List<General>();
        team2Generals = new List<General>();

        // ������� ��� ������� � ����������� General � ������������ �� �� ��������
        General[] allGenerals = FindObjectsOfType<General>();
        foreach (var general in allGenerals)
        {
            if (general.team == "Team1")
            {
                team1Generals.Add(general);
            }
            else if (general.team == "Team2")
            {
                team2Generals.Add(general);
            }
        }
    }

    string GetClosestPointName(Vector3 position)
    {
        float minDistance = Mathf.Infinity;
        string closestPointName = "";

        foreach (Transform point in points)
        {
            if (point == parentObject) continue; // ���������� ������������ ������

            float distance = Vector3.Distance(position, point.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPointName = point.name;
            }
        }

        return closestPointName;
    }

    public void EndTurn()
    {
        currentTeam = (currentTeam == "Team1") ? "Team2" : "Team1";
        Debug.Log("������ ��� �������: " + currentTeam);
    }

    public void SelectGeneral(General general)
    {
        if (general.team == currentTeam)
        {
            selectedGeneral = general;
            Debug.Log("������ ���������� ������� " + currentTeam);
        }
        else
        {
            Debug.Log("������ �� ��� ���� �������");
        }
    }

    void OnGUI()
    {
        // ������ ��� ���������� ����
        if (GUI.Button(new Rect(10, 10, 150, 30), "����� ����"))
        {
            EndTurn();
        }
    }
}


//using UnityEngine;

//public class PlayerMovement : MonoBehaviour
//{
//    public Transform point1; // ����� 1
//    public Transform point2; // ����� 2
//    public float speed = 5f; // �������� �����������

//    private Transform targetPoint; // ������� �����
//    private bool isMoving = false; // ����, ����������� �� ��, ��� ����� � ��������

//    void Start()
//    {
//        // ������������� ��������� ��������� ������ �� ����� 1
//        transform.position = point1.position;
//    }

//    void Update()
//    {
//        // ���������, ��� �� ������ ���� ����
//        if (Input.GetMouseButtonDown(0))
//        {
//            // �������� ������� �����
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;

//            if (Physics.Raycast(ray, out hit))
//            {
//                // ���������, �� ����� ����� ��� ������ ����
//                if (hit.transform == point1)
//                {
//                    targetPoint = point1;
//                    isMoving = true;
//                }
//                else if (hit.transform == point2)
//                {
//                    targetPoint = point2;
//                    isMoving = true;
//                }
//            }
//        }

//        // ���� ����� ������ ������������, ��������� ��������
//        if (isMoving)
//        {
//            MoveToTarget();
//        }
//    }

//    void MoveToTarget()
//    {
//        // ���������� ������ � ������� �����
//        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

//        // ���� ����� ������ ������� �����, ������������� ��������
//        if (transform.position == targetPoint.position)
//        {
//            isMoving = false;
//        }
//    }
//}
