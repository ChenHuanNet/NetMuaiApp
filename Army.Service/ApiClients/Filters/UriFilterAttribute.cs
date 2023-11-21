using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace Army.Service.ApiClients.Filters
{
    /// <summary>
    ///用来处理动态Uri的拦截器 
    /// </summary>
    public class UriFilterAttribute : ApiFilterAttribute
    {
        /// <summary>
        /// 在IHttpApi 方法头部增加  [Header("TenantId")] string tenantId
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            var tenantId = context.HttpContext.RequestMessage.Headers.Contains("TenantId") ? context.HttpContext.RequestMessage.Headers.GetValues("TenantId").FirstOrDefault() : "";


            string host = "http://dilidili9.com";
            switch (tenantId)
            {
                case "dilidili9": host = "http://dilidili9.com"; break;
                case "search": host = "http://175.178.205.131:22117"; break;
            }
            HttpApiRequestMessage requestMessage = context.HttpContext.RequestMessage;

            //去除空值参数
            //1.去除第一个前导?字符
            var dic = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(requestMessage.RequestUri.Query))
                dic = requestMessage.RequestUri.Query.Substring(1)
                    //2.通过&划分各个参数
                    .Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
                    //3.通过=划分参数key和value,且保证只分割第一个=字符
                    .Select(param => param.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries))
                    //4.通过相同的参数key进行分组
                    .GroupBy(part => part[0], part => part.Length > 1 ? part[1] : string.Empty)
                    //5.将相同key的value以,拼接
                    .ToDictionary(group => group.Key, group => string.Join(",", group));

            requestMessage.RequestUri = new Uri(new Uri(host), requestMessage.RequestUri.AbsolutePath);

            foreach (var item in dic)
            {
                if (!string.IsNullOrEmpty(item.Value))
                    requestMessage.AddUrlQuery(item.Key, System.Web.HttpUtility.UrlDecode(item.Value));
            }


            return Task.CompletedTask;
        }

        public override Task OnResponseAsync(ApiResponseContext context)
        {
            //不处理响应的信息
            return Task.CompletedTask;
        }
    }
}
