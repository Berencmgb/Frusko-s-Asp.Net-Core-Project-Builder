using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET.Core_Project_Builder.Boilerplate;

public static class ServiceClientTemplates
{
    public const string AccountServiceClientTemplate =
@"using {project}.Domain.Models;
using {project}.Shared.Models;
using {project}.Shared.Utilities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace {project}.ServiceClients;

public class AccountServiceClient : BaseServiceClient<UserDTO>, IAccountServiceClient
{
    public AccountServiceClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public async Task<Result<string>> RegisterAsync(HttpPayload payload, UserDTO dto)
    {
        var client = await GetClient(payload);

        var postbody = JsonConvert.SerializeObject(dto);

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        var result = await client.PostAsync(@$""{AdaptiveUrl}/Register"", new StringContent(postbody, Encoding.UTF8, ""application/json""));

        var json = await result.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<Result<string>>(json);
    }
}

public interface IAccountServiceClient : IBaseServiceClient<UserDTO>
{
    public Task<Result<string>> RegisterAsync(HttpPayload payload, UserDTO dto);
}
";
}