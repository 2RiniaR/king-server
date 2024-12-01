﻿// <auto-generated />
using System;
using Approvers.King.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Approvers.King.Migrations
{
    [DbContext(typeof(AppService))]
    [Migration("20241201133806_CreateGacha")]
    partial class CreateGacha
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.10");

            modelBuilder.Entity("Approvers.King.Common.Gacha", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("HitProbabilityPermillage")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Gachas");
                });

            modelBuilder.Entity("Approvers.King.Common.GachaItem", b =>
                {
                    b.Property<Guid>("GachaId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RandomMessageId")
                        .HasColumnType("TEXT");

                    b.Property<int>("ProbabilityPermillage")
                        .HasColumnType("INTEGER");

                    b.HasKey("GachaId", "RandomMessageId");

                    b.ToTable("GachaItems");
                });

            modelBuilder.Entity("Approvers.King.Common.Slot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("ConditionPermillage")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Slots");
                });

            modelBuilder.Entity("Approvers.King.Common.User", b =>
                {
                    b.Property<ulong>("DiscordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MonthlyGachaPurchasePrice")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MonthlySlotProfitPrice")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TodaySlotExecuteCount")
                        .HasColumnType("INTEGER");

                    b.HasKey("DiscordId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Approvers.King.Common.GachaItem", b =>
                {
                    b.HasOne("Approvers.King.Common.Gacha", "Gacha")
                        .WithMany("GachaItems")
                        .HasForeignKey("GachaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Gacha");
                });

            modelBuilder.Entity("Approvers.King.Common.Gacha", b =>
                {
                    b.Navigation("GachaItems");
                });
#pragma warning restore 612, 618
        }
    }
}
