using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZCore.Entities;

namespace ZCore.Model
{
    public class MemberInfo : Entity<int>
    {
        [Required, MaxLength(50), DisplayName("会员帐号")]
        public string AccountId { get; set; }

        [MaxLength(50), DisplayName("会员名称")]
        public string MemberName { get; set; }
    }
}
