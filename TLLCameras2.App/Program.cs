    
const string baseUrl = "https://ristmikud.tallinn.ee/last";
const string fullDirectory = @"./images";

await RunCamera(39);

async Task RunCamera(int camera)
{
    do
    {
        try
        {
            var timestamp = (DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .TotalSeconds;

            var filename = $"{camera}_{timestamp}.jpg";
            var fullFilename = Path.Join(fullDirectory, filename);

            await using var imageStream = await GetImage(camera);
            await using var fileStream = File.OpenWrite(fullFilename);

            await imageStream.CopyToAsync(fileStream);

            await Console.Out.WriteLineAsync($"{timestamp} camera {camera} updated.");
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Failed to save image! {e.Message}");
        }

        Thread.Sleep(5000);
    } while (true);
}

async Task<Stream> GetImage(int camera)
{
    using var http = new HttpClient();
    var nonce = (DateTimeOffset.UtcNow - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
        .TotalSeconds;
    var response = await http.GetAsync($"{baseUrl}/cam{camera:D3}.jpg?{nonce}");
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadAsStreamAsync();
}