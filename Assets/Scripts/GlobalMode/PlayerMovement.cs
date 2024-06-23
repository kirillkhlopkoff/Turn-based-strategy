using UnityEngine;
using System.Collections.Generic;

public class General : MonoBehaviour
{
    public string team; // Команда полководца (например, "Team1" или "Team2")
}

public class PlayerMovement : MonoBehaviour
{
    public Transform parentObject; // Родительский объект, содержащий все точки
    public float speed = 5f; // Скорость перемещения

    private Transform targetPoint; // Целевая точка
    private bool isMoving = false; // Флаг, указывающий на то, что игрок в движении
    private Dictionary<string, List<string>> neighbors; // Словарь соседних точек
    private Transform[] points; // Массив точек
    private General selectedGeneral; // Выбранный полководец
    private List<General> team1Generals; // Полководцы команды 1
    private List<General> team2Generals; // Полководцы команды 2
    private string currentTeam = "Team1"; // Текущая команда

    void Start()
    {
        // Получаем все точки из родительского объекта
        points = parentObject.GetComponentsInChildren<Transform>();

        // Инициализируем словарь соседних точек
        InitializeNeighbors();

        // Инициализируем полководцев команд
        InitializeGenerals();

        // Устанавливаем начальное положение полководцев на стартовые точки
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
        // Проверяем, был ли сделан клик мыши
        if (Input.GetMouseButtonDown(0))
        {
            // Получаем позицию клика
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Проверяем, на какую точку был сделан клик
                string clickedPointName = hit.transform.name;
                string currentPointName = GetClosestPointName(transform.position);

                if (neighbors.ContainsKey(currentPointName) && neighbors[currentPointName].Contains(clickedPointName))
                {
                    targetPoint = hit.transform;
                    isMoving = true;
                }
            }
        }

        // Если игрок должен перемещаться, выполняем движение
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        // Перемещаем игрока к целевой точке
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // Если игрок достиг целевой точки, останавливаем движение
        if (transform.position == targetPoint.position)
        {
            isMoving = false;
        }
    }

    void InitializeNeighbors()
    {
        neighbors = new Dictionary<string, List<string>>();

        // Заполняем словарь соседних точек
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

        // Найдите все объекты с компонентом General и распределите их по командам
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
            if (point == parentObject) continue; // Пропускаем родительский объект

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
        Debug.Log("Теперь ход команды: " + currentTeam);
    }

    public void SelectGeneral(General general)
    {
        if (general.team == currentTeam)
        {
            selectedGeneral = general;
            Debug.Log("Выбран полководец команды " + currentTeam);
        }
        else
        {
            Debug.Log("Сейчас не ход этой команды");
        }
    }

    void OnGUI()
    {
        // Кнопка для завершения хода
        if (GUI.Button(new Rect(10, 10, 150, 30), "Конец хода"))
        {
            EndTurn();
        }
    }
}


//using UnityEngine;

//public class PlayerMovement : MonoBehaviour
//{
//    public Transform point1; // Точка 1
//    public Transform point2; // Точка 2
//    public float speed = 5f; // Скорость перемещения

//    private Transform targetPoint; // Целевая точка
//    private bool isMoving = false; // Флаг, указывающий на то, что игрок в движении

//    void Start()
//    {
//        // Устанавливаем начальное положение игрока на точку 1
//        transform.position = point1.position;
//    }

//    void Update()
//    {
//        // Проверяем, был ли сделан клик мыши
//        if (Input.GetMouseButtonDown(0))
//        {
//            // Получаем позицию клика
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;

//            if (Physics.Raycast(ray, out hit))
//            {
//                // Проверяем, на какую точку был сделан клик
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

//        // Если игрок должен перемещаться, выполняем движение
//        if (isMoving)
//        {
//            MoveToTarget();
//        }
//    }

//    void MoveToTarget()
//    {
//        // Перемещаем игрока к целевой точке
//        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

//        // Если игрок достиг целевой точки, останавливаем движение
//        if (transform.position == targetPoint.position)
//        {
//            isMoving = false;
//        }
//    }
//}
