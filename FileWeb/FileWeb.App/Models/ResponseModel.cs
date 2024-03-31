namespace FileWeb.App.Models
{
    public class ResponseModel
    {
        public int Code { get; set; }

        public string Message { get; set; }
    }

    public class ResponseModel<T> : ResponseModel
    {
        public T Data { get; set; }
    }
}
