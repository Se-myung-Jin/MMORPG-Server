using System.ComponentModel.DataAnnotations.Schema;

namespace AccountServer
{
    [Table("Account")]
    public class AccountDb
    {
        public int AccountDbId { get; set; }
        public string AccountName { get; set; }
        public string Password { get; set; }
    }
}
