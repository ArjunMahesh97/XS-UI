using UnityEngine;

public class WordSearchConstantVariables
{
    public const string LettersFolder = "CharPngs/letters_2"; // Folder with letter sprites, based on Resources/...
    public const bool DebugButtonsActive = true; // Enable or disable buttons like next/prev level

    public const int PowerHintInitial = 10; // Initial count of hints
    public const int PowerHintPrice = 25; // Price of hint after player runs out of initial count
    public const int PowerHintUnlockLevel = 2; // Level at which hint power unlocks
    public const int PowerWindInitial = 10; // Initial count of "wind" hints
    public const int PowerWindPrice = 25; // Price of "wind" hint after player runs out of initial count
    public const int PowerWindUnlockLevel = 3; // Level at which widn power unlocks

    public static readonly Color SolutionBackground = new Color(0.85f, 0f, 1f); // Color of solution panel background
    public static readonly Color SolutionForeground = new Color(0.97f, 1f, 0.81f); // Color of solution panel text

    public const int LettersSpacingX = 2; // Horizontal spacing of letters
    public const int LettersSpacingY = 2; // Vertical spacing of letters
    
    public const int LettersOffsetY = 0; // Vertical offset of letters
    public const int LettersTopOffsetY = 0; // Vertical offset of letters at top panel
    public const int LettersBottomOffsetY = 0; // Vertical offset of letters at bottom panel
    
    public static readonly Color LevelNumColorMenu = Color.white; // Color of text with level number in menu
    public static readonly Color LevelNumColor = Color.white; // Color of text with level number in game
    
    public static readonly Color[] LineColors = { // Background colors of drawing word-lines, choosen randomly
        new Color(0.85f, 0f, 1f),
        new Color(0.68f, 0.48f, 0.14f),
        new Color(0.13f, 0.79f, 0.8f),
        new Color(0.63f, 1f, 0.07f),
        new Color(1f, 0.14f, 0.35f)
    };
    public static readonly Color NotLineCharColor = new Color(0.27f, 0.12f, 0.11f); // In-game character color
    public static readonly Color LineCharColor = Color.white; // In-game character color on top of drawing line
    
    public const float LettersBottomScale = 1.3f; // Scale multiplier of letters in bottom panel
    
    public const bool MultiplayerDemoEnabled = true; // Enable button in menu to start multiplayer demo
    public const bool MultiplayerDemoTutorialEnabled = false; // Should tutorial (hint at start) be shown in multiplayer demo
    public static readonly Color EnemySolutionForeground = new Color(0.29f, 0.12f, 0.08f); // Color to colorize words in bottom panel that enemy solved
    public const float EnemyMoveTimeout = 15; // Interval, with which enemy makes a move 
    public const float EnemyWinDelay = 2; // Delay for "you lose" panel after enemy assembles his last word in multiplayer demo

    public static readonly Color LineColor = new Color(0.13f, 0.79f, 0.8f); // Background color of drawing players word-lines in multiplayer
    public static readonly Color EnemyLineColor = new Color(1f, 0.14f, 0.35f); // Background color of drawing enemys word-lines in multiplayer
    
    public const WordSearchBanner BannerInHomeScreen = WordSearchBanner.No; // Where should banner be in home screen
    public const WordSearchBanner BannerInGameScreen = WordSearchBanner.Top; // Where should banner be in game screen
    public const WordSearchBanner BannerInWinScreen = WordSearchBanner.Bottom; // Where should banner be in win screen
    
    public const float BannerHeightHome = 50; // Banner height in pixels in home screen
    public const float BannerHeightWin = 50; // Banner height in pixels in win screen
    public const float BannerHeightGame = 135; // Banner height in pixels in game screen
    
    public const bool ChaptersEnabled = true; // Show chapters display at the top of level completed panel
    public static readonly Color ChapterCompleted = new Color(0.85f, 0f, 1f); // Color of square in UI for completed chapter
    public static readonly Color ChapterNotCompleted = new Color(0.97f, 1f, 0.81f); // Color of square in UI for not completed chapter
}

public enum WordSearchBanner
{
    No,
    Top,
    Bottom
}