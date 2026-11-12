using System.Runtime.InteropServices;
namespace DeliveryApp;
internal static class NativeMethods{
 [DllImport("delivery_native",EntryPoint="delivery_zone",CallingConvention=CallingConvention.Cdecl)] internal static extern int DeliveryZone(int distanceMeters);
}
public static class NativeZoneAdapter{
 public static Result<int,DomainError> GetZone(decimal distanceKm,Func<int,int>? nativeCall=null){
  if(distanceKm<0||distanceKm>200)return new Result<int,DomainError>.Err(new("NATIVE_INPUT_INVALID","Distance must be between 0 and 200 km."));
  try{int meters=decimal.ToInt32(decimal.Round(distanceKm*1000m,0,MidpointRounding.AwayFromZero));int zone=nativeCall is null?NativeMethods.DeliveryZone(meters):nativeCall(meters);if(zone is <1 or >3)return new Result<int,DomainError>.Err(new("NATIVE_RESULT_INVALID","Native library returned an unknown zone."));return new Result<int,DomainError>.Ok(zone);}
  catch(DllNotFoundException ex){return new Result<int,DomainError>.Err(new("NATIVE_LIBRARY_MISSING",ex.Message));}
  catch(EntryPointNotFoundException ex){return new Result<int,DomainError>.Err(new("NATIVE_ENTRY_MISSING",ex.Message));}
  catch(BadImageFormatException ex){return new Result<int,DomainError>.Err(new("NATIVE_ARCHITECTURE_ERROR",ex.Message));}
 }
}
