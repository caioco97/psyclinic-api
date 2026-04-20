namespace PsyClinic.Api.ViewModels.Auth
{
    public class ResponseUserViewModel
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Code { get; set; }
        public List<string> Errors { get; set; } = [];
        public string? UserName { get; set; }
        public string? Email { get; set; }
    }
}
