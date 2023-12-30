using System;
using System.Collections.Generic;

namespace tacotron2_management.Models;

public partial class ExpertListener
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<TblAudio> TblAudios { get; set; } = new List<TblAudio>();
}
