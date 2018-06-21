using Sitecore.Analytics.Configuration;
using Sitecore.Analytics.Pipelines.ExcludeRobots;
using Sitecore.Diagnostics;
using System;
using System.Linq;
using System.Net;
using System.Web;

namespace Sitecore.Support.Analytics.Pipelines.ExcludeRobots
{
  public class CheckIpAddress : Sitecore.Analytics.Pipelines.ExcludeRobots.CheckIpAddress
  {
    private string GetIpFromHttpHeader(HttpContext httpContext)
    {
      string forwardedRequestHttpHeader = AnalyticsSettings.ForwardedRequestHttpHeader;
      if (!string.IsNullOrEmpty(forwardedRequestHttpHeader))
      {
        string str2 = httpContext.Request.Headers[forwardedRequestHttpHeader];
        if (!string.IsNullOrEmpty(str2))
        {
          string str3 = str2.Split(new char[] { ',' }).Last<string>().Trim();
          string str4 = str3.Split(':').First().Trim();
          if (string.IsNullOrEmpty(str4))
          {
            Log.Warn($"Sitecore.Support.379609 :: {forwardedRequestHttpHeader} header does not store a valid IP address ({str2})", this);
          }
          else
          {
            IPAddress address;
            try
            {
              address = IPAddress.Parse(str4);
            }
            catch (FormatException)
            {
              Log.Warn($"Sitecore.Support.379609 :: {forwardedRequestHttpHeader} header does not store a valid IP address ({str2})", this);
              return string.Empty;
            }
            return address.ToString();
          }
        }
      }
      return string.Empty;
    }



    public override void Process(ExcludeRobotsArgs args)
    {
      //IL_0055: Unknown result type (might be due to invalid IL or missing references)
      Assert.ArgumentNotNull((object)args, "args");
      HttpContext current = HttpContext.Current;
      Assert.IsNotNull((object)current, "HttpContext");
      Assert.IsNotNull((object)current.Request, "Request");
      string text = GetIpFromHttpHeader(current);
      if (string.IsNullOrEmpty(text) && current.Request.UserHostAddress != null)
      {
        text = current.Request.UserHostAddress;
      }
      if (AnalyticsSettings.Robots.ExcludeList.ContainsIpAddress(text))
      {
        args.IsInExcludeList = true;
      }
    }

    public CheckIpAddress()      
    {
    }
  }
}