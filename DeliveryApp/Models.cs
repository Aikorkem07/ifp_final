using System.Collections.Immutable;
namespace DeliveryApp;

public abstract record Result<T,E>{private Result(){} public sealed record Ok(T Value):Result<T,E>; public sealed record Err(E Error):Result<T,E>;
 public Result<U,E> Map<U>(Func<T,U> f)=>this switch{Ok x=>new Result<U,E>.Ok(f(x.Value)),Err e=>new Result<U,E>.Err(e.Error),_=>throw new InvalidOperationException()};
 public Result<U,E> Bind<U>(Func<T,Result<U,E>> f)=>this switch{Ok x=>f(x.Value),Err e=>new Result<U,E>.Err(e.Error),_=>throw new InvalidOperationException()}; }
public sealed record DomainError(string Code,string Message){public override string ToString()=>$"{Code}: {Message}";}
public abstract record Maybe<T>{private Maybe(){} public sealed record Some(T Value):Maybe<T>; public sealed record None:Maybe<T>;}
public sealed record DeliveryRequest(string Id,decimal DistanceKm);
public sealed record ValidatedDeliveryRequest{public string Id{get;} public decimal DistanceKm{get;} private ValidatedDeliveryRequest(string id,decimal km){Id=id;DistanceKm=km;}
 public static Result<ValidatedDeliveryRequest,DomainError> Create(string? id,decimal km){if(string.IsNullOrWhiteSpace(id))return new Result<ValidatedDeliveryRequest,DomainError>.Err(new("INVALID_ID","Delivery ID cannot be empty."));if(km<0)return new Result<ValidatedDeliveryRequest,DomainError>.Err(new("NEGATIVE_DISTANCE","Distance cannot be negative."));if(km>200) return new Result<ValidatedDeliveryRequest,DomainError>.Err(new("DISTANCE_TOO_LARGE","Distance cannot exceed 200 km."));return new Result<ValidatedDeliveryRequest,DomainError>.Ok(new(id.Trim(),km));}}
public abstract record DeliveryWorkflowState{private DeliveryWorkflowState(){} public sealed record Ready(ImmutableArray<ValidatedDeliveryRequest> Requests):DeliveryWorkflowState;public sealed record Completed(DeliverySummary Summary):DeliveryWorkflowState;public sealed record Failed(string Reason):DeliveryWorkflowState;}
public sealed record DeliveryQuote(string Id,decimal DistanceKm,decimal Cost,int Zone);
public sealed record DeliverySummary(ImmutableArray<DeliveryQuote> Quotes,int AcceptedCount,int RejectedCount,decimal TotalCost);
public sealed record QuoteDisplay(string DeliveryId,string Distance,string Cost);
public sealed record CategoryTotal(int Zone,int Count,decimal TotalCost);
public sealed record DeliveryReportRow(string Id,decimal DistanceKm,decimal Cost,int Zone);
