using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IPlayMenu : IMenu
{
    [Space]
    public SceneField DefaultScene;

    [Space]
    public List<SceneField> Levels;

    private string scenePath => SceneUtility.GetScenePathByBuildIndex(GetCurrentProgress());
    protected string SceneName => scenePath.Substring(0, scenePath.Length - 6).Substring(scenePath.LastIndexOf('/') + 1);

    protected int GetCurrentProgress()
    {
        switch (PlayerPrefs.GetInt("CurrentProgress"))
        {
            case 1:
                return Levels[0].BuildIndex;
            case 2:
                return Levels[1].BuildIndex;
            case 3:
                return Levels[2].BuildIndex;
            case 4:
                return Levels[3].BuildIndex;
            case 5:
                return Levels[4].BuildIndex;
            case 6:
                return Levels[5].BuildIndex;
            case 7:
                return Levels[6].BuildIndex;
            case 8:
                return Levels[7].BuildIndex;
            case 9:
                return Levels[8].BuildIndex;
            case 10:
                return Levels[9].BuildIndex;
            default:
                return DefaultScene.BuildIndex;
        }
    }
}
