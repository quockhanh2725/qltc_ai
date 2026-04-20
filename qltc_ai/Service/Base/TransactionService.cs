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
        private readonly ICategoryService _categoryService;
        public TransactionService(ICategoryRepository categoryRepo, IBudgetRepository budgetRepo, ITransactionRepository transactionRepo, ICategoryService categoryService)
        {
            _categoryRepo = categoryRepo;
            _budgetRepo = budgetRepo;
            _transactionRepo = transactionRepo;
            _categoryService = categoryService;
        }
        public bool AddTransaction(int accid, int idCate, decimal money, string note)
        {
            var now = DateTime.Now;

            var cate = _categoryRepo.FindById(idCate);
            if (cate == null)
                return false;

            
            var budget = _budgetRepo.FindById(cate.IdNganSach);
            if (budget == null)
                return false;

            if (budget.IdTaiKhoan != accid)
                return false;

            if (budget.Thang?.Month != now.Month || budget.Thang?.Year != now.Year)
                return false;

            decimal daTieu = cate.TienDaTieu ?? 0;
            decimal gioiHan = cate.GioiHanTien ?? 0;

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

            cate.TienDaTieu = (cate.TienDaTieu ?? 0) + money;
            _categoryRepo.UpdateCategory(cate);
            _categoryService.Rating(cate);

            _transactionRepo.Save();
            _categoryRepo.Save();

            return true;
        }

        public bool DeleteTransaction(int idTran)
        {
            var tran = _transactionRepo.FindById(idTran);
            if (tran == null )
                return false;

            var cate = _categoryRepo.FindById(tran.IdDanhMuc.Value);
            if (cate == null)
                return false;

            cate.TienDaTieu = (cate.TienDaTieu ?? 0) - tran.Tien;

            _categoryRepo.UpdateCategory(cate);
            _transactionRepo.DeleteTransaction(tran);
            _categoryService.Rating(cate);

            _categoryRepo.Save();
            _transactionRepo.Save();

            return true;
        }

        public bool UpdateTransaction(int accid , int idTran , decimal newMoney , string newNote)
        {
            var tran = _transactionRepo.FindById(idTran);
            if (tran == null || tran.IdTaiKhoan != accid)
            return false;

            var cate = _categoryRepo.FindById(tran.IdDanhMuc.Value);
            if(cate == null)
            return false;

            var diff = newMoney - tran.Tien;

            if ((cate.TienDaTieu ?? 0) + diff > cate.GioiHanTien)
            return false;

            cate.TienDaTieu = (cate.TienDaTieu ?? 0) + diff;

            tran.Tien = newMoney;
            tran.NoiDung = newNote;

            
            _categoryRepo.UpdateCategory(cate);
            _transactionRepo.Update(tran);
            _categoryService.Rating(cate);

            _categoryRepo.Save();
            _transactionRepo.Save();

            return true;

        }

    }
}
