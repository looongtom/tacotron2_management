using Microsoft.EntityFrameworkCore;

namespace tacotron2_management.Models
{
    [Keyless]
    public class ModelDetails
    {
        public TblTrain Train { get; set; }
        public List<TblAudio> ListAudio { get; set; }
        public List<int>? ListCheckUntrained { get; set; }

        public TblModel Model { get; set; }
    }
}
