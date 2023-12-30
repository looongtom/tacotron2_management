using Microsoft.EntityFrameworkCore;

namespace tacotron2_management.Models
{
    [Keyless]
    public class TrainDetailsViewModel
    {
        public TblTrain Train { get; set; }
        public List<TblAudio> ListAudio { get; set; }
        public List<TblTranscript> ListTranscript { get; set; }
        public List<int>? ListCheckUntrained { get; set; }

    }
}
