using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private int _column=2;
    [SerializeField] private int _row=2;
    private int _currentcolumn=0, _currentRow=0;

    [SerializeField] private GameObject _wallPrefab;
    [SerializeField] private Button _onClickGenerate;
    [SerializeField] private Camera _cam;
    [SerializeField] private InputField _widthInput;
    [SerializeField] private InputField _heightInput;

    private MazeGrid[,] _mazegrid;

    private bool _scanComplete=false;
    
    private float _duration=3.0f;
    
    void Start()
    {
        _cam = GameObject.Find("Main Camera").GetComponent<Camera>();
       
        GenerateGrid();
        _onClickGenerate.onClick.AddListener(OnClickRegenerate);
        CameraMovement();
    }

    private void Update()
    {
        ChangeColorBackground();
    }

    public void CameraMovement()
    {
        float size = _wallPrefab.transform.localScale.x;

        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition.x = Mathf.Round(_column / 1.5f) * size;
        cameraPosition.y = Mathf.Max(13, Mathf.Max(_row, _column) * 6.5f);
        cameraPosition.z = Mathf.Round(-_row * 2.5f) * size;
        Camera.main.transform.position = cameraPosition;
    }

    private void ChangeColorBackground()
    {
        if (_cam != null)
        {
            float time = Mathf.PingPong(Time.time, _duration) / _duration;
            _cam.backgroundColor = Color.Lerp(Color.red, Color.blue, time);
        }
    }

    private void MakeGrid()
    {
        float size = _wallPrefab.transform.localScale.x;
        _mazegrid = new MazeGrid[_row, _column];

        for (int rows = 0; rows < _row; rows++)
        {
            for (int columns = 0; columns < _column; columns++)
            {   
                //GameObject floorWall = Instantiate(_floorPrefab, new Vector3(columns * size, 0, -rows*size), Quaternion.identity);
                GameObject upWall = Instantiate(_wallPrefab, new Vector3(columns * size, 1.75f, -rows * size+1.25f), Quaternion.identity);
                GameObject downWall = Instantiate(_wallPrefab, new Vector3(columns*size,1.75f,-rows *size-1.25f), Quaternion.identity);
                GameObject leftWall = Instantiate(_wallPrefab, new Vector3(columns * size -1.25f, 1.75f, -rows * size), Quaternion.Euler(0, 90.0f, 0));
                GameObject rightWall = Instantiate(_wallPrefab, new Vector3(columns * size +1.25f, 1.75f, -rows * size), Quaternion.Euler(0, 90.0f, 0));

                _mazegrid[rows, columns] = new MazeGrid();
                _mazegrid[rows,columns]._north = upWall;
                _mazegrid[rows, columns]._south = downWall;
                _mazegrid[rows, columns]._west = leftWall;
                _mazegrid[rows, columns]._east = rightWall;

                //floorWall.transform.parent = transform;
                upWall.transform.parent = transform;
                downWall.transform.parent = transform;
                leftWall.transform.parent = transform;
                rightWall.transform.parent = transform;

                if (rows == 0 && columns == 0)
                {
                    Destroy(leftWall);
                }

                if (rows == _row - 1 && columns == _column - 1)
                {
                    Destroy(rightWall);
                }
            }   
        }
    }

    private void OnClickRegenerate()
    {
        UpdateMaze();
        CameraMovement();
    }

    private void GenerateGrid()
    {
        foreach (Transform element in transform)
        {
            Destroy(element.gameObject);
        }

        MakeGrid();

        _currentcolumn = 0;
        _currentRow = 0;
        _scanComplete = false;
        HuntAndKill();
    }

    private void UpdateMaze()
    {
        int row=2;
        int column=2;

        if (int.TryParse(_heightInput.text, out row))
        {
            _row = Mathf.Max(2,row);

        }
        if (int.TryParse(_widthInput.text, out column))
        {
            _column = Mathf.Max(2, column);
        }

        GenerateGrid();
    }

    private void HuntAndKill()
    {
        _mazegrid[_currentcolumn, _currentRow].cellVisited = true;

        while (!_scanComplete)
        {
            Walk();
            Hunt();
        }
        
    }
    private bool CheckUnvisitedNeighbours()
    {
        if (IsCellUnVisited(_currentRow - 1, _currentcolumn))
        {
            return true;
        }
        if (IsCellUnVisited(_currentRow + 1, _currentcolumn))
        {
            return true;
        }
        if (IsCellUnVisited(_currentRow, _currentcolumn + 1))
        {
            return true;
        }
        if (IsCellUnVisited(_currentRow, _currentcolumn - 1))
        {
            return true;
        }

        return false;
    }

    private bool IsCellUnVisited(int row, int column)
    {
        //boundary check and visisted check
        if (row >= 0 && row < _row && column >= 0 && column < _column && !_mazegrid[row, column].cellVisited)
        {
            return true;
        }
        return false;
    }

    private void Walk()
    {
        while (CheckUnvisitedNeighbours())
        {
            float direction = Random.Range(1f, 4f);

            if (direction == 1)
            {
                if (IsCellUnVisited(_currentRow - 1, _currentcolumn))
                {
                    if (_mazegrid[_currentRow, _currentcolumn]._north)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn]._north);
                    }

                    _currentRow--;
                    _mazegrid[_currentRow, _currentcolumn].cellVisited = true;

                    if (_mazegrid[_currentRow, _currentcolumn]._south)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn]._south);
                    }
                }
            }
            else if (direction == 2)
            {
                if (IsCellUnVisited(_currentRow + 1, _currentcolumn))
                {
                    if (_mazegrid[_currentRow, _currentcolumn]._south)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn]._south);
                    }
                    _currentRow++;
                    _mazegrid[_currentRow, _currentcolumn].cellVisited = true;
                    if (_mazegrid[_currentRow, _currentcolumn]._north)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn]._north);
                    }
                }
            }
            else if (direction == 3)
            {
                if (IsCellUnVisited(_currentRow, _currentcolumn - 1))
                {
                    if (_mazegrid[_currentRow, _currentcolumn]._west)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn]._west);
                    }
                    _currentcolumn--;
                    _mazegrid[_currentRow, _currentcolumn].cellVisited = true;

                    if (_mazegrid[_currentRow, _currentcolumn]._east)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn]._east);
                    }
                }
            }
            else if (direction == 4)
            {
                if (IsCellUnVisited(_currentRow, _currentcolumn + 1))
                {
                    if (_mazegrid[_currentRow, _currentcolumn]._east)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn]._east);
                    }
                    _currentcolumn++;
                    _mazegrid[_currentRow, _currentcolumn].cellVisited = true;
                    if (_mazegrid[_currentRow, _currentcolumn]._west)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn]._west);
                    }
                }
            }
        }
    }

    private bool AreThereVisitedNeighbours(int row, int column)
    {
        if(row > 0 && _mazegrid[row -1, column].cellVisited)
        {
            return true;
        }
        if (row < _row - 1 && _mazegrid[row + 1, column].cellVisited)
        {
            return true;
        }
        if (column > 0 && _mazegrid[row, column-1].cellVisited)
        {
            return true;
        }
        if (column < _column - 1 && _mazegrid[row, column + 1].cellVisited)
        {
            return true;
        }
        return false;
    }

    private void Hunt()
    {
        _scanComplete = true;
        for (int i = 0; i < _row; i++)
        {
            for (int j = 0; j < _column; j++)
            {
                if(!_mazegrid[i,j].cellVisited && AreThereVisitedNeighbours(i, j))
                {
                    _scanComplete = false;
                    _currentRow = i;
                    _currentcolumn = j;
                    _mazegrid[_currentRow, _currentcolumn].cellVisited = true;
                    DestroyAdjacentWall();
                    return;
                }
            }
        }
    }

    private void DestroyAdjacentWall()
    {
        bool destroyed = false;

        while (!destroyed)
        {
            float direction = Random.Range(0, 4);

            if (direction == 0)
            {
                //updated mazegrid
                if(_currentRow > 0 && _mazegrid[_currentRow - 1, _currentcolumn].cellVisited)
                {

                    if (_mazegrid[_currentRow, _currentcolumn]._north)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn]._north);
                    }

                    if (_mazegrid[_currentRow - 1, _currentcolumn]._south)
                    {
                        Destroy(_mazegrid[_currentRow - 1, _currentcolumn]._south);
                    }
                    
                    
                    destroyed = true;
                }
            }
            else if (direction == 1)
            {
                if (_currentRow < _row - 1 && _mazegrid[_currentRow + 1, _currentcolumn].cellVisited)
                {
                    if (_mazegrid[_currentRow, _currentcolumn]._south)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn]._south);
                    }
                    if (_mazegrid[_currentRow + 1, _currentcolumn]._north)
                    {
                        Destroy(_mazegrid[_currentRow + 1, _currentcolumn]._north);
                    }

                    destroyed = true;
                }
            }
            else if (direction == 2)
            {
                if (_currentcolumn > 0 && _mazegrid[_currentRow, _currentcolumn - 1].cellVisited)
                {
                    if (_mazegrid[_currentRow, _currentcolumn]._west)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn]._west);
                    }
                    if (_mazegrid[_currentRow, _currentcolumn - 1]._east)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn - 1]._east);
                    }

                    destroyed = true;
                }
            }
            else if (direction == 3)
            {
                if (_currentcolumn < _column -1 && _mazegrid[_currentRow, _currentcolumn + 1].cellVisited)
                {
                    if (_mazegrid[_currentRow, _currentcolumn]._east)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn]._east);
                    }
                    if (_mazegrid[_currentRow, _currentcolumn + 1]._west)
                    {
                        Destroy(_mazegrid[_currentRow, _currentcolumn + 1]._west);
                    }
                  
                    destroyed = true;
                }
            }
        }
    }
}
