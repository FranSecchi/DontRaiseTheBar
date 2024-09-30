using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TestTools;

public class Coffee : CellularAutomataLiquid
{
    public Sprite foamSprite;
    public Sprite coffeeSprite;
    [Range(0f, 1f)]
    public float foamPercent;
    private int foamTime = 0;
    private int foamDTime = 0;
    protected override void ActivateCells()
    {
        foreach (Vector2Int cell in initialLiquidCells)
        {
            if (cell.x >= 0 && cell.x < gridWidth && cell.y >= 0 && cell.y < gridHeight)
            {
                grid[cell.x, cell.y].isOccupied = true;
                cellObjects[cell.x, cell.y].SetActive(true);
            }
        }
    }


    protected override void SetVertical(int x, int y, int i, bool hasEmptyAbove)
    {
        tempGrid[x, y].isOccupied = false;
        tempGrid[x, y].isFoam = (y > fillTopRow && y < foamTopRow);
        tempGrid[x, y - i].isOccupied = !hasEmptyAbove;
        tempGrid[x, y - i].isFoam = (y - i > fillTopRow && y - i < foamTopRow);
    }

    protected override void SetHorizontal(int targetX, int x, int y)
    {
        tempGrid[targetX, y].isOccupied = true;
        tempGrid[x, y].isOccupied = false;
        tempGrid[x, y].isStatic = false;
        tempGrid[x, y].isFoam = false;
        tempGrid[targetX, y].isFoam = false;
    }
    protected override void UpdateGrid()
    {
        if (fillTopRow > bottomRow)
            SetFoam();
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridMask[x, y])
                {
                    grid[x, y].isOccupied = tempGrid[x, y].isOccupied;
                    grid[x, y].isStatic = tempGrid[x, y].isStatic;
                    grid[x, y].isFoam = tempGrid[x, y].isFoam;
                    if (tempGrid[x, y].isFoam && !tempGrid[x, y].isStatic)
                    {
                        cellObjects[x, y].GetComponent<SpriteRenderer>().sprite = foamSprite;
                    }
                    else cellObjects[x, y].GetComponent<SpriteRenderer>().sprite = coffeeSprite;
                    cellObjects[x, y].SetActive(grid[x, y].isOccupied || grid[x, y].isFoam);
                }
            }
        }
    }

    private void SetFoam()
    { 
        if (pressingSpace)
        {
            if (foamTime > foamSpeed && foamTopRow + 1 < gridHeight)
            {
                foamTime = 0;
                foamTopRow++;
            }
            else foamTime++;

            // Fill foam from fillTopRow up to foamTopRow
            for (int i = fillTopRow + 1; i <= foamTopRow; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    if (gridMask[j, i] && !tempGrid[j, i].isStatic)
                        //tempGrid[j, i].isFoam = true;
                        tempGrid[j, i].isFoam = !tempGrid[j, i].isOccupied;
                }
            }
        }
        else
        {
            // Gradually decrease foamTopRow when space bar is released
            if (foamDTime > foamDecreaseSpeed && foamTopRow > fillTopRow)
            {
                foamDTime = 0;

                // Determine percentage of foam to convert to occupied cells
                float percentageToConvert = foamPercent; // 10% of the foam
                int foamToConvert = Mathf.CeilToInt((foamTopRow - fillTopRow) * percentageToConvert);

                // Transform foam to occupied cells from fillTopRow upwards
                int convertedCount = 0;
                for (int i = fillTopRow + 1; i <= foamTopRow; i++)
                {
                    for (int j = 0; j < gridWidth; j++)
                    {
                        if (gridMask[j, i] && tempGrid[j, i].isFoam)
                        {
                            // Transform foam to occupied cell
                            tempGrid[j, i].isFoam = false;
                            tempGrid[j, i].isOccupied = true;
                            convertedCount++;
                        }
                    }
                    if (convertedCount >= foamToConvert)
                        break;
                }

                // Remove foam below foamTopRow
                for (int j = 0; j < gridWidth; j++)
                {
                    if (gridMask[j, foamTopRow])
                        tempGrid[j, foamTopRow].isFoam = false;
                }
                // Move foamTopRow down
                foamTopRow--;
            }
            else
            {
                foamDTime++;
            }

        }
    }


    protected override void End()
    {
        if (fillTopRow >= winRowStart && fillTopRow <= winRowEnd)
        {
            Debug.Log("win");
            GameManager.instance.score += 10 + (fillTopRow - (winRowEnd - winRowStart) / 2 + winRowStart);
        }
        else
        {
            Debug.Log("lose");
            //GameManager.instance.lifes--; 
        }
    }

    protected override void InitMask(int x, int y)
    {
        return;
    }

    protected override void IncreaseSurface(int x, int y)
    {
        return;
    }
}
