namespace DeliveryApp;
public static class FinalDemo
{
    public static int Run(string[] args)
    {
        var ledger=DeliveryLedger.Empty;
        var register=DeliveryOperations.Register(ledger,"DEL-001",5m);
        if(register is not Result<DeliveryLedger,DomainError>.Ok r){Console.WriteLine(register);return 1;}
        ledger=r.Value;
        var confirm=DeliveryOperations.Confirm(ledger,"DEL-001",PricingPolicies.Standard);
        if(confirm is not Result<DeliveryLedger,DomainError>.Ok c){Console.WriteLine(confirm);return 1;}
        ledger=c.Value;
        var dispatched=DeliveryOperations.Dispatch(ledger,"DEL-001");
        if(dispatched is not Result<DeliveryLedger,DomainError>.Ok d){Console.WriteLine(dispatched);return 1;}
        ledger=d.Value;
        var delivered=DeliveryOperations.Deliver(ledger,"DEL-001");
        if(delivered is not Result<DeliveryLedger,DomainError>.Ok done){Console.WriteLine(delivered);return 1;}
        ledger=done.Value;
        Console.WriteLine("FINAL DELIVERY APP: register > confirm > dispatch > deliver");
        foreach(var order in ledger.Orders)Console.WriteLine($"{order.Id}: {order.Status}, {order.Price.ToKzt()}");
        Console.WriteLine("Status report:");
        foreach(var item in DeliveryLedgerReports.ByStatus(ledger))Console.WriteLine($"{item.Status}: {item.Count} / {item.Revenue.ToKzt()}");
        var path=args.Length>1?args[1]:Path.Combine("DeliveryApp","samples","final-orders.json");
        var saved=LedgerJsonStore.Save(path,ledger);
        Console.WriteLine(saved is Result<string,DomainError>.Ok ? $"Saved: {path}" : $"Save failed: {saved}");
        return saved is Result<string,DomainError>.Ok?0:1;
    }
}
