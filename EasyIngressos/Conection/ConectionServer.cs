using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EasyIngressos.Entity;
using EasyIngressos.Managers;

namespace EasyIngressos.Conection
{
    public class ConectionServer
    {
        public EventData[] data { get; set; }
        public static async Task AuthenticateBackend()
        {
            using var client = new HttpClient();
            client.BaseAddress = Opptions.BaseURL;

            try
            {
                var response = await client.PostAsync(LoginRequest.Route, Opptions.Content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<LoginRequest>(responseContent);
                    Opptions.Token = responseObject.data.token;

                    DialogResult result = MessageBox.Show("Sincronizado com o servidor", "Sincronized", MessageBoxButtons.OK);

                    if (result == DialogResult.OK)
                    {
                        await GetData();
                    }
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }finally
            {
                client.Dispose();
            }

        }

        public static async Task GetData()
        {
            using var client = new HttpClient();
            client.BaseAddress = Opptions.BaseURL;
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Opptions.Token}");

            try
            {
                var response = await client.GetAsync(EventsResponse.Route);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<ConectionServer>(responseContent);
                    
                    EventManager.eventData = responseObject.data;
                }
                else
                {
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                client.Dispose();
            }


        }
    }
    public class Opptions
    {
        public static Uri BaseURL { get; set; } = new Uri("https://api.easy.risestudio.com.br/");
        public static string Token;
        public static HttpContent Content = new StringContent(JsonSerializer.Serialize(new Login()), Encoding.UTF8, "application/json");
    }


    public class Login
    {
        public string email { get; set; } = "master@risestudio.com.br";
        public string password { get; set; } = "master*123@";
    }
    public class LoginRequest
    {
        public LoginRespose data { get; set; }
        public bool userValidated { get; set; }
        public static string Route => "admin/login";
        public static string Query => "";
    }

    public class EventsResponse
    {
        public string token { get; set; }
        public bool userValidated { get; set; }
        public static string Route => "system/events";
        public static string Query => "";

    }
    public class LoginRespose
    {
        public string token { get; set; }
        public string type { get; set; }
        public string refreshToken { get; set; }
    }

}

