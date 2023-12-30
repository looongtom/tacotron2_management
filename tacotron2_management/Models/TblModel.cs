using System;
using System.Collections.Generic;

namespace tacotron2_management.Models;

public partial class TblModel
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Url { get; set; }

    public DateTime? TrainDate { get; set; }

    public double? MosAverage { get; set; }

    public string? Status { get; set; }

    //public int TblTrainId { get; set; }

    public virtual ICollection<TblTrain> TblTrains { get; set; } = new List<TblTrain>();
}
