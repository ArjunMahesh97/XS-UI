using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WordsManager
{
    private static readonly List<WordSearchLevel> Levels = new List<WordSearchLevel>();
    static int[] ChapterCompletedLevel = { 0, 3, 5, 10, 15 }; //Array of chapter levels (0 is needed for start)

    public static WordSearchLevel Get(int level)
    {
        if (Levels.Count == 0) Load();

        return Levels[level];
    }

    public static void GetLevelsInChapter(int level, out int positionInChapter, out int totalInChapter)
    {
        //DEPRECATED CHAPTER GEN CONNECTED WITH BOSSES.
        /*int prevBossLevelNum = level - 1;
        while (prevBossLevelNum >= 0 && Levels[prevBossLevelNum].Boss == BossType.NotBoss)
        {
            prevBossLevelNum--;
        }

        positionInChapter = level - prevBossLevelNum;
        
        int bossLevelNum = level;
        while (bossLevelNum < Levels.Count && Levels[bossLevelNum].Boss == BossType.NotBoss)
        {
            bossLevelNum++;
        }
        totalInChapter = bossLevelNum - prevBossLevelNum;*/
        //Chapter division
        int tc = 0;
        int pc = 0;
        for (int i = 0; i < ChapterCompletedLevel.Length; i++)
        {
            if ((level > ChapterCompletedLevel[i] - 1 || level == 0) && level < ChapterCompletedLevel[i + 1])
            {
                tc = ChapterCompletedLevel[i + 1] - ChapterCompletedLevel[i];
                pc = level - ChapterCompletedLevel[i] + 1;
                break;
            }
        }
        totalInChapter = tc;
        positionInChapter = pc;
    }

    public static int GetLevelCount()
    {
        return Levels.Count;
    }

    private static void Load()
    {
        int i = 1;

        while (true)
        {
            TextAsset textAsset = (TextAsset) Resources.Load("WordSearch/" + i, typeof(TextAsset));
            if (textAsset == null) return;
            i++;

            string[] lines = textAsset.text.Split(new[] {'\n', '\r'}, System.StringSplitOptions.RemoveEmptyEntries);
            WordSearchLevel l = new WordSearchLevel();

            string lineTrimmed = lines[0].Trim(' ', '\t', '\r', '\n').Substring("SIZE = ".Length);
            l.Columns = int.Parse(lineTrimmed.Split('x')[0]);
            l.Rows = int.Parse(lineTrimmed.Split('x')[1]);

            lineTrimmed = lines[1].Trim(' ', '\t', '\r', '\n', 's').Substring("TIME = ".Length);
            l.Seconds = int.Parse(lineTrimmed);

            lineTrimmed = lines[2].Trim(' ', '\t', '\r', '\n', '%').Substring("DIFFICULTY = ".Length);
            l.DifficultyPercent = int.Parse(lineTrimmed);

            lineTrimmed = lines[3].Trim(' ', '\t', '\r', '\n', '%').Substring("BOSS = ".Length);
            l.Boss = lineTrimmed.StartsWith("NO")
                ? BossType.NotBoss
                : (lineTrimmed.StartsWith("VER") ? BossType.Vertical : BossType.Horizontal);

            lineTrimmed = lines[4].Trim(' ', '\t', '\r', '\n', '%').Substring("BOSS_SCREENS = ".Length);
            l.BossMultiplier = int.Parse(lineTrimmed);

            lineTrimmed = lines[5].Trim(' ', '\t', '\r', '\n', '%').Substring("ADS = ".Length);
            l.AdsIndex = int.Parse(lineTrimmed);

            lineTrimmed = lines[6].Trim(' ', '\t', '\r', '\n', '%').Substring("PALMUP_ADS = ".Length);
            l.PlamAdsIndex = int.Parse(lineTrimmed);

            l.Words = lines.Skip(7)
                .Select(line => line.Trim(' ', '\t', '\r', '\n'))
                .Where(line => line != "")
                .ToArray();

            Levels.Add(l);
        }
    }
}

public class WordSearchLevel
{
    public string[] Words;

    public int Rows;
    public int Columns;

    public int Seconds;

    public int DifficultyPercent;
    public BossType Boss;
    public int BossMultiplier;

    public int AdsIndex;
    public int PlamAdsIndex;
}

public enum BossType
{
    NotBoss,
    Vertical,
    Horizontal
}