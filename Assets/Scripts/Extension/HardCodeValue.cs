using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HardCodeValue
{
    public static Color GetColorShieldTitle(string id, bool isLegend){
        if (isLegend) return Color.clear;
        
        switch (id){
            case "Beginner":
                if (ColorUtility.TryParseHtmlString("#8D4B1F", out Color beginner)) return beginner; 
                break;
            case "Amateur":
                if (ColorUtility.TryParseHtmlString("#556F97", out Color amateur)) return amateur;
                break;
            case "Pro":
                if (ColorUtility.TryParseHtmlString("#556F97", out Color pro)) return pro; 
                break;
            case "Master":
                if (ColorUtility.TryParseHtmlString("#AD6811", out Color master)) return master; 
                break;
            case "Champion":
                if (ColorUtility.TryParseHtmlString("#024322", out Color champion)) return champion; 
                break;
            default: return Color.clear;
        }
        return new Color();
    }

    public static string GetRankName(string id, bool isLegend){
        if (isLegend) return "Легенда";
        
        switch (id){
            case "Beginner":
                return "Новичок";
                break;
            case "Amateur":
                return "Любитель";
                break;
            case "Pro":
                return "Профессионал";
                break;
            case "Master":
                return "Мастер";
                break;
            case "Champion":
                return "Чемпион";
                break;
        }

        return "";
    }
}
