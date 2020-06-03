using UnityEngine;

public static class WordSearchAnalyticsHelper
{

    

    public static void AnalyticsEvent(string eventName, int? levelNum = null, int? timeTaken = null)
    {



#if DEVTODEV
        DevToDev.CustomEventParams eventParams = new DevToDev.CustomEventParams();
        eventParams.AddParam("Game", "WordSearch");
        if (levelNum.HasValue) eventParams.AddParam("Level" , levelNum.Value);
        if (timeTaken.HasValue) eventParams.AddParam("Time" , timeTaken.Value);
        
        DevToDev.Analytics.CustomEvent(eventName, eventParams);
#endif
#if AMPLITUDE
        var options = new System.Collections.Generic.Dictionary<string, object>();
        options.Add("Game" , "WordSearch");
        if (levelNum.HasValue) options.Add("Level" , levelNum.Value);
        if (timeTaken.HasValue) options.Add("Time" , timeTaken.Value);
        Amplitude.Instance.logEvent(eventName, options);
#endif
    }
}