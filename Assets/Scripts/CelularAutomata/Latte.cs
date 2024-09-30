using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Latte : CellularAutomataLiquid
{
    public List<Vector2Int> initialMilkCells;
    public Sprite milkSprite;
    public Sprite coffeeSprite;
    private bool putMilk = false;
    private List<Vector2Int> initialCells;

    protected override void ActivateCells()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            putMilk = !putMilk;
        }
        initialCells = !putMilk ? initialLiquidCells : initialMilkCells;
        foreach (Vector2Int cell in initialCells)
        {
            if (cell.x >= 0 && cell.x < gridWidth && cell.y >= 0 && cell.y < gridHeight)
            {
                tempGrid[cell.x, cell.y].isMilk = putMilk;
                grid[cell.x, cell.y].isMilk = putMilk;
                grid[cell.x, cell.y].isOccupied = true;

                if (putMilk)
                {
                    cellObjects[cell.x, cell.y].GetComponent<SpriteRenderer>().sprite = milkSprite;
                }
                else cellObjects[cell.x, cell.y].GetComponent<SpriteRenderer>().sprite = coffeeSprite;
                cellObjects[cell.x, cell.y].SetActive(true);
            }
        }
    }
    protected override void UpdateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridMask[x, y])
                {
                    grid[x, y].isOccupied = tempGrid[x, y].isOccupied;
                    grid[x, y].isStatic = tempGrid[x, y].isStatic;
                    grid[x, y].isMilk = tempGrid[x, y].isMilk;

                    if (tempGrid[x, y].isMilk)
                    {
                        cellObjects[x, y].GetComponent<SpriteRenderer>().sprite = milkSprite;
                    }
                    else cellObjects[x, y].GetComponent<SpriteRenderer>().sprite = coffeeSprite;
                    cellObjects[x, y].SetActive(grid[x, y].isOccupied);
                }
            }
        }
    }

    protected override void End()
    {
        int total = 0;
        int coffee = 0;
        int milk = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (gridMask[x, y])
                {
                    total++;
                    grid[x, y].isOccupied = tempGrid[x, y].isOccupied;
                    grid[x, y].isStatic = tempGrid[x, y].isStatic;
                    if(grid[x, y].isMilk) milk++;
                    else coffee++;
                }
            }
        }
        float coffeePercentage = (total > 0) ? (coffee / (float)total) * 100f : 0f;
        float milkPercentage = (total > 0) ? (milk / (float)total) * 100f : 0f;

        Debug.Log($"End: {coffee} / {total} coffee cells ({coffeePercentage:F2}%), {milk} / {total} milk cells ({milkPercentage:F2}%)");
    }

    protected override void InitMask(int x, int y)
    {
        return;
    }

    protected override void SetHorizontal(int targetX, int x, int y)
    {
        tempGrid[targetX, y].isMilk = tempGrid[x, y].isMilk;
        tempGrid[targetX, y].isOccupied = true;
        tempGrid[x, y].isOccupied = false;
        tempGrid[x, y].isMilk = false;
        tempGrid[x, y].isStatic = false;
    }

    protected override void SetVertical(int x, int y, int i, bool hasEmptyAbove)
    {
        tempGrid[x, y - i].isMilk = tempGrid[x, y].isMilk;
        tempGrid[x, y].isOccupied = false;
        tempGrid[x, y - i].isOccupied = !hasEmptyAbove;
    }
    protected override void IncreaseSurface(int x, int y)
    {
        return;
    }
}
