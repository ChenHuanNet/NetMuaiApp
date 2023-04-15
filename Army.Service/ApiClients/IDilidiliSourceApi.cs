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

    }
}
