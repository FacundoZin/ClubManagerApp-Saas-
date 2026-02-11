namespace APIClub.Infrastructure.Interfaces
{
    public interface IDataSeeder
    {
        Task seedTestDataAsync();
        Task seedSociosExistentes();
    }
}
