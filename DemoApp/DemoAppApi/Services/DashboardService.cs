using DemoAppApi.Data;
using DemoAppApi.Interfaces.Employee;
using Microsoft.EntityFrameworkCore;

namespace DemoAppApi.Services
{
    public class DashboardService : IDashboardService
    {
        #region Members

        private readonly AppDbContext _dbContext;
        private readonly ILogger<DashboardService> _logger;

        #endregion

        #region Constructor
        public DashboardService(AppDbContext dbContext, ILogger<DashboardService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        #endregion

        #region Public Methods
        public async Task<Notice> AddNoticeAsync(string title, string description, string username)
        {
            var notice = new Notice
            {
                Title = title,
                Description = description,
                CreatedOn = DateTime.Now,
                Status = "Active",
                Username = username
            };

            try
            {
                _dbContext.Notices.Add(notice);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Notice successfully saved to DB. Id: {NoticeId}", notice.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save notice to DB");
                throw;
            }

            return notice;
        }

        public async Task<bool> DeleteNoticeAsync(int noticeId)
        {
            try
            {
                var notice = await _dbContext.Notices.FindAsync(noticeId);
                if (notice == null)
                {
                    _logger.LogWarning("Delete failed: Notice with Id {NoticeId} not found", noticeId);
                    return false;
                }

                _dbContext.Notices.Remove(notice);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Notice with Id {NoticeId} deleted successfully", noticeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleting notice with Id {NoticeId}", noticeId);
                throw;
            }
        }


        public async Task<Notice?> UpdateNoticeAsync(int noticeId, string title, string description, string status)
        {
            try
            {
                var notice = await _dbContext.Notices.FindAsync(noticeId);
                if (notice == null)
                {
                    _logger.LogWarning("Update failed: Notice with Id {NoticeId} not found", noticeId);
                    return null;
                }

                notice.Title = title;
                notice.Description = description;
                notice.Status = status;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Notice with Id {NoticeId} updated successfully", noticeId);
                return notice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating notice with Id {NoticeId}", noticeId);
                throw;
            }
        }

        public async Task<List<Notice>> GetAllNoticesAsync(string username)
        {
            try
            {
                _logger.LogInformation("Retrieving all notices from database");
                return await _dbContext.Notices.Where(n => n.Username == username).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving all notices");
                throw;
            }
        }

        #endregion
    }
}