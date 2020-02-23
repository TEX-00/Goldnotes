using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Goldnote.Models
{
    public class GoldNote
    {
        
        public int Id { get; set; }
        [DisplayName("金券名")]
        [Required(ErrorMessage ="金券名は必須です")]
        public string GoldNoteName { get; set; }
        [DisplayName("お釣り")]
        public bool Change { get; set; }
        [DisplayName("割引併用")]
         public bool WithDiscount { get; set; }

        [DisplayName("送り先")]
        public bool Destination { get; set; }
        [DisplayName("会計時")]
        public string OnAccounting { get; set; }
        [DisplayName("信計機")]
        public string CreditCardMachine { get; set; }
        [DisplayName("金券送付表")]
        public string GoldNoteSendingPaper { get; set; }
        [DisplayName("特記事項")]
        public string SpecialOptions { get; set; }
        public string ImageAdress { get; set; }
        [DataType(DataType.Date)] 
       
        [DisplayName("編集日")]
        public DateTime EditDate { get;set; }

        [DisplayName("編集者")]
        public string EditerId { get; set; }


    }
}
