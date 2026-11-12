namespace DeliveryApp;
public sealed record StatusCount(DeliveryStatus Status,int Count,decimal Revenue);
public static class DeliveryLedgerReports
{
    public static DeliveryOrder[] ActiveOrders(DeliveryLedger ledger)
        => ledger.Orders.Where(x=>x.Status is DeliveryStatus.Confirmed or DeliveryStatus.Dispatched)
            .OrderByDescending(x=>x.Price).ThenBy(x=>x.Id,StringComparer.Ordinal).ToArray();
    public static StatusCount[] ByStatus(DeliveryLedger ledger)
        => ledger.Orders.GroupBy(x=>x.Status)
            .Select(g=>new StatusCount(g.Key,g.Count(),g.Sum(x=>x.Price)))
            .OrderBy(x=>x.Status).ToArray();
}
