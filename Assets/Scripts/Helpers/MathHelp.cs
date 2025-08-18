public static class MathHelp
{
    public static float Map(float value, float originMin, float originMax, float targetMin, float targetMax)
    {
        float originCoef = (value - originMin) / (originMax - originMin);

        return targetMin + (targetMax - targetMin) * originCoef;
    }
    public static int RewardCount(int count)
    {
        int sum = 0;
        for (int i = 1; i < count + 1; i++)
        {
            sum += ( i * i);
        }
        return sum;
    }
}
