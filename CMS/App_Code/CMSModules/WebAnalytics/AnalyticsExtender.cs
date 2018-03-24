using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.UIControls;

[assembly: RegisterCustomClass("AnalyticsExtender", typeof(AnalyticsExtender))]

/// <summary>
/// Analytics page extender
/// </summary>
public class AnalyticsExtender : PageExtender<CMSPage>
{
    public override void OnInit()
    {
    }
}
