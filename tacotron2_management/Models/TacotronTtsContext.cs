using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace tacotron2_management.Models;

public partial class TacotronTtsContext : DbContext
{
    public TacotronTtsContext()
    {
    }

    public TacotronTtsContext(DbContextOptions<TacotronTtsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ExpertListener> ExpertListeners { get; set; }

    public virtual DbSet<TblAudio> TblAudios { get; set; }

    public virtual DbSet<TblModel> TblModels { get; set; }

    public virtual DbSet<TblTrain> TblTrains { get; set; }

    public virtual DbSet<TblTranscript> TblTranscripts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-7BMV341\\CSDLPTTEST;Initial Catalog=tacotron_tts;Persist Security Info=True;User ID=sa;Password=88888888;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExpertListener>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ExpertLi__3214EC079E02A927");

            entity.ToTable("ExpertListener");

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasMany(d => d.TblAudios).WithMany(p => p.ExpertListeners)
                .UsingEntity<Dictionary<string, object>>(
                    "ExpertListenerTblAudio",
                    r => r.HasOne<TblAudio>().WithMany()
                        .HasForeignKey("TblAudioId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKExpertList893930"),
                    l => l.HasOne<ExpertListener>().WithMany()
                        .HasForeignKey("ExpertListenerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FKExpertList312790"),
                    j =>
                    {
                        j.HasKey("ExpertListenerId", "TblAudioId").HasName("PK__ExpertLi__C6213E4C81B9038C");
                        j.ToTable("ExpertListener_tblAudio");
                        j.IndexerProperty<int>("TblAudioId").HasColumnName("tblAudioId");
                    });
        });

        modelBuilder.Entity<TblAudio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblAudio__3214EC079E98DD5F");

            entity.ToTable("tblAudio");

            entity.Property(e => e.AudioName)
                .HasMaxLength(255)
                .IsUnicode(false);
            //entity.Property(e => e.MosScore).HasColumnName("MOS_score");
            entity.Property(e => e.TblTranscriptId).HasColumnName("tblTranscriptId");

            entity.HasOne(d => d.TblTranscript).WithMany(p => p.TblAudios)
                .HasForeignKey(d => d.TblTranscriptId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKtblAudio739313");

            //entity.HasMany(d => d.TblTrains).WithMany(p => p.TblAudios)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "TblAudioTblTrain",
            //        r => r.HasOne<TblTrain>().WithMany()
            //            .HasForeignKey("TblTrainId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FKtblAudio_t373538"),
            //        l => l.HasOne<TblAudio>().WithMany()
            //            .HasForeignKey("TblAudioId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FKtblAudio_t869569"),
            //        j =>
            //        {
            //            j.HasKey("TblAudioId", "TblTrainId").HasName("PK__tblAudio__166BDDB9F9283F15");
            //            j.ToTable("tblAudio_tblTrain");
            //            j.IndexerProperty<int>("TblAudioId").HasColumnName("tblAudioId");
            //            j.IndexerProperty<int>("TblTrainId").HasColumnName("tblTrainId");
            //        });
        });

        modelBuilder.Entity<TblModel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblModel__3214EC07CA88824B");

            entity.ToTable("tblModel");

            entity.Property(e => e.MosAverage).HasColumnName("MOS_average");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TrainDate).HasColumnType("date");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TblTrain>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblTrain__3214EC072B53610C");

            entity.ToTable("tblTrain");

            entity.Property(e => e.Folder)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TblModelId).HasColumnName("tblModelId");

            //entity.HasOne(d => d.TblModel).WithMany(p => p.TblTrains)
            //    .HasForeignKey(d => d.TblModelId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FKtblTrain608403");
        });

        modelBuilder.Entity<TblTranscript>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblTrans__3214EC07776156F5");

            entity.ToTable("tblTranscript");

            entity.Property(e => e.Content)
                .HasMaxLength(255)
                .IsUnicode(false);

            //entity.HasMany(d => d.TblTrains).WithMany(p => p.TblTranscripts)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "TblTranscriptTblTrain",
            //        r => r.HasOne<TblTrain>().WithMany()
            //            .HasForeignKey("TblTrainId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FKtblTranscr764913"),
            //        l => l.HasOne<TblTranscript>().WithMany()
            //            .HasForeignKey("TblTranscriptId")
            //            .OnDelete(DeleteBehavior.ClientSetNull)
            //            .HasConstraintName("FKtblTranscr477861"),
            //        j =>
            //        {
            //            j.HasKey("TblTranscriptId", "TblTrainId").HasName("PK__tblTrans__B1616452AC8E15E7");
            //            j.ToTable("tblTranscript_tblTrain");
            //            j.IndexerProperty<int>("TblTranscriptId").HasColumnName("tblTranscriptId");
            //            j.IndexerProperty<int>("TblTrainId").HasColumnName("tblTrainId");
            //        });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
