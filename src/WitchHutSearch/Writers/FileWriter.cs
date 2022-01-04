using Microsoft.Extensions.Logging;
using WitchHutSearch.Searcher;

namespace WitchHutSearch.Writers;

public sealed class FileWriter : IHutWriter
{
    private readonly ILogger _logger;
    private readonly string _filePath;
    private readonly FileType _fileType;
    private readonly StreamWriter _stream;

    public FileWriter(ILogger logger, string filename)
    {
        _logger = logger;
        _filePath = Path.Join(Environment.CurrentDirectory, filename.Trim());
        _fileType = Path.GetExtension(_filePath) == ".csv" ? FileType.Csv : FileType.Text;
        _stream = new StreamWriter(_filePath);
    }

    public Task BeginAsync()
        => _fileType == FileType.Csv
            ? _stream.WriteLineAsync("huts,x,z")
            : Task.CompletedTask;

    public async Task<bool> WriteAsync(HutCentre centre)
    {
        await _stream.WriteLineAsync(
            _fileType == FileType.Csv
                ? $"{centre.Huts},{centre.X},{centre.Y}"
                : $"{centre.Huts} huts at {centre.X}, {centre.Y}");
        return true;
    }

    public async Task CompleteAsync(int count)
    {
        await _stream.FlushAsync();
        _stream.Close();

        _logger.LogInformation("Wrote {Locations} locations to {File}", count, _filePath);
    }

    void IDisposable.Dispose()
        => _stream.Dispose();

    private enum FileType
    {
        Text,
        Csv
    }
}