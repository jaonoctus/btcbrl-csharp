namespace BTCBRL.Interfaces
{
    public interface IPriceAPI
    {
        Task<decimal> GetPriceAsync();
    }
}