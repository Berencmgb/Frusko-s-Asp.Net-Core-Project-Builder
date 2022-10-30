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
using {project}.ServiceClient;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace {project}.ServiceClient;

public class UserServiceClient : BaseServiceClient<UserDTO>, IUserServiceClient
{
    public UserServiceClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
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

    public async Task<Result<string>> LoginAsync(HttpPayload payload, UserDTO dto)
    {
        var client = await GetClient(payload);

        var postbody = JsonConvert.SerializeObject(dto);

        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        var result = await client.PostAsync(@$""{AdaptiveUrl}/Login"", new StringContent(postbody, Encoding.UTF8, ""application/json""));

        var json = await result.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<Result<string>>(json);
    }

    protected new string AdaptiveUrl { get => ""Account""; }
}

public interface IUserServiceClient : IBaseServiceClient<UserDTO>
{
    public Task<Result<string>> RegisterAsync(HttpPayload payload, UserDTO dto);
    public Task<Result<string>> LoginAsync(HttpPayload payload, UserDTO dto);
}
";

    public const string BaseServiceClientTemplate =
@"using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using {project}.Shared.Models;
using {project}.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using {project}.Shared.Utilities;

namespace {namespace};

public class BaseServiceClient<TDto> : IBaseServiceClient<TDto> where TDto : BaseDTO
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BaseServiceClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload)
    {
        var client = await GetClient(payload);

        var body = new PostBody { };

        var postbody = JsonConvert.SerializeObject(body);

        var result = await client.PostAsync($""{AdaptiveUrl}/getallasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));

        var json = await result.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
    }

    public async Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload, Pagination page)
    {
        var client = await GetClient(payload);
        
        var body = new PostBody { Pagination = page };
        
        var postbody = JsonConvert.SerializeObject(body);
        
        var result = await client.PostAsync($""{AdaptiveUrl}/getallasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
        var json = await result.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
    }
        
    public async Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload, params Expression<Func<TDto, object>>[] includes)
    {
        var client = await GetClient(payload);
        
        var body = new PostBody { };
        
        foreach (var include in includes)
        {
            body.Includes.Add(include.ToString());
        }
        
        var postbody = JsonConvert.SerializeObject(body);
        
        var result = await client.PostAsync($""{AdaptiveUrl}/getallasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
        var json = await result.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
    }
        
    public async Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload, Pagination page, params Expression<Func<TDto, object>>[] includes)
    {
        var client = await GetClient(payload);
        
        var body = new PostBody { Pagination = page };
        
        foreach (var include in includes)
        {
            body.Includes.Add(include.ToString());
        }
        
        var postbody = JsonConvert.SerializeObject(body);
        
        var result = await client.PostAsync($""{AdaptiveUrl}/getallasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
        var json = await result.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
    }
        
    public async Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, params Expression<Func<TDto, object>>[] includes)
    {
        var client = await GetClient(payload);
        
        PostBody body = new() { Expression = expression.Simplify().ToString() };
        
        foreach (var include in includes)
        {
            body.Includes.Add(include.ToString());
        }
        
        var postbody = JsonConvert.SerializeObject(body);
        
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
        var result = await client.PostAsync($""{AdaptiveUrl}/getallwhereasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
        var json = await result.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
    }
        
    public async Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, Pagination page, params Expression<Func<TDto, object>>[] includes)
    {
        var client = await GetClient(payload);
        
        PostBody body = new() { Expression = expression.Simplify().ToString(), Pagination = page };
        
        foreach (var include in includes)
        {
            body.Includes.Add(include.ToString());
        }
        
        var postbody = JsonConvert.SerializeObject(body);
        
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
        var result = await client.PostAsync($""{AdaptiveUrl}/getallwhereasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
        var json = await result.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
    }
        
    public async Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression)
    {
        var client = await GetClient(payload);
        
        PostBody body = new() { Expression = expression.Simplify().ToString() };
        
        var postbody = JsonConvert.SerializeObject(body);
        
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
        var result = await client.PostAsync($""{AdaptiveUrl}/getallwhereasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
        var json = await result.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
    }
        
    public async Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, Pagination page)
    {
        var client = await GetClient(payload);
        
        PostBody body = new() { Expression = expression.Simplify().ToString(), Pagination = page };
        
        var postbody = JsonConvert.SerializeObject(body);
        
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
        var result = await client.PostAsync($""{AdaptiveUrl}/getallwhereasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
        var json = await result.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
    }
        
    public async Task<TDto> GetSingleWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, params Expression<Func<TDto, object>>[] includes)
    {
        var client = await GetClient(payload);
        
        PostBody body = new() { Expression = expression.Simplify().ToString() };
        
        foreach (var include in includes)
        {
            body.Includes.Add(include.ToString());
        }
        
        var postbody = JsonConvert.SerializeObject(body);
        
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
        var result = await client.PostAsync($""{AdaptiveUrl}/GetSingleWhereAsync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
        var json = await result.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<TDto>(json);
    }
        
    public async Task<TDto> GetSingleWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression)
    {
        var client = await GetClient(payload);
        
        PostBody body = new() { Expression = expression.Simplify().ToString() };
        
        var postbody = JsonConvert.SerializeObject(body);
        
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
        var result = await client.PostAsync($""{AdaptiveUrl}/GetSingleWhereAsync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
        var json = await result.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<TDto>(json);
    }
        
    public async Task<Result<TDto>> CreateAsync(HttpPayload payload, TDto entityDTO)
    {
        try
        {
            var client = await GetClient(payload);
        
            var postbody = JsonConvert.SerializeObject(entityDTO);
        
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
            var result = await client.PostAsync($""{AdaptiveUrl}/createasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
            var json = await result.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<Result<TDto>>(json);
        }
        catch (Exception ex)
        {
            return new Result<TDto> { Success = false, Message = $""{ex.Message}"", Value = null };
        }
        
    }
        
    public async Task<Result<TDto>> UpdateAsync(HttpPayload payload, TDto entityDTO)
    {
        var client = await GetClient(payload);
        
        var postbody = JsonConvert.SerializeObject(entityDTO);
        
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        var result = await client.PutAsync($""{AdaptiveUrl}/Update"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
        var json = await result.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Result<TDto>>(json);
    }
        
    public async Task<Result> DeleteAsync(HttpPayload payload, string reference)
    {
        var client = await GetClient(payload);
        
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
        var result = await client.DeleteAsync($""{AdaptiveUrl}/Delete/{reference}"");
        
        var json = await result.Content.ReadAsStringAsync();
        
        return JsonConvert.DeserializeObject<Result>(json);
    }
        
    protected async Task<HttpClient> GetClient(HttpPayload payload)
    {
        var client = _httpClientFactory.CreateClient({project}WebConstants.ClientScope);
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, payload.SecurityToken);
        
        return client;
    }
        
    protected string AdaptiveUrl
    {
        get
        {
            return """";
        }
    }
}

public interface IBaseServiceClient<TDto> where TDto : BaseDTO
{
    public Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload);
    
    public Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload, Pagination page);
    
    public Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload, params Expression<Func<TDto, object>>[] includes);
    
    public Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload, Pagination page, params Expression<Func<TDto, object>>[] includes);
    
    public Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, params Expression<Func<TDto, object>>[] includes);
    
    public Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, Pagination page, params Expression<Func<TDto, object>>[] includes);
    
    public Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression);
    
    public Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, Pagination page);
    
    public Task<TDto> GetSingleWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, params Expression<Func<TDto, object>>[] includes);
    
    public Task<TDto> GetSingleWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression);
    
    public Task<Result<TDto>> CreateAsync(HttpPayload payload, TDto entityDTO);
    
    public Task<Result<TDto>> UpdateAsync(HttpPayload payload, TDto entityDTO);
    
    public Task<Result> DeleteAsync(HttpPayload payload, string reference);
}
";
}