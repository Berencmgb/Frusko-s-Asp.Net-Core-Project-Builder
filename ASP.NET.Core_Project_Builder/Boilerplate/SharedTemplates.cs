namespace ASP.NET.Core_Project_Builder.Boilerplate
{
    public static class SharedTemplates
    {
        public const string BaseEntityTemplate =
@"namespace {namespace}
{
    public class BaseEntity : IBaseEntity
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public bool IsDeleted { get; set; }
    }

    public interface IBaseEntity
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public bool IsDeleted { get; set; }
    }
}";

        public const string BaseDTOTemplate =
@"namespace {namespace}
{
    public class BaseDTO
    {
        public string? Reference { get; set; }
        public bool IsDeleted { get; set; }
    }
}";

        public const string PaginationTemplate =
@"namespace {namespace}
{
    public class Pagination
    {
        public int PageSize { get; set; } = 10;
        public int Page { get; set; } = 0;

    }
}";

        public const string ResultTemplate =
@"namespace {namespace}
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T Value { get; set; }
    }

    public class Result
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}";

        public const string HttpPayloadTemplate =
@"namespace {namespace}
{
    public class HttpPayload
    {
        public string CurrentUserId { get; set; }
        public string SecurityToken { get; set; }
        public string Uri { get; set; }
    }
}";

        public const string PostBodyTemplate =
@"using {project}.Shared.Models;

namespace {namespace}
{
    public class PostBody
    {
        public string Expression { get; set; }
        public List<string> Includes { get; set; } = new List<string>();
        public Pagination Pagination { get; set; }
    }
}";

        public const string StringHelpersTemplate =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace {namespace};

