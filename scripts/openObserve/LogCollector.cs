using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ColdMint.scripts.debug;
using HttpClient = System.Net.Http.HttpClient;

namespace ColdMint.scripts.openObserve;

/// <summary>
/// <para>LogCollector</para>
/// <para>日志收集器</para>
/// </summary>
public static class LogCollector
{
    private static readonly List<LogData> LogDataList = [];

    /// <summary>
    /// <para>Automatic upload threshold</para>
    /// <para>自动上传的阈值</para>
    /// </summary>
    /// <remarks>
    ///<para>An attempt is made to upload logs when messages reach this number.</para>
    ///<para>当消息到达此数量后将尝试上传日志。</para>
    /// </remarks>
    public static int UploadThreshold { get; set; } = 300;

    private static bool _lockList;

    /// <summary>
    /// <para>httpClient</para>
    /// <para>Http客户</para>
    /// </summary>
    private static HttpClient? _httpClient;

    private static string? _orgId;
    private static string? _streamName;

    /// <summary>
    /// <para>CanUploadLog</para>
    /// <para>是否能上传日志</para>
    /// </summary>
    public static bool CanUploadLog => _httpClient != null;

    /// <summary>
    /// <para>UpdateHttpClient</para>
    /// <para>更新Http客户端</para>
    /// </summary>
    /// <param name="openObserve"></param>
    public static void UpdateHttpClient(OpenObserve openObserve)
    {
        if (openObserve.Address == null || openObserve.AccessToken == null || openObserve.OrgId == null ||
            openObserve.StreamName == null)
        {
            return;
        }

        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(openObserve.Address);
        //Add a Cookie to the request header
        //添加Cookie到请求头
        var cookie = new Cookie("auth_tokens",
            "{\"access_token\":\"Basic " + openObserve.AccessToken + "\",\"refresh_token\":\"\"}");
        httpClient.DefaultRequestHeaders.Add("Cookie", cookie.ToString());
        _httpClient = httpClient;
        _orgId = openObserve.OrgId;
        _streamName = openObserve.StreamName;
    }


    /// <summary>
    /// <para>Push log</para>
    /// <para>推送日志</para>
    /// </summary>
    /// <param name="logRequestBean"></param>
    private static async Task PostLog(List<LogData> logRequestBean)
    {
        if (_httpClient == null)
        {
            return;
        }

        _lockList = true;
        LogCat.LogWithFormat("start_uploading", label: LogCat.LogLabel.LogCollector, false, logRequestBean.Count);
        var httpResponseMessage =
            await _httpClient.PostAsJsonAsync("/api/" + _orgId + "/" + _streamName + "/_json", logRequestBean);
        _lockList = false;
        if (httpResponseMessage.IsSuccessStatusCode)
        {
            LogDataList.RemoveRange(0, logRequestBean.Count);
            LogCat.LogWithFormat("upload_successful", label: LogCat.LogLabel.LogCollector, false,
                logRequestBean.Count, LogDataList.Count);
            if (LogDataList.Count > UploadThreshold)
            {
                //After the upload succeeds, if the threshold is still met, continue uploading.
                //上传成功后，如果依然满足阈值，那么继续上传。
                await PostLog(LogDataList.GetRange(0, UploadThreshold));
            }
        }
        else
        {
            LogCat.LogWithFormat("upload_failed", label: LogCat.LogLabel.LogCollector, false,
                httpResponseMessage.StatusCode.ToString(), LogDataList.Count);
        }
    }

    /// <summary>
    /// <para>Push log information to the cache</para>
    /// <para>推送日志信息到缓存</para>
    /// </summary>
    /// <param name="logData">
    ///<para>Log data</para>
    ///<para>日志信息</para>
    /// </param>
    /// <remarks>
    ///<para>When logs reach the upload threshold, logs are automatically uploaded.</para>
    ///<para>当日志信息到达上传阈值后会自动上传。</para>
    /// </remarks>
    public static async Task Push(LogData logData)
    {
        LogDataList.Add(logData);
        LogCat.LogWithFormat("upload_status", LogCat.LogLabel.LogCollector, false, LogDataList.Count, UploadThreshold);
        if (!_lockList && LogDataList.Count > UploadThreshold)
        {
            //执行上传
            await PostLog(LogDataList.GetRange(0, UploadThreshold));
        }
    }
}

public class LogData
{
    /// <summary>
    /// <para>The AppId of this application</para>
    /// <para>此应用的AppId</para>
    /// </summary>
    public string? AppId { get; set; }

    /// <summary>
    /// <para>message</para>
    /// <para>消息</para>
    /// </summary>
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public string? Message { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    /// <summary>
    /// <para>level</para>
    /// <para>错误等级</para>
    /// </summary>
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    public int Level { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global
}