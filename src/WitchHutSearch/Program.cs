using System.Diagnostics;
using Microsoft.Extensions.Logging;
using WitchHutSearch;
using WitchHutSearch.Generator;

const ulong seed = 10579846526078875722UL; // One of the Hermitcraft seeds if you're curious
var stopwatch = new Stopwatch();
stopwatch.Start();
var threads = Environment.ProcessorCount * 2;
var loggerFactory = LoggerFactory.Create(b => b.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

var requirements = new SearchRequirements { Count = 2 };
var generator = new BiomeGenerator(seed);
var collation = new HutCollation(loggerFactory.CreateLogger<HutCollation>(), requirements);
var worker = new SearchWorker(loggerFactory.CreateLogger<SearchWorker>(), collation, generator);

logger.LogInformation("Searching for {Huts} huts on seed: {Seed}",
    requirements.Count, unchecked((long)seed));

var workers = requirements.CreateSearchRanges(threads)
    .Select(r => new Thread(() => worker.Search(r)))
    .ToArray();

foreach (var thread in workers)
    thread.Start();

foreach (var thread in workers)
    thread.Join();

logger.LogInformation("Search completed - printing results to console");

foreach (var hut in collation.Centres.OrderBy(c => c.DistanceFromOrigin))
    Console.WriteLine($"{hut.Huts} at {hut.X}, {hut.Z}");

stopwatch.Stop();
logger.LogInformation("Search completed in {ElapsedSeconds} seconds",
    stopwatch.Elapsed.TotalSeconds.ToString("F4"));
    