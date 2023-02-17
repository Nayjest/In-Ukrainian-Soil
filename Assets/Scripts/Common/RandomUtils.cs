using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomUtils
{
    public enum Seed
    {
        None,
        Match,
        Session,
        DayOfYear,
        UniqueClientSeed
    }
    public static Dictionary<Seed, int> Seeds;

    static int NewSeed => Random.Range(1, 10000);
    static RandomUtils()
    {
        var clientSeed = PlayerPrefs.GetInt("unique_client_seed", 0);
        if (clientSeed == 0)
        {
            clientSeed = NewSeed;
            PlayerPrefs.SetInt("unique_client_seed", clientSeed);
        }

        Seeds = new Dictionary<Seed, int>
        {
            {Seed.Session, NewSeed },
            {Seed.Match, NewSeed },
            {Seed.UniqueClientSeed, clientSeed },
            {Seed.DayOfYear, System.DateTime.Now.DayOfYear+clientSeed},
        };

        //SceneManager.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static int prevStoredSeed = 0;
    public static void SetSeed(Seed seed)
    {
        if (seed == Seed.None) return;
        if (prevStoredSeed != 0) throw new System.Exception("Can't Set seed, already used");        
        prevStoredSeed = Random.seed;
        Random.seed = Seeds[seed];
    }
    public static void RestoreSeed()
    {
        if (prevStoredSeed == 0) return;
        Random.seed = prevStoredSeed;
        prevStoredSeed = 0;
    }
    public static int Range(int minInclusive, int maxExclusive, Seed seed = Seed.None)
    {
        if (seed == Seed.None) return Random.Range(minInclusive, maxExclusive);
        var prevS = Random.seed;
        Random.seed = Seeds[seed];
        int res = Random.Range(minInclusive, maxExclusive);
        Random.seed = prevS;
        return res;
    }

    public static void Do(Seed seed, System.Action action)
    {
        if (action == null) return;
        if (seed == Seed.None) action.Invoke();
        var prevS = Random.seed;
        Random.seed = Seeds[seed];
        action.Invoke();
        Random.seed = prevS;        
    }
    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            Debug.Log("Set new Math Random Seed");
            Seeds[Seed.Match] = NewSeed;
        }
    }
}
