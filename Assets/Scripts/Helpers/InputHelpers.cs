using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputHelpers
{
    public static Dictionary<int, Task> HashedRoutines = new Dictionary<int, Task>();
    public static Task StartSingletonCoroutine(this MonoBehaviour behaviour, Action action, Predicate<object> doWhile, Func<YieldInstruction> waitStep, bool replace = false)
    {
        int hash;
        unchecked
        {
            hash = 17;
            hash = hash * 31 + behaviour.GetHashCode();
            hash = hash * 31 + action.GetHashCode();
            hash = hash * 31 + doWhile.GetHashCode();
        }

        if (!HashedRoutines.ContainsKey(hash))
            HashedRoutines.Add(hash, null);
        if ((HashedRoutines[hash] != null && replace) || (!HashedRoutines[hash]?.Running ?? false))
        {
            HashedRoutines[hash]?.Stop();
            HashedRoutines[hash] = null;
        }
        if (HashedRoutines[hash] == null)
            return HashedRoutines[hash] = new Task(RepeatWhileEnumerator(hash, action, doWhile, waitStep));
        else return HashedRoutines[hash];
    }
    private static IEnumerator RepeatWhileEnumerator(int hash, Action action, Predicate<object> doWhile, Func<YieldInstruction> waitStep) {
        while (doWhile?.Invoke(null) ?? false)
        {
            action?.Invoke();
            yield return waitStep?.Invoke() ?? new WaitForEndOfFrame();
        }
    }


}
