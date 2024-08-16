using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;


class MyApiAbstraction
{
  private readonly HttpClient _client;

  public MyApiAbstraction(string token)
  {
    _client = new HttpClient() { BaseAddress = new Uri("https://api.github.com/") };
    _client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
  }

  public async Task<string> GetUser()
  {
    HttpResponseMessage response = await _client.GetAsync("user");
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
  }
}

class Program
{
  /// <summary>
  /// Function to load environment variables from .env file
  /// </summary>
  static void LoadDotEnv()
  {
    try
    {
      foreach (string line in File.ReadAllLines(".env"))
      {
        if (line.StartsWith("#"))
          continue;

        string[] parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Debug.Assert(parts.Length == 2);
        Environment.SetEnvironmentVariable(parts[0], parts[1]);
      }
    }
    catch (FileNotFoundException)
    { }
  }

  static string GetEnvVariable(string name)
  {
    string? value = Environment.GetEnvironmentVariable(name);
    if (value == null)
    {
      Console.WriteLine($"{name} not defined in environment or .env file");
      Environment.Exit(1);
    }
    return value;
  }

  static async Task Main(string[] args)
  {
    LoadDotEnv();

    var token = GetEnvVariable("GITHUB_TOKEN");

    MyApiAbstraction api = new MyApiAbstraction(token);
    string user = await api.GetUser();
    Console.WriteLine(user);
  }
}