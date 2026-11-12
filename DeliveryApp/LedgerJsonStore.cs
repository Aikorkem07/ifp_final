using System.Text.Json;
using System.Collections.Immutable;
namespace DeliveryApp;
public static class LedgerJsonStore
{
    private static readonly JsonSerializerOptions Options=new(){WriteIndented=true};
    public static Result<DeliveryLedger,DomainError> Load(string path)
    {
        try
        {
            using var stream=File.OpenRead(path);
            var rows=JsonSerializer.Deserialize<DeliveryOrder[]>(stream,Options);
            if(rows is null)return new Result<DeliveryLedger,DomainError>.Err(new("INVALID_JSON","Expected an array of orders."));
            var ids=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach(var row in rows)
            {
                if(row is null||string.IsNullOrWhiteSpace(row.Id)||row.DistanceKm<0||row.DistanceKm>20||row.Price<0||!Enum.IsDefined(row.Status)||!ids.Add(row.Id))
                    return new Result<DeliveryLedger,DomainError>.Err(new("INVALID_RECORD","Invalid or duplicate delivery order in file."));
                if(row.Status==DeliveryStatus.Draft && row.Price!=0)
                    return new Result<DeliveryLedger,DomainError>.Err(new("INVALID_RECORD","Draft order must have zero price."));
            }
            return new Result<DeliveryLedger,DomainError>.Ok(new(rows.ToImmutableArray()));
        }
        catch(FileNotFoundException ex){return Fail("FILE_NOT_FOUND",ex.Message);}
        catch(DirectoryNotFoundException ex){return Fail("DIRECTORY_NOT_FOUND",ex.Message);}
        catch(JsonException ex){return Fail("MALFORMED_JSON",ex.Message);}
        catch(IOException ex){return Fail("IO_ERROR",ex.Message);}
        catch(UnauthorizedAccessException ex){return Fail("ACCESS_DENIED",ex.Message);}
    }
    public static Result<string,DomainError> Save(string path,DeliveryLedger ledger)
    {
        try
        {
            var full=Path.GetFullPath(path);
            var dir=Path.GetDirectoryName(full)!;
            Directory.CreateDirectory(dir);
            var temp=full+"."+Guid.NewGuid().ToString("N")+".tmp";
            try
            {
                using(var stream=File.Create(temp))JsonSerializer.Serialize(stream,ledger.Orders.ToArray(),Options);
                File.Move(temp,full,true);
            }
            finally{if(File.Exists(temp))File.Delete(temp);}
            return new Result<string,DomainError>.Ok(full);
        }
        catch(IOException ex){return new Result<string,DomainError>.Err(new("IO_ERROR",ex.Message));}
        catch(UnauthorizedAccessException ex){return new Result<string,DomainError>.Err(new("ACCESS_DENIED",ex.Message));}
    }
    private static Result<DeliveryLedger,DomainError> Fail(string code,string msg)=>new Result<DeliveryLedger,DomainError>.Err(new(code,msg));
}