public static class StringHelper
{
    public static List<string> GenerateIncludes(List<string> includes)
    {
        var result = new List<string>();

        foreach (var include in includes)
        {
            var includeString = include.ToString();
            var index = includeString.IndexOf('.');

            includeString = includeString.Remove(0, index + 1);

            if (includeString.Contains("".First()""))
                includeString = includeString.Replace("".First()"", """");

            result.Add(includeString);
        }

        return result;
    }
    
    public static List<string> GenerateIncludes<TEntity>(params Expression<Func<TEntity, object>>[] includes)
    {
        var result = new List<string>();
        
        foreach (var include in includes)
        {
            var includeString = include.ToString();
            var index = includeString.IndexOf('.');
        
            includeString = includeString.Remove(0, index + 1);
        
            if (includeString.Contains("".First()""))
                includeString = includeString.Replace("".First()"", """");
        
            result.Add(includeString);
        }
        
        return result;
    }

    public static bool IsBase64String(this string base64)
    {
        base64 = base64.Trim();
        return (base64.Length % 4 == 0) && Regex.IsMatch(base64, @""^[a-zA-Z0-9\+/]*={0,3}$"", RegexOptions.None);
    }

    public static string GenerateRandomCode(int length)
    {
        var random = new Random();
        
        const string chars = ""012A0BC1DE2FG3HI4JK5789LM6NO7PQ8RS9TUVWXYZ3456"";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string GenerateExpression(string expression)
    {
        expression = expression.Replace(""Convert("", """");
        expression = expression.Replace("", Int32)"", """");

        return expression;
    }
}
";

        public const string ExpressionHelpersTemplate =
@"using System.Linq.Expressions;

namespace {namespace}
{
    public static class ExpressionHelpers
    {
        public static Expression Simplify(this Expression expression)
        {
            var searcher = new DynamicExpressionSearcher();
            searcher.Visit(expression);
            return new DynamicExpressionEvaluator(searcher.DynamicExpressions).Visit(expression);
        }

        public static Expression<T> Simplify<T>(this Expression<T> expression)
        {
            return (Expression<T>)Simplify((Expression)expression);
        }
    }
}";

        public const string EnumHelpers =
@"using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace {namespace}
{
    public static class EnumHelpers
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            string displayName = """";
            displayName = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();

            if (string.IsNullOrWhiteSpace(displayName))
                displayName = enumValue.ToString();

            return displayName;
        }   
    }
}";

        public const string WebConstantsTemplate =
@"using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace {namespace}
{
    public static class {project}WebConstants
    {
        public static string ClientScope { get => ""{project}-Web-Client""; }
        public static string SecurityKey { get; } = ""{new_id}"";

        public static SymmetricSecurityKey SymmetricSecurityKey { get; } = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));

        public static string LoginEndpoint { get => """"; }
        public static string RegisterEndpoint { get => """"; }

        public static JwtSecurityToken Token { get; set; }

        public static string LocalHostUrl { get => """"; }
        public static string HostUrl { get; set; } = LocalHostUrl;
        public static string LiveUrl { get; set; }
        public static string {project}WebIssuer { get => ""{project}-Web""; }
    }
}";

        public const string ApiConstantsTemplate =
@"using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace {namespace}
{
    public static class {project}ApiConstants
    {
        public static string Scope { get => ""{project}-Api""; }
        public static string SecurityKey { get; } = ""{new_id}"";
        public static SymmetricSecurityKey SymmetricSecurityKey { get; } = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
        public static string TestHostUrl { get; set; } = ""{put_url_here}"";
        public static string HostUrl { get; set; } = TestHostUrl;
        public static string LiveUrl { get; set; } = """";
        public static string {project}ApiAudience { get => ""{project}-Api""; }
    }
}";

        public const string DynamicExpressionEvaluatorTemplate =
@"using System.Collections.Generic;
using System.Linq.Expressions;

namespace {namespace}
{
    public class DynamicExpressionEvaluator : ExpressionVisitor
    {
        private HashSet<Expression> dynamicExpressions;

        public DynamicExpressionEvaluator(HashSet<Expression> parameterlessExpressions)
        {
            this.dynamicExpressions = parameterlessExpressions;
        }
        public override Expression Visit(Expression node)
        {
            if (dynamicExpressions.Contains(node))
                return Evaluate(node);
            else
                return base.Visit(node);
        }

        private Expression Evaluate(Expression node)
        {
            if (node.NodeType == ExpressionType.Constant)
            {
                return node;
            }
            object value = Expression.Lambda(node).Compile().DynamicInvoke();
            return Expression.Constant(value, node.Type);
        }
    }
}";

        public const string DynamicExpressionSearcherTemplate =
@"using System.Collections.Generic;
using System.Linq.Expressions;

namespace {namespace}
{
    public class DynamicExpressionSearcher : ExpressionVisitor
    {
        public HashSet<Expression> DynamicExpressions { get; } = new HashSet<Expression>();
        private bool containsParameter = false;

        public override Expression Visit(Expression node)
        {
            bool originalContainsParameter = containsParameter;
            containsParameter = false;
            base.Visit(node);
            if (!containsParameter)
            {
                if (node?.NodeType == ExpressionType.Parameter)
                    containsParameter = true;
                else
                    DynamicExpressions.Add(node);
            }
            containsParameter |= originalContainsParameter;

            return node;
        }
    }
}";

        public const string TokenResolverTemplate =
@"using {project}.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace {namespace};

public class TokenResolver : ITokenResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TokenResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> GetTokenAsync(string audience = null)
    {
        var signingCredentials = new SigningCredentials({project}ApiConstants.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            issuer: {project}WebConstants.{project}WebIssuer,
            audience: !string.IsNullOrWhiteSpace(audience) ? audience : {project}ApiConstants.{project}ApiAudience,
            signingCredentials: signingCredentials,
            claims: _httpContextAccessor.HttpContext?.User.Claims.ToList()
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GetTokenAsync(string audience = null, ClaimsPrincipal claims = null)
    {
        var signingCredentials = new SigningCredentials({project}ApiConstants.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            issuer: {project}WebConstants.{project}WebIssuer,
            audience: !string.IsNullOrWhiteSpace(audience) ? audience : {project}ApiConstants.{project}ApiAudience,
            signingCredentials: signingCredentials,
            claims: claims.Claims != null ? claims.Claims : _httpContextAccessor.HttpContext?.User.Claims.ToList()
        );
        

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public interface ITokenResolver
{
    public Task<string> GetTokenAsync(string audience = null);
    public Task<string> GetTokenAsync(string audience = null, ClaimsPrincipal claims = null);
}

";

        public const string CurrentUserTemplate =
@"namespace {namespace};

public class CurrentUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Reference { get; set; }
    public bool IsSignedIn { get; set; }
}";

        public const string IdentityResolverTemplate =
@"using {project}.Shared.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace {namespace};

public class IdentityResolver : IIdentityResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CurrentUser> GetCurrentAccountAsync()
    {
        var currentUser = new CurrentUser();

        var firstName = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ""FirstName"")?.Value;
        var lastName = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ""LastName"")?.Value;
        var username = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ""UserName"")?.Value;
        var email = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ""Email"")?.Value;
        var reference = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ""Reference"")?.Value;

        currentUser.FirstName = firstName;
        currentUser.LastName = lastName;
        currentUser.Username = username;
        currentUser.Email = email;
        currentUser.Reference = reference;
        currentUser.IsSignedIn = !string.IsNullOrWhiteSpace(currentUser.Reference);
        
        return currentUser;
    }
}

public interface IIdentityResolver
{
    public Task<CurrentUser> GetCurrentAccountAsync();
}";

        public const string PayloadResolverTemplate =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {project}.Shared.Utilities;
public class PayloadResolver : IPayloadResolver
{
    private readonly ITokenResolver _tokenResolver;

    public PayloadResolver(ITokenResolver tokenResolver)
    {
        _tokenResolver = tokenResolver;
    }

    public async Task<HttpPayload> GetPayloadAsync()
    {
        return new HttpPayload
        {
            SecurityToken = await _tokenResolver.GetTokenAsync(null),
            Uri = {project}ApiConstants.HostUrl
        };
    }
}

public interface IPayloadResolver
{
    public Task<HttpPayload> GetPayloadAsync();
}

";


    }
}
