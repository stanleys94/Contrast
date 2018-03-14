using CONTRAST_WEB.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CONTRAST_WEB.Providers
{
    public class JwtProvider
    {
        private static string _tokenUri;

        //default constructor
        public JwtProvider() { }

        public static JwtProvider Create(string tokenUri)
        {
            _tokenUri = tokenUri;
            return new JwtProvider();
        }

        public async Task<string> GetTokenAsync(string TAMSignOnToken)
        {
            using (var client = new HttpClient())
            {
                /*
                client.BaseAddress = new Uri(_tokenUri);
                client.DefaultRequestHeaders.Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                var content = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("Username", username),
                        new KeyValuePair<string, string>("Password", password),
                        //new KeyValuePair<string, string>("grant_type", "password"),
                        //new KeyValuePair<string, string>("device_id", deviceId),
                        new KeyValuePair<string, string>("AppId", clientId),
                    });
                var response = await client.PostAsync(string.Empty, content);
                */
                ///*
                client.BaseAddress = new Uri("https://tam-passport.azurewebsites.net/api/v1/token");

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                TokenJwt model = new TokenJwt();
                /*
                model.Username = username;
                model.Password = password;
                model.AppId = clientId;
                */
                HttpResponseMessage response = client.PostAsync(string.Empty, new StringContent(
                                                new JavaScriptSerializer().Serialize(model), System.Text.Encoding.UTF8, "application/json")).Result
                                               ;
                //*/
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    //return null if unauthenticated
                    return null;
                }
            }
        }

        public JObject DecodePayload(string token)
        {
            var parts = token.Split('.');
            var payload = parts[1];

            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            return JObject.Parse(payloadJson);
        }

        public ClaimsIdentity CreateIdentity(bool isAuthenticated, string userName, dynamic payload)
        {
            //decode the payload from token
            //in order to create a claim            
            //string userId = payload.nameid;
            string iss = payload.iss;
            string aud = payload.aud;
            string sub = payload.sub;
            string iat = payload.iat;
            string exp = payload.exp;
            string jti = payload.jti;
            string unique_name = payload.unique_name;
            string family_name = payload.family_name;
            string email = payload.email;
            string EmployeeId = payload.EmployeeId;

            //spesial gendam
            if (Convert.ToInt32(EmployeeId) > 200000) EmployeeId = (Convert.ToInt32(EmployeeId) - 100000).ToString();
            if(EmployeeId.CompareTo("CONTRASTAP") ==0) EmployeeId= "101419";
            if(EmployeeId.CompareTo("CONTRASTUSER")==0 ||EmployeeId.Trim().ToLower().CompareTo("contrastuser")==0||EmployeeId.Trim().ToLower().CompareTo("contrasts.user")==0) EmployeeId = "100613";
            
            string[] roles = payload.roles.ToObject(typeof(string[]));
            

            var jwtIdentity = new ClaimsIdentity(new JwtIdentity(
                isAuthenticated, EmployeeId, DefaultAuthenticationTypes.ApplicationCookie
                    ));
            
            //add user id
            jwtIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, EmployeeId));
            //add roles
            foreach (var role in roles)
            {
                jwtIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
           
            return jwtIdentity;
        }

        private byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }
    }


    public class JwtIdentity : IIdentity
    {
        private bool _isAuthenticated;
        private string _name;
        private string _authenticationType;

        public JwtIdentity() { }
        public JwtIdentity(bool isAuthenticated, string name, string authenticationType)
        {
            _isAuthenticated = isAuthenticated;
            _name = name;
            _authenticationType = authenticationType;
        }
        public string AuthenticationType
        {
            get
            {
                return _authenticationType;
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return _isAuthenticated;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }
    }
}
