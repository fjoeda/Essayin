using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Essayin.Data
{
    public class QuestionItemModel
    {
        [Required]
        public string Soal { get; set; }
        [Required]
        public string Jawaban { get; set; }
    }
}
