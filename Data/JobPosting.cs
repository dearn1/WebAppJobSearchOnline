using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppJobSearchOnline.Data
{
    [Table("JobPosting")]
    public class JobPosting
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Requirements { get; set; }
        public string Location { get; set; }
        public double SalaryMin { get; set; }
        public double SalaryMax { get; set; }
        public string JobType { get; set; }
        public string Status { get; set; }
        public DateTime StartPosting { get; set; }
        public DateTime EndPosting { get; set; }

    }
}
