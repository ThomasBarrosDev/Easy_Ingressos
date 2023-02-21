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
                var response = await client.PostAsync(LoginResponse.Route, Opptions.Content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<LoginResponse>(responseContent);
                    Opptions.Token = responseObject.token;

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
                var response = await client.GetAsync(WelcomeResponse.Route);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseObject = JsonSerializer.Deserialize<ConectionServer>(responseContent);

                    DialogResult result = MessageBox.Show(responseObject.data[0].age_classification.ToString(), "dados", MessageBoxButtons.OK);

                    EventManager.eventData = responseObject.data;

                    EventManager.SetSqlData(responseObject.data[0]);
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
        public static Uri BaseURL { get; set; } = new Uri("http://localhost:8000/");
        public static string Token;
        public static HttpContent Content = new StringContent(JsonSerializer.Serialize(new Login()), Encoding.UTF8, "application/json");
    }


    public class Login
    {
        public string email { get; set; } = "thomas";
        public string password { get; set; } = "123";
    }
    public class LoginResponse
    {
        public string token { get; set; }
        public bool userValidated { get; set; }
        public static string Route => "auth/login";
        public static string Query => "";

    }

    public class WelcomeResponse
    {
        public string token { get; set; }
        public bool userValidated { get; set; }
        public static string Route => "event";
        public static string Query => "";

    }

}

