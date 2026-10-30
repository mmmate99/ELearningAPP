    public class EnrollmentDto
    {
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal? Rating { get; set; }
        public string? Review { get; set; }
    }