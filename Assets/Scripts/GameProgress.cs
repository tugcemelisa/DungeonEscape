public static class GameProgress
{
    public static int CarryScore { get; set; } = 0;
    public static int CurrentLevel { get; set; } = 1;

    public static void Reset()
    {
        CarryScore   = 0;
        CurrentLevel = 1;
    }
}
