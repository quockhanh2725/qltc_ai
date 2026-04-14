using qltc_ai.Models;

namespace qltc_ai.Service.Base
{
    public interface ICategoryService
    {
        List<Danhmuc> GetAllCatrgory(int id);
        (bool success, decimal thieu) UpdateLimit(int accId, int idCate, decimal newLimit);
    }
}
