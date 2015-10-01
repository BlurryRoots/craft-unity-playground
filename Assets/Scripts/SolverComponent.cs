using UnityEngine;
using System.Collections;

public class SolverComponent : MonoBehaviour {

    public VariableRandomizer Randomizer;
    public string XVariable, YVariable, ZVariable;
    public int UpdateIntervall = 1;

    private int xVariableIndex, yVariableIndex, zVariableIndex;

    void Start () {
        Randomizer = this.GetComponent<VariableRandomizer> ();

        xVariableIndex = (string.IsNullOrEmpty (XVariable))
            ? -1
            : this.Randomizer.VariableIndex (XVariable)
            ;
        yVariableIndex = (string.IsNullOrEmpty (YVariable))
            ? -1
            : this.Randomizer.VariableIndex (YVariable)
            ;
        zVariableIndex = (string.IsNullOrEmpty (ZVariable)) 
            ? -1
            : this.Randomizer.VariableIndex (ZVariable)
            ;
        solveCount = 0;
        minSolveTime = maxSolveTime = this.totalSolveTime = 0;

        var valueObjects = FindObjectsOfType<ValueComponent> ();
        for (int i = 0; i < valueObjects.Length; ++i) {
            Randomizer.Variables[i].Component = valueObjects[i]; 
        }
    }

    // Execution time stats
    private float minSolveTime;
    private float maxSolveTime;
    private float totalSolveTime;
    private int solveCount;

    /// <summary>
    /// Compute some new solutions and emit them as particles.
    /// </summary>
    void Update () {
        if (this.dt > this.UpdateIntervall) {
            this.dt = 0;

#if SuppressTimingAfterGC
            var gcCount = GC.CollectionCount(0);
#endif
            this.Randomizer.Solve ();
            var solveTime = this.Randomizer.LastSolveTime;
            // Update stats unless there happened to be a GC during the solver run.
#if SuppressTimingAfterGC
            if (GC.CollectionCount(0) == gcCount)
            {
#endif
            if (solveCount == 0) {
                minSolveTime = maxSolveTime = totalSolveTime = solveTime;
            }
            else {
                totalSolveTime += solveTime;
                if (solveTime > maxSolveTime)
                    maxSolveTime = solveTime;
                if (solveTime < minSolveTime)
                    minSolveTime = solveTime;
            }
            solveCount++;
#if SuppressTimingAfterGC
            }
#endif
            var position = new Vector3 (
                (float)this.Randomizer.ScalarValue (xVariableIndex),
                (float)this.Randomizer.ScalarValue (yVariableIndex),
                zVariableIndex < 0 ? 0 : (float)this.Randomizer.ScalarValue (zVariableIndex));
        }
        else {
            this.dt += Time.deltaTime;
        }
    }

    void OnGUI () {
        GUILayout.Label (string.Format ("Average solve time: {0}usec\nMin: {1}\nMax: {2}\nTotal variables: {3}\nTotal constraints: {4}",
            totalSolveTime / solveCount,
            minSolveTime,
            maxSolveTime,
            this.Randomizer.CSP.VariableCount,
            this.Randomizer.CSP.ConstraintCount));
    }

    private float dt = 0;

}
