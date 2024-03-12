using PuppeteerSharp;

namespace ScrappingCalcJurosCompostos
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var result = LoadXXXAsync().Result;
            result.ForEach(r => { Console.WriteLine(r); });
        }

        private static async Task<List<string>> LoadXXXAsync()
        {
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });



            var result = new List<string>();
            using (var page = await browser.NewPageAsync())
            {
                for (var periodo = 1; periodo <= 129; periodo++)
                {
                    await page.GoToAsync($"https://www.mobills.com.br/calculadoras/calculadora-juros-compostos/resultado/?valor-inicial=1500&valor-mensal=0&taxa-de-juros=0.00643403011000343&juros_tipo=Anual&periodo={periodo}&periodo_tipo=Meses&juros=8%2C00");
                    await page.WaitForSelectorAsync("span#ci-total");
                    var totalHandle = await page.QuerySelectorAsync("span#ci-total");
                    var totalTextHandle = await totalHandle.GetPropertyAsync("innerText");
                    var innerText = await totalTextHandle.JsonValueAsync();

                    result.Add($"{periodo}*{innerText.ToString().Replace("R$ ", "")}");
                }

                await page.CloseAsync();
            }

            await browser.CloseAsync();

            return await Task.FromResult(result);
        }
    }
}
