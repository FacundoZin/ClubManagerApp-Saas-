namespace APIClub.Domain.Common
{
    public interface IDataSeeder
    {
        Task seedTestDataAsync();
        Task seedSociosExistentes();
    }
}
