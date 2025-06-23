namespace WebAppJobSearchOnline.Constants
{
    public enum Roles
    {
        Admin,
        User
    }

    public static class JobStatus
    {
        public const string Open = "OPEN";
        public const string Closed = "CLOSED";
    }

    public static class JobType
    {
        public const string FullTime = "FULL_TIME";
        public const string PartTime = "PART_TIME";
        public const string Contract = "CONTRACT";
        public const string Internship = "INTERNSHIP";
        public const string Casual = "CASUAL";
    }

    public static class ApplicationStatus
    {
        public const string Applied = "APPLIED";
        public const string Interviewing = "INTERVIEWING";
        public const string Accepted = "ACCEPTED";
        public const string Rejected = "REJECTED";
        public const string Withdrawn = "WITHDRAWN";
    }

    public class MyConstants
    {
    }
}
