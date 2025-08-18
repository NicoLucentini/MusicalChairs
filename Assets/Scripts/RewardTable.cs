using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardTable
{

    public const int Level_1 = 1000;
    public const int Level_2 = 500;
    public const int Level_3 = 300;
    public const int Level_4 = 250;
    public const int Level_5 = 175;
    public const int Level_6 = 100;
    public const int Level_7 = 75;
    public const int Level_8 = 40;
    public const int Level_9 = 25;
    public const int Level_10 = 10;

    public static int GetScore(int level)
    {
        switch (level)
        {
            case 1: return Level_1;
            case 2: return Level_2;
            case 3: return Level_3;
            case 4: return Level_4;
            case 5: return Level_5;
            case 6: return Level_6;
            case 7: return Level_7;
            case 8: return Level_8;
            case 9: return Level_9;
            case 10: return Level_10;
        }
        return 0;
    }
}
