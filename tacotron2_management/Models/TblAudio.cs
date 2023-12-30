    using System;
using System.Collections.Generic;

namespace tacotron2_management.Models;

public partial class TblAudio
{
    public int Id { get; set; }

    public int? TblTranscriptId { get; set; }

    public string? AudioName { get; set; }

    public float Duration { get; set; }

    public float Size { get; set; }

    public bool IsTrained { get; set; }

    //public double? MosScore { get; set; }

    public string? URL { get; set; }


    public virtual TblTranscript TblTranscript { get; set; } = null!;

    public virtual ICollection<ExpertListener> ExpertListeners { get; set; } = new List<ExpertListener>();

    public virtual ICollection<TblTrain> TblTrains { get; set; } = new List<TblTrain>();
}
