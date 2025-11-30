namespace WheelOfFortune.Data
{
    public enum GameState
    {
        Idle = 0,
        Starting = 1,
        WaitingForSpin = 2,
        Spinning = 3,
        ShowingResult = 4,
        Death = 5,
        Reviving = 6,
        ShowingSuperReward = 7,
        Collecting = 8,
        GameOver = 9
    }
}