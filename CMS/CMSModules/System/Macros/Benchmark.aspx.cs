using System;
using System.Linq;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.MacroEngine;

public partial class CMSModules_System_Macros_Benchmark : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        editorElem.Editor.Language = LanguageEnum.HTMLMixed;
    }


    protected void btnRun_Click(object sender, EventArgs e)
    {
        var text = editorElem.Text;

        // Get the number of iterations
        int iterations = ValidationHelper.GetInteger(txtIterations.Text, 0);
        if (iterations <= 0)
        {
            ShowError(GetString("macros.benchmark.invaliditerations"));
            return;
        }

        string result = String.Empty;

        int runs = 0;

        var startTime = DateTime.Now;
        
        double minRunSeconds = double.MaxValue;
        double maxRunSeconds = 0;

        double totalRunSeconds = 0;

        // Run the benchmark
        for (int i = 0; i < iterations; i++)
        {
            var runStart = DateTime.Now;

            // Execute the run
            result = MacroResolver.Resolve(text);

            runs++;

            // Count the run time
            var runEnd = DateTime.Now;
            var runTime = runEnd - runStart;
            var runSeconds = runTime.TotalSeconds;

            if (runSeconds < minRunSeconds)
            {
                minRunSeconds = runSeconds;
            }
            if (runSeconds > maxRunSeconds)
            {
                maxRunSeconds = runSeconds;
            }

            totalRunSeconds += runSeconds;
        }

        var endTime = DateTime.Now;

        if (Math.Abs(minRunSeconds - double.MaxValue) < Math.E)
        {
            minRunSeconds = 0;
        }

        editorElem.Text = text;
        txtOutput.Text = result;

        var totalTime = endTime - startTime;
        
        var totalSeconds = totalTime.TotalSeconds;
        var secondsPerRun = totalSeconds / runs;

        txtResults.Text = String.Format(
@"
Total runs: {0}s
Total benchmark time: {1:f5}s
Total run time: {5:f5}s

Average time per run: {2:f5}s
Min run time: {3:f5}s
Max run time: {4:f5}s

Evaluated text: 
---------------
{6}
"
        , runs
        , totalSeconds
        , secondsPerRun
        , minRunSeconds
        , maxRunSeconds
        , totalRunSeconds
        , text
        );
    }
}
