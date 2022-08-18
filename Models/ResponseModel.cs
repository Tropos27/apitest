namespace Launcher.Models
{
    public class ResponseModel<T>
    {
        public bool Error { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }
    }
}
