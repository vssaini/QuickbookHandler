using Intuit.Ipp.OAuth2PlatformClient;
using QuickbookHandler.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Claims;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace QuickbookHandler
{
    /// <summary>
    /// Summary description for Handler
    /// </summary>
    public class Handler : IHttpHandler, IRequiresSessionState
    {
        private string _state;

        private const string HomeUrl = "/Index.html";

        public static string ClientId = ConfigurationManager.AppSettings["clientId"];
        public static string ClientSecret = ConfigurationManager.AppSettings["clientSecret"];
        public static string RedirectUrl = ConfigurationManager.AppSettings["redirectUrl"];
        public static string Environment = ConfigurationManager.AppSettings["appEnvironment"];

        public static OAuth2Client Auth2Client = new OAuth2Client(ClientId, ClientSecret, RedirectUrl, Environment);

        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.Params["Authenticate"] != null)
            {
                // User has not authenticated to quickbook
                InitiateAuth(context);
            }

            // Get auth token using code
            if (context.Request.Params["code"] != null)
            {
                //Sync the state info and update if it is not the same
                var state = context.Request.Params["state"];
                if (!state.Equals(Auth2Client.CSRFToken, StringComparison.Ordinal))
                {
                    _state = state + " (invalid)";
                    var url = $"{HomeUrl}?state={_state}";
                    context.Response.Redirect(url);
                }

                string code = context.Request.Params["code"] ?? "none";
                string realmId = context.Request.Params["realmId"] ?? "none";
                GetAuthTokens(context, code, realmId);

                if (context.Request.Params["error"] != null)
                {
                    var url = $"{HomeUrl}?error={context.Request.Params["error"]}";
                    context.Response.Redirect(url);
                }
                else
                {
                    var url = $"{HomeUrl}?connectionStatus=true";
                    context.Response.Redirect(url);
                }
            }

            // Check if user passed parameter
            var cModel = Deserialize<CustomerModel>(context);
            if (cModel != null)
            {
                AddCustomer(context, cModel);
            }
        }

        private static void InitiateAuth(HttpContext context)
        {
            var scopes = new List<OidcScopes> { OidcScopes.Accounting };
            string authorizeUrl = Auth2Client.GetAuthorizationURL(scopes);

            var json = new JavaScriptSerializer().Serialize(authorizeUrl);
            context.Response.ContentType = "text/json";
            context.Response.Write(json);
        }

        /// <summary>
        /// Exchange Auth code with Auth Access and Refresh tokens and add them to Claim list
        /// </summary>
        private static void GetAuthTokens(HttpContext context, string code, string realmId)
        {
            if (realmId != null)
            {
                context.Session["realmId"] = realmId;
            }

            context.Request.GetOwinContext().Authentication.SignOut("TempState");
            var tokenResponse = Auth2Client.GetBearerTokenAsync(code).Result;

            var claims = new List<Claim>();

            if (context.Session["realmId"] != null)
            {
                claims.Add(new Claim("realmId", context.Session["realmId"].ToString()));
            }

            if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                claims.Add(new Claim("access_token", tokenResponse.AccessToken));
                claims.Add(new Claim("access_token_expires_at", DateTime.Now.AddSeconds(tokenResponse.AccessTokenExpiresIn).ToString()));
            }

            if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
            {
                claims.Add(new Claim("refresh_token", tokenResponse.RefreshToken));
                claims.Add(new Claim("refresh_token_expires_at", DateTime.Now.AddSeconds(tokenResponse.RefreshTokenExpiresIn).ToString()));
            }

            var id = new ClaimsIdentity(claims, "Cookies");
            context.Request.GetOwinContext().Authentication.SignIn(id);
        }

        private static void AddCustomer(HttpContext context, CustomerModel cModel)
        {
            object data = new
            {
                Status = false,
                Message = "RealmId not found in session"
            };

            if (context.Session["realmId"] == null)
            {
                var json = new JavaScriptSerializer().Serialize(data);
                context.Response.ContentType = "text/json";
                context.Response.Write(json);
            }
            else
            {
                var realmId = context.Session["realmId"].ToString();
                try
                {
                    if (!(context.User is ClaimsPrincipal principal))
                    {
                        throw new Exception("User principal is empty.");
                    }

                    var serviceContext = QbHelper.GetServiceContext(principal, realmId);
                    var addedCustomer = QbHelper.AddCustomer(serviceContext, cModel);
                    data = new
                    {
                        Status = addedCustomer != null,
                        Message = addedCustomer == null ? "Failed to add customer" : "Customer was added successfully!"
                    };
                }
                catch (Exception ex)
                {
                    data = new
                    {
                        Status = false,
                        Message = $"Failed to add customer. More details - {ex.Message}"
                    };
                }

                var json = new JavaScriptSerializer().Serialize(data);
                context.Response.ContentType = "text/json";
                context.Response.Write(json);
            }
        }

        /// <summary>
        /// This function will take HttpContext object and will read the input stream.
        /// It will use the built in JavascriptSerializer framework to deserialize object based given T object type value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        private static T Deserialize<T>(HttpContext context)
        {
            //read the json string
            string jsonData = new StreamReader(context.Request.InputStream).ReadToEnd();

            //cast to specified objectType
            var obj = new JavaScriptSerializer().Deserialize<T>(jsonData);

            //return the object
            return obj;
        }
    }
}