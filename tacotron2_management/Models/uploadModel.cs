using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace tacotron2_management.Models
{
    public class SingleFileModel : ReponseModel
    {
        //[Required(ErrorMessage = "Please select file")]
        public IFormFile? File { get; set; }

        //[Required(ErrorMessage = "Please select a transcript")]
        public int TranscriptID { get; set; }

        //[Required(ErrorMessage = "Please select a audio")]
        public int AudioID { get; set; }
    }

    public class ReponseModel
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsResponse { get; set; }
    }


    public class MultipleFilesModel : ReponseModel
    {

        [Required(ErrorMessage = "Please select files")]
        public List<IFormFile> Files { get; set; }

    }
}
