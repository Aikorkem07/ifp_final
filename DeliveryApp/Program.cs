using DeliveryApp;
using System.Collections.Immutable;

if(args.Length>0 && args[0].Equals("final",StringComparison.OrdinalIgnoreCase))
{ Environment.ExitCode=DeliveryApp.FinalDemo.Run(args); return; }

var command = args.FirstOrDefault()?.ToLowerInvariant() ?? "demo";

var root = Path.Combine("DeliveryApp", "samples");

var file = command switch
{
    "demo" => "deliveries.json",
    "empty" => "empty.json",
    "invalid" => "invalid.json",
    "malformed" => "malformed.json",
    "missing" => "missing.json",
    _ => "deliveries.json"
};

var loaded = DeliveryFileBoundary.Load(Path.Combine(root, file));

if (loaded is Result<ImmutableArray<ValidatedDeliveryRequest>, DomainError>.Err failure)
{
    Console.Error.WriteLine($"Input error [{failure.Error.Code}]: {failure.Error.Message}");
    Environment.ExitCode = 1;
    return;
}

var requests = ((Result<ImmutableArray<ValidatedDeliveryRequest>, DomainError>.Ok)loaded).Value;

var standard = CurriedPricing.CreateScore(500m)(100m);
var express = CurriedPricing.CreateScore(800m)(150m);

Console.WriteLine("DELIVERY COST CALCULATOR — ASSIGNMENT 8");

Console.WriteLine("\nREPORT 1: Delivery quotes (standard pricing)");

foreach (var row in DeliveryPipeline.DetailedReport(requests, standard))
{
    Console.WriteLine($"{row.Id,-8} {row.DistanceKm,6:0.##} km | {row.Cost.ToKzt(),12} | Zone {row.Zone}");
}

Console.WriteLine("\nREPORT 2: Totals grouped by native delivery zone (express pricing)");

foreach (var row in DeliveryPipeline.ZoneReport(requests, express))
{
    Console.WriteLine($"Zone {row.Zone}: {row.Count} deliveries, {row.TotalCost.ToKzt()}");
}

var state = DeliveryPipeline.Complete(
    DeliveryPipeline.Start(requests),
    PricingPolicies.Standard
);

if (state is DeliveryWorkflowState.Completed done)
{
    Console.WriteLine(
        $"\nWorkflow total: {done.Summary.TotalCost.ToKzt()} | Accepted: {done.Summary.AcceptedCount} | Rejected: {done.Summary.RejectedCount}"
    );
}

var processed = 0;

var deferred = DeliveryPipeline.StreamEligible(requests, () => processed++);

Console.WriteLine($"\nDeferred stream created; processed before enumeration: {processed}");

var snapshot = deferred.ToArray();

Console.WriteLine($"After ToArray: processed {processed}; snapshot contains {snapshot.Length} deliveries.");

Console.WriteLine(
    $"\nCurried pricing for 5 km: Standard={standard(5m).ToKzt()}, Express={express(5m).ToKzt()}"
);

Console.WriteLine("Native C integration: delivery zone is calculated by delivery_zone() through P/Invoke.");
