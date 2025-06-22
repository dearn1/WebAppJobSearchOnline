using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppJobSearchOnline.Data
{
    [Table("JobApplication")]
    public class JobApplication
    {
        public int Id { get; set; }
        [Required]
        public int JobPostingId { get; set; }
        public string UserId { get; set; }
        public string Status {  get; set; }
        public DateTime AppliedDate { get; set; }
    }
}
