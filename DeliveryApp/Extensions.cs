using System.Globalization;

namespace DeliveryApp;

public static class DeliveryExtensions
{
    public static IEnumerable<ValidatedDeliveryRequest> WithinServiceRange(
        this IEnumerable<ValidatedDeliveryRequest> source,
        decimal maxKm)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (maxKm < 0)
            throw new ArgumentOutOfRangeException(nameof(maxKm));

        return source.Where(x => x.DistanceKm <= maxKm);
    }

    public static string ToKzt(this decimal amount)
        => $"{amount.ToString("0.00", CultureInfo.InvariantCulture)} KZT";
}
