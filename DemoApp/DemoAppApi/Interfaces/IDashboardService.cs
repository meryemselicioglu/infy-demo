namespace DemoAppApi.Interfaces.Employee
{
    public interface IDashboardService
    {
        Task<Notice> AddNoticeAsync(string title, string description, string username);

        Task<bool> DeleteNoticeAsync(int noticeId);

        Task<Notice?> UpdateNoticeAsync(int noticeId, string title, string description, string status);

        Task<List<Notice>> GetAllNoticesAsync(string username);
    }
}