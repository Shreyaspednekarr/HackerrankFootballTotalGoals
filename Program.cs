using Newtonsoft.Json;
using System.Net.Http.Headers;

public class DataPerPage
{
    public int Page { get; set; }
    public int Per_Page { get; set; }
    public int Total { get; set; }
    public int Total_Pages { get; set; }
    public IEnumerable<Match> Data { get; set; }
}

public class Match
{
    public string Competition { get; set; }
    public int Year { get; set; }
    public string Round { get; set; }
    public string Team1 { get; set; }
    public string Team2 { get; set; }
    public int Team1Goals { get; set; }
    public int Team2Goals { get; set; }
}

class Result
{

    public static async Task<int> getTotalGoals(string team, int year)
    {
        int totalGoals = 0;
        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new Uri("https://jsonmock.hackerrank.com/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage team1Response = await httpClient.GetAsync("api/football_matches?year=" + year + "&team1=" + team + "&page=1").ConfigureAwait(false);
            HttpResponseMessage team2Response = await httpClient.GetAsync("api/football_matches?year=" + year + "&team2=" + team + "&page=1").ConfigureAwait(false);
            if (team1Response.IsSuccessStatusCode && team2Response.IsSuccessStatusCode)
            {
                var team1JsonStr = team1Response.Content.ReadAsStringAsync().Result;
                var team2JsonStr = team2Response.Content.ReadAsStringAsync().Result;

                var team1Result = JsonConvert.DeserializeObject<DataPerPage>(team1JsonStr);
                var team2Result = JsonConvert.DeserializeObject<DataPerPage>(team2JsonStr);

                totalGoals += team1Result.Data.Where(x => x.Team1 == team).Sum(x => x.Team1Goals);
                totalGoals += team2Result.Data.Where(x => x.Team2 == team).Sum(x => x.Team2Goals);

                for(int i = 2; i <= team1Result.Total_Pages; i++)
                {
                    team1Response = await httpClient.GetAsync("api/football_matches?year=" + year + "&team1=" + team + "&page=" + i).ConfigureAwait(false);
                    team1JsonStr = team1Response.Content.ReadAsStringAsync().Result;
                    team1Result = JsonConvert.DeserializeObject<DataPerPage>(team1JsonStr);
                    totalGoals += team1Result.Data.Where(x => x.Team1 == team).Sum(x => x.Team1Goals);
                }

                for (int i = 2; i <= team2Result.Total_Pages; i++)
                {
                    team2Response = await httpClient.GetAsync("api/football_matches?year=" + year + "&team2=" + team + "&page=" + i).ConfigureAwait(false);
                    team2JsonStr = team2Response.Content.ReadAsStringAsync().Result;
                    team2Result = JsonConvert.DeserializeObject<DataPerPage>(team2JsonStr);
                    totalGoals += team2Result.Data.Where(x => x.Team2 == team).Sum(x => x.Team2Goals);
                }
            }
        }
        return totalGoals;
    }

}

public class Program
{
    private static void Main(string[] args)
    {
        var result = Result.getTotalGoals("Barcelona", 2011).Result;
        Console.WriteLine("Hello, World!");
    }
}