using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cocktel : CellularAutomataLiquid
{
    public List<Vector2Int> initialIce;
    public Sprite liquidSprite;
    public Sprite iceSprite;

    protected override void IncreaseSurface(int x, int y)
    {
        halfIce++;
        foreach (var i in initialIce)
        {
            if (gridMask[i.x, i.y + 1])
            {
                tempGrid[i.x, i.y + 1].isIce = true;
                tempGrid[i.x, i.y].isIce = false;
            }
        }
    }
    protected override void InitMask(int x, int y)
    {
        if (initialIce.Contains(new Vector2Int(x, y)))
        {
            grid[x, y].isIce = true;
            grid[x, y].isOccupied = true;
            grid[x, y].isStatic = false;
        }
    }
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

    protected override void SetHorizontal(int targetX, int x, int y)
    {
        tempGrid[targetX, y].isOccupied = true;
        tempGrid[x, y].isOccupied = false;
        tempGrid[x, y].isStatic = false;
        tempGrid[targetX, y].isFoam = false;
    }

    protected override void SetVertical(int x, int y, int i, bool hasEmptyAbove)
    {
        if (tempGrid[x, y].isIce) return;
        tempGrid[x, y].isOccupied = false;
        tempGrid[x, y - i].isOccupied = !hasEmptyAbove;
    }

    protected override void UpdateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridMask[x, y])
                {
                    if (initialIce.Contains(new Vector2Int(x, y)))
                    {
                        grid[x, y].isIce = true;
                        grid[x, y].isOccupied = true;
                        grid[x, y].isStatic = true;
                    }
                    else
                    {
                        grid[x, y].isIce = false;
                        grid[x, y].isStatic = false;
                        grid[x, y].isOccupied = false;
                    }
                    grid[x, y].isOccupied = tempGrid[x, y].isOccupied;
                    grid[x, y].isStatic = tempGrid[x, y].isStatic;
                    if(grid[x, y].isIce)
                        cellObjects[x, y].GetComponent<SpriteRenderer>().sprite = iceSprite;
                    else cellObjects[x, y].GetComponent<SpriteRenderer>().sprite = liquidSprite;
                    cellObjects[x, y].SetActive(grid[x, y].isOccupied);
                }
            }
        }
    }
    protected override void End()
    {
        if (fillTopRow >= winRowStart && fillTopRow <= winRowEnd)
        {
            Debug.Log("win");
        }
    }

}
