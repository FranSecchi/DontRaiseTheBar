using UnityEngine;
public class LiquidCell
{
    public bool isOccupied;
    public bool isStatic;
    public bool isFoam;
    public bool isMilk;
    public bool isIce;
    public int directionChangeCounter;
    public bool moveLeft;

    public LiquidCell(bool occupied, bool isStatic, bool isFoam, bool isMilk, bool isIce)
    {
        isOccupied = occupied;
        this.isStatic = isStatic;
        this.isFoam = isFoam;
        this.directionChangeCounter = 0;
        this.moveLeft = Random.value < 0.5f;
        this.isMilk = isMilk;
        this.isIce = isIce;
    }
}
