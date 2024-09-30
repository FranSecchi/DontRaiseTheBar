using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using System.IO;
using UnityEngine.TestTools;

public abstract class CellularAutomataLiquid : MonoBehaviour
{
    public Minigame menu;
    public GameObject button;
    public bool gizmos = false;
    public Sprite gridSprite;
    protected int gridWidth = 10;
    protected int gridHeight = 10;
    public int winRowStart = 10;
    public int winRowEnd = 10;
    public float cellSize = 1f;
    public float updateInterval = 50f;
    public float timeToFinish = 2f;
    public int fallSpeed;
    public int fillSpeed;
    public int foamSpeed = 2;
    public int foamDecreaseSpeed = 2;
    public float removalinterval = 0.1f;
    public int moveRate;
    public int halfIce;
    public GameObject liquidPrefab;
    public List<Vector2Int> initialLiquidCells;
    //private List<Vector2Int> initialLiquidCells;

    private EventInstance instance;

    protected LiquidCell[,] grid;
    protected LiquidCell[,] tempGrid;
    protected GameObject[,] cellObjects;
    protected bool[,] gridMask;
    private bool temp = false;
    protected int bottomRow;
    protected int fillTopRow;
    protected int foamTopRow;
    protected bool pressingSpace = false;
    private float updateTime = 0f;
    private bool start = false; 
    private bool spacePressedOnce = false;
    private Coroutine endCroroutine = null;
    private Coroutine countdownCoroutine = null;
    #region Gizmos
    void OnDrawGizmos()
    {
        if (gizmos)
        {
            if (!temp)
            {
                temp = true;
                LoadGridMask();
            }
            Gizmos.color = Color.blue;

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (gridMask[x, y])
                    {
                        if(y == winRowStart || y == winRowEnd || y == halfIce)
                        {
                            Gizmos.color = Color.magenta;
                        }
                        else if (tempGrid != null && tempGrid[x, y].isIce)
                        {
                            Gizmos.color = Color.cyan;
                        }
                        else if (tempGrid != null && tempGrid[x, y].isStatic)
                        {
                            Gizmos.color = Color.green;
                            if (y == fillTopRow)
                                Gizmos.color = Color.black;
                        }
                        else if (tempGrid != null && tempGrid[x, y].isFoam)
                        {
                            Gizmos.color = Color.gray;
                        }
                        else if (tempGrid != null && tempGrid[x, y].isOccupied && !tempGrid[x, y].isStatic)
                        {
                            Gizmos.color = Color.black;
                        }
                        else
                            Gizmos.color = Color.blue;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
                    Gizmos.DrawCube(new Vector3(x * cellSize * transform.parent.localScale.x/100 + transform.position.x, y * cellSize * transform.parent.localScale.x / 100 + transform.position.y, 0), Vector3.one * (cellSize / 100 * 0.9f));
                }
            }
            foreach (Vector2Int cell in initialLiquidCells)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(new Vector3(cell.x * cellSize * transform.parent.localScale.x / 100 + transform.position.x, cell.y * cellSize * transform.parent.localScale.x / 100 + transform.position.y, 0), Vector3.one * (cellSize / 100 * 0.9f));
            }
        }
    }
    #endregion

    void Start()
    {
        LoadGridMask();
        InitializeGrid();
        //InvokeRepeating(nameof(UpdateSimulation), 0f, updateInterval);
    }
    private void Update()
    {
        //CheckResetGrid();
        CheckInput();
        updateTime += Time.deltaTime;
        if (updateTime > updateInterval / 100f)
        {
            UpdateSimulation();
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !spacePressedOnce)
        {
            instance = AudioManager.instance.CreateEventInstance(FMODEvents.instance.liquid_pour);
            instance.start();
            button.SetActive(false);
            spacePressedOnce = true; 
            pressingSpace = true;    
            start = true;            
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && pressingSpace)
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
            pressingSpace = false;
            button.SetActive(true);
            Debug.Log("End");
            if (countdownCoroutine == null)
            {
                countdownCoroutine = StartCoroutine(CountdownTimer()); // Start the countdown timer
            }
        }

        if (start && pressingSpace)
        {
            menu.WriteTime();
            ActivateCells();
        }
    }

    private void CheckResetGrid()
    {
        if (Input.GetKey(KeyCode.R))
        {
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (gridMask[x, y])
                    {
                        grid[x, y] = new LiquidCell(false, false, false, false, false);
                        cellObjects[x, y].SetActive(false);
                    }
                }
            }
            updateTime = 0f;
            fillTopRow = bottomRow;
            foamTopRow = bottomRow;
            spacePressedOnce = false;
        }
    }
    void LoadGridMask()
    {
        if (gridSprite == null)
        {
            Debug.LogError("Sprite missing.");
            return;
        }
        gridWidth = gridSprite.texture.width;
        gridHeight = gridSprite.texture.height;
        //initialLiquidCells = new List<Vector2Int>();
        Texture2D texture = gridSprite.texture;
        gridMask = new bool[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                float u = (float)x / (gridWidth - 1);
                float v = (float)y / (gridHeight - 1);

                Color pixelColor = texture.GetPixelBilinear(u, v);
                //if(pixelColor.b < 0.5f && pixelColor.r > 0.5f)
                //{
                //    initialLiquidCells.Add(new Vector2Int(x, y));
                //}
                gridMask[x, y] = pixelColor.grayscale > 0.5f;
            }
        }
    }
    void InitializeGrid()
    {
        grid = new LiquidCell[gridWidth, gridHeight];
        cellObjects = new GameObject[gridWidth, gridHeight];
        bottomRow = -1;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (gridMask[x, y])
                {
                    if (bottomRow < 0)
                    {
                        fillTopRow = y;
                        foamTopRow = y;
                        bottomRow = y;
                    }
                    grid[x, y] = new LiquidCell(false, false, false, false, false);
                    InitMask(x, y);

                    Vector3 position = new Vector3(x * cellSize * transform.parent.localScale.x / 100 + transform.position.x, y * cellSize * transform.parent.localScale.x / 100 + transform.position.y, 0);
                    cellObjects[x, y] = Instantiate(liquidPrefab, position, Quaternion.identity, transform);
                    cellObjects[x, y].SetActive(false);
                }
            }
        }
    }
    void UpdateSimulation()
    {
        updateTime = 0f;

        tempGrid = new LiquidCell[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridMask[x, y]) tempGrid[x, y] = new LiquidCell(grid[x, y].isOccupied, grid[x, y].isStatic, grid[x, y].isFoam, grid[x, y].isMilk, grid[x, y].isIce);
            }
        }

        SetFlow();


        // Update grid
        UpdateGrid();
    }

    private void SetFlow()
    {

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if(gridMask[x, y] && !grid[x, y].isOccupied && y < fillTopRow)
                {
                    tempGrid[x, y].isOccupied = true;
                    tempGrid[x, y].isStatic = true;
                }
                else if (gridMask[x, y] && y > 0 && grid[x, y].isOccupied && !grid[x, y].isStatic) // Filled cell to check
                {
                    if (gridMask[x, y - 1] && !tempGrid[x, y - 1].isOccupied)// Cell below not filled
                    {
                        SetVerticalFlow(x, y);
                    }
                    else if (y - fillTopRow < fillSpeed)
                    {
                        tempGrid[x, y].isStatic = true;
                        CheckNeighbours(x, y);
                        if (tempGrid[x, y].isStatic && y > fillTopRow) // Move surface up
                        {
                            fillTopRow = y;
                            IncreaseSurface(x, y);
                        }
                    }
                }
            }
        }
    }

    protected abstract void IncreaseSurface(int x, int y);

    protected void SetVerticalFlow(int x, int y)
    {
        bool hasEmptyAbove = false;
        if (x <= initialLiquidCells[initialLiquidCells.Count - 1].x && x >= initialLiquidCells[0].x)
        {
            for (int i = 1; y + i < gridHeight && gridMask[x, y + i] && i < fallSpeed; i++)//Check fast fall
            {
                if (!tempGrid[x, y + i].isOccupied)
                {
                    hasEmptyAbove = true;
                    break;
                }
            }
        }
        for (int i = 1; y - i > 0 && gridMask[x, y - i] && !tempGrid[x, y - i].isStatic && i <= fallSpeed; i++) // Apply fast fall
        {
            if (!tempGrid[x, y - i].isOccupied)
            {
                SetVertical(x, y, i, hasEmptyAbove);
            }
        }
    }

    protected void CheckNeighbours(int x, int y)
    {
        LiquidCell cell = grid[x, y];

        if (cell.directionChangeCounter <= 0)
        {
            cell.moveLeft = Random.value < 0.5f;
            cell.directionChangeCounter = moveRate;
        }
        else
        {
            cell.directionChangeCounter--;
        }

        // Try to flow in the chosen direction
        int direction = cell.moveLeft ? -1 : 1;
        if (!TryFlow(x, y, direction))
        {
            // try the opposite direction
            cell.moveLeft = !cell.moveLeft;
            TryFlow(x, y, -direction);
        }
    }

    private bool TryFlow(int x, int y, int direction)
    {
        for (int i = 1; ; i++)
        {
            int newX = x + i * direction;

            if (newX < 0 || newX >= gridWidth || !gridMask[newX, y])
            {
                return false;
            }

            // Attempt to set horizontal flow
            if (SetHorizontalFlow(x, y, newX - x))
            {
                return true;
            }
        }
    }

    private bool SetHorizontalFlow(int x, int y, int flow)
    {
        int targetX = x + flow;

        if (!tempGrid[targetX, y].isOccupied)
        {
            SetHorizontal(targetX, x, y);
            return true;
        }

        return false;
    }
    protected abstract void InitMask(int x, int y);
    protected abstract void ActivateCells();
    protected abstract void SetHorizontal(int targetX, int x, int y);
    protected abstract void SetVertical(int x, int y, int i, bool hasEmptyAbove);
    protected abstract void UpdateGrid();

    private IEnumerator RemoveLostCells()
    {
        int y = fillTopRow + 1;
        if (y < gridHeight)
        {
            while (true)
            {
                bool anyOccupied = false;

                for (int x = 0; x < gridWidth; x++)
                {
                    if (gridMask[x, y] && grid[x, y].isOccupied)
                    {
                        grid[x, y].isOccupied = false;
                        cellObjects[x, y].SetActive(false);
                        anyOccupied = true;

                        yield return new WaitForSeconds(removalinterval);
                    }
                }

                if (!anyOccupied)
                {
                    break;
                }
            }
        }
    }
    private IEnumerator CountdownTimer()
    {
        float timeRemaining = timeToFinish;
        Debug.Log("Time's up!");
        if (endCroroutine == null)
            endCroroutine = StartCoroutine(RemoveLostCells());
        // Countdown to 1 second
        while (timeRemaining > 0)
        {
            //if (pressingSpace)
            //{
            //    countdownCoroutine = null;
            //    endCroroutine = null;
            //    yield break;
            //}
            // Calculate time left
            float timeLeft = timeToFinish - timeRemaining;

            // Log the countdown if time is within the last 3 seconds
            if (timeLeft <= 3 && timeLeft > 2)
            {
                Debug.Log("3");
            }
            else if (timeLeft <= 2 && timeLeft > 1)
            {
                Debug.Log("2");
            }
            else if (timeLeft <= 1 && timeLeft > 0)
            {
                Debug.Log("1");
            }

            // Wait for 1 second before checking again
            yield return new WaitForSeconds(1f);

            // Update remaining time
            timeRemaining -= 1f;
        }
        End();
        int plus = 0;
        string s = "";
        if (fillTopRow >= winRowStart && fillTopRow <= winRowEnd)
        {
            plus += 10;
            s = "+" + plus + "!!";
            GameManager.instance.lastNpcHappy = true;
        }
        else
        {
            plus -= (Mathf.Abs(fillTopRow - ((winRowEnd - winRowStart) / 2 + winRowStart)));
            s = "+" + 0 + "...";
            GameManager.instance.lastNpcHappy = false;
        }

        if (plus > 0)
        {
            GameManager.instance.score += plus;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.sucess, new Vector3(0,0,0));
        }
        else
        {
            GameManager.instance.lifes--;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.glass, new Vector3(0,0,0));
            AudioManager.instance.PlayOneShot(FMODEvents.instance.fail, new Vector3(0,0,0));
        }
        
            // Debug log the results
            menu.WriteText(s);
        // Ensure logging is done after countdown ends
    }
    protected abstract void End();
}
/*
 * // Downward flow
                    if(!gridMask[x, y - 1])
                    {
                        newGrid[x, y].isStatic = true;
                    }
                    else if (!newGrid[x, y - 1].isOccupied)
                    {
                        newGrid[x, y - 1].isOccupied = true;
                        newGrid[x, y].isOccupied = false;
                        if (y - 1 <= 0)
                        {
                            newGrid[x, y - 1].isStatic = true;
                        }
                    }
                    if(gridMask[x, y - 1] && newGrid[x, y - 1].isStatic)
                    {
                        newGrid[x, y].isStatic = true;
                        //Debug.Log("static: " + x + ", " + y);
                    }
                    // Horizontal flowint emptyNeighbors = 0;
                    int emptyNeighbors = 0;
                    if (x > 0 && gridMask[x - 1, y] && !newGrid[x - 1, y].isOccupied) emptyNeighbors++;
                    if (x < gridWidth - 1 && gridMask[x + 1, y] && !newGrid[x + 1, y].isOccupied) emptyNeighbors++;
                    if (emptyNeighbors > 0 && newGrid[x,y].isStatic)
                    {
                        if (x > 0 && gridMask[x - 1, y] && !newGrid[x - 1, y].isOccupied)
                        {
                            newGrid[x - 1, y].isOccupied = true;
                            newGrid[x, y].isOccupied = false;
                            newGrid[x, y].isStatic = false;
                        }
                        else if (x < gridWidth - 1 && gridMask[x + 1, y] && !newGrid[x + 1, y].isOccupied)
                        {
                            newGrid[x + 1, y].isOccupied = true;
                            newGrid[x, y].isOccupied = false;
                            newGrid[x, y].isStatic = false;
                        }
                    }
 */

