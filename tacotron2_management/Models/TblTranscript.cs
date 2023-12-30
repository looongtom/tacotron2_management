using System;
using System.Collections.Generic;

namespace tacotron2_management.Models;

public partial class TblTranscript
{
    public int Id { get; set; }

    public string? Content { get; set; }

    public bool IsTrained { get; set; }

    public virtual ICollection<TblAudio> TblAudios { get; set; } = new List<TblAudio>();

    public virtual ICollection<TblTrain> TblTrains { get; set; } = new List<TblTrain>();
}
