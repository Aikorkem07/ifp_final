using System.Collections.Immutable;using DeliveryApp;
var tests=new (string,Action)[]{
("Final register and confirm workflow",()=>{var x=Registered();var r=DeliveryOperations.Confirm(x,"A",PricingPolicies.Standard);Assert(r is Result<DeliveryLedger,DomainError>.Ok v&&v.Value.Orders[0].Price==700m);}),
("Final dispatch and deliver workflow",()=>{var l=Confirmed();var d=DeliveryOperations.Dispatch(l,"A");Assert(d is Result<DeliveryLedger,DomainError>.Ok);var delivered=DeliveryOperations.Deliver(((Result<DeliveryLedger,DomainError>.Ok)d).Value,"A");Assert(delivered is Result<DeliveryLedger,DomainError>.Ok x&&x.Value.Orders[0].Status==DeliveryStatus.Delivered);}),
("Final cancel draft",()=>Assert(DeliveryOperations.Cancel(Registered(),"A") is Result<DeliveryLedger,DomainError>.Ok)),
("Final reject dispatch before confirmation",()=>Assert(DeliveryOperations.Dispatch(Registered(),"A") is Result<DeliveryLedger,DomainError>.Err)),
("Final reject duplicate ID",()=>Assert(DeliveryOperations.Register(Registered(),"a",2) is Result<DeliveryLedger,DomainError>.Err)),
("Final reject out-of-range order",()=>Assert(DeliveryOperations.Register(DeliveryLedger.Empty,"Z",21) is Result<DeliveryLedger,DomainError>.Err)),
("Final reject cancel after delivery",()=>{var d=DeliveryOperations.Dispatch(Confirmed(),"A");var l=((Result<DeliveryLedger,DomainError>.Ok)d).Value;var delivered=DeliveryOperations.Deliver(l,"A");Assert(DeliveryOperations.Cancel(((Result<DeliveryLedger,DomainError>.Ok)delivered).Value,"A") is Result<DeliveryLedger,DomainError>.Err);}),
("Final reprice changes confirmed quote",()=>{var r=DeliveryOperations.Reprice(Confirmed(),"A",PricingPolicies.Express);Assert(r is Result<DeliveryLedger,DomainError>.Ok x&&x.Value.Orders[0].Price==1100m);}),
("Final reports group and filter",()=>{var x=Confirmed();Assert(DeliveryLedgerReports.ActiveOrders(x).Length==1&&DeliveryLedgerReports.ByStatus(x)[0].Count==1);}),
("Final file missing",()=>Assert(LedgerJsonStore.Load(Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".json")) is Result<DeliveryLedger,DomainError>.Err)),
("Final malformed file",()=>{var f=Path.GetTempFileName();try{File.WriteAllText(f,"[{oops]");Assert(LedgerJsonStore.Load(f) is Result<DeliveryLedger,DomainError>.Err);}finally{File.Delete(f);}}),
("Final save and reload",()=>{var f=Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".json");try{Assert(LedgerJsonStore.Save(f,Confirmed()) is Result<string,DomainError>.Ok);Assert(LedgerJsonStore.Load(f) is Result<DeliveryLedger,DomainError>.Ok x&&x.Value.Orders.Length==1);}finally{if(File.Exists(f))File.Delete(f);}}),
("Factory accepts valid zero distance",()=>Assert(Valid("Z",0).DistanceKm==0)),
("Factory trims ID",()=>Assert(Valid(" A ",2).Id=="A")),
("Factory rejects blank ID",()=>Assert(ValidatedDeliveryRequest.Create(" ",1) is Result<ValidatedDeliveryRequest,DomainError>.Err)),
("Factory rejects negative distance",()=>Assert(ValidatedDeliveryRequest.Create("A",-1) is Result<ValidatedDeliveryRequest,DomainError>.Err)),
("Factory rejects over 200 km",()=>Assert(ValidatedDeliveryRequest.Create("A",201) is Result<ValidatedDeliveryRequest,DomainError>.Err)),
("WithinServiceRange filters",()=>Assert(Data().WithinServiceRange(5).Count()==3)),
("ToKzt formats money",()=>Assert(12.5m.ToKzt()=="12.50 KZT")),
("Standard and express currying differ",()=>Assert(CurriedPricing.CreateScore(500m)(100m)(5m)!=CurriedPricing.CreateScore(800m)(150m)(5m))),("LINQ report deterministic tie order",()=>{var r=DeliveryPipeline.DetailedReport(Data(),_=>100);Assert(r[0].Id=="A"&&r[1].Id=="B");}),
("LINQ zone grouping counts",()=>Assert(DeliveryPipeline.ZoneReport(Data(),PricingPolicies.Standard).Sum(x=>x.Count)==3)),
("Empty report returns no rows",()=>Assert(DeliveryPipeline.DetailedReport(ImmutableArray<ValidatedDeliveryRequest>.Empty,PricingPolicies.Standard).Length==0)),
("No matching records returns empty",()=>Assert(Data().WithinServiceRange(0).Count()==0)),
("Deferred stream does not process before iteration",()=>{int n=0;var q=DeliveryPipeline.StreamEligible(Data(),()=>n++);Assert(n==0);}),
("Deferred stream processes during iteration",()=>{int n=0;var q=DeliveryPipeline.StreamEligible(Data(),()=>n++).ToArray();Assert(n==3&&q.Length==3);}),
("Materialised snapshot can be reused",()=>{int n=0;var a=DeliveryPipeline.StreamEligible(Data(),()=>n++).ToArray();var b=a.ToArray();Assert(n==3&&b.Length==3);}),
("Native zone real call short distance",()=>Assert(NativeZoneAdapter.GetZone(2m) is Result<int,DomainError>.Ok z&&z.Value==1)),
("Native adapter rejects invalid input",()=>Assert(NativeZoneAdapter.GetZone(201m) is Result<int,DomainError>.Err)),
("Native adapter translates injected load failure",()=>Assert(NativeZoneAdapter.GetZone(2m,_=>throw new DllNotFoundException("test")) is Result<int,DomainError>.Err)),
("Workflow produces accepted and rejected counts",()=>{var s=DeliveryPipeline.Complete(DeliveryPipeline.Start(Data()),PricingPolicies.Standard);Assert(s is DeliveryWorkflowState.Completed c&&c.Summary.AcceptedCount==3&&c.Summary.RejectedCount==1);}),
("Workflow rejects repeated completion",()=>{var first=DeliveryPipeline.Complete(DeliveryPipeline.Start(Data()),PricingPolicies.Standard);var second=DeliveryPipeline.Complete(first,PricingPolicies.Standard);Assert(second is DeliveryWorkflowState.Failed);}),
("Parser rejects invalid domain record without partial data",()=>Assert(InputParser.Parse("[{\"id\":\"OK\",\"distanceKm\":1},{\"id\":\"BAD\",\"distanceKm\":-1}]") is Result<ImmutableArray<ValidatedDeliveryRequest>,DomainError>.Err)),
("Missing file is safe failure",()=>{var p=Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".json");Assert(DeliveryFileBoundary.Load(p) is Result<ImmutableArray<ValidatedDeliveryRequest>,DomainError>.Err);}),
("Malformed file is safe and can be deleted after read",()=>{var p=Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".json");try{File.WriteAllText(p,"[{]");Assert(DeliveryFileBoundary.Load(p) is Result<ImmutableArray<ValidatedDeliveryRequest>,DomainError>.Err);}finally{if(File.Exists(p))File.Delete(p);}Assert(!File.Exists(p));}),
("Successful file load releases file handle",()=>{var p=Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".json");try{File.WriteAllText(p,"[{\"id\":\"A\",\"distanceKm\":1}]");Assert(DeliveryFileBoundary.Load(p) is Result<ImmutableArray<ValidatedDeliveryRequest>,DomainError>.Ok);}finally{if(File.Exists(p))File.Delete(p);}Assert(!File.Exists(p));}),
("Malformed JSON parser boundary",()=>{var p=Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".json");try{File.WriteAllText(p,"[{]");var r=DeliveryFileBoundary.Load(p);Assert(r is Result<ImmutableArray<ValidatedDeliveryRequest>,DomainError>.Err e&&e.Error.Code=="MALFORMED_JSON");}finally{if(File.Exists(p))File.Delete(p);}})
};
int passed=0;foreach(var t in tests){try{t.Item2();Console.WriteLine("PASS "+t.Item1);passed++;}catch(Exception e){Console.WriteLine("FAIL "+t.Item1+": "+e.Message);}}Console.WriteLine($"Passed: {passed}/{tests.Length}");if(passed!=tests.Length)Environment.ExitCode=1;
static void Assert(bool ok){if(!ok)throw new Exception("Assertion failed");}
static ValidatedDeliveryRequest Valid(string id,decimal km){var r=ValidatedDeliveryRequest.Create(id,km);return ((Result<ValidatedDeliveryRequest,DomainError>.Ok)r).Value;}
static ImmutableArray<ValidatedDeliveryRequest> Data()=>ImmutableArray.Create(Valid("B",2),Valid("A",2),Valid("C",5),Valid("FAR",25));
static DeliveryLedger Registered()=>((Result<DeliveryLedger,DomainError>.Ok)DeliveryOperations.Register(DeliveryLedger.Empty,"A",2)).Value;
static DeliveryLedger Confirmed()=>((Result<DeliveryLedger,DomainError>.Ok)DeliveryOperations.Confirm(Registered(),"A",PricingPolicies.Standard)).Value;
