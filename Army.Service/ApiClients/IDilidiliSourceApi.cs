using Army.Domain.Consts;
using Army.Domain.Dto;
using Army.Service.ApiClients.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Army.Service.ApiClients
{
    public interface IDilidiliSourceApi : IHttpApi
    {
        [HttpGet("/media/{mediaId}/")]
        ITask<string> GetSourceHtml(string mediaId);



        [HttpGet("/play/{mediaId}/{detailId}.html")]
        ITask<string> GetSourceItemHtml(string mediaId, string detailId);


        [HttpGet("ssszz.php")]
        [UriFilter]
        ITask<string> SearchVideoAsync([Header("TenantId")] string tenantId, string q, int top = 10, int dect = 0, string other_kkk217 = AppConfigHelper.DiliDiliSourceHostEncoding);


        [HttpGet("tv/{detailId}/{num}.html")]
        [UriFilter]
        ITask<string> GetVideoHtml(string detailId, int num, string qp);
    }
}
