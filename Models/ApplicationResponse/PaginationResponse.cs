namespace BookingHotel.Models.ApplicationResponse
{
    public class PaginationResponse<T>
    {
        public int Limit { get; set; }
        public int Page { get; set; }
        public int Total {  get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Data { get; set; } = new List<T>();

        public PaginationResponse(int limit, int page, int total, int totalPages, IEnumerable<T> data)
        {
            Limit = limit;
            Page = page;
            Total = total;
            TotalPages = totalPages;
            Data = data;
        }
    }
}
