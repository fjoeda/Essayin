using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Essayin.Data
{
    public class QuizModel
    {
        [Required]
        public string Code { get; set; }
    }
}
