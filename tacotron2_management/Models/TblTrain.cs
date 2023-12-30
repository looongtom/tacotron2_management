using System;
using System.Collections.Generic;

namespace tacotron2_management.Models;

public partial class TblTrain
{
    public int Id { get; set; }

    public int? TblModelId { get; set; }

    public string Folder { get; set; }

    //public virtual TblModel TblModel { get; set; } = null!;

    public virtual ICollection<TblAudio> TblAudios { get; set; } = new List<TblAudio>();

    public virtual ICollection<TblTranscript> TblTranscripts { get; set; } = new List<TblTranscript>();
}
