using System.Collections.Immutable;
namespace DeliveryApp;
// Final-project extension: six meaningful delivery workflows sharing one immutable state.
public enum DeliveryStatus { Draft, Confirmed, Dispatched, Delivered, Cancelled }
public sealed record DeliveryOrder(string Id, decimal DistanceKm, decimal Price, DeliveryStatus Status);
public sealed record DeliveryLedger(ImmutableArray<DeliveryOrder> Orders)
{
    public static DeliveryLedger Empty => new(ImmutableArray<DeliveryOrder>.Empty);
}
public static class DeliveryOperations
{
    public static Result<DeliveryLedger,DomainError> Register(DeliveryLedger ledger,string? id,decimal km)
    {
        var valid=ValidatedDeliveryRequest.Create(id,km);
        return valid.Bind(request =>
        {
            if (request.DistanceKm>DeliveryPipeline.MaximumDistance)
                return new Result<DeliveryLedger,DomainError>.Err(new("OUT_OF_RANGE","Delivery is outside the 20 km service area."));
            if (ledger.Orders.Any(x=>string.Equals(x.Id,request.Id,StringComparison.OrdinalIgnoreCase)))
                return new Result<DeliveryLedger,DomainError>.Err(new("DUPLICATE_ID","Delivery ID already exists."));
            return new Result<DeliveryLedger,DomainError>.Ok(ledger with
            { Orders=ledger.Orders.Add(new DeliveryOrder(request.Id,request.DistanceKm,0,DeliveryStatus.Draft)) });
        });
    }
    public static Result<DeliveryLedger,DomainError> Confirm(DeliveryLedger ledger,string id,Func<decimal,decimal> price)
    {
        ArgumentNullException.ThrowIfNull(price);
        return Change(ledger,id,DeliveryStatus.Draft,DeliveryStatus.Confirmed,x=>x with { Price=price(x.DistanceKm),Status=DeliveryStatus.Confirmed });
    }
    public static Result<DeliveryLedger,DomainError> Dispatch(DeliveryLedger ledger,string id)
        => Change(ledger,id,DeliveryStatus.Confirmed,DeliveryStatus.Dispatched,x=>x with { Status=DeliveryStatus.Dispatched });
    public static Result<DeliveryLedger,DomainError> Deliver(DeliveryLedger ledger,string id)
        => Change(ledger,id,DeliveryStatus.Dispatched,DeliveryStatus.Delivered,x=>x with { Status=DeliveryStatus.Delivered });
    public static Result<DeliveryLedger,DomainError> Cancel(DeliveryLedger ledger,string id)
    {
        var lookup=Find(ledger,id);
        return lookup.Bind(x=>x.Status switch
        {
            DeliveryStatus.Draft or DeliveryStatus.Confirmed => Replace(ledger,x with { Status=DeliveryStatus.Cancelled }),
            _=>new Result<DeliveryLedger,DomainError>.Err(new("INVALID_STATE","Only draft or confirmed orders can be cancelled."))
        });
    }
    public static Result<DeliveryLedger,DomainError> Reprice(DeliveryLedger ledger,string id,Func<decimal,decimal> price)
    {
        ArgumentNullException.ThrowIfNull(price);
        return Change(ledger,id,DeliveryStatus.Confirmed,DeliveryStatus.Confirmed,x=>x with {Price=price(x.DistanceKm)});
    }
    public static Maybe<DeliveryOrder> Lookup(DeliveryLedger ledger,string id)
        => ledger.Orders.FirstOrDefault(x=>string.Equals(x.Id,id,StringComparison.OrdinalIgnoreCase)) is { } order
           ? new Maybe<DeliveryOrder>.Some(order):new Maybe<DeliveryOrder>.None();
    private static Result<DeliveryOrder,DomainError> Find(DeliveryLedger ledger,string id)
        => Lookup(ledger,id) switch
        {
            Maybe<DeliveryOrder>.Some s => new Result<DeliveryOrder,DomainError>.Ok(s.Value),
            _ => new Result<DeliveryOrder,DomainError>.Err(new("ORDER_NOT_FOUND",$"Order '{id}' does not exist."))
        };
    private static Result<DeliveryLedger,DomainError> Change(DeliveryLedger ledger,string id,DeliveryStatus expected,DeliveryStatus next,Func<DeliveryOrder,DeliveryOrder> update)
        => Find(ledger,id).Bind(order=>order.Status==expected
           ? Replace(ledger,update(order))
           : new Result<DeliveryLedger,DomainError>.Err(new("INVALID_STATE",$"Expected {expected}, found {order.Status}; cannot transition to {next}.")));
    private static int FindIndex(ImmutableArray<DeliveryOrder> orders,string id)
    {for(int i=0;i<orders.Length;i++)if(string.Equals(orders[i].Id,id,StringComparison.OrdinalIgnoreCase))return i;return -1;}
    private static Result<DeliveryLedger,DomainError> Replace(DeliveryLedger ledger,DeliveryOrder order)
    {
        var index=FindIndex(ledger.Orders,order.Id);
        if(index<0)return new Result<DeliveryLedger,DomainError>.Err(new("ORDER_NOT_FOUND","Order does not exist."));
        return new Result<DeliveryLedger,DomainError>.Ok(ledger with {Orders=ledger.Orders.SetItem(index,order)});
    }
}
