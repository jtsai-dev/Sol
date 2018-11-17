using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string GetToken(string ModelJson)
        {
            return JwtTokenHandler.CreateJwtToken();
        }

        [HttpPost]
        public string Valid()
        {
            var user = _sesson;
            return "ok";
        }

        [HttpPost]
        public string Upload()
        {
            foreach (var file in Request.Files)
            {

            }
            return "ok";
        }
    }

    [Auth]
    public class BaseController : Controller
    {
        protected readonly ClaimsSession _sesson;
        public BaseController()
        {
            _sesson = new ClaimsSession();
        }
    }

    public class JwtTokenHandler
    {
        private static string _key = "jtjtjtjtjtjtjtjtjtjt";
        private static string _issuer = "Jt_Server";
        private static string _audience = "Jt_Client";

        public static string CreateJwtToken()
        {
            var now = DateTime.UtcNow;
            var notbefore = now.AddSeconds(10);
            var expires = now.AddHours(12);

            var claimsIdentity = new ClaimsIdentity(new[]
           {
                new Claim(JtClaimTypes.AccontId, "1"),
                new Claim(JtClaimTypes.AccountName, "jt"),
                new Claim(JtClaimTypes.NickName, "JTsai"),
            });

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateJwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                subject: claimsIdentity,
                notBefore: notbefore,
                expires: expires,
                issuedAt: now,
                signingCredentials: signingCredentials);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        private static bool LifetimeValidator(
            DateTime? notBefore, 
            DateTime? expires, 
            SecurityToken securityToken, 
            TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires)
                    return true;
            }
            return false;
        }

        public static void RetiveToken(string token)
        {
            try
            {
                SecurityToken securityToken;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    ValidAudience = _audience,
                    ValidIssuer = _issuer,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = LifetimeValidator,
                    IssuerSigningKey = securityKey,
                };

                var principal = handler.ValidateToken(token, validationParameters, out securityToken);
                Thread.CurrentPrincipal = principal;
                HttpContext.Current.User = principal;
            }
            catch (SecurityTokenInvalidLifetimeException ex)
            {
                throw new SecurityTokenInvalidLifetimeException("token超时，请重新换取Token：" + ex.Message);
            }
            catch (SecurityTokenValidationException ex)
            {
                throw new SecurityTokenValidationException("无效的Token：" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("接口鉴权失败，原因：" + ex.Message);
            }
        }
    }
    public class JtClaimTypes
    {
        public static string AccontId { get; set; } = ClaimTypes.NameIdentifier;
        public static string AccountName { get; set; } = ClaimTypes.Name;
        public static string NickName { get; set; } = ClaimTypes.GivenName;
    }

    public class AuthAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //return base.AuthorizeCore(httpContext);
            var token = string.Empty;
            if (!TryGetToken(HttpContext.Current.Request, out token))
            {
                return true;
            }
            if (string.IsNullOrEmpty(token))
            {
                // todo: null token
                return false;
            }
            // todo: valid token
            return ValidToken(token);
        }

        private bool ValidToken(string token)
        {
            JwtTokenHandler.RetiveToken(token);
            return true;
        }

        private static string[] _whiteList = new string[] {
            "home/gettoken",
        };
        private bool TryGetToken(HttpRequest request, out string token)
        {
            token = null;

            if (_whiteList.Any(p => request.Url.AbsolutePath.ToLower().Contains(p)))
            {
                return false;
            }

            var authHeaders = request.Headers.GetValues("auth");
            if (authHeaders != null && authHeaders.Count() > 0)
            {
                token = authHeaders.ElementAt(0);
            }
            return true;
        }
    }

    public class WebApiPrincipalAccessor
    {
        public virtual ClaimsPrincipal Principal => HttpContext.Current.User as ClaimsPrincipal ?? Thread.CurrentPrincipal as ClaimsPrincipal;
        public static WebApiPrincipalAccessor Instance => new WebApiPrincipalAccessor();
    }
    public class ClaimsSession
    {
        protected WebApiPrincipalAccessor PrincipalAccessor { get; }
        internal ClaimsSession()
        {
            PrincipalAccessor = WebApiPrincipalAccessor.Instance;
        }
        public long AccountId
        {
            get
            {
                var memberIdClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == JtClaimTypes.AccontId);
                if (string.IsNullOrEmpty(memberIdClaim?.Value))
                {
                    throw new Exception("接口认证失败");
                }

                string memberId = memberIdClaim.Value;
                return long.Parse(memberId);
            }
        }
        public string AccountName
        {
            get
            {
                var accountNameClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == JtClaimTypes.AccountName);
                return accountNameClaim.Value;
            }
        }
        public string NickName
        {
            get
            {
                var nickNameClaim = PrincipalAccessor.Principal?.Claims.FirstOrDefault(c => c.Type == JtClaimTypes.NickName);
                return nickNameClaim.Value;
            }
        }
    }
}