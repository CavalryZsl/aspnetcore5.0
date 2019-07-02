using Moq;
using System;
using System.Data;
using Working.Models.Repository;
using Xunit;
using Dapper;
using Working.Models.DataModel;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Net.Http.Headers;
using System.Linq;

namespace Working.XUnitTest
{
    /// <summary>
    /// HomeController����
    /// </summary>
    [Trait("Controller���ɲ���", "HomeController")]
    public class HomeControllerIntegrationTesting : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        public HomeControllerIntegrationTesting(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }
        /***************************
         * 
         * ע�⣬���ɲ���ʱ��Ҫ�ѿ����Working.XunitTest bin dubgeĿ¼һ��
         * 
         **************************/

        /// <summary>
        /// ����û��Ȩ��ʱ������ض������
        /// </summary>
        [Fact]
        public void GetUsers_Redirect_Return()
        {
            var request = "/userindex";
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var response = client.GetAsync(request).Result;
            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.StartsWith("http://localhost/login",
                response.Headers.Location.OriginalString);
        }
        [Fact]
        public void Login_Redirect_Return()
        {
            var request = "/login";
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("userName", "gsw"));
            values.Add(new KeyValuePair<string, string>("password", "gsw"));
            values.Add(new KeyValuePair<string, string>("returnUrl", "/"));
            var content = new FormUrlEncodedContent(values);
            var response = client.PostAsync(request, content).Result;
            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/", response.Headers.Location.ToString());
        }
        /// <summary>
        /// get����
        /// </summary>
        [Fact]
        public void GetUserRoles_Default_Return()
        {
            var cookies = Login();
            Assert.NotNull(cookies);
            //��¼�ɹ����ѯ
            var request = "/userroles?departmentID=1";
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("Cookie", cookies);
            var response = client.GetAsync(request).Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            response.EnsureSuccessStatusCode();
            var responseJson = response.Content.ReadAsStringAsync().Result;
            var backJson = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseJson>(responseJson);
            Assert.Equal(2, (backJson.data as IList)?.Count);
        }

        IEnumerable<string> Login()
        {
            //��¼��ȡcookie
            var request = "/login";
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("userName", "admin"));
            values.Add(new KeyValuePair<string, string>("password", "admin"));
            values.Add(new KeyValuePair<string, string>("returnUrl", "/"));
            var content = new FormUrlEncodedContent(values);
            var response = client.PostAsync(request, content).Result;
            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> list))
            {
                return list;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// post����
        /// </summary>
        [Fact]
        public void AddUser_Default_Return()
        {
            var cookies = Login();
            Assert.NotNull(cookies);
            //��¼�ɹ�������û�
            var request = "/adduser";
            var data = new Dictionary<string, string>();
            data.Add("ID", "1");
            data.Add("DepartmentID", "2");
            data.Add("RoleID", "1");
            data.Add("UserName", "wangwu");
            data.Add("Password", "wangwu");
            data.Add("Name", $"����{DateTime.Now.ToString("MMddHHmmss")}");
            var content = new FormUrlEncodedContent(data);
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("Cookie", cookies);
            var response = client.PostAsync(request, content).Result;
            response.EnsureSuccessStatusCode();
            var responseJson = response.Content.ReadAsStringAsync().Result;
            var backJson = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseJson>(responseJson);
            Assert.Equal(1, backJson.result);
        }

    }
}
