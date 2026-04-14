using qltc_ai.Models;
using qltc_ai.Repositories;
using System.Text;

namespace qltc_ai.Service.Base
{
    public class TransactionService : ITransactionService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IBudgetRepository _budgetRepo;
        private readonly ITransactionRepository _transactionRepo;
        public TransactionService(ICategoryRepository categoryRepo, IBudgetRepository budgetRepo, ITransactionRepository transactionRepo)
        {
            _categoryRepo = categoryRepo;
            _budgetRepo = budgetRepo;
            _transactionRepo = transactionRepo;
        }
        public bool AddTransaction(int accid, int idCate, decimal money, string note)
        {
            var now = DateTime.Now;

            var category = _categoryRepo.GetCategoryById(idCate);
            if (category == null)
                return false;

            
            var budget = _budgetRepo.FindById(category.IdNganSach);
            if (budget == null)
                return false;

            if (budget.IdTaiKhoan != accid)
                return false;

            if (budget.Thang?.Month != now.Month || budget.Thang?.Year != now.Year)
                return false;

            decimal daTieu = category.TienDaTieu ?? 0;
            decimal gioiHan = category.GioiHanTien ?? 0;

            if (daTieu + money > gioiHan)
                return false;

            var gd = new Giaodich
            {
                IdTaiKhoan = accid,
                IdDanhMuc = idCate,
                Tien = money,
                NoiDung = note,
                NgayGiaoDich = now
            };

            _transactionRepo.AddTransaction(gd);

            category.TienDaTieu = (category.TienDaTieu ?? 0) + money;
            _categoryRepo.UpdateCategory(category);

            _transactionRepo.Save();
            _categoryRepo.Save();

            return true;
        }
    }
}
