using MailKit;
using qltc_ai.Models;
using qltc_ai.Models.AI;
using qltc_ai.Repositories;
using qltc_ai.Service.Base.AI;

namespace qltc_ai.Service.Base
{
    public class TransactionService : ITransactionService
    {
        private readonly ICategoryRepository _categoryRepo;
        private readonly IBudgetRepository _budgetRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly ICategoryService _categoryService;
        private readonly AIService _aiService;

        public TransactionService(ICategoryRepository categoryRepo, IBudgetRepository budgetRepo, ITransactionRepository transactionRepo, ICategoryService categoryService, AIService aIService)
        {
            _categoryRepo = categoryRepo;
            _budgetRepo = budgetRepo;
            _transactionRepo = transactionRepo;
            _categoryService = categoryService;
            _aiService = aIService;
        }

        public List<Giaodich> GetByAccount(int accId)
        {
            return _transactionRepo.GetByAccount(accId);
        }

        public List<Giaodich> GetByAccountAndMonth(int accId, int month, int year)
        {
            return _transactionRepo.GetByAccountAndMonth(accId, month, year);
        }

        public ServiceResult AddTransaction(int accId, int idDetail, decimal money, string note, string typeTran)
        {
            if (money <= 0)
                return new ServiceResult
                {
                    Success = false,
                    Message = "Số tiền không hợp lệ"
                };
            var now = DateTime.Now;
            ChiTietDanhMuc? detail;
            Ngansach? budget;

            if (typeTran == "ThuNhap")
            {

                budget = _budgetRepo.GetByMonth(accId, now.Month, now.Year);
                if (budget == null)
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Không tìm thấy ngân sách"
                    };


                detail = _categoryRepo.GetByBudgetT(budget.IdNganSach);
                if (detail == null)
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Không tìm thấy danh mục thu nhập"
                    };
            }
            else
            {

                detail = _categoryRepo.FindById(idDetail);
                if (detail == null)
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Danh mục không tồn tại"
                    };

                budget = _budgetRepo.FindById(detail.IdNganSach ?? 0);
                if (budget == null || budget.IdTaiKhoan != accId)
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Ngân sách không hợp lệ"
                    };

                if (budget.Thang?.Month != now.Month || budget.Thang?.Year != now.Year)
                    return new ServiceResult
                    {
                        Success = false,
                        Message = "Ngân sách không thuộc tháng hiện tại"
                    };
            }

            var cate = detail.IdDanhMucNavigation;
            if (cate == null)
                return new ServiceResult
                {
                    Success = false,
                    Message = "Không tìm thấy danh mục"
                };

            if (typeTran != cate.LoaiDanhMuc)
                return new ServiceResult
                {
                    Success = false,
                    Message = "Loại giao dịch không hợp lệ"
                };


            if (cate.LoaiDanhMuc == "ChiTieu")
            {
                decimal daTieu = detail.TienDaTieu ?? 0;
                decimal gioiHan = detail.GioiHanTien ?? 0;

                if (gioiHan <= 0)
                    return new ServiceResult
                    {
                        Success = false,
                        Message =
                    $"Danh mục {cate.TenDanhMuc} chưa có giới hạn"
                    };

                if (daTieu + money > gioiHan)
                {
                    decimal conLai = gioiHan - daTieu;
                    return new ServiceResult
                    {
                        Success = false,
                        Message =
                    $"Vượt giới hạn {cate.TenDanhMuc}. "
                    + $"Còn lại {conLai:N0}đ"
                    };
                }


                detail.TienDaTieu = daTieu + money;


            }
            else if (cate.LoaiDanhMuc == "ThuNhap")
            {
                budget.TongTien = (budget.TongTien ?? 0) + money;
                detail.TienDaTieu = (detail.TienDaTieu ?? 0) + money;
            }

            _categoryService.Rating(detail);

            var gd = new Giaodich
            {
                IdTaiKhoan = accId,
                IdChiTiet = detail.IdChiTiet,
                Tien = money,
                NoiDung = note,
                LoaiGiaoDich = typeTran,
                NgayGiaoDich = now
            };

            _transactionRepo.AddTransaction(gd);

            _categoryRepo.UpdateCategory(detail);
            _budgetRepo.UpdateBudget(budget);

            _transactionRepo.Save();
            _categoryRepo.Save();

            return new ServiceResult
            {
                Success = true,
                Message =
            $"Đã thêm giao dịch "
            + $"{cate.TenDanhMuc} "
            + $"{money:N0}đ"
            };
        }

        public bool UpdateTransaction(int accId, int idTran, decimal newMoney, string newNote)
        {
            if (newMoney <= 0)
                return false;

            var tran = _transactionRepo.FindById(idTran);
            if (tran == null || tran.IdTaiKhoan != accId)
                return false;

            var chiTiet = _categoryRepo.FindById(tran.IdChiTiet ?? 0);
            if (chiTiet == null)
                return false;


            if (tran.LoaiGiaoDich == "ChiTieu")
            {
                decimal diff = newMoney - (tran.Tien ?? 0);
                decimal newDaTieu = (chiTiet.TienDaTieu ?? 0) + diff;
                decimal gioiHan = chiTiet.GioiHanTien ?? 0;

                if (gioiHan > 0 && newDaTieu > gioiHan)
                    return false;

                chiTiet.TienDaTieu = newDaTieu;
                _categoryService.Rating(chiTiet);
                _categoryRepo.UpdateCategory(chiTiet);
            }

            tran.Tien = newMoney;
            tran.NoiDung = newNote;

            _transactionRepo.Update(tran);
            _transactionRepo.Save();
            _categoryRepo.Save();

            return true;
        }

        public bool DeleteTransaction(int idTran)
        {
            var tran = _transactionRepo.FindById(idTran);
            if (tran == null)
                return false;


            if (tran.LoaiGiaoDich == "ChiTieu" && tran.IdChiTiet.HasValue)
            {
                var chiTiet = _categoryRepo.FindById(tran.IdChiTiet.Value);
                if (chiTiet != null)
                {
                    chiTiet.TienDaTieu = Math.Max(0, (chiTiet.TienDaTieu ?? 0) - (tran.Tien ?? 0));
                    _categoryService.Rating(chiTiet);
                    _categoryRepo.UpdateCategory(chiTiet);
                    _categoryRepo.Save();
                }
            }

            _transactionRepo.DeleteTransaction(tran);
            _transactionRepo.Save();

            return true;
        }

        public ServiceResult AddByChat(int accId, string text, decimal money)
        {

            int idDanhMuc = _aiService.PredictCategory(text);

            var now = DateTime.Now;


            ChiTietDanhMuc? detail = _categoryRepo.GetByCategoryId(accId, idDanhMuc, now.Month, now.Year);

            if (detail == null)
                return new ServiceResult
                {
                    Success = false,
                    Message = "Không tìm thấy danh mục"
                };


            string? typeTran = detail.IdDanhMucNavigation.LoaiDanhMuc;


            var result = AddTransaction(accId, detail.IdChiTiet, money, text, typeTran);

            if (result.Success)
            {
                var path = "data/data.txt";
                var newLine = $"{text}|{idDanhMuc}";

                var exists = File.Exists(path) &&
                             File.ReadAllLines(path).Any(l => l.Trim() == newLine);

                if (!exists)
                    File.AppendAllText(path, Environment.NewLine + newLine);
            }

            return result;
        }
    }
}
