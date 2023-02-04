using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Server
{
    [Table("Account")]
    public class AccountDb
    {
        public int AccountDbId { get; set; }
        public string AccountName { get; set; }
        public ICollection<PlayerDb> Players { get; set; }
    }

    [Table("Player")]
    public class PlayerDb
    {
        public int PlayerDbId { get; set; }
        public string PlayerName { get; set; }
        public AccountDb Account { get; set; }
    }
}
